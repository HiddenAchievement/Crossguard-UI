namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// Provides a reset function for reusable objects.
    /// </summary>
    public interface IResettable
    {
        /// <summary>
        /// Return this object to an appropriate state for re-using.
        /// </summary>
        void Reset();
    }
}