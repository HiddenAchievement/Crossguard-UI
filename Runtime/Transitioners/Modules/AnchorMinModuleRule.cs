using System;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    [Serializable]
    [StyleModuleRule("RectTransform/Anchor (Min)")]
    public class AnchorMinModuleRule : IStyleModuleRule
    {
        public Vector2 AnchorMin;

        public Type ModuleType => typeof(AnchorMinModule);
        public IStyleModule GetModuleInstance() => AnchorMinModule.Create();
    }
}