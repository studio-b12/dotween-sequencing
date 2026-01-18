using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Misc/UnityEvent")]
    public class UnityEventStep : TweenStepBase
    {
        [SerializeField] private UnityEvent onEntered;

        protected override Tween CreateTween()
        {
            return DOVirtual.DelayedCall(0f, () =>
            {
                onEntered?.Invoke();
            });
        }
    }
}
