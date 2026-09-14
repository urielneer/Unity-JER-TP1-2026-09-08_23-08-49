using UnityEngine;
using System.Collections.Generic;

public class Mouse : MonoBehaviour
{
    #region    Variables
        #region    Asset-To-Asset Data
                [SerializeField] Material[] _materials;
                private Material[] I_Mat_A1_Materials;
                [SerializeField] LayerMask _target;
                private LayerMask I_LM_Target;
                private bool I_Bool_IsToggled = false;
                private bool I_Bool_IsTargetMoving = false;
                private BasePlayer O_BP_Selected;
                public BasePlayer Selected => O_BP_Selected;
        #endregion Asset-To-Asset Data
        #region    Unity Methods
            protected void Awake()
            {
                I_LM_Target = _target;
                I_Mat_A1_Materials = _materials;
                this.gameObject.GetComponent<MeshRenderer>().material = I_Mat_A1_Materials[0];
            }
            private void Update()
            {
                O_BP_Selected = GetClosestPlayer();
                
                if (O_BP_Selected != null)
                {
                    if (O_BP_Selected.IsMoving)
                        this.gameObject.GetComponent<MeshRenderer>().material = I_Mat_A1_Materials[2];
                    else
                        this.gameObject.GetComponent<MeshRenderer>().material = (!I_Bool_IsToggled) ?
                                                                                I_Mat_A1_Materials[0]: 
                                                                                I_Mat_A1_Materials[1];
                }
                else
                    this.gameObject.GetComponent<MeshRenderer>().material = (!I_Bool_IsToggled) ?
                                                                            I_Mat_A1_Materials[0]: 
                                                                            I_Mat_A1_Materials[1];
            }
            private void OnDrawGizmos()
            {
                Transform Tfm_Origin = transform;

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position + new Vector3(0f, 2f, 0f), 1f);

            }
        #endregion Unity Methods
        #region    Custom Methods
            public BasePlayer GetClosestPlayer()
            {
                // Variables //
                    Transform[] Tfm_A1_Result = new Transform[0];
                    Collider[] Col_A1_HitBuffer = new Collider[10];
                    Vector3 Vec3_Origin = transform.position + new Vector3(0f, 2f, 0f);    
                    int Int_Count  = Physics.OverlapSphereNonAlloc(
                                                                    Vec3_Origin,
                                                                    1f,
                                                                    Col_A1_HitBuffer,
                                                                    I_LM_Target
                                                                );
                    if (Int_Count == 0) return null;
                // Collisions //
                    foreach (Collider Col_Hit in Col_A1_HitBuffer)
                    {
                        // Ignore Own Collision //
                            if (Col_Hit == this.gameObject.GetComponent<MeshCollider>()) break;
                        // Ignore null //
                            if (Col_Hit.transform.parent == null) break;
                                I_Bool_IsTargetMoving = Col_Hit.transform.parent.GetComponent<BasePlayer>().IsMoving;
                        // Verify Hit //
                                return Col_Hit.GetComponent<BasePlayer>();
                    }

                return null;
            }
            public void MouseToggle()
            {
                I_Bool_IsToggled = !I_Bool_IsToggled;
                if (!I_Bool_IsTargetMoving)
                    this.gameObject.GetComponent<MeshRenderer>().material = (!I_Bool_IsToggled) ?
                                                                            I_Mat_A1_Materials[0]: 
                                                                            I_Mat_A1_Materials[1];
            }
        #endregion Custom Methods
    #endregion Variables
}
