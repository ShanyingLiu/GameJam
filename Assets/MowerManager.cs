using UnityEngine;
using UnityEngine.SceneManagement;

public class MowerController : MonoBehaviour
{
    public static GameObject Instance;
    private lawnmower_runner runner;
    private Rigidbody rb;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this.gameObject;
            DontDestroyOnLoad(this.gameObject);

            runner = GetComponent<lawnmower_runner>();
            rb = GetComponent<Rigidbody>();

            SceneManager.sceneLoaded += OnSceneLoaded;
            UpdateRunnerState(SceneManager.GetActiveScene().name);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateRunnerState(scene.name);
    }

    private void UpdateRunnerState(string sceneName)
    {
        bool isRunMower = sceneName == "RunMower";

        if (runner != null)
            runner.enabled = isRunMower;

        if (rb != null)
            rb.isKinematic = !isRunMower;
        if (isRunMower)
        {
            rb.useGravity = true;
            transform.position = Vector3.zero;
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            transform.position = Vector3.zero;
        }
    }
}