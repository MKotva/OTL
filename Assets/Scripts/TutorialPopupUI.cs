using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPopupUI : MonoBehaviour
{
    [Header("Pages")]
    public GameObject[] pages;

    [Header("Buttons")]
    public Button nextButton;
    public Button previousButton;
    public Button closeButton;

    [Header("Optional Text")]
    public TMP_Text nextButtonText;

    private int currentPage;
    private System.Action onFinished;

    void Awake()
    {
        if (nextButton != null)
            nextButton.onClick.AddListener(NextPage);

        if (previousButton != null)
            previousButton.onClick.AddListener(PreviousPage);

        if (closeButton != null)
            closeButton.onClick.AddListener(FinishTutorial);
    }

    public void Show(System.Action finishedCallback)
    {
        onFinished = finishedCallback;
        currentPage = 0;

        gameObject.SetActive(true);

        UpdatePage();
    }

    public void NextPage()
    {
        if (pages == null || pages.Length == 0)
        {
            FinishTutorial();
            return;
        }

        if (currentPage >= pages.Length - 1)
        {
            FinishTutorial();
            return;
        }

        currentPage++;
        UpdatePage();
    }

    public void PreviousPage()
    {
        if (pages == null || pages.Length == 0)
            return;

        if (currentPage <= 0)
            return;

        currentPage--;
        UpdatePage();
    }

    public void FinishTutorial()
    {
        if (onFinished != null)
            onFinished.Invoke();
    }

    void UpdatePage()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null)
                pages[i].SetActive(i == currentPage);
        }

        if (previousButton != null)
            previousButton.interactable = currentPage > 0;

        if (nextButtonText != null)
        {
            if (currentPage >= pages.Length - 1)
                nextButtonText.text = "Start";
            else
                nextButtonText.text = "Next";
        }
    }
}