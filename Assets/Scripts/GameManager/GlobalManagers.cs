using GNW.GameManager;
using UnityEngine;

public class GlobalManagers : MonoBehaviour
{

    [field: SerializeField] public GameManager GameManager { get; private set; }
    
    #region Instance

    private static GlobalManagers _instance;
    public static GlobalManagers Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    #endregion
}
