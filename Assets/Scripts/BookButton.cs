using System;
using UnityEngine;
using UnityEngine.UI;

public class SimpleSlideBook : MonoBehaviour
{
    [Header("Panel Movement")]
    public RectTransform panel;

    [Range(-3000f, 3000f)]
    public float openX = 0f;

    [Range(0f, 3000f)]
    public float slideDistance = 900f;

    public float slideSpeed = 8f;

    [Header("Images")]
    public Image bgImage;          // optional
    public Image pageImage;        // REQUIRED (your 'Page' object)

    [Header("Main Buttons")]
    public Button openBookButton;  // outside panel
    public Button exitButton;      // inside panel
    public Button nextButton;
    public Button backButton;

    [Header("Pages (sprites)")]
    public Sprite[] pages;

    [Header("Bookmarks (tabs)")]
    public Button[] bookmarkButtons = new Button[4];

    [Tooltip("Which page each bookmark goes to (0 = first page). Must match bookmarkButtons size.")]
    public int[] bookmarkPages = new int[4];

    [Tooltip("How far the selected bookmark sticks out (pixels).")]
    public float selectedBookmarkXOffset = 20f;

    [Tooltip("Optional: make selected bookmark slightly bigger.")]
    public float selectedBookmarkScale = 1.05f;

    [Header("Bookmark Section Ranges (auto-extend based on current page)")]
    public BookmarkSection[] bookmarkSections = new BookmarkSection[4];

    [Serializable]
    public class BookmarkSection
    {
        public string name;        // optional label (Main/Bees/Flies/Wasps)
        public int startPage;      // inclusive
        public int endPage;        // inclusive
    }

    [Header("Page Link Buttons (show on certain pages AND jump to a page)")]
    public PageLinkButton[] pageLinkButtons;

    [Serializable]
    public class PageLinkButton
    {
        public Button button;

        [Tooltip("Pages this button appears on (0 = first page). Example: 0,2,5")]
        public int[] showOnPages;

        [Tooltip("When clicked, jump to this page (0 = first page).")]
        public int goToPage;
    }

    [Header("Debug")]
    public bool logClicks = false;

    private bool isOpen = false;
    private int pageIndex = 0;

    private Vector2[] bookmarkBasePositions;
    private Vector3[] bookmarkBaseScales;

