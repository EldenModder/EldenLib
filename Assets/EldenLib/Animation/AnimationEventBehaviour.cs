using System;
using UnityEngine;

namespace EldenLib.Animation
{
    public class AnimationEventBehaviour : StateMachineBehaviour
    {
        public string eventName;
        [Range(0f, 1f)] public float triggerTime;
        bool hasTriggered;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            hasTriggered = false;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            float currentTime = stateInfo.normalizedTime * 1f;
            if (!hasTriggered && currentTime >= triggerTime)
            {
                NotifyReceiver(animator);
                hasTriggered = true;
            }
        }

        private void NotifyReceiver(Animator animator)
        {
            AnimationEventReceiver receiver = animator.GetComponent<AnimationEventReceiver>();
            receiver?.OnAnimationEventTriggered(eventName);
        }
    }
}
