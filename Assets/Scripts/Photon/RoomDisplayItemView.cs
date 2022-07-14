using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RoomDisplayItemView : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _background;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Toggle _isOpenToggle;

    private string _roomName;
    private Action<string> _onClickCallback;

    public void OnPointerClick(PointerEventData eventData)
    {
        _onClickCallback?.Invoke(_roomName);
    }

    public void SetNameAndOnClickCallback(string roomName, Action<string> onClickCallback)
    {
        _roomName = roomName;
        _onClickCallback = onClickCallback;
        _text.text = roomName;
    }

    public void SetSelected(bool isSelected)
    {
        _background.color = isSelected ? _selectedColor : _defaultColor;
    }

    public void SetIsOpen(bool isOpen)
    {
        _isOpenToggle.isOn = isOpen;
    }
}
