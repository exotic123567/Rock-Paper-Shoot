using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InitialIdleBackgroundScene : MonoBehaviour
{
    public GameObject backgroundTilePrefab; // The tile prefab
    public Transform player; // Reference to the player
    public int rows = 5; // Number of rows of tiles
    public int columns = 5; // Number of columns of tiles
    public float tileSize = 1.28f; // Size of the tile (128x128 pixels in Unity units)

    private GameObject[,] tiles;

    void Start()
    {
        StartCoroutine(FindLocalPlayersClassAfterSpawn());
        // Create a grid of background tiles
        tiles = new GameObject[rows, columns];

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector3 position = new Vector3(x * tileSize, y * tileSize, 1);
                GameObject tile = Instantiate(backgroundTilePrefab, position, Quaternion.identity);
                tile.transform.parent = transform; // Keep tiles under one parent for better organization
                tiles[y, x] = tile;
            }
        }
    }
    private Vector3 GetRandomPositionOnMap() {
        return new Vector3(Random.Range(-17f, 17f), Random.Range(-17f, 17f), 0f);
    }

    void Update()
    {
        if (player != null)
        {
            // Continuously update the tile positions based on player movement
            RepositionTiles();
        }
    }

    void RepositionTiles()
    {
        // Get the player's current position
        Vector2 playerPos = player.position;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector3 tilePosition = tiles[y, x].transform.position;

                // Check if tile is too far to the left or right, and reposition it
                if (tilePosition.x < playerPos.x - (columns / 2 * tileSize))
                {
                    tilePosition.x += columns * tileSize;
                }
                else if (tilePosition.x > playerPos.x + (columns / 2 * tileSize))
                {
                    tilePosition.x -= columns * tileSize;
                }

                // Check if tile is too far up or down, and reposition it
                if (tilePosition.y < playerPos.y - (rows / 2 * tileSize))
                {
                    tilePosition.y += rows * tileSize;
                }
                else if (tilePosition.y > playerPos.y + (rows / 2 * tileSize))
                {
                    tilePosition.y -= rows * tileSize;
                }

                // Apply the new tile position
                tiles[y, x].transform.position = tilePosition;
            }
        }
    }

    private System.Collections.IEnumerator FindLocalPlayersClassAfterSpawn()
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
                    break; // Once we find the local player, we stop searching
                }
            }

            yield return new WaitForSeconds(0.25f); // Check every 0.25 seconds
        }
    }
}
