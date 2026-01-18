using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Rendering/TrailRenderer/Time")]
    public class TrailRendererTimeStep : TweenStepWithTweenOptions<TrailRenderer>
    {
        [SerializeField] private float endValue = 0.5f;
        [SerializeField] private float duration = 0.5f;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out TrailRenderer trailRenderer))
                return null;
            
            return trailRenderer.DOTime(endValue, duration);
        }
    }
}
