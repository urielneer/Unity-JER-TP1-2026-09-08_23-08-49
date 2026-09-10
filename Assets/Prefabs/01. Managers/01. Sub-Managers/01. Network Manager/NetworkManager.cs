using UnityEngine;
using System.Collections.Generic;
using static CustomExtension.ArrayExtensions;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

#region ...
#endregion ...

public class NetworkManager : BaseManager<NetworkManager>
{
    #region Variables
    #region Menu
    #endregion Menu
    #region Manager-To-Manager Data
    private bool O_Bool_IsStatusOn = false;
    public bool IsStatusOn => O_Bool_IsStatusOn;
    #endregion Manager-To-Manager Data
    #endregion Variables

    #region Methods
    #region Unity Methods
    protected override void Awake()
    {
        // Singleton Operations //
        base.Awake();
        // Variables //
    }
    #endregion Unity Methods

    #region Override Methods
    public override void OnStartUp()
    {
        if (!IsStatusOn)
        {
            // Resume Base //
            base.OnStartUp();
            // Log On //
            MasterManager.Instance.MenuManager.OpenMenu("LoadingMenu");
            PhotonNetwork.OfflineMode = false;
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout = 2000;
            O_Bool_IsStatusOn = true;
            Debug.Log("[Network Manager] Status: 'On'");
        }
    }
    #endregion Override Methods

    #region PUN Methods
    #region Title Room
    public override void OnConnectedToMaster()
    {
        // PUN //
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
        // Menu Manager Communication - Change to Loading //
        MasterManager.Instance.MenuManager.OpenMenu("TitleMenu");
        Debug.Log("[Network Manager] Status: 'Joined Master'");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("[Network Manager] Status: 'Joined Lobby'");
    }
    #endregion Title Room

    #region Find Room
    public override void OnJoinedRoom()
    {
        // Menu Manager Communication //
        // Variables //
        MenuTypeWaitingRoom Ctm_MTWR_Reference = (MasterManager.Instance.MenuManager.GetMenuReference("WaitingRoom")).gameObject.GetComponent<MenuTypeWaitingRoom>();
        // Update Room Name //
        Ctm_MTWR_Reference.SetUpRoom();
        Ctm_MTWR_Reference.JoinRoom();
        Debug.Log("[Network Manager] Status: 'Joined room'");
    }

    public override void OnRoomListUpdate(List<RoomInfo> Lst_PUNRI_RoomList)
    {
        // Menu Manager Communication - Update List //
        MenuTypeFindRoom Ctm_MTFR_Reference = (MasterManager.Instance.MenuManager.GetMenuReference("FindRoomMenu")).gameObject.GetComponent<MenuTypeFindRoom>();
        Ctm_MTFR_Reference.ClearRoomList();
        Ctm_MTFR_Reference.UpdateList(Lst_PUNRI_RoomList);
        Debug.Log("[Network Manager] Status: 'Updated List'");
    }
    #endregion Find Room

    #region Waiting Room
    public override void OnPlayerEnteredRoom(Player PUNPl_NewPlayer)
    {
        Debug.Log("[Network Manager] Status: 'Player Entered Room'");
    }

    public override void OnPlayerLeftRoom(Player PUNPl_OtherPlayer) // ANYONE LEAVES THE ROOM //
    {
        // Variables //
        MenuTypeWaitingRoom Ctm_MTWR_Reference = (MasterManager.Instance.MenuManager.GetMenuReference("WaitingRoom")).gameObject.GetComponent<MenuTypeWaitingRoom>();
        // Menu Manager Communication // 
        // Update Room //
        Ctm_MTWR_Reference.SetUpRoom();
        Debug.Log("[Network Manager] Status: 'Player Left Room'");
    }

    public override void OnLeftRoom() // CURRENT PLAYER LEAVES THE ROOM //
    {
        Debug.Log("[Network Manager] Status: 'Left Room'");
    }
    #endregion Waiting Room

    #region Error Room
    public override void OnCreateRoomFailed(short Shrt_ReturnCode, string Str_Message)
    {
        // Menu Manager Communication - Change to Loading //
        MasterManager.Instance.MenuManager.OpenMenu("ErrorMenu");
        (MasterManager.Instance.MenuManager.GetMenuReference("CreateRoomMenu")).gameObject.GetComponent<MenuTypeError>().PrintError(Str_Message);
        Debug.Log("[Network Manager] Status: 'ERROR FOUND!: " + Str_Message + "'");
    }
    #endregion Error Room
    #endregion PUN Methods

    #region Custom Methods
    public void StartGame()
    {
        // PUN //
        PhotonNetwork.LoadLevel(1);
        // Lock Room Mid Game //
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }

