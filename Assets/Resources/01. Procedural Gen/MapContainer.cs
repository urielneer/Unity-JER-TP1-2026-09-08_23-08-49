
using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using static CustomExtension.ArrayExtensions;

#region    "using" Script Clarification
    using Hashtable = ExitGames.Client.Photon.Hashtable;
#endregion "using" Script Clarification
public class MapContainer : MonoBehaviourPunCallbacks
{
    #region    Variables
        #region    Hashtables (Communication among instances)
            // Map //
                private const string MapTileRadius_KEY = "MapTileRadius";
                private const string MapAxisX_KEY = "MapAxisX";
                private const string MapAxisY_KEY = "MapAxisY";
                private const string MapData_KEY = "MapData";
                private const string MapWallIDs_KEY = "MapWallIDs";
            // Character //
                private const string PlayerID_KEY = "PlayerID";
                private const string PlayerList_KEY = "PlayerList";
            // Character //
                private const string BulletOwnerID_KEY = "BulletOwnerID";
                private const string BulletOwnerName_KEY = "BulletOwnerName";
                private const string BulletPos_KEY = "BulletPos";
                private const string BulletQuat_KEY = "BulletQuat";
                private const string BulletDir_KEY = "BulletDir";
                private const string BulletType_KEY = "BulletType";
        #endregion Hashtables (Communication among instances)
    #endregion Variables
    #region    Methods
        #region    Unity Methods
            public void Awake()
            {
            }
            void Start()
            {

            }
        #endregion Unity Methods
        #region    Override Methods
            public virtual void OnStartUp()
            {
            }
            public virtual void OnFixedUpdate()
            {
            }
        #endregion Override Methods
        #region    Custom Methods
        #endregion Custom Methods
        #region    PUN         
            #region    Hastable
            #endregion Hastable  
            #region    RPC
                #region    Hashtables
                #endregion Hashtables 
                #region    Await
                #endregion Await 
            #endregion RPC
        #endregion PUN
    #endregion Methods
}
