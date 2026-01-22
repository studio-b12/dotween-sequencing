using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStep("Misc/UnityEvent")]
    public class UnityEventStep : TweenStepBase
    {
        [SerializeField] private UnityEvent onPlayForwards;
        [SerializeField] private UnityEvent onPlayBackwards;

        protected override Tween CreateTween()
        {
            return TweenStepUtils.CreateReversibleInstant(
                onForward: () => onPlayForwards.Invoke(),
                onBackwards: () => onPlayBackwards.Invoke()
            );
        }
    }
}
