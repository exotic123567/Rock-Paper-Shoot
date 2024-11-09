using UnityEngine;

public class GameStartInitializer : MonoBehaviour
{
    public GameObject textWriterControllerObject; // Assign the TextWriterController GameObject in the Inspector

    private void Awake()
    {
        textWriterControllerObject.SetActive(false); // Disable at start
    }

    // Call this method when the player is initialized on the server
    public void OnPlayerInitialized(GameObject player)
    {
        uint playerType = player.GetComponent<PlayerController>().playerclasspublicvalue;
        textWriterControllerObject.SetActive(true); // Enable the TextWriterController GameObject

        // Pass the player class type to the TextWriterController
        var textWriterController = textWriterControllerObject.GetComponent<TextWriterController>();
        textWriterController.InitializeTextWriters(playerType);

        
    }
}