    void Start()
    {
        // ---------- VALIDATION ----------
        if (panel == null)
        {
            Debug.LogError("SimpleSlideBook: panel is not assigned.");
            enabled = false;
            return;
        }

        if (pageImage == null)
        {
            Debug.LogError("SimpleSlideBook: pageImage is not assigned (drag your Page Image here).");
            enabled = false;
            return;
        }

        if (pages == null || pages.Length == 0)
        {
            Debug.LogError("SimpleSlideBook: pages[] is empty. Add page sprites in inspector.");
            enabled = false;
            return;
        }

        // Prevent BG/Page from blocking clicks
        if (bgImage != null) bgImage.raycastTarget = false;
        pageImage.raycastTarget = false;

        // Hook main buttons
        if (openBookButton != null) openBookButton.onClick.AddListener(OpenBook);
        if (exitButton != null) exitButton.onClick.AddListener(CloseBook);
        if (nextButton != null) nextButton.onClick.AddListener(NextPage);
        if (backButton != null) backButton.onClick.AddListener(BackPage);

        // Hook bookmarks + store base positions/scales
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

        // Hook page link buttons
        if (pageLinkButtons != null && pageLinkButtons.Length > 0)
        {
            for (int i = 0; i < pageLinkButtons.Length; i++)
            {
                int index = i;

                if (pageLinkButtons[index] != null && pageLinkButtons[index].button != null)
                {
                    pageLinkButtons[index].button.onClick.AddListener(() => GoToPageFromLink(index));
                }
            }
        }

        // Start CLOSED
        panel.anchoredPosition = new Vector2(openX - slideDistance, panel.anchoredPosition.y);
        isOpen = false;

        // Start on page 0
        pageIndex = 0;
        SetPage(pageIndex);

        UpdateNavButtons();
        UpdatePageLinkButtons();
        UpdateBookmarkVisualsByCurrentPage(); // <-- NEW
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

    void OpenBook()
    {
        isOpen = true;

        pageIndex = 0;
        SetPage(pageIndex);

        UpdateNavButtons();
        UpdatePageLinkButtons();
        UpdateBookmarkVisualsByCurrentPage(); // <-- NEW

        if (openBookButton != null)
            openBookButton.interactable = false;
    }

    void CloseBook()
    {
        isOpen = false;

        if (openBookButton != null)
            openBookButton.interactable = true;
    }

    void NextPage()
    {
        if (pageIndex < pages.Length - 1)
        {
            pageIndex++;
            SetPage(pageIndex);
        }

        UpdateNavButtons();
        UpdatePageLinkButtons();
        UpdateBookmarkVisualsByCurrentPage(); // <-- NEW
    }

    void BackPage()
    {
        if (pageIndex > 0)
        {
            pageIndex--;
            SetPage(pageIndex);
        }

        UpdateNavButtons();
        UpdatePageLinkButtons();
        UpdateBookmarkVisualsByCurrentPage(); // <-- NEW
    }

    void GoToBookmark(int bookmarkIndex)
    {
        if (bookmarkPages == null || bookmarkPages.Length != bookmarkButtons.Length)
        {
            Debug.LogError("SimpleSlideBook: bookmarkPages must be the same size as bookmarkButtons.");
            return;
        }

        int targetPage = ClampPage(bookmarkPages[bookmarkIndex]);

        if (logClicks)
            Debug.Log("Bookmark click: " + bookmarkIndex + " -> page " + targetPage);

        pageIndex = targetPage;
        SetPage(pageIndex);

        UpdateNavButtons();
        UpdatePageLinkButtons();
        UpdateBookmarkVisualsByCurrentPage(); // <-- NEW (don’t “lock” to clicked bookmark)
    }

    void GoToPageFromLink(int linkIndex)
    {
        if (pageLinkButtons == null || linkIndex < 0 || linkIndex >= pageLinkButtons.Length)
            return;

        int targetPage = ClampPage(pageLinkButtons[linkIndex].goToPage);

        if (logClicks)
            Debug.Log("PageLink click: " + linkIndex + " -> page " + targetPage);

        pageIndex = targetPage;
        SetPage(pageIndex);

        UpdateNavButtons();
        UpdatePageLinkButtons();
        UpdateBookmarkVisualsByCurrentPage(); // <-- NEW
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
            backButton.interactable = (pageIndex > 0);

        if (nextButton != null)
            nextButton.interactable = (pageIndex < pages.Length - 1);
    }

    // ---------- NEW: pick the bookmark based on current page ----------
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

        return -1; // none matched
    }

    // Restores exact bookmark positions/scales, then pops selected
    void UpdateBookmarkVisuals(int selectedIndex)
    {
        for (int i = 0; i < bookmarkButtons.Length; i++)
        {
            var btn = bookmarkButtons[i];
            if (btn == null) continue;

            RectTransform rt = btn.GetComponent<RectTransform>();
            if (rt == null) continue;

            // Restore original placement (prevents drifting)
            rt.anchoredPosition = bookmarkBasePositions[i];
            rt.localScale = bookmarkBaseScales[i];

            if (i == selectedIndex)
            {
                rt.anchoredPosition = bookmarkBasePositions[i] + new Vector2(selectedBookmarkXOffset, 0f);
                rt.localScale = bookmarkBaseScales[i] * selectedBookmarkScale;

                // Selected draws above other bookmarks
                rt.SetAsLastSibling();
            }
        }
    }

    void UpdatePageLinkButtons()
    {
        if (pageLinkButtons == null) return;

        for (int i = 0; i < pageLinkButtons.Length; i++)
        {
            var link = pageLinkButtons[i];
            if (link == null || link.button == null) continue;

            bool show = false;

            if (link.showOnPages != null)
            {
                for (int p = 0; p < link.showOnPages.Length; p++)
                {
                    if (link.showOnPages[p] == pageIndex)
                    {
                        show = true;
                        break;
                    }
                }
            }

            link.button.gameObject.SetActive(show);
        }
    }
}