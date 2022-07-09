using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab.ClientModels;
using PlayFab;
using System;

public class CatalogInfoManager : MonoBehaviour
{
    #region

    [SerializeField] private Button _retrieveCatalogButton;
    [SerializeField] private TextMeshProUGUI _messageText;
    [Space]
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemTypeText;
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;
    [SerializeField] private RectTransform _catalogDisplayItemsParent;
    [SerializeField] private CatalogDisplayItem _catalogDisplayItemPrefab;
    [SerializeField] private RectTransform _catalogInfoGroup;

    private readonly Dictionary<string, CatalogItem> _catalogItemsByID = new Dictionary<string, CatalogItem>();
    private readonly Dictionary<CatalogDisplayItem, string> _itemIDsByDisplayItem = new Dictionary<CatalogDisplayItem, string>();

    private readonly List<CatalogDisplayItem> _catalogDisplayItems = new List<CatalogDisplayItem>();

    #endregion


    #region UnityMethods

    private void Awake()
    {
        _retrieveCatalogButton.onClick.AddListener(OnRetrieveCatalogButtonClick);
    }

    private void Start()
    {
        _catalogInfoGroup.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _retrieveCatalogButton.onClick.RemoveAllListeners();
        ClearCatalog();
    }

    #endregion


    #region Methods

    private void OnRetrieveCatalogButtonClick()
    {
        _messageText.text = "Retrieving catalog...";
        _retrieveCatalogButton.interactable = false;

        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), OnGetCatalogSuccess, OnFailure);

    }

    private void OnFailure(PlayFabError error)
    {
        _messageText.text = error.GenerateErrorReport();
        _retrieveCatalogButton.interactable = true;
    }

    private void OnGetCatalogSuccess(GetCatalogItemsResult result)
    {
        ClearCatalog();

        foreach (var item in result.Catalog)
        {
            _catalogItemsByID.Add(item.ItemId, item);

            var displayItem = Instantiate(_catalogDisplayItemPrefab, _catalogDisplayItemsParent);
            displayItem.SubscribeOnClick(OnItemSelected);
            displayItem.SetText(item.DisplayName);
            _itemIDsByDisplayItem.Add(displayItem, item.ItemId);
            _catalogDisplayItems.Add(displayItem);
            displayItem.SetSelected(false);
        }


        _retrieveCatalogButton.gameObject.SetActive(false);
        _messageText.gameObject.SetActive(false);
        _catalogInfoGroup.gameObject.SetActive(true);
    }

    private void OnItemSelected(CatalogDisplayItem selectedItem)
    {
        foreach (var item in _catalogDisplayItems)
            item.SetSelected(false);

        if (!_itemIDsByDisplayItem.ContainsKey(selectedItem))
        {
            FillItemInfo("");
            return;
        }

        var itemId = _itemIDsByDisplayItem[selectedItem];

        if (!_catalogItemsByID.ContainsKey(itemId))
        {
            FillItemInfo("");
            return;
        }

        FillItemInfo(itemId);
        selectedItem.SetSelected(true);
    }

    private void FillItemInfo(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
            FillEmpty();

        var item = _catalogItemsByID[itemID];

        if (item == null)
            FillEmpty();

        _itemNameText.text = item.DisplayName;
        _itemDescriptionText.text = item.Description;

        if (item.Bundle != null)
            _itemTypeText.text = "Is: bundle";
        else if (item.Container != null)
            _itemTypeText.text = "Is: container";
        else
            _itemTypeText.text = "Is: item";

        void FillEmpty()
        {
            _itemNameText.text = "";
            _itemTypeText.text = "";
            _itemDescriptionText.text = "";
        }
    }

    private void ClearCatalog()
    {
        foreach (var item in _catalogDisplayItems)
        {
            item.UnsubscribeOnClick(OnItemSelected);
            Destroy(item.gameObject);
        }

        _catalogDisplayItems.Clear();
        _itemIDsByDisplayItem.Clear();
        _catalogItemsByID.Clear();
    }

    #endregion
}
