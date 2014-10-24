using UnityEngine;
using System.Collections;

public class Brain : MonoBehaviour
{
    public float Range = 15f;
    public float Duration = 30f;

    public enum State
    {
        APPEARING,
        WAITING,
        DRAGGING,
        FALLING,
        IDLE,
        DISAPPEARING
    }

    State _state;
    GameObject _brainSpot;

    public static Brain CreateBrain(BrainPrefab brainPrefab, GameObject brainSpot)
    {
        GameObject brainGO = (GameObject)GameObject.Instantiate(Resources.Load(brainPrefab.Path));
        Brain brain = brainGO.GetComponent<Brain>();
        brain.Init(brainSpot);
        return brain;
    }

    void Init(GameObject brainSpot)
    {
        SetAppearing();
        _brainSpot = brainSpot;
        transform.parent = _brainSpot.transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update ()
    {
        switch(_state)
        {
        case State.APPEARING:
            AppearingBehavior();
            break;
        case State.IDLE:
            IdleBehavior();
            break;
        case State.DISAPPEARING:
            DisappearingBehavior();
            break;
        case State.FALLING:
            FallingBehavior();
            break;
        }
    }

    const float kAppearTime = 0.5f;
    float _appearingRemainingTime;
    void SetAppearing()
    {
        _state = State.APPEARING;
        transform.localScale = Vector3.zero;
        iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.one,
                                               "easetype", iTween.EaseType.easeOutBack,
                                               "time", kAppearTime));
        _appearingRemainingTime = kAppearTime;
    }

    void SetDisappearing()
    {
        _state = State.DISAPPEARING;
    }

    void SetIdle()
    {
        _state = State.IDLE;
    }

    void SetFalling()
    {
        _state = State.FALLING;
    }

    void AppearingBehavior()
    {
        _appearingRemainingTime -= Time.deltaTime;
        if (_appearingRemainingTime <= 0f)
        {
            SetIdle();
        }
    }

    void DisappearingBehavior()
    {
    }

    void FallingBehavior()
    {
    }

    void IdleBehavior()
    {
    }
}
