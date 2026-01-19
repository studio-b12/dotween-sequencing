using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Misc/Enable Behaviour")]
    public class EnableComponentStep : TweenStepBase<Behaviour>
    {
        [SerializeField] private bool enabled = true;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Behaviour behaviour)) 
                return null;
            
            return TweenStepUtils.CreateReversibleInstant(
                onForward: () => behaviour.enabled = enabled,
                onBackwards: () => behaviour.enabled = !enabled
            );
        }
    }
}
