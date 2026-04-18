using UnityEngine;

namespace EldenLib.StateMachine
{
    public abstract class BaseState : IState
    {
        protected readonly Transform player;
        protected readonly Animator animator;

        protected static readonly int locomotionHash = Animator.StringToHash("Locomotion");

        protected const float crossFadeDuration = 0.1f;

        protected BaseState(Transform player, Animator animator)
        {
            this.player = player;
            this.animator = animator;
        }
        public virtual void FixedUpdate()
        {
            //noop
        }

        public virtual void OnEnter()
        {
            //noop
        }

        public virtual void OnExit()
        {
            //noop
        }

        public virtual void Update()
        {
            //noop
        }
    }
}
