using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonManager : MonoBehaviour, IConnectionCallbacks, ILobbyCallbacks, IMatchmakingCallbacks, IInRoomCallbacks
{
    [SerializeField] private ServerSettings _serverSettings;
    [Space]
    [SerializeField] private Canvas _roomsCanvas;
    [SerializeField] private MainMenuPanel _mainMenuPanel;
    [SerializeField] private CreateRoomPanel _createRoomPanel;
    [SerializeField] private JoinRoomPanel _joinRoomPanel;
    [SerializeField] private InRoomPanel _inRoomPanel;

    private LoadBalancingClient _lbc;

    private byte _maxPlayers;

    private TypedLobby _customLobby = new TypedLobby("customLobby", LobbyType.Default);

    private Dictionary<string, RoomInfo> _cachedRoomList = new Dictionary<string, RoomInfo>();

    #region UnityMethods

    void Start()
    {
        _roomsCanvas.enabled = false;

        _lbc = new LoadBalancingClient();
        _lbc.AddCallbackTarget(this);

        if (!_lbc.ConnectUsingSettings(_serverSettings.AppSettings))
            Debug.LogError("Error connection!");

        SubscribeButtons();
    }

    void Update()
    {
        if (_lbc == null)
            return;

        _lbc.Service();

        var state = _lbc.State.ToString();
        //Debug.Log($"State: {state}, userId: {_lbc.UserId}");
    }

    private void OnDestroy()
    {
        _lbc.RemoveCallbackTarget(this);
        UnsubscribeButtons();
    }

    #endregion


    #region Methods

    private void SubscribeButtons()
    {
        _mainMenuPanel.CreateRoomButton.onClick.AddListener(ShowCreateRoomPanel);
        _mainMenuPanel.JoinRoomButton.onClick.AddListener(ShowJoinRoomPanel);

        _createRoomPanel.CreateRoom.onClick.AddListener(CreateRoom);
        _createRoomPanel.BackButton.onClick.AddListener(ShowMainMenuPanel);

        _joinRoomPanel.BackButton.onClick.AddListener(ShowMainMenuPanel);
        _joinRoomPanel.JoinButton.onClick.AddListener(JoinRoom);

        _inRoomPanel.LeaveRoomButton.onClick.AddListener(LeaveRoom);
        _inRoomPanel.IsOpenToggle.onValueChanged.AddListener(SetRoomOpen);
    }

    private void UnsubscribeButtons()
    {
        _mainMenuPanel.CreateRoomButton.onClick.RemoveAllListeners();
        _mainMenuPanel.JoinRoomButton.onClick.RemoveAllListeners();

        _createRoomPanel.CreateRoom.onClick.RemoveAllListeners();
        _createRoomPanel.BackButton.onClick.RemoveAllListeners();

        _joinRoomPanel.BackButton.onClick.RemoveAllListeners();
        _joinRoomPanel.JoinButton.onClick.RemoveAllListeners();

        _inRoomPanel.LeaveRoomButton.onClick.RemoveAllListeners();
        _inRoomPanel.IsOpenToggle.onValueChanged.RemoveAllListeners();
    }

    private void ShowMainMenuPanel()
    {
        _mainMenuPanel.SetEnabled(true);
        _createRoomPanel.SetEnabled(false);
        _joinRoomPanel.SetEnabled(false);
        _inRoomPanel.SetEnabled(false);
    }

    private void ShowCreateRoomPanel()
    {
        _createRoomPanel.SetEnabled(true);
        _mainMenuPanel.SetEnabled(false);
        _joinRoomPanel.SetEnabled(false);
        _inRoomPanel.SetEnabled(false);
    }

    private void ShowJoinRoomPanel()
    {
        _joinRoomPanel.SetEnabled(true);
        _mainMenuPanel.SetEnabled(false);
        _createRoomPanel.SetEnabled(false);
        _inRoomPanel.SetEnabled(false);
    }

    private void ShowInRoomPanel()
    {
        _inRoomPanel.SetEnabled(true);
        _createRoomPanel.SetEnabled(false);
        _joinRoomPanel.SetEnabled(false);
        _mainMenuPanel.SetEnabled(false);

        _inRoomPanel.SetRoomName(_lbc.CurrentRoom.Name);
        UpdateRoomControl();
    }

    private void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = _maxPlayers;

        EnterRoomParams enterRoomParams = new EnterRoomParams();
        enterRoomParams.RoomOptions = roomOptions;
        if (!string.IsNullOrWhiteSpace(_createRoomPanel.Input.text))
            enterRoomParams.RoomName = _createRoomPanel.Input.text;

        _lbc.OpCreateRoom(enterRoomParams);
    }

    private void JoinRoom()
    {
        if (string.IsNullOrEmpty(_joinRoomPanel.SelectedRoomName))
            return;

        JoinRoom(_joinRoomPanel.SelectedRoomName);
    }

    private void JoinRoom(string roomName)
    {
        EnterRoomParams enterRoomParams = new EnterRoomParams();
        enterRoomParams.RoomName = roomName;
        _lbc.OpJoinRoom(enterRoomParams);
    }

    private void LeaveRoom()
    {
        _lbc.OpLeaveRoom(false);
    }

    private void SetRoomOpen(bool isOpen)
    {
        if (CheckIfMasterClient())
            _lbc.CurrentRoom.IsOpen = isOpen;
    }

    private bool CheckIfMasterClient()
    {
        var masterClient = _lbc.CurrentRoom.Players[_lbc.CurrentRoom.MasterClientId];

        return masterClient != null && masterClient.UserId.Equals(_lbc.UserId);
    }

    private void UpdateRoomControl()
    {
        var isMasterClient = CheckIfMasterClient();

        _inRoomPanel.IsOpenToggle.interactable = isMasterClient;
        _inRoomPanel.IsOpenToggle.isOn = _lbc.CurrentRoom.IsOpen;
    }

    #endregion


    #region IConnectionCallbacks

    public void OnConnected()
    {
        
    }

    public void OnConnectedToMaster()
    {
        _lbc.OpJoinLobby(_customLobby);
        _roomsCanvas.enabled = true;
        ShowMainMenuPanel();
    }

    public void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"OnDisconnected: {cause}");
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
        
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
        
    }

    #endregion


    #region ILobbyCallbacks

    public void OnJoinedLobby()
    {
        Debug.Log("Joined lobby");
    }

    public void OnLeftLobby()
    {
        
    }

    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate");

        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                _cachedRoomList.Remove(info.Name);
                _joinRoomPanel.RemoveRoom(info.Name);
            }
            else
            {
                if (_cachedRoomList.ContainsKey(info.Name))
                    _cachedRoomList[info.Name] = info;
                else
                {
                    _cachedRoomList.Add(info.Name, info);
                    _joinRoomPanel.AddRoom(info.Name);
                }
                _joinRoomPanel.UpdateRoomIsOpen(info.Name, info.IsOpen);
            }
        }
    }

    public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        
    }

    #endregion


    #region IMatchmakingCallbacks

    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        
    }

    public void OnCreatedRoom()
    {
        Debug.Log("Room created");
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Create room failed: {message}");
    }

    public void OnJoinedRoom()
    {
        Debug.Log($"Joined room: {_lbc.CurrentRoom.Name}");
        ShowInRoomPanel();
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Join room failed: {message}");
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
        
    }

    public void OnLeftRoom()
    {
        Debug.Log("Left room");
        ShowMainMenuPanel();
    }

    #endregion


    #region IInRoomCallbacks

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.UserId} entered the room");
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.UserId} left the room");
    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        UpdateRoomControl();
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"{newMasterClient.UserId} is now Master Client");
        UpdateRoomControl();
    }

    #endregion
}
