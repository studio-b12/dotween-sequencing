using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Rendering/Material/Float")]
    public class MaterialFloatStep : TweenStepWithTweenOptions<Renderer>
    {
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private string property = "_Glossiness";
        [SerializeField] private TweenValue<float> values = new(0f);

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Renderer renderer)) 
                return null;
            
            if (!renderer.sharedMaterial)
                return null;

            var tween = renderer.material.DOFloat(values.To, property, duration);

            if (values.UseFrom)
            {
                tween = tween.From(values.From);
            }
            
            return tween;
        }
    }
}
