using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalProgressManager : MonoBehaviour
{
    public static GlobalProgressManager Instance;

    public static event Action OnProgressChanged;

    [Header("Glöbal Progress")]
    [SerializeField] private int identifyCorrectCount = 0;
    [SerializeField] private int buildABugCorrectCount = 0;

    [Header("Full Completion")]
    [Tooltip("How many Identify bugs must be solved before the final completion asset can turn on.")]
    public int identifyRequiredForCompletion = 9;

    [Tooltip("How many Build-A-Bug bugs must be solved before the final completion asset can turn on.")]
    public int buildABugRequiredForCompletion = 9;

    [Tooltip("The object that turns on once both games are fully solved.")]
    public GameObject fullCompletionAsset;

    [Tooltip("How long to wait after both games are complete before turning on the full completion asset.")]
    public float fullCompletionDelay = 1f;

    private bool fullCompletionActivated = false;
    private Coroutine fullCompletionRoutine;

    // Session-only solved Identify bug IDs
    private readonly HashSet<string> solvedIdentifyBugIds = new HashSet<string>();

    // Generic session flags
    private readonly HashSet<string> sessionFlags = new HashSet<string>();

    public int IdentifyCorrectCount => identifyCorrectCount;
    public int BuildABugCorrectCount => buildABugCorrectCount;
    public int TotalCorrectCount => identifyCorrectCount + buildABugCorrectCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (fullCompletionAsset != null)
            fullCompletionAsset.SetActive(false);
    }

    private void Start()
    {
        CheckFullCompletion();
    }

    public void MarkIdentifyBugSolved(string bugId)
    {
        if (string.IsNullOrWhiteSpace(bugId))
            return;

        if (solvedIdentifyBugIds.Add(bugId))
        {
            identifyCorrectCount = solvedIdentifyBugIds.Count;
            NotifyProgressChanged();
        }
    }

    public bool IsIdentifyBugSolved(string bugId)
    {
        if (string.IsNullOrWhiteSpace(bugId))
            return false;

        return solvedIdentifyBugIds.Contains(bugId);
    }

    public void AddBuildABugCorrect()
    {
        buildABugCorrectCount++;
        NotifyProgressChanged();
    }

    public void SetBuildABugCorrectCount(int value)
    {
        buildABugCorrectCount = Mathf.Max(0, value);
        NotifyProgressChanged();
    }

    public void SetFlag(string flagId)
    {
        if (string.IsNullOrWhiteSpace(flagId))
            return;

        sessionFlags.Add(flagId);
    }

    public bool HasFlag(string flagId)
    {
        if (string.IsNullOrWhiteSpace(flagId))
            return false;

        return sessionFlags.Contains(flagId);
    }

    public void ClearFlag(string flagId)
    {
        if (string.IsNullOrWhiteSpace(flagId))
            return;

        sessionFlags.Remove(flagId);
    }

    public void ResetAllProgress()
    {
        identifyCorrectCount = 0;
        buildABugCorrectCount = 0;
        solvedIdentifyBugIds.Clear();
        sessionFlags.Clear();

        fullCompletionActivated = false;

        if (fullCompletionRoutine != null)
        {
            StopCoroutine(fullCompletionRoutine);
            fullCompletionRoutine = null;
        }

        if (fullCompletionAsset != null)
            fullCompletionAsset.SetActive(false);

        NotifyProgressChanged();
    }

    private void NotifyProgressChanged()
    {
        CheckFullCompletion();
        OnProgressChanged?.Invoke();
    }

    private void CheckFullCompletion()
    {
        if (fullCompletionActivated)
            return;

        if (fullCompletionAsset == null)
            return;

        bool identifyComplete = identifyCorrectCount >= identifyRequiredForCompletion;
        bool buildABugComplete = buildABugCorrectCount >= buildABugRequiredForCompletion;

        if (!identifyComplete || !buildABugComplete)
            return;

        if (fullCompletionRoutine == null)
            fullCompletionRoutine = StartCoroutine(ActivateFullCompletionAfterDelay());
    }

    private IEnumerator ActivateFullCompletionAfterDelay()
    {
        if (fullCompletionDelay > 0f)
            yield return new WaitForSecondsRealtime(fullCompletionDelay);

        if (fullCompletionAsset != null)
            fullCompletionAsset.SetActive(true);

        fullCompletionActivated = true;
        fullCompletionRoutine = null;
    }
}
