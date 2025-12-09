using System.Collections;
using UnityEngine;

namespace SojaExiles
{
    public class SafeopencloseDoor : MonoBehaviour
    {
        [Header("Settings")]
        public Transform doorHinge;
        public float openAngle = -90f;
        public float openSpeed = 2f;

        [Header("Audio")]
        private AudioClip doorOpenClip;
        private AudioSource audioSource;

        private Quaternion closedRotation;
        private Quaternion targetRotation;
        private bool isOpening = false;
        private bool hasOpened = false;

        void Start()
        {
            closedRotation = doorHinge.localRotation;
            targetRotation = closedRotation;

            doorOpenClip = Resources.Load<AudioClip>("DrawersAndDoors/dooropen");
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
            audioSource.playOnAwake = false;
        }

        void Update()
        {
            if (isOpening)
            {
                doorHinge.localRotation = Quaternion.Slerp(doorHinge.localRotation, targetRotation, Time.deltaTime * openSpeed);
            }
        }

        public void OpenDoor()
        {
            if (hasOpened) return;

            targetRotation = Quaternion.Euler(doorHinge.localEulerAngles + new Vector3(0, openAngle, 0));
            isOpening = true;
            hasOpened = true;

            if (doorOpenClip)
            {
                audioSource.clip = doorOpenClip;
                audioSource.time = 0.2f;
                audioSource.Play();
            }

            Debug.Log("Safe door opening...");
        }
    }
}
