using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AmmoSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    private const int MaxPrefabCount = 50;
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.OnServerStarted += SpawnAmmoStart;
    }

    private void SpawnAmmoStart() {
        NetworkManager.Singleton.OnServerStarted -= SpawnAmmoStart;
        NetworkObjectPool.Singleton.InitializePool();
        for (int i =0; i< 30; i++) {
            SpawnAmmo();
        }
        StartCoroutine(SpawnOverTime());
    }
    private void SpawnAmmo() {
        NetworkObject obj = NetworkObjectPool.Singleton.GetNetworkObject(prefab, GetRandomPositionOnMap(),
        Quaternion.identity);
        obj.GetComponent<Ammo>().prefab = prefab;
        if (!obj.IsSpawned) obj.Spawn(true);
    }
    private Vector3 GetRandomPositionOnMap() {
        return new Vector3(Random.Range(-17f, 17f), Random.Range(-17f, 17f), 0f);
    }
    private IEnumerator SpawnOverTime() {
        while (NetworkManager.Singleton.ConnectedClients.Count > 0) {
            yield return new WaitForSeconds(2f);
            if (NetworkObjectPool.Singleton.GetCurrentPrefabCount(prefab) < MaxPrefabCount) {
                SpawnAmmo();
            }
            
        }
    }
}
