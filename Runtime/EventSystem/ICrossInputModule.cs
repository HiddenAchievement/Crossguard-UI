using UnityEngine;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// Make an adapter to look up information from whatever type of input module you're using.
    /// </summary>
    public interface ICrossInputModule
    {
        float RepeatDelay { get; }
        float RepeatRate { get; }
    }
}
