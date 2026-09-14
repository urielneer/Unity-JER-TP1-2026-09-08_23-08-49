using Photon.Pun;
using UnityEngine;
using static CustomExtension.ArrayExtensions;

public class Bullet : MonoBehaviourPunCallbacks
{
    #region    Variables
            BasePlayer IO_Ctm_BP_Owner;
            BasePlayer PlayerOwner => IO_Ctm_BP_Owner;
            string I_Str_OwnerNickname;
            int I_Int_BP_OwnerID;
            Vector3 I_Vec3_BP_Dir;
            float I_Flt_Speed;
            float I_Flt_Damage;
            int I_Int_Type;
            BasePlayer R_Ctm_BP_Target;

            Vector3 R_Vec3_Speed;
        [Header(" Bullet Settings")]
            [SerializeField] float _lifetime;
            float I_Flt_Lifetime;
            [SerializeField] LayerMask _detectionLayers;
            LayerMask I_LyrM_DetectionLayers;
            [SerializeField] Material[] _materials;
            private Material[] I_Mat_A1_Materials;

    [SerializeField] private GameObject _icePrefab;
    #endregion Variables
    #region    Methods
    #region    Unity Methods
    public void Awake()
            {
                I_Mat_A1_Materials = _materials;
                I_Flt_Lifetime = _lifetime;
                I_LyrM_DetectionLayers = _detectionLayers;
            }public override void OnEnable()
            {
                base.OnEnable();
                // Ensure bullet is unparented to prevent remote transform hierarchy jitter
                transform.SetParent(null);
            }
            void Update() 
            {
                /* Managing each in a manager would be a hassle as long as this  
                uses a "Peer To Peer" like logic. Each bullet would have it's  
                own Photon View ID (which would ruin everything) */
                OnUpdate(Time.fixedDeltaTime);
            }
        #endregion Unity Methods
        #region    Override Methods
            public virtual void OnStartUp()
            {
                Destroy(this.gameObject, I_Flt_Lifetime);
            }
            public virtual void OnUpdate(float Flt_FixedDT)
            {
                ExecuteMovement(Flt_FixedDT);
            }
        #endregion Override Methods
        #region    Custom Methods
            #region    Set Up
                public void SetData(string Str_Input, int Int_Input, Vector3 Vec3_Input, float Flt_Input1, float Flt_Input2, int Int_Input2 = 0)
                {
                    I_Str_OwnerNickname = Str_Input;
                    I_Int_BP_OwnerID = Int_Input;
                    I_Vec3_BP_Dir = Vec3_Input;
                    I_Flt_Speed = Flt_Input1;
                    I_Flt_Damage = Flt_Input2;
                    I_Int_Type = Int_Input2;

                    R_Vec3_Speed = I_Vec3_BP_Dir * I_Flt_Speed;
                    R_Ctm_BP_Target = null;

                    this.gameObject.GetComponent<MeshRenderer>().material = I_Mat_A1_Materials[I_Int_Type];
                }
            #endregion Set Up
            #region    Damage
                public void OnHit(Collider Col_Hit)
                {
                    // Is Own Source? //
                        if (Col_Hit.gameObject.layer == 8 && Col_Hit.TryGetComponent<BasePlayer>(out BasePlayer Ctm_BP_Target1)) 
                        {
                            if (Ctm_BP_Target1.ID != I_Int_BP_OwnerID)
                            { 
                                R_Ctm_BP_Target = Ctm_BP_Target1;
                                Destroy(this.gameObject);
                            }
                        }
                    // Is Own's Child Source? //
                        else
                        if (Col_Hit.gameObject.layer == 8 && Col_Hit.transform.parent.gameObject.TryGetComponent<BasePlayer>(out BasePlayer Ctm_BP_Target2))
                        {
                            if (Ctm_BP_Target2.ID != I_Int_BP_OwnerID)
                            { 
                                R_Ctm_BP_Target = Ctm_BP_Target2;
                                Destroy(this.gameObject);
                            }
                        }
                        // Is Wall or Obstacle? //
                        else if ((5 < Col_Hit.gameObject.layer) && (Col_Hit.gameObject.layer < 8))
                        {
                            // Si es Hielo (Tipo 3), spawnea el piso antes de borrarse //
                            if (I_Int_Type == 3)
                            {
                                GameObject icePrefab = Resources.Load<GameObject>("03. Obstacles/IceField");
                                if (icePrefab != null)
                                {
                                    Vector3 spawnPos = new Vector3(transform.position.x, 0.05f, transform.position.z);
                                    Instantiate(icePrefab, spawnPos, Quaternion.identity);
                                    Debug.Log("[Hielo Spawneado con �xito en: " + spawnPos + "]");
                                }
                            }

                            R_Ctm_BP_Target = null;
                            Destroy(this.gameObject);
                        }

    }
            #endregion Damage
            #region    Movement
                public void ExecuteMovement(float Flt_FixedDT)
                {
                    // Variables //
                        float Flt_Distance = R_Vec3_Speed.magnitude * Flt_FixedDT;
                        Vector3 Vec3_Step = R_Vec3_Speed * Flt_FixedDT;
                        Bounds Bnds_Self = this.gameObject.GetComponent<MeshCollider>().bounds;
                    // Proceed ? //
                        if (Flt_Distance <= 0f) return;
                    // Collisions //
                        if (Physics.Raycast(transform.position, I_Vec3_BP_Dir, out RaycastHit hit, Flt_Distance, I_LyrM_DetectionLayers))
                        {
                            transform.position = hit.point;
                            OnHit(hit.collider);
                            return;
                        }
                    // Move //
                        transform.Translate(Vec3_Step, Space.World);
                }
    #endregion Movement
    #endregion Custom Methods
    #endregion Methods

