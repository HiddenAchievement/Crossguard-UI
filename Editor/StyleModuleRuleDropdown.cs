using System;
using System.Linq;
using System.Reflection;
using HiddenAchievement.CrossguardUi.Modules;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace HiddenAchievement.CrossguardUi
{
    public class StyleModuleRuleDropdown : AdvancedDropdown
    {
        private class Item : AdvancedDropdownItem
        {
            public Item(Type type, string menuName) : base(menuName)
            {
                Type = type;
            }

            public Type Type { get; }
        }

        static readonly (Type Type, string MenuName)[] cache = TypeCache.GetTypesDerivedFrom<IStyleModuleRule>()
            .Where(x => !x.IsAbstract)
            .Where(x => !x.IsSpecialName)
            .Where(x => !x.IsGenericType)
            .Select(x =>
            {
                var attribute = x.GetCustomAttribute<StyleModuleRuleAttribute>();
                var menuName = attribute == null ? x.Name : attribute.MenuName;
                return (x, menuName);
            })
            .OrderBy(x => x.menuName)
            .ToArray();

        public event Action<Type> OnTypeSelected;

        public StyleModuleRuleDropdown(AdvancedDropdownState state) : base(state)
        {
            var minSize = minimumSize;
            minSize.y = 200f;
            minimumSize = minSize;
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Style Rule");
            foreach (var (type, menuName) in cache)
            {
                var splitStrings = menuName.Split('/');
                var parent = root;
                // Item lastItem = null;

                for (int i = 0; i < splitStrings.Length; i++)
                {
                    var str = splitStrings[i];

                    var foundChildItem = parent.children.FirstOrDefault(item => item.name == str);
                    if (foundChildItem != null)
                    {
                        parent = foundChildItem;
                        // lastItem = (Item)foundChildItem;
                        continue;
                    }

                    var child = new Item(type, str);
                    parent.AddChild(child);
                    parent = child;
                }
            }
            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);

            if (item is Item componentItem)
            {
                OnTypeSelected?.Invoke(componentItem.Type);
            }
        }
    }
}