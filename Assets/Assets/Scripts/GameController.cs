using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("Game State")]
    public bool skipIntroForTesting = false;

    [Header("Object References")]
    [SerializeField] private PlayerControl playerControl;
    [SerializeField] private ObjectFallController objectFallController;
    [SerializeField] private CanonController cannonController;
    [SerializeField] private GameObject AmbushedPanel;
    [SerializeField] private TMP_Text LivesText;
    [SerializeField] private Canvas GameOverCanvas;
    [SerializeField] private TMP_Text TimerText;

    [Header("Intro Sequence Settings")]
    [SerializeField] private Vector3 playerStartPosition = new Vector3(-12, -5.13f, 0);
    [SerializeField] private Vector3 playerEndPosition = new Vector3(-7, -5.13f, 0);
    [SerializeField] private float walkInSpeed = 3f;

    [Header("Object References")]
    [SerializeField] private Canvas PauseMenuCanvas;

    [Header("Object References")]
    [SerializeField] private DifficultyController difficultyController;

    private Animator playerAnimator;
    private Rigidbody2D playerRb;
    private bool isPaused = false;

    void Awake()
    {
        if (playerControl != null)
        {
            playerControl.OnPlayerDeath += WhenPlayerDies;
            playerControl.OnHealthChanged += UpdateLivesUI;
            difficultyController.OnDifficultyChanged += HandleDifficultyChange;
        }

        GameOverCanvas.gameObject.SetActive(false);
        AmbushedPanel.SetActive(false);

        playerAnimator = playerControl.GetComponent<Animator>();
        playerRb = playerControl.GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // We set the time scale to 1 here to ensure it's normal when the game/scene starts.
        Time.timeScale = 1f;

        if (skipIntroForTesting)
        {
            StartGameplay();
        }
        else
        {
            StartCoroutine(IntroSequence());
        }
    }

    void Update()
    {
        // Listen for the Escape key to toggle the pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void HandleDifficultyChange(int difficultyLevel)
    {
        float newSpawnRate;

        // Use a switch statement to translate the difficulty level to a spawn rate
        switch (difficultyLevel)
        {
            case 1: // Easy
                newSpawnRate = 2f;
                break;
            case 2: // Medium
                newSpawnRate = 1f;
                break;
            case 3: // Hard
                newSpawnRate = 0.5f;
                break;
            default: // A fallback just in case
                newSpawnRate = 2f;
                break;
        }

        // Tell the object fall controller to use the new rate
        if (objectFallController != null)
        {
            objectFallController.SetMissileSpawnRate(newSpawnRate);
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            // Pause the game
            Time.timeScale = 0f; // This freezes all physics and time-based movement
            PauseMenuCanvas.gameObject.SetActive(true);
        }
        else
        {
            // Unpause the game
            Time.timeScale = 1f; // This resumes normal time
            PauseMenuCanvas.gameObject.SetActive(false);
        }
    }

    public void OnBackButton()
    {
        // Important: Always unpause before changing scenes
        Time.timeScale = 1f;
        // Assuming your start screen is the first scene in the build order (index 0)
        SceneManager.LoadScene(0);
    }

    public void OnNewGameButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnQuitButton()
    {
        Debug.Log("Quitting Game..."); // This message shows in the editor
        Application.Quit(); // This only works in a built version of the game
    }

    private IEnumerator IntroSequence()
    {
        playerControl.SetMovement(false);
        playerRb.bodyType = RigidbodyType2D.Kinematic;
        playerControl.transform.position = playerStartPosition;

        playerAnimator.SetBool("IsGrounded", true);
        playerAnimator.SetFloat("MoveSpeed", 1f);

        while (playerControl.transform.position.x < playerEndPosition.x)
        {
            playerControl.transform.position = Vector3.MoveTowards(playerControl.transform.position, playerEndPosition, walkInSpeed * Time.deltaTime);
            yield return null;
        }

        playerControl.transform.position = playerEndPosition;
        playerAnimator.SetFloat("MoveSpeed", 0f);

        AmbushedPanel.SetActive(true);
        yield return new WaitForSeconds(3f);
        AmbushedPanel.SetActive(false);

        StartGameplay();
    }

    void StartGameplay()
    {
        playerControl.SetMovement(true);
        playerRb.bodyType = RigidbodyType2D.Dynamic;
        objectFallController.StartSpawning();
        cannonController.StartShooting();
    }

    // --- THIS IS THE CORRECTED METHOD ---
    void WhenPlayerDies()
    {
        // If the game over screen is already showing, do nothing.
        if (GameOverCanvas.gameObject.activeSelf) return;

        // Show the game over screen.
        GameOverCanvas.gameObject.SetActive(true);

        // Set the final time on the score text.
        TimerText.text = "You Lasted: " + Mathf.Round(Time.timeSinceLevelLoad) + "s";

        // Stop all game action by pausing the physics and game time.
        Time.timeScale = 0f;
    }

    // --- THIS IS THE CORRECTED METHOD ---
    void UpdateLivesUI(int currentLives)
    {
        if (LivesText != null)
        {
            LivesText.text = "Lives: " + currentLives;
        }
    }

    public void RetryClicked()
    {
        // The scene is reloaded, and Start() will reset Time.timeScale to 1.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        if (difficultyController != null)
        {
            difficultyController.OnDifficultyChanged -= HandleDifficultyChange;
        }

        if (playerControl != null)
        {
            playerControl.OnPlayerDeath -= WhenPlayerDies;
            playerControl.OnHealthChanged -= UpdateLivesUI;
        }
    }
}