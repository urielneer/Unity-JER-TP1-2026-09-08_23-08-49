using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using static CustomExtension.ArrayExtensions;
using System;

public class CustomInputManager : BaseManager<CharacterManager>
{
    private PlayerInputActions O_IAct_PIA_Controller;
    public PlayerInputActions ControllerInput => O_IAct_PIA_Controller;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void OnStartUp()
    {
        base.OnStartUp();
        O_IAct_PIA_Controller = new PlayerInputActions();
        AddKeyListeners();
    }

    public void EnableInputPlayerActions()
    {
        O_IAct_PIA_Controller.Player.Enable();
        O_IAct_PIA_Controller.Chat.Disable();
        MasterManager.Instance.GameManager.HideCursor();
    }

    public void DisableInputPlayerActions()
    {
        O_IAct_PIA_Controller.Player.Disable();
        O_IAct_PIA_Controller.Chat.Enable();
        MasterManager.Instance.GameManager.ShowCursor();
    }

    private void AddKeyListeners()
    {
        PlayerInputActions.PlayerActions IAA_Player = O_IAct_PIA_Controller.Player;
        PlayerInputActions.ChatActions IAA_Chat = O_IAct_PIA_Controller.Chat;

        // Move
        IAA_Player.Move.performed -= OnMoveKeyPressed;
        IAA_Player.Move.performed += OnMoveKeyPressed;
        IAA_Player.Move.canceled -= OnMoveKeyCancelled;
        IAA_Player.Move.canceled += OnMoveKeyCancelled;

        // Rotate
        IAA_Player.Rotate.performed -= OnRotateKeyPressed;
        IAA_Player.Rotate.performed += OnRotateKeyPressed;
        IAA_Player.Rotate.canceled -= OnRotateKeyCancelled;
        IAA_Player.Rotate.canceled += OnRotateKeyCancelled;

        // Jump
        IAA_Player.Jump.performed -= OnJumpKeyPressed;
        IAA_Player.Jump.performed += OnJumpKeyPressed;
        IAA_Player.Jump.canceled -= OnJumpKeyCancelled;
        IAA_Player.Jump.canceled += OnJumpKeyCancelled;

        // Shoot
        IAA_Player.Shoot.performed -= OnShootKeyPressed;
        IAA_Player.Shoot.performed += OnShootKeyPressed;

        // Pause
        IAA_Player.Pause.performed -= OnPauseKeyPressed;
        IAA_Player.Pause.performed += OnPauseKeyPressed;

        // ScoreBoard
        IAA_Player.Shoot.performed -= OnScoreBoardKeyPressed; // Nota: tenías asignado Shoot acá en vez de ScoreBoard
        IAA_Player.Shoot.performed += OnScoreBoardKeyPressed;
        IAA_Player.Shoot.canceled -= OnScoreBoardKeyCancelled;
        IAA_Player.Shoot.canceled += OnScoreBoardKeyCancelled;

        // Chat
        IAA_Player.Chat.performed -= OnChatKeyPressed;
        IAA_Player.Chat.performed += OnChatKeyPressed;

        // Debug
        IAA_Player.Debug.performed -= OnDebugKeyPressed;
        IAA_Player.Debug.performed += OnDebugKeyPressed;

        // Submit (Chat)
        IAA_Chat.Submit.performed -= OnSubmitChatMessage;
        IAA_Chat.Submit.performed += OnSubmitChatMessage;
    }

    public void OnDeactivateControllerInput(string Str_Input)
    {
        DisableInputPlayerActions();
    }

    public void OnActivateControllerInput(string Str_Input)
    {
        EnableInputPlayerActions();
    }

    public void OnMoveKeyPressed(InputAction.CallbackContext IACbC_Context)
    {
        if (SceneManager.GetActiveScene().buildIndex != 1) return;
        MasterManager.Instance.CharacterManager.OwnPlayer.OnStartedMoving(IACbC_Context.ReadValue<Vector2>());
    }

    public void OnRotateKeyPressed(InputAction.CallbackContext IACbC_Context)
    {
        if (SceneManager.GetActiveScene().buildIndex != 1) return;
        MasterManager.Instance.CharacterManager.OwnPlayer.OnStartedRotating(IACbC_Context.ReadValue<Vector2>());
    }

    public void OnJumpKeyPressed(InputAction.CallbackContext IACbC_Context)
    {
        if (SceneManager.GetActiveScene().buildIndex != 1) return;
        MasterManager.Instance.CharacterManager.OwnPlayer.OnStartedJumping();
    }

