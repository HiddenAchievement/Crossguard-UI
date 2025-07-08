using System;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    [Serializable]
    [StyleModuleRule("RectTransform/Anchor (Max)")]
    public class AnchorMaxModuleRule : IStyleModuleRule
    {
        public Vector2 AnchorMax;

        public Type ModuleType => typeof(AnchorMaxModule);
        public IStyleModule GetModuleInstance() => AnchorMaxModule.Create();
    }
}