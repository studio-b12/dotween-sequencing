using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Misc/Enable Behaviour")]
    public class EnableComponentStep : TweenStepBase<Behaviour>
    {
        [SerializeField] private bool enabledValue = true;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Behaviour behaviour)) 
                return null;
            
            return DOVirtual.DelayedCall(0f, () => behaviour.enabled = enabledValue);
        }
    }
}
