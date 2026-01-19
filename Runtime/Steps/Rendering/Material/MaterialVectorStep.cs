using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Rendering/Material/Vector")]
    public class MaterialVectorStep : TweenStepWithTweenOptions<Renderer>
    {
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private string property = "_MainTex_ST";
        [SerializeField] private TweenValue<Vector4> values = new(new Vector4(1, 1, 0, 0));

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Renderer renderer)) 
                return null;
            
            if (!renderer.sharedMaterial)
                return null;

            var tween = renderer.material.DOVector(values.To, property, duration);

            if (values.UseFrom)
            {
                tween = tween.From(values.From);
            }
            
            return tween;
        }
    }
}
