using System;

namespace HiddenAchievement.CrossguardUi
{
	/// <summary>
	/// State flags for menu elements.
	/// </summary>
    [Serializable]
	public enum InteractState
	{
        Normal = 0,
        Highlighted,
        Selected,
        Pressed,
        Isolated,
        Checked,
        Disabled,
        Count
	}
}
