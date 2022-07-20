using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour
{
    #region Fields

    private Beams _beams;
    private GameObject _beamsParent;

    protected float _maxHealth;
    protected float _health;
    protected bool _isFiring;

    #endregion


    #region Properties

    public float Health => _health;

    #endregion


    #region Methods

    public virtual void Init(Beams beams, GameObject beamsParent)
    {
        if (beamsParent == null || beams == null)
        {
            Debug.LogError("<Color=Red><b>Missing</b></Color> Beams Reference.");
        }
        else
        {
            _beams = beams;
            _beamsParent = beamsParent;
            _beamsParent.SetActive(false);
        }
    }

    

    public virtual void Update() 
    {
        if (_beamsParent != null && _isFiring != _beamsParent.activeInHierarchy)
        {
            _beamsParent.SetActive(_isFiring);
        }
    }

    public virtual void SetDamage(float damage)
    {
        if (_beams != null)
            _beams.SetDamage(damage);
    }

    public virtual void SetHealth(float maxHealth, float currentHealth)
    {
        SetMaxHealth(maxHealth);
        SetHealth(currentHealth);
    }

    public virtual void SetHealth(float health)
    {
        _health = health;
    }

    public virtual void SetMaxHealth(float maxHealth)
    {
        _maxHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        _health -= damage;

        if (_health <= 0)
            PhotonNetwork.LeaveRoom();
    }

    public virtual void OnPhotonSerialize(PhotonStream stream)
    {

    }


    #endregion


    #region IPunObservable

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Debug.Log(stream);

        if (stream.IsWriting)
        {
            stream.SendNext(_isFiring);
            stream.SendNext(_health);
        }
        else
        {
            _isFiring = (bool)stream.ReceiveNext();
            _health = (float)stream.ReceiveNext();
        }
    }

    #endregion
}


public class LocalPlayerBehaviour : PlayerBehaviour
{
    private List<ItemInstance> _inventory;

    public LocalPlayerBehaviour()
    {
        RetrieveUserInventory();
    }
     
    public override void Update()
    {
        if (Input.GetButtonDown("Fire1"))
            if (!_isFiring)
                _isFiring = true;

        if (Input.GetButtonUp("Fire1"))
            if (_isFiring)
                _isFiring = false;

        if (Input.GetKeyDown(KeyCode.H))
            UseHealingPotion();

        base.Update();
    }

    private void UseHealingPotion()
    {
        if (_inventory == null)
            return;

        var healthPotion = _inventory.Find(x => x.ItemId.Equals("health_potion"));

        if (healthPotion == null)
            return;

        PlayFabClientAPI.ConsumeItem(new ConsumeItemRequest
        {
            ConsumeCount = 1,
            ItemInstanceId = healthPotion.ItemInstanceId
        },
        success => 
        {
            Debug.Log("Healing potion used successfully");
            _health = _maxHealth;
        },
        error =>
        {
            Debug.Log("Couldn't use item.\nReason: " + error.GenerateErrorReport());

            if (error.Error == PlayFabErrorCode.ItemNotFound || error.Error == PlayFabErrorCode.NoRemainingUses)
                PurchaseHealthPotion();
        });
    }

    private void PurchaseHealthPotion()
    {
        PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
        {
            CatalogVersion = "FirstCatalog",
            ItemId = "health_potion",
            Price = 5,
            VirtualCurrency = "GP"
        }, 
        success =>
        {
            Debug.Log("Healing potion purchased successfully");
            UseHealingPotion();
        },
        error =>
        {
            Debug.Log("Couldn't purchase item.\nReason: " + error.GenerateErrorReport());
        });
    }

    private void RetrieveUserInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            success =>
            {
                Debug.Log("Inventory retrieved successfully");
                _inventory = success.Inventory;
            },
            error =>
            {
                Debug.Log("Couldn't retrieve inventory.\nReason: " + error.GenerateErrorReport());
            });
    }

    public override void OnPhotonSerialize(PhotonStream stream)
    {
        base.OnPhotonSerialize(stream);

        stream.SendNext(_isFiring);
        stream.SendNext(_health);
    }
}

public class RemotePlayerBehaviour : PlayerBehaviour
{
    public override void OnPhotonSerialize(PhotonStream stream)
    {
        base.OnPhotonSerialize(stream);

        _isFiring = (bool)stream.ReceiveNext();
        _health = (float)stream.ReceiveNext();
    }
}

