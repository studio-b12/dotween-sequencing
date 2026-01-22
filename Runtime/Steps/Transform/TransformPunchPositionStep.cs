using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStep("Transform/Punch/Position")]
    public class TransformPunchPositionStep : TweenStepWithTweenOptions<Transform>
    {
        [SerializeField] private Vector3 punch = Vector3.one;
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private int vibrato = 10;
        [SerializeField] private float elasticity = 1f;
        [SerializeField] private bool snapping = false;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Transform transform))
                return null;
            
            return transform.DOPunchPosition(punch, duration, vibrato, elasticity, snapping);
        }
    }
}
