using UnityEngine;
using DG.Tweening;

namespace Common
{
    public class Crown : MonoBehaviour
    {

        private void Update()
        {
            transform.DORotate(new Vector3(0, 0, 10), 0.5f, RotateMode.LocalAxisAdd);
        }
    }
}