    #region    External Classe Methods
    // Solo el cliente que disparo dispara la parte de red, si no se duplica por cada copia local de la bala //
    private bool IsOwnerLocal()
    {
        if (MasterManager.Instance == null) return false;
        if (MasterManager.Instance.CharacterManager == null) return false;
        BasePlayer Ctm_BP_Own = MasterManager.Instance.CharacterManager.OwnPlayer;
        return (Ctm_BP_Own != null && Ctm_BP_Own.ID == I_Int_BP_OwnerID);
    }
    // Todo impacto: hace dano y reposiciona al jugador //
    // El dano va por el Character Manager para que la vida baje igual en todos los clientes //
    private void ApplyHit(BasePlayer Ctm_BP_Target)
    {
        if (Ctm_BP_Target == null) return;
        if (Ctm_BP_Target.ID == I_Int_BP_OwnerID) return;
        if (!IsOwnerLocal()) return;
        Debug.Log("[Bullet] Impacto tipo " + I_Int_Type + " sobre " + Ctm_BP_Target.ID + " por " + I_Flt_Damage);
        MasterManager.Instance.CharacterManager.DamageCharacter(Ctm_BP_Target.ID, I_Flt_Damage);
    }
    private void OnDestroy()
    {
        // Special Behaviour //
        switch (I_Int_Type)
        {
            case 0:
                // Regular Rock Behaviour (Impacto directo) //
                if (R_Ctm_BP_Target != null)
                {
                    R_Ctm_BP_Target.HijackPush(this.transform.forward, 10f);
                }
                ApplyHit(R_Ctm_BP_Target);
                break;

            case 1:
                // Flip (Impacto directo) //
                if (R_Ctm_BP_Target != null)
                {
                    R_Ctm_BP_Target.ExecuteFlip();
                }
                ApplyHit(R_Ctm_BP_Target);
                break;

            case 2:
                // Fan / Ventilador (�rea / R�faga) //
                Collider[] Col_A1_HitCollidersWind = Physics.OverlapSphere(this.transform.position, 5f, I_LyrM_DetectionLayers);
                foreach (Collider Col_Hit in Col_A1_HitCollidersWind)
                {
                    if (Col_Hit.TryGetComponent<BasePlayer>(out BasePlayer target))
                    {
                        target.HijackPush(this.transform.forward, 15f);
                        ApplyHit(target);
                    }
                }
                break;

            case 3:
                // Freeze / Campo Helado //
                if (_icePrefab != null)
                {
                    Vector3 spawnPos = transform.position;
                    if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 20f))
                    {
                        spawnPos = hit.point + Vector3.up * 0.05f;
                    }
                    Instantiate(_icePrefab, spawnPos, Quaternion.identity);
                }
                ApplyHit(R_Ctm_BP_Target);
                break;

            case 4:
                // Slow Potion (�rea) //
                Collider[] Col_A1_HitColliders1 = Physics.OverlapSphere(this.transform.position, 4f, I_LyrM_DetectionLayers);
                foreach (Collider Col_Hit in Col_A1_HitColliders1)
                {
                    if (Col_Hit.TryGetComponent<BasePlayer>(out BasePlayer target))
                    {
                        target.UsePotion(4f, 0.4f);
                        ApplyHit(target);
                    }
                }
                break;

            case 5:
                // Banana (Impacto directo) //
                if (R_Ctm_BP_Target != null)
                {
                    R_Ctm_BP_Target.ExecuteFalling();
                }
                ApplyHit(R_Ctm_BP_Target);
                break;

            case 6:
                // Fast Potion (Consumo directo) //
                if (R_Ctm_BP_Target != null)
                {
                    R_Ctm_BP_Target.UsePotion(4f, 1.8f);
                }
                ApplyHit(R_Ctm_BP_Target);
                break;

            case 7:
                // Bomb (�rea) //
                Collider[] Col_A1_HitColliders3 = Physics.OverlapSphere(this.transform.position, 6f, I_LyrM_DetectionLayers);
                foreach (Collider Col_Hit in Col_A1_HitColliders3)
                {
                    if (Col_Hit.TryGetComponent<BasePlayer>(out BasePlayer target))
                    {
                        target.HijackBomb(this.transform.position, 18f, 6f, 1.5f);
                        target.ExecuteFalling();
                        ApplyHit(target);
                    }
                }
                break;
        }
    }
    #endregion External Classe Methods
}
