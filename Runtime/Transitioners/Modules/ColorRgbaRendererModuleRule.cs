using System;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    [Serializable]
    [StyleModuleRule("Color (RGBA)")]
    public class ColorRgbaRendererModuleRule : IStyleModuleRule
    {
        public Color Color;

        public Type ModuleType => typeof(ColorRgbaRendererModule);
        public IStyleModule GetModuleInstance() => ColorRgbaRendererModule.Create();
    }
}