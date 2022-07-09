using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Text;
using System;

public class PlayFabLogin : MonoBehaviour
{
    #region Fields

    private const string TITLE_ID = "6E2E1";

    private const string AUTHENTICATION_GUID_PREF = "AuthenticationGuid";
    private const string PERSISTENT_AUTHENTICATION_EMAIL = "PersistentAuthenticatonEmail";
    private const string PERSISTENT_AUTHENTICATION_PASSWORD = "PersistentAuthenticatonPassword";

    [SerializeField] private TextMeshProUGUI _infoText;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _resetAnonimousButton;
    [SerializeField] private Button _persistentLogInButton;
    [SerializeField] private Button _changeAccountButton;

    [SerializeField] private LoginInputWindow _loginWindow;
    [SerializeField] private SigninInputWindow _signinWindow;

    private string _password;
    private string _confirmPassword;
    private string _email;
    private string _userName;

    #endregion


    #region UnityMethods

    private void Awake()
    {
        _continueButton.onClick.AddListener(OnContinueButtonClick);
        _resetAnonimousButton.onClick.AddListener(OnResetAnonimousButtonClick);
        _persistentLogInButton.onClick.AddListener(OnPersistentLogInButtonClick);
        _changeAccountButton.onClick.AddListener(OnChangeAccountButtonClick);

        _changeAccountButton.gameObject.SetActive(false);

        _continueButton.interactable = false;
        _resetAnonimousButton.interactable = false;
        _persistentLogInButton.interactable = false;

        _loginWindow.WindowPanel.gameObject.SetActive(false);
        _signinWindow.WindowPanel.gameObject.SetActive(false);

        _loginWindow.CancelButton.onClick.AddListener(OnCancelLogInButtonClick);
        _signinWindow.CancelButton.onClick.AddListener(OnCancelSignInButtonClick);
        _loginWindow.ConfirmButton.onClick.AddListener(OnLogInButtonClick);
        _signinWindow.ConfirmButton.onClick.AddListener(OnSignInButtonClick);
        _loginWindow.NewAccButton.onClick.AddListener(OnCreateNewAccountButtonClick);

        _loginWindow.EmailInput.onValueChanged.AddListener(OnEmailInputValueChanged);
        _signinWindow.EmailInput.onValueChanged.AddListener(OnEmailInputValueChanged);
        _loginWindow.PasswordInput.onValueChanged.AddListener(OnPasswordInputValueChanged);
        _signinWindow.PasswordInput.onValueChanged.AddListener(OnPasswordInputValueChanged);
        _signinWindow.ConfirmPasswordInput.onValueChanged.AddListener(OnConfirmPasswordInputValueChanged);
        _signinWindow.UsernameInput.onValueChanged.AddListener(OnUserNameInputValueChanged);
    }

