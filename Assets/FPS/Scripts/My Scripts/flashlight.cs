using UnityEngine;
using System.Collections;

namespace Unity.FPS.Gameplay
{
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerFlashlightManager : MonoBehaviour
    {
        [Tooltip("The flashlight GameObject (should include the model and the Light component)")]
        public GameObject FlashlightObject;

        [Tooltip("Reference to the Light component on the flashlight")]
        public Light FlashlightLight;

        [Tooltip("Transform where the flashlight is attached (usually camera or hand)")]
        public Transform FlashlightSocket;

        [Tooltip("Key to toggle the flashlight")]
        public KeyCode ToggleKey = KeyCode.F;

        [Tooltip("Sound to play when toggling the flashlight")]
        public AudioClip ToggleSound;

        [Header("References")]
        public FlashlightRayPickup flashlightflag;
        public DialogueSystem dialogueSystem; // ← Assign in Inspector

        AudioSource m_AudioSource;
        PlayerInputHandler m_InputHandler;
        bool m_IsFlashlightOn;
        private bool flashlightok = false;
        private bool messageShown = false;
        public GameObject crosshair;
        
        void Start()
        {
            m_InputHandler = GetComponent<PlayerInputHandler>();

            m_AudioSource = gameObject.AddComponent<AudioSource>();
            m_AudioSource.playOnAwake = false;

            if (FlashlightObject != null)
            {
                FlashlightLight.enabled = false;
                m_IsFlashlightOn = false;
            }
        }

        void Update()
        {
            if (flashlightflag.flashlightpicked)
            {
                flashlightok = true;

                if (!messageShown && dialogueSystem != null)
                {
                    dialogueSystem.StartDialogue(new string[] {
                        "A flashlight... it's not very powerful but it'll surely help."
                    });
                    messageShown = true;
                }
            }

            if (Input.GetKeyDown(ToggleKey) && crosshair.activeSelf)
            {   
                m_IsFlashlightOn = !m_IsFlashlightOn;
                if (FlashlightLight != null)
                {
                    FlashlightLight.enabled = m_IsFlashlightOn;

                    if (flashlightok && ToggleSound != null)
                        m_AudioSource.PlayOneShot(ToggleSound);
                }
            }
        }

        IEnumerator FlashlightRedFadeRoutine(Light flashlightLight, float totalDuration = 4f, float fadeColorDuration = 2f, float fadeIntensityDuration = 2f, float darkDuration = 1f)
        {
            if (flashlightLight == null)
                yield break;

            Color originalColor = flashlightLight.color;
            float originalIntensity = flashlightLight.intensity;

            float elapsed = 0f;
            while (elapsed < fadeColorDuration)
            {
                elapsed += Time.deltaTime;
                flashlightLight.color = Color.Lerp(originalColor, Color.red, elapsed / fadeColorDuration);
                yield return null;
            }
            flashlightLight.color = Color.red;

            elapsed = 0f;
            while (elapsed < fadeIntensityDuration)
            {
                elapsed += Time.deltaTime;
                flashlightLight.intensity = Mathf.Lerp(originalIntensity, 0f, elapsed / fadeIntensityDuration);
                yield return null;
            }
            flashlightLight.intensity = 0f;

            yield return new WaitForSeconds(darkDuration);

            flashlightLight.color = originalColor;
            flashlightLight.intensity = originalIntensity;
        }
    }
}
