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
    // vars to see how many games have been won
    public bool BothGamesWon = false;
    public int GamesWon = 0;
    // build a Bug manger references
    public BugBuildGameManager buildABugGameManger;
    public bool isBaBWon;
    public GameObject BuildABugWin;
    public GameObject BuildABugAndInsectIDWin;

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
        // set build a bug references 
        isBaBWon = buildABugGameManger.IsBaBWon;
        BuildABugWin = buildABugGameManger.BaBWon;
        BuildABugAndInsectIDWin = buildABugGameManger.BaBIDIWon;

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
        GamesWon++;
        Debug.Log(GamesWon);
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
    //public void BuildABugSolved()
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
        //GamesWon = 0;
        //BothGamesWon = false;
        NotifyProgressChanged();
    }

    private void NotifyProgressChanged()
    {
        OnProgressChanged?.Invoke();
        //if (GamesWon==2)
        //{
        //    BothGamesWon =true;

        //}
    }
    // Show the win screen in Build a big base on what Games Won value is
    public void BuildABugWon()
    {

        if (isBaBWon)
        {
            GamesWon++;
            Debug.Log(GamesWon);
        }
        if (GamesWon == 0 && isBaBWon == true)
        {
            BuildABugWin.SetActive(true);
        }
        if (GamesWon > 0 && isBaBWon == true)
        {
            BuildABugAndInsectIDWin.SetActive(true);
        }
    }
}