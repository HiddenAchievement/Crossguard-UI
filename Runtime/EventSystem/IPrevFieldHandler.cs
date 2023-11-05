using UnityEngine.EventSystems;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPrevFieldHandler: IEventSystemHandler
    {
        void OnPrevField(BaseEventData eventData);
    }
}
