using UnityEngine;

namespace EldenLib.AI.BehaviorTree
{
    public abstract class Tree : MonoBehaviour
    {
        protected Node _root = null;
        protected void Start()
        {
            _root = SetupTree();
        }

        protected void Update()
        {
            if (_root != null) _root.Evaluate();
        }

        protected abstract Node SetupTree();
    }
}
