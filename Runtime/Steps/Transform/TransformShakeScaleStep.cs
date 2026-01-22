using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStep("Transform/Shake/Scale")]
    public class TransformShakeScaleStep : TweenStepWithTweenOptions<Transform>
    {
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private float strength = 1f;
        [SerializeField] private int vibrato = 10;
        [SerializeField] private float randomness = 90f;
        [SerializeField] private bool fadeOut = true;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Transform transform))
                return null;
            
            return transform.DOShakeScale(duration, strength, vibrato, randomness, fadeOut);
        }
    }
}
