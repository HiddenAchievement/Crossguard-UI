using System;

namespace HiddenAchievement.CrossguardUi.Modules
{
    [Serializable]
    [StyleModuleRule("CanvasGroup/Alpha (CanvasGroup)")]
    public class AlphaCanvasGroupModuleRule : IStyleModuleRule
    {
        public float Alpha;

        public Type ModuleType => typeof(AlphaCanvasGroupModule);
        public IStyleModule GetModuleInstance() => AlphaCanvasGroupModule.Create();
    }
}