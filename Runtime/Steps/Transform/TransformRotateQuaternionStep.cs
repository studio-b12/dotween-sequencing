using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Transform/Rotate Quaternion")]
    public class TransformRotateQuaternionStep : TweenStepWithTweenOptions<Transform>
    {
        [SerializeField] private TweenValue<Quaternion> values;
        [SerializeField] private float duration = 0.5f;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Transform transform))
                return null;

            var tween = transform.DORotateQuaternion(values.To, duration);

            if (values.UseFrom)
            {
                tween = tween.From(values.From);
            }

            return tween;
        }
    }
}
