using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Player")]
    public Damageable player;

    [Header("Tutorial")]
    public bool showTutorialOnStart = true;
    public TutorialPopupUI tutorialPopupPrefab;
    public Transform tutorialParent;

    [Header("Game Over")]
    public GameOverUI gameOverPrefab;
    public Transform gameOverParent;

    [Header("Game Systems")]
    public EnemyOrchestrator enemyOrchestrator;

    private TutorialPopupUI activeTutorialPopup;
    private GameOverUI activeGameOverPopup;

    private bool gamePaused;
    private bool gameStarted;
    private bool gameOver;

    void Start()
    {
        Time.timeScale = 1f;

        if (enemyOrchestrator != null)
            enemyOrchestrator.SetSpawningEnabled(false);

        if (showTutorialOnStart && tutorialPopupPrefab != null)
        {
            StartTutorial();
        }
        else
        {
            StartGame();
        }
    }

    void Update()
    {
        if (!gameStarted)
            return;

        if (gameOver)
            return;

        if (player == null)
        {
            TriggerGameOver();
            return;
        }

        if (player.IsDead || player.health <= 0f)
        {
            TriggerGameOver();
        }
    }

    void StartTutorial()
    {
        PauseGame();

        activeTutorialPopup = Instantiate(
            tutorialPopupPrefab,
            tutorialParent
        );

        activeTutorialPopup.gameObject.SetActive(true);
        activeTutorialPopup.Show(OnTutorialFinished);
    }

    void OnTutorialFinished()
    {
        if (activeTutorialPopup != null)
            Destroy(activeTutorialPopup.gameObject);

        StartGame();
    }

    void StartGame()
    {
        gameStarted = true;
        ResumeGame();

        if (enemyOrchestrator != null)
            enemyOrchestrator.SetSpawningEnabled(true);
    }

    void TriggerGameOver()
    {
        gameOver = true;

        if (enemyOrchestrator != null)
            enemyOrchestrator.SetSpawningEnabled(false);

        PauseGame();

        if (gameOverPrefab != null)
        {
            activeGameOverPopup = Instantiate(
                gameOverPrefab,
                gameOverParent
            );

            activeGameOverPopup.gameObject.SetActive(true);

            activeGameOverPopup.Show(
                GetScoreText(),
                ResetGame,
                EndGame
            );
        }
    }

    public static void ResetGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public static void EndGame()
    {
        Time.timeScale = 1f;

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void PauseGame()
    {
        gamePaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        gamePaused = false;
        Time.timeScale = 1f;
    }

    public bool IsGamePaused()
    {
        return gamePaused;
    }

    string GetScoreText()
    {
        //if (scoreBoard == null)
        //    return "0";

        //object scoreValue = TryGetScoreValue(scoreBoard);

        //if (scoreValue == null)
         return "0";

        //return scoreValue.ToString();
    }

    object TryGetScoreValue(Component component)
    {
        System.Type type = component.GetType();

        string[] memberNames =
        {
            "score",
            "Score",
            "currentScore",
            "CurrentScore",
            "points",
            "Points"
        };

        for (int i = 0; i < memberNames.Length; i++)
        {
            FieldInfo field = type.GetField(
                memberNames[i],
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            if (field != null)
                return field.GetValue(component);

            PropertyInfo property = type.GetProperty(
                memberNames[i],
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            if (property != null)
                return property.GetValue(component);
        }

        string[] methodNames =
        {
            "GetScore",
            "GetCurrentScore",
            "GetPoints"
        };

        for (int i = 0; i < methodNames.Length; i++)
        {
            MethodInfo method = type.GetMethod(
                methodNames[i],
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            if (method != null && method.GetParameters().Length == 0)
                return method.Invoke(component, null);
        }

        return null;
    }
}