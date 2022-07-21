using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class CharacterSelectionWidget : MonoBehaviour
{
    #region Fields

    [SerializeField] private RectTransform _existingCharacterTextsContainer;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _expText;
    [SerializeField] private TextMeshProUGUI _goldText;
    [Space]
    [SerializeField] private TextMeshProUGUI _newCharacterText;
    [Space]
    [SerializeField] private Button _button;
    [SerializeField] private Image _background;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _selectedColor;

    private Action<CharacterSelectionWidget> _onClick;

    #endregion


    #region UnityMethods

    private void Awake()
    {
        _button.onClick.AddListener(OnButtonClick);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }

    #endregion


    #region Methods

    private void OnButtonClick()
    {
        _onClick?.Invoke(this);
    }

    public void SubscribeOnclick(Action<CharacterSelectionWidget> callback)
    {
        _onClick += callback;
    }

    public void UnsubscribeOnclick(Action<CharacterSelectionWidget> callback)
    {
        _onClick -= callback;
    }

    public void SetNewCharacterDisplay()
    {
        _newCharacterText.gameObject.SetActive(true);
        _existingCharacterTextsContainer.gameObject.SetActive(false);
    }

    public void SetCharacterInfoDisplay(string name, string level, string exp, string gold)
    {
        _nameText.text = name;
        _levelText.text = level;
        _expText.text = exp;
        _goldText.text = gold;

        _newCharacterText.gameObject.SetActive(false);
        _existingCharacterTextsContainer.gameObject.SetActive(true);
    }

    public void SetSelected(bool isSelected)
    {
        var color = isSelected ? _selectedColor : _defaultColor;
        _background.color = color;
    }

    #endregion
}
