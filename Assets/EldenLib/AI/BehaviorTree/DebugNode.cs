using EldenLib.AI.BehaviorTree;
using UnityEngine;

namespace EldenLib
{
    public class DebugNode : Node
    {
        private string _message;

        public DebugNode(string message) => this._message = message;

        public override EldenLib.AI.BehaviorTree.NodeState Evaluate()
        {
            Debug.Log($"<color=#35E88C>Debug Node : {_message}</color>");
            return NodeState.SUCCESS;
        }
    }
}
