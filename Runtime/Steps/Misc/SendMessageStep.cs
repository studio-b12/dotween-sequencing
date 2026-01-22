using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStep("Misc/SendMessage")]
    public class SendMessageStep : TweenStepBase<GameObject>
    {
        [SerializeField] private string forwardMessage = "OnPlayForward";
        [SerializeField] private string backwardsMessage = "OnPlayBackwards";

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out GameObject receiver))
                return null;
            
            return TweenStepUtils.CreateReversibleInstant(
                onForward: () => receiver.SendMessage(forwardMessage, SendMessageOptions.DontRequireReceiver),
                onBackwards: () => receiver.SendMessage(backwardsMessage, SendMessageOptions.DontRequireReceiver)
            );
        }
    }
}
