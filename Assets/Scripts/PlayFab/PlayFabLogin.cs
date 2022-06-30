using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Text;

public class PlayFabLogin : MonoBehaviour
{
    #region Fields

    private static string TITLE_ID = "6E2E1";

    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private TextMeshProUGUI _infoText;
    [SerializeField] private Button _logInButton;
    [SerializeField] private Button _continueButton;

    #endregion


    #region UnityMethods

    private void Awake()
    {
        _continueButton.interactable = false;
        _logInButton.interactable = false;
        _nameInput.onValueChanged.AddListener(OnInputValueChange);
        _logInButton.onClick.AddListener(OnLogInButtonClick);
        _continueButton.onClick.AddListener(OnContinueButtonClick);
    }

     public void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = TITLE_ID;
        }
    }

    #endregion


    #region Methods

    private void OnLoginSuccess(LoginResult result)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append($"You are now logged in!\n");
        sb.Append($"Your id is {result.PlayFabId}\n");
        if (result.NewlyCreated)
            sb.Append("This is your fist time!");
        else
            sb.Append($"Last tame you've logged in at: {result.LastLoginTime}");

        Debug.Log(sb.ToString());
        _infoText.text = $"<color=#009900>{sb.ToString()}</color>";

        _logInButton.interactable = false;
        _nameInput.interactable = false;
        _continueButton.interactable = true;
    }

    private void OnLoginFailure(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError($"Something went wrong: {errorMessage}");
        _infoText.text = $"<color=#ff0000>Something went wrong: {errorMessage}</color>";
    }

    private void OnContinueButtonClick()
    {
        _continueButton.interactable = false;
        SceneManager.LoadScene(1);
    }

    private void OnLogInButtonClick()
    {
        var name = _nameInput.text.TrimStart().TrimEnd();
        if (string.IsNullOrWhiteSpace(name))
        {

        }
        else
        {
            var request = new LoginWithCustomIDRequest
            {
                CustomId = name,
                CreateAccount = true
            };
            PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
        }
    }

    private void ClearInfoMessage()
    {
        _infoText.text = "";
    }

    private void OnInputValueChange(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            _logInButton.interactable = false;
        else
            _logInButton.interactable = true;
        ClearInfoMessage();
    }

    #endregion
}

