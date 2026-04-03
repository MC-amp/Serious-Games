using System;
using UnityEngine;
using UnityEngine.UI;

public class TraitToggle : MonoBehaviour
{
    [Serializable]
    public struct SpeciesWeight
    {
        public string speciesId;
        public float weight;
    }

    [SerializeField] private Toggle toggle;
    [SerializeField] private SpeciesWeight[] weights;

    public Toggle Toggle => toggle;
    public SpeciesWeight[] Weights => weights;
}