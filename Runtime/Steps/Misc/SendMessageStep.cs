using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Misc/SendMessage")]
    public class SendMessageStep : TweenStepBase
    {
        [SerializeField] private GameObject receiver;
        [SerializeField] private string message = "OnTweenCallback";

        protected override Tween CreateTween()
        {
            return DOVirtual.DelayedCall(0f, () =>
            {
                if (receiver)
                    receiver.SendMessage(message, SendMessageOptions.DontRequireReceiver);
            });
        }
    }
}
