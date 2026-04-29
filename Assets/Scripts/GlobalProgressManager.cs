using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalProgressManager : MonoBehaviour
{
    public static GlobalProgressManager Instance;

    public static event Action OnProgressChanged;

    public enum GameId
    {
        Identify,
        BuildABug
    }

    [Header("Glöbal Progress")]
    [SerializeField] private int identifyCorrectCount = 0;
    [SerializeField] private int buildABugCorrectCount = 0;

    [Header("Game Completion Requirements")]
    [Tooltip("How many Identify bugs must be solved before Identify counts as complete.")]
    public int identifyRequiredForCompletion = 9;

    [Tooltip("How many Build-A-Bug bugs must be solved before Build-A-Bug counts as complete.")]
    public int buildABugRequiredForCompletion = 9;

    [Header("Optional Full Completion Asset")]
    [Tooltip("Optional object that turns on once both games are fully solved. Usually this only works if the object lives in the same scene as this manager.")]
    public GameObject fullCompletionAsset;

    [Tooltip("How long to wait after both games are complete before turning on the optional full completion asset.")]
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

    public bool IsIdentifyComplete => identifyCorrectCount >= identifyRequiredForCompletion;
    public bool IsBuildABugComplete => buildABugCorrectCount >= buildABugRequiredForCompletion;
    public bool IsFullyComplete => IsIdentifyComplete && IsBuildABugComplete;

    public int CompletedGameCount
    {
        get
        {
            int count = 0;

            if (IsIdentifyComplete)
                count++;

            if (IsBuildABugComplete)
                count++;

            return count;
        }
    }

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

    public bool IsGameComplete(GameId gameId)
    {
        switch (gameId)
        {
            case GameId.Identify:
                return IsIdentifyComplete;

            case GameId.BuildABug:
                return IsBuildABugComplete;

            default:
                return false;
        }
    }

    public bool IsOtherGameComplete(GameId currentGame)
    {
        switch (currentGame)
        {
            case GameId.Identify:
                return IsBuildABugComplete;

            case GameId.BuildABug:
                return IsIdentifyComplete;

            default:
                return false;
        }
    }

    public int GetCorrectCount(GameId gameId)
    {
        switch (gameId)
        {
            case GameId.Identify:
                return identifyCorrectCount;

            case GameId.BuildABug:
                return buildABugCorrectCount;

            default:
                return 0;
        }
    }

    public int GetRequiredCount(GameId gameId)
    {
        switch (gameId)
        {
            case GameId.Identify:
                return identifyRequiredForCompletion;

            case GameId.BuildABug:
                return buildABugRequiredForCompletion;

            default:
                return 0;
        }
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

        if (!IsFullyComplete)
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
