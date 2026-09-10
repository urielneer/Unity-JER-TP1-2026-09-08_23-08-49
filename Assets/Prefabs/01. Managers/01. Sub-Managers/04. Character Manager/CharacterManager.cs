using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using static CustomExtension.ArrayExtensions;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CharacterManager : BaseManager<CharacterManager>
{
    [Header("All Character Settings")]
    [SerializeField] private float _maxHealth;
    private float IO_Flt_MaxHealth;
    public float MaxHealth => IO_Flt_MaxHealth;

    [Header("Manager Settings")]
    [SerializeField] private Transform _mapContainer;
    private Transform I_Trfm_MapContainer;

    [SerializeField] private float _shootCooldown;
    private float IO_Flt_ShootCooldown;
    public float ShootCooldown => IO_Flt_ShootCooldown;

    private BasePlayer O_Ctm_BP_OwnPlayer;
    public BasePlayer OwnPlayer => O_Ctm_BP_OwnPlayer;

    private BasePlayer[] O_Ctm_BP_A1_PlayerList;
    public BasePlayer[] PlayerList => O_Ctm_BP_A1_PlayerList;

    private const string PlayerID_KEY = "PlayerID";
    private const string PlayerList_KEY = "PlayerList";

    protected override void Awake()
    {
        base.Awake();
        I_Trfm_MapContainer = _mapContainer;
        IO_Flt_MaxHealth = _maxHealth;
        IO_Flt_ShootCooldown = _shootCooldown;
    }

    public override void OnStartUp()
    {
        base.OnStartUp();
        O_Ctm_BP_A1_PlayerList = new BasePlayer[0];

        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                break;
            case 1:
                Vector3 Vec3_Pos = Vector3.zero;
                Quaternion Quat_Rot = Quaternion.identity;

                BasePlayer Ctm_BP_Own = PhotonNetwork.Instantiate("Tank_Master", Vec3_Pos, Quat_Rot).GetComponent<BasePlayer>();
                O_Ctm_BP_OwnPlayer = Ctm_BP_Own;
                Ctm_BP_Own.SetData(PhotonNetwork.LocalPlayer.NickName, Ctm_BP_Own.GetComponent<PhotonView>().ViewID, IO_Flt_MaxHealth);
                O_Ctm_BP_OwnPlayer.OnStartUp();

                I_Trfm_MapContainer = Object.FindFirstObjectByType<MapContainer>().gameObject.transform;
                Ctm_BP_Own.gameObject.transform.SetParent(I_Trfm_MapContainer, true);

                Hashtable Hsh_MapKeys = new Hashtable
                {
                    { PlayerID_KEY, Ctm_BP_Own.GetComponent<PhotonView>().ViewID },
                };

                I_Trfm_MapContainer.GetComponent<MapContainer>().UpdateAllCharacterManagers(Hsh_MapKeys);
                break;
        }
    }

    public override void OnUpdate(float Flt_FixedDT)
    {
        base.OnUpdate(Flt_FixedDT);

        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                break;
            case 1:
                if (O_Ctm_BP_A1_PlayerList.Length == 0) return;
                foreach (BasePlayer Ctm_BP_Player in O_Ctm_BP_A1_PlayerList)
                {
                    Ctm_BP_Player.OnUpdate(); // CS1501 Fix
                }
                break;
        }
    }

    public override void OnFixedUpdate(float Flt_FixedDT)
    {
        base.OnFixedUpdate(Flt_FixedDT);

        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                break;
            case 1:
                if (O_Ctm_BP_A1_PlayerList.Length == 0) return;
                foreach (BasePlayer Ctm_BP_Player in O_Ctm_BP_A1_PlayerList)
                {
                    Ctm_BP_Player.OnFixedUpdate(); // CS1501 Fix
                }
                break;
        }
    }

    public Hashtable AddToPlayerList(BasePlayer Ctm_BP_Client)
    {
        AddNewToArray(ref O_Ctm_BP_A1_PlayerList, Ctm_BP_Client);

        int[] Int_A1_IDList = new int[0];
        foreach (BasePlayer Ctm_BP_Element in O_Ctm_BP_A1_PlayerList)
        {
            AddNewToArray(ref Int_A1_IDList, Ctm_BP_Element.GetComponent<PhotonView>().ViewID);
        }

        Hashtable Hsh_MapKeys = new Hashtable
        {
            { PlayerList_KEY, Int_A1_IDList },
        };

        return Hsh_MapKeys;
    }

    public void OverwritePlayerList(BasePlayer[] Ctm_BP_A1_NewList)
    {
        O_Ctm_BP_A1_PlayerList = Ctm_BP_A1_NewList;
    }

    public void SpawnCharacter(int Int_ID)
    {
        Hashtable Hsh_MapKeys = new Hashtable
        {
            { PlayerID_KEY, Int_ID },
        };

        I_Trfm_MapContainer.GetComponent<MapContainer>().ReSpawnCharacter(Hsh_MapKeys);
    }

    public void KillCharacter(int Int_ID)
    {
        Hashtable Hsh_MapKeys = new Hashtable
        {
            { PlayerID_KEY, Int_ID },
        };

        I_Trfm_MapContainer.GetComponent<MapContainer>().KillCharacter(Hsh_MapKeys);
    }
}