using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MowerController : MonoBehaviour
{
    public static GameObject Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this.gameObject;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
