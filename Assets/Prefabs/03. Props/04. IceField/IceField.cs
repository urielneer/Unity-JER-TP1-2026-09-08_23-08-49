using UnityEngine;

public class IceField : MonoBehaviour
{
    [SerializeField] private PhysicsMaterial _icePhysicMaterial;
    [SerializeField] private float _fieldDuration = 6f;

    private void Start()
    {
        Destroy(gameObject, _fieldDuration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<BasePlayer>(out BasePlayer player))
        {
            if (other.TryGetComponent<Collider>(out Collider col))
            {
                col.material = _icePhysicMaterial;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<BasePlayer>(out BasePlayer player))
        {
            if (other.TryGetComponent<Collider>(out Collider col))
            {
                col.material = null;
            }
        }
    }
}