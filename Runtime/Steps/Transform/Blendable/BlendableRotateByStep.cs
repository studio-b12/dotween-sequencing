using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Transform/Blendable/Rotate By")]
    public class BlendableRotateByStep : TweenStepWithTweenOptions<Transform>
    {
        [SerializeField] private Vector3 by;
        [SerializeField] private float duration = 0.5f;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Transform transform))
                return null;
            
            return transform.DOBlendableRotateBy(by, duration);
        }
    }
}
