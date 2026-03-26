using System;
using UnityEngine;

namespace EldenLib.Controls.GameEditor
{
    /// <summary>
    /// Flying camera controller for editor-like apps.
    /// Attach this script to the GameObject holding the main camera
    /// Useful for level editors
    /// </summary>
    public class FlyingCameraController : MonoBehaviour
    {
        /// <summary>
        /// Controls for the camera :
        /// wasdrf : Basic Movement
        /// shift : make camera Accelerate
        /// space : moves camera on X and Z axis only. So camera doesn't gain any height
        /// q : change mode
        /// </summary>

        [Header("Camera Parameters")]
        [SerializeField, Tooltip("Mouse rotation sensitivity")]
        public float _mouseRotationSensitivity = 5.0f;
        [SerializeField, Tooltip("Regular speed")]
        public float _speed = 10.0f;
        [SerializeField, Tooltip("Gravity force")]
        public float _gravity = 20.0f;

        [Header("Acceleration Parameters")]
        [SerializeField, Tooltip("Multiply by how long the shift key is held")]
        public float _shiftAdd = 25.0f;
        [SerializeField, Tooltip("Maximum speed when holding shift")]
        public float _maxShift = 100.0f;

        [Header("Key")]
        [SerializeField, Tooltip("Which mouse key invoke camera  rotation (0 = left, 1 = right"), Range(0, 1)]
        public int _mouseKeyForRotation = 1;

        [Header("Limits")]
        [SerializeField, Tooltip("Total move")]
        public float _totalRun = 1.0f;
        [SerializeField, Tooltip("Total Rotation Y")]
        public float _rotationY = 0.0f;
        [SerializeField, Tooltip("Vertical Y Rotation Maximum")]
        public float _maximumY = 90.0f;
        [SerializeField, Tooltip("Vertical Y Rotation Minimum")]
        public float _minimumY = -90.0f;

        ///<summary>
        /// Camera Controls
        ///</summary>

        void FixedUpdate()
        {
            //rotate cam
            if (Input.GetMouseButton(_mouseKeyForRotation))
            {
                float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * _mouseRotationSensitivity;
                _rotationY += Input.GetAxis("Mouse Y") * _mouseRotationSensitivity;
                _rotationY = Mathf.Clamp(_rotationY, _minimumY, _maximumY);
                transform.localEulerAngles = new Vector3(-_rotationY, rotationX, 0.0f);
            }

            //get movement direction
            Vector3 dir = GetMovemementDirection();

            //move faster if shift held down
            if (_shiftAdd != 0 && Input.GetKey(KeyCode.LeftShift))
            {
                _totalRun += Time.deltaTime;
                dir = dir * _totalRun * _shiftAdd;
                dir.x = Mathf.Clamp(dir.x, -_maxShift, _maxShift);
                dir.y = Mathf.Clamp(dir.y, -_maxShift, _maxShift);
                dir.z = Mathf.Clamp(dir.z, -_maxShift, _maxShift);
            }
            //else move normal
            else
            {
                _totalRun = Mathf.Clamp(_totalRun * 0.5f, 1.0f, 1000.0f);
                dir = dir * _speed;
            }

            //set new position
            dir = dir * Time.deltaTime;
            transform.Translate(dir);
        }

        private Vector3 GetMovemementDirection()
        {
            //get direction from input
            Vector3 dir_velocity = new Vector3(
                Input.GetAxis("Horizontal"),
                0.0f,
                Input.GetAxis("Vertical"));

            if (Input.GetKey(KeyCode.F))  dir_velocity += new Vector3(0f, -1f, 0f); 
            if (Input.GetKey(KeyCode.R))  dir_velocity += new Vector3(0f, 1f, 0f);

            return dir_velocity;
        }

        //Reset rotation to a given-point
        public void ResetRotation(Vector3 lookAt) => transform.LookAt(lookAt);
    }
}
