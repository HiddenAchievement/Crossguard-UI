using UnityEngine.EventSystems;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMoveEndHandler: IEventSystemHandler
    {
        void OnMoveEnd(BaseEventData eventData);
    }
}
