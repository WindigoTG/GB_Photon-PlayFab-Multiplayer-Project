using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class InRoomPanel : BasePanel
{
    [SerializeField] private Button _leaveRoomButton;

    public Button LeaveRoomButton => _leaveRoomButton;
}
