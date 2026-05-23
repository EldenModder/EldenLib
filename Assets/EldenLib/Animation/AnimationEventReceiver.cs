using System.Collections.Generic;
using UnityEngine;

namespace EldenLib.Animation
{
    public class AnimationEventReceiver : MonoBehaviour
    {
        [SerializeField] List<AnimationEvent> _animationEvents = new();

        public void OnAnimationEventTriggered(string eventName)
        {
            AnimationEvent matchingEvent = _animationEvents.Find(se => se.eventName == eventName);
            matchingEvent?.OnAnimationEvent?.Invoke();
        }
    }
}
