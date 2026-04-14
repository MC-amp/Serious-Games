using System;
using UnityEngine;
using UnityEngine.UI;

public class BookButton : MonoBehaviour
{
    [Header("Panel Movement")]
    public RectTransform panel;

    [Range(-3000f, 3000f)]
    public float openX = 0f;

    [Range(0f, 3000f)]
    public float slideDistance = 900f;

    public float slideSpeed = 8f;

    [Header("Images")]
    public Image bgImage;
    public Image pageImage;

    [Header("Main Buttons")]
    public Button openBookButton;
    public Button exitButton;
    public Button nextButton;
    public Button backButton;
    public Button outsideCloseButton;

    [Header("Tutorial")]
    public BookTutorialController tutorialController;

    [Header("Pages (sprites)")]
    public Sprite[] pages;

    [Header("Bookmarks (tabs)")]
    public Button[] bookmarkButtons = new Button[4];
    public int[] bookmarkPages = new int[4];

    public float selectedBookmarkXOffset = 20f;
    public float selectedBookmarkScale = 1.05f;

    [Header("Bookmark Section Ranges")]
    public BookmarkSection[] bookmarkSections = new BookmarkSection[4];

    [System.Serializable]
    public class BookmarkSection
    {
        public string name;
        public int startPage;
        public int endPage;
    }

    [Header("Audio Sources")]
    public AudioSource openAudioSource;
    public AudioSource closeAudioSource;
    public AudioSource pageSfxAudioSource;
    public AudioSource bookmarkAudioSource;

    [Header("Audio Clips")]
    public AudioClip openClip;
    public AudioClip closeClip;
    public AudioClip pageTurnClip1;
    public AudioClip pageTurnClip2;
    public AudioClip bookmarkClip;

    [Header("Audio Volume")]
    [Range(0f, 1f)]
    public float bookVolume = 1f;

    private bool isOpen = false;
    private int pageIndex = 0;

    private Vector2[] bookmarkBasePositions;
    private Vector3[] bookmarkBaseScales;

    void Start()
    {
        if (panel == null)
        {
            Debug.LogError("BookButton: panel is not assigned.");
            enabled = false;
            return;
        }

        if (pageImage == null)
        {
            Debug.LogError("BookButton: pageImage is not assigned.");
            enabled = false;
            return;
        }

        if (pages == null || pages.Length == 0)
        {
            Debug.LogError("BookButton: pages[] is empty.");
            enabled = false;
            return;
        }

        if (bgImage != null) bgImage.raycastTarget = false;
        pageImage.raycastTarget = false;

        ApplyBookVolume();

        if (openBookButton != null) openBookButton.onClick.AddListener(OpenBook);
        if (exitButton != null) exitButton.onClick.AddListener(CloseBook);
        if (nextButton != null) nextButton.onClick.AddListener(NextPage);
        if (backButton != null) backButton.onClick.AddListener(BackPage);
        if (outsideCloseButton != null) outsideCloseButton.onClick.AddListener(CloseBook);

        bookmarkBasePositions = new Vector2[bookmarkButtons.Length];
        bookmarkBaseScales = new Vector3[bookmarkButtons.Length];

        for (int i = 0; i < bookmarkButtons.Length; i++)
        {
            int index = i;

            if (bookmarkButtons[index] != null)
            {
                RectTransform rt = bookmarkButtons[index].GetComponent<RectTransform>();
                if (rt != null)
                {
                    bookmarkBasePositions[index] = rt.anchoredPosition;
                    bookmarkBaseScales[index] = rt.localScale;
                }

                bookmarkButtons[index].onClick.AddListener(() => GoToBookmark(index));
            }
        }

        panel.anchoredPosition = new Vector2(openX - slideDistance, panel.anchoredPosition.y);
        isOpen = false;

        if (outsideCloseButton != null)
            outsideCloseButton.gameObject.SetActive(false);

        pageIndex = 0;
        SetPage(pageIndex);

        UpdateNavButtons();
        UpdateBookmarkVisualsByCurrentPage();
    }

    void OnValidate()
    {
        ApplyBookVolume();
    }

    void Update()
    {
        float targetX = isOpen ? openX : (openX - slideDistance);

        panel.anchoredPosition = Vector2.Lerp(
            panel.anchoredPosition,
            new Vector2(targetX, panel.anchoredPosition.y),
            Time.unscaledDeltaTime * slideSpeed
        );
    }

    void ApplyBookVolume()
    {
        float savedBookVolume = PlayerPrefs.GetFloat("BookVolume", 1f);
        float finalVolume = Mathf.Clamp01(bookVolume * savedBookVolume);

        if (openAudioSource != null) openAudioSource.volume = finalVolume;
        if (closeAudioSource != null) closeAudioSource.volume = finalVolume;
        if (pageSfxAudioSource != null) pageSfxAudioSource.volume = finalVolume;
        if (bookmarkAudioSource != null) bookmarkAudioSource.volume = finalVolume;
    }

    void PlayOpenSound()
    {
        if (openAudioSource == null || openClip == null) return;
        if (openAudioSource.isPlaying) return;

        openAudioSource.clip = openClip;
        openAudioSource.Play();
    }

    void PlayCloseSound()
    {
        if (closeAudioSource == null || closeClip == null) return;
        if (closeAudioSource.isPlaying) return;

        closeAudioSource.clip = closeClip;
        closeAudioSource.Play();
    }

