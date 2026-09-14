using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UIElements;
using static CustomExtension.ArrayExtensions;

public class BasePlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    #region    Variables
        #region    Player Data
            string IO_Str_OwnerNickname;
            public string OwnerNickname => IO_Str_OwnerNickname;
            int IO_Int_ID;
            public int ID => IO_Int_ID;
            float IO_Flt_Health;
            public float Health => IO_Flt_Health;
            PlayerType R_E_PT_ClientType;
            public PlayerType ClientType => R_E_PT_ClientType;
            public bool IsDead => R_Bool_IsDead;
            [Header(" Vigilante - Alcances")]
                [SerializeField] float _killRange = 12f;
                [SerializeField] float _killAngle = 70f;
                [SerializeField] float _tagRange = 2.5f;
            [Header(" Vigilante - Ciclo de Movimiento")]
                [SerializeField] float _vigilantMoveSeconds = 5f;
                [SerializeField] float _vigilantStillSeconds = 3f;
                private bool R_Bool_MovementLocked = false;

            private bool R_Bool_CanShoot = true;
            private bool R_Bool_IsDead = false;
            private bool R_Bool_IsRotationLocked = false;
            private float R_Flt_SpeedModifier = 1;
            private Coroutine R_Crtn_Active = null;
        #endregion Player Data   
        #region    Character 
            [Header(" Sight Sensor Settings")]
                [SerializeField] SightSensor _prefabSightSensor;
                SightSensor I_Ctm_SS_PrefabSightSensor;
                SightSensor R_Ctm_SS_SightSensor = null;
                [SerializeField] FogWar _prefabFogWar;
                FogWar I_Ctm_FW_PrefabFogWar;
                FogWar R_Ctm_FW_FogWar = null;
            [Space(10)]
            [Header(" Mouse Settings")]
                [SerializeField] Mouse _prefabMouse;
                Mouse I_Ctm_Mse_PrefabMouse;
                Mouse R_Ctm_Mse_Mouse = null;
                [SerializeField] float _mouseSpeed;
                private float I_Flt_MouseSpeed;
                [SerializeField] Vector2 _maxDistances;
                private Vector2 I_Vec2_MaxDistances;
                private float R_Flt_MaxDistance;
                float R_Flt_MouseDir;
            [Space(10)]
            [Header(" Inventory Settings")]
                int Int_ItemIndex = -1;
                [SerializeField] LayerMask _collisionLayers;
                LayerMask I_LyrM_CollisionLayers;
            [Space(10)]
            [Header(" Skill Settings")]
                int Int_SkillIndex = 0;
            [Space(10)]
            [Header(" Camera Settings")]
                [SerializeField] Transform _cameraAnchor;
                private Transform IO_Tfm_CameraAnchor;
                public Transform CameraAnchor => IO_Tfm_CameraAnchor; 
                [SerializeField] Vector3 _cameraPosition;
                private Vector3 I_Vec3_CamPos;
                [SerializeField] Vector3 _cameraRotation;
                private Vector3 I_Vec3_CamRot;
            [Space(10)]
            [Header(" Body Settings")]
                [SerializeField] Transform _canonAnchor;
                private Transform IO_Tfm_CanonAnchor;
                public Transform CanonAnchor => IO_Tfm_CanonAnchor; 
                [SerializeField] Vector2 _canonRotationBounds;
                private Vector2 I_Vec2_CanonRotationBounds;
                [SerializeField] float _canonRotationStrength;
                private float I_Flt_CanonRotationStrength;
                [SerializeField] GameObject[] _bodyParts;
                private GameObject[] I_GObj_A1_BodyParts;
                [SerializeField] Material[] _materials;
                private Material[] I_Mat_A1_Materials;
            [Space(10)]
            [Header(" Hijack Settings")]
                [SerializeField] float _controlFlipSeconds;
                private float I_Flt_ControlFlipSeconds;
                bool O_Bool_IsFlipped;
                public bool IsFlipped => O_Bool_IsFlipped;
                [SerializeField] float _fallStateSeconds;
                private float I_Flt_FallStateSeconds;
                bool O_Bool_IsFallen;
                public bool IsFallen => O_Bool_IsFallen;
            [Space(10)]
        #endregion Character   
        #region    Movement
            [Header(" Movement Speed Settings")]
                // Input //
                    [SerializeField] float _movementSpeed;
                    private float I_Flt_MovementSpeed;
                    [SerializeField] Vector2 _movementSpeedBounds;
                    private Vector2 I_Vec2_MovementSpeedBounds;
                // Runtime //
                    // X //
                        private bool I_Bool_IsMovingX = false;
                        private float I_Flt_CurrentMovementSpeedX;
                    // Y //
                        private bool I_Bool_IsMovingY = false;
                        private float I_Flt_CurrentMovementSpeedY;
            [Header(" Movement Acceleration Settings")]
                // Input //
                    [SerializeField] float _movementAccelRate;
                    private float I_Flt_MovementAccelRate;
                    [SerializeField] float _movementDecelRate;
                    private float I_Flt_MovementDecelRate;
                    [SerializeField] Vector2 _movementAccelBounds;
                    private Vector2 I_Vec2_MovementAccelBounds;
                // Runtime //
                    // X //
                        private float R_Flt_CurrentMovementAccelX = 0;
                    // Y //
                        private float R_Flt_CurrentMovementAccelY = 0;
            [Header(" Movement Direction Flip Settings")]
                // Input //
                    [SerializeField] float _movementFlippedTime;
                    private float I_Flt_MovementFlippedTime;
                    [SerializeField, Range(0f,1f)] float _movementFlippedSpeedLoss;
                    private float I_Flt_MovementFlippedSpeedLoss;
                    [SerializeField, Range(0f,1f)] float _movementFlippedAccelLoss;
                    private float I_Flt_MovementFlippedAccelLoss;
                // Runtime //
                    // X //
                        private int R_Int_CurrentInputMovementX = 0;
                        private int R_Int_LastInputMovementX = 0;
                        private float R_Flt_MovementFlippedCounterX = 0;
                    // Y //
                        private int R_Int_CurrentInputMovementY = 0;
                        private int R_Int_LastInputMovementY = 0;
                        private float R_Flt_MovementFlippedCounterY = 0;
            [Space(10)]
        #endregion Movement
        #region    Rotation
            [Header(" Rotation Speed Settings")]
                // Input //
                    [SerializeField] float _rotationSpeed;
                    private float I_Flt_RotationSpeed;
                    [SerializeField] Vector2 _rotationSpeedBounds;
                    private Vector2 I_Vec2_RotationSpeedBounds;
                // Runtime //
                    private bool R_Bool_IsRotating = false;
                    private float R_Flt_CurrentRotationSpeed;
                    private float R_Flt_CurrentCanonRotation;
            [Header(" Rotation Acceleration Settings")]
                // Input //
                    [SerializeField] float _rotationAccelRate;
                    private float I_Flt_RotationAccelRate;
                    [SerializeField] float _rotationDecelRate;
                    private float I_Flt_RotationDecelRate;
                    [SerializeField] Vector2 _rotationAccelBounds;
                    private Vector2 I_Vec2_RotationAccelBounds;
                // Runtime //
                    private float I_Flt_CurrentRotationAccel = 0;
            [Header(" Rotation Direction Flip Settings")]
                // Input //
                    [SerializeField] float _rotationFlippedTime;
                    private float I_Flt_RotationFlippedTime;
                    [SerializeField, Range(0f,1f)] float _rotationFlippedSpeedLoss;
                    private float I_Flt_RotationFlippedSpeedLoss;
                    [SerializeField, Range(0f,1f)] float _rotationFlippedAccelLoss;
                    private float I_Flt_RotationFlippedAccelLoss;
                // Runtime //
                    private int R_Int_CurrentInputRotationX = 0;
                    private int R_Int_CurrentInputRotationY = 0;
                    private int R_Int_LastInputRotation = 0;
                    private float R_Flt_RotateFlippedCounter = 0;
            [Space(10)]
        #endregion Rotation
        #region    Jumping
            [Header(" Jumping Settings")]
                // Input //
                    [SerializeField] float _jumpForce;
                    private float I_Flt_JumpForce;
                    [SerializeField] float _fallMultiplier;
                    private float I_Flt_FallMultiplier;
                    [SerializeField] float _lowJumpMultiplier;
                    private float I_Flt_LowJumpMultiplier;
                // Runtime //
                    private bool R_Bool_IsJumping = false;
            [Header(" Ground Check Settings")]
                // Input //
                    [SerializeField] Transform _groundCheck;
                    private Transform I_Tfm_GroundCheck;
                    [SerializeField] float _groundCheckRadius;
                    private float I_Flt_GroundCheckRadius;
                    [SerializeField] LayerMask _groundLayer;
                    private LayerMask I_LM_GroundLayer;
                // Runtime //
                    private bool R_Bool_IsGrounded = false;
        #endregion Jumping
        #region    External Classes
            bool O_Bool_IsMoving = false;
            public bool IsMoving => O_Bool_IsMoving;
        #endregion External Classes
    #endregion Variables
    #region    Methods
        #region    Unity Methods
            void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
            {
                if (stream.IsWriting) 
                {
                        stream.SendNext(IO_Str_OwnerNickname);
                        stream.SendNext(IO_Int_ID);
                        stream.SendNext(IO_Flt_Health);
                        stream.SendNext(R_E_PT_ClientType);
                        stream.SendNext(O_Bool_IsMoving);
                }
                else
                {
                    IO_Str_OwnerNickname = (string) stream.ReceiveNext();
                    IO_Int_ID = (int) stream.ReceiveNext();
                    IO_Flt_Health = (float) stream.ReceiveNext();
                    R_E_PT_ClientType = (PlayerType) stream.ReceiveNext();
                    O_Bool_IsMoving = (bool) stream.ReceiveNext();
                }
            }
            public void Awake()
            {
                // Inventory //
                    I_LyrM_CollisionLayers = _collisionLayers;
                // Sight Sensor //
                    I_Ctm_SS_PrefabSightSensor = _prefabSightSensor;
                    I_Ctm_FW_PrefabFogWar = _prefabFogWar;
                // Mouse //
                    I_Ctm_Mse_PrefabMouse = _prefabMouse;
                    I_Flt_MouseSpeed = _mouseSpeed;
                    I_Vec2_MaxDistances = _maxDistances;
                // Body //
                    IO_Tfm_CanonAnchor = _canonAnchor;
                    I_Vec2_CanonRotationBounds = _canonRotationBounds;
                    I_Flt_CanonRotationStrength = _canonRotationStrength;
                    I_GObj_A1_BodyParts =_bodyParts;
                    I_Mat_A1_Materials = _materials;
                // Camera //
                    IO_Tfm_CameraAnchor = _cameraAnchor;
                    I_Vec3_CamPos = _cameraPosition;
                    I_Vec3_CamRot = _cameraRotation;
                    I_Vec2_CanonRotationBounds = _canonRotationBounds;
                    I_Flt_CanonRotationStrength = _canonRotationStrength;
                // Movement //
                    I_Flt_MovementSpeed = _movementSpeed;
                    I_Vec2_MovementSpeedBounds = _movementSpeedBounds;
                    I_Flt_MovementAccelRate = _movementAccelRate;
                    I_Flt_MovementDecelRate = _movementDecelRate;
                    I_Vec2_MovementAccelBounds = _movementAccelBounds;
                    I_Flt_MovementFlippedTime = _movementFlippedTime;
                    I_Flt_MovementFlippedSpeedLoss = _movementFlippedSpeedLoss;
                    I_Flt_MovementFlippedAccelLoss = _movementFlippedAccelLoss;
        
                    I_Flt_CurrentMovementSpeedX = 0;
                    R_Flt_CurrentMovementAccelX = 1;

                    I_Flt_CurrentMovementSpeedY = 0;
                    R_Flt_CurrentMovementAccelY = 1;
                // Rotation //
                    I_Flt_RotationSpeed = _rotationSpeed;
                    I_Vec2_RotationSpeedBounds = _rotationSpeedBounds;
                    I_Flt_RotationAccelRate = _rotationAccelRate;
                    I_Flt_RotationDecelRate = _rotationDecelRate;
                    I_Vec2_RotationAccelBounds = _rotationAccelBounds;
                    I_Flt_RotationFlippedTime = _rotationFlippedTime;
                    I_Flt_RotationFlippedSpeedLoss = _rotationFlippedSpeedLoss;
                    I_Flt_RotationFlippedAccelLoss = _rotationFlippedAccelLoss;
                
                    R_Flt_CurrentRotationSpeed = 0;
                    I_Flt_CurrentRotationAccel = 1;
                // Jump //
                    I_Flt_JumpForce = _jumpForce;
                    I_Flt_FallMultiplier = _fallMultiplier;
                    I_Flt_LowJumpMultiplier = _lowJumpMultiplier;

                    I_Tfm_GroundCheck  = _groundCheck;
                    I_Flt_GroundCheckRadius = _groundCheckRadius;
                    I_LM_GroundLayer = _groundLayer;

                    R_Bool_IsJumping = false;
                    R_Bool_IsGrounded = true;

                    R_Ctm_SS_SightSensor = null;
                    R_Ctm_Mse_Mouse = null;


                    SetVisibility(true);
                // Fix the funny Kinematic Telepoprt Glitch //
                    if (!photonView.IsMine)
                        this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }
        #endregion Unity Methods
        #region    Override Methods
            public virtual void OnStartUp()
            {
                // Cooldown Flags //
                    R_Bool_CanShoot = true;
                    R_Bool_IsDead = false;
                // Attach camera //
                    Camera Cam_Main = Camera.main;
                    if (Cam_Main == null) Cam_Main = UnityEngine.Object.FindFirstObjectByType<Camera>();
                    if (Cam_Main == null)
                    {
                        Debug.LogError("[Base Player] No se encontro ninguna camara para enganchar al jugador");
                        return;
                    }
                    Cam_Main.transform.SetParent(this.CameraAnchor.gameObject.transform, false);
                // Reposition transform //
                    Cam_Main.transform.localPosition = I_Vec3_CamPos;
                    Cam_Main.transform.localEulerAngles = I_Vec3_CamRot;
            }
            public virtual void OnUpdate(float Flt_FixedDT)
            {
                // Only execute Own //
                    if (!GetComponent<PhotonView>().IsMine) return;
                // Character Logic //
                    ExecuteJumpReset(Flt_FixedDT);
                    UpdateIsMoving();
                    CheckTagged();
            }
            public virtual void OnFixedUpdate(float Flt_FixedDT)
            {
                // Only execute Own //
                    if (!GetComponent<PhotonView>().IsMine) return;
                // Character Logic //
                    ExecuteMovement(Flt_FixedDT);
                    ExecuteMouseMovement(Flt_FixedDT);
                    ExecuteBodyRotation(Flt_FixedDT);
                    ExecuteCanonRotation(Flt_FixedDT);
                    ExecuteJumpVariableGravity(Flt_FixedDT);
                    PercieveContact(Flt_FixedDT);
            }
        #endregion Override Methods
        #region    Custom Methods
            #region    Player Data
                public void SetMaterial(int Int_Input = -1)
                {
                    // Error //
                        if (Int_Input == -1)
                            Int_Input = (R_E_PT_ClientType == PlayerType.Chaser) ? 1 : 0;
                    // Material //
                        I_GObj_A1_BodyParts[0].GetComponent<MeshRenderer>().material = I_Mat_A1_Materials[Int_Input];
                }
                bool Bool_FirstSetData = false;
                public void SetData(string Str_Input, int Int_Input, float Flt_Input, PlayerType E_PT_ClientType)
                {
                    // Variables //
                        IO_Str_OwnerNickname = Str_Input;
                        IO_Int_ID = Int_Input;
                        IO_Flt_Health = Flt_Input;
                        R_E_PT_ClientType = E_PT_ClientType;
                    // Type Differences //
                        if (E_PT_ClientType == PlayerType.Vigilant)
                        {
                                // La camara siempre queda libre //
                                R_Bool_IsRotationLocked = false;
                                R_Flt_MaxDistance = I_Vec2_MaxDistances.x;
                        }
                        else
                        {
                                R_Bool_IsRotationLocked = false;
                                R_Flt_MaxDistance = I_Vec2_MaxDistances.y;
                        }
                    // Rotation lock - Un solo ciclo por jugador //
                        RestartRotationCycle();
                        Bool_FirstSetData = true;
                    if (R_E_PT_ClientType == PlayerType.Vigilant)
                        StartCoroutine(GameOver());
                }
                // Evita que se acumulen ciclos en paralelo peleando por el mismo flag //
                private Coroutine R_Crtn_RotationCycle = null;
                public void RestartRotationCycle()
                {
                    // Frenar el ciclo anterior //
                        if (R_Crtn_RotationCycle != null)
                        {
                            StopCoroutine(R_Crtn_RotationCycle);
                            R_Crtn_RotationCycle = null;
                        }
                    // El Corredor nunca queda bloqueado //
                        if (R_E_PT_ClientType != PlayerType.Vigilant)
                        {
                            R_Bool_IsRotationLocked = false;
                            return;
                        }
                    // Solo el Vigilante cicla //
                        R_Crtn_RotationCycle = StartCoroutine(ReenableRotation());
                }
                public void UpdateIsMoving()
                {
                    O_Bool_IsMoving = (
                                          (I_Flt_CurrentMovementSpeedX > 0) && (I_Flt_CurrentMovementSpeedY > 0) || // Is Moving ? //
                                          (R_Flt_CurrentRotationSpeed > 0)                                       || // Is Rotating ? //
                                          O_Bool_IsFallen                                                           // Has Fallen ? //
                                      ) ?
                                        true :
                                        false;
                }
                public void SetVisibility(bool Bool_IsVisible)
                {
                    // Toggle Visuals & Collisions //
                        foreach (GameObject GObj_Element in I_GObj_A1_BodyParts)
                        {
                            // Toggle All Renderers //
                                // Disable //
                                    GObj_Element.GetComponent<Renderer>().enabled = Bool_IsVisible;
                            // Toggle Colliders //
                                // Disable //
                                    if (GObj_Element.TryGetComponent<Collider>(out Collider Coll_Element))
                                        Coll_Element.enabled = Bool_IsVisible;
                        }
                    // Toggle Gravity //
                        this.GetComponent<Rigidbody>().useGravity = Bool_IsVisible;
                }
            #endregion Player Data
            #region    Rotation
                private void ExecuteBodyRotation(float Flt_FixedDT)
                {
                    // Variables //
                        Quaternion Qtn_DeltaRotation = Quaternion.identity;
                        bool Bool_IsFlippingDirection = // Detect ONE-TIME Direction Flip Event //
                            R_Bool_IsRotating                                      && // Is Rotating? //
                            R_Int_CurrentInputRotationX != 0                        && // Is Rotating towards a direction? //
                            R_Int_LastInputRotation == -R_Int_CurrentInputRotationX && // Is Rotating opposite to last stored direction //
                            (R_Flt_RotateFlippedCounter > 0);                         // Has switched recently? (AKA: Not suddendly stop for a long period and then turned back) //
                        float Flt_TargetBaseSpeed = 0;
                    // Calculate "Flip Acceleration" //
                        if (Bool_IsFlippingDirection)
                        {
                            #region    Direction Flip [Flip]
                                // Apply Momentum & Acceleration Loss ONCE On The Turn Frame //
                                    R_Flt_CurrentRotationSpeed *= (1f - I_Flt_RotationFlippedSpeedLoss);
                                    I_Flt_CurrentRotationAccel *= (1f - I_Flt_RotationFlippedAccelLoss);
                                // Bounds - Min Check (Force speed to minimum) //
                                    I_Flt_CurrentRotationAccel = Mathf.Max(I_Flt_CurrentRotationAccel, I_Vec2_RotationAccelBounds.x);
                            #endregion Direction Flip [Flip]
                        }
                    // Timer Hard Reset //
                        if (R_Int_CurrentInputRotationX == 0 && R_Flt_RotateFlippedCounter == 0)
                        {
                            R_Flt_RotateFlippedCounter = I_Flt_RotationFlippedTime;
                        }
                    // Calculate "Current Speed", "Acceleration" & "Deceleration" //
                        if (R_Bool_IsRotating)
                        {
                            #region    Direction Flip [Reset Timer]
                                R_Flt_RotateFlippedCounter = I_Flt_RotationFlippedTime;
                                R_Int_LastInputRotation = R_Int_CurrentInputRotationX;
                            #endregion Direction Flip [Reset Timer]
                            #region    Accelerate & Move
                                // Regular //
                                    // Acceleration //
                                        // Ramp up Acceleration Rate //
                                            I_Flt_CurrentRotationAccel += I_Flt_RotationAccelRate * Flt_FixedDT;
                                        // Bounds - Max Check (Can't go above this point) //
                                            I_Flt_CurrentRotationAccel = Mathf.Clamp(I_Flt_CurrentRotationAccel, 0, I_Vec2_RotationAccelBounds.y);
                                    // Speed //
                                        // Ramp up Speed using Acceleration //
                                            Flt_TargetBaseSpeed = I_Flt_RotationSpeed * I_Flt_CurrentRotationAccel;
                                            R_Flt_CurrentRotationSpeed += Flt_TargetBaseSpeed * Flt_FixedDT;
                                        // Bounds - Max Check (Can't go beyond this point) //
                                            R_Flt_CurrentRotationSpeed = Mathf.Clamp(R_Flt_CurrentRotationSpeed, 0, I_Vec2_RotationSpeedBounds.y);
                            #endregion Accelerate & Move
                        }
                        else
                        {
                            #region    Direction Flip [Countdown]
                                R_Flt_RotateFlippedCounter--;
                            #endregion Direction Flip [Countdown]
                            #region    Reset Acceleration
                                // Is Regular Acceleration Active? //
                                    I_Flt_CurrentRotationAccel = (I_Flt_CurrentRotationAccel > I_Vec2_RotationAccelBounds.x) ?
                                                                // Reduce Acceleration //
                                                                    I_Flt_CurrentRotationAccel -= I_Flt_RotationAccelRate * Flt_FixedDT :
                                                                // Bounds - Min Check (Reset Acceleration to 1.00 [AKA: 100%]) //
                                                                    I_Flt_CurrentRotationAccel = 1;
                            #endregion Reset Acceleration
                            #region    Decelerate & Stop 
                                // Regular - Deceleration //
                                    // Ramp down Speed through Deceleration //
                                        R_Flt_CurrentRotationSpeed -= I_Flt_RotationDecelRate * Flt_FixedDT;
                                    // Bounds - Min Check (Force speed to 0 if under minimum) //
                                        R_Flt_CurrentRotationSpeed = Mathf.Max(R_Flt_CurrentRotationSpeed, 0f);
                            #endregion Decelerate & Stop 
                        }

                    // Calculate "Rotation Delta" //
                        float Flt_YDegrees = R_Flt_SpeedModifier * R_Int_LastInputRotation * R_Flt_CurrentRotationSpeed * Flt_FixedDT;
                        Qtn_DeltaRotation = Quaternion.Euler(0f, Flt_YDegrees, 0f);

                    // Apply Rotation //
                        if (Mathf.Abs(Flt_YDegrees) > 0f)
                        {
                            Rigidbody Rb_Player = GetComponent<Rigidbody>();
                            Rb_Player.MoveRotation(Rb_Player.rotation * Qtn_DeltaRotation);
                        }
                }
                private void ExecuteCanonRotation(float Flt_FixedDT)
                {
                    // Update rotation //
                }
            #endregion Rotation
            #region    Movement
                private void ExecuteMouseMovement(float Flt_FixedDT)
                {
                    // Solo el Vigilante tiene mira //
                        if (R_Ctm_Mse_Mouse == null) return;
                    // Variables //
                        Vector3 Vec3_Displacement = Vector3.zero;
                    // Set Displacement //
                        Vec3_Displacement += XAxisMovement(Flt_FixedDT);
                        Vec3_Displacement += YAxisMovement(Flt_FixedDT);
                    // Apply Movement //
                        R_Ctm_Mse_Mouse.transform.localPosition += (R_Flt_MouseDir * I_Flt_MouseSpeed * new Vector3(0f,0f,1f) * Flt_FixedDT);
                        R_Ctm_Mse_Mouse.transform.localPosition = new Vector3(0f, 0f, Mathf.Clamp(R_Ctm_Mse_Mouse.transform.localPosition.z, 2.2f, R_Flt_MaxDistance));
                }
                private void ExecuteMovement(float Flt_FixedDT)
                {
                    // Variables //
                    Vector3 Vec3_Displacement = Vector3.zero;
                    // Set Displacement //
                    Vec3_Displacement += XAxisMovement(Flt_FixedDT);
                    Vec3_Displacement += YAxisMovement(Flt_FixedDT);

                    // Chequeo de Fricci�n //
                    Collider selfCol = GetComponent<Collider>();
                    bool isSliding = selfCol != null && selfCol.material != null && selfCol.material.dynamicFriction < 0.1f;

                    if (isSliding)
                    {
                        // En hielo usa fuerzas: el PhysicMaterial se encarga de hacerlo patinar
                        GetComponent<Rigidbody>().AddForce(Vec3_Displacement / Flt_FixedDT, ForceMode.Acceleration);
                    }
                    // Apply Movement Regular //
                    else if (Vec3_Displacement.sqrMagnitude > 0f)
                    {
                        GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + Vec3_Displacement);
                    }
                }
    private Vector3 XAxisMovement(float Flt_FixedDT)
                {
                    // Variables //
                        Vector3 Vec3_Displacement = Vector3.zero;
                        bool Bool_IsFlippingDirection = // Detect ONE-TIME Direction Flip Event //
                            I_Bool_IsMovingX                                         && // Is Moving? //
                            R_Int_CurrentInputMovementX != 0                         && // Is Moving towards a direction? //
                            R_Int_LastInputMovementX == -R_Int_CurrentInputMovementX && // Is Moving opposite to last stored direction //
                            (R_Flt_MovementFlippedCounterX > 0);                        // Has switched recently? (AKA: Not suddendly stop for a long period and then turned back) //
                        float Flt_TargetBaseSpeed = 0;
                    // Calculate "Flip Acceleration" //
                        if (Bool_IsFlippingDirection)
                        {
                            #region    Direction Flip [Flip]
                                // Apply Momentum & Acceleration Loss ONCE On The Turn Frame //
                                    I_Flt_CurrentMovementSpeedX *= (1f - I_Flt_MovementFlippedSpeedLoss);
                                    R_Flt_CurrentMovementAccelX *= (1f - I_Flt_MovementFlippedAccelLoss);
                                // Bounds - Min Check (Force speed to minimum) //
                                    R_Flt_CurrentMovementAccelX = Mathf.Max(R_Flt_CurrentMovementAccelX, I_Vec2_MovementAccelBounds.x);
                            #endregion Direction Flip [Flip]
                        }
                    // Timer Hard Reset //
                        if (R_Int_CurrentInputMovementX == 0 && R_Flt_MovementFlippedCounterX == 0)
                        {
                            R_Flt_MovementFlippedCounterX = I_Flt_MovementFlippedTime;
                        }
                    // Calculate "Current Speed", "Acceleration" & "Deceleration" //
                        if (I_Bool_IsMovingX)
                        {
                            #region    Direction Flip [Reset Timer]
                                R_Flt_MovementFlippedCounterX = I_Flt_MovementFlippedTime;
                                R_Int_LastInputMovementX = R_Int_CurrentInputMovementX;
                            #endregion Direction Flip [Reset Timer]
                            #region    Accelerate & Move
                                // Regular //
                                    // Acceleration //
                                        // Ramp up Acceleration Rate //
                                            R_Flt_CurrentMovementAccelX += I_Flt_MovementAccelRate * Flt_FixedDT;
                                        // Bounds - Max Check (Can't go above this point) //
                                            R_Flt_CurrentMovementAccelX = Mathf.Clamp(R_Flt_CurrentMovementAccelX, 0, I_Vec2_MovementAccelBounds.y);
                                    // Speed //
                                        // Ramp up Speed using Acceleration //
                                            Flt_TargetBaseSpeed = I_Flt_MovementSpeed * R_Flt_CurrentMovementAccelX;
                                            I_Flt_CurrentMovementSpeedX += Flt_TargetBaseSpeed * Flt_FixedDT;
                                        // Bounds - Max Check (Can't go beyond this point) //
                                            I_Flt_CurrentMovementSpeedX = Mathf.Clamp(I_Flt_CurrentMovementSpeedX, 0, I_Vec2_MovementSpeedBounds.y);
                            #endregion Accelerate & Move
                        }
                        else
                        {
                            #region    Direction Flip [Countdown]
                                R_Flt_MovementFlippedCounterX--;
                            #endregion Direction Flip [Countdown]
                            #region    Reset Acceleration
                                // Is Regular Acceleration Active? //
                                    R_Flt_CurrentMovementAccelX = (R_Flt_CurrentMovementAccelX > I_Vec2_MovementAccelBounds.x) ?
                                                                // Reduce Acceleration //
                                                                    R_Flt_CurrentMovementAccelX -= I_Flt_MovementAccelRate * Flt_FixedDT :
                                                                // Bounds - Min Check (Reset Acceleration to 1.00 [AKA: 100%]) //
                                                                    R_Flt_CurrentMovementAccelX = 1;
                            #endregion Reset Acceleration
                            #region    Decelerate & Stop 
                                // Regular - Decceleration //
                                    // Ramp down Speed through Deceleration //
                                        I_Flt_CurrentMovementSpeedX -= I_Flt_MovementDecelRate * Flt_FixedDT;
                                    // Bounds - Min Check (Force speed to 1 if under minimum) //
                                        I_Flt_CurrentMovementSpeedX = Mathf.Max(I_Flt_CurrentMovementSpeedX, 0f);
                            #endregion Decelerate & Stop 
                        }
                    // Calculate "Displacement Vector" //
                        return R_Flt_SpeedModifier * transform.right * R_Int_LastInputMovementX * I_Flt_CurrentMovementSpeedX * Flt_FixedDT;
                }
                private Vector3 YAxisMovement(float Flt_FixedDT)
                {
                    // Variables //
                        bool Bool_IsFlippingDirection = // Detect ONE-TIME Direction Flip Event //
                            I_Bool_IsMovingY                                         && // Is Moving? //
                            R_Int_CurrentInputMovementY != 0                         && // Is Moving towards a direction? //
                            R_Int_LastInputMovementY == -R_Int_CurrentInputMovementY && // Is Moving opposite to last stored direction //
                            (R_Flt_MovementFlippedCounterY > 0);                        // Has switched recently? (AKA: Not suddendly stop for a long period and then turned back) //
                        float Flt_TargetBaseSpeed = 0;
                    // Calculate "Flip Acceleration" //
                        if (Bool_IsFlippingDirection)
                        {
                            #region    Direction Flip [Flip]
                                // Apply Momentum & Acceleration Loss ONCE On The Turn Frame //
                                    I_Flt_CurrentMovementSpeedY *= (1f - I_Flt_MovementFlippedSpeedLoss);
                                    R_Flt_CurrentMovementAccelY *= (1f - I_Flt_MovementFlippedAccelLoss);
                                // Bounds - Min Check (Force speed to minimum) //
                                    R_Flt_CurrentMovementAccelY = Mathf.Max(R_Flt_CurrentMovementAccelY, I_Vec2_MovementAccelBounds.x);
                            #endregion Direction Flip [Flip]
                        }
                    // Timer Hard Reset //
                        if (R_Int_CurrentInputMovementY == 0 && R_Flt_MovementFlippedCounterY == 0)
                        {
                            R_Flt_MovementFlippedCounterY = I_Flt_MovementFlippedTime;
                        }
                    // Calculate "Current Speed", "Acceleration" & "Deceleration" //
                        if (I_Bool_IsMovingY)
                        {
                            #region    Direction Flip [Reset Timer]
                                R_Flt_MovementFlippedCounterY = I_Flt_MovementFlippedTime;
                                R_Int_LastInputMovementY = R_Int_CurrentInputMovementY;
                            #endregion Direction Flip [Reset Timer]
                            #region    Accelerate & Move
                                // Regular //
                                    // Acceleration //
                                        // Ramp up Acceleration Rate //
                                            R_Flt_CurrentMovementAccelY += I_Flt_MovementAccelRate * Flt_FixedDT;
                                        // Bounds - Max Check (Can't go above this point) //
                                            R_Flt_CurrentMovementAccelY = Mathf.Clamp(R_Flt_CurrentMovementAccelY, 0, I_Vec2_MovementAccelBounds.y);
                                    // Speed //
                                        // Ramp up Speed using Acceleration //
                                            Flt_TargetBaseSpeed = I_Flt_MovementSpeed * R_Flt_CurrentMovementAccelY;
                                            I_Flt_CurrentMovementSpeedY += Flt_TargetBaseSpeed * Flt_FixedDT;
                                        // Bounds - Max Check (Can't go beyond this point) //
                                            I_Flt_CurrentMovementSpeedY = Mathf.Clamp(I_Flt_CurrentMovementSpeedY, 0, I_Vec2_MovementSpeedBounds.y);
                            #endregion Accelerate & Move
                        }
                        else
                        {
                            #region    Direction Flip [Countdown]
                                R_Flt_MovementFlippedCounterY--;
                            #endregion Direction Flip [Countdown]
                            #region    Reset Acceleration
                                // Is Regular Acceleration Active? //
                                    R_Flt_CurrentMovementAccelY = (R_Flt_CurrentMovementAccelY > I_Vec2_MovementAccelBounds.x) ?
                                                                // Reduce Acceleration //
                                                                    R_Flt_CurrentMovementAccelY -= I_Flt_MovementAccelRate * Flt_FixedDT :
                                                                // Bounds - Min Check (Reset Acceleration to 1.00 [AKA: 100%]) //
                                                                    R_Flt_CurrentMovementAccelY = 1;
                            #endregion Reset Acceleration
                            #region    Decelerate & Stop 
                                // Regular - Decceleration //
                                    // Ramp down Speed through Deceleration //
                                        I_Flt_CurrentMovementSpeedY -= I_Flt_MovementDecelRate * Flt_FixedDT;
                                    // Bounds - Min Check (Force speed to 1 if under minimum) //
                                        I_Flt_CurrentMovementSpeedY = Mathf.Max(I_Flt_CurrentMovementSpeedY, 0f);
                            #endregion Decelerate & Stop 
                        }
                    // Calculate "Displacement Vector" //
                        return R_Flt_SpeedModifier * transform.forward * R_Int_LastInputMovementY * I_Flt_CurrentMovementSpeedY * Flt_FixedDT;
                }
            #endregion Movement
            #region    Jump
                private void ExecuteJump()
                { 
                    // Jump //
                        Rigidbody RB_Self = GetComponent<Rigidbody>();
                        RB_Self.linearVelocity = new Vector3(RB_Self.linearVelocity.x, I_Flt_JumpForce, RB_Self.linearVelocity.z);
                }
                private void ExecuteJumpReset(float Flt_FixedDT)
                { 
                    // Ground Check //
                        R_Bool_IsGrounded = Physics.CheckSphere(I_Tfm_GroundCheck.position, I_Flt_GroundCheckRadius, I_LM_GroundLayer);
                    // Reset "Can Jump" //
                        if (R_Bool_IsGrounded && (GetComponent<Rigidbody>().linearVelocity.y <= 0.1f)) 
                            R_Bool_IsJumping = false;
                }
                private void ExecuteJumpVariableGravity(float Flt_FixedDT)
                { 
                    // Stop If Dead //
                        if (R_Bool_IsDead) return;
                    // Gravity Control //
                        if ((GetComponent<Rigidbody>().linearVelocity.y <= 0.1f)) 
                            // Fall Multiplier //
                                GetComponent<Rigidbody>().linearVelocity += Vector3.up * Physics.gravity.y * (I_Flt_FallMultiplier - 1f) * Flt_FixedDT;
                        else if ((GetComponent<Rigidbody>().linearVelocity.y >= 0.1f) && !R_Bool_IsJumping)  
                            // Short Jump //
                                GetComponent<Rigidbody>().linearVelocity += Vector3.up * Physics.gravity.y * (I_Flt_LowJumpMultiplier - 1f) * Flt_FixedDT;
                    
                    /* "(Multiplier - 1) is used because the RigidBody's gravity is counted already and has a value of "1" */
                }
    #endregion Jump
            #region    Combat
                #region    Shoot
                    private void ExecuteStraightShot(int Int_Index) // Ahora es la tecnica default // // Ahora es el item o habilidad //
                    {
                  
                        R_Bool_CanShoot = false;
                        MasterManager.Instance.BulletManager.SynchronizeBullet(this, this.transform.forward, Int_Index);
                        float Flt_Cooldown = 0.5f;
                        if (MasterManager.Instance != null && MasterManager.Instance.CharacterManager != null)
                        {
                            Flt_Cooldown = Mathf.Max(0.2f, MasterManager.Instance.CharacterManager.ShootCooldown);
                        }
                        // Reenable Cooldown //
                        StartCoroutine(ReenableShootingDelay(Flt_Cooldown));
                    }
                    private void ExecuteCurvedShot()
                    {
                        R_Bool_CanShoot = false;
                        MasterManager.Instance.BulletManager.SynchronizeBullet(this, this.transform.forward);
                        float Flt_Cooldown = 0.5f;
                        if (MasterManager.Instance != null && MasterManager.Instance.CharacterManager != null)
                        {
                            Flt_Cooldown = Mathf.Max(0.2f, MasterManager.Instance.CharacterManager.ShootCooldown);
                        }
                        StartCoroutine(ReenableShootingDelay(Flt_Cooldown));
                    }
                #endregion Shoot
                #region    Spawn
                    public void ExecuteSpawn(bool Bool_ResetHealth = true)
                    {
                        // Variables //
                            R_Bool_CanShoot = true;
                            R_Bool_IsDead = false;
                            O_Bool_IsFlipped = false;
                            O_Bool_IsFallen = false;
                        // La vida solo se restaura en un respawn real, no al reposicionar por impacto //
                            if (Bool_ResetHealth)
                                IO_Flt_Health = MasterManager.Instance.CharacterManager.MaxHealth;
                        // Spawn Other Prefabs //
                            if (photonView.IsMine)
                            {
                                // Sight //
                                    if (R_Ctm_SS_SightSensor == null)
                                    {
                                        if (R_E_PT_ClientType == PlayerType.Vigilant)
                                        {
                                            R_Ctm_SS_SightSensor = Instantiate(I_Ctm_SS_PrefabSightSensor, Vector3.zero, Quaternion.identity);
                                            R_Ctm_SS_SightSensor.transform.SetParent(this.CameraAnchor.gameObject.transform, false);
                                            R_Ctm_SS_SightSensor.transform.localPosition = new Vector3(0f, -2f, 0f);
                                            R_Ctm_SS_SightSensor.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                                        }
                                    }
                                // Mouse - Solo el Vigilante tiene mira //
                                    if (R_Ctm_Mse_Mouse == null)
                                    {
                                        if (R_E_PT_ClientType == PlayerType.Vigilant)
                                        {
                                            R_Ctm_Mse_Mouse = Instantiate(I_Ctm_Mse_PrefabMouse, Vector3.zero, Quaternion.identity);
                                            R_Ctm_Mse_Mouse.transform.localPosition = new Vector3(0f, 0f, 5f);
                                            R_Ctm_Mse_Mouse.transform.SetParent(this.transform);
                                        }
                                    }
                                // Fog //
                                    if (R_Ctm_FW_FogWar == null)
                                    {
                                        if (R_E_PT_ClientType == PlayerType.Vigilant)
                                        {
                                            R_Ctm_FW_FogWar = Instantiate(I_Ctm_FW_PrefabFogWar, Vector3.zero, Quaternion.identity);
                                            R_Ctm_FW_FogWar.transform.localPosition = new Vector3(0f, 0f, 0f);
                                            R_Ctm_FW_FogWar.transform.SetParent(this.transform);
                                        }
                                    }
                            }
                        // Position - Si el mapa todavia no genero los spawnpoints, se reintenta //
                            this.transform.position = MasterManager.Instance.MapManager.SpawnPoints[PhotonNetwork.LocalPlayer.ActorNumber].transform.position;
                            this.transform.rotation = MasterManager.Instance.MapManager.SpawnPoints[PhotonNetwork.LocalPlayer.ActorNumber].transform.rotation;
                        // Visibility //
                            SetVisibility(true);
                            PhotonNetwork.SendAllOutgoingCommands();
                            MasterManager.Instance.CharacterManager.ChangePlayerMaterial(null, (R_E_PT_ClientType == PlayerType.Chaser) ? 1 : 0);
                        // Rotation lock - Reinicia el ciclo, no acumula otro //
                            RestartRotationCycle();
                    }
                    private void ExecuteReSpawnDelay()
                    {
                        StartCoroutine(RespawnDelay(MasterManager.Instance.GameManager.RespawnCooldown));
                    }
                    private Coroutine R_Crtn_SpawnRetry;
                    // Devuelve FALSE si todavia no hay ningun spawnpoint vivo //
                    private bool TryPositionAtSpawnPoint()
                    {
                        // Variables //
                            if (MasterManager.Instance == null) return false;
                            if (MasterManager.Instance.MapManager == null) return false;
                            GameObject[] GObj_A1_Spawns = MasterManager.Instance.MapManager.SpawnPoints;
                            if (GObj_A1_Spawns == null || GObj_A1_Spawns.Length == 0) return false;
                        // Descartar los destruidos - En la ronda 2 pueden quedar referencias de la ronda 1 //
                            GameObject[] GObj_A1_Valid = new GameObject[0];
                            foreach (GameObject GObj_Candidate in GObj_A1_Spawns)
                            {
                                if (GObj_Candidate == null) continue;
                                AddNewToArray(ref GObj_A1_Valid, GObj_Candidate);
                            }
                            if (GObj_A1_Valid.Length == 0) return false;
                        // Colocar //
                            GameObject GObj_Chosen = GObj_A1_Valid[UnityEngine.Random.Range(0, GObj_A1_Valid.Length)];
                            this.transform.position = GObj_Chosen.transform.position;
                            this.transform.rotation = GObj_Chosen.transform.rotation;
                            if (TryGetComponent<Rigidbody>(out Rigidbody Rb_Self)) Rb_Self.linearVelocity = Vector3.zero;
                            return true;
                    }
                    // El mapa se genera de forma asincronica - Sin esto el jugador queda en (0,0,0), debajo del piso //
                    private System.Collections.IEnumerator RetrySpawnPositionRoutine()
                    {
                        float Flt_Timeout = 20f;
                        while (Flt_Timeout > 0f)
                        {
                            // Mantenerlo flotando sobre el origen para que no se caiga al vacio mientras espera //
                                this.transform.position = new Vector3(0f, 4f, 0f);
                                if (TryGetComponent<Rigidbody>(out Rigidbody Rb_Self)) Rb_Self.linearVelocity = Vector3.zero;
                            // Esperar //
                                yield return new WaitForSeconds(0.2f);
                                Flt_Timeout -= 0.2f;
                            // Reintentar //
                                if (TryPositionAtSpawnPoint())
                                {
                                    Debug.Log("[Base Player] Spawnpoint listo, jugador reposicionado");
                                    R_Crtn_SpawnRetry = null;
                                    yield break;
                                }
                        }
                        Debug.LogError("[Base Player] No aparecio ningun spawnpoint en 20s - El jugador queda sobre el origen");
                        R_Crtn_SpawnRetry = null;
                    }
                #endregion Spawn
                #region    Damage
                    public void ExecuteDeath()
                    {
                        // Ya estaba muerto - Evita contar la misma muerte dos veces //
                            if (R_Bool_IsDead) return;
                        // Variables //
                            R_Bool_IsDead = true;
                            I_Bool_IsMovingX = false;
                            I_Bool_IsMovingY = false;
                            R_Bool_IsRotating = false;
                            R_Bool_IsJumping = false;        
                            R_Bool_CanShoot = false;        
                        // Visibility //
                            SetVisibility(false);
                        // Respawn
                            ExecuteReSpawnDelay();
                    }
                #endregion Damage
            #endregion Combat
        #endregion Custom Methods
        #region Listener Methods
            #region    Callee Methods
                #region   OnStarted
                    public void OnStartedMouseMoving(Vector2 Vec2_Input) 
                    {
                        // Stop If Dead //
                            if (R_Bool_IsDead) return;
                        // Stop If Fallen //
                            if (O_Bool_IsFallen) return;  
                        // Stop If RotationLocked //
                            if (R_Bool_IsRotationLocked) return;      
                        // Proceed //
                            R_Flt_MouseDir = (Vec2_Input.y < 0)?
                                                -1:
                                                (Vec2_Input.y > 0)?
                                                    +1:
                                                    +0;   
                        // Have Controls Been Flipped ? //
                            R_Flt_MouseDir *= (O_Bool_IsFlipped) ? -1: +1;  
                            R_Flt_MouseDir *= (O_Bool_IsFlipped) ? -1: +1;   
                    }
                    public void OnStartedMoving(Vector2 Vec2_Input)
                    {
                        // Stop If Dead //
                            if (R_Bool_IsDead) return;
                        // Stop If Fallen //
                            if (O_Bool_IsFallen) return;
                        // El Vigilante solo se mueve en su ventana del ciclo //
                            if (R_E_PT_ClientType == PlayerType.Vigilant && R_Bool_MovementLocked) return;
                        // Proceed //
                            I_Bool_IsMovingX = true;
                            R_Int_CurrentInputMovementX = (Vec2_Input.x < 0)?
                                                            -1:
                                                            (Vec2_Input.x > 0)?
                                                              +1:
                                                              +0;  
                            I_Bool_IsMovingY = true;
                            R_Int_CurrentInputMovementY = (Vec2_Input.y < 0)?
                                                            -1:
                                                            (Vec2_Input.y > 0)?
                                                              +1:
                                                              +0;
                        // Have Controls Been Flipped ? //
                            R_Int_CurrentInputMovementX *= (O_Bool_IsFlipped) ? -1: +1;  
                            R_Int_CurrentInputMovementY *= (O_Bool_IsFlipped) ? -1: +1;  
                    }
                    public void OnStartedRotating(Vector2 Vec2_Input) 
                    {
                        // Stop If Dead //
                            if (R_Bool_IsDead) return;
                        // Stop If Fallen //
                            if (O_Bool_IsFallen) return;   
                        // Stop If RotationLocked //
                            if (R_Bool_IsRotationLocked) return; 
                        // Proceed //
                            R_Bool_IsRotating = true;
                            R_Int_CurrentInputRotationX = (Vec2_Input.x < 0)?
                                                            -1:
                                                            (Vec2_Input.x > 0)?
                                                              +1:
                                                              +0;  
                            R_Int_CurrentInputRotationY = (Vec2_Input.y < 0)?
                                                            -1:
                                                            (Vec2_Input.y > 0)?
                                                              +1:
                                                              +0;
                        // Have Controls Been Flipped ? //
                            R_Int_CurrentInputRotationX *= (O_Bool_IsFlipped) ? -1: +1;  
                            R_Int_CurrentInputRotationY *= (O_Bool_IsFlipped) ? -1: +1;  
                    }
                    public void OnStartedJumping()
                    {
                        // Stop If Dead //
                            if (R_Bool_IsDead) return;
                        // El Vigilante no salta cuando esta en su ventana quieta //
                            if (R_E_PT_ClientType == PlayerType.Vigilant && R_Bool_MovementLocked) return;
                        // Proceed //
                            if (!R_Bool_IsGrounded) return;

                            R_Bool_IsJumping = true;
                            ExecuteJump();
                    }
                    public void OnStartedStraightShooting() 
                    {
                        // Stop If Dead //
                            if (R_Bool_IsDead) return;
                        // Proceed //
                            if (!R_Bool_CanShoot) return;   
                        // Player Type Logic //
                            if (R_E_PT_ClientType == PlayerType.Vigilant)
                            {
                                InstaKillAttempt(FindNearestChaserInSight());
                            }
                        // El Corredor no dispara, solo usa items //
                    }
                    public void OnStartedCurvedShooting() 
                    {
                        // Stop If Dead //
                            if (R_Bool_IsDead) return;
                        // Proceed //
                            if (!R_Bool_CanShoot) return;    
                            if (R_E_PT_ClientType == PlayerType.Vigilant)
                            {
                                ExecuteStraightShot(Int_SkillIndex+1);
                            }
                            else
                            {
                                if (Int_ItemIndex > -1)
                                ExecuteStraightShot(Int_ItemIndex+4);
                                Int_ItemIndex = -1;
                            }
                            StartCoroutine(ReenableShootingDelay(MasterManager.Instance.CharacterManager.ShootCooldown));
                    }
                    public void OnStartedItemToggle() 
                    {
                        // Stop If Dead //
                            if (R_Bool_IsDead) return;
                        // Proceed //
                            if (!R_Bool_CanShoot) return;
                        // Stop If RotationLocked //
                            if (R_Bool_IsRotationLocked) return;      
                            ExecuteCurvedShot();
                            StartCoroutine(ReenableShootingDelay(MasterManager.Instance.CharacterManager.ShootCooldown));
                    }
                    public void OnStartedSkillToggle() 
                    {
                        // Stop If Dead //
                            if (R_Bool_IsDead) return;
                        // Proceed //
                            if (!R_Bool_CanShoot) return;
                        // Stop If RotationLocked //
                            if (R_Bool_IsRotationLocked) return;      
                            ExecuteCurvedShot();
                            StartCoroutine(ReenableShootingDelay(MasterManager.Instance.CharacterManager.ShootCooldown));
                    }
                #endregion OnStarted
                #region    OnStopped
                    public void OnStoppedMouseMoving(Vector2 Vec2_Input) 
                    {
                        // Proceed //
                            R_Flt_MouseDir = (Vec2_Input.y < 0)?
                                                -1:
                                                (Vec2_Input.y > 0)?
                                                    +1:
                                                    +0;   
                        // Have Controls Been Flipped ? //
                            R_Flt_MouseDir *= (O_Bool_IsFlipped) ? -1: +1;  
                            R_Flt_MouseDir *= (O_Bool_IsFlipped) ? -1: +1;   
                    }
                    public void OnStoppedMoving(Vector2 Vec2_Input) 
                    {
                        // Proceed //
                            I_Bool_IsMovingX = false;
                            R_Int_CurrentInputMovementX = (Vec2_Input.x < 0)?
                                                            -1:
                                                            (Vec2_Input.x > 0)?
                                                              +1:
                                                              +0;  
                            I_Bool_IsMovingY = true;
                            R_Int_CurrentInputMovementY = (Vec2_Input.y < 0)?
                                                            -1:
                                                            (Vec2_Input.y > 0)?
                                                              +1:
                                                              +0;   
                        // Have Controls Been Flipped ? //
                            R_Int_CurrentInputMovementX *= (O_Bool_IsFlipped) ? -1: +1;  
                            R_Int_CurrentInputMovementY *= (O_Bool_IsFlipped) ? -1: +1;   
                    }
                    public void OnStoppedRotating(Vector2 Vec2_Input) 
                    {
                        // Proceed //
                            R_Bool_IsRotating = false;
                            R_Int_CurrentInputRotationX = (Vec2_Input.x < 0)?
                                                            -1:
                                                            (Vec2_Input.x > 0)?
                                                              +1:
                                                              +0;  
                            R_Int_CurrentInputRotationY = (Vec2_Input.y < 0)?
                                                            -1:
                                                            (Vec2_Input.y > 0)?
                                                              +1:
                                                              +0;
                        // Have Controls Been Flipped ? //
                            R_Int_CurrentInputRotationX *= (O_Bool_IsFlipped) ? -1: +1;  
                            R_Int_CurrentInputRotationY *= (O_Bool_IsFlipped) ? -1: +1;  
                    }
                    public void OnStoppedJumping() 
                    {
                        R_Bool_IsJumping = false;
                    }       
                #endregion OnStopped
            #endregion Callee Methods
        #endregion Listener Methods
        #region    Coroutines Methods
            private System.Collections.IEnumerator SpeedPotions(float Flt_DelaySeconds, float Flt_Speed)
            {
                R_Flt_SpeedModifier = Mathf.Clamp(Flt_Speed, 0.1f, 2f);
                yield return new WaitForSeconds(Flt_DelaySeconds);
                R_Flt_SpeedModifier = 1;
            }
            private System.Collections.IEnumerator RespawnDelay(float Flt_DelaySeconds)
            {
                yield return new WaitForSeconds(Flt_DelaySeconds);
                // El respawn es un RPC a todos - Si lo dispara cada cliente se manda una vez por jugador //
                    if (!PhotonNetwork.IsMasterClient) yield break;
                    Debug.Log("[Base Player] Respawn de " + IO_Int_ID);
                    MasterManager.Instance.CharacterManager.SpawnCharacter(IO_Int_ID);
            }
            private System.Collections.IEnumerator ReenableShootingDelay(float Flt_DelaySeconds)
            {
                R_Bool_CanShoot = false;

                yield return new WaitForSeconds(Flt_DelaySeconds);

                R_Bool_CanShoot = true;
            }
            // Ciclo del Vigilante: se mueve 5s, queda quieto 3s. Camara y disparo nunca se bloquean //
            private System.Collections.IEnumerator ReenableRotation()
            {
                while (true)
                {
                    // Puede moverse //
                        R_Bool_MovementLocked = false;
                        yield return new WaitForSeconds(_vigilantMoveSeconds);
                    // Quieto //
                        R_Bool_MovementLocked = true;
                        I_Bool_IsMovingX = false;
                        I_Bool_IsMovingY = false;
                        R_Int_CurrentInputMovementX = 0;
                        R_Int_CurrentInputMovementY = 0;
                        yield return new WaitForSeconds(_vigilantStillSeconds);
                }
            }
            private System.Collections.IEnumerator ResetFlipping(float Flt_DelaySeconds)
            {
                O_Bool_IsFlipped = true;

                MasterManager.Instance.CharacterManager.ChangePlayerMaterial(null, 2);

                yield return new WaitForSeconds(Flt_DelaySeconds);

                MasterManager.Instance.CharacterManager.ChangePlayerMaterial(null);
        
                O_Bool_IsFlipped = false;
            }
            private System.Collections.IEnumerator ResetFalling(float Flt_DelaySeconds)
            {
                O_Bool_IsFallen = true;
        
                MasterManager.Instance.CharacterManager.ChangePlayerMaterial(null, 3);

                yield return new WaitForSeconds(Flt_DelaySeconds);
        
                MasterManager.Instance.CharacterManager.ChangePlayerMaterial(null);

                O_Bool_IsFallen = false;
            }
            private Coroutine activeRoutine;


            private System.Collections.IEnumerator GameOver()
            {
                if (MasterManager.Instance.CharacterManager.GameOver == true) 
                    yield break;
                else
                    yield return new WaitForSeconds(30);

                MasterManager.Instance.CharacterManager.EndGame(true);
            }
        #endregion Coroutines Methods
    #endregion Methods    

    #region    External Classe Methods
                public void OnHit(Collider Col_Hit)
                {
                    // Is Own Source? //
                        if (Col_Hit.gameObject.layer == 8 && Col_Hit.TryGetComponent<BasePlayer>(out BasePlayer Ctm_BP_Target1)) 
                        {
                            if (Ctm_BP_Target1.ID != ID)
                            { 
                                ExecuteTagged();
                            }
                        }
                    // Is Own's Child Source? //
                        else
                        if (Col_Hit.gameObject.layer == 8 && Col_Hit.transform.parent.gameObject.TryGetComponent<BasePlayer>(out BasePlayer Ctm_BP_Target2))
                        {
                            if (Ctm_BP_Target2.ID != ID)
                            { 
                                ExecuteTagged();
                            }
                        }
                    // Pickapable? //
                        else 
                        if (Col_Hit.gameObject.layer == 9)
                        {
                            Pickupable Ctm_Pkp_Item = Col_Hit.GetComponent<Pickupable>();
                            if (Ctm_Pkp_Item != null && Int_ItemIndex == -1)
                            {
                                Int_ItemIndex = Ctm_Pkp_Item.Type;
                                // Vuelve al pool en vez de destruirse //
                                    Ctm_Pkp_Item.PoolDespawn();
                                ((MenuTypeHUD) MasterManager.Instance.MenuManager.GetMenuReference("HUD")).UpdateItemDisplay(Int_ItemIndex);
                            }
                        }
                    
                }
                private MeshCollider R_Col_SelfCache = null;
                private readonly Collider[] R_Col_A1_HitBuffer = new Collider[10];
                public void PercieveContact(float Flt_FixedDT)
                {
                    // Cache - GetComponent cada frame es caro //
                        if (R_Col_SelfCache == null)
                            R_Col_SelfCache = I_GObj_A1_BodyParts[0].gameObject.GetComponent<MeshCollider>();
                        if (R_Col_SelfCache == null) return;
                    // Variables //
                        Bounds Bnds_Self = R_Col_SelfCache.bounds;
                        Transform Tfm_Self = R_Col_SelfCache.transform;
                        int Int_Count  = Physics.OverlapBoxNonAlloc(
                                                                        Bnds_Self.center,
                                                                        Bnds_Self.extents,
                                                                        R_Col_A1_HitBuffer,
                                                                        transform.rotation,
                                                                        I_LyrM_CollisionLayers
                                                                   );
                    // Collisions //
                        for (int Int_Index = 0; Int_Index < Int_Count; Int_Index++)
                        {
                            Collider Col_Hit = R_Col_A1_HitBuffer[Int_Index];
                            // Ignore null //
                                if (Col_Hit == null) continue;
                            // Ignore Own Collision //
                                if (Col_Hit == R_Col_SelfCache) continue;
                            // Compute Overlap //
                                bool Bool_IsOverlapping = Physics.ComputePenetration(
                                                                                        // In //
                                                                                            R_Col_SelfCache,
                                                                                            Tfm_Self.position,
                                                                                            Tfm_Self.rotation,
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
                }
        #region    Coroutines Methods
            #region    All
                public void HijackPush(Vector3 Vec3_Dir, float Flt_Force)  
                {
                    // Doesn't Affects Vigilant //
                        if (R_E_PT_ClientType == PlayerType.Vigilant) return;
                    // Proceed //
                        this.GetComponent<Rigidbody>().AddForce(Vec3_Dir * Flt_Force, ForceMode.Impulse);
                        Debug.Log("Pushed");
                }
            #endregion All
            #region    Vigilant
                #region    
                #endregion 
                #region    Damage Type
                    public void ExecuteTagged()
                    {
                        // Only Affects Vigilant - Tocaron al Vigilante //
                            if (R_E_PT_ClientType != PlayerType.Vigilant) return;
                        // Proceed - Ganan los Corredores //
                            Debug.Log("Tagged: Game Over");
                            MasterManager.Instance.CharacterManager.EndGame(false);
                    }
                #endregion Damage Type
                #region    Hijack Type
                    public void ToggleSkill()
                    {
                        // Only Affects Vigilant //
                            if (R_E_PT_ClientType != PlayerType.Vigilant) return;
                        // Proceed //
                            Int_SkillIndex = (Int_SkillIndex + 1 == 3) ? 0 : Int_SkillIndex + 1;
/* ! */                     //Menu Update//
                    }
                    public void HijackBomb(Vector3 Vec3_Source, float Flt_Force, float Flt_Radius, float Flt_Upward)  
                    {
                        // Doesn't Affects Vigilant //
                            if (R_E_PT_ClientType == PlayerType.Vigilant) return;
                        // Proceed //
                            this.GetComponent<Rigidbody>().AddExplosionForce(Flt_Force, Vec3_Source, Flt_Radius, Flt_Upward, ForceMode.Impulse);
                            Debug.Log("Bombed");
                    }
                    public void HijackFlip(float Flt_DelaySeconds)  
                    {
                        // Doesn't Affects Vigilant //
                            if (R_E_PT_ClientType == PlayerType.Vigilant) return;
                        // Exit ? //
                            if (O_Bool_IsFlipped == false) return;
                        // Proceed //
                            StartCoroutine(ResetFlipping(Flt_DelaySeconds));
                            Debug.Log("Flipped");
                    }
/* ! */             public void HijackFreeze()  
                    {
                        // Doesn't Affects Vigilant //
                            if (R_E_PT_ClientType == PlayerType.Vigilant) return;
                        // Proceed //
                            // Physical MATERIAL //
                            Debug.Log("Freeze");
                    }
                    // Busca al Corredor mas cercano dentro del cono de vision, sin depender de LayerMasks //
                    public BasePlayer FindNearestChaserInSight()
                    {
                        BasePlayer Ctm_BP_Best = null;
                        float Flt_BestDistance = float.MaxValue;
                        BasePlayer[] Ctm_BP_A1_All = MasterManager.Instance.CharacterManager.PlayerList;
                        if (Ctm_BP_A1_All == null) return null;
                        foreach (BasePlayer Ctm_BP_Other in Ctm_BP_A1_All)
                        {
                            if (Ctm_BP_Other == null) continue;
                            if (Ctm_BP_Other == this) continue;
                            if (Ctm_BP_Other.ClientType != PlayerType.Chaser) continue;
                            if (Ctm_BP_Other.IsDead) continue;
                            // Distancia //
                                Vector3 Vec3_Delta = Ctm_BP_Other.transform.position - this.transform.position;
                                float Flt_Distance = Vec3_Delta.magnitude;
                                if (Flt_Distance > _killRange) continue;
                            // Angulo - Tiene que estar adelante //
                                if (Vector3.Angle(this.transform.forward, Vec3_Delta.normalized) > (_killAngle * 0.5f)) continue;
                            // Mejor candidato //
                                if (Flt_Distance < Flt_BestDistance)
                                {
                                    Flt_BestDistance = Flt_Distance;
                                    Ctm_BP_Best = Ctm_BP_Other;
                                }
                        }
                        return Ctm_BP_Best;
                    }
                    // El Vigilante revisa si un Corredor lo alcanzo //
                    private bool R_Bool_TagReported = false;
                    private void CheckTagged()
                    {
                        if (R_Bool_TagReported) return;
                        if (R_E_PT_ClientType != PlayerType.Vigilant) return;
                        BasePlayer[] Ctm_BP_A1_All = MasterManager.Instance.CharacterManager.PlayerList;
                        if (Ctm_BP_A1_All == null) return;
                        foreach (BasePlayer Ctm_BP_Other in Ctm_BP_A1_All)
                        {
                            if (Ctm_BP_Other == null) continue;
                            if (Ctm_BP_Other == this) continue;
                            if (Ctm_BP_Other.ClientType != PlayerType.Chaser) continue;
                            if (Ctm_BP_Other.IsDead) continue;
                            if (Vector3.Distance(this.transform.position, Ctm_BP_Other.transform.position) > _tagRange) continue;
                            // Lo tocaron - Ganan los Corredores //
                                R_Bool_TagReported = true;
                                ExecuteTagged();
                                return;
                        }
                    }
                    public void InstaKillAttempt(BasePlayer BP_Target)
                    {
                        // Only Affects Vigilant - Es SU habilidad //
                            if (R_E_PT_ClientType != PlayerType.Vigilant) return;
                        // Sin objetivo cerca //
                            if (BP_Target == null) return;
                        // El objetivo ya paso el filtro de distancia y angulo - El SightSensor solo agrega obstaculos //
                            bool Bool_Visible = (R_Ctm_SS_SightSensor == null) ?
                                                    true :
                                                    R_Ctm_SS_SightSensor.IsEnemyVisible(BP_Target.transform);
                        // Proceed //
                            if (Bool_Visible)
                            {
                                BP_Target.ExecuteInstaDeath();
                            }
                            else
                            {
                                ForceResetRotationLock();
                            }
                            Debug.Log("Insta Kill Attempted");
                    }
                #endregion Hijack Type
                #region    Reset Type
                    public void ForceResetRotationLock()
                    {
                        // Fallo el intento - Se reposiciona, pero la camara sigue libre //
                            MasterManager.Instance.CharacterManager.SpawnCharacter(IO_Int_ID);
                    }
                #endregion Reset Type
            #endregion Vigilant
            #region    Chaser
                #region    
                #endregion 
                #region    Damage Type
                    private System.Collections.IEnumerator HitFlash()
                    {
                        // Sin materiales suficientes //
                            if (I_Mat_A1_Materials == null || I_Mat_A1_Materials.Length < 4) yield break;
                        // Flash //
                            I_GObj_A1_BodyParts[0].GetComponent<MeshRenderer>().material = I_Mat_A1_Materials[3];
                            yield return new WaitForSeconds(0.15f);
                        // Restaurar segun rol //
                            SetMaterial();
                    }
                    // Devuelve TRUE si el jugador sobrevivio al golpe //
                    // El filtro de "no es el Vigilante" lo hace el Character Manager, que mira el dueño //
                    // del PhotonView. Aca no se puede usar R_E_PT_ClientType: si el SetData todavia no //
                    // llego a esta maquina, el enum vale 0 = Vigilant y el golpe se perdia en silencio //
                    public bool ExecuteDamage(float Flt_Damage)
                    {
                        // Ya esta muerto - Se devuelve TRUE para que el Master no lo vuelva a matar //
                            if (R_Bool_IsDead) return true;
                        // Proceed //
                            IO_Flt_Health = Mathf.Max(0.0f, IO_Flt_Health - Flt_Damage);
                        // Feedback de impacto //
                            StartCoroutine(HitFlash());
                        // La muerte la dispara el Master desde el Character Manager, no cada cliente //
                            return (IO_Flt_Health > 0.0f);
                    }
                    public void ExecuteInstaDeath()
                    {
                        // Doesn't Affect Vigilant - Por dueño, no por enum (default = Vigilant) //
                            int Int_SelfActor = (photonView.Owner != null) ? photonView.Owner.ActorNumber : -1;
                            if (Int_SelfActor == MasterManager.Instance.CharacterManager.GetVigilantActorNumber()) return;
                        // Proceed //
                            Debug.Log("[Base Player] InstaKill sobre " + IO_Int_ID);
                            IO_Flt_Health = 0;
                            MasterManager.Instance.CharacterManager.KillCharacter(IO_Int_ID);
                    }
                #endregion Damage Type
                #region    Item Type
                    public void DestroyFromInventory()
                    {
                        // Doesn't Affect Vigilant //
                            if (R_E_PT_ClientType == PlayerType.Vigilant) return;
                        // Proceed //
                            Int_ItemIndex = -1;
                            ((MenuTypeHUD) MasterManager.Instance.MenuManager.GetMenuReference("HUD")).UpdateItemDisplay(Int_ItemIndex);
                    }
                #endregion Item Type
                #region    Hijack Type
                    public void ExecuteFlip()
                    {
                        // Doesn't Affect Vigilant //
                            if (R_E_PT_ClientType == PlayerType.Vigilant) return;
                        // Proceed //
                            StartCoroutine(ResetFlipping(I_Flt_ControlFlipSeconds));
                    }
                    public void ExecuteFalling()
                    {
                        // Doesn't Affect Vigilant //
                            if (R_E_PT_ClientType == PlayerType.Vigilant) return;
                        // Proceed //
                            StartCoroutine(ResetFalling(I_Flt_FallStateSeconds));
                    }
                    public void UsePotion(float Flt_DelaySeconds, float Flt_Speed)
                    {
                        // Doesn't Affect Vigilant //
                            if (R_E_PT_ClientType == PlayerType.Vigilant) return;
                        // Exit ? //
                            if (R_Flt_SpeedModifier != 1) return;
                        // Proceed //
                            StartCoroutine(SpeedPotions(Flt_DelaySeconds, Flt_Speed));
                    }
/* ! */             public void UseBanana()
                    {
                        // Doesn't Affect Vigilant //
                            if (R_E_PT_ClientType == PlayerType.Vigilant) return;
                        // Proceed //
                    }
                #endregion Hijack Type
            #endregion Chaser
        #endregion Coroutines Methods
    #endregion External Classe Methods
}
