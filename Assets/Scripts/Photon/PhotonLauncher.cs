using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    #region Fields

    [SerializeField] TextMeshProUGUI _infoText;
    [SerializeField] Button _connectButton;
    [SerializeField] Button _disconnectButton;
    [Space]
    [SerializeField] RectTransform _roomPanel;
    [SerializeField] Button _joinRoomButton;
    [SerializeField] Button _leaveRoomButton;
    [SerializeField] TextMeshProUGUI _roomInfoText;
    [SerializeField] TMP_InputField _roomNameInput;

    private string _gameVersion = "1";

    #endregion


    #region UnityMethods

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        _gameVersion = Application.version;

        _connectButton.onClick.AddListener(OnConnectButtonClick);
        _disconnectButton.onClick.AddListener(OnDisonnectButtonClick);
        _connectButton.gameObject.SetActive(true);
        _disconnectButton.gameObject.SetActive(false);

        _roomPanel.gameObject.SetActive(false);
        _joinRoomButton.onClick.AddListener(OnJoinRoomButtonClick);
        _leaveRoomButton.onClick.AddListener(OnLeaveRoomButtonClick);
    }

    #endregion


    #region PUN Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("You are now connected to PUN Master server");
        _infoText.text = $"<color=#009900>You are now connected to PUN Master server</color>";

        _connectButton.gameObject.SetActive(false);
        _disconnectButton.gameObject.SetActive(true);
        _roomPanel.gameObject.SetActive(true);
        _joinRoomButton.gameObject.SetActive(true);
        _leaveRoomButton.gameObject.SetActive(false);
        _roomNameInput.interactable = true;
        _roomInfoText.text = "";

        base.OnConnected();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (cause == DisconnectCause.DisconnectByClientLogic)
        {
            Debug.Log($"You are no longer connected to Photon Network");
            _infoText.text = $"<color=#0000ff>You are no longer connected to Photon Network</color>";
        }
        else
        {
            Debug.Log($"Disconnected due to:\n{cause}");
            _infoText.text = $"<color=#ff0000>Disconnected due to:\n{cause}</color>";
        }

        _connectButton.gameObject.SetActive(true);
        _connectButton.interactable = true;
        _disconnectButton.gameObject.SetActive(false);
        _roomPanel.gameObject.SetActive(false);

        base.OnDisconnected(cause);
    }

    public override void OnJoinedRoom()
    {
        _joinRoomButton.gameObject.SetActive(false);
        _leaveRoomButton.gameObject.SetActive(true);
        _roomNameInput.interactable = false;

        Debug.Log("You joined the room");
        _roomInfoText.text = $"<color=#009900>You have joined the room</color>";

        base.OnJoinedRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Unable to join the room due to:\n{message}");
        _roomInfoText.text = $"<color=#ff0000>Unable to join the room due to:zn{message}</color>";

        base.OnJoinRoomFailed(returnCode, message);
    }


    public override void OnLeftRoom()
    {
        _joinRoomButton.gameObject.SetActive(true);
        _leaveRoomButton.gameObject.SetActive(false);
        _roomNameInput.interactable = true;

        Debug.Log($"You left the room");
        _roomInfoText.text = $"<color=#0000ff>You left the room</color>";

        base.OnLeftRoom();
    }

    #endregion


    #region Methods

    private void OnConnectButtonClick()
    {
        _connectButton.interactable = false;
        _infoText.text = "Connecting...";

        PhotonNetwork.GameVersion = _gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    private void OnDisonnectButtonClick()
    {
        PhotonNetwork.Disconnect();
    }

    private void OnJoinRoomButtonClick()
    {
        var roomName = _roomNameInput.text.Trim();
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.Log($"Enter a room name");
            _roomInfoText.text = $"<color=#ff0000>Enter a room name</color>";
            return;
        }

        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = 2, IsVisible = true }, TypedLobby.Default);
    }

    private void OnLeaveRoomButtonClick()
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion
}
