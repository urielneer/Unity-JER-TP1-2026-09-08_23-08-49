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
            [SerializeField] private BaseMenuLabel _itemLabel;
            private BaseMenuLabel IO_Ctm_BML_ItemLabel;
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
                    IO_Ctm_BML_ItemLabel = _itemLabel;
            }
        #endregion Unity Methods
        #region    Custom Methods
            public void UpdateItemDisplay(int Int_ItemIndex)
            {
                // Variables //
                    // Nombres de ítems de Corredor - Pickupable.Type 0-3 = Bullet type 4-7 //
                        string[] Str_A1_ItemNames = { "Poción Lenta", "Banana", "Poción Rápida", "Bomba" };
                    string Str_Display = (Int_ItemIndex >= 0 && Int_ItemIndex < Str_A1_ItemNames.Length) ? Str_A1_ItemNames[Int_ItemIndex] : "Ninguno";
                // Update //
                    IO_Ctm_BML_ItemLabel.Overwrite(Str_Display);
            }
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