using System;
using System.Collections;
using System.Collections.Generic;
using HiddenAchievement.CrossguardUi.Modules;
using LitMotion;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi
{
    public class ModularTransitionManager
    {
        private class ModuleSettings : IResettable
        {
            public IStyleModule Module;
            public readonly List<IStyleModuleRule> StyleRules = new();
            public int ActiveState = -1;
            
            public void Initialize(int stateCount)
            {
                if (StyleRules.Capacity < stateCount)
                {
                    StyleRules.Capacity = stateCount;
                }
                for (int i = 0; i < stateCount; i++)
                {
                    StyleRules.Add(null);
                }
            }

            public void Reset()
            {
                Module = null;
                ActiveState = -1;
                StyleRules.Clear();
            }
        }

        private class ComponentData : IResettable
        {
            public Transform Transform;
            public readonly List<ModuleSettings> Settings = new();

            public void Reset()
            {
                Transform = null;
                for (int i = 0; i < Settings.Count; i++)
                {
                    s_moduleSettingsPool.Return(Settings[i]);
                }
                Settings.Clear();
            }
        }
        
        private static readonly Dictionary<string, ComponentData> s_componentTable = new();
        private static readonly CrossInstancePool<ModuleSettings> s_moduleSettingsPool = new(() => new ModuleSettings());
        private static readonly CrossInstancePool<ComponentData>  s_componentDataPool = new(() => new ComponentData());
        
        private readonly List<ComponentData> _componentData = new();
        private readonly Dictionary<Type, IStyleModule> _moduleCatalog = new();
        private float _transitionTime;
        private Transform _transform;

        private ModularStyleEntry[][] _stateStyles;
        private int _stateCount;
        private bool _mutuallyExclusive;

        /// <summary>
        /// Initializes the manager, so it's ready to use.
        /// </summary>
        /// <param name="transform">The root Transform of the GameObject we're applying transitions to.</param>
        /// <param name="transitionTime">The time in seconds that the transitions should occur over.</param>
        /// <param name="mutuallyExclusive">Whether the states should be considered mutually exclusive, or layered.</param>
        /// <param name="stateStyles">The style definitions for each of the states.</param>
        public void Initialize(Transform transform, float transitionTime, bool mutuallyExclusive, ModularStyleEntry[][] stateStyles)
        {
            _transform      = transform;
            _transitionTime = transitionTime;
            _mutuallyExclusive = mutuallyExclusive;
            _stateStyles    = stateStyles;
            _stateCount     = stateStyles.Length;
        }

        /// <summary>
        /// Tidy up when destroying things.
        /// </summary>
        public void Cleanup()
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

        /// <summary>
        /// Process and pre-cache all state settings for this transitioner.
        /// </summary>
        public void InitializeStates()
        {
            for (int i = 0; i < _stateCount; i++)
            {
                ProcessStyleEntries(i, _stateStyles[i]);
            }
            
            // We no longer need the ComponentData in a Dictionary now. Extract it.
            foreach (var val in s_componentTable.Values)
            {
                _componentData.Add(val);
            }
            s_componentTable.Clear();
        }
        
        private void ProcessStyleEntries(int state, ModularStyleEntry[] entries)
        {
            if (entries == null) return;
            for (int i = 0; i < entries.Length; i++)
            {
                string path = entries[i].ComponentPath;
                if (!s_componentTable.TryGetValue(path, out var componentData))
                {
                    Transform transform = UiUtilities.FindComponent(_transform, path);
                    if (transform == null)
                    {
                        Debug.LogError($"ModularTransitionManager: Cannot process state {state}. No transform found at path \"{path}\"");
                    }
                    componentData = s_componentDataPool.Fetch();
                    componentData.Transform = transform;
                    s_componentTable.Add(path, componentData);
                }
                ProcessStyleEntry(state, entries[i], componentData);
            }
        }

        private void ProcessStyleEntry(int state, ModularStyleEntry entry, ComponentData componentData)
        {
            for (int i = 0; i < entry.Style.Length; i++)
            {
                IStyleModuleRule rule = entry.Style[i];
                ModuleSettings settings = GetModuleSettingsForRule(rule, componentData);
                settings.StyleRules[state] = rule;
                settings.Module.CacheComponent(componentData.Transform);
            }
        }

        private ModuleSettings GetModuleSettingsForRule(IStyleModuleRule rule, ComponentData componentData)
        {
            ModuleSettings settings;
            for (int i = 0; i < componentData.Settings.Count; i++)
            {
                settings = componentData.Settings[i];
                if (settings.Module.GetType() == rule.ModuleType)
                {
                    return settings;
                }
            }
            
            settings = s_moduleSettingsPool.Fetch();
            settings.Initialize(_stateCount);
            settings.Module = GetModuleForRule(rule);
            componentData.Settings.Add(settings);
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
        
        /// <summary>
        /// Revert all components to a pristine state. Must be able to work without initialization running first.
        /// </summary>
        /// <param name="transform">The transform of the component being manipulated.</param>
        /// <param name="styleEntries">Style entries for the current state.</param>
        public static void ClearAllComponents(Transform transform, ModularStyleEntry[] styleEntries)
        {
            // This is only called in the editor, so we can't assume things are initialized. We're using the Normal
            // state as a shortcut here, since if you're following best practices, most modules should be represented.
            if (styleEntries == null) return;
            foreach (ModularStyleEntry entry in styleEntries)
            {
                Transform componentTransform = UiUtilities.FindComponent(transform, entry.ComponentPath);
                if (componentTransform == null) continue;
                foreach (var rule in entry.Style)
                {
                    if (rule == null)
                    {
                        Debug.LogError($"Missing rule in style for {componentTransform}", transform.gameObject);
                    }
                    IStyleModule module = rule.GetModuleInstance();
                    module.ClearComponent(componentTransform);
                    module.Free();
                }
            }
        }

        /// <summary>
        /// Immediately forces the appearance of the UI element into the specified state. Must be able to work without
        /// initialization running first.
        /// </summary>
        /// <param name="transform">The transform of the component being manipulated.</param>
        /// <param name="styleEntries">Style entries for the current state.</param>
        public static void ForceAppearance(Transform transform, ModularStyleEntry[] styleEntries)
        {
            // This is only called in the editor, so we can't assume things are initialized.
            if (styleEntries == null || styleEntries.Length == 0) return;
            
            foreach (ModularStyleEntry entry in styleEntries)
            {
                Transform componentTransform = UiUtilities.FindComponent(transform, entry.ComponentPath);
                if (componentTransform == null) continue;
                foreach (var rule in entry.Style)
                {
                    IStyleModule module = rule.GetModuleInstance();
                    module.ForceComponentRule(componentTransform, rule);
                    module.Free();
                }
            }
        }

        /// <summary>
        /// Turn on a specified state. (The effects may not be immediately visible if they are overridden by a higher
        /// priority state.)
        /// </summary>
        /// <param name="state">The state to turn on.</param>
        /// <param name="immediate">Whether to change states immediately, with no animation.</param>
        /// <param name="easing">The transition easing to use (if this is not immediate).</param>
        public void TransitionOn(int state, bool immediate, Ease easing = Ease.Linear)
        {
            if (_stateStyles == null) return;
            immediate |= _transitionTime == 0;
            for (int i = 0; i < _componentData.Count; i++)
            {
                ComponentData componentData = _componentData[i];
                for (int j = 0; j < componentData.Settings.Count; j++)
                {
                    TransitionOn(state, immediate, componentData.Transform, componentData.Settings[j], easing);
                }
            }
        }

        /// <summary>
        /// Turn off a specified state. (The effects may not be immediately visible if there is a higher priority state
        /// overriding the effects of this state.)
        /// </summary>
        /// <param name="state">The state to turn off.</param>
        /// <param name="immediate">Whether to change states immediately, with no animation.</param>
        /// <param name="stateFlags">The current states.</param>
        /// <param name="easing">The transition easing to use (if this is not immediate).</param>
        public void TransitionOff(int state, bool immediate, BitArray stateFlags, Ease easing = Ease.Linear)
        {
            if (_stateStyles == null) return;
            immediate |= _transitionTime == 0;
            for (int i = 0; i < _componentData.Count; i++)
            {
                ComponentData componentData = _componentData[i];
                for (int j = 0; j < componentData.Settings.Count; j++)
                {
                    TransitionOff(state, immediate, componentData.Transform, componentData.Settings[j], stateFlags, easing);
                }
            }
        }
        
        private void TransitionOn(int state, bool immediate, Transform component, ModuleSettings settings, Ease easing)
        {
            // Check to see if state priority currently overrules any transition from this module on this component.
            if (state <= settings.ActiveState && !_mutuallyExclusive) return;

            // See if there is a rule for this state.
            IStyleModuleRule rule = settings.StyleRules[state];

            if (rule == null)
            {
                if (!_mutuallyExclusive || settings.ActiveState == 0) return;

                // If this is mutually exclusive, we should still turn off the current state for this component/module.
                settings.ActiveState = 0;
                rule = settings.StyleRules[0];
                TransitionComponentByRule(component, settings.Module, rule, immediate, easing);

                return;
            }
            
            // Next, run the transition.
            settings.ActiveState = state;
            TransitionComponentByRule(component, settings.Module, rule, immediate, easing);
        }
        
        private void TransitionOff(int state, bool immediate, Transform component, ModuleSettings settings, BitArray stateFlags, Ease easing)
        {
            // Check to see if state priority currently overrules any transition from this module on this component.
            if (state != settings.ActiveState) return;
            
            // See if there was a rule for this state.
            IStyleModuleRule rule = settings.StyleRules[state];
            if (rule == null) return; // If there was no rule applied for this module in this state, we have nothing to do.
            
            // Find the first active state that has a rule for this module.
            for (int i = state - 1; i >= 0; i--)
            {
                if (!stateFlags[i]) continue;
                rule = settings.StyleRules[i];
                if (rule == null) continue;
                settings.ActiveState = i;
                TransitionComponentByRule(component, settings.Module, rule, immediate, easing);
                return;
            }
            // If we got here, somehow, transition to Normal.
            rule = settings.StyleRules[0];
            if (rule == null) return;
            settings.ActiveState = 0;
            TransitionComponentByRule(component, settings.Module, rule, immediate, easing);
        }

        private void TransitionComponentByRule(Transform component, IStyleModule module, IStyleModuleRule rule,
            bool immediate, Ease easing)
        {
            if (immediate)
            {
                module.Transition(component, rule);
            }
            else
            {
                module.Transition(component, rule, _transitionTime, easing);
            }
        }
    }
}