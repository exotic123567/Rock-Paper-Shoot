using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class AmmoSelectionManager : MonoBehaviour
{
    [SerializeField] private Button[] ammoButtons; // Array of buttons for each ammo type
    public PlayerController playerController;

    private void Start()
    {
        // Ensure all outlines are initially turned off & initially select Rock Ammo
        for (int i = 0; i < ammoButtons.Length; i++)
        {
            Outline outline = ammoButtons[i].GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = false;
            }
        }
        Outline selectedOutline = ammoButtons[0].GetComponent<Outline>();
        if (selectedOutline != null)
        {
            selectedOutline.enabled = true;
        }
    }

    // Method to call when a button is clicked
    public void SelectRockAmmo()
    {
        UpdateAmmoSelection(0);
    }

    public void SelectPaperAmmo()
    {
        UpdateAmmoSelection(1);
    }

    public void SelectScissorAmmo()
    {
        UpdateAmmoSelection(2);
    }

    private void UpdateAmmoSelection(uint ammoType)
    {
        // Disable all outlines
        for (int i = 0; i < ammoButtons.Length; i++)
        {
            Outline outline = ammoButtons[i].GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = false;
            }
        }
        if (AudioManager.instance != null) {
            AudioManager.instance.PlayAmmoTypeseleectionSound();
        }

        // Enable the selected button's outline
        Outline selectedOutline = ammoButtons[ammoType].GetComponent<Outline>();
        if (selectedOutline != null)
        {
            selectedOutline.enabled = true;
        }

        // Call the ServerRpc to update ammo type on the server
        if (playerController != null && playerController.IsOwner)
        {
            playerController.UpdateSelectedAmmoTypeServerRpc(ammoType);  // Call the ServerRpc from the client
        }
    }
}