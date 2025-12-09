using UnityEngine;

public class PlayerLocationTracker : MonoBehaviour
{
    public enum PlayerLocationState
    {
        None,
        LivingRoom,
        Kitchen,
        Attic,
        DBBedroom,
        KidsBedroom,
        Bathroom,
        Closet
        // Add more as needed
    }

    public PlayerLocationState playerIsIn = PlayerLocationState.None;

    // This method needs to be called when player enters a PlayerZone
    public void UpdatePlayerLocation(GameObject playerZone)
    {
        PlayerLocationState previousLocation = playerIsIn;

        string zoneName = playerZone.name;

        if (zoneName == "LivingRoom1" || zoneName == "LivingRoom2" || zoneName == "LivingRoom3")
            playerIsIn = PlayerLocationState.LivingRoom;
        else if (zoneName == "Kitchen")
            playerIsIn = PlayerLocationState.Kitchen;
        else if (zoneName == "Attic")
            playerIsIn = PlayerLocationState.Attic;
        else if (zoneName == "DBBedroom")
            playerIsIn = PlayerLocationState.DBBedroom;
        else if (zoneName == "KidsRoomHitbox")
            playerIsIn = PlayerLocationState.KidsBedroom;
        else if (zoneName == "Bathroom")
            playerIsIn = PlayerLocationState.Bathroom;
        else if (zoneName == "DBCloset" || zoneName == "LivingRoomCloset" || zoneName == "LivingRoomCloset2")
            playerIsIn = PlayerLocationState.Closet;
        else 
            playerIsIn = PlayerLocationState.None;

        if (playerIsIn != previousLocation)
            Debug.Log($"Player changed room to: {playerIsIn}");
    }
}
