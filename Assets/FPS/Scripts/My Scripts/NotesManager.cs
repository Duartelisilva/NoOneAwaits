using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.FPS.Gameplay;
using Unity.FPS.Game;

public class NotesVisibilityManager : MonoBehaviour
{
    public int notesLimit = 12;
    public float pickRange = 3f;
    public Transform playerCamera;
    public Renderer sigilRenderer;
    public AudioClip pickupSound;

    public ObjectiveUI objectiveUI;
    public DialogueSystem dialogueSystem;

    private Color baseEmission;
    private bool halfwayMessageShown = false;
    private bool almostThereMessageShown = false;
    private bool completedMessageShown = false;
    private bool notesObjectiveCompleted = false;

    private string[] noteTags = {
        "notes_livingroom",
        "notes_kitchen",
        "notes_kidsroom",
        "notes_doublebedroom",
        "notes_bathroom"
    };

    private Dictionary<string, List<GameObject>> notesByZone = new Dictionary<string, List<GameObject>>();
    private HashSet<GameObject> activeNotes = new HashSet<GameObject>();
    public int collectedaux;
    public int collectedCount { get; private set; } = 0;

    void Start()
    {
        CollectNotesByTags();
        SetNotesActive(false); // Only disable all notes at start, no initial spawning here

        if (sigilRenderer != null)
        {
            Color originalEmission = sigilRenderer.material.GetColor("_EmissionColor");
            baseEmission = originalEmission.maxColorComponent > 0 ? originalEmission / originalEmission.maxColorComponent : Color.white;
            sigilRenderer.material.EnableKeyword("_EMISSION");
            UpdateEmission();
        }
    }

    // Called by another script to spawn initial notes when appropriate
    public void SpawnInitialNotes()
    {
        var zones = notesByZone.Keys.ToList();

        // Determine how many notes to spawn (max 3 or less if close to limit)
        int notesToSpawn = Mathf.Min(3, notesLimit - collectedCount);

        // Active count per zone
        Dictionary<string, int> activePerZone = GetActiveNotesPerZone();

        int spawned = 0;
        while (spawned < notesToSpawn && zones.Count > 0)
        {
            // Filter zones with less than 2 active notes
            var validZones = zones.Where(z => activePerZone.ContainsKey(z) ? activePerZone[z] < 2 : true).ToList();

            if (validZones.Count == 0) break;

            string chosenZone = validZones[Random.Range(0, validZones.Count)];

            SpawnOneNoteInZone(chosenZone);

            activePerZone[chosenZone] = activePerZone.ContainsKey(chosenZone) ? activePerZone[chosenZone] + 1 : 1;

            spawned++;

            // Remove zone if reached 2 notes
            if (activePerZone[chosenZone] >= 2)
                zones.Remove(chosenZone);
        }
    }

    void Update()
    {
        collectedCount = collectedaux;

        if (Input.GetKeyDown(KeyCode.E))
            TryCollectNote();
    }

    private void CollectNotesByTags()
    {
        notesByZone.Clear();
        foreach (var tag in noteTags)
        {
            var foundNotes = GameObject.FindGameObjectsWithTag(tag).ToList();
            notesByZone[tag] = foundNotes;
            Debug.Log($"Found {foundNotes.Count} notes with tag '{tag}'");
        }
    }

    public void SetNotesActive(bool active)
    {
        foreach (var zone in notesByZone.Values)
            foreach (var note in zone)
                if (note != null)
                    note.SetActive(active);

        activeNotes.Clear();
        if (active)
        {
            foreach (var zone in notesByZone.Values)
                foreach (var note in zone)
                    if (note != null && note.activeSelf)
                        activeNotes.Add(note);
        }
    }

    private void SpawnOneNoteInZone(string zone)
    {
        if (!notesByZone.ContainsKey(zone)) return;

        var notesInZone = notesByZone[zone];
        var inactiveNotes = notesInZone.Where(n => n != null && !activeNotes.Contains(n) && !n.activeSelf).ToList();
        if (inactiveNotes.Count == 0) return;

        var noteToActivate = inactiveNotes[Random.Range(0, inactiveNotes.Count)];
        noteToActivate.SetActive(true);
        activeNotes.Add(noteToActivate);

        Debug.Log($"Spawned a note in zone '{zone}'");
    }

