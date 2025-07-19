using UnityEngine;
using UnityEngine.SceneManagement;

public class MowerController : MonoBehaviour
{
    public static GameObject Instance;
    private lawnmower_runner runner;
    private Rigidbody rb;
    private ParticleSystem[] particleSystems;
    private bool particlesEnabled = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this.gameObject;
            DontDestroyOnLoad(this.gameObject);

            runner = GetComponent<lawnmower_runner>();
            rb = GetComponent<Rigidbody>();
            particleSystems = GetComponentsInChildren<ParticleSystem>();

            SceneManager.sceneLoaded += OnSceneLoaded;
            UpdateRunnerState(SceneManager.GetActiveScene().name);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        if (rb == null || particleSystems == null || particleSystems.Length == 0)
            return;

        bool shouldEnable = rb.velocity.magnitude > 2.5f;
        if (shouldEnable != particlesEnabled)
        {
            particlesEnabled = shouldEnable;
            foreach (var ps in particleSystems)
            {
                var emission = ps.emission;
                emission.enabled = particlesEnabled;
                if (particlesEnabled && !ps.isPlaying)
                    ps.Play();
                else if (!particlesEnabled && ps.isPlaying)
                    ps.Stop();
            }
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
            transform.eulerAngles = Vector3.zero;
        }
        else
        {
            rb.useGravity = false;
            transform.position = Vector3.zero;
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }
}
