using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class SpinningTopsGameManager :MonoBehaviourPunCallbacks
{

    [Header("UI")]
    public GameObject uI_InformPanelGameobject;
    public TextMeshProUGUI uI_InformText;
    public GameObject searchForGamesButtonGameobject;
    public GameObject adjust_Button;
    public GameObject place_Button;
    public GameObject raycastCenter_Image;
    public GameObject scaleSlider;

    [Header("AR Related Gameobjects")]
    public GameObject arSession;
    public GameObject arSessionOrigin;

    [Header("Non-AR Gameobjects")]
    public GameObject nonARModeCamera;
   

    // Start is called before the first frame update
    void Start()
    {
        uI_InformPanelGameobject.SetActive(true);


        //Checking the game mode and according to that gameplayer scene setup will be adjusted.
        object gameModeType;
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(MultiplayerARSpinnerTopGame.GAME_MODE, out gameModeType))
        {
            if ((MultiplayerARSpinnerTopGame.GameModeTypes)gameModeType == MultiplayerARSpinnerTopGame.GameModeTypes.AR_Mode)
            {
                //Game mode is AR.
                Debug.Log("Game mode is AR");

                //Setting up the scene for Non-AR Game mode.
                //Basically, activating gameobjects related to AR scene and de-activating Non-AR stuff.

                //AR Related UI Gameobjects in the scene.
                adjust_Button.SetActive(true);
                place_Button.SetActive(true);
                raycastCenter_Image.SetActive(true);
                scaleSlider.SetActive(true);
                searchForGamesButtonGameobject.SetActive(false);


                //AR Foundation Gameobjects in the scene.
                arSession.SetActive(true);
                arSessionOrigin.SetActive(true);

                //Non-AR Gameobjects in the scene.
                nonARModeCamera.SetActive(false);


                uI_InformText.text = "Move phone to detect planes and place the Battle Arena!";


            }
            else if ((MultiplayerARSpinnerTopGame.GameModeTypes) gameModeType == MultiplayerARSpinnerTopGame.GameModeTypes.NonAR_Mode)
            {
                //Game mode is Non-AR.
                Debug.Log("Game mode is Non-AR");

                //Setting up the scene for Non-AR Game mode.
                //Basically, de-activating gameobjects related to AR scene.

                //AR Related UI Gameobjects in the scene.
                adjust_Button.SetActive(false);
                place_Button.SetActive(false);
                raycastCenter_Image.SetActive(false);
                scaleSlider.SetActive(false);
                searchForGamesButtonGameobject.SetActive(true);

                // AR Foundation Gameobjects in the scene.
                arSession.SetActive(false);
                arSessionOrigin.SetActive(false);

                //Non-AR Gameobjects in the scene.
                nonARModeCamera.SetActive(true);


                //Informing user to start the game
                uI_InformText.text = "Search for games to Battle!";

            }
        }

    }

  
    #region UI Callback Methods
    public void JoinRandomRoom()
    {
        uI_InformText.text = "Searching for available rooms...";

        PhotonNetwork.JoinRandomRoom();
        searchForGamesButtonGameobject.SetActive(false);
    }


    public void OnQuitMatchButtonClicked()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneLoader.Instance.LoadScene("Scene_Lobby");
        }
    }
    #endregion


    #region PHOTON Callback Methods
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
      
        Debug.Log(message);
        uI_InformText.text = message;

        //If there is no random room to join, a new random room is created.
        CreateAndJoinRoom();
    }


    public override void OnJoinedRoom()
    {
        adjust_Button.SetActive(false);
        raycastCenter_Image.SetActive(false);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            uI_InformText.text = "Joined to " + PhotonNetwork.CurrentRoom.Name + ". Waiting for other players...";


        }
        else
        {
            uI_InformText.text = "Joined to " + PhotonNetwork.CurrentRoom.Name;
            StartCoroutine(DeactivateAfterSeconds(uI_InformPanelGameobject, 2.0f));
        }

        Debug.Log( " joined to "+ PhotonNetwork.CurrentRoom.Name);
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined to "+ PhotonNetwork.CurrentRoom.Name+ " Player count "+ PhotonNetwork.CurrentRoom.PlayerCount);
        uI_InformText.text = newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " Player count " + PhotonNetwork.CurrentRoom.PlayerCount;

        StartCoroutine(DeactivateAfterSeconds(uI_InformPanelGameobject, 2.0f));
    }


    public override void OnLeftRoom()
    {
        SceneLoader.Instance.LoadScene("Scene_Lobby");
    }


    #endregion


    #region Private Methods
    void CreateAndJoinRoom()
    {
        string randomRoomName = "Room" + Random.Range(0,1000);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;

        //Creating the room
        PhotonNetwork.CreateRoom(randomRoomName,roomOptions);

    }

    IEnumerator DeactivateAfterSeconds(GameObject _gameObject, float _seconds)
    {
        yield return new WaitForSeconds(_seconds);
        _gameObject.SetActive(false);

    }


    #endregion



}
