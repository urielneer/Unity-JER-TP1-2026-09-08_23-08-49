using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BasePlayer : MonoBehaviourPunCallbacks
{
    public string OwnerNickname { get; private set; }
    public int ID { get; private set; }
    public float Health { get; private set; }

    [Header("Camera Settings")]
    [SerializeField] private Transform cameraAnchor;
    [SerializeField] private Vector3 cameraPosition;
    [SerializeField] private Vector3 cameraRotation;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float rotationSpeed = 15f;

    [Header("Combat Settings")]
    [SerializeField] private GameObject stonePrefab;
    [SerializeField] private Transform canonAnchor;
    [SerializeField] private float throwCooldown = 1.5f;

    public Transform CanonAnchor => canonAnchor;

    private Rigidbody rb;
    private Vector3 moveInput;
    private float lastThrowTime = -999f;
    private bool isDead = false;
    private bool isShooting = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // Forzamos la ejecución de OnStartUp por si el MasterManager no lo está llamando explícitamente
        OnStartUp();
    }

    public virtual void OnStartUp()
    {
        ExecuteSpawn();

        // Si este jugador no es el nuestro en la red, ignoramos la cámara para que no la roben
        if (photonView != null && !photonView.IsMine) return;

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            if (cameraAnchor == null)
            {
                Transform foundAnchor = transform.Find("CameraAnchor");
                if (foundAnchor != null)
                {
                    cameraAnchor = foundAnchor;
                }
                else
                {
                    GameObject newAnchor = new GameObject("CameraAnchor");
                    newAnchor.transform.SetParent(transform);
                    newAnchor.transform.localPosition = new Vector3(0f, 1.5f, -3f);
                    cameraAnchor = newAnchor.transform;
                }
            }

            mainCamera.transform.SetParent(cameraAnchor, false);
            mainCamera.transform.localPosition = cameraPosition == Vector3.zero ? new Vector3(0f, 0f, 0f) : cameraPosition;
            mainCamera.transform.localEulerAngles = cameraRotation == Vector3.zero ? Vector3.zero : cameraRotation;
        }
    }

    public void OnUpdate()
    {
        if (!photonView.IsMine || isDead) return;

        if (isShooting && Time.time >= lastThrowTime + throwCooldown)
        {
            ThrowStone();
        }
    }

    public void OnFixedUpdate()
    {
        if (!photonView.IsMine || isDead) return;

        if (moveInput.sqrMagnitude > 0.01f)
        {
            Vector3 targetPosition = rb.position + moveInput * movementSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPosition);

            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    private void ThrowStone()
    {
        lastThrowTime = Time.time;

        if (stonePrefab != null && canonAnchor != null)
        {
            PhotonNetwork.Instantiate(stonePrefab.name, canonAnchor.position, canonAnchor.rotation);
        }
    }

    public void SetData(string nickname, int id, float health)
    {
        OwnerNickname = nickname;
        ID = id;
        Health = health;
    }

    public void SetVisibility(bool isVisible)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            r.enabled = isVisible;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
        {
            c.enabled = isVisible;
        }

        if (rb != null)
        {
            rb.useGravity = isVisible;
        }
    }

    public void ExecuteDamage(float amount = 10f)
    {
        if (isDead) return;

        Health -= amount;
        if (Health <= 0)
        {
            ExecuteDeath();
        }
    }

    public void ExecuteDeath()
    {
        isDead = true;
        SetVisibility(false);
    }

    public void ExecuteSpawn()
    {
        isDead = false;
        Health = 100f;
        SetVisibility(true);
        lastThrowTime = Time.time; // Resetea el tiempo para evitar disparos al spawnear
    }

    public void OnStartedMoving(Vector2 input = default)
    {
        moveInput = new Vector3(input.x, 0f, input.y).normalized;
    }

    public void OnStoppedMoving()
    {
        moveInput = Vector3.zero;
    }

    public void OnStartedRotating(Vector2 input = default)
    {
    }

    public void OnStoppedRotating()
    {
    }

    public void OnStartedJumping()
    {
    }

    public void OnStoppedJumping()
    {
    }

    public void OnStartedShooting()
    {
        if (photonView.IsMine)
        {
            isShooting = true;
        }
    }

    public void OnStoppedShooting()
    {
        if (photonView.IsMine)
        {
            isShooting = false;
        }
    }
}