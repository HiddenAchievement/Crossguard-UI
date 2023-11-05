
namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// Provides string tokens to the ControllerPrompts for displaying controller buttons/keys.
    /// </summary>
    public interface IControllerTokenProvider
    {
        string LookupToken(string actionName);
    }
}
