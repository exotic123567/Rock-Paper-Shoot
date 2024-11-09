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
    [FormerlySerializedAs("playerAmmoAndHp")] [FormerlySerializedAs("playerAmmo")] [SerializeField] private PlayerAttributeVariables playerAttribute;
    [SerializeField] public float moveSpeed = 3f;
    private NetworkVariable<float> moveSpeedsynchronized = new NetworkVariable<float>(3f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
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
    private NetworkVariable<uint> playerScore = new NetworkVariable<uint>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> isInitialized = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private NetworkVariable<uint> playertype = new NetworkVariable<uint>(0); // 0 -> Rock , 1 -> Paper , 2 -> Scissor
    [SerializeField] public uint playerclasspublicvalue;
    [SerializeField] private TextMeshProUGUI rockAmmoText;
    [SerializeField] private TextMeshProUGUI paperAmmoText;
    [SerializeField] private TextMeshProUGUI scissorAmmoText;
    private BulletShooter bulletShooterScript;
    [SerializeField] private Color originalColor; // Store original player color
    [SerializeField] private Color hitHueColor;// Store hit player color
    public float colorChangeDuration = 0.25f; // Duration of the color change
    [SerializeField] private GameObject audioManager;
    private GameObject ammoSelectionUI;

    private GameObject playerClassDisplayUI;
    [SerializeField] private TextMeshProUGUI playerScoreText;
    // Reference to your joystick script
    public GameObject joystick;
    public VariableJoystick joystickerScript;
    private Vector2 movementInput;
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        spriteRendForSkinBody = GetComponent<SpriteRenderer>();
        GameObject  ChildGameObject2 = transform.GetChild (1).gameObject; // Object for skin color
        GameObject  ChildGameObject3 = transform.GetChild (2).gameObject; // Object for hand that shoots
        spriteRendForSkinBody = ChildGameObject2.GetComponent<SpriteRenderer>();
        originalColor = spriteRendForSkinBody.color;
        spriteRendForHand = ChildGameObject3.GetComponent<SpriteRenderer>();
        rockAmmoText = GameObject.FindWithTag("rock_ammo_tmpro").GetComponentInChildren<TextMeshProUGUI>();
        paperAmmoText = GameObject.FindWithTag("paper_ammo_tmpro").GetComponentInChildren<TextMeshProUGUI>();
        scissorAmmoText = GameObject.FindWithTag("scissor_ammo_tmpro").GetComponentInChildren<TextMeshProUGUI>();
        bsobject = GameObject.FindWithTag("BulletShooter");
        audioManager = GameObject.FindWithTag("AudioManager");
        playerClassDisplayUI = GameObject.FindWithTag("InitialClassDisplayer");
        playerScoreText = GameObject.FindWithTag("ScoreText").GetComponentInChildren<TextMeshProUGUI>();
        
        joystick = GameObject.FindWithTag("leftjoystick");  /////
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
    
    // Function to handle color change when hit
    public void OnHit(string hexColor)
    {
        if (spriteRendForSkinBody != null)
        {
            StopAllCoroutines(); // Stop any running color change coroutine
            StartCoroutine(SmoothColorChange(hexColor));
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayHitSound();
            }
        }
    }

    private IEnumerator SmoothColorChange(string hexColor)
    {
        Debug.Log("Inside the SmoothColorChange Coroutine...");
        
        //float elapsedTime = 0;
        // Smoothly change from the original color to the hit color
        //while (elapsedTime < colorChangeDuration)
        //{
        //    spriteRendForSkinBody.color = Color.Lerp(originalColor, hitHueColor, elapsedTime / colorChangeDuration);
        //    elapsedTime += Time.deltaTime;
        //    yield return null;
        //}
        spriteRendForSkinBody.color = Color.red; // Ensure final color is set to hit color

        // Revert to original color after delay
        yield return new WaitForSeconds(0.2f);

        //elapsedTime = 0;
        // Smoothly revert back to the original color
        //while (elapsedTime < colorChangeDuration)
        //{
        //    spriteRendForSkinBody.color = Color.Lerp(hitHueColor, originalColor, elapsedTime / colorChangeDuration);
        //    elapsedTime += Time.deltaTime;
        //    yield return null;
        //}
        spriteRendForSkinBody.color = originalColor; // Ensure final color is set to original
        Debug.Log("Color changed OnHit!");
        
    }



    public override void OnNetworkSpawn() {
        // Ensure all clients update their sprite when the sprite index changes
        spriteIndex.OnValueChanged += OnSpriteIndexChanged;
        rock_ammo.OnValueChanged += (oldValue, newValue) => UpdateInspectorValuesAndCallUpdateUI(oldValue, newValue);
        paper_ammo.OnValueChanged += (oldValue, newValue) => UpdateInspectorValuesAndCallUpdateUI(oldValue, newValue);
        scissor_ammo.OnValueChanged += (oldValue, newValue) => UpdateInspectorValuesAndCallUpdateUI(oldValue, newValue);
        playerScore.OnValueChanged += (oldValue, newValue) => UpdateInspectorScorePoints(oldValue, newValue);

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
        playerAttribute.rock_ammo = rock_ammo.Value;
        playerAttribute.paper_ammo = paper_ammo.Value;
        playerAttribute.scissor_ammo = scissor_ammo.Value;
        UpdateAmmoUI();
    }
    private void UpdateInspectorScorePoints(uint oldValue, uint newValue) {
        playerAttribute.playerscore = playerScore.Value;
        UpdateScoreLocally();
        //UpdateScoreUI();
    }

    private void Start() {
        // Update the sprite initially on spawn
        UpdateSprite(spriteIndex.Value);
        DisplayClassValueOnLog();
        UpdateScoreLocally();
        playerAttribute.rock_ammo = rock_ammo.Value;
        playerAttribute.paper_ammo = paper_ammo.Value;
        playerAttribute.scissor_ammo = scissor_ammo.Value;
        playerAttribute.playerscore = playerScore.Value;
        // Optionally set `moveSpeed` on the server if needed.
        if (IsServer)
        {
            moveSpeedsynchronized.Value = 3f; // Or any desired value
        }

        if (IsOwner)
        {
            // Start the coroutine to check for initialization
            StartCoroutine(WaitForInitializationAndShowcase());
        
            ammoSelectionUI = GameObject.FindWithTag("AmmoIndicator");
            AmmoSelectionManager amsm = ammoSelectionUI.GetComponent<AmmoSelectionManager>();
            amsm.playerController = this;
        
            joystickerScript = joystick.GetComponent<VariableJoystick>();  /////
        }
        
        joystickerScript = joystick.GetComponent<VariableJoystick>();   ///////

        UpdateAmmoUI();
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayBGMusic();
        }
        healthbarObject = GameObject.FindWithTag("HealthBar");
    }
    
    // Server Authoritative movement code...
    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // FixedUpdate logic now resides in the server-side RPC
        if (!IsOwner || !Application.isFocused) return;

        // Get input from the joystick
        float horizontal = joystickerScript.Horizontal;
        float vertical = joystickerScript.Vertical;

        // Store the movement input
        movementInput = new Vector2(horizontal, vertical);

        // Normalize the input if necessary to prevent diagonal speed boost
        if (movementInput.magnitude > 1)
        {
            movementInput = movementInput.normalized;
        }

        // Read keyboard input
        moveDirection = playerControls.ReadValue<Vector2>();

        // Send input to the server for processing
        if (moveDirection != Vector2.zero || movementInput != Vector2.zero)
        {
            Vector2 inputToSend = moveDirection != Vector2.zero ? moveDirection : movementInput;
            SendMovementInputServerRpc(inputToSend);
        }
    }

    // ServerRpc to receive movement input from the client
    [ServerRpc(RequireOwnership = false)]
    private void SendMovementInputServerRpc(Vector2 input)
    {
        // Calculate the new position based on input
        Vector2 newPosition = rb.position + input * moveSpeed * Time.fixedDeltaTime; // rb.position + input * moveSpeedsynchronized.Value * Time.fixedDeltaTime;

        // Clamp the player's position within the boundaries
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        // Move the player server-side
        rb.MovePosition(newPosition);

        // Rotate player if there is movement
        if (input != Vector2.zero)
        {
            float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
            rb.rotation = angle - 90f; // Adjust the rotation as needed
        }

        // Send updated position to all clients
        UpdatePositionClientRpc(newPosition, input);
    }

    // ClientRpc to update clients with the new position
    [ClientRpc]
    private void UpdatePositionClientRpc(Vector2 newPosition, Vector2 input)
    {
        if (!IsOwner)
        {
            // Update non-owner clients
            rb.MovePosition(newPosition);

            // Rotate player if there is movement
            if (input != Vector2.zero)
            {
                float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
                rb.rotation = angle - 90f; // Adjust the rotation as needed
            }
        }
    }
    
    // Client authoritative code
    /*private void FixedUpdate()
    {
        if (!IsOwner || !Application.isFocused) return;

        // Read input from the joystick and keyboard
        float horizontal = joystickerScript.Horizontal;
        float vertical = joystickerScript.Vertical;
        movementInput = new Vector2(horizontal, vertical);

        // Normalize the input if necessary to prevent diagonal speed boost
        if (movementInput.magnitude > 1)
        {
            movementInput = movementInput.normalized;
        }

        // Check for keyboard input
        moveDirection = playerControls.ReadValue<Vector2>();

        // Determine final movement direction, prioritizing keyboard over joystick if both are active
        Vector2 finalMovement = moveDirection != Vector2.zero ? moveDirection : movementInput;

        // Move the player locally and clamp the position within the defined boundaries
        Vector2 newPosition = rb.position + finalMovement * moveSpeedsynchronized.Value * Time.deltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        rb.MovePosition(newPosition);

        // Rotate player if there is movement
        if (finalMovement != Vector2.zero)
        {
            float angle = Mathf.Atan2(finalMovement.y, finalMovement.x) * Mathf.Rad2Deg;
            rb.rotation = angle - 90f; // Adjust the rotation as needed
        }

        // Send the updated position and rotation to the server
        SendPositionToServerRpc(newPosition, rb.rotation);
    }

    // ServerRpc to update the player's position on the server
    [ServerRpc]
    private void SendPositionToServerRpc(Vector2 position, float rotation)
    {
        // Update position on other clients
        UpdatePositionClientRpc(position, rotation);
    }

    // ClientRpc to broadcast position updates to all clients
    [ClientRpc]
    private void UpdatePositionClientRpc(Vector2 position, float rotation)
    {
        if (!IsOwner)
        {
            // Only non-owners update their position and rotation
            rb.MovePosition(position);
            rb.rotation = rotation;
        }
    }*/
    
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
        Debug.Log("Random number generated is : "+randomclass);
        playertype.Value = (uint)randomclass;
        playerclasspublicvalue = playertype.Value;
        DisplayClassValueOnLog();
        isInitialized.Value = true;
    }
    void DisplayClassValueOnLog() {
        Debug.Log("Player's class is : "+playertype.Value);
    }
    
    private IEnumerator WaitForInitializationAndShowcase()
    {
        // Wait until isInitialized becomes true
        while (!isInitialized.Value)
        {
            yield return null; // Wait for the next frame
        }

        // Once initialized, showcase class type on the canvas
        playerClassDisplayUI.GetComponent<GameStartInitializer>().OnPlayerInitialized(this.gameObject);
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayTextwriterSound();
        }
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

    private void UpdateScoreLocally()
    {
        if (!IsOwner) return;
        playerScoreText.text = $"SCORE: {playerScore.Value}";
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
                        playerAttribute.rock_ammo = rock_ammo.Value;
                    }
                    break;
                case 1:
                    if (paper_ammo.Value < maxPaperAmmo.Value)
                    {
                        paper_ammo.Value += 1;
                        playerAttribute.paper_ammo = paper_ammo.Value;
                    }
                    break;
                case 2:
                    if (scissor_ammo.Value < maxScissorAmmo.Value)
                    {
                        scissor_ammo.Value += 1;
                        playerAttribute.scissor_ammo = scissor_ammo.Value;
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


    public void DecreaseHP(uint amount, ulong shooterId)
    {
        if (IsServer)
        {
            hp.Value -= amount;
            if (hp.Value <= 0)
            {
                Debug.Log("Player's HP reached 0, awarding points to the shooter and despawning player!");

                // Notify the shooter (increase their score) before despawning
                if (shooterId != 0)
                {
                    PlayerController shooterPlayer = NetworkManager.Singleton.SpawnManager.SpawnedObjects[shooterId].GetComponent<PlayerController>();
                    shooterPlayer.IncreaseScoreServerRpc(); // Award points to the shooter
                }

                // Stop background music if available
                if (AudioManager.instance != null)
                {
                    AudioManager.instance.StopBGMusic();
                }

                // Despawn the player
                DespawnPlayer();
            }

            // Update the player's HP bar on clients
            UpdateHPBarClientRpc((int)hp.Value);
        }

        Debug.Log("HP decreased by " + amount + ", current HP: " + hp.Value);
    }
    [ServerRpc(RequireOwnership = false)]
    public void IncreaseScoreServerRpc()
    {
        playerScore.Value += 1; // Increase the score by a fixed value
        Debug.Log("Player score updated. Current score: " + playerScore.Value);
        UpdateScoreLocally();
    }

    [ClientRpc]
    public void UpdateHPBarClientRpc(int newHP)
    {
        if (IsOwner) // Each client will update its own health bar
        {
            healthbarObject.GetComponent<HealthBar>().SetHealth(newHP);
            GameObject  healthbartextobject = healthbarObject.transform.GetChild (3).gameObject;
            healthbartextobject.GetComponent<TextMeshProUGUI>().text = newHP.ToString();
        }
    }

    public void UpdateHPBarUI()
    {
        if (IsOwner)
        {
            healthbarObject.GetComponent<HealthBar>().SetHealth((int)hp.Value);
        }
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
            UpdateHPBarClientRpc((int)hp.Value);
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
public class PlayerAttributeVariables
{
    public uint rock_ammo;
    public uint paper_ammo;
    public uint scissor_ammo;
    [FormerlySerializedAs("hpvalue")] public uint playerscore;
}