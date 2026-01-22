using System;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    public class TweenVector2 : TweenValue<Vector2>
    {
        public Vector2Axes Axes;

        public TweenVector2()
        {
            Axes = Vector2Axes.X | Vector2Axes.Y;
        }

        public TweenVector2(Vector2 from, Vector2 to, Vector2Axes axes = Vector2Axes.X | Vector2Axes.Y) : base(from, to)
        {
            Axes = axes;
        }

        public TweenVector2(Vector2 to, Vector2Axes axes = Vector2Axes.X | Vector2Axes.Y) : base(to)
        {
            Axes = axes;
        }
    }
}