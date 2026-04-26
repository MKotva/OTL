using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("Text")]
    public TMP_Text scoreText;

    [Header("Buttons")]
    public Button resetButton;
    public Button endButton;

    private System.Action onResetClicked;
    private System.Action onEndClicked;

    void Awake()
    {
        if (resetButton != null)
            resetButton.onClick.AddListener(HandleResetClicked);

        if (endButton != null)
            endButton.onClick.AddListener(HandleEndClicked);
    }

    public void Show(string score, System.Action resetCallback, System.Action endCallback)
    {
        onResetClicked = resetCallback;
        onEndClicked = endCallback;

        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    public void HandleResetClicked()
    {
        GameManager.ResetGame();
    }

    public void HandleEndClicked()
    {
        GameManager.EndGame();
    }
}