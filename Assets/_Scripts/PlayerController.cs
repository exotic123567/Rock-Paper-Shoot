using System;
using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.Serialization;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Player_Ammo_Variables playerAmmo;
    [SerializeField] public float moveSpeed;
    public InputAction playerControls;
    public InputAction fireAction;
    private Rigidbody2D rb;
    Vector2 moveDirection = Vector2.zero;
    [FormerlySerializedAs("sprites")] public Sprite[] spritesForPlayerSkinColor;
    public Sprite[] spritesForHandColor;
    private SpriteRenderer spriteRendForSkinBody;
    private SpriteRenderer spriteRendForHand;
    [SerializeField] private GameObject healthbarObject;
    [SerializeField] private GameObject[] ammoPrefabs; // Reference to the ammo prefab
    [SerializeField] private Transform shootPoint;  // The point from where ammo will be fired (could be a child object)
    [SerializeField] private GameObject bsobject;
    [SerializeField] private float minX = -18f;  // Minimum X position
    [SerializeField] private float maxX = 18f;   // Maximum X position
    [SerializeField] private float minY = -18f;  // Minimum Y position
    [SerializeField] private float maxY = 18f;   // Maximum Y position


    // Object pool reference 
    [SerializeField] private NetworkObjectPool objectPool;

    // Create a NetworkVariable to store the sprite index
    private NetworkVariable<int> spriteIndex = new NetworkVariable<int>();
    private NetworkVariable<uint> rock_ammo = new NetworkVariable<uint>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<uint> paper_ammo = new NetworkVariable<uint>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<uint> scissor_ammo = new NetworkVariable<uint>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<uint> hp = new NetworkVariable<uint>(2, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<uint> maxRockAmmo = new NetworkVariable<uint>(2, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<uint> maxPaperAmmo = new NetworkVariable<uint>(2, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<uint> maxScissorAmmo = new NetworkVariable<uint>(2, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<uint> selectedAmmoType = new NetworkVariable<uint>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server); // By default, rock is chosen

    private NetworkVariable<uint> playertype = new NetworkVariable<uint>(0); // 0 -> Rock , 1 -> Paper , 2 -> Scissor
    [SerializeField] private TextMeshProUGUI rockAmmoText;
    [SerializeField] private TextMeshProUGUI paperAmmoText;
    [SerializeField] private TextMeshProUGUI scissorAmmoText;
    private BulletShooter bulletShooterScript;
    private GameObject ammoSelectionUI;
    // Reference to your joystick script
    public GameObject joystick;
    public bl_Joystick joystickerScript;
    private Vector2 movementInput;
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        spriteRendForSkinBody = GetComponent<SpriteRenderer>();
        GameObject  ChildGameObject2 = transform.GetChild (1).gameObject; // Object for skin color
        GameObject  ChildGameObject3 = transform.GetChild (2).gameObject; // Object for hand that shoots
        spriteRendForSkinBody = ChildGameObject2.GetComponent<SpriteRenderer>();
        spriteRendForHand = ChildGameObject3.GetComponent<SpriteRenderer>();
        rockAmmoText = GameObject.FindWithTag("rock_ammo_tmpro").GetComponentInChildren<TextMeshProUGUI>();
        paperAmmoText = GameObject.FindWithTag("paper_ammo_tmpro").GetComponentInChildren<TextMeshProUGUI>();
        scissorAmmoText = GameObject.FindWithTag("scissor_ammo_tmpro").GetComponentInChildren<TextMeshProUGUI>();
        bsobject = GameObject.FindWithTag("BulletShooter");
        
        //joystick = GameObject.FindWithTag("leftjoystick");  /////
        Debug.Log("Bullet shooter has been found in the scene!");
    }

    private void OnEnable() {
        playerControls.Enable();
        fireAction.Enable();
        fireAction.performed += OnFire;
        Debug.Log("Inside the OnEnable function...");
    }

    private void OnDisable() {
        playerControls.Disable();
        fireAction.Disable();
        fireAction.performed -= OnFire;
        Debug.Log("Inside the OnDisable function...");
    }
    private void OnFire(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        
        Debug.Log("Fire action triggered!");
        
        // Handle your firing logic here, like shooting or using ammo
        Shoot();
    }

    public void ShootPublicly()
    {
        Debug.Log("Fire action triggered!");
        Shoot();
    }
    private void Shoot()
    {
        // Add your shooting logic here
        // Instead, try finding the bullet shooter object in the scene 
        Debug.Log("Player is shooting!");
        bulletShooterScript = bsobject.GetComponent<BulletShooter>();
        if (selectedAmmoType.Value == 0 && rock_ammo.Value > 0)
        {
            bulletShooterScript.TryShootAmmo((int)selectedAmmoType.Value,this.gameObject);
        }
        else if (selectedAmmoType.Value == 1 && paper_ammo.Value > 0)
        {
            bulletShooterScript.TryShootAmmo((int)selectedAmmoType.Value,this.gameObject);
        }
        else if (selectedAmmoType.Value == 2 && scissor_ammo.Value > 0)
        {
            bulletShooterScript.TryShootAmmo((int)selectedAmmoType.Value,this.gameObject);
        }
    }

    public override void OnNetworkSpawn() {
        // Ensure all clients update their sprite when the sprite index changes
        spriteIndex.OnValueChanged += OnSpriteIndexChanged;
        rock_ammo.OnValueChanged += (oldValue, newValue) => UpdateInspectorValuesAndCallUpdateUI(oldValue, newValue);
        paper_ammo.OnValueChanged += (oldValue, newValue) => UpdateInspectorValuesAndCallUpdateUI(oldValue, newValue);
        scissor_ammo.OnValueChanged += (oldValue, newValue) => UpdateInspectorValuesAndCallUpdateUI(oldValue, newValue);

        // Only the server should decide the sprite
        if (IsServer) {
            AssignClassToPlayer();
            ChangeSpriteRandomly();
        }
        Debug.Log("Inside OnNetworkSpawn");

        base.OnNetworkSpawn();
    }

    private void OnDestroy() {
        spriteIndex.OnValueChanged -= OnSpriteIndexChanged;
        rock_ammo.OnValueChanged -= UpdateInspectorValuesAndCallUpdateUI;
        paper_ammo.OnValueChanged -= UpdateInspectorValuesAndCallUpdateUI;
        scissor_ammo.OnValueChanged -= UpdateInspectorValuesAndCallUpdateUI;
    }
    private void UpdateInspectorValuesAndCallUpdateUI(uint oldValue, uint newValue) {
        playerAmmo.rock_ammo = rock_ammo.Value;
        playerAmmo.paper_ammo = paper_ammo.Value;
        playerAmmo.scissor_ammo = scissor_ammo.Value;
        UpdateAmmoUI();
    }

    private void Start() {
        // Update the sprite initially on spawn
        UpdateSprite(spriteIndex.Value);
        DisplayClassValueOnLog();
        playerAmmo.rock_ammo = rock_ammo.Value;
        playerAmmo.paper_ammo = paper_ammo.Value;
        playerAmmo.scissor_ammo = scissor_ammo.Value;
        ammoSelectionUI = GameObject.FindWithTag("AmmoIndicator");
        AmmoSelectionManager amsm = ammoSelectionUI.GetComponent<AmmoSelectionManager>();
        amsm.playerController = this;
        
        //joystickerScript = joystick.GetComponent<bl_Joystick>();   ///////

        UpdateAmmoUI();
    }

    private void Update() {
        if (!IsOwner || !Application.isFocused) return;
        // Get input from the joystick
        //float horizontal = joystickerScript.Horizontal;  /////
        //float vertical = joystickerScript.Vertical; /////

        // Store the movement input
        //movementInput = new Vector2(horizontal, vertical); /////

        // Normalize the input if necessary to prevent diagonal speed boost
        //if (movementInput.magnitude > 1)  /////
        //{
        //    movementInput = movementInput.normalized;
        //}  /////
        moveDirection = playerControls.ReadValue<Vector2>();
    }

    private void FixedUpdate() {
        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector2 finalMovement = Vector2.zero;

        // Check if keyboard input (moveDirection) is active
        if (moveDirection != Vector2.zero)
        {
            finalMovement = moveDirection;  // Use keyboard input
        }
        // If no keyboard input, check for joystick input (movementInput)
        //else if (movementInput != Vector2.zero)  /////
        //{
        //    finalMovement = movementInput;  // Use joystick input
        //}

        // Calculate the new position based on the final movement direction
        Vector2 newPosition = rb.position + finalMovement * moveSpeed * Time.fixedDeltaTime;

        // Clamp the player's X and Y position within the defined boundaries
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        // Move the player to the clamped position
        rb.MovePosition(newPosition);

        // Rotate player if there is movement
        if (finalMovement != Vector2.zero)
        {
            // Calculate the angle to rotate the player towards the movement direction
            float angle = Mathf.Atan2(finalMovement.y, finalMovement.x) * Mathf.Rad2Deg;
            rb.rotation = angle - 90f;  // Adjust the rotation as needed
        }
    }
    
    Transform FindChildWithTag(Transform parent, string tag) {
    foreach (Transform child in parent) {
        if (child.CompareTag(tag)) {
            return child;
        }
    }
    return null; // If no child with the given tag is found
}


    // Server-side method to pick a random sprite
    void ChangeSpriteRandomly() {
        if (spritesForPlayerSkinColor.Length == 0) {
            Debug.LogError("No sprites found! Make sure to assign the sprites in the Inspector.");
            return;
        }

        // Pick a random index from the array of sprites
        int randomIndex = UnityEngine.Random.Range(0, spritesForPlayerSkinColor.Length);

        // Set the value of the NetworkVariable, which will trigger the OnValueChanged event
        spriteIndex.Value = randomIndex;
    }

    void AssignClassToPlayer() {
        int randomclass =  UnityEngine.Random.Range(0,3);
        playertype.Value = (uint)randomclass;
        DisplayClassValueOnLog();
    }
    void DisplayClassValueOnLog() {
        Debug.Log("Player's class is : "+playertype.Value);
    }

    // This method is called whenever the sprite index changes (for all clients)
    private void OnSpriteIndexChanged(int oldIndex, int newIndex) {
        UpdateSprite(newIndex);
    }

    // Update the sprite based on the index
    void UpdateSprite(int index) {
        if (index >= 0 && index < spritesForPlayerSkinColor.Length) {
            spriteRendForSkinBody.sprite = spritesForPlayerSkinColor[index];
            spriteRendForHand.sprite = spritesForHandColor[index];
            //Debug.Log("SpritRenderer's sprite has been changed to : "+ index);
        }
        else {
            Debug.LogError("Invalid sprite index: " + index);
        }
    }
    private void UpdateAmmoUI()
    {
        if (!IsOwner) return;
        rockAmmoText.text = $"{rock_ammo.Value}/{maxRockAmmo.Value}";
        paperAmmoText.text = $"{paper_ammo.Value}/{maxPaperAmmo.Value}";
        scissorAmmoText.text = $"{scissor_ammo.Value}/{maxScissorAmmo.Value}";
    }
    // Server side code to handle ammo updates from clients inside server instance
    public void UpdateAmmo(uint ammoType)
    {
        if (IsServer) {
            switch (ammoType) {
                case 0:
                    if (rock_ammo.Value < maxRockAmmo.Value)
                    {
                        rock_ammo.Value += 1;
                        playerAmmo.rock_ammo = rock_ammo.Value;
                    }
                    break;
                case 1:
                    if (paper_ammo.Value < maxPaperAmmo.Value)
                    {
                        paper_ammo.Value += 1;
                        playerAmmo.paper_ammo = paper_ammo.Value;
                    }
                    break;
                case 2:
                    if (scissor_ammo.Value < maxScissorAmmo.Value)
                    {
                        scissor_ammo.Value += 1;
                        playerAmmo.scissor_ammo = scissor_ammo.Value;
                    }
                    break;
                default:
                    Debug.LogError("Invalid ammo update request received");
                    break;
            }
            UpdateAmmoUI();
            Debug.Log("Ammos updated for Player!");
        }
    }

    [ServerRpc]
    public void UpdateSelectedAmmoTypeServerRpc(uint ammoType)
    {
        // Server updates the NetworkVariable (since server has write permission)
        selectedAmmoType.Value = ammoType;
        Debug.Log("Ammo type updated to: " + ammoType);
    }

    public bool IsMaxAmmo(uint ammoType)
    {
        return ammoType switch
        {
            0 => rock_ammo.Value == maxRockAmmo.Value,
            1 => paper_ammo.Value == maxPaperAmmo.Value,
            2 => scissor_ammo.Value == maxScissorAmmo.Value,
            _ => false // Default case if ammoType doesn't match 0, 1, or 2
        };
    }


    public void DecreaseHP(uint amount)
    {
        if (IsServer)
        {
            hp.Value -= amount;
            if (hp.Value <= 0)
            {
                Debug.Log("Player's HP reached 0, despawning player!");
                // Despawn the player
                DespawnPlayer();
            }
        }
        Debug.Log("HP decreased by " + amount + ", current HP: " + hp.Value);
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void DecreaseAmmoServerRpc(int ammoType)
    {
        switch (ammoType)
        {
            case 0:
                if (rock_ammo.Value > 0)
                {
                    rock_ammo.Value -= 1;
                }
                else
                {
                    Debug.LogWarning("No rock ammo left");
                }
                break;
            case 1:
                if (paper_ammo.Value > 0)
                {
                    paper_ammo.Value -= 1;
                }
                else
                {
                    Debug.LogWarning("No paper ammo left");
                }
                break;
            case 2:
                if (scissor_ammo.Value > 0)
                {
                    scissor_ammo.Value -= 1;
                }
                else
                {
                    Debug.LogWarning("No scissor ammo left");
                }
                break;
            default:
                Debug.LogError("Invalid ammo type");
                break;
        }
    }


    private void DespawnPlayer()
    {
        if (IsServer)
        {
            // Optionally, you can add some death animation or effects before despawning
            NetworkObject.Despawn();  // This despawns the player from the network
        }
    }

    public bool shouldthisbulletdefeatplayer(uint bulletTypeValue)
    {
        if (bulletTypeValue == 0 && playertype.Value == 2) return true;
        else if (bulletTypeValue == 1 && playertype.Value == 0) return true;
        else if (bulletTypeValue == 2 && playertype.Value == 1) return true;
        else return false;
    }
}
[Serializable] // Allows ammo variables to be serialized and visible in the Inspector
public class Player_Ammo_Variables
{
    public uint rock_ammo;
    public uint paper_ammo;
    public uint scissor_ammo;
}