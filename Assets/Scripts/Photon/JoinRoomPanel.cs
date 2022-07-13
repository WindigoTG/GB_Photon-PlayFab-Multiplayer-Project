using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class JoinRoomPanel : BasePanel
{
    [SerializeField] private Button _joinButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private RoomDisplayItemView _roomItemPrefab;
    [SerializeField] private RectTransform _roomDisplayParent;

    Dictionary<string, RoomDisplayItemView> _rooms = new Dictionary<string, RoomDisplayItemView>();

    private string _selectedRoomName;

    public string SelectedRoomName => _selectedRoomName;

    public Button JoinButton => _joinButton;
    public Button BackButton => _backButton;

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

    public void RemoveRoom(string roomName)
    {
        if (!_rooms.ContainsKey(roomName))
            return;

        GameObject.Destroy(_rooms[roomName].gameObject);
        _rooms.Remove(roomName);
    }
}
