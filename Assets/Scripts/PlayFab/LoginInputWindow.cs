using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class LoginInputWindow : CredentialsInputWindowBase
{
    #region Fields

    [SerializeField] protected Button _newAccButton;

    #endregion


    #region Properties
    public Button NewAccButton => _newAccButton;

    #endregion
}
