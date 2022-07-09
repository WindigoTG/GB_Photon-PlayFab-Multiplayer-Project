using System;
using UnityEngine;
using TMPro;

[Serializable]
public class SigninInputWindow : CredentialsInputWindowBase
{
    #region Fields

    [SerializeField] protected TMP_InputField _confirmPasswordInput;
    [SerializeField] protected TMP_InputField _usernameInput;

    #endregion


    #region Properties

    public TMP_InputField ConfirmPasswordInput => _confirmPasswordInput;
    public TMP_InputField UsernameInput => _usernameInput;

    #endregion


    #region Methods

    public override void ClearInputs()
    {
        base.ClearInputs();
        _confirmPasswordInput.text = "";
        _usernameInput.text = "";
    }

    #endregion
}
