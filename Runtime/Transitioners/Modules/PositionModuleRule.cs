using System;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    [Serializable]
    [StyleModuleRule("Position")]
    public class PositionModuleRule : IStyleModuleRule
    {
        public Vector2 Position;

        public Type ModuleType => typeof(PositionModule);
        public IStyleModule GetModuleInstance() => PositionModule.Create();
    }
}