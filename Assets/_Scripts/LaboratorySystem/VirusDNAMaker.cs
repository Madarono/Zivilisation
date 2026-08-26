using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SpriteRequirement
{
    public Sprite sprite;
    public int req;
}

public class VirusDNAMaker : MonoBehaviour
{
    public static VirusDNAMaker instance { get; private set; }

    public Gradient severityColors;

    [Header("Severity Visuals")]
    public SpriteRequirement[] severityVisuals;
    public SpriteRequirement[] infectionVisuals;
    public SpriteRequirement[] lethalityVisuals;

    [Header("Mimic")]
    public Image[] mimicOverlays;
    public Image[] mimicSpikes;
    public Image mimicCore;

    [Header("Virus")]
    public Image[] virusOverlays;
    public Image[] virusSpikes;
    public Image virusCore;

    void Awake()
    {
        instance = this;
    }

    public void SetVirusDNA(float infection, float severity, float lethality, bool mimic)
    {
        Image[] overlays = SetOverlays(mimic);
        Image[] spikes = SetSpikes(mimic);
        Image core = SetCore(mimic);

        //Infection
        int infectionVisualId = GetVisualIndex(infection, infectionVisuals);
        int prevIndex = Mathf.Max(infectionVisualId - 1, 0); 

        Sprite nextInfectionSprite = infectionVisuals[infectionVisualId].sprite;
        Sprite currentInfectionSprite = infectionVisuals[prevIndex].sprite;

        float tierProgress = GetTierProgress(infection, infectionVisualId);

        int spikeCount = spikes.Length;
        for (int i = 0; i < spikeCount; i++)
        {
            float spikeThreshold = (float)i / spikeCount;

            if (tierProgress >= spikeThreshold) 
            {
                spikes[i].sprite = nextInfectionSprite;
            }
            else 
            {
                spikes[i].sprite = currentInfectionSprite;
            }
        }

        //Lethality
        int lethalityVisualId = GetVisualIndex(lethality, lethalityVisuals);
        core.sprite = lethalityVisuals[lethalityVisualId].sprite;

        //Severity
        int severityVisualId = GetVisualIndex(severity, severityVisuals);

        foreach (var overlay in overlays)
        {
            overlay.color = severityColors.Evaluate(severity / 100f);
            overlay.sprite = severityVisuals[severityVisualId].sprite;
        }
    }

    float GetTierProgress(float value, int id)
    {
        if (id == 0) return 1f;

        float prevReq = infectionVisuals[id - 1].req;
        float targetReq = infectionVisuals[id].req;

        return Mathf.Clamp01((value - prevReq) / (targetReq - prevReq));
    }

    int GetVisualIndex(float value, SpriteRequirement[] spriteReq)
    {
        for (int i = 0; i < spriteReq.Length; i++)
        {
            if (value <= spriteReq[i].req) return i;
        }

        return spriteReq.Length - 1;
    }

    Image[] SetOverlays(bool mimic) => mimic ? mimicOverlays : virusOverlays;
    Image[] SetSpikes(bool mimic) => mimic ? mimicSpikes : virusSpikes;
    Image SetCore(bool mimic) => mimic ? mimicCore : virusCore;
}