using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// 
    /// </summary>
    public class CrossToggleGroupBase : UIBehaviour
    {
        protected readonly List<ICrossToggle> _toggles = new();

        protected CrossToggleGroupBase()
        {
        }

        public virtual bool AllowSwitchOff
        {
            get => true;
            // ReSharper disable once ValueParameterNotUsed
            set {}
        }
        
        protected override void Start()
        {
            EnsureValidState();
            base.Start();
        }
        
        protected virtual void ValidateToggleIsInGroup(ICrossToggle toggle)
        {
            if (toggle == null || !_toggles.Contains(toggle))
                throw new ArgumentException(string.Format("Toggle {0} is not part of ToggleGroup {1}", new object[] {toggle, this}));
        }
        
        /// <summary>
        /// Notify the group that the given toggle is enabled.
        /// </summary>
        /// <param name="toggle">The toggle that got triggered on.</param>
        /// <param name="sendCallback">If other toggles should send onValueChanged.</param>
        public virtual void NotifyToggleOn(ICrossToggle toggle, bool sendCallback = true)
        {
        }
        
        /// <summary>
        /// Notify the group that the given toggle is disabled.
        /// </summary>
        /// <param name="toggle">The toggle that got triggered off.</param>
        /// <param name="sendCallback">If other toggles should send onValueChanged.</param>
        public virtual void NotifyToggleOff(ICrossToggle toggle, bool sendCallback = true)
        {
        }
        
        /// <summary>
        /// Unregister a toggle from the group.
        /// </summary>
        /// <param name="toggle">The toggle to remove.</param>
        public virtual void UnregisterToggle(ICrossToggle toggle)
        {
            if (_toggles.Contains(toggle))
                _toggles.Remove(toggle);
        }
        
        /// <summary>
        /// Register a toggle with the toggle group so it is watched for changes and notified if another toggle in the group changes.
        /// </summary>
        /// <param name="toggle">The toggle to register with the group.</param>
        public virtual void RegisterToggle(ICrossToggle toggle)
        {
            if (!_toggles.Contains(toggle))
                _toggles.Add(toggle);
        }

        /// <summary>
        /// Ensure that the toggle group still has a valid state. This is only relevant when a ToggleGroup is Started
        /// or a Toggle has been deleted from the group.
        /// </summary>
        public virtual void EnsureValidState()
        {
        }
        
        /// <summary>
        /// Are any of the toggles on?
        /// </summary>
        /// <returns>Are and of the toggles on?</returns>
        public virtual bool AnyTogglesOn()
        {
            return _toggles.Find(x => x.IsOn) != null;
        }
        
        /// <summary>
        /// Returns the toggles in this group that are active.
        /// </summary>
        /// <returns>The active toggles in the group.</returns>
        /// <remarks>
        /// Toggles belonging to this group but are not active either because their GameObject is inactive or because the Toggle component is disabled, are not returned as part of the list.
        /// </remarks>
        public virtual IEnumerable<ICrossToggle> ActiveToggles()
        {
            return _toggles.Where(x => x.IsOn);
        }

        /// <summary>
        /// Switch all toggles off.
        /// </summary>
        /// <remarks>
        /// This method can be used to switch all toggles off, regardless of whether the allowSwitchOff property is enabled or not.
        /// </remarks>
        public virtual void SetAllTogglesOff(bool sendCallback = true)
        {
        }
    }
}
