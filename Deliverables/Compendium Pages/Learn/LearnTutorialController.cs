using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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
        public Button[] extraAllowedButtons;
        public Vector2 arrowPosition;
        public float rotationZ;
        public ArrowMoveDirection moveDirection = ArrowMoveDirection.Horizontal;
    }

    [Header("Scene")]
    public string mainMenuSceneName = "MainMenu";

    [Header("Tutorial State")]
    public TutorialStep currentStep = TutorialStep.ClickBookButton;

    [Header("Step Boxes")]
    public GameObject step1Box;
    public GameObject step2Box;
    public GameObject step3Box;
    public GameObject step4Box;
    public Button finalBoxButton;

    [Header("Required Click Targets")]
    public Button bookButton;
    public Button nextPageButton;
    public Button tutorialBookmarkButton;

    [Header("Optional Menu Exit")]
    public Button tutorialExitButton;

    [Header("Buttons To Hide During Lock")]
    public Button[] buttonsToHideDuringLock;

    [Header("Button Locking")]
    public bool lockButtonsDuringTutorial = true;
    public bool allowExitButtonDuringTutorial = true;

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

    private Dictionary<Selectable, bool> originalInteractableStates =
        new Dictionary<Selectable, bool>();

    void Start()
    {
        SaveOriginalButtonStates();

        if (bookButton != null)
            bookButton.onClick.AddListener(NotifyBookButtonClicked);

        if (nextPageButton != null)
            nextPageButton.onClick.AddListener(NotifyNextPageClicked);

        if (tutorialBookmarkButton != null)
            tutorialBookmarkButton.onClick.AddListener(NotifyBookmarkClicked);

        if (tutorialExitButton != null)
            tutorialExitButton.onClick.AddListener(ExitTutorial);

        if (finalBoxButton != null)
            finalBoxButton.onClick.AddListener(CompleteTutorial);

        RefreshTutorialUI();
    }

    void Update()
    {
        UpdateArrowPosition();
    }

    void SaveOriginalButtonStates()
    {
        originalInteractableStates.Clear();

        Selectable[] allSelectables = FindObjectsByType<Selectable>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (Selectable selectable in allSelectables)
        {
            if (selectable != null && !originalInteractableStates.ContainsKey(selectable))
            {
                originalInteractableStates.Add(selectable, selectable.interactable);
            }
        }
    }

    void RefreshTutorialUI()
    {
        if (step1Box != null) step1Box.SetActive(currentStep == TutorialStep.ClickBookButton);
        if (step2Box != null) step2Box.SetActive(currentStep == TutorialStep.ClickNextPage);
        if (step3Box != null) step3Box.SetActive(currentStep == TutorialStep.ClickBookmark);
        if (step4Box != null) step4Box.SetActive(currentStep == TutorialStep.CloseCompendium);

        if (finalBoxButton != null)
            finalBoxButton.gameObject.SetActive(currentStep == TutorialStep.FinalMessage);

        switch (currentStep)
        {
            case TutorialStep.ClickBookButton:
                SetArrowFromSettings(step1Arrow);
                LockButtonsExcept(step1Arrow);
                break;

            case TutorialStep.ClickNextPage:
                SetArrowFromSettings(step2Arrow);
                LockButtonsExcept(step2Arrow);
                break;

            case TutorialStep.ClickBookmark:
                SetArrowFromSettings(step3Arrow);
                LockButtonsExcept(step3Arrow);
                break;

            case TutorialStep.CloseCompendium:
                SetArrowFromSettings(step4Arrow);
                LockButtonsExcept(step4Arrow);
                break;

            case TutorialStep.FinalMessage:
                HideArrow();
                RestoreAllButtons();
                break;

            case TutorialStep.Complete:
                HideArrow();
                RestoreAllButtons();
                break;
        }
    }

void LockButtonsExcept(ArrowStepSettings settings)
{
    if (!lockButtonsDuringTutorial)
        return;

    Selectable[] allSelectables = FindObjectsByType<Selectable>(
        FindObjectsInactive.Include,
        FindObjectsSortMode.None
    );

    foreach (Selectable selectable in allSelectables)
    {
        if (selectable == null) continue;

        if (selectable.CompareTag("TutorialIgnore"))
        {
            selectable.interactable = true;
            continue;
        }

        selectable.interactable = false;
    }

    if (settings != null && settings.targetButton != null)
        settings.targetButton.interactable = true;

    if (settings != null && settings.extraAllowedButtons != null)
    {
        for (int i = 0; i < settings.extraAllowedButtons.Length; i++)
        {
            if (settings.extraAllowedButtons[i] != null)
                settings.extraAllowedButtons[i].interactable = true;
        }
    }

    // Hide buttons EXCEPT the current target or allowed extra buttons
    if (buttonsToHideDuringLock != null)
    {
        foreach (Button btn in buttonsToHideDuringLock)
        {
            if (btn == null) continue;

            bool isTarget =
                settings != null &&
                settings.targetButton == btn;

            bool isExtraAllowed = false;

            if (settings != null && settings.extraAllowedButtons != null)
            {
                for (int i = 0; i < settings.extraAllowedButtons.Length; i++)
                {
                    if (settings.extraAllowedButtons[i] == btn)
                    {
                        isExtraAllowed = true;
                        break;
                    }
                }
            }

            btn.gameObject.SetActive(isTarget || isExtraAllowed);
        }
    }
}
void RestoreAllButtons()
{
    foreach (var pair in originalInteractableStates)
    {
        if (pair.Key != null)
            pair.Key.interactable = pair.Value;
    }

    // NEW: bring hidden buttons back
    if (buttonsToHideDuringLock != null)
    {
        foreach (Button btn in buttonsToHideDuringLock)
        {
            if (btn != null)
                btn.gameObject.SetActive(true);
        }
    }

    if (finalBoxButton != null && currentStep == TutorialStep.FinalMessage)
        finalBoxButton.interactable = true;
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

    public void NotifyCompendiumClosed()
    {
        if (currentStep != TutorialStep.CloseCompendium) return;

        currentStep = TutorialStep.FinalMessage;
        RefreshTutorialUI();
    }

    public void CompleteTutorial()
    {
        if (currentStep != TutorialStep.FinalMessage) return;

        currentStep = TutorialStep.Complete;
        RefreshTutorialUI();
        ExitTutorial();
    }

    public void ExitTutorial()
    {
        RestoreAllButtons();
        SceneManager.LoadScene(mainMenuSceneName);
    }
}