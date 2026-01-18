using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStepPath("Misc/SetActive")]
    public class SetActiveStep : TweenStepBase
    {
        [SerializeField] private GameObject targetObject;
        [SerializeField] private bool active = true;

        protected override Tween CreateTween()
        {
            return DOVirtual.DelayedCall(0f, () =>
            {
                if (targetObject) 
                    targetObject.SetActive(active);
            });
        }
    }
}
