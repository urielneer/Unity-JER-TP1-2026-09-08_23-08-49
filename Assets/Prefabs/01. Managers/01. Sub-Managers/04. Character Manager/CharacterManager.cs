using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using static CustomExtension.ArrayExtensions;


#region    "using" Script Clarification
    using Hashtable = ExitGames.Client.Photon.Hashtable;
#endregion "using" Script Clarification
#region    Enums
    public enum PlayerType { Vigilant, Chaser };
#endregion Enums
public class CharacterManager : BaseManager<CharacterManager>
{
    #region    Variables
            PlayerType IO_E_PT_ClientType;
            public PlayerType ClientType => IO_E_PT_ClientType;
        [Header(" All Character Settings")]
            [SerializeField] float _maxHealth;
            float IO_Flt_MaxHealth;
            public float MaxHealth => IO_Flt_MaxHealth;
        [Header(" Manager Settings")]
            [SerializeField] private Transform _mapContainer;
            private Transform I_Trfm_MapContainer;
            [SerializeField] private float _shootCooldown;
            float IO_Flt_ShootCooldown;
            public float ShootCooldown => IO_Flt_ShootCooldown;
            BasePlayer O_Ctm_BP_OwnPlayer;
            public BasePlayer OwnPlayer => O_Ctm_BP_OwnPlayer; 
            BasePlayer[] O_Ctm_BP_A1_PlayerList;
            public BasePlayer[] PlayerList => O_Ctm_BP_A1_PlayerList;
        #region    Hashtables (Communication among instances)
            private const string PlayerName_KEY = "PlayerName";
            private const string PlayerMatIndex_KEY = "PlayerrMatIndex";
            private const string PlayerType_KEY = "PlayerType";
            private const string PlayerID_KEY = "PlayerID";
            private const string PlayerList_KEY = "PlayerList";
        #endregion Hashtables (Communication among instances)
    #endregion Variables
    #region    Methods
        #region    Unity Methods
            protected override void Awake()
            {
                // Singleton Operations //
                    base.Awake();
                // Variables //
                    I_Trfm_MapContainer = _mapContainer;
                    IO_Flt_MaxHealth = _maxHealth;
                    IO_Flt_ShootCooldown = _shootCooldown;
            }
        #endregion Unity Methods
        #region    Override Methods
            #pragma warning disable CS1998
            public async override void OnStartUp()
            #pragma warning restore CS1998
            {
                // Resume Base //
                    base.OnStartUp();
                // Variables //
                    O_Ctm_BP_A1_PlayerList = new BasePlayer[0];
                // Scene dependant Behaviour //
                    switch (SceneManager.GetActiveScene().buildIndex) 
                    {
                        case 0:
                        break;
                        case 1: // Level //

                            // Rol - Se resuelve desde la Room Property (el RPC del lobby puede no haber llegado) //
                                ResolveClientTypeFromRoom();
                            // Variables //
                                Vector3 Vec3_Pos = Vector3.zero;
                                Quaternion Quat_Rot = Quaternion.identity;
                            // Spawn Player //
                                O_Ctm_BP_OwnPlayer = (PhotonNetwork.Instantiate("02. Player/Player_Master", Vec3_Pos, Quat_Rot)).GetComponent<BasePlayer>();
                                O_Ctm_BP_OwnPlayer.SetData(PhotonNetwork.LocalPlayer.NickName, O_Ctm_BP_OwnPlayer.GetComponent<PhotonView>().ViewID, IO_Flt_MaxHealth, IO_E_PT_ClientType);
                                O_Ctm_BP_OwnPlayer.OnStartUp();
                                ChangePlayerMaterial(null);
                            // Spawn //
                                O_Ctm_BP_OwnPlayer.ExecuteSpawn();
                            // MapManager - Container //
                                I_Trfm_MapContainer = (Object.FindFirstObjectByType<MapContainer>()).gameObject.transform;
                                O_Ctm_BP_OwnPlayer.gameObject.transform.SetParent(I_Trfm_MapContainer, true);
                                #region    Synchronize
                                    Hashtable Hsh_MapKeys = new Hashtable
                                    {
                                        { PlayerID_KEY, O_Ctm_BP_OwnPlayer.GetComponent<PhotonView>().ViewID },
                                        { PlayerName_KEY, PhotonNetwork.LocalPlayer.NickName },
                                        { PlayerType_KEY, (PlayerType) IO_E_PT_ClientType }
                                    }; 
                                #endregion Synchronize
                            // Update Everywhere //
                                UpdateAllCharacterManagers(Hsh_MapKeys);
                        break;
                    }
            }
            public override void OnUpdate(float Flt_FixedDT)
            {
                // Resume Base //
                    base.OnUpdate(Flt_FixedDT);
                // Scene dependant Behaviour //
                    switch (SceneManager.GetActiveScene().buildIndex) 
                    {
                        case 0:
                        break;
                        case 1: // Level //
                            if (O_Ctm_BP_A1_PlayerList.Length == 0) return;
                            foreach (BasePlayer Ctm_BP_Player in O_Ctm_BP_A1_PlayerList) 
                                Ctm_BP_Player.OnUpdate(Flt_FixedDT);
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
                        break;
                        case 1: // Level //
                            if (O_Ctm_BP_A1_PlayerList.Length == 0) return;
                            foreach (BasePlayer Ctm_BP_Player in O_Ctm_BP_A1_PlayerList) 
                                Ctm_BP_Player.OnFixedUpdate(Flt_FixedDT);
                        break;
                    }
            }
        #endregion Override Methods
        #region    Custom Methods
            public void Changetype(PlayerType E_PT_New)
            {
                IO_E_PT_ClientType = E_PT_New;
            }
            private MenuTypeHUD GetHUD()
            {
                BaseMenuType Ctm_BMT_HUD = MasterManager.Instance.MenuManager.GetMenuReference("HUD");
                if (Ctm_BMT_HUD == null) return null;
                return Ctm_BMT_HUD.gameObject.GetComponent<MenuTypeHUD>();
            }
            private System.Collections.IEnumerator ShowRoleRoutine()
            {
                // Esperar a que el HUD este abierto - Arranca con el LoadingMenu adelante //
                    float Flt_Timeout = 10f;
                    MenuTypeHUD Ctm_MTH_HUD = GetHUD();
                    while (Flt_Timeout > 0f && (Ctm_MTH_HUD == null || !Ctm_MTH_HUD.gameObject.activeInHierarchy))
                    {
                        Flt_Timeout -= Time.deltaTime;
                        yield return null;
                        Ctm_MTH_HUD = GetHUD();
                    }
                    if (Ctm_MTH_HUD == null) yield break;
                // Mostrar //
                    Ctm_MTH_HUD.ShowRole(IO_E_PT_ClientType);
                    yield return new WaitForSeconds(2f);
                    Ctm_MTH_HUD.ClearRole();
            }
            // Unica fuente de verdad sobre quien es el Vigilante //
            public int GetVigilantActorNumber()
            {
                // Room Property - Elegido en el lobby //
                    if (PhotonNetwork.CurrentRoom != null &&
                        PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("VigilantActor", out object Obj_Actor))
                    {
                        int Int_Stored = (int) Obj_Actor;
                        if (Int_Stored != -1) return Int_Stored;
                    }
                // Fallback - Si nunca se eligio, el Master es el Vigilante //
                    return (PhotonNetwork.MasterClient != null) ? PhotonNetwork.MasterClient.ActorNumber : -1;
            }
            public void ResolveClientTypeFromRoom()
            {
                // Variables //
                    int Int_VigilantActor = GetVigilantActorNumber();
                // Asignar //
                    IO_E_PT_ClientType = (PhotonNetwork.LocalPlayer.ActorNumber == Int_VigilantActor) ?
                                            PlayerType.Vigilant :
                                            PlayerType.Chaser;
                    Debug.Log("[Character Manager] Rol asignado: " + IO_E_PT_ClientType + " (Actor " + PhotonNetwork.LocalPlayer.ActorNumber + ", Vigilante es Actor " + Int_VigilantActor + ")");
                // Reset de partida - Esto corre en cada arranque de Level //
                    R_Int_ChaserDeaths = 0;
                    R_Bool_GameOver = false;
                    O_Ctm_BP_A1_PlayerList = new BasePlayer[0];
                // Avisar el rol en pantalla - El temporizado corre aca porque el HUD puede estar inactivo //
                    StartCoroutine(ShowRoleRoutine());
            }
            public Hashtable AddToPlayerList(BasePlayer Ctm_BP_Client)
            {
                AddNewToArray(ref O_Ctm_BP_A1_PlayerList, Ctm_BP_Client);
                #region    Synchronize
                    int[] Int_A1_IDList = new int[0];
                    foreach (BasePlayer Ctm_BP_Element in O_Ctm_BP_A1_PlayerList) 
                        AddNewToArray(ref Int_A1_IDList, Ctm_BP_Element.GetComponent<PhotonView>().ViewID);
                    Hashtable Hsh_MapKeys = new Hashtable
                    {
                        { PlayerList_KEY, Int_A1_IDList },
                    }; 
                    return Hsh_MapKeys;
                #endregion Synchronize
            }
            public void OverwritePlayerList(BasePlayer[] Ctm_BP_A1_NewList)
            {
                O_Ctm_BP_A1_PlayerList = Ctm_BP_A1_NewList;
            }
            public void SpawnCharacter(int Int_ID)
            {
                #region    Synchronize
                    Hashtable Hsh_MapKeys = new Hashtable
                    {
                        { PlayerID_KEY, Int_ID },
                    }; 
                    ReSpawnCharacter(Hsh_MapKeys);
                #endregion Synchronize
            }
            public void KillCharacter(int Int_ID)
            {
                #region    Synchronize
                    Hashtable Hsh_MapKeys = new Hashtable
                    {
                        { PlayerID_KEY, Int_ID },
                    };
                    KillCharacter(Hsh_MapKeys);
                #endregion Synchronize
            }
            // Reposiciona sin curar - Se usa cuando la bala pega pero el jugador sobrevive //
            public void RepositionCharacter(int Int_ID)
            {
                #region    Synchronize
                    Hashtable Hsh_MapKeys = new Hashtable
                    {
                        { PlayerID_KEY, Int_ID },
                    };
                    GetComponent<PhotonView>().RPC(nameof(RPC_AlertCharacterRepositionToClients), RpcTarget.All, Hsh_MapKeys);
                #endregion Synchronize
            }
            // Todo el dano pasa por aca para que la vida sea igual en todos los clientes //
            public void DamageCharacter(int Int_ID, float Flt_Damage)
            {
                GetComponent<PhotonView>().RPC(nameof(RPC_DamageCharacter), RpcTarget.All, Int_ID, Flt_Damage);
            }
            #region    Condiciones de Victoria
                [Header(" Condiciones de Victoria")]
                    [SerializeField] private int _killsToWin = 3;
                private int R_Int_ChaserDeaths = 0;
                private bool R_Bool_GameOver = false;
                public bool GameOver => R_Bool_GameOver;
                private void RegisterChaserDeath(int Int_ID)
                {
                    // Salir si ya termino - Cada rama avisa, si no la cuenta falla en silencio //
                        if (R_Bool_GameOver)
                        {
                            Debug.LogWarning("[Character Manager] Muerte ignorada: la partida ya figura terminada (GameOver quedo en true)");
                            return;
                        }
                    // Solo cuentan los Corredores //
                        PhotonView PV_Dead = PhotonView.Find(Int_ID);
                        if (PV_Dead == null)
                        {
                            Debug.LogWarning("[Character Manager] Muerte ignorada: no se encontro el PhotonView " + Int_ID);
                            return;
                        }
                    // Quien cayo NO tiene que ser el Vigilante //
                    // Se mira el dueño del PhotonView contra la Room Property, no el enum del BasePlayer: //
                    // PlayerType.Vigilant es el indice 0, o sea el valor por defecto, asi que si el SetData //
                    // todavia no llego a esta maquina cualquier Corredor figura como Vigilante y no se contaba //
                        int Int_DeadActor = (PV_Dead.Owner != null) ? PV_Dead.Owner.ActorNumber : -1;
                        if (Int_DeadActor == GetVigilantActorNumber())
                        {
                            Debug.Log("[Character Manager] Muerte ignorada: cayo el Vigilante (Actor " + Int_DeadActor + ")");
                            return;
                        }
                    // Sumar muerte //
                        R_Int_ChaserDeaths++;
                        Debug.Log("[Character Manager] Corredor eliminado (" + R_Int_ChaserDeaths + "/" + _killsToWin + ")");
                    // Gana el Vigilante //
                        if (R_Int_ChaserDeaths >= _killsToWin)
                            EndGame(true);
                }
                public void EndGame(bool Bool_VigilantWon)
                {
                    if (R_Bool_GameOver) return;
                    GetComponent<PhotonView>().RPC(nameof(RPC_EndGame), RpcTarget.All, Bool_VigilantWon);
                    MasterManager.Instance.MenuManager.SwitchScene(1);
                }
                [SerializeField] private float _restartDelay = 5f;
                [SerializeField] private float _cleanRestartPause = 1.5f;
                private System.Collections.IEnumerator RestartRoundRoutine()
                {
                    yield return new WaitForSecondsRealtime(_restartDelay);
                    // Sacar la pantalla de resultado en TODOS los clientes //
                        GameResultScreen.Hide();
                    // Salir si ya no estamos en la sala //
                        if (!PhotonNetwork.InRoom)
                        {
                            Debug.LogWarning("[Character Manager] No se pudo reiniciar: ya no estamos en la sala");
                            yield break;
                        }
                    // El Master maneja el cambio de escena, el resto lo sigue por AutomaticallySyncScene //
                        if (!PhotonNetwork.IsMasterClient) yield break;
                    // Reinicio limpio - Se pasa por el menu para que la ronda nueva arranque igual que la primera //
                    // Recargar el nivel encima de si mismo dejaba la camara y los managers con datos de la ronda vieja //
                    // El segundo paso lo maneja GameResultScreen desde el evento de carga de escena, //
                    // porque esta corrutina muere apenas cambia la escena //
                        GameResultScreen.RequestCleanRestart(_cleanRestartPause);
                }
            #endregion Condiciones de Victoria
        #endregion Custom Methods
        #region    PUN         
            #region    Hastable
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
            #endregion Hastable  
            #region    RPC
                #region    Regular
                        public void UpdateAllCharacterManagers(Hashtable Hsh_Input)
                        {
                            GetComponent<PhotonView>().RPC(nameof(RPC_SpawnCharacter), RpcTarget.All, Hsh_Input); 
                        }
                        public void KillCharacter(Hashtable Hsh_Input)
                        {
                            GetComponent<PhotonView>().RPC(nameof(RPC_AlertCharacterDeathToClients), RpcTarget.All, Hsh_Input); 
                        }
                        public void ReSpawnCharacter(Hashtable Hsh_Input)
                        {
                            GetComponent<PhotonView>().RPC(nameof(RPC_AlertCharacterRespawnToClients), RpcTarget.All, Hsh_Input); 
                        }
                        public void ChangePlayerMaterial(BasePlayer Ctm_BP_Input = null, int Int_Input = -1)
                        {
                            if (Ctm_BP_Input == null)
                                Ctm_BP_Input = O_Ctm_BP_OwnPlayer;
                            Hashtable Hsh_MapKeys = new Hashtable
                            {
                                { PlayerID_KEY, Ctm_BP_Input.GetComponent<PhotonView>().ViewID },
                                { PlayerMatIndex_KEY, Int_Input }
                            }; 
                            GetComponent<PhotonView>().RPC(nameof(RPC_CharacterMaterial), RpcTarget.All, Hsh_MapKeys); 
                        }
                #endregion Regular
                #region    Hashtables
                    // Character //
                        [PunRPC]
                        private void RPC_CharacterMaterial(Hashtable Hsh_Input)
                        {
                            // Variables //
                                BasePlayer Ctm_BP_Client = (PhotonView.Find((int)Hsh_Input[PlayerID_KEY])).GetComponent<BasePlayer>();
                            // Set Up //
                                Ctm_BP_Client.SetMaterial((int)Hsh_Input[PlayerMatIndex_KEY]); 
                        }
                        [PunRPC]
                        private void RPC_SpawnCharacter(Hashtable Hsh_Input)
                        {
                            // Variables //
                                BasePlayer Ctm_BP_Client = (PhotonView.Find((int)Hsh_Input[PlayerID_KEY])).GetComponent<BasePlayer>();
                            // Set Up //
                                Ctm_BP_Client.SetData((string)Hsh_Input[PlayerName_KEY], (int)Hsh_Input[PlayerID_KEY], IO_Flt_MaxHealth, (PlayerType)Hsh_Input[PlayerType_KEY]); 
                                Ctm_BP_Client.SetVisibility(true);
                                ChangePlayerMaterial(Ctm_BP_Client);
                                PhotonNetwork.SendAllOutgoingCommands();
                            // Force Update Other Client's Room Prefab //
                                GetComponent<PhotonView>().RPC(nameof(RPC_AddCharacterToMaster),RpcTarget.MasterClient, Hsh_Input);
                        }
                        [PunRPC]
                        private void RPC_AddCharacterToMaster(Hashtable Hsh_Input)
                        {
                            if (PhotonNetwork.IsMasterClient)
                            {
                                if (IsPlayerIDHashtable(Hsh_Input))
                                {
                                    // Variables //
                                        BasePlayer Ctm_BP_Client = (PhotonView.Find((int)Hsh_Input[PlayerID_KEY])).GetComponent<BasePlayer>();
                                    // Set Up //
                                        Hashtable Hsh_NewList = MasterManager.Instance.CharacterManager.AddToPlayerList(Ctm_BP_Client);
                                    // Force Update Other Client's Room Prefab //
                                        GetComponent<PhotonView>().RPC(nameof(RPC_ForceCharacterUpdateToClients),RpcTarget.All, Hsh_NewList);
                                }
                            }
                        }
                        [PunRPC]
                        private void RPC_ForceCharacterUpdateToClients(Hashtable Hsh_Input)
                        {
                            if (IsPlayerListHashtable(Hsh_Input))
                            {
                                if (!PhotonNetwork.IsMasterClient)
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
                        private void RPC_DamageCharacter(int Int_ID, float Flt_Damage)
                        {
                            // Variables //
                                PhotonView PV_Target = PhotonView.Find(Int_ID);
                                if (PV_Target == null)
                                {
                                    Debug.LogWarning("[Character Manager] Dano perdido: no se encontro el PhotonView " + Int_ID);
                                    return;
                                }
                                BasePlayer Ctm_BP_Target = PV_Target.GetComponent<BasePlayer>();
                                if (Ctm_BP_Target == null) return;
                            // Solo los Corredores reciben dano - Se mira el dueño, no el enum (default = Vigilant) //
                                int Int_TargetActor = (PV_Target.Owner != null) ? PV_Target.Owner.ActorNumber : -1;
                                if (Int_TargetActor == GetVigilantActorNumber())
                                {
                                    Debug.Log("[Character Manager] Dano ignorado: el objetivo es el Vigilante");
                                    return;
                                }
                            // Aplicar en TODOS los clientes para que la vida quede igual en todos //
                                bool Bool_Survived = Ctm_BP_Target.ExecuteDamage(Flt_Damage);
                                Debug.Log("[Character Manager] Dano " + Flt_Damage + " a " + Int_ID + " -> vida " + Ctm_BP_Target.Health + (Bool_Survived ? " (sigue vivo)" : " (cae)"));
                            // Solo el Master decide muerte o reposicion, asi no se duplica //
                                if (!PhotonNetwork.IsMasterClient) return;
                                if (Bool_Survived) RepositionCharacter(Int_ID);
                                else               KillCharacter(Int_ID);
                        }
                        [PunRPC]
                        private void RPC_AlertCharacterRepositionToClients(Hashtable Hsh_Input)
                        {
                            if (IsPlayerIDHashtable(Hsh_Input))
                            {
                                // Variables //
                                    PhotonView PV_Target = PhotonView.Find((int)Hsh_Input[PlayerID_KEY]);
                                    if (PV_Target == null) return;
                                    BasePlayer Ctm_BP_Client = PV_Target.GetComponent<BasePlayer>();
                                    if (Ctm_BP_Client == null) return;
                                // Mover sin curar //
                                    Ctm_BP_Client.ExecuteSpawn(false);
                            }
                        }
                        [PunRPC]
                        private void RPC_AlertCharacterDeathToClients(Hashtable Hsh_Input)
                        {
                            if (IsPlayerIDHashtable(Hsh_Input))
                            {
                                // Variables //
                                    PhotonView PV_Target = PhotonView.Find((int)Hsh_Input[PlayerID_KEY]);
                                    if (PV_Target == null) return;
                                    BasePlayer Ctm_BP_Client = PV_Target.GetComponent<BasePlayer>();
                                    if (Ctm_BP_Client == null) return;
                                // Ya estaba caido - No se cuenta de nuevo //
                                    if (Ctm_BP_Client.IsDead)
                                    {
                                        Debug.Log("[Character Manager] Muerte repetida ignorada para " + Ctm_BP_Client.ID);
                                        return;
                                    }
                                // Kill //
                                    Debug.Log("[Character Manager] Cae el jugador " + Ctm_BP_Client.ID + " (" + Ctm_BP_Client.ClientType + ")");
                                    Ctm_BP_Client.ExecuteDeath();
                                // Victoria - El Master lleva la cuenta de corredores caidos //
                                    if (PhotonNetwork.IsMasterClient) RegisterChaserDeath((int)Hsh_Input[PlayerID_KEY]);
                            }
                        }
                        [PunRPC]
                        private void RPC_EndGame(bool Bool_VigilantWon)
                        {
                            // Evitar doble disparo //
                                if (R_Bool_GameOver) return;
                                R_Bool_GameOver = true;
                            // Gano este cliente ? //
                                bool Bool_IAmVigilant = (IO_E_PT_ClientType == PlayerType.Vigilant);
                                bool Bool_IWon = (Bool_IAmVigilant == Bool_VigilantWon);
                            // Motivo - Cambia segun el rol de quien lee //
                                string Str_Reason;
                                if (Bool_VigilantWon)
                                    Str_Reason = Bool_IAmVigilant ?
                                                    "Eliminaste a los Corredores " + _killsToWin + " veces." :
                                                    "El Vigilante eliminó a los Corredores " + _killsToWin + " veces.";
                                else
                                    Str_Reason = Bool_IAmVigilant ?
                                                    "Un Corredor te alcanzó." :
                                                    "Los Corredores alcanzaron al Vigilante.";
                            // Cortar el juego //
                                MasterManager.Instance.GameManager.GameEnd();
                            // Pantalla propia - No usa el ErrorMenu, asi victoria y derrota se ven distintas //
                                GameResultScreen.Show(Bool_IWon, Str_Reason, _restartDelay);
                            Debug.Log("[Character Manager] Fin de partida. Gano " + (Bool_VigilantWon ? "el Vigilante" : "los Corredores") + ". Este cliente es " + IO_E_PT_ClientType + " -> " + (Bool_IWon ? "VICTORIA" : "DERROTA"));
                            // Reinicio de ronda - No hace falta rehacer la sala //
                                StartCoroutine(RestartRoundRoutine());
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
                #endregion Hashtables 
            #endregion RPC
        #endregion PUN
    #endregion Methods
}
