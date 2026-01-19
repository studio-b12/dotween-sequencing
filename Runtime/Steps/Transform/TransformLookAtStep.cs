using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Transform/LookAt")]
    public class TransformLookAtStep : TweenStepWithTweenOptions<Transform>
    {
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private AxisConstraint axisConstraint = AxisConstraint.None;
        [SerializeField] private Vector3 up = Vector3.up;
        [SerializeField] private Vector3Axes axes = Vector3Axes.X | Vector3Axes.Y | Vector3Axes.Z;
        [SerializeField] private Vector3 worldPosition;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Transform transform))
                return null;
            
            if (axes == 0)
                return null;
            
            Vector3 target = Vector3.zero;

            if (axes.HasFlag(Vector3Axes.X)) target.x = worldPosition.x;
            if (axes.HasFlag(Vector3Axes.Y)) target.y = worldPosition.y;
            if (axes.HasFlag(Vector3Axes.Z)) target.z = worldPosition.z;

            return transform.DOLookAt(target, duration, axisConstraint, up);
        }
    }
}
