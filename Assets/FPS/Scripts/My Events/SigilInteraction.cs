using UnityEngine;

public class SigilSelfInteract : MonoBehaviour
{
    public GameObject keyObject;
    public float fadeDuration = 2f;
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.E;

    private Material material;
    private Color originalColor;
    //private bool isFading = false;
    private Transform playerCamera;
    public NotesVisibilityManager NotesVisibilityManager;

    private AudioClip sigilSound;
    private AudioSource audioSource;

    public DialogueSystem dialogueSystem;

    public bool interactionTriggered = false;
    public WanderAndChase dollAI; // assign in inspector or find in Start()
    public ObjectiveUI objectiveUI;  
    public GameObject finalkey2;
    void Start()
    {
        material = GetComponent<Renderer>().material;
        originalColor = material.color;
        material.EnableKeyword("_EMISSION");
        if (keyObject != null)
            keyObject.SetActive(false);

        playerCamera = Camera.main.transform;

        sigilSound = Resources.Load<AudioClip>("SpecialEffects/magicsigil");
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
    }

    void Update()
    {
        if (/*isFading || */interactionTriggered || (NotesVisibilityManager.collectedCount < NotesVisibilityManager.notesLimit)) 
        {
            return;
        }

        else if (Input.GetKeyDown(interactKey))
        {
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            {
                if (hit.transform == transform)
                {
                    interactionTriggered = true;

                    if (dialogueSystem != null)
                    {
                        dollAI?.SetPaused(true);  // pause doll movement

                        string[] lines = new string[]
                        {
                            "Alright, time to cast the spell.",
                            "Alakazoo, shimmer and shine,",
                            "Twist the shadows, cross the line.",
                            "By ancient words, the key appears,",
                            "Unlock the door, dispel your fears.",
                            "...",
                            "...",
                            "Let's hope this works..."
                        };

                        dialogueSystem.OnDialogueComplete += () => {
                            dollAI?.SetPaused(false); // resume doll movement
                            BeginFadeSequence();
                        };
                        dialogueSystem.StartDialogue(lines);
                    }
                    else
                    {
                        BeginFadeSequence();
                    }
                }
            }
        }
    }

    void BeginFadeSequence()
    {
        if (dialogueSystem != null)
            dialogueSystem.OnDialogueComplete -= BeginFadeSequence;

        StartCoroutine(FadeOut());
    }

    System.Collections.IEnumerator FadeOut()
    {
        //isFading = true;
        float timer = 0f;

        if (sigilSound)
        {
            audioSource.clip = sigilSound;
            audioSource.Play();
        }

        Color baseEmission = material.GetColor("_EmissionColor");

        while (timer < fadeDuration)
        {
            float intensity = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            material.SetColor("_EmissionColor", baseEmission * intensity);
            timer += Time.deltaTime;
            yield return null;
        }

        /*Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;*/

        material.SetColor("_EmissionColor", baseEmission * 0f);

        if (keyObject != null)
            keyObject.SetActive(true);
            finalkey2.SetActive(true);
            objectiveUI.SetObjective("pick the key and leave");
        
        //isFading = false;
    }

    
}
