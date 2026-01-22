using System;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    public class TweenValue<T>
    {
        public bool UseFrom;
        public T From;
        public T To;

        public TweenValue()
        {
            UseFrom = false;
            From = default;
            To = default;
        }
        
        public TweenValue(T from, T to)
        {
            UseFrom = true;
            From = from;
            To = to;
        }
        
        public TweenValue(T to)
        {
            UseFrom = false;
            To = to;
        }
    }
}