using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BookTutorialController : MonoBehaviour
{
    public enum TutorialStep
    {
        ClickBookButton,
        ClickNextPage,
        ClickBookmark,
        CloseCompendium,
        FinalMessage,
        Complete
    }

    public enum ArrowMoveDirection
    {
        Horizontal,
        Vertical
    }

    [System.Serializable]
    public class ArrowStepSettings
    {
        public Button targetButton;
        public Vector2 arrowPosition;
        public float rotationZ;
        public ArrowMoveDirection moveDirection = ArrowMoveDirection.Horizontal;
    }

    [Header("Scene")]
    public string mainMenuSceneName = "MainMenu";

    [Header("Tutorial State")]
    public TutorialStep currentStep = TutorialStep.ClickBookButton;

    [Header("Step Boxes (shown above compendium)")]
    public GameObject step1Box;
    public GameObject step2Box;
    public GameObject step3Box;
    public GameObject step4Box;

    [Header("Final Box (shown behind compendium)")]
    public Button finalBoxButton;

    [Header("Required Click Targets")]
    public Button bookButton;
    public Button nextPageButton;
    public Button tutorialBookmarkButton;

    [Header("Optional Menu Back Button")]
    public Button menuBackButton;

    [Header("Arrow")]
    public RectTransform tutorialArrow;
    public float arrowMoveDistance = 20f;
    public float arrowMoveSpeed = 2f;

    [Header("Arrow Settings Per Step")]
    public ArrowStepSettings step1Arrow;
    public ArrowStepSettings step2Arrow;
    public ArrowStepSettings step3Arrow;
    public ArrowStepSettings step4Arrow;

    [Header("Debug")]
    public bool manualArrowPlacementMode = false;

    private Vector2 currentArrowPosition;
    private float currentArrowRotation;
    private ArrowMoveDirection currentMoveDirection;

    void Start()
    {
        if (bookButton != null)
            bookButton.onClick.AddListener(NotifyBookButtonClicked);

        if (nextPageButton != null)
            nextPageButton.onClick.AddListener(NotifyNextPageClicked);

        if (tutorialBookmarkButton != null)
            tutorialBookmarkButton.onClick.AddListener(NotifyBookmarkClicked);

        // Final box returns to menu
        if (finalBoxButton != null)
            finalBoxButton.onClick.AddListener(ExitTutorial);

        // Optional separate menu back button returns to menu
        if (menuBackButton != null)
            menuBackButton.onClick.AddListener(ExitTutorial);

        RefreshTutorialUI();
    }

    void Update()
    {
        UpdateArrowPosition();
    }

    void RefreshTutorialUI()
    {
        if (step1Box != null) step1Box.SetActive(currentStep == TutorialStep.ClickBookButton);
        if (step2Box != null) step2Box.SetActive(currentStep == TutorialStep.ClickNextPage);
        if (step3Box != null) step3Box.SetActive(currentStep == TutorialStep.ClickBookmark);
        if (step4Box != null) step4Box.SetActive(currentStep == TutorialStep.CloseCompendium);

        // Final box shows only on final step, but does NOT auto-exit
        if (finalBoxButton != null)
            finalBoxButton.gameObject.SetActive(currentStep == TutorialStep.FinalMessage);

        switch (currentStep)
        {
            case TutorialStep.ClickBookButton:
                SetArrowFromSettings(step1Arrow);
                break;

            case TutorialStep.ClickNextPage:
                SetArrowFromSettings(step2Arrow);
                break;

            case TutorialStep.ClickBookmark:
                SetArrowFromSettings(step3Arrow);
                break;

            case TutorialStep.CloseCompendium:
                SetArrowFromSettings(step4Arrow);
                break;

            case TutorialStep.FinalMessage:
            case TutorialStep.Complete:
                HideArrow();
                break;
        }
    }

    void SetArrowFromSettings(ArrowStepSettings settings)
    {
        if (tutorialArrow == null || settings == null)
        {
            HideArrow();
            return;
        }

        currentArrowPosition = settings.arrowPosition;
        currentArrowRotation = settings.rotationZ;
        currentMoveDirection = settings.moveDirection;

        tutorialArrow.gameObject.SetActive(true);

        if (!manualArrowPlacementMode)
        {
            tutorialArrow.anchoredPosition = currentArrowPosition;
            tutorialArrow.rotation = Quaternion.Euler(0f, 0f, currentArrowRotation);
        }
    }

    void UpdateArrowPosition()
    {
        if (tutorialArrow == null) return;
        if (!tutorialArrow.gameObject.activeSelf) return;
        if (manualArrowPlacementMode) return;

        float move = Mathf.Sin(Time.unscaledTime * arrowMoveSpeed) * arrowMoveDistance;

        if (currentMoveDirection == ArrowMoveDirection.Horizontal)
        {
            tutorialArrow.anchoredPosition = currentArrowPosition + new Vector2(move, 0f);
        }
        else
        {
            tutorialArrow.anchoredPosition = currentArrowPosition + new Vector2(0f, move);
        }

        tutorialArrow.rotation = Quaternion.Euler(0f, 0f, currentArrowRotation);
    }

    void HideArrow()
    {
        if (tutorialArrow != null)
            tutorialArrow.gameObject.SetActive(false);
    }

    public void NotifyBookButtonClicked()
    {
        if (currentStep != TutorialStep.ClickBookButton) return;

        currentStep = TutorialStep.ClickNextPage;
        RefreshTutorialUI();
    }

    public void NotifyNextPageClicked()
    {
        if (currentStep != TutorialStep.ClickNextPage) return;

        currentStep = TutorialStep.ClickBookmark;
        RefreshTutorialUI();
    }

    public void NotifyBookmarkClicked()
    {
        if (currentStep != TutorialStep.ClickBookmark) return;

        currentStep = TutorialStep.CloseCompendium;
        RefreshTutorialUI();
    }

    // Called by BookButton.cs when the compendium closes
    public void NotifyCompendiumClosed()
    {
        if (currentStep != TutorialStep.CloseCompendium) return;

        currentStep = TutorialStep.FinalMessage;
        RefreshTutorialUI();
    }

    public void ExitTutorial()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}