using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Transform/Blendable/Move By")]
    public class BlendableMoveByStep : TweenStepWithTweenOptions<Transform>
    {
        [SerializeField] private Vector3 by;
        [SerializeField] private float duration = 0.5f;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Transform transform))
                return null;
            
            return transform.DOBlendableMoveBy(by, duration);
        }
    }
}
