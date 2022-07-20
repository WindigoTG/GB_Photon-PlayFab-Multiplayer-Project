using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region Methods

    [SerializeField, Min(1.0f)] private float _maxHealth;
    [SerializeField] private Beams _beams;
    [SerializeField] private GameObject _beamsParent;

    [SerializeField] private string _damageKey = "Damage";
    [SerializeField] private string _maxHealthKey = "MaxHealth";

    [SerializeField] private PlayerUI _playerUiPrefab;

    private PlayerBehaviour _playerBehaviour;

    #endregion


    #region UnityMethods

    void Start()
    {
        _playerBehaviour = photonView.IsMine ? new LocalPlayerBehaviour() : new RemotePlayerBehaviour();
        _playerBehaviour.Init(_beams, _beamsParent);
        _playerBehaviour.SetHealth(_maxHealth, _maxHealth);

        if (photonView.IsMine)
            RetrieveCustomValues();

        PhotonNetwork.AddCallbackTarget(_beams);

        CameraWork _cameraWork = gameObject.GetComponent<CameraWork>();

        if (_cameraWork != null)
        {
            if (photonView.IsMine)
            {
                _cameraWork.OnStartFollowing();
            }
        }
        else
        {
            Debug.LogError("<Color=Red><b>Missing</b></Color> CameraWork Component on player Prefab.", this);
        }

        if (_playerUiPrefab != null)
        {
            var ui = Instantiate(_playerUiPrefab);
            ui.SetTarget(this, _playerBehaviour);
        }
        else
        {
            Debug.LogWarning("<Color=Red><b>Missing</b></Color> PlayerUiPrefab reference on player Prefab.", this);
        }
    }

    void Update()
    {
        _playerBehaviour?.Update();
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(_beams);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        var attack = other.GetComponent<IAttack>();

        if (attack == null)
            return;

        _playerBehaviour.TakeDamage(attack.Damage);
    }

    public void OnTriggerStay(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        var attack = other.GetComponent<IAttack>();

        if (attack == null)
            return;

        _playerBehaviour.TakeDamage(attack.Damage * Time.deltaTime);
    }

    #endregion


    #region IPunObservable

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        _playerBehaviour?.OnPhotonSerialize(stream);
    }

    #endregion


    #region Methods

    private void RetrieveCustomValues()
    {
        if (PlayFabDataHolder.Instance == null || string.IsNullOrEmpty(PlayFabDataHolder.Instance.PlayFabID))
            return;

        var playFabID = PlayFabDataHolder.Instance.PlayFabID;

        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = playFabID
        },
        result =>
        {
            if (result.Data == null)
                return;

            if (result.Data.ContainsKey(_damageKey))
                if (float.TryParse(result.Data[_damageKey].Value, out float damage))
                    _playerBehaviour.SetDamage(damage);

            if (result.Data.ContainsKey(_maxHealthKey))
                if (float.TryParse(result.Data[_maxHealthKey].Value, out float health))
                    _playerBehaviour.SetHealth(health, health);
            
        },
        error =>
        {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });
        
    }

    #endregion
}
