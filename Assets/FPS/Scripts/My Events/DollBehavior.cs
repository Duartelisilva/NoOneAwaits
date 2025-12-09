using UnityEngine;
using System.Collections;

public class DollController : MonoBehaviour
{
    public Animator animator;
    public AudioClip creepyVoiceClip;
    private AudioSource audioSource;

    private Vector3 startPos;
    private Vector3 targetPos;
    private float moveTimer = 0f;
    private float moveDuration = 2f;
    private float moveHeight = 0.4f;
    private bool isMoving = false;

    public bool startLoop = false;
    public int aux = 0;

    public LoopingAudioController loopingAudioController;  // Assign in Inspector

    private SkinnedMeshRenderer[] skinnedRenderers;

    void Start()
    {
        if (animator != null)
            animator.applyRootMotion = false;

        startPos = transform.position;
        targetPos = startPos + new Vector3(0, moveHeight, 0);

        skinnedRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (var renderer in skinnedRenderers)
        {
            foreach (var mat in renderer.materials)
            {
                if (mat.HasProperty("_Color"))
                {
                    Color c = mat.color;
                    c.a = 1f;
                    mat.color = c;
                }
            }
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
    }

    public void TriggerInteraction()
    {
        // Stop current looping audio (fade out) immediately on interaction
        if (loopingAudioController != null)
            loopingAudioController.fadeDuration = 1f; // or shorter duration
            loopingAudioController.StopAudio();

        FindFirstObjectByType<DollAuxiliary>()?.FadeFlashlightSequenceAndEnd();

        startPos = transform.position;
        targetPos = startPos + new Vector3(0, moveHeight, 0);
        moveTimer = 0f;
        isMoving = true;

        if (animator != null)
            animator.SetTrigger("ReactNow");
    }

    void Update()
    {
        if (isMoving)
        {
            moveTimer += Time.deltaTime;
            float t = Mathf.Clamp01(moveTimer / moveDuration);
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            aux++;

            if (aux > 50)
                startLoop = true;

            if (t >= 1f)
            {
                isMoving = false;
                StartCoroutine(StopLoopAfterSeconds(3f));
            }
        }
    }

    private IEnumerator StopLoopAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        startLoop = false;
        // Do NOT start new ambience here; DollAuxiliary will handle that before disabling doll
    }
}
