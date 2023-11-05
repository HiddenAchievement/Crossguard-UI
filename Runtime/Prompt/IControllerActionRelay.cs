using UnityEngine;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// Relays Actions to ControllerPrompts.
    /// </summary>
    public interface IControllerActionRelay
    {
        void RegisterInterestInAction(string action);
        void UnregisterInterestInAction(string action);
    }
}
