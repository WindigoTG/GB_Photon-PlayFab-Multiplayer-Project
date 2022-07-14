using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class InRoomPanel : BasePanel
{
    [SerializeField] private Button _leaveRoomButton;
    [SerializeField] private TMP_InputField _roomNameInput;
    [SerializeField] private Toggle _isOpenToggle;

    public Button LeaveRoomButton => _leaveRoomButton;
    public Toggle IsOpenToggle => _isOpenToggle;

    public void SetRoomName(string roomName)
    {
        _roomNameInput.text = roomName;
    }
}
