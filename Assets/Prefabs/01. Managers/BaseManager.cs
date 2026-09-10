using Photon.Pun;
using UnityEngine;
using static CustomExtension.ArrayExtensions;

public class BaseManager<T> : MonoBehaviourPunCallbacks where T : MonoBehaviourPunCallbacks
{
    protected static T _instance;
    public static T Instance => _instance;

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;

            if (transform.parent != null)
            {
                transform.SetParent(null);
            }

            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public virtual T GetInstance()
    {
        return _instance;
    }

    public virtual void OnStartUp()
    {
    }

    public virtual void OnFixedUpdate(float Flt_FixedDT)
    {
    }

    public virtual void OnUpdate(float Flt_FixedDT)
    {
    }
}