using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class JoinRoomPanel : BasePanel
{
    [SerializeField] private Button _joinSelectedButton;
    [SerializeField] private Button _joinByNameButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private TMP_InputField _input;
    [SerializeField] private RoomDisplayItemView _roomItemPrefab;
    [SerializeField] private RectTransform _roomDisplayParent;

    Dictionary<string, RoomDisplayItemView> _rooms = new Dictionary<string, RoomDisplayItemView>();

    private string _selectedRoomName;

    public string SelectedRoomName => _selectedRoomName;
    public Button JoinSelectedButton => _joinSelectedButton;
    public Button JoinByNameButton => _joinByNameButton;
    public Button BackButton => _backButton;
    public TMP_InputField Input => _input;

    public void AddRoom(string roomName)
    {
        var room = GameObject.Instantiate(_roomItemPrefab, _roomDisplayParent);
        room.SetNameAndOnClickCallback(roomName, OnRoomSelected);
        _rooms.Add(roomName, room);
    }

    private void OnRoomSelected(string roomName)
    {
        foreach (var kvp in _rooms)
            kvp.Value.SetSelected(false);

        _rooms[roomName].SetSelected(true);

        _selectedRoomName = roomName;
    }

    public void UpdateRoomIsOpen(string roomName, bool isOpen)
    {
        if (!_rooms.ContainsKey(roomName))
            return;

        _rooms[roomName].SetIsOpen(isOpen);
    }

    public void RemoveRoom(string roomName)
    {
        if (!_rooms.ContainsKey(roomName))
            return;

        if (!string.IsNullOrEmpty(_selectedRoomName) && _selectedRoomName.Equals(roomName))
            _selectedRoomName = null;

        GameObject.Destroy(_rooms[roomName].gameObject);
        _rooms.Remove(roomName);
    }
}
