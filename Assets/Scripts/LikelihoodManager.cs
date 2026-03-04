using System.Collections.Generic;
using UnityEngine;

public class LikelihoodManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TraitToggle[] traits;
    [SerializeField] private SpeciesBar[] speciesBars;

    private Dictionary<string, SpeciesBar> _barById;
    private Dictionary<string, float> _maxScoreBySpecies;

    private void Awake()
    {
        BuildLookups();
        HookTraitEvents();
        RecalculateAndApply();
    }

    private void OnDestroy()
    {
        UnhookTraitEvents();
    }

    private void BuildLookups()
    {
        _barById = new Dictionary<string, SpeciesBar>();
        foreach (var bar in speciesBars)
        {
            if (bar == null || string.IsNullOrWhiteSpace(bar.SpeciesId)) continue;
            _barById[bar.SpeciesId] = bar;
        }

        _maxScoreBySpecies = new Dictionary<string, float>();
        foreach (var trait in traits)
        {
            if (trait == null || trait.Weights == null) continue;

            foreach (var w in trait.Weights)
            {
                if (string.IsNullOrWhiteSpace(w.speciesId)) continue;
                if (!_maxScoreBySpecies.ContainsKey(w.speciesId))
                    _maxScoreBySpecies[w.speciesId] = 0f;

                _maxScoreBySpecies[w.speciesId] += Mathf.Max(0f, w.weight);
            }
        }

        foreach (var kvp in _barById)
        {
            if (!_maxScoreBySpecies.ContainsKey(kvp.Key))
                _maxScoreBySpecies[kvp.Key] = 1f;
        }
    }

    private void HookTraitEvents()
    {
        foreach (var trait in traits)
        {
            if (trait == null || trait.Toggle == null) continue;
            trait.Toggle.onValueChanged.AddListener(_ => RecalculateAndApply());
        }
    }

    private void UnhookTraitEvents()
    {
        foreach (var trait in traits)
        {
            if (trait == null || trait.Toggle == null) continue;
            trait.Toggle.onValueChanged.RemoveListener(_ => RecalculateAndApply());
        }
    }

    public void RecalculateAndApply()
    {
        var scoreBySpecies = new Dictionary<string, float>();

        foreach (var speciesId in _maxScoreBySpecies.Keys)
            scoreBySpecies[speciesId] = 0f;

        foreach (var trait in traits)
        {
            if (trait == null || trait.Toggle == null) continue;
            if (!trait.Toggle.isOn) continue;

            foreach (var w in trait.Weights)
            {
                if (string.IsNullOrWhiteSpace(w.speciesId)) continue;
                if (!scoreBySpecies.ContainsKey(w.speciesId))
                    scoreBySpecies[w.speciesId] = 0f;

                scoreBySpecies[w.speciesId] += w.weight;
            }
        }

        foreach (var kvp in _barById)
        {
            string speciesId = kvp.Key;
            SpeciesBar bar = kvp.Value;

            float score = scoreBySpecies.TryGetValue(speciesId, out var s) ? s : 0f;
            float max = _maxScoreBySpecies.TryGetValue(speciesId, out var m) ? m : 1f;

            float fill01 = (max <= 0f) ? 0f : Mathf.Clamp01(score / max);
            bar.SetFill01(fill01);
        }
    }
}