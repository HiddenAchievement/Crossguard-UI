using System;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    [Serializable]
    [StyleModuleRule("RectTransform/Pivot")]
    public class PivotModuleRule : IStyleModuleRule
    {
        public Vector2 Pivot;

        public Type ModuleType => typeof(PivotModule);
        public IStyleModule GetModuleInstance() => PivotModule.Create();
    }
}