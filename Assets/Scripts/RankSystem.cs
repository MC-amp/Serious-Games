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
    [Tooltip("Drag the scene's SFXPlayer here so rank unlock sounds use the shared SFX volume system.")]
    public SFXPlayer sfxPlayer;

    [Header("Progress")]
    [SerializeField] private int correctSolvedCount = 0;

    public int CorrectSolvedCount => correctSolvedCount;

    private void Awake()
    {
        if (sfxPlayer == null)
            sfxPlayer = FindObjectOfType<SFXPlayer>();
    }

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

        PlayRankSfx(rank.activationSfx);
    }

    private void PlayRankSfx(AudioClip clip)
    {
        if (clip == null)
            return;

        if (sfxPlayer == null)
            sfxPlayer = FindObjectOfType<SFXPlayer>();

        if (sfxPlayer != null)
            sfxPlayer.PlayCustomSFX(clip);
    }
}
