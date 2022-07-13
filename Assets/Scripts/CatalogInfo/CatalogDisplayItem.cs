using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using System;

public class CatalogDisplayItem : MonoBehaviour, IPointerClickHandler
{
    #region Fields

    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] Image _background;

    private Action<CatalogDisplayItem> _onClickCallback;

    #endregion


    #region Methods

    public void OnPointerClick(PointerEventData eventData)
    {
        _onClickCallback?.Invoke(this);
    }

    public void SetText(string text)
    {
        _text.text = text;
    }

    public void SubscribeOnClick(Action<CatalogDisplayItem> callback)
    {
        _onClickCallback += callback;
    }

    public void UnsubscribeOnClick(Action<CatalogDisplayItem> callback)
    {
        _onClickCallback -= callback;
    }

    public void SetSelected(bool isSelected)
    {
        var color = _background.color;
        color.a = isSelected ? 1 : 0;
        _background.color = color;
    }

    #endregion
}
