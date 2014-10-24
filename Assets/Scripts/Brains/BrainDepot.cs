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

    Brain _selectedBrain;

    float _remainingTimeUntilNextBrain;
    public float BrainFallHeight = 18f;

    Vector3 GetBrainPositionByTouch()
    {
        Plane plane = new Plane(Vector3.up, new Vector3(0f, BrainFallHeight, 0f));
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }

    bool TouchInsideBrainDepotArea()
    {
        return (Input.mousePosition.y / Screen.height) < 0.1f;
    }

    void CheckBrainClicked()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 20f))
            {
                Brain clickedBrain = hit.collider.gameObject.GetComponent<Brain>();
                if (clickedBrain != null)
                {
                    _selectedBrain = clickedBrain;
                    _selectedBrain.OnBrainPressed(GetBrainPositionByTouch());
                }
            }
        }
        if (Input.GetMouseButtonUp(0) && _selectedBrain != null)
        {
            if (_selectedBrain.OnBrainReleased(GetBrainPositionByTouch(), TouchInsideBrainDepotArea()))
            {
                Brains.Remove(_selectedBrain);
            }
            _selectedBrain = null;
        }

        if (_selectedBrain != null)
        {
            _selectedBrain.OnBrainMoved(GetBrainPositionByTouch());
        }
    }

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
        CheckBrainClicked();
    }
}