    private void TryCollectNote()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickRange))
        {
            GameObject hitObj = hit.collider.gameObject;

            if (activeNotes.Contains(hitObj))
            {
                string collectedZone = noteTags.FirstOrDefault(tag => hitObj.CompareTag(tag));

                Vector3 notePosition = hitObj.transform.position;

                if (pickupSound != null)
                    AudioSource.PlayClipAtPoint(pickupSound, notePosition);

                collectedCount++;
                collectedaux++;
                activeNotes.Remove(hitObj);
                hitObj.SetActive(false);

                UpdateEmission();
                UpdateObjectiveText();
                CheckMilestones();

                SpawnNoteAfterCollect(collectedZone);

                return;
            }
        }
    }

    private Dictionary<string, int> GetActiveNotesPerZone()
    {
        var dict = new Dictionary<string, int>();

        foreach (var zone in notesByZone.Keys)
        {
            int count = notesByZone[zone].Count(note => note != null && activeNotes.Contains(note));
            dict[zone] = count;
        }
        return dict;
    }

    private void SpawnNoteAfterCollect(string collectedZone)
    {
        if (collectedCount + activeNotes.Count >= notesLimit)
        {
            Debug.Log("Notes limit reached, no more notes will spawn.");
            return;
        }

        var activePerZone = GetActiveNotesPerZone();

        // Zones that are not the collectedZone, and have less than 2 active notes
        var candidateZones = notesByZone.Keys
            .Where(z => z != collectedZone && (!activePerZone.ContainsKey(z) || activePerZone[z] < 2))
            .ToList();

        if (candidateZones.Count == 0)
        {
            Debug.Log("No zones available to spawn new note respecting zone constraints.");
            return;
        }

        string zoneToSpawn = candidateZones[Random.Range(0, candidateZones.Count)];
        SpawnOneNoteInZone(zoneToSpawn);
    }

    private void UpdateEmission()
    {
        float intensity = Mathf.Clamp01(collectedCount / (float)notesLimit);
        Color emission = baseEmission * intensity;

        sigilRenderer.material.EnableKeyword("_EMISSION");
        sigilRenderer.material.SetColor("_EmissionColor", emission);
    }

    private void UpdateObjectiveText()
    {
        if (notesObjectiveCompleted) return;

        if (collectedCount < notesLimit)
        {
            objectiveUI.SetObjective($"collect all the notes ({collectedCount}/{notesLimit})");
        }
        else
        {
            objectiveUI.SetObjective("find the sigil and cast the spell");
            notesObjectiveCompleted = true;
        }
    }

    private void CheckMilestones()
    {
        int maxCount = notesLimit;

        if (!halfwayMessageShown && collectedCount >= maxCount / 2)
        {
            ShowPassiveMessage("halfway there");
            halfwayMessageShown = true;
        }
        else if (!almostThereMessageShown && collectedCount >= maxCount * 5 / 6)
        {
            ShowPassiveMessage("almost there");
            almostThereMessageShown = true;
        }
        else if (!completedMessageShown && collectedCount >= maxCount)
        {
            ShowPassiveMessage("I'm close to leaving this place...");
            completedMessageShown = true;
        }
    }

    private void ShowPassiveMessage(string message)
    {
        if (dialogueSystem != null)
        {
            dialogueSystem.ShowPassiveMessage(message);
        }
    }

    public void ResetAllNotes()
    {
        SetNotesActive(false);
        collectedCount = 0;
        collectedaux = 0;
        SpawnInitialNotes();
        UpdateEmission();
        UpdateObjectiveText();
        halfwayMessageShown = false;
        almostThereMessageShown = false;
        completedMessageShown = false;
        notesObjectiveCompleted = false;
    }
}
