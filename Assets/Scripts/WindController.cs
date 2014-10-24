using UnityEngine;
using System.Collections;

public class WindController : MonoBehaviour
{
    public static Vector3 WindForce;
    public float TimeBetweenWindChanges = 10f;
    public float MaxWindForce = 2f;
    float _remainingTimeUntilWindChange;

    void Start()
    {
        _remainingTimeUntilWindChange = TimeBetweenWindChanges;
        ChangeWindForce();
    }

    void ChangeWindForce()
    {
        WindForce = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        WindForce.Normalize();
        WindForce = WindForce * MaxWindForce; Random.Range(0f, MaxWindForce);
    }

    // Update is called once per frame
    void Update ()
    {
        _remainingTimeUntilWindChange -= Time.deltaTime;
        if (_remainingTimeUntilWindChange <= 0f)
        {
            _remainingTimeUntilWindChange = TimeBetweenWindChanges;
            ChangeWindForce();
        }
    }
}
