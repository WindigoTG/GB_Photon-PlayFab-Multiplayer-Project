using Photon.Pun;
using UnityEngine;

public class Beams : MonoBehaviour, IPunObservable, IAttack
{
    #region Fields

    [SerializeField] float _damage = 0.1f;

    #endregion


    #region Properties

    public float Damage => _damage;

    #endregion


    #region IAttack

    public void SetDamage(float damage)
    {
        _damage = damage;
    }

    #endregion


    #region IPunObservable

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
            stream.SendNext(_damage);
        else
            _damage = (float)stream.ReceiveNext();
    }

    #endregion
}

public interface IAttack
{
    public float Damage { get; }
    public void SetDamage(float damage);
}
