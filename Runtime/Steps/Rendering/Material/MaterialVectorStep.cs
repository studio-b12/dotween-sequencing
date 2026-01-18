using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Rendering/Material/Vector")]
    public class MaterialVectorStep : TweenStepWithTweenOptions<Renderer>
    {
        [SerializeField] private string property = "_MainTex_ST";
        [SerializeField] private Vector4 endValue = new Vector4(1, 1, 0, 0);
        [SerializeField] private float duration = 0.5f;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Renderer renderer)) 
                return null;
            
            if (!renderer.sharedMaterial)
                return null;

            return renderer.material.DOVector(endValue, property, duration);
        }
    }
}
