using UnityEngine;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITransitioner
    {
        float TransitionTime { get; }
        bool IsInteractable { get; set; }
        void SetStateFlag(InteractState flag, bool immediate, bool force = false);
        bool GetStateFlag(InteractState flag);
        void ClearStateFlag(InteractState flag, bool immediate, bool force = false);
        void ResetState();
    }
}
