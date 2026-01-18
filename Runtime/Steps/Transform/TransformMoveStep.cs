using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Transform/Move")]
    public class TransformMoveStep : TweenStepWithTweenOptions<Transform>
    {
        [SerializeField] private Vector3Axes axes = Vector3Axes.X | Vector3Axes.Y | Vector3Axes.Z;
        [SerializeField] private Vector3 endValue;
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private bool snapping = false;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Transform t)) return null;
            
            if (axes == 0)
                return null;
            
            Vector3 target = t.position;

            if (axes.HasFlag(Vector3Axes.X)) target.x = endValue.x;
            if (axes.HasFlag(Vector3Axes.Y)) target.y = endValue.y;
            if (axes.HasFlag(Vector3Axes.Z)) target.z = endValue.z;

            return t.DOMove(target, duration, snapping);
        }
    }
}
