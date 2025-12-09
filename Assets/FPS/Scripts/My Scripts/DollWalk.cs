using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using System.Collections;

public class WanderAndChase : MonoBehaviour
{
    public float wanderRadius = 3f;
    public float wanderInterval = 5f;
    public float sightRange = 8f;
    public float fieldOfView = 120f;
    public float loseSightDelay = 5f;
    public Transform player;
    private bool waiting = false;
    private float idleWaitTime = 0f;

    public AudioClip walkSound;
    public AudioMixerGroup mixerGroup;
    private AudioSource audioSource;
    private int walkCycleCounter = 0;
    private bool soundOnCooldown = false;

    private NavMeshAgent agent;
    private Animator animator;
    private float timer;
    private float lostSightTimer = 0f;
    public bool chasing = false;
    private Vector3 lastKnownPlayerPos;
    private bool isWalking = false;
    private bool chaseLinePlayed = false;

    public BadEndingScreen badEndingScreen;
    public static bool goodEndingTriggered = false;
    public AudioClip[] randomLines;
    public AudioClip[] chaseLines;
    public AudioClip finalLineClip; // Assign this line in Inspector
    public bool contactTriggered = false;
    public NotesVisibilityManager notescounter;
    private bool isPaused = false;
    private bool savedIsWalking = false;
    public int notesnumber = 30;
    public GameObject crosshair;
    public GameObject objectivesUI;
    public GameObject goawaylocation;
    private bool hasReachedGoAwayLocation = true;
    private int goaway = 0;

    void Start()
    {   
        if (goodEndingTriggered)
        {
            Destroy(gameObject);
            return;
        }

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource != null && mixerGroup != null)
        {
            audioSource.outputAudioMixerGroup = mixerGroup;
            audioSource.spatialBlend = 1f; // 3D sound
            audioSource.spatialize = true;
        }

