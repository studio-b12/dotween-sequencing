using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Rendering/Material/Float")]
    public class MaterialFloatStep : TweenStepWithTweenOptions<Renderer>
    {
        [SerializeField] private string property = "_Glossiness";
        [SerializeField] private float endValue = 0f;
        [SerializeField] private float duration = 0.5f;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Renderer renderer)) 
                return null;
            
            if (!renderer.sharedMaterial)
                return null;

            return renderer.material.DOFloat(endValue, property, duration);
        }
    }
}
