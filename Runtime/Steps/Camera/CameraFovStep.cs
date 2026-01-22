using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStep("Camera/Field Of View")]
    public class CameraFovStep : TweenStepWithTweenOptions<Camera>
    {
        [SerializeField] private float duration = 0.5f;
        [TweenValueDrawer("Field Of View")]
        [SerializeField] private TweenValue<float> values = new(60f);

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Camera camera)) 
                return null;
            
            var tween = camera.DOFieldOfView(values.To, duration);

            if (values.UseFrom)
            {
                tween = tween.From(values.From);
            }
            
            return tween;
        }
    }
}
