using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public abstract class CredentialsInputWindowBase
{
    #region Fields

    [SerializeField] protected RectTransform _windowPanel;
    [SerializeField] protected TMP_InputField _emailInput;
    [SerializeField] protected TMP_InputField _passwordInput;
    [SerializeField] protected Button _confirmButton;
    [SerializeField] protected Button _cancelButton;

    #endregion


    #region Properties

    public RectTransform WindowPanel => _windowPanel;
    public TMP_InputField EmailInput => _emailInput;
    public TMP_InputField PasswordInput => _passwordInput;
    public Button ConfirmButton => _confirmButton;
    public Button CancelButton => _cancelButton;

    #endregion


    #region Methods

    public virtual void ClearInputs()
    {
        _emailInput.text = "";
        _passwordInput.text = "";
    }

    #endregion
}
