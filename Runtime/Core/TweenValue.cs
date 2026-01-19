using System;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    public class TweenValue<T>
    {
        public bool UseFrom = true;
        public T From;
        public T To;

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