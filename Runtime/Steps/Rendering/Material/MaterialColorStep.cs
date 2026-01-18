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
        [SerializeField] private string property = "_Color";
        [SerializeField] private Color endValue = Color.white;
        [SerializeField] private float duration = 0.5f;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Renderer renderer)) 
                return null;
            
            if (!renderer.sharedMaterial)
                return null;
            
            return renderer.material.DOColor(endValue, property, duration);
        }
    }
}
