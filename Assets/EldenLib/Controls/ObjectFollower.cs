using UnityEngine;

namespace EldenLib.Controls
{
    /// <summary>
    /// Make the object follow another one and look at it, with optional offsets and smooths movement
    /// </summary>
    public class ObjectFollower : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField, Tooltip("Target to follow")] 
        public GameObject _targetToFollow;

        [Header("Offset Parameter")]
        [SerializeField, Tooltip("Position offset to keep from target")] 
        public Vector3 _positionOffset = Vector3.zero;
        [SerializeField, Tooltip("LookAt offset from target (will look at target position + this offset)")]
        public Vector3 _lookAtOffset = Vector3.zero;

        [Header("Control Parameters")]
        [SerializeField, Tooltip("if true, will rotate to look at target (turn to false to disable rotation")]
        public bool _controlRotation = true;
        [SerializeField, Tooltip("if true, will move to target position (turn to false to disable movement")]
        public bool _controlPosition = true;

        [Header("Damping Parameters")]
        [SerializeField, Tooltip("if value > 0f, will lerp toward target position with this factor as damping time")]
        public float _positionDampingTime = 0.25f;
        [SerializeField, Tooltip("if value > 0f, will lerp rotation to look at target with this factor as damping time")]
        public float _rotationDampingTime = 0.25f;
        [SerializeField, Tooltip("if true, we will init offset and rotation from starting transform (will override PositionOffset)")]
        public bool _setOffsetFromTransform = true;

        //movement velocity for damping
        private Vector3 _moveVelocity = Vector3.zero;

        /// <summary>
        /// init controller
        /// </summary>
        void Start()
        {
            // init offset and look at offset from starting transform
            if (_setOffsetFromTransform) 
            {
                _positionOffset = transform.position - _targetToFollow.transform.position;
            }        
        }

        //follow target
        void FixedUpdate()
        {
            if (_controlPosition)
            {
                //get target position
                Vector3 targetPos = _targetToFollow.transform.position + _positionOffset;

                //move to target
                transform.position = _positionDampingTime > 0f ?
                    Vector3.SmoothDamp(transform.position, targetPos, ref _moveVelocity, _positionDampingTime) :
                    targetPos;

                //prevent shaking
                if ((transform.position - targetPos).magnitude < 0.05f)
                    transform.position = targetPos;
            }

            if (_controlRotation)
            {
                //get target rotation
                Quaternion targetRotation = Quaternion.LookRotation((
                    _targetToFollow.transform.position + _lookAtOffset) - transform.position);

                //rotate to look at target
                transform.rotation = _rotationDampingTime > 0 ?
                    Quaternion.Slerp(transform.rotation, targetRotation, (1f / _rotationDampingTime) * Time.deltaTime) :
                    targetRotation;
            }
        }
    }
}