    public void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = TITLE_ID;
        }

        SilentLogIn();
    }

    public void OnDestroy()
    {
        _continueButton.onClick.RemoveAllListeners();
        _resetAnonimousButton.onClick.RemoveAllListeners();
        _persistentLogInButton.onClick.RemoveAllListeners();

        _loginWindow.CancelButton.onClick.RemoveAllListeners();
        _signinWindow.CancelButton.onClick.RemoveAllListeners();
        _loginWindow.ConfirmButton.onClick.RemoveAllListeners();
        _signinWindow.ConfirmButton.onClick.RemoveAllListeners();
        _loginWindow.NewAccButton.onClick.RemoveAllListeners();

        _loginWindow.EmailInput.onValueChanged.RemoveAllListeners();
        _signinWindow.EmailInput.onValueChanged.RemoveAllListeners();
        _loginWindow.PasswordInput.onValueChanged.RemoveAllListeners();
        _signinWindow.PasswordInput.onValueChanged.RemoveAllListeners();
        _signinWindow.ConfirmPasswordInput.onValueChanged.RemoveAllListeners();
        _signinWindow.UsernameInput.onValueChanged.RemoveAllListeners();
    }

    #endregion


    #region Methods

    private void SilentLogIn()
    {
        if (PlayerPrefs.HasKey(PERSISTENT_AUTHENTICATION_EMAIL) && PlayerPrefs.HasKey(PERSISTENT_AUTHENTICATION_PASSWORD))
        {
            _email = PlayerPrefs.GetString(PERSISTENT_AUTHENTICATION_EMAIL);
            _password = PlayerPrefs.GetString(PERSISTENT_AUTHENTICATION_PASSWORD);

            PlayFabClientAPI.LoginWithEmailAddress(new LoginWithEmailAddressRequest
            {
                Email = _email,
                Password = _password,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams { GetUserAccountInfo = true }
            },
        OnPlayfabLoginSuccess,
        error =>  AnonimousLogin());
        }
        else
            AnonimousLogin();
    }

    private void AnonimousLogin()
    {
        _infoText.text = "Connecting...";
        _continueButton.interactable = false;
        _resetAnonimousButton.interactable = false;
        _persistentLogInButton.interactable = false;

        var isAccountCreationNeeded = !PlayerPrefs.HasKey(AUTHENTICATION_GUID_PREF);
        var userId = PlayerPrefs.GetString(AUTHENTICATION_GUID_PREF, Guid.NewGuid().ToString());

        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest
        {
            CustomId = userId,
            CreateAccount = isAccountCreationNeeded
        },
        success =>
        {
            PlayerPrefs.SetString(AUTHENTICATION_GUID_PREF, userId);
            OnAnonimousLoginSuccess(success);
        },
        OnLoginFailure);
    }

    private void OnAnonimousLoginSuccess(LoginResult result)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append($"You are now logged in!\n");
        sb.Append($"Your id is {result.PlayFabId}\n");
        if (result.NewlyCreated)
            sb.Append("This is your fist time!");
        else
            sb.Append($"Last time you've logged in at: {result.LastLoginTime}");

        Debug.Log(sb.ToString());
        _infoText.text = $"<color=#009900>{sb.ToString()}</color>";

        _continueButton.interactable = true;
        _resetAnonimousButton.interactable = true;
        _persistentLogInButton.interactable = true;
    }

    private void OnLoginFailure(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError($"Something went wrong: {errorMessage}");
        _infoText.text = $"<color=#ff0000>Something went wrong: {errorMessage}</color>";

        _signinWindow.CancelButton.interactable = true;
        _loginWindow.CancelButton.interactable = true;
        _signinWindow.ConfirmButton.interactable = true;
        _loginWindow.ConfirmButton.interactable = true;
    }

    private void OnResetAnonimousButtonClick()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        PlayerPrefs.DeleteKey(AUTHENTICATION_GUID_PREF);
        _continueButton.interactable = false;
        _resetAnonimousButton.interactable = false;
        AnonimousLogin();
    }

    private void OnContinueButtonClick()
    {
        _continueButton.interactable = false;
        SceneManager.LoadScene(1);
    }

    private void OnPersistentLogInButtonClick()
    {
        ClearInfoMessage();

        _loginWindow.WindowPanel.gameObject.SetActive(true);
        _loginWindow.ClearInputs();
    }

     private void ClearInfoMessage()
    {
        _infoText.text = "";
    }

    private void OnCancelLogInButtonClick()
    {
        ClearInfoMessage();

        _loginWindow.WindowPanel.gameObject.SetActive(false);

        if (!PlayFabClientAPI.IsClientLoggedIn())
            AnonimousLogin();
    }

    private void OnLogInButtonClick()
    {
        PlayFabClientAPI.LoginWithEmailAddress(new LoginWithEmailAddressRequest
        {
            Email = _email,
            Password = _password, 
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams { GetUserAccountInfo = true}
        },
        OnPlayfabLoginSuccess,
        OnLoginFailure);

        _infoText.text = "Logging in...";

        _loginWindow.ConfirmButton.interactable = false;
    }

    private void OnPlayfabLoginSuccess(LoginResult result)
    {
        _infoText.text = $"<color=#009900>Welcome back, {result.InfoResultPayload.AccountInfo.Username}!</color>";

        _continueButton.interactable = true;

        _loginWindow.WindowPanel.gameObject.SetActive(false);
        _signinWindow.WindowPanel.gameObject.SetActive(false);
        _resetAnonimousButton.interactable = false;

        _changeAccountButton.gameObject.SetActive(true);

        PlayerPrefs.SetString(PERSISTENT_AUTHENTICATION_EMAIL, _email);
        PlayerPrefs.SetString(PERSISTENT_AUTHENTICATION_PASSWORD, _password);
    }

    private void OnSignInButtonClick()
    {
        if (!_password.Equals(_confirmPassword))
        {
            _infoText.text = $"<color=#ff9900>Passwords don't match</color>";
            return;
        }

        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest
        {
            Email = _email,
            Username = _userName,
            Password = _password,
            RequireBothUsernameAndEmail = true
        },
        OnPlayfabSignInSuccess,
        OnLoginFailure);

        _infoText.text = "Creating account...";

        _signinWindow.ConfirmButton.interactable = false;
    }

    private void OnPlayfabSignInSuccess(RegisterPlayFabUserResult result)
    {
        _infoText.text = $"<color=#009900>Welcome, {result.Username}!</color>";

        _continueButton.interactable = true;

        _loginWindow.WindowPanel.gameObject.SetActive(false);
        _signinWindow.WindowPanel.gameObject.SetActive(false);
        _resetAnonimousButton.interactable = false;
        _changeAccountButton.gameObject.SetActive(true);

        PlayerPrefs.SetString(PERSISTENT_AUTHENTICATION_EMAIL, _email);
        PlayerPrefs.SetString(PERSISTENT_AUTHENTICATION_PASSWORD, _password);
    }


    private void OnCancelSignInButtonClick()
    {
        ClearInfoMessage();
        _signinWindow.WindowPanel.gameObject.SetActive(false);
        _loginWindow.WindowPanel.gameObject.SetActive(true);
        _loginWindow.ClearInputs();
    }

    private void OnCreateNewAccountButtonClick()
    {
        ClearInfoMessage();
        _signinWindow.WindowPanel.gameObject.SetActive(true);
        _loginWindow.WindowPanel.gameObject.SetActive(false);
        _signinWindow.ClearInputs();
    }

    private void OnChangeAccountButtonClick()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        PlayerPrefs.DeleteKey(PERSISTENT_AUTHENTICATION_EMAIL);
        PlayerPrefs.DeleteKey(PERSISTENT_AUTHENTICATION_PASSWORD);
        OnPersistentLogInButtonClick();
        _changeAccountButton.gameObject.SetActive(false);
    }

    private void OnEmailInputValueChanged(string value)
    {
        _email = value.Trim();

        SetConfirmButtonsInteractable();
    }

    private void OnUserNameInputValueChanged(string value)
    {
        _userName = value.Trim();

        SetConfirmButtonsInteractable();
    }

    private void OnPasswordInputValueChanged(string value)
    {
        _password = value.Trim();

        SetConfirmButtonsInteractable();
    }

    private void OnConfirmPasswordInputValueChanged(string value)
    {
        _confirmPassword = value.Trim();

        SetConfirmButtonsInteractable();
    }

    private void SetConfirmButtonsInteractable()
    {
        _loginWindow.ConfirmButton.interactable = !(string.IsNullOrEmpty(_email) || string.IsNullOrEmpty(_password));
        _signinWindow.ConfirmButton.interactable = !(string.IsNullOrEmpty(_email) || string.IsNullOrEmpty(_password) ||
            string.IsNullOrEmpty(_userName) || string.IsNullOrEmpty(_confirmPassword));

        ClearInfoMessage();
    }

    #endregion
}

