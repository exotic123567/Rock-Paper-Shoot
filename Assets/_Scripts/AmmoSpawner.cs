using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AmmoSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject prefab;
    private const int MaxPrefabCount = 50;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Debug.Log("Inside AmmoSpawner OnNetworkSpawn function...");
            SpawnAmmoStart();
        }
    }

    private void SpawnAmmoStart() 
    {
        NetworkObjectPool.Singleton.InitializePool();
        for (int i = 0; i < 30; i++) 
        {
            SpawnAmmo();
        }
        StartCoroutine(SpawnOverTime());
    }

    private void SpawnAmmo() 
    {
        if (NetworkObjectPool.Singleton == null)
        {
            Debug.LogError("NetworkObjectPool is not initialized!");
            return;
        }
        NetworkObject obj = NetworkObjectPool.Singleton.GetNetworkObject(prefab, GetRandomPositionOnMap(), Quaternion.identity);
        if (obj == null)
        {
            Debug.LogError("Failed to get a NetworkObject from the pool.");
            return;
        }
        obj.GetComponent<Ammo>().prefab = prefab;
        if (!obj.IsSpawned) obj.Spawn(true);
    }

    private Vector3 GetRandomPositionOnMap() 
    {
        return new Vector3(Random.Range(-17f, 17f), Random.Range(-17f, 17f), 0f);
    }

    private IEnumerator SpawnOverTime() 
    {
        while (NetworkManager.Singleton.IsServer && NetworkManager.Singleton.IsListening) 
        {
            yield return new WaitForSeconds(2f);
            if (NetworkObjectPool.Singleton.GetCurrentPrefabCount(prefab) < MaxPrefabCount) 
            {
                SpawnAmmo();
            }
        }
    }
}