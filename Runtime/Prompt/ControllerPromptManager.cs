using System;
using System.Collections.Generic;
using UnityEngine;


namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// Manages the controller prompts that appear in the game UI.
    /// </summary>
    public class ControllerPromptManager
    {
        private class ActionInfo
        {
            public string Token = string.Empty;
            public readonly List<ControllerPrompt> Prompts = new List<ControllerPrompt>(1);
        }

        private static ControllerPromptManager s_instance;
        public static ControllerPromptManager Instance => s_instance ?? (s_instance = new ControllerPromptManager());

        private IControllerTokenProvider _tokenProvider;
        private IControllerActionRelay _actionRelay;

        private readonly Stack<string> _layerStack = new Stack<string>();
        
        private string _activeLayer = string.Empty;
        private HashSet<string> _activeActions = new HashSet<string>();
        
        private readonly Dictionary<string, ActionInfo> _actionPrompts = new Dictionary<string, ActionInfo>();
        

        #region Public Interface
        public void PushLayer(string layer)
        {
            // Deal with old active layer.
            if (_activeLayer != string.Empty)
            {
                _layerStack.Push(_activeLayer);
            }

            // Prepare new active layer.
            _activeLayer = layer;
        }

        public void PopLayer(string layer)
        {
            if (_activeLayer != layer) return; // Do we want to dig this out of the stack?
            
            // Deal with old active layer.
            _activeLayer = string.Empty;
            
            // Prepare new active layer.
            if (_layerStack.Count > 0)
            {
                _activeLayer = _layerStack.Pop();
            }
        }

        public void RegisterControllerPrompt(ControllerPrompt prompt)
        {
            // Add prompt to control list.
            ActionInfo controlInfo;
            if (!_actionPrompts.TryGetValue(prompt.ActionName, out controlInfo))
            {
                controlInfo = new ActionInfo
                {
                    Token = _tokenProvider?.LookupToken(prompt.ActionName)
                };
                _actionPrompts.Add(prompt.ActionName, controlInfo);
                _actionRelay?.RegisterInterestInAction(prompt.ActionName);
            }
            controlInfo.Prompts.Add(prompt);
            prompt.SetToken(controlInfo.Token);
		}

        public void UnregisterControllerPrompt(ControllerPrompt prompt)
        {
            // Remove prompt from control list.
            if (_actionPrompts.TryGetValue(prompt.ActionName, out ActionInfo controlInfo))
            {
                controlInfo.Prompts.Remove(prompt);
                if (controlInfo.Prompts.Count == 0)
                {
                    _actionPrompts.Remove(prompt.ActionName);
                    _actionRelay?.UnregisterInterestInAction(prompt.ActionName);
                }
            }
        }

        public ICollection<string> GetWatchedActions()
        {
            return _actionPrompts.Keys;
        }

        public void UpdatePromptToken(string actionName, string token)
        {
            if (_actionPrompts.TryGetValue(actionName, out ActionInfo controlInfo))
            {
                for (int i = 0; i < controlInfo.Prompts.Count; i++)
                {
                    controlInfo.Prompts[i].SetToken(token);
                }
            }
        }

        public void PressControl(string actionName)
        {
            if (!_actionPrompts.TryGetValue(actionName, out ActionInfo controlInfo)) return;

            _activeActions.Add(actionName);
            for (int i = 0; i < controlInfo.Prompts.Count; i++)
            {
                ControllerPrompt prompt = controlInfo.Prompts[i];
                if ((prompt.Layer != _activeLayer) || !prompt.gameObject.activeInHierarchy) continue;
                prompt.PressControl();
                return;
            }
        }

        public void ReleaseControl(string actionName)
        {
            if (!_actionPrompts.TryGetValue(actionName, out ActionInfo controlInfo)) return;
            if (!_activeActions.Contains(actionName)) return;
            _activeActions.Remove(actionName);
            for (int i = 0; i < controlInfo.Prompts.Count; i++)
            {
                ControllerPrompt prompt = controlInfo.Prompts[i];
                if ((prompt.Layer != _activeLayer) || !prompt.gameObject.activeInHierarchy) continue;
                prompt.ReleaseControl();
                return;
            }
        }

        public void SetTokenProvider(IControllerTokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
            RefreshTokens();
        }

        public void ClearTokenProvider(IControllerTokenProvider tokenProvider)
        {
            if (_tokenProvider == tokenProvider)
            {
                _tokenProvider = null;
            }
        }

        public void SetActionRelay(IControllerActionRelay actionRelay)
        {
            _actionRelay = actionRelay;
            // Register existing actions.
            foreach (string action in _actionPrompts.Keys)
            {
                _actionRelay.RegisterInterestInAction(action);
            }
        }

        public void ClearActionRelay(IControllerActionRelay actionRelay)
        {
            if (_actionRelay == actionRelay)
            {
                _actionRelay = null;
            }
        }

        public void RefreshTokens()
        {
            if (_tokenProvider == null) return;
            
            foreach (KeyValuePair<string, ActionInfo> entry in _actionPrompts)
            {
                string token = _tokenProvider.LookupToken(entry.Key);
                ActionInfo controlInfo = entry.Value;
                controlInfo.Token = token;
                for (int i = 0; i < controlInfo.Prompts.Count; i++)
                {
                    controlInfo.Prompts[i].SetToken(token);
                }
            }
        }
        
        #endregion
    }
}
