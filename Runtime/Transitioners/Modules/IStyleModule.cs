using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    public interface IStyleModule : IResettable
    {
        /// <summary>
        /// Return to pool (if there is one).
        /// </summary>
        void Free();

        /// <summary>
        /// Caches any parts of a UI component that this module is interested in.
        /// </summary>
        /// <param name="component">The recttransform of the component we would like to register.</param>
        void CacheComponent(RectTransform component);
        
        /// <summary>
        /// Set this component to a pristine state. Called only in-editor.
        /// </summary>
        /// <param name="component"></param>
        void ClearComponent(RectTransform component);

        /// <summary>
        /// Force-applies a specific rule to this component. Called only in-editor.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="rule"></param>
        void ForceComponentRule(RectTransform component, IStyleModuleRule rule);
        
        /// <summary>
        /// Do an immediate runtime transition, based on the module rule.
        /// </summary>
        /// <param name="component">The component being changed.</param>
        /// <param name="rule">The rule to apply.</param>
        void Transition(RectTransform component, IStyleModuleRule rule);

        /// <summary>
        /// Do an animated runtime transition, based on the module rule.
        /// </summary>
        /// <param name="component">The component being changed.</param>
        /// <param name="rule">The rule to apply.</param>
        /// <param name="duration">The amount of time the transition should take</param>
        void Transition(RectTransform component, IStyleModuleRule rule, float duration);
    }
}