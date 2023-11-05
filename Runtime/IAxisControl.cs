using UnityEngine;
using UnityEngine.UI;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    ///  Axis-based component that hijacks navigation controls.
    /// </summary>
    public interface IAxisControl
    {
        bool Horizontal { get; }
        bool ReverseValue { get; }

        bool WrapValues { get; }
        
        ITransitioner Transitioner { get; }
        
        bool IsAtMin { get; }
        bool IsAtMax { get; }

        Selectable FindSelectableOnLeft();
        Selectable FindSelectableOnRight();
        Selectable FindSelectableOnUp();
        Selectable FindSelectableOnDown();

        bool IsActive();
        bool IsInteractable();

        void Increment();
        void Decrement();
    }
}
