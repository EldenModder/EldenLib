using UnityEngine;

namespace EldenLib.Controls.GameEditor
{
    /// <summary>
    /// A script to hold objects "in hands" and place them around the level using the mouse controls.
    /// To use this script attach it to your camera object and set the 'DefaultObjectToPlace' 
    /// public property for the starting type,
    /// or call SetObjectToPlace() to change object to place at runtime
    /// </summary>
    public class ObjectsPlacer : MonoBehaviour
    {
        [Header("Object")]
        [Tooltip("Starting object to place (used to set from Unity's editor).")]
        public GameObject DefaultObjectToPlace;
        [Tooltip("The object we place around the level when clicking.")]
        private GameObject ObjectToPlace;

        [Tooltip("If value > 0, will snap objects position to this grid size.")]
        public float SnapToGrid = 0.25f;

        [Tooltip("If true, will snap to grid also on Y axis (if not will only snap on X and Z).")]
        public bool SnapToGridOnAxisY = false;

        [Tooltip("If true, will debug-draw ray from camera to target.")]
        public bool ShowDebugRay = true;

        [Tooltip("If true, will clone object when placing it. " +
            "If false, will place the object only once and drop it.\r\n")]
        public bool CloneObjectOnPlacement = true;

        [Tooltip("Which mouse key will place the object")]
        public int MouseKeyToPlaceObject = 0;

        [Tooltip("Special layer we use to prevent collision and interaction " +
            "with other objects while we hold this object in hands.\r\n" +
            "If you use this layer for anything else, please pick a different layer," + 
            "otherwise the preview object might push it.")]
        public int NoCollisionLayerId = 31;

        [Tooltip("Original layer of the object-in-hands")]
        private int prevLayer = 0;

        [Tooltip("// Filter with which layers we are allowed to collide with when placing " +
            "the object.\r\n " +
            "For example, if you only want to be able to place objects only on floor and not on other objects," +
            " set this to the floor layer id.")]
        public int TargetsToPlaceOnLayersFilter = int.MaxValue;

        [Tooltip("The max renderers extent height of the object in hand.")]
        private float targetMaxExtentHeight = 0f;

        [Tooltip("Store the previous state of the preview object detection collision")]
        private bool prevRigidBodyDetectCollision = false;

        [Tooltip("Where to hold the object on Y axis, in percents, when playing it" +
            "If equals 1f will hold the object from its bottom point\r\n" +
            "If 0.5, from it's center" +
            "If 0.0, from it's top" )]
        public float PivotY = 1f;

        [Tooltip("if true, will adjust position Y while placing objects based on pivot.")]
        public bool AdjustPositionFromPivotY = false;

        /// <summary>
        /// Start this instance.
        /// </summary>
        public void Start()
        {
            // if we start with an object type to place, instanciate it
            if (DefaultObjectToPlace != null)
            {
                SetObjectToPlace(DefaultObjectToPlace, true);
            }
        }

        /// <summary>
        /// Set the object type we are currently placing.
        /// </summary>
        /// <param name="target">New object to place.</param>
		/// <param name="isPrefab">If true, it means this object is a prefab and we need to instanciate it first.</param>
		public void SetObjectToPlace(GameObject target, bool isPrefab)
        {
            // set the new object we need to place
            ObjectToPlace = target;

            // if we got a prefab, instanciate it
            if (ObjectToPlace) ObjectToPlace = Instantiate(ObjectToPlace);
            else Debug.LogWarning("No object to set !");

            // make the object in preview mode
            TurnToPreviewMode(ObjectToPlace);
        }

