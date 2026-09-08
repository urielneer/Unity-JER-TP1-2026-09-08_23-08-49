using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using static CustomExtension.ArrayExtensions;

public class MenuTypeHUD : BaseMenuType
{
    #region    Variables
        [Header("HUD GUI Settings")]
            [SerializeField] private Menu_ChatElement _chat;
            private Menu_ChatElement IO_Ctm_MCE_Chat;   
            public  Menu_ChatElement ChatElement => IO_Ctm_MCE_Chat;
            [SerializeField] private MenuListContainerKillFeed _killFeed;
            private MenuListContainerKillFeed IO_Ctm_MLCKF_KillFeed;  
            public MenuListContainerKillFeed KillFeed => IO_Ctm_MLCKF_KillFeed; 
    #endregion Variables
    #region    Methods
        #region    Unity Methods
            protected override void Awake()
            {
                // Resume Base //
                    base.Awake();
                // Variables //
                    IO_Ctm_MCE_Chat = _chat;
                    IO_Ctm_MLCKF_KillFeed = _killFeed;
            }
        #endregion Unity Methods
        #region    Custom Methods
            public void LeftRoom()
            {
                IO_Ctm_MCE_Chat.LeftRoom();
                IO_Ctm_MLCKF_KillFeed.LeftRoom();
            }
            public void JoinRoom()
            {
                IO_Ctm_MCE_Chat.JoinRoom();
                IO_Ctm_MLCKF_KillFeed.JoinRoom();
            }
            public void GameStart()
            {
                IO_Ctm_MCE_Chat.GameStart();
                IO_Ctm_MLCKF_KillFeed.GameStart();
            }
            public void GameEnd()
            {
                IO_Ctm_MCE_Chat.GameEnd();
                IO_Ctm_MLCKF_KillFeed.GameEnd();
            }
        #endregion Custom Methods
    #endregion Methods
}