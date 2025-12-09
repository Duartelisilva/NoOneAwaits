using UnityEngine;

public class RoomTrigger : MonoBehaviour
{   
    bool firsttime = false;
    public LightIntensityController lightController;  // Reference to your light script
    public string playerTag = "Player";
    public float redlevel;
    public LoopingAudioController audioController;
    public ObjectiveUI objectiveUI;
    public Lockdown ld;
    void OnTriggerEnter(Collider other)
    {   
        if(!firsttime && !ld.lockdownActive)
        {
            if (other.CompareTag(playerTag))
            {
                // Trigger your event here
                objectiveUI.SetObjective("explore the bedroom");
                lightController.SetRedLevel(redlevel);  // Example: increase red level to 50%
                lightController.SetAllLightsIntensity(0);
                Debug.Log("Player entered the room, event triggered.");
                firsttime = true;
                audioController.SetIntensity(1f);


            }
        }
    }
}
