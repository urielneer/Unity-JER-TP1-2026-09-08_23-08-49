using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using static CustomExtension.ArrayExtensions;

#region    "using" Script Clarification
    using Hashtable = ExitGames.Client.Photon.Hashtable;
#endregion "using" Script Clarification
public class CharacterManager : BaseManager<CharacterManager>
{
    #region    Variables
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
            public async override void OnStartUp()
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
                            // Variables //
                                Vector3 Vec3_Pos = Vector3.zero;
                                Quaternion Quat_Rot = Quaternion.identity;
                            // Spawn Player //
                                BasePlayer Ctm_BP_Own = (PhotonNetwork.Instantiate("Tank_Master", Vec3_Pos, Quat_Rot)).GetComponent<BasePlayer>();
                                O_Ctm_BP_OwnPlayer = Ctm_BP_Own;
                                Ctm_BP_Own.SetData(PhotonNetwork.LocalPlayer.NickName, Ctm_BP_Own.GetComponent<PhotonView>().ViewID, IO_Flt_MaxHealth);
                                O_Ctm_BP_OwnPlayer.OnStartUp();
                            // MapManager - Container //
                                I_Trfm_MapContainer = (Object.FindFirstObjectByType<MapContainer>()).gameObject.transform;
                                Ctm_BP_Own.gameObject.transform.SetParent(I_Trfm_MapContainer, true);
                            #region    Synchronize
                                Hashtable Hsh_MapKeys = new Hashtable
                                {
                                    { PlayerID_KEY, Ctm_BP_Own.GetComponent<PhotonView>().ViewID },
                                }; 
                                I_Trfm_MapContainer.GetComponent<MapContainer>().UpdateAllCharacterManagers(Hsh_MapKeys);
                            #endregion Synchronize
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
                    I_Trfm_MapContainer.GetComponent<MapContainer>().ReSpawnCharacter(Hsh_MapKeys);
                #endregion Synchronize
            }
            public void KillCharacter(int Int_ID)
            {
                #region    Synchronize
                    Hashtable Hsh_MapKeys = new Hashtable
                    {
                        { PlayerID_KEY, Int_ID },
                    }; 
                    I_Trfm_MapContainer.GetComponent<MapContainer>().KillCharacter(Hsh_MapKeys);
                #endregion Synchronize
            }
        #endregion Custom Methods
    #endregion Methods
}
