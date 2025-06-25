using System;

namespace HiddenAchievement.CrossguardUi
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class StyleModuleRuleAttribute : Attribute
    {
        public StyleModuleRuleAttribute(string menuName)
        {
            MenuName = menuName;
        }
        
        public string MenuName { get; }
    }
}