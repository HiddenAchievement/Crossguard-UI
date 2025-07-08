using System;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi.Modules
{
    [Serializable]
    [StyleModuleRule("Image/Sprite")]
    public class SpriteModuleRule : IStyleModuleRule
    {
        public Sprite Sprite;

        public Type ModuleType => typeof(SpriteModule);
        public IStyleModule GetModuleInstance() => SpriteModule.Create();
    }
}