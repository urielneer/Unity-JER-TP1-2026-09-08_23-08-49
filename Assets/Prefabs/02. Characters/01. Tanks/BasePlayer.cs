using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UIElements;

public class BasePlayer : MonoBehaviourPunCallbacks
{
    #region    Variables
        #region    Player Data
            string IO_Str_OwnerNickname;
            public string OwnerNickname => IO_Str_OwnerNickname;
            int IO_Int_ID;
            public int ID => IO_Int_ID;
            float IO_Flt_Health;
            public float Health => IO_Flt_Health;

            private bool R_Bool_CanShoot = true;
            private bool R_Bool_IsDead = false;
        #endregion Player Data   
        #region    Character 
            [Header(" Camera Settings")]
                [SerializeField] Transform _cameraAnchor;
                private Transform IO_Tfm_CameraAnchor;
                public Transform CameraAnchor => IO_Tfm_CameraAnchor; 
                [SerializeField] Vector3 _cameraPosition;
                private Vector3 I_Vec3_CamPos;
                [SerializeField] Vector3 _cameraRotation;
                private Vector3 I_Vec3_CamRot;
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
    #endregion Variables
    #region    Methods
        #region    Unity Methods
            public void Awake()
            {
                // Body //
                    IO_Tfm_CanonAnchor = _canonAnchor;
                    I_Vec2_CanonRotationBounds = _canonRotationBounds;
                    I_Flt_CanonRotationStrength = _canonRotationStrength;
                    I_GObj_A1_BodyParts =_bodyParts;
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
            }
        #endregion Unity Methods
        #region    Override Methods
            public virtual void OnStartUp()
            {
                // Cooldown Flags //
                    R_Bool_CanShoot = true;
                    R_Bool_IsDead = false;
                // Attach camera //
                    Camera Cam_Main = UnityEngine.Object.FindFirstObjectByType<Camera>();
                    Cam_Main.transform.SetParent(this.CameraAnchor.gameObject.transform, false);
                // Reposition transform //
                    Cam_Main.transform.localPosition = I_Vec3_CamPos;
                    Cam_Main.transform.localEulerAngles = I_Vec3_CamRot;
                // Spawn //
                    ExecuteSpawn();
            }
            public virtual void OnUpdate(float Flt_FixedDT)
            {
                // Only execute Own //
                    if (!GetComponent<PhotonView>().IsMine) return;
                // Character Logic //
                    ExecuteJumpReset(Flt_FixedDT);
            }
            public virtual void OnFixedUpdate(float Flt_FixedDT)
            {
                // Only execute Own //
                    if (!GetComponent<PhotonView>().IsMine) return;
                // Character Logic //
                    ExecuteMovement(Flt_FixedDT);
                    ExecuteBodyRotation(Flt_FixedDT);
                    ExecuteCanonRotation(Flt_FixedDT);
                    ExecuteJumpVariableGravity(Flt_FixedDT);
            }
        #endregion Override Methods
        #region    Custom Methods
            #region    Player Data
                public void SetData(string Str_Input, int Int_Input, float Flt_Input)
                {
                    IO_Str_OwnerNickname = Str_Input;
                    IO_Int_ID = Int_Input;
                    IO_Flt_Health = Flt_Input;
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
                                    GObj_Element.GetComponent<Collider>().enabled = Bool_IsVisible;
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
                        float Flt_YDegrees = R_Int_LastInputRotation * R_Flt_CurrentRotationSpeed * Flt_FixedDT;
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
                private void ExecuteMovement(float Flt_FixedDT)
                {
                    // Variables //
                        Vector3 Vec3_Displacement = Vector3.zero;
                    // Set Displacement //
                        Vec3_Displacement += XAxisMovement(Flt_FixedDT);
                        Vec3_Displacement += YAxisMovement(Flt_FixedDT);
                    // Apply Movement //
                        if (Vec3_Displacement.sqrMagnitude > 0f)
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
                        return transform.right * R_Int_LastInputMovementX * I_Flt_CurrentMovementSpeedX * Flt_FixedDT;
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
                        return transform.forward * R_Int_LastInputMovementY * I_Flt_CurrentMovementSpeedY * Flt_FixedDT;
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
                    private void ExecuteShot()
                    {
                        R_Bool_CanShoot = false;
                        MasterManager.Instance.BulletManager.SynchronizeBullet(this, this.transform.forward);
                    }
                #endregion Shoot
                #region    Spawn
                    public void ExecuteSpawn()
                    {      
                        // Variables //
                            R_Bool_CanShoot = true;
                            R_Bool_IsDead = false;
                        // Position //
                            int Int_RandIndex = UnityEngine.Random.Range(0, MasterManager.Instance.MapManager.SpawnPoints.Length);
                            this.transform.position = MasterManager.Instance.MapManager.SpawnPoints[Int_RandIndex].gameObject.transform.position;
                            this.transform.rotation = MasterManager.Instance.MapManager.SpawnPoints[Int_RandIndex].gameObject.transform.rotation;
                        // Visibility //
                            SetVisibility(true);
                    }
                    private void ExecuteReSpawnDelay()
                    {
                        StartCoroutine(RespawnDelay(MasterManager.Instance.GameManager.RespawnCooldown));
                    }
                #endregion Spawn
                #region    Damage
                    public void ExecuteDeath()
                    {
                        // Variables //
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
                    public void ExecuteDamage(float Flt_Damage)
                    {
                        IO_Flt_Health = Mathf.Max(0.0f, IO_Flt_Health - Flt_Damage);
                        if (IO_Flt_Health <= 0) MasterManager.Instance.CharacterManager.KillCharacter(IO_Int_ID);
                    }
                #endregion Damage
            #endregion Combat
        #endregion Custom Methods
        #region Listener Methods
            #region    Callee Methods
                #region   OnStarted
                    public void OnStartedMoving(Vector2 Vec2_Input) 
                    {
                        // Stop If Dead //
                            if (R_Bool_IsDead) return;
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
                    }
                    public void OnStartedRotating(Vector2 Vec2_Input) 
                    {
                        // Stop If Dead //
                            if (R_Bool_IsDead) return;
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
                    }
                    public void OnStartedJumping() 
                    {
                        // Stop If Dead //
                            if (R_Bool_IsDead) return;
                        // Proceed //
                            if (!R_Bool_IsGrounded) return;

                            R_Bool_IsJumping = true;
                            ExecuteJump();
                    }
                    public void OnStartedShooting() 
                    {
                        // Stop If Dead //
                            if (R_Bool_IsDead) return;
                        // Proceed //
                            if (!R_Bool_CanShoot) return;
                            ExecuteShot();
                            StartCoroutine(ReenableShootingDelay(MasterManager.Instance.CharacterManager.ShootCooldown));
                    }
                #endregion OnStarted
                #region    OnStopped
                    public void OnStoppedMoving(Vector2 Vec2_Input) 
                    {
                        // Stop If Dead //
                            if (R_Bool_IsDead) return;
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
                    }
                    public void OnStoppedRotating(Vector2 Vec2_Input) 
                    {
                        // Stop If Dead //
                            if (R_Bool_IsDead) return;
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
                    }
                    public void OnStoppedJumping() 
                    {
                        R_Bool_IsJumping = false;
                    }       
                #endregion OnStopped
            #endregion Callee Methods
        #endregion Listener Methods
        #region    Coroutines Methods
            private System.Collections.IEnumerator RespawnDelay(float Flt_DelaySeconds)
            {
                yield return new WaitForSeconds(Flt_DelaySeconds);
                MasterManager.Instance.CharacterManager.SpawnCharacter(IO_Int_ID);
            }
            private System.Collections.IEnumerator ReenableShootingDelay(float Flt_DelaySeconds)
            {
                R_Bool_CanShoot = false;

                yield return new WaitForSeconds(Flt_DelaySeconds);

                R_Bool_CanShoot = true;
            }
        #endregion Coroutines Methods
    #endregion Methods
}
