using System.Collections;
using UnityEngine;

namespace EldenLib.Camera.Effect
{
    /// <summary>
    /// Camera Shake
    /// </summary>
    public class CameraShake2D : MonoBehaviour
    {
        public static CameraShake2D Instance;

        void Awake()
        {
            Instance = this;
        }

        public IEnumerator ScreenShake(float duration, float magnitude)
        {
            Vector3 originalPos = new Vector3(0, 0, -1);
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                float Xoffset = Random.Range(-0.5f, 0.5f) * magnitude;
                float Yoffset = Random.Range(-0.5f, 0.5f) * magnitude;
                transform.localPosition = new Vector3 (Xoffset, Yoffset, -1);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.localPosition = originalPos;
        }
    }
}
