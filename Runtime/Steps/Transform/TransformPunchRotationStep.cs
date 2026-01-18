using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Transform/Punch/Rotation")]
    public class TransformPunchRotationStep : TweenStepWithTweenOptions<Transform>
    {
        [SerializeField] private Vector3 punch = Vector3.one * 10f;
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private int vibrato = 10;
        [SerializeField] private float elasticity = 1f;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Transform transform))
                return null;
            
            return transform.DOPunchRotation(punch, duration, vibrato, elasticity);
        }
    }
}
