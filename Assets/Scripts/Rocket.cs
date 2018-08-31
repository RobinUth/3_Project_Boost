using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float LevelLoadDelay = 2f;
    bool debugColision = false;

    [SerializeField] AudioClip mainEngine, death, finish;

    [SerializeField] ParticleSystem mainEngineParticles, deathParticles, finishParticles;

    Rigidbody rigidbody;
    AudioSource audioSource;

    bool isTransitioning = false;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        if (!isTransitioning)
        {

            RespondToThrustInput();
            RespondToRotateInput();
        }
        if(Debug.isDebugBuild)
        {
            RespondToDebugKey();
        }
    }

    private void RespondToDebugKey()
    {
        if (Input.GetKey(KeyCode.C))
        {
            if (debugColision == true)
            {
                debugColision = false;
            }
            else
            {
                debugColision = true;
            }
        }
        if (Input.GetKey(KeyCode.L))
        {
            LoadNextScene();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(isTransitioning || debugColision == true)
        {
            return;
        }
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                Debug.Log("OK");
                break;

            case "Finish":
                StartSuccessSequence();
                break;

            default:

                StartDeathSequence();
                break;
        }

    }

    private void StartSuccessSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(finish);
        finishParticles.Play();
        Invoke("LoadNextScene", LevelLoadDelay);
    }

    private void StartDeathSequence()
    {
        print("hit something");
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("LoadFirstLevel", LevelLoadDelay);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        
        if(nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void RespondToThrustInput()
    {
        // float thrustThisFrame = mainThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            StopApplyingThrust();
        }

    }

    private void StopApplyingThrust()
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    private void ApplyThrust()
    {
        rigidbody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    private void RespondToRotateInput()
    {
        rigidbody.angularVelocity = Vector3.zero; // remove rotation due to physics

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }

        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

    }

}
