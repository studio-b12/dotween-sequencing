using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Transform/Rotate")]
    public class TransformRotateStep : TweenStepWithTweenOptions<Transform>
    {
        [SerializeField] private Vector3Axes axes = Vector3Axes.X | Vector3Axes.Y | Vector3Axes.Z;
        [SerializeField] private Vector3 endValue;
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private RotateMode rotateMode = RotateMode.Fast;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Transform transform))
                return null;
            
            if (axes == 0)
                return null;
            
            Vector3 target = transform.eulerAngles;

            if (axes.HasFlag(Vector3Axes.X)) target.x = endValue.x;
            if (axes.HasFlag(Vector3Axes.Y)) target.y = endValue.y;
            if (axes.HasFlag(Vector3Axes.Z)) target.z = endValue.z;

            return transform.DORotate(target, duration, rotateMode);
        }
    }
}
