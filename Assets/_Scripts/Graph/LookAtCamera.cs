using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARLuft
{
    public class LookAtCamera : MonoBehaviour
    {
        public float MaxDistanceToLook = 2;
        public bool InverseForward;

        private Transform cameraTransform;

        private void Update()
        {
            if (!cameraTransform)
            {
                cameraTransform = Camera.main.transform;
                return;
            }

            float distance = Vector3.Distance(cameraTransform.position, transform.position);
            if (distance < MaxDistanceToLook)
            {
                Vector3 dir = transform.position - cameraTransform.position;
                Quaternion lookAtRotation = Quaternion.LookRotation(dir);
                int inverse = InverseForward ? 180 : 0;
                Quaternion lookAtRotationOnly_Y = Quaternion.Euler(transform.rotation.eulerAngles.x, lookAtRotation.eulerAngles.y + inverse, transform.rotation.eulerAngles.z);
                transform.rotation = lookAtRotationOnly_Y;
            }
        }
    }
}
