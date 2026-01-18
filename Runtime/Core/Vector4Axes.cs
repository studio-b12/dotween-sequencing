using System;

namespace Rehawk.DOTweenSequencing
{
    [Flags]
    public enum Vector4Axes
    {
        X = 1 << 0,
        Y = 1 << 1,
        Z = 1 << 2,
        W = 1 << 3
    }
}