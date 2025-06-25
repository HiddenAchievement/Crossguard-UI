using System;

namespace HiddenAchievement.CrossguardUi.Modules
{
    [Serializable]
    [StyleModuleRule("Alpha (Renderer)")]
    public class AlphaRendererModuleRule : IStyleModuleRule
    {
        public float Alpha;
        
        public Type ModuleType => typeof(AlphaRendererModule);
        public IStyleModule GetModuleInstance() => AlphaRendererModule.Create();
    }
}