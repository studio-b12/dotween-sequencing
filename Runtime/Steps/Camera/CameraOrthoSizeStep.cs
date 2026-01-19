using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Camera/Ortho Size")]
    public class CameraOrthoSizeStep : TweenStepWithTweenOptions<Camera>
    {
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private TweenValue<float> values = new(5f);

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Camera camera)) 
                return null;
            
            var tween = camera.DOOrthoSize(values.To, duration);

            if (values.UseFrom)
            {
                tween = tween.From(values.From);
            }
            
            return tween;
        }
    }
}
