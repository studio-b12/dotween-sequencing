using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Transform/Move Local")]
    public class TransformLocalMoveStep : TweenStepWithTweenOptions<Transform>
    {
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private bool snapping = false;
        [SerializeField] private Vector3Axes axes = Vector3Axes.X | Vector3Axes.Y | Vector3Axes.Z;
        [SerializeField] private TweenValue<Vector3> values;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Transform transform)) 
                return null;
            
            if (axes == 0)
                return null;
            
            Vector3 target = transform.localPosition;

            if (axes.HasFlag(Vector3Axes.X)) target.x = values.To.x;
            if (axes.HasFlag(Vector3Axes.Y)) target.y = values.To.y;
            if (axes.HasFlag(Vector3Axes.Z)) target.z = values.To.z;

            var tween = transform.DOLocalMove(target, duration, snapping);

            if (values.UseFrom)
            {
                Vector3 origin = transform.localPosition;
                
                if (axes.HasFlag(Vector3Axes.X)) origin.x = values.From.x;
                if (axes.HasFlag(Vector3Axes.Y)) origin.y = values.From.y;
                if (axes.HasFlag(Vector3Axes.Z)) origin.z = values.From.z;

                tween = tween.From(origin);
            }
            
            return tween;
        }
    }
}
