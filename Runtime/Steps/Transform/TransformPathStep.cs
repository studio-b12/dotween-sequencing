using System;
using DG.Tweening;
using UnityEngine;

namespace Rehawk.DOTweenSequencing
{
    [Serializable]
    [TweenStep("Transform/Path/World")]
    public class TransformPathStep : TweenStepWithTweenOptions<Transform>
    {
        [SerializeField] private Vector3[] waypoints = new Vector3[2];
        [SerializeField] private float duration = 1f;
        [SerializeField] private PathType pathType = PathType.CatmullRom;
        [SerializeField] private PathMode pathMode = PathMode.Full3D;
        [SerializeField] private int resolution = 10;
        [SerializeField] private Color gizmoColor = Color.green;

        protected override Tween CreateTween()
        {
            if (!TryGetTarget(out Transform transform))
                return null;
            
            return transform.DOPath(waypoints, duration, pathType, pathMode, resolution, gizmoColor);
        }
    }
}
