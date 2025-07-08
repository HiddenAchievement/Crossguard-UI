using System;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    [Serializable]
    [StyleModuleRule("RectTransform/Anchored Position")]
    public class AnchoredPositionModuleRule : IStyleModuleRule
    {
        public Vector2 Position;

        public Type ModuleType => typeof(AnchoredPositionModule);
        public IStyleModule GetModuleInstance() => AnchoredPositionModule.Create();
    }
}