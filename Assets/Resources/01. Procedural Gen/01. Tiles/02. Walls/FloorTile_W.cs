using UnityEngine;

#region    ...
#endregion ...

public class FloorTile_W : Tiles
{
    #region    Variables
        #region    Asset-To-Asset Data
            [SerializeField] GameObject _spawnPoint;
            private GameObject IO_GObj_SpawnPoint;
            public GameObject SpawnPoint => IO_GObj_SpawnPoint;
        #endregion Asset-To-Asset Data
        #region    Unity Methods
            protected void Awake()
            {
                IO_GObj_SpawnPoint = _spawnPoint;
            }
        #endregion Unity Methods
    #endregion Variables
}
