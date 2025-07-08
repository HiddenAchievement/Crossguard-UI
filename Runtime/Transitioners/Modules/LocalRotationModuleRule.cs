using System;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    [Serializable]
    [StyleModuleRule("Transform/Local Rotation")]
    public class LocalRotationModuleRule : IStyleModuleRule
    {
        public Vector3 Rotation;

        public Type ModuleType => typeof(LocalRotationModule);
        public IStyleModule GetModuleInstance() => LocalRotationModule.Create();
    }
}