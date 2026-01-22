using System;
using DG.Tweening;

namespace Rehawk.DOTweenSequencing
{
    public static class TweenStepUtils
    {
        /// <summary>
        /// A tiny-duration tween that calls:
        /// - onForward when played forward past its position
        /// - onBackwards when rewound back before its position
        /// </summary>
        public static Tween CreateReversibleInstant(Action onForward, Action onBackwards)
        {
            const float markerDuration = 0.0001f;

            bool isForwardApplied = false;

            Tween tween = DOTween.To(() => 0f, _ => { }, 1f, markerDuration)
                                 .SetEase(Ease.Linear);

            tween.OnPlay(() =>
            {
                if (isForwardApplied) 
                    return;
                
                isForwardApplied = true;
                onForward?.Invoke();
            });

            tween.OnRewind(() =>
            {
                if (!isForwardApplied)
                    return;
                
                isForwardApplied = false;
                onBackwards?.Invoke();
            });

            tween.OnComplete(() =>
            {
                if (isForwardApplied) 
                    return;
                
                isForwardApplied = true;
                onForward?.Invoke();
            });

            return tween;
        }
    }
}