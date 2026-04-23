using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FreeplayManager : MonoBehaviour
{
    [Header("Bug Slöts")]
    public BugSlot headSlot;
    public BugSlot bodySlot;
    public BugSlot legSlot;
    public BugSlot wingSlot;

    [Header("Species Bars")]
    public SpeciesBar[] speciesBars;

    [Header("Finish Evaluation")]
    public Button finishButton;
    public TextMeshProUGUI resultText;

    [Header("Result Names - Pure")]
    [TextArea] public string pureBeeResult = "You built a Pure Bee!";
    [TextArea] public string pureFlyResult = "You built a Pure Fly!";
    [TextArea] public string pureWaspResult = "You built a Pure Wasp!";

    [Header("Result Names - 50 / 50 Hybrids")]
    [TextArea] public string flyWaspHybridResult = "You built a Hybrid Predator!";
    [TextArea] public string beeFlyHybridResult = "You built a Pollinator Scavenger!";
    [TextArea] public string beeWaspHybridResult = "You built a Stinging Worker!";

    [Header("Result Names - 75% Dominant")]
    [TextArea] public string beeDominant75Result = "You built a Worker Variant!";
    [TextArea] public string flyDominant75Result = "You built a Scavenger Variant!";
    [TextArea] public string waspDominant75Result = "You built an Apex Hunter!";

    [Header("Result Names - 50% Dominant Fallback")]
    [TextArea] public string flyDominant50Result = "You built a Fly-Dominant Hybrid!";
    [TextArea] public string beeDominant50Result = "You built a Bee-Dominant Hybrid!";
    [TextArea] public string waspDominant50Result = "You built a Wasp-Dominant Hybrid!";

    [Header("Fallback Results")]
    [TextArea] public string emptyBuildResult = "Build something first!";
    [TextArea] public string unusualSpecimenResult = "You built an Unusual Specimen!";

    private Dictionary<string, SpeciesBar> barLookup = new Dictionary<string, SpeciesBar>();

    private BugPartOption lastHeadPart;
    private BugPartOption lastBodyPart;
    private BugPartOption lastLegPart;
    private BugPartOption lastWingPart;

    private void Awake()
    {
        BuildBarLookup();
    }

    private void Start()
    {
        if (finishButton != null)
            finishButton.onClick.AddListener(EvaluateBuild);

        if (resultText != null)
            resultText.gameObject.SetActive(false);

        RecalculateBars(true);
        CacheCurrentParts();
    }

    private void Update()
    {
        if (BuildChanged())
        {
            RecalculateBars(false);
            CacheCurrentParts();

            if (resultText != null && resultText.gameObject.activeSelf)
                resultText.gameObject.SetActive(false);
        }
    }

    private void BuildBarLookup()
    {
        barLookup.Clear();

        if (speciesBars == null)
            return;

        foreach (SpeciesBar bar in speciesBars)
        {
            if (bar == null)
                continue;

            string speciesId = bar.SpeciesId;

            if (string.IsNullOrWhiteSpace(speciesId))
                continue;

            if (!barLookup.ContainsKey(speciesId))
                barLookup.Add(speciesId, bar);
            else
                Debug.LogWarning("FreeplayManager: Duplicate SpeciesBar id found: " + speciesId);
        }
    }

    private bool BuildChanged()
    {
        BugPartOption currentHead = headSlot != null ? headSlot.EquippedPart : null;
        BugPartOption currentBody = bodySlot != null ? bodySlot.EquippedPart : null;
        BugPartOption currentLeg = legSlot != null ? legSlot.EquippedPart : null;
        BugPartOption currentWing = wingSlot != null ? wingSlot.EquippedPart : null;

        return currentHead != lastHeadPart ||
               currentBody != lastBodyPart ||
               currentLeg != lastLegPart ||
               currentWing != lastWingPart;
    }

    private void CacheCurrentParts()
    {
        lastHeadPart = headSlot != null ? headSlot.EquippedPart : null;
        lastBodyPart = bodySlot != null ? bodySlot.EquippedPart : null;
        lastLegPart = legSlot != null ? legSlot.EquippedPart : null;
        lastWingPart = wingSlot != null ? wingSlot.EquippedPart : null;
    }

    private void RecalculateBars(bool logWarnings)
    {
        Dictionary<string, int> speciesCounts = BuildSpeciesCounts();

        foreach (KeyValuePair<string, SpeciesBar> kvp in barLookup)
        {
            string speciesId = kvp.Key;
            SpeciesBar bar = kvp.Value;

            int count = speciesCounts.ContainsKey(speciesId) ? speciesCounts[speciesId] : 0;
            float fillAmount = count / 4f;
            bar.SetFill01(fillAmount);
        }

        if (logWarnings)
            WarnForMissingBars(speciesCounts);
    }

    private Dictionary<string, int> BuildSpeciesCounts()
    {
        Dictionary<string, int> speciesCounts = new Dictionary<string, int>();

        CountPart(headSlot, speciesCounts);
        CountPart(bodySlot, speciesCounts);
        CountPart(legSlot, speciesCounts);
        CountPart(wingSlot, speciesCounts);

        return speciesCounts;
    }

    private void CountPart(BugSlot slot, Dictionary<string, int> speciesCounts)
    {
        if (slot == null || slot.EquippedPart == null)
            return;

        string speciesId = slot.EquippedPart.bugType.ToString();

        if (!speciesCounts.ContainsKey(speciesId))
            speciesCounts[speciesId] = 0;

        speciesCounts[speciesId]++;
    }

    private void WarnForMissingBars(Dictionary<string, int> speciesCounts)
    {
        foreach (KeyValuePair<string, int> kvp in speciesCounts)
        {
            if (!barLookup.ContainsKey(kvp.Key))
            {
                Debug.LogWarning(
                    "FreeplayManager: No SpeciesBar found for species '" + kvp.Key + "'. " +
                    "Make sure the SpeciesBar SpeciesId matches the BugType name exactly."
                );
            }
        }
    }

    public void EvaluateBuild()
    {
        Dictionary<string, int> speciesCounts = BuildSpeciesCounts();
        int totalParts = GetTotalPlacedParts(speciesCounts);

        if (resultText == null)
            return;

        if (totalParts == 0)
        {
            resultText.text = emptyBuildResult;
            resultText.gameObject.SetActive(true);
            return;
        }

        Dictionary<string, float> speciesPercentages = BuildSpeciesPercentages(speciesCounts);
        string result = DetermineBuildResult(speciesPercentages);

        resultText.text = result;
        resultText.gameObject.SetActive(true);
    }

    private int GetTotalPlacedParts(Dictionary<string, int> speciesCounts)
    {
        int total = 0;

        foreach (KeyValuePair<string, int> kvp in speciesCounts)
            total += kvp.Value;

        return total;
    }

    private Dictionary<string, float> BuildSpeciesPercentages(Dictionary<string, int> speciesCounts)
    {
        Dictionary<string, float> percentages = new Dictionary<string, float>();

        foreach (KeyValuePair<string, int> kvp in speciesCounts)
            percentages[kvp.Key] = kvp.Value / 4f;

        return percentages;
    }

    private string DetermineBuildResult(Dictionary<string, float> speciesPercentages)
    {
        float bee = GetPercent(speciesPercentages, "Bee");
        float fly = GetPercent(speciesPercentages, "Fly");
        float wasp = GetPercent(speciesPercentages, "Wasp");

        if (Mathf.Approximately(bee, 1f))
            return pureBeeResult;

        if (Mathf.Approximately(fly, 1f))
            return pureFlyResult;

        if (Mathf.Approximately(wasp, 1f))
            return pureWaspResult;

        if (Mathf.Approximately(fly, 0.5f) && Mathf.Approximately(wasp, 0.5f))
            return flyWaspHybridResult;

        if (Mathf.Approximately(bee, 0.5f) && Mathf.Approximately(fly, 0.5f))
            return beeFlyHybridResult;

        if (Mathf.Approximately(bee, 0.5f) && Mathf.Approximately(wasp, 0.5f))
            return beeWaspHybridResult;

        if (Mathf.Approximately(bee, 0.75f))
            return beeDominant75Result;

        if (Mathf.Approximately(fly, 0.75f))
            return flyDominant75Result;

        if (Mathf.Approximately(wasp, 0.75f))
            return waspDominant75Result;

        if (Mathf.Approximately(fly, 0.5f))
            return flyDominant50Result;

        if (Mathf.Approximately(bee, 0.5f))
            return beeDominant50Result;

        if (Mathf.Approximately(wasp, 0.5f))
            return waspDominant50Result;

        return unusualSpecimenResult;
    }

    private float GetPercent(Dictionary<string, float> speciesPercentages, string speciesId)
    {
        return speciesPercentages.TryGetValue(speciesId, out float value) ? value : 0f;
    }

    public void ForceRefresh()
    {
        RecalculateBars(false);
        CacheCurrentParts();
    }

    public void ClearAllSlots()
    {
        if (headSlot != null) headSlot.ClearSlot();
        if (bodySlot != null) bodySlot.ClearSlot();
        if (legSlot != null) legSlot.ClearSlot();
        if (wingSlot != null) wingSlot.ClearSlot();

        if (resultText != null)
            resultText.gameObject.SetActive(false);

        RecalculateBars(false);
        CacheCurrentParts();
    }
}