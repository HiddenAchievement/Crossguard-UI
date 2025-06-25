using System;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    [Serializable]
    [StyleModuleRule("Color (RGB)")]
    public class ColorRgbRendererModuleRule : IStyleModuleRule
    {
        public Color Color;

        public Type ModuleType => typeof(ColorRgbRendererModule);
        public IStyleModule GetModuleInstance() => ColorRgbRendererModule.Create();
    }
}