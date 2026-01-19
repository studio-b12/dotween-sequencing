using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    // TODO: Use material blocks.
    
    [Serializable]
    [TweenStepPath("Rendering/Material/Color")]
    public class MaterialColorStep : TweenStepWithTweenOptions<Renderer>
    {
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private string property = "_Color";
        [SerializeField] private TweenValue<Color> values = new(Color.white);

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Renderer renderer)) 
                return null;
            
            if (!renderer.sharedMaterial)
                return null;
            
            var tween = renderer.material.DOColor(values.To, duration);
            
            if (values.UseFrom)
            {
                tween = tween.From(values.From);
            }
            
            return tween;
        }
    }
}
