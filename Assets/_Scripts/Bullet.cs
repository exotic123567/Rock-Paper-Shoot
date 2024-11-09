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
    private NetworkVariable<ulong> shooterId = new NetworkVariable<ulong>();
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
    
    // Method to set shooterId on the server when bullet is fired
    public void SetShooterId(ulong shooterClientId)
    {
        if (IsServer) // Ensure only the server sets this value
        {
            shooterId.Value = shooterClientId;
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
                plc.DecreaseHP(tempdecreaseval, shooterId.Value); 
                
                
                
                // Trigger the color change on the client side
                TriggerColorChangeClientRpc(plc.OwnerClientId, "#FF7D7D");
                Debug.Log("HP Decreased!!!");
            }
            Debug.Log("Player collided with bullet!");
        }
        BulletNetworkObjectPool.Singleton.ReturnNetworkObject(NetworkObject, prefab);
        NetworkObject.Despawn();
    }
    [ClientRpc]
    private void TriggerColorChangeClientRpc(ulong targetClientId, string hexColor)
    {
        // Get the player that needs to change color
        foreach (var player in FindObjectsOfType<PlayerController>())
        {
            if (player.OwnerClientId == targetClientId)
            {
                player.OnHit(hexColor); // Call the OnHit method to change color on client
            }
        }
    }
}
