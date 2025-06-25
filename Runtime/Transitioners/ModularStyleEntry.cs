using HiddenAchievement.CrossguardUi.Modules;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi
{
    [System.Serializable]
    public class ModularStyleEntry
    {
        public string ComponentPath;

        [SerializeReference]
        public IStyleModuleRule[] Style;
    }
}