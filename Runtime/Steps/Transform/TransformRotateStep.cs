using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStep("Transform/Rotate")]
    public class TransformRotateStep : TweenStepWithTweenOptions<Transform>
    {
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private RotateMode rotateMode = RotateMode.Fast;
        [SerializeField] private Vector3Axes axes = Vector3Axes.X | Vector3Axes.Y | Vector3Axes.Z;
        [SerializeField] private TweenValue<Vector3> values;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Transform transform))
                return null;
            
            if (axes == 0)
                return null;
            
            Vector3 target = transform.localEulerAngles;

            if (axes.HasFlag(Vector3Axes.X)) target.x = values.To.x;
            if (axes.HasFlag(Vector3Axes.Y)) target.y = values.To.y;
            if (axes.HasFlag(Vector3Axes.Z)) target.z = values.To.z;

            var tween = transform.DORotate(target, duration, rotateMode);

            if (values.UseFrom)
            {
                Vector3 origin = transform.localEulerAngles;
                
                if (axes.HasFlag(Vector3Axes.X)) origin.x = values.From.x;
                if (axes.HasFlag(Vector3Axes.Y)) origin.y = values.From.y;
                if (axes.HasFlag(Vector3Axes.Z)) origin.z = values.From.z;

                tween = tween.From(origin);
            }
            
            return tween;
        }
    }
}
