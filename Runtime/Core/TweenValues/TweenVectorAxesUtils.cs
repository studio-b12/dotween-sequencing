using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    public static class TweenVectorAxesUtils
    {
        public static bool AnyAxes(this TweenVector3 tweenVector)
        {
            return tweenVector != null && tweenVector.Axes != 0;
        }

        public static Vector3 WithAxesFrom(this Vector3 baseValue, Vector3 source, Vector3Axes axes)
        {
            if ((axes & Vector3Axes.X) != 0) baseValue.x = source.x;
            if ((axes & Vector3Axes.Y) != 0) baseValue.y = source.y;
            if ((axes & Vector3Axes.Z) != 0) baseValue.z = source.z;
            return baseValue;
        }

        public static Vector3 ApplyTo(this Vector3 baseValue, TweenVector3 tweenVector)
        {
            if (tweenVector == null) return baseValue;
            return baseValue.WithAxesFrom(tweenVector.To, tweenVector.Axes);
        }

        public static Vector3 ApplyFrom(this Vector3 baseValue, TweenVector3 tweenVector)
        {
            if (tweenVector == null) return baseValue;
            return baseValue.WithAxesFrom(tweenVector.From, tweenVector.Axes);
        }

        public static bool TryBuildFrom(Vector3 current, TweenVector3 tweenVector, out Vector3 from)
        {
            from = current;

            if (tweenVector == null || !tweenVector.UseFrom)
                return false;

            from = from.WithAxesFrom(tweenVector.From, tweenVector.Axes);
            return true;
        }

        public static bool AnyAxes(this TweenVector2 tweenVector)
        {
            return tweenVector != null && tweenVector.Axes != 0;
        }

        public static Vector2 WithAxesFrom(this Vector2 baseValue, Vector2 source, Vector2Axes axes)
        {
            if ((axes & Vector2Axes.X) != 0) baseValue.x = source.x;
            if ((axes & Vector2Axes.Y) != 0) baseValue.y = source.y;
            return baseValue;
        }

        public static Vector2 ApplyTo(this Vector2 baseValue, TweenVector2 tweenVector)
        {
            if (tweenVector == null) return baseValue;
            return baseValue.WithAxesFrom(tweenVector.To, tweenVector.Axes);
        }

        public static Vector2 ApplyFrom(this Vector2 baseValue, TweenVector2 tweenVector)
        {
            if (tweenVector == null) return baseValue;
            return baseValue.WithAxesFrom(tweenVector.From, tweenVector.Axes);
        }

        public static bool TryBuildFrom(Vector2 current, TweenVector2 tweenVector, out Vector2 from)
        {
            from = current;

            if (tweenVector == null || !tweenVector.UseFrom)
                return false;

            from = from.WithAxesFrom(tweenVector.From, tweenVector.Axes);
            return true;
        }
    }
}
