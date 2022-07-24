using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class CharacterManager : MonoBehaviour
{
    #region Fields

    [SerializeField] private CharacterSelectionWidget _characterWidgetPrefab;
    [SerializeField] private RectTransform _characterWidgetParent;
    [SerializeField] private RectTransform _characterSelectionPanel;
    [Space]
    [SerializeField] private RectTransform _characterCreationPanel;
    [SerializeField] private TMP_InputField _characterNameInput;
    [SerializeField] private Button _createCharacterButton;
    [SerializeField] private Button _cancelButton;
    [Space]
    [SerializeField] private Button _mocBattleButton;
    [SerializeField] private int _winGoldAmount = 100;
    [SerializeField] private int _loseGoldAmount = 50;
    [SerializeField] private int _winXPAmount = 10;

    private List<CharacterResult> _characters;
    private List<CharacterSelectionWidget> _characterWidgets;
    private Dictionary<string, string> _characterIDsByName;
    private Dictionary<string, Dictionary<string, int>> _characterStatisticsById;
    private Dictionary<CharacterSelectionWidget, string> _charactersByWidget;

    private string _selectedCharacter;
    private CharacterSelectionWidget _selectedCharacterWidget;

    private readonly string LEVEL_KEY = "Level";
    private readonly string EXP_KEY = "XP";
    private readonly string GOLD_KEY = "Gold";

    #endregion


    #region UnityMethods

    private void Start()
    {
        _characterNameInput.onValueChanged.AddListener(value => _createCharacterButton.interactable = !string.IsNullOrWhiteSpace(value));
        _createCharacterButton.onClick.AddListener(CreateCharacter);
        _cancelButton.onClick.AddListener(ShowCharacterSelectionPanel);

        _characterCreationPanel.gameObject.SetActive(false);
        _characterSelectionPanel.gameObject.SetActive(false);

        _mocBattleButton.gameObject.SetActive(false);
        _mocBattleButton.onClick.AddListener(SimulateBattle);

        RetrieveCharacterList();
    }

    private void OnDestroy()
    {
        _characterNameInput.onValueChanged.RemoveAllListeners();
        _createCharacterButton.onClick.RemoveAllListeners();
        _cancelButton.onClick.RemoveAllListeners();
        _mocBattleButton.onClick.RemoveAllListeners();
    }

    #endregion


    #region Methods

    private void CreateCharacter()
    {
        var name = _characterNameInput.text.Trim();

        if (_characterIDsByName.ContainsKey(name))
        {
            Debug.Log("Character with that name already exists!");
            return;
        }

        PlayFabClientAPI.GrantCharacterToUser(new GrantCharacterToUserRequest { 
            CharacterName = name,
            ItemId = "wooden_sword"
        },
            result =>
            {
                _characterIDsByName.Add(name, result.CharacterId);
                CreateNewStatisticsForCharacter(name);
            },
            error => { Debug.Log($"Couldn't create character\nReason: {error.GenerateErrorReport()}"); });
    }

    private void CreateNewStatisticsForCharacter(string characterName)
    {
        var characterId = _characterIDsByName[characterName];

        var newStatistics = new Dictionary<string, int>
        {
            {LEVEL_KEY, 1},
            {EXP_KEY, 0},
            {GOLD_KEY, 500}
        };

        _characterStatisticsById.Add(characterId, newStatistics);

        PlayFabClientAPI.UpdateCharacterStatistics(new UpdateCharacterStatisticsRequest
        {
            CharacterId = characterId,
            CharacterStatistics = newStatistics
        },
        result =>
        {
            CreateNewCharacterWidget(characterName, characterId);
            CreateEmptyWidget();
            ShowCharacterSelectionPanel();
        },
        error =>
        {
            Debug.Log($"Couldn't update character statistic\nReason: {error.GenerateErrorReport()}");
        });
    }

    private void CreateNewCharacterWidget(string characterName, string characterId)
    {
        var widget = _selectedCharacterWidget;
        widget.UnsubscribeOnclick(OnCreateNewCharacterClicked);
        var statistic = _characterStatisticsById[characterId];

        widget.SetCharacterInfoDisplay(characterName, 
                    statistic[LEVEL_KEY].ToString(),
                    statistic[EXP_KEY].ToString(),
                    statistic[GOLD_KEY].ToString());

        _characterWidgets.Add(widget);
        _charactersByWidget.Add(widget, characterName);

        widget.SubscribeOnclick(OnSelectCharacterClicked);
    }

    private void OnSelectCharacterClicked(CharacterSelectionWidget characterWidget)
    {
        foreach (var widget in _characterWidgets)
            widget.SetSelected(false);

        characterWidget.SetSelected(true);

        _selectedCharacter = _charactersByWidget[characterWidget];
        _mocBattleButton.gameObject.SetActive(true);

        _selectedCharacterWidget = characterWidget;
    }

    private void OnCreateNewCharacterClicked(CharacterSelectionWidget characterWidget)
    {
        _selectedCharacterWidget = characterWidget;
        ShowCharacterCreationPanel();
    }

    private void ShowCharacterSelectionPanel()
    {
        _characterCreationPanel.gameObject.SetActive(false);
        _characterSelectionPanel.gameObject.SetActive(true);
    }

    private void ShowCharacterCreationPanel()
    {
        _characterCreationPanel.gameObject.SetActive(true);
        _characterSelectionPanel.gameObject.SetActive(false);

        _characterNameInput.text = "";
    }

    private void RetrieveCharacterList()
    {
        PlayFabClientAPI.GetAllUsersCharacters(new ListUsersCharactersRequest(),
            OnCharacterListRetrieved,
            error => { Debug.Log(error.GenerateErrorReport()); });
    }

    private void OnCharacterListRetrieved(ListUsersCharactersResult result)
    {
        _characters = result.Characters;

        CreateCharacterWidgets();
    }

    private void CreateCharacterWidgets()
    {
        _characterWidgets = new List<CharacterSelectionWidget>();
        _characterStatisticsById = new Dictionary<string, Dictionary<string, int>>();
        _charactersByWidget = new Dictionary<CharacterSelectionWidget, string>();
        _characterIDsByName = new Dictionary<string, string>();

        foreach (var character in _characters)
        {
            _characterIDsByName.Add(character.CharacterName, character.CharacterId);

            var widget = Instantiate(_characterWidgetPrefab, _characterWidgetParent);
            widget.SetCharacterInfoDisplay(character.CharacterName, "...", "...", "...");

            PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest { CharacterId = character.CharacterId },
                result =>
                {
                    widget.SetCharacterInfoDisplay(character.CharacterName,
                        result.CharacterStatistics[LEVEL_KEY].ToString(),
                        result.CharacterStatistics[EXP_KEY].ToString(),
                        result.CharacterStatistics[GOLD_KEY].ToString());

                        _characterStatisticsById.Add(character.CharacterId, result.CharacterStatistics);
                },
                error => { Debug.Log($"Unable to retrieve character statistics for {character.CharacterName}\nReason: {error.GenerateErrorReport()}"); });

            _characterWidgets.Add(widget);
            _charactersByWidget.Add(widget, character.CharacterName);

            widget.SubscribeOnclick(OnSelectCharacterClicked);
        }

        CreateEmptyWidget();
        ShowCharacterSelectionPanel();
    }

    private void CreateEmptyWidget()
    {
        var emptyWidget = Instantiate(_characterWidgetPrefab, _characterWidgetParent);
        emptyWidget.SubscribeOnclick(OnCreateNewCharacterClicked);
        emptyWidget.SetNewCharacterDisplay();
        _characterWidgets.Add(emptyWidget);
    }

    private void SimulateBattle()
    {
        if (string.IsNullOrEmpty(_selectedCharacter))
            return;

        var characterId = _characterIDsByName[_selectedCharacter];

        var statistics = _characterStatisticsById[characterId];

        var gold = statistics[GOLD_KEY];
        if (gold < _loseGoldAmount)
        {
            Debug.Log("You don't have enough gold to play");
            return;
        }

        var level = statistics[LEVEL_KEY];
        var exp = statistics[EXP_KEY];

        var randomChance = UnityEngine.Random.Range(0.0f, 100.0f);

        if (randomChance < 50.0f)
        {
            Debug.Log("You lose!");

            gold -= _loseGoldAmount;
        }
        else
        {
            Debug.Log("You win!");

            gold += _winGoldAmount;
            exp += _winXPAmount;

            if (exp >= (level * level) * 10 + (level * 5))
                level++;
        }

        statistics[GOLD_KEY] = gold;
        statistics[LEVEL_KEY] = level;
        statistics[EXP_KEY] = exp;

        UpdateExistingCharacterStatistics(characterId);
    }

    private void UpdateExistingCharacterStatistics(string characterId)
    {
        if (!_characterStatisticsById.ContainsKey(characterId))
            return;

        var statistics = _characterStatisticsById[characterId];

        PlayFabClientAPI.UpdateCharacterStatistics(new UpdateCharacterStatisticsRequest
        {
            CharacterId = characterId,
            CharacterStatistics = statistics
        },
        result =>
        {
            Debug.Log($"Character statistics updated");
        },
        error =>
        {
            Debug.Log($"Couldn't update character statistic\nReason: {error.GenerateErrorReport()}");
        });

        _selectedCharacterWidget.SetCharacterInfoDisplay(
            _charactersByWidget[_selectedCharacterWidget],
            statistics[LEVEL_KEY].ToString(),
            statistics[EXP_KEY].ToString(),
            statistics[GOLD_KEY].ToString()
            );
    }

    #endregion
}
