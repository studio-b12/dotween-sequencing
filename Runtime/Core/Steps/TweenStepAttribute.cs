using System;

namespace Rehawk.DOTweenSequencing
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class TweenStepAttribute : Attribute
    {
        /// <summary>
        /// Controls how a TweenStep appears in the Add Step menu.
        /// Use paths like "UI/CanvasGroup/Fade" or "Transform/Move".
        /// </summary>
        public string Path { get; }
        
        public TweenStepAttribute(string path) => Path = path;
    }
}
