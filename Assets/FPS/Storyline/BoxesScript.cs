using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    [Header("Settings")]
    public float typingSpeed = 0.01f;
    public float inputDelayAfterTypingStart = 0.1f; // Delay before input is accepted after typing starts

    private string[] dialogueChunks;
    private int currentChunkIndex;
    private bool isTyping;
    private Coroutine typingCoroutine;
    private Coroutine passiveRoutine;
    private float inputDelayTimer = 0f;
    public System.Action OnDialogueComplete;
    public bool gamebegin = true;
    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(string[] chunks)
    {
        if (passiveRoutine != null)
        {
            StopCoroutine(passiveRoutine);
            passiveRoutine = null;
            dialoguePanel.SetActive(false);
        }

        dialogueChunks = chunks;
        currentChunkIndex = 0;
        dialoguePanel.SetActive(true);
        DisablePlayerMovement(true);
        ShowTextChunk();
    }

    void Update()
    {
        if (!dialoguePanel.activeSelf) return;

        if (inputDelayTimer > 0f)
        {
            inputDelayTimer -= Time.deltaTime;
            return; // Ignore input during delay
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = dialogueChunks[currentChunkIndex];
                isTyping = false;
            }
            else
            {
                currentChunkIndex++;
                if (currentChunkIndex < dialogueChunks.Length)
                    ShowTextChunk();
                else
                    EndDialogue();
            }
        }
    }

    void ShowTextChunk()
    {
        inputDelayTimer = inputDelayAfterTypingStart;
        typingCoroutine = StartCoroutine(TypeText(dialogueChunks[currentChunkIndex]));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        dialogueChunks = null;

        if(!gamebegin)
        DisablePlayerMovement(false);
        
        OnDialogueComplete?.Invoke();
    }


    public void DisablePlayerMovement(bool disable)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        var movement = player.GetComponent("PlayerCharacterController") as MonoBehaviour;
        var look = player.GetComponent("PlayerInputHandler") as MonoBehaviour;

        if (movement != null) movement.enabled = !disable;
        if (look != null) look.enabled = !disable;
    }

    public bool DialogueIsActive => dialoguePanel.activeSelf && !isTyping;

    public void ShowPassiveMessage(string message)
    {
        if (!dialoguePanel.activeSelf || (dialoguePanel.activeSelf && !isTyping && dialogueChunks == null))
        {
            if (passiveRoutine != null)
            {
                StopCoroutine(passiveRoutine);
                passiveRoutine = null;
            }
            passiveRoutine = StartCoroutine(PassiveMessageRoutine(message));
        }
    }

    private IEnumerator PassiveMessageRoutine(string message)
    {
        dialoguePanel.SetActive(true);
        dialogueText.text = "";
        isTyping = true;

        foreach (char c in message)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        yield return new WaitForSeconds(2f);

        dialoguePanel.SetActive(false);
        passiveRoutine = null;
    }
}
