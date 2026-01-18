using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Camera/Ortho Size")]
    public class CameraOrthoSizeStep : TweenStepWithTweenOptions<Camera>
    {
        [SerializeField] private float endValue = 5f;
        [SerializeField] private float duration = 0.5f;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Camera camera)) 
                return null;
            
            return camera.DOOrthoSize(endValue, duration);
        }
    }
}
