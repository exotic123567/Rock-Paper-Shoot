using UnityEngine;
using Unity.Netcode;

public class FireBtnKeLiyeScript : MonoBehaviour
{
    private PlayerController localPlayerController;
    private GameObject player;

    private void Start()
    {
        // Find the player object using the Player tag
        StartCoroutine(FindLocalPlayerAfterSpawn());
    }
    private System.Collections.IEnumerator FindLocalPlayerAfterSpawn()
    {
        // Keep checking until the local player's object is found
        while (player == null)
        {
            // Find all objects with the "Player" tag
            GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");

            // Loop through all players and find the one that is owned by the local client
            foreach (GameObject playerObject in playerObjects)
            {
                NetworkBehaviour networkBehaviour = playerObject.GetComponent<NetworkBehaviour>();
                if (networkBehaviour != null && networkBehaviour.IsOwner)
                {
                    player = playerObject;
                    localPlayerController = player.GetComponent<PlayerController>(); // Get PlayerController component

                    if (localPlayerController != null)
                    {
                        Debug.Log("Local player found: " + localPlayerController.gameObject.name);
                    }
                    break; // Once we find the local player, we stop searching
                }
            }

            yield return new WaitForSeconds(0.25f); // Check every 0.25 seconds
        }
    }
    

    // Call this when you want the player to shoot
    public void ShootAmmoForLocalPlayer()
    {
        if (localPlayerController != null)
        {
            // Call the shoot function for the local player
            localPlayerController.ShootPublicly();
        }
        else
        {
            Debug.LogError("No local player available to shoot.");
        }
    }
}