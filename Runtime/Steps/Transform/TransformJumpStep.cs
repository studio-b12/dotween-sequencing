using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStep("Transform/Jump")]
    public class TransformJumpStep : TweenStepWithTweenOptions<Transform>
    {
        [SerializeField] private Vector3Axes axes = Vector3Axes.X | Vector3Axes.Y | Vector3Axes.Z;
        [SerializeField] private Vector3 endValue;
        [SerializeField] private float jumpPower = 2f;
        [SerializeField] private int numJumps = 1;
        [SerializeField] private float duration = 1f;
        [SerializeField] private bool snapping = false;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Transform transform))
                return null;
            
            if (axes == 0)
                return null;
            
            Vector3 target = transform.position;

            if (axes.HasFlag(Vector3Axes.X)) target.x = endValue.x;
            if (axes.HasFlag(Vector3Axes.Y)) target.y = endValue.y;
            if (axes.HasFlag(Vector3Axes.Z)) target.z = endValue.z;

            return transform.DOJump(target, jumpPower, numJumps, duration, snapping);
        }
    }
}
