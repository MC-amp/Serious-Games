using System;
using System.Collections.Generic;
using UnityEngine;

public class GlobalProgressManager : MonoBehaviour
{
    public static GlobalProgressManager Instance;

    public static event Action OnProgressChanged;

    [Header("Glöbal Progress")]
    [SerializeField] private int identifyCorrectCount = 0;
    [SerializeField] private int buildABugCorrectCount = 0;

    // Session-only solved Identify bug IDs
    private readonly HashSet<string> solvedIdentifyBugIds = new HashSet<string>();

    // Generic session flags
    private readonly HashSet<string> sessionFlags = new HashSet<string>();

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
        NotifyProgressChanged();
    }

    private void NotifyProgressChanged()
    {
        OnProgressChanged?.Invoke();
    }
}