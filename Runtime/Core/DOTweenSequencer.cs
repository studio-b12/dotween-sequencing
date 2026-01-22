using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Rehawk.DOTweenSequencing
{
    /// <summary>
    /// Builds and controls a DOTween <see cref="Sequence"/> from an inspector-authored list of <see cref="ITweenStep"/>.
    /// Supports forward/backward playback, pause/resume, restarting, killing, duration scaling (including instant play),
    /// and coroutine helpers to wait for completion or rewind.
    /// </summary>
    [AddComponentMenu("DOTween/DOTween Sequencer")]
    public class DOTweenSequencer : MonoBehaviour
    {
        [Tooltip("If true, the sequence will automatically play when this component is enabled.")]
        [SerializeField] private bool playOnEnable = false;

        [Tooltip("If true and Play On Enable is active, the sequence will restart from the beginning when enabled. If false, it will continue from its current position.")]
        [SerializeField] private bool restartOnEnable = false;
 
        [Tooltip("If true, the sequence will be automatically killed when it completes.")]
        [SerializeField] private bool autoKill = false;

        [Tooltip("If true, DOTween may recycle the internal sequence instance for better performance.")]
        [SerializeField] private bool recyclable = true;

        [Tooltip("Number of times the sequence should loop. 0 means no looping, -1 means infinite looping.")]
        [SerializeField] private int loops = 0;

        [Tooltip("Loop behavior when loops are enabled (Restart or Yoyo).")]
        [SerializeField] private LoopType loopType = LoopType.Restart;

        [Tooltip("Determines how the sequence is updated (Normal, Late, Fixed, Manual).")]
        [SerializeField] private UpdateType updateType = UpdateType.Normal;

        [Tooltip("If true, the sequence will ignore Unity's Time.timeScale.")]
        [SerializeField] private bool ignoreTimeScale = false;

        [Tooltip("Multiplies the entire sequence duration.\n2 = twice as long (slower)\n0.5 = half as long (faster)\n<= 0 = plays instantly.")]
        [SerializeField] private float durationMultiplier = 1f;
        
        [Tooltip("Ordered list of tween steps that are used to build the DOTween sequence.")]
        [SerializeReference] public List<ITweenStep> steps = new();

        [Tooltip("Invoked when the sequence starts playing.")]
        [SerializeField] private UnityEvent onStarted;

        [Tooltip("Invoked when the sequence is paused.")]
        [SerializeField] private UnityEvent onPaused;

        [Tooltip("Invoked when the sequence completes (after all loops, if any).")]
        [SerializeField] private UnityEvent onCompleted;

        [Tooltip("Invoked when a sequence step completes.")]
        [SerializeField] private UnityEvent onStepCompleted;

        [Tooltip("Invoked when the sequence is fully rewound back to time 0.")]
        [SerializeField] private UnityEvent onRewound;
        
        private Sequence sequence;
        private float internalDurationMultiplier;

        /// <summary>
        /// True while the underlying DOTween <see cref="Sequence"/> is actively playing
        /// (forward or backward). Returns false if the sequence is paused, complete,
        /// rewound, killed, or not yet built.
        /// </summary>
        public bool IsPlaying => sequence?.IsPlaying() ?? false;
        
        /// <summary>
        /// The last requested playback direction for the sequence.
        /// <para>
        /// Used to determine which direction <see cref="Resume"/> should continue in
        /// after a pause. This is updated by <see cref="Play"/> and <see cref="PlayBackwards"/>,
        /// and does not change automatically when the sequence reaches the start or end.
        /// </para>
        /// </summary>
        public PlayDirection LastDirection { get; private set; } = PlayDirection.Forward;
        
#if UNITY_EDITOR
        public float EditorElapsed => sequence?.Elapsed(includeLoops: false) ?? 0f;
        public float EditorDuration => sequence?.Duration(includeLoops: false) ?? 0f;
        public bool HasSequence => sequence != null && sequence.IsActive();
#endif

        /// <summary>
        /// Raised when the sequence starts playing (in sync with <see cref="onStarted"/>).
        /// </summary>
        public event Action Started;
        
        /// <summary>
        /// Raised when the sequence is paused (in sync with <see cref="onPaused"/>).
        /// </summary>
        public event Action Paused;
        
        /// <summary>
        /// Raised when the sequence completes (in sync with <see cref="onCompleted"/>).
        /// </summary>
        public event Action Completed;
        
        /// <summary>
        /// Raised each time the sequence completes a loop iteration (in sync with <see cref="onStepCompleted"/>).
        /// </summary>
        public event Action StepCompleted;
        
        /// <summary>
        /// Raised when the sequence is rewound back to the start (in sync with <see cref="onRewound"/>).
        /// </summary>
        public event Action Rewound;
        
        private void Awake()
        {
            internalDurationMultiplier = durationMultiplier;
        }

        private void OnEnable()
        {
            if (!playOnEnable) 
                return;

            if (sequence == null) 
                Build();

            if (restartOnEnable)
            {
                Restart();
            }
            else
            {
                Play();
            }
        }

        private void OnDestroy() => Kill();
        
        /// <summary>
        /// Sets a runtime duration multiplier for the built sequence.
        /// <para>2 = slower (double duration), 0.5 = faster (half duration), &lt;= 0 = treated as instant by Play/PlayBackwards.</para>
        /// </summary>
        public void SetDurationMultiplier(float multiplier)
        {
            internalDurationMultiplier = multiplier;
            ApplyDurationMultiplier();
        }
        
        /// <summary>
        /// (Re)builds the DOTween <see cref="Sequence"/> from the current step list and settings.
        /// <para>
        /// Kills any existing sequence, creates a new one, wires callbacks/events, applies loops and steps,
        /// then applies the current duration multiplier and pauses the result.
        /// </para>
        /// </summary>
        public void Build()
        {
            Kill();

            sequence = DOTween.Sequence()
                               .SetAutoKill(autoKill)
                               .SetRecyclable(recyclable)
                               .SetUpdate(updateType, ignoreTimeScale)
                               .OnPlay(() =>
                               {
                                   onStarted.Invoke();
                                   Started?.Invoke();
                               })
                               .OnPause(() =>
                               {
                                   onPaused.Invoke();
                                   Paused?.Invoke();
                               })
                               .OnComplete(() =>
                               {
                                   onCompleted.Invoke();
                                   Completed?.Invoke();
                               })
                               .OnStepComplete(() =>
                               {
                                   onStepCompleted.Invoke();
                                   StepCompleted?.Invoke();
                               })
                               .OnRewind(() =>
                               {
                                   onRewound.Invoke();
                                   Rewound?.Invoke();
                               })
                               .OnKill(() => sequence = null);

            if (loops != 0)
                sequence.SetLoops(loops, loopType);

            for (int i = 0; i < steps.Count; i++)
            {
                ITweenStep step = steps[i];
                step?.AddTo(sequence);
            }

            ApplyDurationMultiplier();
            
            sequence.Pause();
        }

        /// <summary>
        /// Plays the sequence forward using the serialized duration multiplier.
        /// </summary>
        /// <param name="withCallbacks">If true, firing callbacks is allowed when playing instantly (multiplier &lt;= 0).</param>
        /// <param name="restartIfComplete">If true, restarts when the sequence is already complete.</param>
        public void Play(bool withCallbacks = true, bool restartIfComplete = true) => Play(durationMultiplier, withCallbacks, restartIfComplete);
        
        /// <summary>
        /// Plays the sequence forward using a specific duration multiplier.
        /// <para>
        /// If <paramref name="multiplier"/> &lt;= 0, the sequence completes instantly.
        /// </para>
        /// </summary>
        /// <param name="multiplier">Duration multiplier (2 = slower, 0.5 = faster, &lt;= 0 = instant).</param>
        /// <param name="withCallbacks">If true, callbacks are invoked when completing instantly.</param>
        /// <param name="restartIfComplete">If true, restarts when the sequence is already complete.</param>
        public void Play(float multiplier, bool withCallbacks = true, bool restartIfComplete = true)
        {
            if (sequence == null)
                Build();

            LastDirection = PlayDirection.Forward;
            SetDurationMultiplier(multiplier);

            if (multiplier <= 0f)
            {
                sequence.Complete(withCallbacks);
                return;
            }

            if (restartIfComplete && sequence.IsComplete())
            {
                sequence.Restart(includeDelay: true);
                return;
            }

            sequence.PlayForward();
        }

        /// <summary>
        /// Plays the sequence backwards using the serialized duration multiplier.
        /// </summary>
        /// <param name="goToEndIfAtStart">If true and the sequence is at time 0, jumps to the end before playing backwards.</param>
        public void PlayBackwards(bool goToEndIfAtStart = true) => PlayBackwards(durationMultiplier, goToEndIfAtStart);
        
        /// <summary>
        /// Plays the sequence backwards using a specific duration multiplier.
        /// <para>
        /// If <paramref name="multiplier"/> &lt;= 0, the sequence rewinds instantly.
        /// </para>
        /// </summary>
        /// <param name="multiplier">Duration multiplier (2 = slower, 0.5 = faster, &lt;= 0 = instant).</param>
        /// <param name="goToEndIfAtStart">If true and the sequence is at time 0, jumps to the end before playing backwards.</param>
        public void PlayBackwards(float multiplier, bool goToEndIfAtStart = true)
        {
            if (sequence == null)
                Build();

            LastDirection = PlayDirection.Backward;
            SetDurationMultiplier(multiplier);

            if (multiplier <= 0f)
            {
                sequence.Goto(0f, andPlay: false);
                sequence.Rewind();
                return;
            }

            if (goToEndIfAtStart && Mathf.Approximately(sequence.Elapsed(), 0f))
            {
                float end = sequence.Duration(includeLoops: false);
                sequence.Goto(end, andPlay: false);
            }

            sequence.PlayBackwards();
        }
        
        /// <summary>
        /// Restarts the sequence from the beginning using the current duration multiplier.
        /// Automatically builds the sequence if needed.
        /// </summary>
        public void Restart()
        {
            if (sequence == null) 
                Build();
            
            ApplyDurationMultiplier();
            sequence.Restart();
        }

        /// <summary>
        /// Pauses the sequence if it exists.
        /// </summary>
        public void Pause() => sequence?.Pause();
        
        /// <summary>
        /// Continues playback after a pause, resuming in the last requested direction (forward or backward).
        /// <para>
        /// Does nothing if the sequence is at the end (forward) or at time 0 (backward).
        /// </para>
        /// </summary>
        public void Resume()
        {
            if (sequence == null)
                return;

            if (LastDirection == PlayDirection.Forward && sequence.IsComplete())
                return;
            
            if (LastDirection == PlayDirection.Backward && Mathf.Approximately(sequence.Elapsed(), 0f))
                return;
            
            if (LastDirection == PlayDirection.Backward)
            {
                sequence.PlayBackwards();
            }
            else
            {
                sequence.PlayForward();
            }
        }
        
        /// <summary>
        /// Ensures the sequence exists and is positioned at the start (time 0),
        /// applying start values immediately.
        /// </summary>
        public void GotoStart(bool andPlay = false)
        {
            if (sequence == null)
                Build();

            sequence.Goto(0f, andPlay);
        }

        /// <summary>
        /// Ensures the sequence exists and is positioned at the end (duration),
        /// applying end values immediately.
        /// </summary>
        public void GotoEnd(bool andPlay = false)
        {
            if (sequence == null)
                Build();

            float end = sequence.Duration(includeLoops: false);
            sequence.Goto(end, andPlay);
        }
        
        /// <summary>
        /// Kills the current sequence, if any.
        /// </summary>
        /// <param name="complete">If true, completes the tween before killing (DOTween behaviour).</param>
        public void Kill(bool complete = true)
        {
            sequence?.Kill(complete);
        }

        /// <summary>
        /// Coroutine helper that optionally builds and plays the sequence, then yields until it is complete.
        /// <para>
        /// If the effective multiplier is &lt;= 0, it completes instantly and the coroutine ends immediately.
        /// </para>
        /// </summary>
        /// <param name="autoBuild">If true, builds the sequence when missing.</param>
        /// <param name="autoPlay">If true, starts playback automatically.</param>
        /// <param name="multiplier">Optional duration multiplier override.</param>
        /// <param name="withCallbacks">If true, callbacks are invoked when completing instantly.</param>
        public IEnumerator WaitForComplete(bool autoBuild = true, bool autoPlay = true, float? multiplier = null,
                                           bool withCallbacks = true)
        {
            if (sequence == null)
            {
                if (!autoBuild)
                    yield break;

                Build();
            }

            if (sequence == null)
                yield break;

            float currentMultiplier = multiplier ?? durationMultiplier;

            if (autoPlay)
                Play(currentMultiplier, withCallbacks);

            if (currentMultiplier <= 0f)
                yield break;

            yield return WaitUntil(() => sequence.IsComplete());
        }
        
        /// <summary>
        /// Coroutine helper that optionally builds and plays the sequence backwards, then yields until it is fully rewound.
        /// <para>
        /// If the effective multiplier is &lt;= 0, it rewinds instantly and the coroutine ends immediately.
        /// </para>
        /// </summary>
        /// <param name="autoBuild">If true, builds the sequence when missing.</param>
        /// <param name="autoPlay">If true, starts backward playback automatically.</param>
        /// <param name="multiplier">Optional duration multiplier override.</param>
        public IEnumerator WaitForRewind(bool autoBuild = true, bool autoPlay = true,
                                         float? multiplier = null)
        {
            if (sequence == null)
            {
                if (!autoBuild)
                    yield break;

                Build();
            }

            if (sequence == null)
                yield break;

            float currentMultiplier = multiplier ?? durationMultiplier;

            if (autoPlay)
                PlayBackwards(currentMultiplier);

            if (currentMultiplier <= 0f)
                yield break;

            yield return WaitUntil(() => !sequence.IsPlaying() && Mathf.Approximately(sequence.Elapsed(), 0f));
        }
        
        private void ApplyDurationMultiplier()
        {
            if (sequence == null)
                return;

            // multiplier <= 0 means "instant" (we don't set timescale here; Play() handles it)
            if (internalDurationMultiplier > 0f)
            {
                sequence.timeScale = 1f / internalDurationMultiplier;
            }
            else
            {
                sequence.timeScale = 1f;
            }
        }

        private IEnumerator WaitUntil(Func<bool> predicate)
        {
            while (sequence != null && sequence.IsActive() && !predicate())
                yield return null;
        }
    }
}
