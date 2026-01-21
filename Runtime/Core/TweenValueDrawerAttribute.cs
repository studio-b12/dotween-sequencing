using System;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TweenValueDrawerAttribute : PropertyAttribute
    {
        public string Label { get; }
        
        public bool OverrideLabel => !string.IsNullOrEmpty(Label);
        
        public TweenValueDrawerAttribute(string label = null)
        {
            Label = label;
        }
    }
}