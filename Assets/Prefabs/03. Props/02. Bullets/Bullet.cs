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
    #endregion Variables
    #region    Methods
        #region    Unity Methods
            public void Awake()
            {
                I_Mat_A1_Materials = _materials;
                I_Flt_Lifetime = _lifetime;
                I_LyrM_DetectionLayers = _detectionLayers;
            }
            void FixedUpdate() 
            {
                /* Managing each in a manager would be a hassle as long as this  
                uses a "Peer To Peer" like logic. Each bullet would have it's  
                own Photon View ID (which would ruin everything) */
                OnFixedUpdate(Time.fixedDeltaTime);
            }
        #endregion Unity Methods
        #region    Override Methods
            public virtual void OnStartUp()
            {
                R_Vec3_Speed = I_Vec3_BP_Dir * I_Flt_Speed;
                R_Ctm_BP_Target = null;
                Destroy(this.gameObject, I_Flt_Lifetime);
            }
            public virtual void OnFixedUpdate(float Flt_FixedDT)
            {
                ExecuteMovement(Flt_FixedDT);
            }
        #endregion Override Methods
        #region    Custom Methods
            #region    Set Up
                public void SetData(string Str_Input, int Int_Input, Vector3 Vec3_Input, float Flt_Input1, float Flt_Input2, int Int_Input2)
                {
                    I_Str_OwnerNickname = Str_Input;
                    I_Int_BP_OwnerID = Int_Input;
                    I_Vec3_BP_Dir = Vec3_Input;
                    I_Flt_Speed = Flt_Input1;
                    I_Flt_Damage = Flt_Input2;
                    I_Int_Type = Int_Input2;
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
                        else 
                        if ( (5 < Col_Hit.gameObject.layer) && (Col_Hit.gameObject.layer < 8) )
                        { 
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
                        Bounds Bnds_Self = this.gameObject.GetComponent<MeshCollider>().bounds;
                        Collider[] Col_A1_HitBuffer = new Collider[10];
                        int Int_Count  = Physics.OverlapBoxNonAlloc(
                                                                        Bnds_Self.center,
                                                                        Bnds_Self.extents,
                                                                        Col_A1_HitBuffer,
                                                                        transform.rotation,
                                                                        I_LyrM_DetectionLayers
                                                                   );
                    // Collisions //
                        foreach (Collider Col_Hit in Col_A1_HitBuffer)
                        {
                            // Ignore Own Collision //
                                if (Col_Hit == this.gameObject.GetComponent<MeshCollider>()) continue;
                            // Ignore null //
                                if (Col_Hit == null) continue;
                            // Compute Overlap //
                                bool Bool_IsOverlapping = Physics.ComputePenetration(
                                                                                        // In //
                                                                                            this.gameObject.GetComponent<MeshCollider>(),
                                                                                            this.gameObject.GetComponent<MeshCollider>().transform.position,
                                                                                            this.gameObject.GetComponent<MeshCollider>().transform.rotation,
                                                                                            Col_Hit,
                                                                                            Col_Hit.transform.position,
                                                                                            Col_Hit.transform.rotation,
                                                                                        // Out //
                                                                                            out Vector3 Vec3_Dir,
                                                                                            out float Flt_OverlappingDistance
                                                                                    );
                                if ( Bool_IsOverlapping )
                                { 
                                    OnHit(Col_Hit);
                                }
                        }
                    // Move //
                        transform.Translate(R_Vec3_Speed * Flt_FixedDT, Space.World);
                }
            #endregion Movement
        #endregion Custom Methods
    #endregion Methods

    #region    External Classe Methods
        private void OnDestroy()
        {
            // Exit ? //
                if (R_Ctm_BP_Target == null) return;
            //  Special Behaviour //
                switch (I_Int_Type)
                {
                    case 0:
                        // Regular Rock Behaviour //
                            R_Ctm_BP_Target.ExecuteDamage(I_Flt_Damage);
                            R_Ctm_BP_Target.HijackPush(this.transform.forward, 10f);
                    break;
                    case 1:
                        // Flip //
                            R_Ctm_BP_Target.ExecuteFlip();
                    break;
                    case 2:
                        // Fan //
                            ///////// Send this transform.position for a spawn point
                    break;
                    case 3:
                        // Freeze /
                            ///////// PHYSICAL MATERIAL 
                    break;
                    case 4:
                        // Slow Potion //
                            float Flt_PotionRadius1 = 0;
                            Collider[] Col_A1_HitColliders1 = Physics.OverlapSphere(
                                                                                    this.transform.position,
                                                                                    Flt_PotionRadius1
                                                                                  );
                            foreach (Collider Col_Hit in Col_A1_HitColliders1 )
                            {
                                R_Ctm_BP_Target.UsePotion(0f,0f);
                            }
                    break;
                    case 5:
                        // Banana //
                            R_Ctm_BP_Target.ExecuteDamage(I_Flt_Damage);
                            R_Ctm_BP_Target.ExecuteFalling();
                    break;
                    case 6:
                        // Fast Potion //
                            float Flt_PotionRadius2 = 0;
                            Collider[] Col_A1_HitColliders2 = Physics.OverlapSphere(
                                                                                    this.transform.position,
                                                                                    Flt_PotionRadius2
                                                                                  );
                            foreach (Collider Col_Hit in Col_A1_HitColliders2 )
                            {
                                R_Ctm_BP_Target.UsePotion(0f,0f);
                            }
                    break;
                    case 7:
                        // Bomb //
                            float Flt_Force = 0;
                            float Flt_ExplosionRadius3 = 0;
                            float Flt_UpwardsModifier = 0;
                            Collider[] Col_A1_HitColliders3 = Physics.OverlapSphere(
                                                                                    this.transform.position,
                                                                                    Flt_ExplosionRadius3
                                                                                  );
                            foreach (Collider Col_Hit in Col_A1_HitColliders3 )
                            {
                                Col_Hit.GetComponent<BasePlayer>().HijackBomb(this.transform.position, Flt_Force, Flt_ExplosionRadius3, Flt_UpwardsModifier);
                            }
                    break;
                }
        }
    #endregion External Classe Methods
}
