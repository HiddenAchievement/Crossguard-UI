using System;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    [Serializable]
    [StyleModuleRule("Scale")]
    public class ScaleModuleRule : IStyleModuleRule
    {
        public Vector2 Scale;

        public Type ModuleType => typeof(ScaleModule);
        public IStyleModule GetModuleInstance() => ScaleModule.Create();
    }
}