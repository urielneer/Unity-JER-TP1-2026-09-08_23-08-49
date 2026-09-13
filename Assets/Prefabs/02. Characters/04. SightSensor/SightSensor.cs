using UnityEngine;
using static CustomExtension.ArrayExtensions;

#region    ...
#endregion ...

public class SightSensor : MonoBehaviour
{
    #region    Variables
        #region    Asset-To-Asset Data
            [Header(" Sight Sensor Settings")]
                [SerializeField] float _viewDistance;
                float I_Flt_ViewDistance;
                [SerializeField] float _viewAngle;
                float I_Flt_ViewAngle;
                [SerializeField] LayerMask _targetLayer;
                LayerMask I_LM_TargetLayer;
                [SerializeField] LayerMask _obstacleLayer;
                LayerMask I_LM_ObstacleLayer;
        #endregion Asset-To-Asset Data
    #endregion Variables
    #region    Unity Methods
        protected void Awake()
        {
            I_Flt_ViewDistance = _viewDistance;
            I_Flt_ViewAngle = _viewAngle;
            I_LM_TargetLayer = _targetLayer;
            I_LM_ObstacleLayer = _obstacleLayer;
        }

        private void OnDrawGizmosSelected()
        {
            Transform Tfm_Origin = transform;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(Tfm_Origin.position, I_Flt_ViewDistance);

            Gizmos.color = Color.blue;
            Vector3 Vec3_LeftRayDir  = Quaternion.Euler(0, -I_Flt_ViewAngle/2f, 0) * Tfm_Origin.forward;
            Vector3 Vec3_RightRayDir = Quaternion.Euler(0, +I_Flt_ViewAngle/2f, 0) * Tfm_Origin.forward;
            Gizmos.DrawRay(Tfm_Origin.position, Vec3_LeftRayDir  * I_Flt_ViewDistance);
            Gizmos.DrawRay(Tfm_Origin.position, Vec3_RightRayDir * I_Flt_ViewDistance);
        }
    #endregion Unity Methods
    #region    Custom Methods
        public bool IsEnemyVisible(Transform Tfm_Target)
        {
            // Variables //
                Collider[] Col_A1_HitBuffer = new Collider[10];
                Vector3 Vec3_Origin = transform.position;    
                int Int_Count  = Physics.OverlapSphereNonAlloc(
                                                                Vec3_Origin,
                                                                I_Flt_ViewDistance,
                                                                Col_A1_HitBuffer,
                                                                I_LM_TargetLayer
                                                            );
            // Collisions //
                foreach (Collider Col_Hit in Col_A1_HitBuffer)
                {
                    // Ignore Own Collision //
                        if (Col_Hit == this.gameObject.GetComponent<MeshCollider>()) continue;
                    // Ignore null //
                        if (Col_Hit == null) continue;
                    // Is Target ? //
                        if (Col_Hit.transform == Tfm_Target) continue;
                    // Verify Hit //
                        if ( CanPercieveTarget(Col_Hit.transform) )
                        { 
                            return true;
                        }
                }
            // return //
                return false;
        }
        public Transform[] FindVisibleEnemy()
        {
            // Variables //
                Transform[] Tfm_A1_Result = new Transform[0];
                Collider[] Col_A1_HitBuffer = new Collider[10];
                Vector3 Vec3_Origin = transform.position;    
                int Int_Count  = Physics.OverlapSphereNonAlloc(
                                                                Vec3_Origin,
                                                                I_Flt_ViewDistance,
                                                                Col_A1_HitBuffer,
                                                                I_LM_TargetLayer
                                                            );
            // Collisions //
                foreach (Collider Col_Hit in Col_A1_HitBuffer)
                {
                    // Ignore Own Collision //
                        if (Col_Hit == this.gameObject.GetComponent<MeshCollider>()) continue;
                    // Ignore null //
                        if (Col_Hit == null) continue;
                    // Verify Hit //
                        if ( CanPercieveTarget(Col_Hit.transform) )
                        { 
                            AddNewToArray(ref Tfm_A1_Result, Col_Hit.transform);
                        }
                }
            // return //
                return (Tfm_A1_Result.Length > 0) ?
                           Tfm_A1_Result:
                           null;
        }
        protected bool CanPercieveTarget(Transform Tfm_Target)
        {
            // Fail Case //
                if (Tfm_Target == null) 
                    return false;
            // Variables //
                Vector3 Vec3_Origin = transform.position;
                Vector3 Vec3_Target = Tfm_Target.position;
                Vector3 Vec3_Dir = Vec3_Target - Vec3_Origin;
                float Flt_Dist = Vec3_Dir.magnitude;
                Vector3 Vec3_Forward = transform.forward;
                float Flt_Angle = Vector3.Angle(Vec3_Forward, Vec3_Dir.normalized);
            // Percieve //
                // Is In Perception Range ? //
                    if (Flt_Dist > I_Flt_ViewDistance) 
                        return false;
                // Is In Perception Angle ? //
                    if (Flt_Angle > I_Flt_ViewAngle / 2f) 
                        return false;
                // Is Behind An Obstacle ? //
                    if (Physics.Raycast(Vec3_Origin, Vec3_Dir.normalized, out RaycastHit RHit_Hit, Flt_Dist, I_LM_ObstacleLayer)) 
                        return false;
            // End //
                return true;
        }
    #endregion Custom Methods
}
