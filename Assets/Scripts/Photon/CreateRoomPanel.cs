using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[Serializable]
public class CreateRoomPanel : BasePanel
{
    [SerializeField] private Button _createRoom;
    [SerializeField] private Button _backButton;
    [SerializeField] private TMP_InputField _input;
    [SerializeField] private Toggle _isOpenToggle;
    [SerializeField] private Toggle _isVisibleToggle;

    public Button CreateRoom => _createRoom;
    public Button BackButton => _backButton;
    public TMP_InputField Input => _input;
    public Toggle IsOpenToggle => _isOpenToggle;
    public Toggle IsVisibleToggle => _isVisibleToggle;
}
