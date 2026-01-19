using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Misc/SetActive")]
    public class SetActiveStep : TweenStepBase<GameObject>
    {
        [SerializeField] private bool active = true;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out GameObject obj)) 
                return null;

            return TweenStepUtils.CreateReversibleInstant(
                onForward: () => obj.SetActive(active),
                onBackwards: () => obj.SetActive(!active)
            );
        }
    }
}
