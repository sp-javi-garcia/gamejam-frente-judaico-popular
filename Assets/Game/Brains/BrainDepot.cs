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

    public List<Brain> Brains = new List<Brain>();
	public List<Brain> ActiveBrains = new List<Brain>();
    public Transform ConveyorBeltStartPoint;
    public Transform ConveyorBeltEndPoint;
    public float ConveyorSpeed;

    bool _startSequenceCompleted = false;

    public int MaxBrains = 6;
    public bool HandleTaps { get; set; }

    void Start()
    {
        Application.targetFrameRate = 60;
        StartCoroutine(StartSequence());
    }

    Vector3 GetFinalBrainConveyorPosition(int index)
    {
        return ConveyorBeltStartPoint.position + ((float) (index)) * (ConveyorBeltEndPoint.position - ConveyorBeltStartPoint.position) / (MaxBrains - 1);
    }

    public Vector3 GetBrainPositionByBrain(Brain brain)
    {
        return GetFinalBrainConveyorPosition(Brains.IndexOf(brain));
    }

    IEnumerator StartSequence()
    {
        for (int i = 0; i < StartingBrains; ++i)
        {
            Brain starterBrain = AddBrain();
            starterBrain.transform.position = GetFinalBrainConveyorPosition(i);
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

    Brain AddBrain()
    {
        if (Brains.Count < MaxBrains)
        {
            BrainPrefab brainPrefab = ChooseRandomPrefab();
            Brain brain = Brain.CreateBrain(brainPrefab, this, ConveyorBeltEndPoint.position);
            Brains.Add(brain);
            return brain;
        }
        return null;
    }

    Brain _selectedBrain;

    float _remainingTimeUntilNextBrain;
    public float BrainFallHeight = 18f;

    Vector3 GetBrainPositionByTouch()
    {
        Plane plane = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));
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

    bool _onMouseClick = false;
    void CheckBrainClicked()
    {
        // Editor part
        if (HandleTaps && Input.GetMouseButtonDown(0))
        {
            _onMouseClick = true;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 20f))
            {
                Brain clickedBrain = hit.collider.gameObject.GetComponent<Brain>();
                if (clickedBrain != null && clickedBrain.CanBeThrown)
                {
                    _selectedBrain = clickedBrain;
                    _selectedBrain.OnBrainPressed(GetBrainPositionByTouch());
                }
            }
        }
        if (Input.GetMouseButtonUp(0) && _selectedBrain != null)
        {
            _onMouseClick = false;
            if (_selectedBrain.OnBrainReleased(GetBrainPositionByTouch(), TouchInsideBrainDepotArea()))
            {
                Brains.Remove(_selectedBrain);

				_selectedBrain.transform.parent = null;
				ActiveBrains.Add(_selectedBrain);
            }
            _selectedBrain = null;
        }

        if (_selectedBrain != null && _onMouseClick)
        {
            _selectedBrain.OnBrainMoved(GetBrainPositionByTouch());
        }

        // touches part
        if (Input.touchCount == 1)
        {
            Touch touch = Input.touches[0];
            if (HandleTaps && touch.phase == TouchPhase.Began)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
                if (Physics.Raycast(ray, out hit, 20f))
                {
                    Brain clickedBrain = hit.collider.gameObject.GetComponent<Brain>();
                    if (clickedBrain != null && clickedBrain.CanBeThrown)
                    {
                        _selectedBrain = clickedBrain;
                        _selectedBrain.OnBrainPressed(GetBrainPositionByTouch());
                    }
                }
            }
            else if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                if (_selectedBrain != null)
                {
                    _selectedBrain.OnBrainMoved(GetBrainPositionByTouch());
                }
            }
            else if (touch.phase == TouchPhase.Ended && _selectedBrain != null)
            {
                if (_selectedBrain.OnBrainReleased(GetBrainPositionByTouch(), TouchInsideBrainDepotArea()))
                {
                    Brains.Remove(_selectedBrain);
                }
                _selectedBrain = null;
            }
        }
    }

    public void ResetSelectedBrain()
    {
        _selectedBrain = null;
    }

    void UpdateConveyorBeltBrainMovement()
    {
        for (int i = 0; i < Brains.Count; ++i)
        {
            Brain brain = Brains[i];
            Vector3 finalPosition = GetFinalBrainConveyorPosition(i);
            if (brain.transform.position != finalPosition)
            {
                float distance = Vector3.Distance(finalPosition, brain.transform.position);
                float movementAmount = ConveyorSpeed * Time.deltaTime;
                if (distance < movementAmount)
                {
                    brain.transform.position = finalPosition;
                }
                else
                {
                    Vector3 movementVector = Vector3.Normalize(finalPosition - brain.transform.position) * movementAmount;
                    brain.transform.position += movementVector;
                }
            }
        }
    }

    void Update ()
    {
        if (_startSequenceCompleted && Brains.Count < MaxBrains)
        {
            _timeUntilNextBrain -= Time.deltaTime;
            if (_timeUntilNextBrain <= 0f)
            {
                _timeUntilNextBrain = AddBrainTime;
                AddBrain();
            }
        }
        CheckBrainClicked();
        UpdateConveyorBeltBrainMovement();
    }
}
