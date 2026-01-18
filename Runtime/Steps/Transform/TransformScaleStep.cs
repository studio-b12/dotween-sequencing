using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Transform/Scale")]
    public class TransformScaleStep : TweenStepWithTweenOptions<Transform>
    {
        [SerializeField] private Vector3Axes axes = Vector3Axes.X | Vector3Axes.Y | Vector3Axes.Z;
        [SerializeField] private Vector3 endValue = Vector3.one;
        [SerializeField] private float duration = 0.5f;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Transform transform))
                return null;
            
            if (axes == 0)
                return null;
            
            Vector3 target = transform.localScale;

            if (axes.HasFlag(Vector3Axes.X)) target.x = endValue.x;
            if (axes.HasFlag(Vector3Axes.Y)) target.y = endValue.y;
            if (axes.HasFlag(Vector3Axes.Z)) target.z = endValue.z;
            
            return transform.DOScale(target, duration);
        }
    }
}
