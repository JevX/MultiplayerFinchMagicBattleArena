using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameModeSelectionManager : MonoBehaviour
{


    #region UI Callback Methods
    public void OnARModeSelected()
    {
        //Setting the gamemode to player custom properties so that Game Mode info can be accessed in all scenes. 
        //Setting Game mode as AR.
        ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable { { MultiplayerARSpinnerTopGame.GAME_MODE, MultiplayerARSpinnerTopGame.GameModeTypes.AR_Mode} };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);

        //Loading the gameplay scene
        SceneLoader.Instance.LoadScene("Scene_Gameplay");


    }

    public void OnNonARModeSelected()
    {
        //Setting the gamemode to player custom properties so that Game Mode info can be accessed in all scenes. 
        //Setting Game mode as Non-AR.
        ExitGames.Client.Photon.Hashtable playerSelectionProp = new ExitGames.Client.Photon.Hashtable { { MultiplayerARSpinnerTopGame.GAME_MODE, MultiplayerARSpinnerTopGame.GameModeTypes.NonAR_Mode} };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProp);

        //Loading the gameplay scene
        SceneLoader.Instance.LoadScene("Scene_Gameplay");
    }


    #endregion

}
