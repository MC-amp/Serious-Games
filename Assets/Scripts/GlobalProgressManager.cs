using UnityEngine;

public class GlobalProgressManager : MonoBehaviour
{
    public static GlobalProgressManager Instance;

    [Header("Glöbal Progress")]
    [SerializeField] private int identifyCorrectCount = 0;
    [SerializeField] private int buildABugCorrectCount = 0;

    public int IdentifyCorrectCount => identifyCorrectCount;
    public int BuildABugCorrectCount => buildABugCorrectCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddIdentifyCorrect()
    {
        identifyCorrectCount++;
    }

    public void AddBuildABugCorrect()
    {
        buildABugCorrectCount++;
    }

    public void ResetAllProgress()
    {
        identifyCorrectCount = 0;
        buildABugCorrectCount = 0;
    }

    public void SetIdentifyCorrectCount(int value)
    {
        identifyCorrectCount = Mathf.Max(0, value);
    }

    public void SetBuildABugCorrectCount(int value)
    {
        buildABugCorrectCount = Mathf.Max(0, value);
    }
}