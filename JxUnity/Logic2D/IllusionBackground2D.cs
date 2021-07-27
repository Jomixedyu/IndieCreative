using UnityEngine;

namespace JxUnity.Logic2D
{
    public class IllusionBackground2D : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("如为空，则使用主摄像机")]
        Transform mcamera;

        [SerializeField]
        [Tooltip("启用后distance将生效，不再使用z轴作为距离")]
        bool enableDistanceField = false;

        [SerializeField]
        [Range(0, 10)]
        [Tooltip("背景距离，需要使enableDistanceField = true才可生效")]
        float distance = 0f;

        Vector3 origin;

        private void Awake()
        {
            if (mcamera == null)
            {
                mcamera = Camera.main.transform;
            }
            origin = transform.position;
        }

        void Update()
        {
            float z = enableDistanceField ? distance : origin.z;
            Vector3 v = origin + (origin - mcamera.position) * z;
            v.z = origin.z;
            transform.position = v;
        }
    }
}