using UnityEngine;

#region    ...
#endregion ...

public class FloorTile_W : MonoBehaviour
{
    #region    Variables
        #region    Asset-To-Asset Data
            [SerializeField] GameObject _spawnPoint;
            private GameObject IO_GObj_SpawnPoint;
            public GameObject SpawnPoint => IO_GObj_SpawnPoint;
        #region    Unity Methods
            protected void Awake()
            {
                IO_GObj_SpawnPoint = _spawnPoint;
            }
        #endregion Unity Methods
        #endregion Asset-To-Asset Data
    #endregion Variables
}
