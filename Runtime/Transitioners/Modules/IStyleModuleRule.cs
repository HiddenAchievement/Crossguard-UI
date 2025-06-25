using System;

namespace HiddenAchievement.CrossguardUi.Modules
{
    public interface IStyleModuleRule
    {
        Type ModuleType { get; }
        IStyleModule GetModuleInstance();
    }
}