using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Misc/Interval")]
    public class IntervalStep : TweenStepBase
    {
        [SerializeField] private float duration = 0.25f;
        
        protected override Tween CreateTween() => DOVirtual.DelayedCall(duration, () => { });
    }
}
