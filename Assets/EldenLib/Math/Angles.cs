using UnityEngine;

namespace EldenLib.Math
{
    /// <summary>
    /// Math Angles Utilities
    /// </summary>
    public class Angles
    {
        /// <summary>
        /// Get Angle between two points, without Y axis
        /// </summary>
        /// <param name="from">First Point.</param>
        /// <param name="to">Second Point</param>
        /// <returns>Angle between points</returns>
        public static float AngleXZ(Vector3 from, Vector3 to)
        {
            Vector3 p1 = new Vector2 (from.x, from.z);
            Vector3 p2 = new Vector2 (to.x, to.z);
            return Angle(p1, p2);
        }

        /// <summary>
        /// Get angle between two 2d points.
        /// </summary>
        /// <param name="p1">First point.</param>
        /// <param name="p2">Second point.</param>
        /// <returns>Angle between points.</returns>
        public static float Angle(Vector2 p1, Vector2 p2)
            => Mathf.Atan2(p2.y - p1.y, p2.x - p1.x) * 180 / Mathf.PI;

        /// <summary>
        /// Get avarage between two angles.
        /// Note: this also represent the angle directly between two other angles.
        /// </summary>
        /// <returns>Avarage value between two angles.</returns>
        public float AngleAvg(float a, float b) => Mathf.LerpAngle(a, b, 0.5f);
    }
}
