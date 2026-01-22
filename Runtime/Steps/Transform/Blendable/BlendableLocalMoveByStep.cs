using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStep("Transform/Blendable/Local Move By")]
    public class BlendableLocalMoveByStep : TweenStepWithTweenOptions<Transform>
    {
        [SerializeField] private Vector3 by;
        [SerializeField] private float duration = 0.5f;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Transform transform))
                return null;
            
            return transform.DOBlendableLocalMoveBy(by, duration);
        }
    }
}
