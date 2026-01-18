using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Light/Color")]
    public class LightColorStep : TweenStepWithTweenOptions<Light>
    {
        [SerializeField] private Color endValue = Color.white;
        [SerializeField] private float duration = 0.5f;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Light light)) 
                return null;
            
            return light.DOColor(endValue, duration);
        }
    }
}
