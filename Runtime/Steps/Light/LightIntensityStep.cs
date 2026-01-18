using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Light/Intensity")]
    public class LightIntensityStep : TweenStepWithTweenOptions<Light>
    {
        [SerializeField] private float endValue = 1f;
        [SerializeField] private float duration = 0.5f;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Light light)) 
                return null;
            
            return light.DOIntensity(endValue, duration);
        }
    }
}
