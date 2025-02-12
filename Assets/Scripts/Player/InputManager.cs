using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    PlayerInput inputActions;
    PlayerMovement player;
    PlayerWeapon weapon;
    UiManager uiManager;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>();
        weapon = FindFirstObjectByType<PlayerWeapon>();
        uiManager = FindFirstObjectByType<UiManager>();

        inputActions = new PlayerInput();
        inputActions.Gameplay.Move.performed += MovePlayer;
        inputActions.Gameplay.Jump.performed += JumpPlayer;
        inputActions.Gameplay.SwitchWeapon.performed += SwitchWeaponPlayer;
        inputActions.Gameplay.UseWeapon.performed += UseWeapon;
        inputActions.Gameplay.UseWeaponSecondary.performed += UseWeaponSecondary;
        inputActions.Gameplay.OpenInventory.performed += OpenInventory;
        inputActions.Gameplay.Quickfall.performed += QuickFallPlayer;
        inputActions.Gameplay.Enable();
        
        inputActions.Inventory.CloseInventory.performed += CloseInventory;

        inputActions.Ui.LeftMousePress.performed += JUiLeftMousePress;
        inputActions.Ui.RightMousePress.performed += JUiRightMousePress;
        inputActions.Ui.MiddleMousePress.performed += JUiMiddleMousePress;
        inputActions.Ui.Enable();
    }


    private void MovePlayer(InputAction.CallbackContext context)
    {
        if(player != null)
        {
            StartCoroutine(player.Move(context));
        }
    }

    private void JumpPlayer(InputAction.CallbackContext context)
    {
        if (player != null)
        {
            StartCoroutine(player.Jump(context));
        }
    }

    private void SwitchWeaponPlayer(InputAction.CallbackContext context)
    {
        if(weapon != null)
        {
            weapon.SwitchWeapon();
        }
    }

    private void UseWeapon(InputAction.CallbackContext context)
    {
        weapon.UseWeapon();
    }

    private void UseWeaponSecondary(InputAction.CallbackContext context)
    {
        weapon.UseWeaponSecondary();
    }

    private void OpenInventory(InputAction.CallbackContext context)
    {
        uiManager.ActivateUi(1, true);

        inputActions.Gameplay.Disable();
        inputActions.Inventory.Enable();
        player.ChangeGameplay(false);
    }

    private void CloseInventory(InputAction.CallbackContext context)
    {
        uiManager.ActivateUi(1, false);

        inputActions.Gameplay.Enable();
        inputActions.Inventory.Disable();
        player.ChangeGameplay(true);
    }

    private void JUiLeftMousePress(InputAction.CallbackContext context)
    {
        uiManager.LeftMousePress();
    }

    private void JUiRightMousePress(InputAction.CallbackContext context)
    {
        uiManager.RightMousePress();
    }

    private void JUiMiddleMousePress(InputAction.CallbackContext context)
    {
        uiManager.MiddleMousePress();
    }

    private void QuickFallPlayer(InputAction.CallbackContext context)
    {
        StartCoroutine(player.QuickFall(context));
    }
}
