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

            Vector3 R_Vec3_Speed;
        [Header(" Bullet Settings")]
            [SerializeField] float _lifetime;
            float I_Flt_Lifetime;
            [SerializeField] LayerMask _detectionLayers;
            LayerMask I_LyrM_DetectionLayers;
    #endregion Variables
    #region    Methods
        #region    Unity Methods
            public void Awake()
            {
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
                Destroy(this.gameObject, I_Flt_Lifetime);
            }
            public virtual void OnFixedUpdate(float Flt_FixedDT)
            {
                ExecuteMovement(Flt_FixedDT);
            }
        #endregion Override Methods
        #region    Custom Methods
            #region    Set Up
                public void SetData(string Str_Input, int Int_Input, Vector3 Vec3_Input, float Flt_Input1, float Flt_Input2)
                {
                    I_Str_OwnerNickname = Str_Input;
                    I_Int_BP_OwnerID = Int_Input;
                    I_Vec3_BP_Dir = Vec3_Input;
                    I_Flt_Speed = Flt_Input1;
                    I_Flt_Damage = Flt_Input2;
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
                                Ctm_BP_Target1.ExecuteDamage(I_Flt_Damage);
                                Destroy(this.gameObject);
                            }
                        }
                    // Is Own's Child Source? //
                        else
                        if (Col_Hit.gameObject.layer == 8 && Col_Hit.transform.parent.gameObject.TryGetComponent<BasePlayer>(out BasePlayer Ctm_BP_Target2))
                        {
                            if (Ctm_BP_Target2.ID != I_Int_BP_OwnerID)
                            { 
                                Ctm_BP_Target2.ExecuteDamage(I_Flt_Damage);
                                Destroy(this.gameObject);
                            }
                        }
                    // Is Wall or Obstacle? //
                        else 
                        if ( (5 < Col_Hit.gameObject.layer) && (Col_Hit.gameObject.layer < 8) )
                        { 
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
                                Debug.Log(
                                        Col_Hit);
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
}
