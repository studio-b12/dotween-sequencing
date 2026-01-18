using System;

namespace Rehawk.DOTweenSequencing
{
    /// <summary>
    /// Controls how a TweenStep appears in the Add Step menu.
    /// Use paths like "UI/CanvasGroup/Fade" or "Transform/Move".
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class TweenStepPathAttribute : Attribute
    {
        public string Path { get; }
        public TweenStepPathAttribute(string path) => Path = path;
    }
}