    public void OnShootKeyPressed(InputAction.CallbackContext IACbC_Context)
    {
        if (SceneManager.GetActiveScene().buildIndex != 1) return;
        MasterManager.Instance.CharacterManager.OwnPlayer.OnStartedShooting();
    }

    public void OnPauseKeyPressed(InputAction.CallbackContext IACbC_Context)
    {
        if (SceneManager.GetActiveScene().buildIndex != 1) return;

        MenuTypeHUD Ctm_MTH_Reference = (MasterManager.Instance.MenuManager.GetMenuReference("HUD")).gameObject.GetComponent<MenuTypeHUD>();
        Ctm_MTH_Reference.ChatElement.DisableChatUsage();

        if (MasterManager.Instance.MenuManager.CurrentMenu.MenuName == "PauseMenu")
        {
            MasterManager.Instance.MenuManager.OpenMenu("HUD");
            EnableInputPlayerActions();
        }
        else
        {
            MasterManager.Instance.MenuManager.OpenMenu("PauseMenu");
            DisableInputPlayerActions();
        }

        StartCoroutine(ClearExtraCharNextFrame());
    }

    public void OnScoreBoardKeyPressed(InputAction.CallbackContext IACbC_Context)
    {
        if (SceneManager.GetActiveScene().buildIndex != 1) return;
        StartCoroutine(ClearExtraCharNextFrame());
    }

    public void OnChatKeyPressed(InputAction.CallbackContext IACbC_Context)
    {
        if (SceneManager.GetActiveScene().buildIndex != 1) return;

        MenuTypeHUD Ctm_MTH_Reference = (MasterManager.Instance.MenuManager.GetMenuReference("HUD")).gameObject.GetComponent<MenuTypeHUD>();
        Ctm_MTH_Reference.ChatElement.EnableChatUsage();
        DisableInputPlayerActions();

        StartCoroutine(ClearExtraCharNextFrame());
    }

    public void OnDebugKeyPressed(InputAction.CallbackContext IACbC_Context)
    {
        if (SceneManager.GetActiveScene().buildIndex != 1) return;
        MasterManager.Instance.CharacterManager.OwnPlayer.ExecuteDamage(200f);
    }

    public void OnMoveKeyCancelled(InputAction.CallbackContext IACbC_Context)
    {
        if (SceneManager.GetActiveScene().buildIndex != 1) return;
        MasterManager.Instance.CharacterManager.OwnPlayer.OnStoppedMoving(); // CS1501 Fix
    }

    public void OnRotateKeyCancelled(InputAction.CallbackContext IACbC_Context)
    {
        if (SceneManager.GetActiveScene().buildIndex != 1) return;
        MasterManager.Instance.CharacterManager.OwnPlayer.OnStoppedRotating(); // CS1501 Fix
    }

    public void OnJumpKeyCancelled(InputAction.CallbackContext IACbC_Context)
    {
        if (SceneManager.GetActiveScene().buildIndex != 1) return;
        MasterManager.Instance.CharacterManager.OwnPlayer.OnStoppedJumping();
    }

    public void OnScoreBoardKeyCancelled(InputAction.CallbackContext IACbC_Context)
    {
        if (SceneManager.GetActiveScene().buildIndex != 1) return;
        StartCoroutine(ClearExtraCharNextFrame());
    }

    public void OnSubmitChatMessage(InputAction.CallbackContext IACbC_Context)
    {
        MenuTypeHUD Ctm_MTH_Reference = (MasterManager.Instance.MenuManager.GetMenuReference("HUD")).gameObject.GetComponent<MenuTypeHUD>();
        BaseMenuInputField Cym_BMIF_Submit = Ctm_MTH_Reference.ChatElement.Element;
        string Str_Submit = Ctm_MTH_Reference.ChatElement.Element.InputField.text;

        Ctm_MTH_Reference.ChatElement.DisableChatUsage();
        EnableInputPlayerActions();

        if (Str_Submit.Length > 0)
        {
            Ctm_MTH_Reference.ChatElement.SendChat(PhotonNetwork.LocalPlayer.NickName, Str_Submit);
        }

        Cym_BMIF_Submit.Overwrite("");
        StartCoroutine(ClearExtraCharNextFrame());
    }

    private System.Collections.IEnumerator ClearExtraCharNextFrame()
    {
        yield return null;
    }
}