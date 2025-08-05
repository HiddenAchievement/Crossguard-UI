using System;

namespace HiddenAchievement.CrossguardUi.Modules
{
    [Serializable]
    [StyleModuleRule("CanvasRenderer/Alpha (Renderer)")]
    public class AlphaRendererModuleRule : IStyleModuleRule
    {
        public float Alpha;
        
        public Type ModuleType => typeof(AlphaRendererModule);
        public IStyleModule GetModuleInstance() => AlphaRendererModule.Create();
    }
}