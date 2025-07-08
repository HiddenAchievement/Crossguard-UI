using System;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    [Serializable]
    [StyleModuleRule("Transform/Local Scale")]
    public class LocalScaleModuleRule : IStyleModuleRule
    {
        public Vector3 Scale;

        public Type ModuleType => typeof(LocalScaleModule);
        public IStyleModule GetModuleInstance() => LocalScaleModule.Create();
    }
}