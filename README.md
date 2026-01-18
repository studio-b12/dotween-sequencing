# DOTween Sequencer
[![License: MIT](https://img.shields.io/badge/License-MIT-brightgreen.svg)](LICENSE)

**Inspector-driven** DOTween sequence builder with **reusable**, **type-safe** tween steps and advanced **editor tooling**.

<img width="504" height="705" alt="image" src="https://github.com/user-attachments/assets/961dc08e-6b77-4f13-8696-5071d1fa95d8" />

## 📥 Installation

#### Install via git URL

Open the Package Manager in Unity and choose Add package from git URL, then enter:

```
https://github.com/rehavvk/dotween-sequencing.git
```

from the `Add package from git URL` option.

## ▶️ Plain DOTween vs DOTween Sequencer Playback

### Plain DOTween

When using DOTween directly:

```csharp
sequence.Play();
sequence.PlayBackwards();
```

- You must manually manage:
  - Restarting when complete 
  - Jumping to end before playing backwards 
  - Direction-aware resume logic
- Inspector authoring is not supported
- Reuse requires custom code or prefabs

### DOTween Sequencer

DOTween Sequencer wraps these behaviors into **explicit, predictable semantics**:

`Play()`
- Plays the sequence **forward**
- Optionally restarts if already complete
- Supports **instant playback** via duration multiplier ≤ 0
- Automatically rebuilds the sequence if needed

`PlayBackwards()`
- Plays the sequence **backward**
- Automatically jumps to the end if currently at time 0
- Supports instant rewind
- Safe to call repeatedly

`Pause()` / `Resume()`
- `Pause()` stops playback without changing direction
- `Resume()` continues **in the last requested direction**
- No need to track direction manually

> In short: **you express intent, the sequencer handles edge cases.**

## 🧩 DOTweenSequencer Component Overview

The **DOTweenSequencer** component builds and controls a DOTween `Sequence` from an inspector-authored list of steps.

### Playback Settings

| Setting               | Description                                        |
| --------------------- | -------------------------------------------------- |
| **Play On Enable**    | Automatically plays when the GameObject is enabled |
| **Restart On Enable** | Restarts instead of resuming when enabled          |

### Sequence Settings

| Setting                 | Description                                         |
| ----------------------- | --------------------------------------------------- |
| **Auto Kill**           | Kills the sequence when completed                   |
| **Recyclable**          | Allows DOTween to recycle the sequence instance     |
| **Loops**               | Number of loops (0 = none, -1 = infinite)           |
| **Loop Type**           | Restart or Yoyo                                     |
| **Update Type**         | Normal, Late, Fixed, or Manual                      |
| **Ignore Time Scale**   | Runs independently of `Time.timeScale`              |
| **Duration Multiplier** | Scales the entire sequence duration (≤ 0 = instant) |

### Events

Each event is available both as:

- **UnityEvent** (Inspector)
- **C# event** (Code)

| Event             | Fired when               |
| ----------------- | ------------------------ |
| **Started**       | Sequence starts playing  |
| **Paused**        | Sequence is paused       |
| **Completed**     | Sequence fully completes |
| **StepCompleted** | A step completes         |
| **Rewound**       | Sequence reaches time 0  |

## 🧱 Tween Steps

A **TweenStep** is a small, reusable unit that knows how to add itself to a DOTween `Sequence`.

Each step:
- Is serializable
- Has a target type
- Decides how it is added to the sequence (Append / Join)
- Encapsulates exactly one tween action

This makes steps:

- Reusable
- Testable
- Editor-friendly

## ➕ Creating Custom TweenSteps

You can add your own steps by implementing `ITweenStep` or one of the provided tween base classes.

```csharp
[Serializable]
[TweenStepPath("Transform/Rotate")]
public class TransformRotateStep : TweenStepBase<Transform>
{
    [SerializeField] private Vector3 endValue;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private RotateMode rotateMode = RotateMode.Fast;

    protected override Tween CreateTween()
    {
        if (!TryGetTarget(out Transform transform))
            return null;

        return transform.DORotate(endValue, duration, rotateMode);
    }
}
```

### Tween Base Classes

| Base Type                            | Target | Ease / Relative | Typical Use                          |
| ------------------------------------ |--------|-----------------| ------------------------------------ |
| `TweenStepBase`                      | ❌      | ❌               | Intervals, callbacks                 |
| `TweenStepBase<TTarget>`             | ✅      | ❌               | Targeted steps without tween options |
| `TweenStepWithTweenOptions`          | ❌      | ✅               | Virtual or value-based tweens        |
| `TweenStepWithTweenOptions<TTarget>` | ✅      | ✅               | Most animation steps                 |

## 🔌 DOTween Plugins Support

DOTween Sequencer provides **ready-made** TweenSteps for several official DOTween plugins.
These steps are thin, type-safe wrappers around the same extension methods you would normally call in code.

### Supported plugin step packages

| DOTween Plugin  | Description                                           |
|-----------------|-------------------------------------------------------|
| **Audio**       | Volume fades, pitch changes, and audio-related tweens |
| **Physics**     | Rigidbody position, rotation, and force-based tweens  |
| **Physics2D**   | Rigidbody2D movement and rotation tweens              |
| **Sprites**     | SpriteRenderer color and alpha tweens                 |
| **TextMeshPro** | TMP text color, alpha, and text-related tweens        |
| **UI**          | Canvas Group and co                                   |

> [!IMPORTANT]
> Some plugins will require the [**PRO**](https://dotween.demigiant.com/pro.php) version of DOTween.

---

***Happy tweening with DOTween Sequencer!***
