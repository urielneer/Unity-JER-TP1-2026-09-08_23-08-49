using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using static CustomExtension.ArrayExtensions;
using UnityEngine.SceneManagement;

public class GameManager : BaseManager<GameManager>
{
    #region    Variables
        [Header(" Manager Settings")]
            [SerializeField] private float _respawnCooldown;
            float IO_Flt_RespawnCooldown;
            public float RespawnCooldown => IO_Flt_RespawnCooldown;
    #endregion Variables
    #region    Methods
        #region    Unity Methods
            protected override void Awake()
            {
                // Singleton Operations //
                    base.Awake();
                // Variables //
                    IO_Flt_RespawnCooldown = _respawnCooldown;
            }
        #endregion Unity Methods
        #region    Override Methods
            #pragma warning disable CS1998
            public async override void OnStartUp()
            #pragma warning restore CS1998
            {
                // Resume Base //
                    base.OnStartUp();
                // Scene dependant Behaviour //
                    switch (SceneManager.GetActiveScene().buildIndex) 
                    {
                        case 0:
                        break;
                        case 1: // Level //
                            // Spawn Player //
                                
                        break;
                    }
            }
            public override void OnFixedUpdate(float Flt_FixedDT)
            {
                // Resume Base //
                    base.OnFixedUpdate(Flt_FixedDT);
                // Scene dependant Behaviour //
                    switch (SceneManager.GetActiveScene().buildIndex) 
                    {
                        case 0:
                            MasterManager.Instance.InputManager.DisableInputPlayerActions();
                        break;
                        case 1: // Level //   
                        break;
                    }
            }
        #endregion Override Methods
        #region    Custom Methods
            #region    Networking
                public void LeftRoom()
                {
                    // Variables //
                        MenuTypeHUD Ctm_MTH_Reference = (MasterManager.Instance.MenuManager.GetMenuReference("HUD")).gameObject.GetComponent<MenuTypeHUD>();
                        MenuTypeScoreBoard Ctm_MTSB_Reference = (MasterManager.Instance.MenuManager.GetMenuReference("Scoreboard")).gameObject.GetComponent<MenuTypeScoreBoard>();
                        MenuTypeWaitingRoom Ctm_MTWR_Reference = (MasterManager.Instance.MenuManager.GetMenuReference("WaitingRoom")).gameObject.GetComponent<MenuTypeWaitingRoom>();
                    // Menu Manager Communication // 
                        Ctm_MTWR_Reference.LeftRoom(PhotonNetwork.LocalPlayer.NickName);
                        Ctm_MTH_Reference.LeftRoom();
                        Ctm_MTSB_Reference.LeftRoom();
                }
                public void JoinRoom()
                {
                    // Variables //
                        MenuTypeHUD Ctm_MTH_Reference = (MasterManager.Instance.MenuManager.GetMenuReference("HUD")).gameObject.GetComponent<MenuTypeHUD>();
                        MenuTypeScoreBoard Ctm_MTSB_Reference = (MasterManager.Instance.MenuManager.GetMenuReference("Scoreboard")).gameObject.GetComponent<MenuTypeScoreBoard>();
                        MenuTypeWaitingRoom Ctm_MTWR_Reference = (MasterManager.Instance.MenuManager.GetMenuReference("WaitingRoom")).gameObject.GetComponent<MenuTypeWaitingRoom>();
                    // Menu Manager Communication // 
                        Ctm_MTWR_Reference.JoinRoom();
                        Ctm_MTH_Reference.JoinRoom();
                        Ctm_MTSB_Reference.JoinRoom();
                }
                public void GameStart()
                {
                    // Input Manager Communication // 
                        MasterManager.Instance.InputManager.OnStartUp();
                    // Variables //
                        MenuTypeHUD Ctm_MTH_Reference = (MasterManager.Instance.MenuManager.GetMenuReference("HUD")).gameObject.GetComponent<MenuTypeHUD>();
                        MenuTypeScoreBoard Ctm_MTSB_Reference = (MasterManager.Instance.MenuManager.GetMenuReference("Scoreboard")).gameObject.GetComponent<MenuTypeScoreBoard>();
                    // Menu Manager Communication // 
                        Ctm_MTH_Reference.GameStart();
                        Ctm_MTSB_Reference.GameStart();
                    MasterManager.Instance.InputManager.EnableInputPlayerActions();
                    Debug.Log("[Game Manager] Status: 'Game Started'");
                }
                public void GameEnd()
                {
                    // Variables //
                        MenuTypeHUD Ctm_MTH_Reference = (MasterManager.Instance.MenuManager.GetMenuReference("HUD")).gameObject.GetComponent<MenuTypeHUD>();
                        MenuTypeScoreBoard Ctm_MTSB_Reference = (MasterManager.Instance.MenuManager.GetMenuReference("Scoreboard")).gameObject.GetComponent<MenuTypeScoreBoard>();
                    // Menu Manager Communication // 
                        Ctm_MTH_Reference.GameEnd();
                        Ctm_MTSB_Reference.GameEnd();
                    MasterManager.Instance.InputManager.DisableInputPlayerActions();
                    Debug.Log("[Game Manager] Status: 'Game Over'");
                }
                public void ShowCursor()
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
                public void HideCursor()
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            #endregion Networking
            #region    PowerUp
            #endregion PowerUp
        #endregion Custom Methods
    #endregion Methods
}
