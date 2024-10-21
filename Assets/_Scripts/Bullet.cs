using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public GameObject prefab;
    [SerializeField] public int bulltypepublic;
    private NetworkVariable<uint> bulletType = new NetworkVariable<uint>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private void Awake() {
        
    }
    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
    }
    private void OnDestroy() {
        
    }
    private void Start()
    {
        if (IsServer)
        {
            bulletType.Value = (uint)bulltypepublic;
            StartCoroutine(DeleteAfterSomeTime());
        }
    }

    private IEnumerator DeleteAfterSomeTime()
    {
        Debug.Log("Inside Delete after some time");
        yield return new WaitForSeconds(2f);
        
        BulletNetworkObjectPool.Singleton.ReturnNetworkObject(NetworkObject, prefab);
        NetworkObject.Despawn();
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (!col.CompareTag("Player")) return;
        if (!NetworkManager.Singleton.IsServer) return; //Only in the server instance, updating of ammo happens
        if (col.TryGetComponent(out PlayerController plc))
        {
            uint tempdecreaseval = 1;
            if (plc.shouldthisbulletdefeatplayer(bulletType.Value))
            {
                plc.DecreaseHP(tempdecreaseval); 
                Debug.Log("HP Decreased!!!");
            }
            Debug.Log("Player collided with bullet!");
        }
        BulletNetworkObjectPool.Singleton.ReturnNetworkObject(NetworkObject, prefab);
        NetworkObject.Despawn();
    }
}
