using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BrainPrefab
{
    public string Path;
    public float Chance;
}

public class BrainDepot : MonoBehaviour
{
    public List<BrainPrefab> BrainPrefabs;
    public int StartingBrains;
    float _timeUntilNextBrain;
    public float AddBrainTime = 10f;
    float kDelayBetweenStartBrains = 0.5f;

    public List<GameObject> BrainSpots;
    public List<Brain> Brains = new List<Brain>();

    bool _startSequenceCompleted = false;

    void Start()
    {
        StartCoroutine(StartSequence());
    }

    IEnumerator StartSequence()
    {
        for (int i = 0; i < StartingBrains; ++i)
        {
            AddBrain();
            yield return new WaitForSeconds(kDelayBetweenStartBrains);
        }
        _timeUntilNextBrain = AddBrainTime;
        _startSequenceCompleted = true;
    }

    BrainPrefab ChooseRandomPrefab()
    {
        float maxChance = 0f;
        for (int i = 0; i < BrainPrefabs.Count; ++i)
        {
            maxChance += BrainPrefabs[i].Chance;
        }

        float chance = Random.Range(0f, maxChance);
        float accumChance = 0f;
        for (int i = 0 ; i < BrainPrefabs.Count; ++i)
        {
            accumChance += BrainPrefabs[i].Chance;
            if (chance < accumChance)
                return BrainPrefabs[i];
        }
        return BrainPrefabs[0];
    }

    void AddBrain()
    {
        if (Brains.Count < BrainSpots.Count)
        {
            BrainPrefab brainPrefab = ChooseRandomPrefab();
            Brain brain = Brain.CreateBrain(brainPrefab, BrainSpots[Brains.Count]);
            Brains.Add(brain);
        }
    }

    float _remainingTimeUntilNextBrain;

    void Update ()
    {
        if (_startSequenceCompleted && Brains.Count < BrainSpots.Count)
        {
            _timeUntilNextBrain -= Time.deltaTime;
            if (_timeUntilNextBrain <= 0f)
            {
                _timeUntilNextBrain = AddBrainTime;
                AddBrain();
            }
        }
    }
}
