#if UNITY_IOS || UNITY_ANDROID
#define INTERACT_MOBILE
#elif UNITY_STANDALONE || UNITY_WEBGL
#define INTERACT_DESKTOP
#else
#define INTERACT_CONSOLE
#endif

using System;
using System.Collections.Generic;
using HiddenAchievement.CrossguardUi.Modules;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi
{
    public class ModularTransitioner : AbstractTransitioner
    {
        private static readonly Dictionary<string, ComponentData> s_componentTable = new();
        private static readonly CrossInstancePool<ModuleSettings> s_moduleSettingsPool = new(() => new ModuleSettings());
        private static readonly CrossInstancePool<ComponentData>  s_componentDataPool = new(() => new ComponentData());
        
        private class ModuleSettings : IResettable
        {
            public IStyleModule Module;
            public IStyleModuleRule[] StyleRules = new IStyleModuleRule[(int)InteractState.Count];
            public InteractState ActiveState = InteractState.Normal;

            public void Reset()
            {
                Module = null;
                ActiveState = 0;
                for (int i = 0; i < StyleRules.Length; i++)
                {
                    StyleRules[i] = null;
                }
            }
        }

        private class ComponentData : IResettable
        {
            public RectTransform RectTransform;
            public List<ModuleSettings> ModuleSettings = new();

            public void Reset()
            {
                RectTransform = null;
                for (int i = 0; i < ModuleSettings.Count; i++)
                {
                    s_moduleSettingsPool.Return(ModuleSettings[i]);
                }
                ModuleSettings.Clear();
            }
        }
        
        [SerializeField]
        [Tooltip("Assign the scriptable object containing this element's style.")]
        private ModularStyle _style;
        
        private List<ComponentData> _componentData = new();
        private Dictionary<Type, IStyleModule> _moduleCatalog = new();
        
        /// <inheritdoc />
        public override float TransitionTime => _style == null ? 0 : _style.TransitionTime;
        
        public ModularStyle Style
        {
            get => _style;
            set
            {
                _style = value;
                if (Application.isPlaying)
                {
                    InitializeStates();
                }
#if UNITY_EDITOR
                else
                {
                    ManualInitializeAppearance();
                }
#endif
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _componentData.Count; i++)
            {
                s_componentDataPool.Return(_componentData[i]);
            }
            _componentData.Clear();
            foreach (IStyleModule module in _moduleCatalog.Values)
            {
                module.Free();
            }
            _moduleCatalog.Clear();
        }

        /// <inheritdoc />
        protected override void InitializeStates()
        {
            if (_style == null)
            {
                Debug.LogWarning($"ModularTransitioner {name} is missing a Style.", gameObject);
                return;
            }
            ProcessStyleEntries(InteractState.Normal, _style.NormalState);

#if INTERACT_DESKTOP
            ProcessStyleEntries(InteractState.Highlighted, _style.HighlightedState);
#endif        
            ProcessStyleEntries(InteractState.Selected, _style.SelectedState);
#if !INTERACT_MOBILE
            ProcessStyleEntries(InteractState.Isolated, _style.IsolatedState);
#endif
            ProcessStyleEntries(InteractState.Checked, _style.CheckedState);
            ProcessStyleEntries(InteractState.Pressed, _style.PressedState);
            ProcessStyleEntries(InteractState.Disabled, _style.DisabledState);
            
            // We no longer need the ComponentData in a Dictionary now. Extract it.
            foreach (var val in s_componentTable.Values)
            {
                _componentData.Add(val);
            }
            s_componentTable.Clear();
        }
        
        private void ProcessStyleEntries(InteractState state, ModularStyleEntry[] entries)
        {
            if (entries == null) return;
            for (int i = 0; i < entries.Length; i++)
            {
                string path = entries[i].ComponentPath;
                if (!s_componentTable.TryGetValue(path, out var componentData))
                {
                    RectTransform rectTransform = FindComponent(path);
                    componentData = s_componentDataPool.Fetch();
                    componentData.RectTransform = rectTransform;
                    s_componentTable.Add(path, componentData);
                }
                ProcessStyleEntry(state, entries[i], componentData);
            }
        }

        private void ProcessStyleEntry(InteractState state, ModularStyleEntry entry, ComponentData componentData)
        {
            for (int i = 0; i < entry.Style.Length; i++)
            {
                IStyleModuleRule rule = entry.Style[i];
                ModuleSettings settings = GetModuleSettingsForRule(rule, componentData);
                settings.StyleRules[(int)state] = rule;
                settings.Module.CacheComponent(componentData.RectTransform);
            }
        }

        private ModuleSettings GetModuleSettingsForRule(IStyleModuleRule rule, ComponentData componentData)
        {
            ModuleSettings settings;
            for (int i = 0; i < componentData.ModuleSettings.Count; i++)
            {
                settings = componentData.ModuleSettings[i];
                if (settings.Module.GetType() == rule.ModuleType)
                {
                    return settings;
                }
            }

            settings = new ModuleSettings { Module = GetModuleForRule(rule) };
            componentData.ModuleSettings.Add(settings);
            return settings;
        }
        
        private IStyleModule GetModuleForRule(IStyleModuleRule rule)
        {
            if (_moduleCatalog.TryGetValue(rule.ModuleType, out IStyleModule module))
            {
                return module;
            }
            _moduleCatalog[rule.ModuleType] = module = rule.GetModuleInstance();
            return module;
        }
        #region AbstractTransitioner
        /// <inheritdoc />
        protected override void ClearAllComponents()
        {
            // This is only called in the editor, so we can't assume things are initialized. We're using the Normal
            // state as a shortcut here, since if you're following best practices, most modules should be represented.
            if (_style == null) return;
            ModularStyleEntry[] styleEntries = _style.NormalState;
            foreach (ModularStyleEntry entry in styleEntries)
            {
                RectTransform rectTransform = FindComponent(entry.ComponentPath);
                if (rectTransform == null) continue;
                foreach (var rule in entry.Style)
                {
                    IStyleModule module = rule.GetModuleInstance();
                    module.ClearComponent(rectTransform);
                    module.Free();
                }
            }
        }

        /// <inheritdoc />
        protected override void ForceAppearance(InteractState state)
        {
            // This is only called in the editor, so we can't assume things are initialized.
            if (_style == null) return;
            ModularStyleEntry[] styleEntries = null;
            switch (state)
            {
                case InteractState.Normal:
                    styleEntries = _style.NormalState;
                    break;
                case InteractState.Highlighted:
                    styleEntries = _style.HighlightedState;
                    break;
                case InteractState.Selected:
                    styleEntries = _style.SelectedState;
                    break;
                case InteractState.Checked:
                    styleEntries = _style.CheckedState;
                    break;
                case InteractState.Pressed:
                    styleEntries = _style.PressedState;
                    break;
                case InteractState.Disabled:
                    styleEntries = _style.DisabledState;
                    break;
            }

            if ((styleEntries == null) || (styleEntries.Length == 0)) return;
            
            foreach (ModularStyleEntry entry in styleEntries)
            {
                RectTransform rectTransform = FindComponent(entry.ComponentPath);
                if (rectTransform == null) continue;
                foreach (var rule in entry.Style)
                {
                    IStyleModule module = rule.GetModuleInstance();
                    module.ForceComponentRule(rectTransform, rule);
                    module.Free();
                }
            }
        }

        /// <inheritdoc />
        protected override void TransitionOn(InteractState state, bool immediate)
        {
            if (_style == null) return;
            immediate |= _style.TransitionTime == 0;
            for (int i = 0; i < _componentData.Count; i++)
            {
                ComponentData componentData = _componentData[i];
                for (int j = 0; j < componentData.ModuleSettings.Count; j++)
                {
                    TransitionOn(state, immediate, componentData.RectTransform, componentData.ModuleSettings[j]);
                }
            }
        }
        
        /// <inheritdoc />
        protected override void TransitionOff(InteractState state, bool immediate)
        {
            if (_style == null) return;
            
            immediate |= _style.TransitionTime == 0;
            for (int i = 0; i < _componentData.Count; i++)
            {
                ComponentData componentData = _componentData[i];
                for (int j = 0; j < componentData.ModuleSettings.Count; j++)
                {
                    TransitionOff(state, immediate, componentData.RectTransform, componentData.ModuleSettings[j]);
                }
            }
        }
        #endregion

        private void TransitionOn(InteractState state, bool immediate, RectTransform component, ModuleSettings settings)
        {
            // Check to see if state priority currently overrules any transition from this module on this component.
            if (state <= settings.ActiveState) return;

            // See if there is a rule for this state.
            IStyleModuleRule rule = settings.StyleRules[(int)state];
            if (rule == null) return; // If there is no rule, we have nothing to do do.
            
            // Next, run the transition.
            settings.ActiveState = state;
            TransitionComponentByRule(component, settings.Module, rule, immediate);
        }
        
        private void TransitionOff(InteractState state, bool immediate, RectTransform component, ModuleSettings settings)
        {
            // Check to see if state priority currently overrules any transition from this module on this component.
            if (state != settings.ActiveState) return;
            
            // See if there was a rule for this state.
            IStyleModuleRule rule = settings.StyleRules[(int)state];
            if (rule == null) return; // If there was no rule applied for this module in this state, we have nothing to do.
            
            // Find the first active state that has a rule for this module.
            for (int i = (int)state - 1; i >= 0; i--)
            {
                if (!_stateFlags[i]) continue;
                rule = settings.StyleRules[i];
                if (rule == null) continue;
                settings.ActiveState = (InteractState)i;
                TransitionComponentByRule(component, settings.Module, rule, immediate);
                return;
            }
            // If we got here, somehow, transition to Normal.
            rule = settings.StyleRules[(int)InteractState.Normal];
            if (rule == null) return;
            settings.ActiveState = InteractState.Normal;
            TransitionComponentByRule(component, settings.Module, rule, immediate);
        }

        private void TransitionComponentByRule(RectTransform component, IStyleModule module, IStyleModuleRule rule,
            bool immediate)
        {
            if (immediate)
            {
                module.Transition(component, rule);
            }
            else
            {
                module.Transition(component, rule, _style.TransitionTime);
            }
        }
    }
}