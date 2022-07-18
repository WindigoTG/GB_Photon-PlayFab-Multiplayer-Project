using UnityEngine;

public class PlayFabDataHolder : MonoBehaviour
{
    public static PlayFabDataHolder Instance;

    public string PlayFabID;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
            if (Instance != this)
            Destroy(gameObject);
    }
}
