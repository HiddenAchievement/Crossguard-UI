namespace HiddenAchievement.CrossguardUi
{
    public interface ICrossToggle
    {
        bool IsOn { get; set; }
        int Id {  get; set; }
        void SetIsOnWithoutNotify(bool value);
    }
}