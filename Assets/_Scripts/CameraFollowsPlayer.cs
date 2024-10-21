using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CameraFollowsPlayer : MonoBehaviour
{
    public Transform target; // The player’s transform
    public Vector3 offset; // Offset from the player's position
    public float smoothSpeed = 0.125f; // Smooth camera movement speed
    // Start is called before the first frame update

    private void Start()
    {
        // Try to find the player after they have spawned
        StartCoroutine(FindPlayerAfterSpawn());
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }

    // Optionally look at the target for a top-down game, you may skip this part if not needed
    void LookAtTarget()
    {
        transform.LookAt(target);
    }

    private System.Collections.IEnumerator FindPlayerAfterSpawn()
    {
        while (target == null)
        {
            // Find all objects with the "Player" tag
            GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");

            // Loop through all players and find the one that is owned by the local client
            foreach (GameObject playerObject in playerObjects)
            {
                NetworkBehaviour networkBehaviour = playerObject.GetComponent<NetworkBehaviour>();
                if (networkBehaviour != null && networkBehaviour.IsOwner)
                {
                    target = playerObject.transform;
                    break; // Once we find the local player, we stop searching
                }
            }

            yield return new WaitForSeconds(0.25f); // Check every 0.25 seconds
        }
    }
}
