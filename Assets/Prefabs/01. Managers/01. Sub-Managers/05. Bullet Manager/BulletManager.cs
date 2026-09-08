using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using static CustomExtension.ArrayExtensions;

#region    "using" Script Clarification
    using Hashtable = ExitGames.Client.Photon.Hashtable;
#endregion "using" Script Clarification

public class BulletManager : BaseManager<BulletManager>
{ 
    #region    Variables
        [Header(" All Character Settings")]
            [SerializeField] float _speed;
            float I_Flt_Speed;
            [SerializeField] float _damage;
            float I_Flt_Damage;
        [Header(" Manager Settings")]
            [SerializeField] Bullet _prefab;
            Bullet I_Ctm_Bllt_Prefab;
            [SerializeField] private Transform _mapContainer;
            private Transform I_Trfm_MapContainer;
        #region    Hashtables (Communication among instances)
            private const string BulletOwnerID_KEY = "BulletOwnerID";
            private const string BulletOwnerName_KEY = "BulletOwnerName";
            private const string BulletPos_KEY = "BulletPos";
            private const string BulletQuat_KEY = "BulletQuat";
            private const string BulletDir_KEY = "BulletDir";
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
                    I_Flt_Speed = _speed;
                    I_Flt_Damage = _damage;
                    I_Ctm_Bllt_Prefab = _prefab;
            }
        #endregion Unity Methods
        #region    Override Methods
            public override void OnStartUp()
            {
                // Resume Base //
                    base.OnStartUp();
                // Scene dependant Behaviour //
                    switch (SceneManager.GetActiveScene().buildIndex) 
                    {
                        case 0:
                        break;
                        case 1: // Level //
                            // MapManager - Container //
                                I_Trfm_MapContainer = (Object.FindFirstObjectByType<MapContainer>()).gameObject.transform;
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
                        break;
                    }
            }
        #endregion Override Methods
        #region    Custom Methods
            public void SpawnBullet(string Str_OwnerNickname, int Int_ID, Vector3 Vec3_Pos, Quaternion Quat_Rot, Vector3 Vec3_Dir)
            {
                #region    Spawn
                    // Spawn Bullet //
                        Bullet Ctm_Bllt_Own = (Instantiate(I_Ctm_Bllt_Prefab, Vec3_Pos, Quat_Rot)).GetComponent<Bullet>();
                    // Setup Bullets Dir & Speed //
                        Ctm_Bllt_Own.SetData(Str_OwnerNickname, Int_ID, Vec3_Dir, I_Flt_Speed, I_Flt_Damage);
                    // Start Up // 
                        Ctm_Bllt_Own.OnStartUp();
                    // MapManager - Container //
                        Ctm_Bllt_Own.gameObject.transform.SetParent(I_Trfm_MapContainer, true);
                #endregion Spawn
            }
            public void SynchronizeBullet(BasePlayer Ctm_BP_Owner, Vector3 Vec3_Dir)
            {
                // Variables //
                    Transform Tfm_SpawnSource = Ctm_BP_Owner.CanonAnchor;
                    Vector3 Vec3_Pos = Tfm_SpawnSource.transform.position;
                    Quaternion Quat_Rot = Tfm_SpawnSource.transform.rotation;
                // Synchronize //
                    Hashtable Hsh_MapKeys = new Hashtable
                    {
                        { BulletOwnerName_KEY , Ctm_BP_Owner.OwnerNickname },
                        { BulletOwnerID_KEY   , Ctm_BP_Owner.ID            },
                        { BulletPos_KEY       , Vec3_Pos                   },
                        { BulletQuat_KEY      , Quat_Rot                   },
                        { BulletDir_KEY       , Vec3_Dir                   },
                    }; 
                    I_Trfm_MapContainer.GetComponent<MapContainer>().UpdateAllBulletManagers(Hsh_MapKeys);
            }
        #endregion Custom Methods
    #endregion Methods
}
