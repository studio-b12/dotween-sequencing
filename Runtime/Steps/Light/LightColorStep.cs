using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStep("Light/Color")]
    public class LightColorStep : TweenStepWithTweenOptions<Light>
    {
        [SerializeField] private float duration = 0.5f;
        [TweenValueDrawer("Color")]
        [SerializeField] private TweenValue<Color> values = new(Color.white);

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Light light)) 
                return null;
            
            var tween = light.DOColor(values.To, duration);
            
            if (values.UseFrom)
            {
                tween = tween.From(values.From);
            }
            
            return tween;
        }
    }
}
