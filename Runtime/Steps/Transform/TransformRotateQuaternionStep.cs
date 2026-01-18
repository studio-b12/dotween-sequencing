using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Transform/Rotate Quaternion")]
    public class TransformRotateQuaternionStep : TweenStepWithTweenOptions<Transform>
    {
        [SerializeField] private Quaternion endValue = Quaternion.identity;
        [SerializeField] private float duration = 0.5f;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Transform transform))
                return null;
            
            return transform.DORotateQuaternion(endValue, duration);
        }
    }
}
