using UnityEngine;
using Photon.Pun;

public class Pickupable : MonoBehaviour
{
    #region    Variables
        public int Type = 0;
        [SerializeField] private Vector3 _parkPosition = new Vector3(0f, -500f, 0f);
        private MeshRenderer R_MR_Renderer;
        private Collider R_Col_Collider;
        private bool O_Bool_InUse = false;
        public bool InUse => O_Bool_InUse;
    #endregion Variables
    #region    Methods
        #region    Unity Methods
            private void Awake()
            {
                R_MR_Renderer = GetComponent<MeshRenderer>();
                R_Col_Collider = GetComponent<Collider>();
            }
        #endregion Unity Methods
        #region    Pool Methods
            // Llamado por el Master para sacar una pieza del pool //
            public void PoolSpawn(Vector3 Vec3_Position, int Int_Type)
            {
                GetComponent<PhotonView>().RPC(nameof(RPC_PoolSpawn), RpcTarget.All, Vec3_Position, Int_Type);
            }
            // Llamado cuando un jugador lo agarra //
            public void PoolDespawn()
            {
                GetComponent<PhotonView>().RPC(nameof(RPC_PoolDespawn), RpcTarget.All);
            }
        #endregion Pool Methods
        #region    PUN
            #region    RPC
                [PunRPC]
                private void RPC_PoolSpawn(Vector3 Vec3_Position, int Int_Type)
                {
                    Type = Int_Type;
                    transform.position = Vec3_Position;
                    O_Bool_InUse = true;
                    SetPresence(true);
                }
                [PunRPC]
                private void RPC_PoolDespawn()
                {
                    O_Bool_InUse = false;
                    SetPresence(false);
                    transform.position = _parkPosition;
                }
            #endregion RPC
        #endregion PUN
        #region    Custom Methods
            // No se usa SetActive: un GameObject inactivo no recibe RPCs en PUN2 //
            private void SetPresence(bool Bool_Visible)
            {
                if (R_MR_Renderer == null) R_MR_Renderer = GetComponent<MeshRenderer>();
                if (R_Col_Collider == null) R_Col_Collider = GetComponent<Collider>();
                if (R_MR_Renderer != null) R_MR_Renderer.enabled = Bool_Visible;
                if (R_Col_Collider != null) R_Col_Collider.enabled = Bool_Visible;
            }
        #endregion Custom Methods
    #endregion Methods
}
