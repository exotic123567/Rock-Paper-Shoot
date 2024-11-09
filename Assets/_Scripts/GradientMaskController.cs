using Unity.Netcode;
using UnityEngine;

public class GradientMaskController : MonoBehaviour
{
    public Transform player;  // Reference to the player
    public SpriteRenderer mask; // Reference to the radial gradient mask (Sprite Renderer)

    [SerializeField] private float boundaryRadius = 8f; // Radius of the visible area
    [SerializeField] private float fadeRadius = 5f;      // Radius for gradient fade-out effect

    private Color initialMaskColor; // Store initial color to reset alpha

    private void Start()
    {
        StartCoroutine(FindLocalPlayerAfterSpawn());
        // Get the initial color of the mask
        initialMaskColor = mask.color;
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
                    player = playerObject.transform;
                    break; // Stop searching once we find the local player
                }
            }

            yield return new WaitForSeconds(0.25f); // Check every 0.25 seconds
        }
    }

    private void Update()
    {
        if (player != null)
        {
            // Position the mask to follow the player
            mask.transform.position = player.position;

            // Update the boundary visibility based on player distance
            UpdateBoundaryVisibility();
        }
    }

    private void UpdateBoundaryVisibility()
    {
        // Calculate the distance between the player and the center of the boundary
        Vector3 playerPosition = new Vector3(player.position.x, player.position.y, 0f);
        Vector3 maskCenter = new Vector3(mask.transform.position.x, mask.transform.position.y, 0f);
        float distanceToBoundary = Vector3.Distance(playerPosition, maskCenter);

        // Calculate alpha based on the distance to create a fading effect
        float alpha = Mathf.Clamp01((boundaryRadius - distanceToBoundary) / fadeRadius);
        Color maskColor = initialMaskColor;
        maskColor.a = alpha;
        mask.color = maskColor;
    }
}
