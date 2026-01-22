using System;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    public class TweenVector3 : TweenValue<Vector3>
    {
        public Vector3Axes Axes;

        public TweenVector3()
        {
            Axes = Vector3Axes.X | Vector3Axes.Y | Vector3Axes.Z;
        }
        
        public TweenVector3(Vector3 from, Vector3 to, Vector3Axes axes = Vector3Axes.X | Vector3Axes.Y | Vector3Axes.Z) : base(from, to)
        {
            Axes = axes;
        }

        public TweenVector3(Vector3 to, Vector3Axes axes = Vector3Axes.X | Vector3Axes.Y | Vector3Axes.Z) : base(to)
        {
            Axes = axes;
        }
    }
}