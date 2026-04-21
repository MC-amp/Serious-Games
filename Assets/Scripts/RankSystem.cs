using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankSystem : MonoBehaviour
{
    [System.Serializable]
    public class RankEntry
    {
        [Tooltip("Höw many correct bugs are needed to unlock this rank.")]
        public int requiredCorrect = 1;

        [Tooltip("The object that turns on when this rank is reached.")]
        public GameObject rankObject;

        [Tooltip("Delay before this rank activates (seconds).")]
        public float activationDelay = 0f;

        [Tooltip("Sound played when this rank activates.")]
        public AudioClip activationSfx;

        [HideInInspector] public bool hasActivated = false;
    }

    [Header("Rank Setup")]
    public List<RankEntry> ranks = new List<RankEntry>();

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Progress")]
    [SerializeField] private int correctSolvedCount = 0;

    public int CorrectSolvedCount => correctSolvedCount;
    //the win screens
    public GameObject IDWin;
    public GameObject IDBaBWin;
    public bool IsIDWon = false;

    private void Start()
    {
        foreach (var rank in ranks)
        {
            if (rank.rankObject != null)
                rank.rankObject.SetActive(false);

            rank.hasActivated = false;
        }
    }

    public void AddCorrectAnswer()
    {
        correctSolvedCount++;
        CheckRanks();
    }

    public void ResetRanks()
    {
        correctSolvedCount = 0;

        foreach (var rank in ranks)
        {
            rank.hasActivated = false;

            if (rank.rankObject != null)
                rank.rankObject.SetActive(false);
        }
    }

    private void CheckRanks()
    {
        foreach (var rank in ranks)
        {
            if (rank.hasActivated)
                continue;

            if (correctSolvedCount >= rank.requiredCorrect)
            {
                StartCoroutine(ActivateRankWithDelay(rank));
                rank.hasActivated = true;
            }
        }
    }

    private IEnumerator ActivateRankWithDelay(RankEntry rank)
    {
        yield return new WaitForSeconds(rank.activationDelay);

        if (rank.rankObject != null)
            rank.rankObject.SetActive(true);

        if (rank.activationSfx != null && audioSource != null)
            audioSource.PlayOneShot(rank.activationSfx);
    }
     void IDWon()
    {
        if(correctSolvedCount == 9)
        {
            IsIDWon = true;
            Debug.Log("it worked!!");
        }
    }
}