using System;
using UnityEngine.Events;

namespace EldenLib.Animation
{
    [Serializable]
    public class AnimationEvent
    {
        public string eventName;
        public UnityEvent OnAnimationEvent;
    }
}
