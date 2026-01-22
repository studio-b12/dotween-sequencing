using System;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    public abstract class TweenStepBase : ITweenStep
    {
        [SerializeField] private bool enabled = true;

        [SerializeField] private string title;

        [SerializeField] private TweenPlacement placement = TweenPlacement.Append;

        [Min(0)]
        [SerializeField] private float delay = 0f;

        public void AddTo(Sequence sequence)
        {
            if (!enabled) 
                return;

            Tween tween = CreateTween();
            if (tween == null)
                return;

            if (delay > 0f) 
                tween.SetDelay(delay);
            
            ConfigureTween(tween);
            
            switch (placement)
            {
                case TweenPlacement.Append: 
                    sequence.Append(tween); 
                    break;
                case TweenPlacement.Join:   
                    sequence.Join(tween); 
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected abstract Tween CreateTween();
        protected virtual void ConfigureTween(Tween tween) {}
    }

    [Serializable]
    public abstract class TweenStepBase<TTarget> : TweenStepBase where TTarget : Object
    {
        [SerializeField] private TTarget target;

        protected bool TryGetTarget(out TTarget target)
        {
            target = this.target;
            
            if (target)
                return true;
            
            Debug.LogWarning($"[{GetType().Name}] Missing target of type {typeof(TTarget).Name}.");
            return false;
        }
    }
        
    [Serializable]
    public abstract class TweenStepWithTweenOptions : TweenStepBase
    {
        [SerializeField] private Ease ease = Ease.OutQuad;
        [SerializeField] private bool relative = false;

        protected override void ConfigureTween(Tween tween)
        {
            tween.SetEase(ease);
            
            if (relative) 
                tween.SetRelative();
        }
    }

    [Serializable]
    public abstract class TweenStepWithTweenOptions<TTarget> : TweenStepBase<TTarget> where TTarget : Object
    {
        [SerializeField] private Ease ease = Ease.OutQuad;
        [SerializeField] private bool relative = false;

        protected override void ConfigureTween(Tween tween)
        {
            tween.SetEase(ease);
            
            if (relative)
                tween.SetRelative();
        }
    }
}