    public void EndGame()
    {
        // PUN //
        PhotonNetwork.LoadLevel(0);
        // Lock Room Mid Game //
        PhotonNetwork.CurrentRoom.IsOpen = true;
    }

    public void LeaveGame()
    {
        // Game Manager Communication // 
        GetComponent<PhotonView>().RPC(nameof(RPC_EndGameForAll), RpcTarget.All, "");
        Debug.Log("[Network Manager] Status: 'Leaving Game'");
    }

    public void JoinRoom(RoomInfo PUNRI_Input)
    {
        // Variables //
        MenuTypeFindRoom Ctm_MTFR_Reference = (MasterManager.Instance.MenuManager.GetMenuReference("FindRoomMenu")).gameObject.GetComponent<MenuTypeFindRoom>();
        MenuTypeWaitingRoom Ctm_MTWR_Reference = (MasterManager.Instance.MenuManager.GetMenuReference("WaitingRoom")).gameObject.GetComponent<MenuTypeWaitingRoom>();
        // Menu Manager - Clear List //
        Ctm_MTFR_Reference.ClearRoomList();
        // Join //
        PhotonNetwork.JoinRoom(PUNRI_Input.Name);
        // Menu Manager Communication //
        // Change to Loading //
        MasterManager.Instance.MenuManager.OpenMenu("LoadingMenu");
        // Game Manager Communication // 
        MasterManager.Instance.GameManager.LeftRoom();
        Debug.Log("[Network Manager] Status: 'Joining Room'");
    }

    public void CreateRoom()
    {
        // Variables //
        MenuTypeCreateRoom Ctm_MTCR_Reference = (MasterManager.Instance.MenuManager.GetMenuReference("CreateRoomMenu")).gameObject.GetComponent<MenuTypeCreateRoom>();
        MenuTypeWaitingRoom Ctm_MTWR_Reference = (MasterManager.Instance.MenuManager.GetMenuReference("WaitingRoom")).gameObject.GetComponent<MenuTypeWaitingRoom>();
        Ctm_MTCR_Reference.UpdateRoomData();
        RoomOptions PUNRO_Settings = new RoomOptions
        {
            MaxPlayers = Ctm_MTCR_Reference.RoomSize,
            IsVisible = true,
            IsOpen = true,
            EmptyRoomTtl = 0,
            CleanupCacheOnLeave = true
        };
        // Error Exit //
        if (string.IsNullOrEmpty(Ctm_MTCR_Reference.RoomName))
        {
            return;
        }
        // Map Manager Communication - Reset Bounds //
        MasterManager.Instance.MapManager.SetBounds(Ctm_MTCR_Reference.MapWidth, Ctm_MTCR_Reference.MapHeight);
        // Create Room //
        PhotonNetwork.CreateRoom(Ctm_MTCR_Reference.RoomName, PUNRO_Settings);
        Ctm_MTWR_Reference.SetUpRoom();
        // Menu Manager Communication - Change to Loading //
        MasterManager.Instance.MenuManager.OpenMenu("LoadingMenu");
        Debug.Log("[Network Manager] Status: 'Creating Room'");
    }

    public void LeaveRoom()
    {
        // Variables //
        MenuTypeFindRoom Ctm_MTFR_Reference = (MasterManager.Instance.MenuManager.GetMenuReference("FindRoomMenu")).gameObject.GetComponent<MenuTypeFindRoom>();
        MenuTypeWaitingRoom Ctm_MTWR_Reference = (MasterManager.Instance.MenuManager.GetMenuReference("WaitingRoom")).gameObject.GetComponent<MenuTypeWaitingRoom>();
        // Game Manager Communication // 
        // Leave Room //
        MasterManager.Instance.GameManager.LeftRoom();
        // Menu Manager Communication // 
        // Update GUI //
        Ctm_MTWR_Reference.SetUpRoom();
        // Change to Loading //
        MasterManager.Instance.MenuManager.OpenMenu("LoadingMenu");
        // Menu Manager Communication // 
        // Remove Room //
        Ctm_MTFR_Reference.ClearRoomList();
        Ctm_MTWR_Reference.ClearPlayerList();
        // Network Manager - Leave //
        PhotonNetwork.LeaveRoom();
        // Menu Manager Communication // 
        // Clear All //
        Ctm_MTFR_Reference.RemoveRoomFromList();
        Ctm_MTFR_Reference.ClearRoomList();
        Ctm_MTWR_Reference.ClearPlayerList();
        Debug.Log("[Network Manager] Status: 'Leaving Room'");
    }
    #endregion Custom Methods
    #endregion Methods

    #region PUN
    #region RPC
    [PunRPC]
    private void RPC_StartGameForAll(string UNUSED)
    {
    }

    [PunRPC]
    private void RPC_EndGameForAll(string UNUSED)
    {
    }
    #endregion RPC
    #endregion PUN
}