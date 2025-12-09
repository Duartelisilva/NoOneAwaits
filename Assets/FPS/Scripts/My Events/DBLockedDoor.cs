using System.Collections;
using UnityEngine;

namespace SojaExiles
{
    public class DBLockedDoor : MonoBehaviour
    {
        public Animator openandclose;
        public bool open = false;
        public Transform Player;

        public FirstEventTrigger eventTrigger;
        private bool eventTriggered = false;
        private bool showingLockedDialogue = false;

        private bool hasKey = false;
        public LightIntensityController lightController;

        public ObjectiveUI objectiveUI;
        public LoopingAudioController audioController;

        private AudioClip doorOpenClip;
        private AudioClip doorCloseClip;
        private AudioSource audioSource;

        public DialogueSystem dialogueSystem;
        public Lockdown ld;
        private bool firstTimeOpenedWithKey = false;
        private bool isBusy = false;

        void Start()
        {
            open = false;
            openandclose.enabled = false;

            if (objectiveUI == null)
                objectiveUI = FindFirstObjectByType<ObjectiveUI>();

            if (audioController == null)
                audioController = FindFirstObjectByType<LoopingAudioController>();

            doorOpenClip = Resources.Load<AudioClip>("DrawersAndDoors/dooropen");
            doorCloseClip = Resources.Load<AudioClip>("DrawersAndDoors/doorclose");

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
            audioSource.playOnAwake = false;
        }

        void Update()
        {  
            if(ld.lockdownActive)
            openandclose.enabled = true;
            if (showingLockedDialogue && Input.GetKeyDown(KeyCode.E))
            {
                showingLockedDialogue = false;
                StartCoroutine(HandleLockedDoorEvent());
            }
        }

        void OnMouseOver()
        {   
            if (isBusy || Player == null) return;
            {
                float dist = Vector3.Distance(Player.position, transform.position);
                if (dist < 15 && Input.GetKeyDown(KeyCode.E))
                {
                    if (hasKey || ld.lockdownActive)
                    {
                        if (!open)
                            StartCoroutine(opening());
                        else
                            StartCoroutine(closing());
                    }
                    else if (!eventTriggered && !showingLockedDialogue && dialogueSystem != null)
                    {
                        showingLockedDialogue = true;
                        dialogueSystem.StartDialogue(new string[] { "It's locked..." });
                    }
                }
            }
        }

        IEnumerator HandleLockedDoorEvent()
        {
            // Movement should already be disabled by DialogueSystem
            eventTrigger.TriggerEvent();
            eventTriggered = true;
            lightController?.SetRedLevel(25f);
            audioController?.SetIntensity(0f);

            yield return new WaitForSeconds(4f);
            objectiveUI?.SetObjective("investigate the sound source");
            if (dialogueSystem != null)
            {
                dialogueSystem.StartDialogue(new string[] { "Wh-what was that?" });
            }
        }

        public void SetHasKey()
        {
            hasKey = true;
            open = false;
            openandclose.enabled = true;
            openandclose.Play("Closing");
        }

        IEnumerator opening()
        {   
            isBusy = true;
            openandclose.Play("Opening");
            open = true;

            if (!firstTimeOpenedWithKey && hasKey)
            {
                firstTimeOpenedWithKey = true;
                //objectiveUI?.SetObjective("explore the bedroom");
            }

            if (doorOpenClip && !audioSource.isPlaying)
            {
                audioSource.clip = doorOpenClip;
                audioSource.time = 0.2f;
                audioSource.Play();
            }

            yield return new WaitForSeconds(1f);
            isBusy = false;
        }


        IEnumerator closing()
        {   
            isBusy = true;
            openandclose.Play("Closing");
            open = false;

            yield return new WaitForSeconds(0.5f);
            if (doorCloseClip && !audioSource.isPlaying)
            {
                audioSource.clip = doorCloseClip;
                audioSource.Play();
            }

            yield return new WaitForSeconds(0.5f);
            isBusy = false;
        }


        public IEnumerator silentclosing()
        {   
            if(!open) 
            yield break;

            isBusy = true;
            openandclose.Play("Closing");
            open = false;

            yield return new WaitForSeconds(0.5f);

            yield return new WaitForSeconds(0.5f);
            isBusy = false;
        }
    }
}