    void PlayPageTurnSound()
    {
        if (pageSfxAudioSource == null) return;
        if (pageSfxAudioSource.isPlaying) return;

        AudioClip clipToPlay = null;

        if (pageTurnClip1 != null && pageTurnClip2 != null)
        {
            clipToPlay = UnityEngine.Random.value < 0.5f ? pageTurnClip1 : pageTurnClip2;
        }
        else if (pageTurnClip1 != null)
        {
            clipToPlay = pageTurnClip1;
        }
        else if (pageTurnClip2 != null)
        {
            clipToPlay = pageTurnClip2;
        }

        if (clipToPlay != null)
        {
            pageSfxAudioSource.clip = clipToPlay;
            pageSfxAudioSource.Play();
        }
    }

    void PlayBookmarkSound()
    {
        if (bookmarkAudioSource == null || bookmarkClip == null) return;
        if (bookmarkAudioSource.isPlaying) return;

        bookmarkAudioSource.clip = bookmarkClip;
        bookmarkAudioSource.Play();
    }

    public void OpenBook()
    {
        ApplyBookVolume();

        isOpen = true;

        pageIndex = 0;
        SetPage(pageIndex);

        UpdateNavButtons();
        UpdateBookmarkVisualsByCurrentPage();

        PlayOpenSound();

        if (openBookButton != null)
            openBookButton.interactable = false;

        if (outsideCloseButton != null)
            outsideCloseButton.gameObject.SetActive(true);
    }

    public void CloseBook()
    {
        if (!isOpen)
            return;

        ApplyBookVolume();

        isOpen = false;

        PlayCloseSound();

        if (openBookButton != null)
            openBookButton.interactable = true;

        if (outsideCloseButton != null)
            outsideCloseButton.gameObject.SetActive(false);

        if (tutorialController != null)
            tutorialController.NotifyCompendiumClosed();
    }

    void NextPage()
    {
        ApplyBookVolume();

        if (pageIndex < pages.Length - 1)
        {
            pageIndex++;
            SetPage(pageIndex);
            PlayPageTurnSound();
        }

        UpdateNavButtons();
        UpdateBookmarkVisualsByCurrentPage();
    }

    void BackPage()
    {
        ApplyBookVolume();

        if (pageIndex > 0)
        {
            pageIndex--;
            SetPage(pageIndex);
            PlayPageTurnSound();
        }

        UpdateNavButtons();
        UpdateBookmarkVisualsByCurrentPage();
    }

    void GoToBookmark(int bookmarkIndex)
    {
        ApplyBookVolume();

        if (bookmarkPages == null || bookmarkPages.Length != bookmarkButtons.Length)
        {
            Debug.LogError("BookButton: bookmarkPages must be the same size as bookmarkButtons.");
            return;
        }

        int targetPage = ClampPage(bookmarkPages[bookmarkIndex]);

        PlayBookmarkSound();

        if (pageIndex != targetPage)
        {
            pageIndex = targetPage;
            SetPage(pageIndex);
            PlayPageTurnSound();
        }

        UpdateNavButtons();
        UpdateBookmarkVisualsByCurrentPage();
    }

    int ClampPage(int p)
    {
        if (p < 0) p = 0;
        if (p > pages.Length - 1) p = pages.Length - 1;
        return p;
    }

    void SetPage(int index)
    {
        index = ClampPage(index);

        pageImage.sprite = pages[index];
        pageImage.enabled = true;
        pageImage.color = Color.white;
    }

    void UpdateNavButtons()
    {
        if (backButton != null)
        {
            bool showBack = pageIndex > 0;
            backButton.gameObject.SetActive(showBack);
        }

        if (nextButton != null)
        {
            bool showNext = pageIndex < pages.Length - 1;
            nextButton.gameObject.SetActive(showNext);
        }
    }

    void UpdateBookmarkVisualsByCurrentPage()
    {
        int selectedBookmark = GetBookmarkIndexForPage(pageIndex);
        UpdateBookmarkVisuals(selectedBookmark);
    }

    int GetBookmarkIndexForPage(int page)
    {
        if (bookmarkSections == null) return -1;
        if (bookmarkSections.Length != bookmarkButtons.Length) return -1;

        for (int i = 0; i < bookmarkSections.Length; i++)
        {
            int start = bookmarkSections[i].startPage;
            int end = bookmarkSections[i].endPage;

            if (page >= start && page <= end)
                return i;
        }

        return -1;
    }

    void UpdateBookmarkVisuals(int selectedIndex)
    {
        for (int i = 0; i < bookmarkButtons.Length; i++)
        {
            var btn = bookmarkButtons[i];
            if (btn == null) continue;

            RectTransform rt = btn.GetComponent<RectTransform>();
            if (rt == null) continue;

            rt.anchoredPosition = bookmarkBasePositions[i];
            rt.localScale = bookmarkBaseScales[i];

            if (i == selectedIndex)
            {
                rt.anchoredPosition = bookmarkBasePositions[i] + new Vector2(selectedBookmarkXOffset, 0f);
                rt.localScale = bookmarkBaseScales[i] * selectedBookmarkScale;
                rt.SetAsLastSibling();
            }
        }
    }
}