        /// <summary>
        /// Make an object in preview mode, which means:
        /// 	1. it won't collide with anything.
        /// 	2. we store its extent height for internal usage.
        /// 	3. its collision layer will be set to unused.
        /// Undo this action with RestoreFromPreviewMode();
        /// </summary>
        /// <param name="target">Target to make preview mode.</param>
        private void TurnToPreviewMode(GameObject target)
        {
            // set the object to the unused layer so it won't interact with stuff
            prevLayer = ObjectToPlace.layer;
            ObjectToPlace.layer = NoCollisionLayerId;

            // get the extent height of the object based on its renderer and child renderers
            // we get the extent height so we know where to place the preview object
            targetMaxExtentHeight = 0f;
            Renderer[] childRenderers = ObjectToPlace.GetComponentsInChildren<Renderer>();
            foreach (var renderer in childRenderers)
            {
                var curr = renderer.bounds.extents.y;
                targetMaxExtentHeight = Mathf.Max(targetMaxExtentHeight, curr);
            }

            // disable rigid body
            Rigidbody rigidBody = ObjectToPlace.GetComponent<Rigidbody>();
            if (rigidBody != null)
            {
                prevRigidBodyDetectCollision = rigidBody.detectCollisions;
                rigidBody.detectCollisions = false;
            }

            // disable all object colliders
            foreach (Collider collider in ObjectToPlace.GetComponentsInChildren<Collider>())
            {
                collider.enabled = false;
            }
        }

        /// <summary>
        /// Restores an object from preview mode (assuming it was the last object to turn into preview mode by this ObjectsPlacer).
        /// </summary>
        /// <param name="target">Target object to restore from preview mode.</param>
        private void RestoreFromPreviewMode(GameObject target)
        {
            // restore original layer
            target.layer = prevLayer;

            // restore original rigid body collision detection state
            Rigidbody rigidBody = target.GetComponent<Rigidbody>();
            if (rigidBody != null)
            {
                rigidBody.detectCollisions = prevRigidBodyDetectCollision;
            }

            // enable all object colliders
            foreach (Collider collider in target.GetComponentsInChildren<Collider>())
            {
                collider.enabled = true;
            }
        }

        /// <summary>
        /// Place down the object currently held in hands.
        /// </summary>
        /// <param name="clone">If true, will clone the object-in-hand but keep on holding it. If false, will just place down the object we held.</param>
        public void PlaceObject(GameObject target, bool clone = false)
        {
            // clone target
            if (clone)
            {
                target = Instantiate(target);
            }

            // cancel preview mode
            RestoreFromPreviewMode(target);
        }

        /// <summary>
        /// Handle objects placement.
        /// </summary>
        void Update()
        {
            // if no object type to place - skip.
            if (ObjectToPlace == null)
            {
                return;
            }

            // get ray from camera to mouse position
            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);

            // debug draw ray
            if (ShowDebugRay)
            {
                Debug.DrawRay(ray.origin, ray.direction, Color.green);
            }

            // test if we hit a valid target we can place object on:
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 1000f, ~(1 << NoCollisionLayerId) & TargetsToPlaceOnLayersFilter))
            {
                // get collision point
                Vector3 collisionPoint = hitInfo.point;

                // snap to grid
                if (SnapToGrid > 0)
                {
                    collisionPoint = new Vector3(
                        Mathf.Round(collisionPoint.x / SnapToGrid) * SnapToGrid,
                        SnapToGridOnAxisY ? Mathf.Round(collisionPoint.y / SnapToGrid) * SnapToGrid : collisionPoint.y,
                        Mathf.Round(collisionPoint.z / SnapToGrid) * SnapToGrid);
                }

                // update the position of the object-in-hand, to make it show where you are going to place it
                float extraY = AdjustPositionFromPivotY ? -targetMaxExtentHeight + targetMaxExtentHeight * (PivotY * 2f) : 0f;
                ObjectToPlace.transform.position = collisionPoint + new Vector3(0, extraY, 0);
                ObjectToPlace.transform.rotation = Quaternion.identity;

                // if click, leave object where we placed it
                if (Input.GetMouseButtonUp(MouseKeyToPlaceObject))
                {
                    // place object
                    PlaceObject(ObjectToPlace, CloneObjectOnPlacement);

                    // if we don't clone, lose pointer to object type
                    if (!CloneObjectOnPlacement)
                    {
                        ObjectToPlace = null;
                    }
                }
            }
        }
    }
}