using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStep("Rendering/TrailRenderer/Time")]
    public class TrailRendererTimeStep : TweenStepWithTweenOptions<TrailRenderer>
    {
        [SerializeField] private float duration = 0.5f;
        [TweenValueDrawer("Time")]
        [SerializeField] private TweenValue<float> values = new(0.5f);

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out TrailRenderer trailRenderer))
                return null;
            
            var tween = trailRenderer.DOTime(values.To, duration);

            if (values.UseFrom)
            {
                tween = tween.From(values.From);
            }
            
            return tween;
        }
    }
}
