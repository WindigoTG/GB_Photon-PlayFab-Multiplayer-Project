using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private Vector3 _screenOffset = new Vector3(0f, 30f, 0f);

    [SerializeField]
    private Text _playerNameText;

    [Tooltip("UI Slider to display Player's Health")]
    [SerializeField]
    private Slider _playerHealthSlider;

    PlayerBehaviour _target;
	GameObject _followTarget;

    float _characterControllerHeight;

    Transform _targetTransform;

    Renderer _targetRenderer;

    CanvasGroup _canvasGroup;

    Vector3 _targetPosition;

	#endregion


	#region UnityMethods

	void Awake()
	{

		_canvasGroup = this.GetComponent<CanvasGroup>();

		this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
	}

	void Update()
	{
		if (_followTarget == null)
		{
			Destroy(this.gameObject);
			return;
		}

		if (_playerHealthSlider != null)
		{
			_playerHealthSlider.value = _target.Health;
		}
	}

	void LateUpdate()
	{

		if (_targetRenderer != null)
		{
			_canvasGroup.alpha = _targetRenderer.isVisible ? 1f : 0f;
		}

		if (_targetTransform != null)
		{
			_targetPosition = _targetTransform.position;
			_targetPosition.y += _characterControllerHeight;

			transform.position = Camera.main.WorldToScreenPoint(_targetPosition) + _screenOffset;
		}
	}

	#endregion


	#region Public Methods

	public void SetTarget(MonoBehaviourPunCallbacks followTarget, PlayerBehaviour healthTargettarget)
	{
		_target = healthTargettarget;
		_targetTransform = followTarget.GetComponent<Transform>();
		_targetRenderer = followTarget.GetComponentInChildren<Renderer>();
		_followTarget = followTarget.gameObject;

		CharacterController _characterController = followTarget.GetComponent<CharacterController>();

		if (_characterController != null)
		{
			_characterControllerHeight = _characterController.height;
		}

		if (_playerNameText != null)
		{
			_playerNameText.text = followTarget.photonView.Owner.NickName;
		}
	}

	#endregion
}
