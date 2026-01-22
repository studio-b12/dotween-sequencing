using DG.Tweening;

namespace Rehawk.DOTweenSequencing
{
    /// <summary>
    /// Represents a step in a DOTween sequence.
    /// Provides functionality to add a specific tween step into a <see cref="Sequence"/>.
    /// </summary>
    public interface ITweenStep
    {
        void AddTo(Sequence sequence);
    }
}
