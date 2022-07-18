using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region Fields

    [SerializeField] private Button _leaveRoomButton;
    [SerializeField] private GameObject _playerPrefab;

    #endregion


    #region UnityMethods

    void Start()
    {
        _leaveRoomButton.onClick.AddListener(LeaveRoom);

        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(2);

            return;
        }

        PhotonNetwork.Instantiate(_playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
    }

    private void OnDestroy()
    {
        _leaveRoomButton.onClick.RemoveAllListeners();
    }

    #endregion


    #region Methods

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion


    #region PUN Callbacks

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.UserId} entered the room");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.UserId} left the room");
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(2);
    }

    #endregion
}
