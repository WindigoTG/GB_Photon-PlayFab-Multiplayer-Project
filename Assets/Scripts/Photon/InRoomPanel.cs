using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class InRoomPanel : BasePanel
{
    [SerializeField] private Button _leaveRoomButton;
    [SerializeField] private Button _startGameButton;
    [SerializeField] private TMP_InputField _roomNameInput;
    [SerializeField] private Toggle _isOpenToggle;
    [SerializeField] private Toggle _isVisibleToggle;

    public Button LeaveRoomButton => _leaveRoomButton;
    public Button StartGameButton => _startGameButton;
    public Toggle IsOpenToggle => _isOpenToggle;
    public Toggle IsVisibleToggle => _isVisibleToggle;

    public void SetRoomName(string roomName)
    {
        _roomNameInput.text = roomName;
    }
}
