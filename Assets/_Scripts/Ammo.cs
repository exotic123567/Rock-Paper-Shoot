using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Ammo : NetworkBehaviour
{
    public GameObject prefab;
    private SpriteRenderer spriterend;
    public Sprite[] sprites;
    private NetworkVariable<uint> ammoType = new NetworkVariable<uint>(0); // 0 -> Rock , 1 -> Paper , 2 -> Scissor

    private void Awake() {
        spriterend = GetComponent<SpriteRenderer>();
    }
    
    public override void OnNetworkSpawn() {
        // Ensure all clients update their sprite when the sprite index changes
        ammoType.OnValueChanged += OnSpriteIndexChanged;

        // Only the server should decide the sprite
        if (IsServer) {
            AssignClassToAmmo();
        } else {
            // Sync the sprite based on ammoType.Value when clients spawn the object
            UpdateSprite(ammoType.Value);
        }

        base.OnNetworkSpawn();
    }
    private void OnDestroy() {
        // Cleanup the event listener
        ammoType.OnValueChanged -= OnSpriteIndexChanged;
    }
    private void OnSpriteIndexChanged(uint oldIndex, uint newIndex) {
        UpdateSprite(newIndex);
    }
    void UpdateSprite(uint index) {
        if (index >= 0 && index < sprites.Length) {
            spriterend.sprite = sprites[index];
            // Debug.Log("Ammo's sprite has been changed to : "+ index);
        }
        else {
            Debug.LogError("Invalid sprite index: " + index);
        }
    }
    private void OnTriggerEnter2D(Collider2D col) {
        if (!col.CompareTag("Player")) return;
        if (!NetworkManager.Singleton.IsServer) return; //Only in the server instance, updating of ammo happens
        if (col.TryGetComponent(out PlayerController plc)) {
            if (!plc.IsMaxAmmo(ammoType.Value))
            {
                plc.UpdateAmmo(ammoType.Value);
                Debug.Log("Player collided with ammo!");
                if (AudioManager.instance != null)
                {
                    AudioManager.instance.PlayAmmoPickupSound();
                }
                NetworkObject.Despawn();
                NetworkObjectPool.Singleton.ReturnNetworkObject(NetworkObject, prefab);
                
            }
        }
    }
    void AssignClassToAmmo() {
        int randomclass =  UnityEngine.Random.Range(0,sprites.Length);
        ammoType.Value = (uint)randomclass;
        //DisplayClassValueOnLog();
    }
    void DisplayClassValueOnLog() {
        Debug.Log("Ammo's class is : "+ammoType.Value);
    }
}
