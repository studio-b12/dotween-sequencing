using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Camera/Field Of View")]
    public class CameraFovStep : TweenStepWithTweenOptions<Camera>
    {
        [SerializeField] private float endValue = 60f;
        [SerializeField] private float duration = 0.5f;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Camera camera)) 
                return null;
            
            return camera.DOFieldOfView(endValue, duration);
        }
    }
}
