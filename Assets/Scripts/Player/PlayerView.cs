using UnityEngine;

namespace Player
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private Transform cameraLookAtPoint;

        public Transform CameraPoint => cameraLookAtPoint;
    }
}