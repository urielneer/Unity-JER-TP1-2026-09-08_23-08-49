
using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using static CustomExtension.ArrayExtensions;

#region    "using" Script Clarification
    using Hashtable = ExitGames.Client.Photon.Hashtable;
#endregion "using" Script Clarification
public class MapContainer : MonoBehaviourPunCallbacks
{
    #region    Variables
        #region    Hashtables (Communication among instances)
            // Map //
                private const string MapTileRadius_KEY = "MapTileRadius";
                private const string MapAxisX_KEY = "MapAxisX";
                private const string MapAxisY_KEY = "MapAxisY";
                private const string MapData_KEY = "MapData";
                private const string MapWallIDs_KEY = "MapWallIDs";
            // Character //
                private const string PlayerID_KEY = "PlayerID";
                private const string PlayerList_KEY = "PlayerList";
            // Character //
                private const string BulletOwnerID_KEY = "BulletOwnerID";
                private const string BulletOwnerName_KEY = "BulletOwnerName";
                private const string BulletPos_KEY = "BulletPos";
                private const string BulletQuat_KEY = "BulletQuat";
                private const string BulletDir_KEY = "BulletDir";
        #endregion Hashtables (Communication among instances)
    #endregion Variables
    #region    Methods
        #region    Unity Methods
            public void Awake()
            {
            }
            void Start()
            {

            }
        #endregion Unity Methods
        #region    Override Methods
            public virtual void OnStartUp()
            {
            }
            public virtual void OnFixedUpdate()
            {
            }
        #endregion Override Methods
        #region    Custom Methods
            // Hashtables //
                public void UpdateAllMapManagers(Hashtable Hsh_MapKeys)
                {
                    GetComponent<PhotonView>().RPC(nameof(RPC_ForceMapUpdateToClients), RpcTarget.All, Hsh_MapKeys); 
                }
                public void UpdateAllCharacterManagers(Hashtable Hsh_Input)
                {
                    GetComponent<PhotonView>().RPC(nameof(RPC_AddCharacterToMaster), RpcTarget.MasterClient, Hsh_Input); 
                }
                public void UpdateAllBulletManagers(Hashtable Hsh_Input)
                {
                    GetComponent<PhotonView>().RPC(nameof(RPC_ForceBulletUpdateToClients), RpcTarget.All, Hsh_Input); 
                }
                public void KillCharacter(Hashtable Hsh_Input)
                {
                    GetComponent<PhotonView>().RPC(nameof(RPC_AlertCharacterDeathToClients), RpcTarget.All, Hsh_Input); 
                }
                public void ReSpawnCharacter(Hashtable Hsh_Input)
                {
                    GetComponent<PhotonView>().RPC(nameof(RPC_AlertCharacterRespawnToClients), RpcTarget.All, Hsh_Input); 
                }
            // Coroutine Flag //
                public void ReduceClientLoadCounter()
                {
                    GetComponent<PhotonView>().RPC(nameof(RPC_ReduceClientLoadCounter), RpcTarget.MasterClient); 
                }
                public void MasterWaitCompleted()
                {
                    GetComponent<PhotonView>().RPC(nameof(RPC_MasterWaitCompleted), RpcTarget.All); 
                }
        #endregion Custom Methods
        #region    PUN         
            #region    Hastable
                // Map //
                    private bool AreMapManagerHashtables(Hashtable Hsh_Input)
                    {
                        if (
                            Hsh_Input.ContainsKey(MapTileRadius_KEY) &&
                            Hsh_Input.ContainsKey(MapAxisX_KEY)      &&
                            Hsh_Input.ContainsKey(MapAxisY_KEY)      &&
                            Hsh_Input.ContainsKey(MapData_KEY)       &&
                            Hsh_Input.ContainsKey(MapWallIDs_KEY)
                           )
                        { 
                            return true; 
                        }
                        else
                        { 
                            UnityEngine.Debug.LogError("\"MapContiner.cs\"'s \"AreMapManagerHashtables()\" returned FALSE, review Hastables");
                            return false; 
                        }
                    }
                // Character //
                    private bool IsPlayerIDHashtable(Hashtable Hsh_Input)
                    {
                        if ( Hsh_Input.ContainsKey(PlayerID_KEY) )
                        { 
                            return true; 
                        }
                        else
                        { 
                            UnityEngine.Debug.LogError("\"MapContiner.cs\"'s \"IsPlayerIDHashtable()\" returned FALSE, review Hastables");
                            return false; 
                        }
                    }
                    private bool IsPlayerListHashtable(Hashtable Hsh_Input)
                    {
                        if ( Hsh_Input.ContainsKey(PlayerList_KEY) )
                        { 
                            return true; 
                        }
                        else
                        { 
                            UnityEngine.Debug.LogError("\"MapContiner.cs\"'s \"IsPlayerListHashtable()\" returned FALSE, review Hastables");
                            return false; 
                        }
                    }
                // Bullet //
                    private bool AreBulletDataHashtables(Hashtable Hsh_Input)
                    {
                        if ( 
                            Hsh_Input.ContainsKey(BulletOwnerID_KEY)   &&
                            Hsh_Input.ContainsKey(BulletOwnerName_KEY) &&
                            Hsh_Input.ContainsKey(BulletPos_KEY)       &&
                            Hsh_Input.ContainsKey(BulletQuat_KEY)      &&
                            Hsh_Input.ContainsKey(BulletDir_KEY)
                           )
                        { 
                            return true; 
                        }
                        else
                        { 
                            UnityEngine.Debug.LogError("\"MapContiner.cs\"'s \"AreBulletDataHashtables()\" returned FALSE, review Hastables");
                            return false; 
                        }
                    }
            #endregion Hastable  
            #region    RPC
                #region    Hashtables
                    // Map //
                        [PunRPC]
                        private void RPC_ForceMapUpdateToClients(Hashtable Hsh_Properties)
                        {
                            if (!PhotonNetwork.IsMasterClient)
                            {
                                if (AreMapManagerHashtables(Hsh_Properties))
                                {
                                    // Import new properties //
                                        MasterManager.Instance.MapManager.ApplyMapManagerProperties(Hsh_Properties);                           
                                    // Load Map //
                                        MasterManager.Instance.MapManager.LoadMap();
                                    // MenuManager - Switch Screen //
                                        MasterManager.Instance.MenuManager.OpenMenu("HUD"); 
                                }
                            }
                        }
                    // Character //
                        [PunRPC]
                        private void RPC_AddCharacterToMaster(Hashtable Hsh_Input)
                        {
                            if (PhotonNetwork.IsMasterClient)
                            {
                                if (IsPlayerIDHashtable(Hsh_Input))
                                {
                                    // Variables //
                                        BasePlayer Ctm_BP_Client = (PhotonView.Find((int)Hsh_Input[PlayerID_KEY])).GetComponent<BasePlayer>();
                                        Hashtable Hsh_NewList = MasterManager.Instance.CharacterManager.AddToPlayerList(Ctm_BP_Client);
                                    // Force Update Other Client's Room Prefab //
                                        GetComponent<PhotonView>().RPC(nameof(RPC_ForceCharacterUpdateToClients),RpcTarget.All, Hsh_NewList);
                                }
                            }
                        }
                        [PunRPC]
                        private void RPC_ForceCharacterUpdateToClients(Hashtable Hsh_Input)
                        {
                            if (!PhotonNetwork.IsMasterClient)
                            {
                                if (IsPlayerListHashtable(Hsh_Input))
                                {
                                    // Variables //
                                        BasePlayer[] Ctm_BP_A1_NewList = new BasePlayer[0];
                                        foreach (int Int_ID in ((int[])Hsh_Input[PlayerList_KEY])) 
                                            AddNewToArray(ref Ctm_BP_A1_NewList, (PhotonView.Find(Int_ID)).GetComponent<BasePlayer>());
                                    // Force Update //
                                        MasterManager.Instance.CharacterManager.OverwritePlayerList(Ctm_BP_A1_NewList);
                                }
                            }
                        }
                        [PunRPC]
                        private void RPC_AlertCharacterDeathToClients(Hashtable Hsh_Input)
                        {
                            if (IsPlayerIDHashtable(Hsh_Input))
                            {
                                // Variables //
                                    BasePlayer Ctm_BP_Client = (PhotonView.Find((int)Hsh_Input[PlayerID_KEY])).GetComponent<BasePlayer>();
                                // Kill //
                                    Ctm_BP_Client.ExecuteDeath();
                            }
                        }
                        [PunRPC]
                        private void RPC_AlertCharacterRespawnToClients(Hashtable Hsh_Input)
                        {
                            if (IsPlayerIDHashtable(Hsh_Input))
                            {
                                // Variables //
                                    BasePlayer Ctm_BP_Client = (PhotonView.Find((int)Hsh_Input[PlayerID_KEY])).GetComponent<BasePlayer>();
                                // Spawn //
                                Ctm_BP_Client.ExecuteSpawn();
                            }
                        }
                    // Bullet //
                        [PunRPC]
                        private void RPC_ForceBulletUpdateToClients(Hashtable Hsh_Input)
                        {
                            if (AreBulletDataHashtables(Hsh_Input))
                            {
                                // Force Spawn //
                                    MasterManager.Instance.BulletManager.SpawnBullet(
                                                                                     (string)Hsh_Input[BulletOwnerName_KEY], 
                                                                                     (int)Hsh_Input[BulletOwnerID_KEY],
                                                                                     (Vector3)Hsh_Input[BulletPos_KEY], 
                                                                                     (Quaternion)Hsh_Input[BulletQuat_KEY], 
                                                                                     (Vector3)Hsh_Input[BulletDir_KEY]
                                                                                    );
                            }
                        }
                #endregion Hashtables 
                #region    Coroutines
                        [PunRPC]
                        private void RPC_ReduceClientLoadCounter()
                        {
                            if (PhotonNetwork.IsMasterClient)
                            {
                                    MasterManager.Instance.MapManager.ReduceCountdown();     
                            }
                        }
                        [PunRPC]
                        private void RPC_MasterWaitCompleted()
                        {
                            if (!PhotonNetwork.IsMasterClient)
                            {
                                    MasterManager.Instance.MapManager.MatserStartUpCompleted();     
                            }
                        }
                #endregion Coroutines 
            #endregion RPC
        #endregion PUN
    #endregion Methods
}
