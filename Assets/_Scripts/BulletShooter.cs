using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class BulletShooter : NetworkBehaviour
{
    [SerializeField] private GameObject[] prefabs; // element 1-> Rock bullet, element 2 -> Paper bullet, element 3 -> Scissor bullet

    private int bulletSpeed = 3;
    private Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnServerStarted += initpool;
        }
    }

    private void initpool()
    {
        NetworkManager.Singleton.OnServerStarted -= initpool;
        BulletNetworkObjectPool.Singleton.InitializePool();
    }

    public void TryShootAmmo(int ammoTypeIndex, GameObject playerObject)
    {
        
        Vector3 shootingDirection = playerObject.transform.right;
        Vector3 playerPosition = playerObject.transform.position;
        
        // Get the shooter's NetworkObjectId to identify the player who fired
        ulong shooterId = playerObject.GetComponent<NetworkObject>().NetworkObjectId;

        // Client requests the server to spawn the bullet
        ShootAmmoServerRpc(ammoTypeIndex, playerPosition, shootingDirection, shooterId);
        if (playerObject.TryGetComponent(out PlayerController plc))
        {
            plc.DecreaseAmmoServerRpc(ammoTypeIndex);
        }
        
    }
    [ServerRpc(RequireOwnership = false)]
    private void ShootAmmoServerRpc(int ammoTypeIndex, Vector3 playerPosition, Vector3 shootingDirection, ulong shooterId)
    {
        // This runs only on the server
        ShootAmmo(ammoTypeIndex, playerPosition, shootingDirection, shooterId);
        PlayShooterSoundClientRPC();
    }
    private void ShootAmmo(int ammoTypeIndex, Vector3 playerPosition, Vector3 shootingDirection, ulong shooterId)
    {
        if (!IsServer) return;  // Ensure only the server spawns bullets

        float spawnDistance = 1.0f;  // Adjust spawn distance from the player
        Vector3 spawnPosition = playerPosition + shootingDirection * spawnDistance;

        // Use the bullet pool to spawn a bullet object
        NetworkObject obj = BulletNetworkObjectPool.Singleton.GetNetworkObject(prefabs[ammoTypeIndex], spawnPosition, Quaternion.identity);

        if (!obj.IsSpawned)
        {
            obj.Spawn(true);  // Spawn bullet on the server and replicate to all clients
        }

        // Set the prefab reference in the bullet script
        Bullet bullet = obj.GetComponent<Bullet>();
        bullet.prefab = prefabs[ammoTypeIndex];

        // Assign the shooterId to the bullet
        bullet.SetShooterId(shooterId);

        // Apply velocity to the bullet
        Rigidbody2D bulletRb = obj.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.velocity = shootingDirection * bulletSpeed;
        }

        Debug.Log("Bullet has been shot by player with ID: " + shooterId);
    }

    [ClientRpc]
    private void PlayShooterSoundClientRPC()
    {
        if (!IsOwner) return;
        if (AudioManager.instance != null) {
            AudioManager.instance.PlayBulletshooterSound();
        }
    }
}
