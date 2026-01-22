using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStep("Light/Intensity")]
    public class LightIntensityStep : TweenStepWithTweenOptions<Light>
    {
        [SerializeField] private float duration = 0.5f;
        [TweenValueDrawer("Intensity")]
        [SerializeField] private TweenValue<float> values = new(0, 1);

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Light light)) 
                return null;
            
            var tween = light.DOIntensity(values.To, duration);
            
            if (values.UseFrom)
            {
                tween = tween.From(values.From);
            }

            return tween;
        }
    }
}