        agent.speed *= 1f + 0.5f * notescounter.collectedCount / (notescounter.notesLimit + 4);
        timer = wanderInterval;
    }

    void Update()
    {   
        wanderInterval = 5 - 3 * notescounter.collectedCount / (notescounter.notesLimit + 4);

        if (!GameSettings.isHardMode)
        sightRange = 8 + 4 * notescounter.collectedCount / (notescounter.notesLimit + 4);

        wanderRadius = 20;

        if (!GameSettings.isHardMode)
        fieldOfView = 240 + 120 * notescounter.collectedCount / (notescounter.notesLimit + 4);
        else fieldOfView = 360;

        if (isPaused) 
        {
            agent.isStopped = true;
            animator.SetBool("GoWalk", false);
            animator.SetBool("GoIdle", true);
            return;
        }
        
        agent.isStopped = false;
        if (contactTriggered)
        return;
        
        if (goodEndingTriggered)
        {
            Destroy(gameObject);
            return;
        }

        bool wasWalking = isWalking;

        if (goaway >= 2)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                // Arrived at goawaylocation
                goaway = 0;
                waiting = true;  // reset wandering
                timer = 0f;
                hasReachedGoAwayLocation = true;
            }
            else
            {
                agent.SetDestination(goawaylocation.transform.position);
                hasReachedGoAwayLocation = false;
            }
        }
        else
        {
            if (CanSeePlayer())
            {
                lastKnownPlayerPos = player.position;
                agent.SetDestination(lastKnownPlayerPos);

                if (!chasing)
                {
                    // Just started chasing
                    chasing = true;
                    goaway++;
                    if (!chaseLinePlayed && chaseLines.Length > 0 && audioSource != null && !audioSource.isPlaying && hasReachedGoAwayLocation)
                    {
                        AudioClip randomClip = chaseLines[Random.Range(0, chaseLines.Length)];
                        audioSource.PlayOneShot(randomClip);
                        chaseLinePlayed = true;
                    }


                }

                lostSightTimer = 0f;
            }

            else if (chasing && !CanSeePlayer()) // mudei (falta verificar)
            {
                lostSightTimer += Time.deltaTime;

                if (lostSightTimer < loseSightDelay)
                {
                    agent.SetDestination(lastKnownPlayerPos);
                }
                else
                {
                    chasing = false;
                    chaseLinePlayed = false; // allow line to play again next time
                    lostSightTimer = 0f;
                    timer = wanderInterval;
                }
            }

            if (!chasing)
            {
                if (waiting)
                {
                    idleWaitTime -= Time.deltaTime;
                    if (idleWaitTime <= 0f)
                    {
                        Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, NavMesh.AllAreas);
                        agent.SetDestination(newPos);
                        waiting = false;
                        timer = 0f;
                    }
                }
                else
                {
                    timer += Time.deltaTime;
                    if (timer >= wanderInterval)
                    {
                        waiting = true;
                        idleWaitTime = Random.Range(0f, wanderInterval);
                        timer = 0f;
                    }
                }
            }
        }

        RotateTowardMovement();

        isWalking = agent.remainingDistance > agent.stoppingDistance && agent.hasPath;

        if (isWalking != wasWalking)
        {
            animator.SetBool("GoWalk", isWalking);
            animator.SetBool("GoIdle", !isWalking);

            if (isWalking && !chasing) // Only play walking lines if NOT chasing
            {
                walkCycleCounter++;

                if (!soundOnCooldown && randomLines.Length > 0 && audioSource != null && !audioSource.isPlaying)
                {
                    AudioClip randomClip = randomLines[Random.Range(0, randomLines.Length)];
                    audioSource.PlayOneShot(randomClip);
                    soundOnCooldown = true;
                    walkCycleCounter = 0;
                }
            }
            else if (soundOnCooldown && walkCycleCounter >= 2)
            {
                soundOnCooldown = false;
            }
        }
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 dirToPlayer = player.position - transform.position;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (dirToPlayer.magnitude <= sightRange && angle <= fieldOfView * 0.5f)
        {
            if (Physics.Raycast(transform.position + Vector3.up, dirToPlayer.normalized, out RaycastHit hit, sightRange))
            {
                if (hit.transform == player)
                    return true;
            }
        }
        return false;
    }

    Vector3 RandomNavSphere(Vector3 origin, float maxDist, int layermask)
    {
        float minDist = 2f; // Set your minimum distance here

        for (int i = 0; i < 30; i++)
        {
            Vector3 randDirection = Random.insideUnitSphere * maxDist;
            randDirection.y = 0;
            randDirection += origin;

            if (Vector3.Distance(origin, randDirection) >= minDist &&
                NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, maxDist, layermask))
            {
                return navHit.position;
            }
        }

        // Fallback if no valid point found
        return origin;
    }


    void RotateTowardMovement()
    {
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion rotation = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (goodEndingTriggered || contactTriggered) return;

        if (other.CompareTag("Player") && badEndingScreen != null)
        {
            contactTriggered = true;
            crosshair.SetActive(false);
            objectivesUI.SetActive(false);
            StartCoroutine(PlayFinalLineAfterDelay());
            badEndingScreen.ShowBadEnding();
        }
    }

    private IEnumerator PlayFinalLineAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        if (audioSource != null && finalLineClip != null)
        {
            if (!audioSource.isPlaying)
                audioSource.PlayOneShot(finalLineClip);

        }
    }
    
        public void SetPaused(bool paused)
        {
            if (isPaused == paused) return;

            isPaused = paused;
            if (paused)
            {
                savedIsWalking = isWalking;
                agent.isStopped = true;
                animator.SetBool("GoWalk", false);
                animator.SetBool("GoIdle", true);
            }
            else
            {
                agent.isStopped = false;
                animator.SetBool("GoWalk", savedIsWalking);
                animator.SetBool("GoIdle", !savedIsWalking);
            }
        }

}
