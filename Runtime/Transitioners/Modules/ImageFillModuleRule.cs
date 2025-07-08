using System;

namespace HiddenAchievement.CrossguardUi.Modules
{
    [Serializable]
    [StyleModuleRule("Image/Fill Amount")]
    public class ImageFillModuleRule : IStyleModuleRule
    {
        public float Fill;

        public Type ModuleType => typeof(ImageFillModule);
        public IStyleModule GetModuleInstance() => ImageFillModule.Create();
    }
}