using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Rehawk.DOTweenSequencing.Editor
{
    public sealed class TweenStepAdvancedDropdown : AdvancedDropdown
    {
        private readonly List<(Type type, string path)> items;
        private readonly Action<Type> onPick;

        public TweenStepAdvancedDropdown(AdvancedDropdownState state, IEnumerable<Type> stepTypes, Action<Type> onPick)
            : base(state)
        {
            this.onPick = onPick;
            items = stepTypes.Select(t => (t, GetMenuPath(t)))
                             .OrderBy(x => x.Item2, StringComparer.OrdinalIgnoreCase)
                             .ToList();

            minimumSize = new Vector2(380, 520);
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Add Tween Step");

            foreach ((Type type, string path) in items)
            {
                string[] parts = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(p => p.Trim())
                                     .Where(p => p.Length > 0)
                                     .ToArray();

                AdvancedDropdownItem parent = root;

                for (int i = 0; i < parts.Length - 1; i++)
                {
                    string folder = parts[i];
                    AdvancedDropdownItem existing = parent.children.FirstOrDefault(c => c.name == folder);
                    if (existing == null)
                    {
                        existing = new AdvancedDropdownItem(folder);
                        parent.AddChild(existing);
                    }
                    parent = existing;
                }

                string leaf = parts.Length > 0 ? parts[^1] : ObjectNames.NicifyVariableName(type.Name);
                parent.AddChild(new StepItem(leaf, type));
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            if (item is StepItem stepItem)
                onPick?.Invoke(stepItem.StepType);
        }

        private static string GetMenuPath(Type t)
        {
            string stepName = t.Name.EndsWith("Step", StringComparison.Ordinal)
                ? t.Name.Substring(0, t.Name.Length - 4)
                : t.Name;
            stepName = ObjectNames.NicifyVariableName(stepName);

            var attribute = (TweenStepPathAttribute)Attribute.GetCustomAttribute(t, typeof(TweenStepPathAttribute));
            
            if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Path))
            {
                string path = attribute.Path.Trim();

                if (path.Contains("/"))
                    return $"{path.Trim('/')}";

                stepName = path;
            }

            string namespaceName = string.IsNullOrEmpty(t.Namespace) ? "" : t.Namespace.Replace('.', '/');

            return $"{namespaceName}/{stepName}";
        }
        
        private sealed class StepItem : AdvancedDropdownItem
        {
            public readonly Type StepType;
            public StepItem(string name, Type stepType) : base(name) => StepType = stepType;
        }
    }
}
