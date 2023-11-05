using UnityEngine.EventSystems;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// 
    /// </summary>
    public interface INextFieldHandler: IEventSystemHandler
    {
        void OnNextField(BaseEventData eventData);
    }
}
