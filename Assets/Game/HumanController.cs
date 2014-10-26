using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HumanController : MonoBehaviour
{
    Animator _animator;
    public Vector3 RunAwayVector;
    public float RunAwayTimer;
    bool _isRunningAway;
    float kZombieRangeSqrt = 35f;
    // Use this for initialization
    void Start ()
    {
        _animator = GetComponentInChildren<Animator>();
        _animator.SetFloat("speed", 0.3f);
    }

    void CheckForZombies()
    {
        List<Zombie> zombies = ZombieSquad.Instance.Zombies;
        for (int i = 0; i < zombies.Count; ++i)
        {
            if (Vector3.SqrMagnitude(transform.position - zombies[i].transform.position) < kZombieRangeSqrt)
            {
                _isRunningAway = true;
                _animator.SetFloat("speed", 1f);
            }
        }
    }

    float _remainingTimeUntilCheck = 0.5f;
    void Update ()
    {
        if (!_isRunningAway)
        {
            _remainingTimeUntilCheck -= Time.deltaTime;
            if (_remainingTimeUntilCheck <= 0f)
            {
                _remainingTimeUntilCheck = 0.5f;
                CheckForZombies();
            }
        }
        else
        {
            RunAwayTimer -= Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(RunAwayVector);
            transform.position += RunAwayVector * Time.deltaTime;
            if (RunAwayTimer <= 0f)
                Destroy(gameObject);
        }
    }
}
