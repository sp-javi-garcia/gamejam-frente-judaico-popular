using UnityEngine;
using System.Collections;

public class WindController : MonoBehaviour
{
    public static Vector3 WindForce;
    public float TimeBetweenWindChanges = 10f;
    public float MaxWindForce = 2f;
    public GameObject Arrow;
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
        float windForceMagnitude = Random.Range(0f, MaxWindForce);
        WindForce = WindForce * windForceMagnitude;
        UI3dController.Instance.WindSpeedText.text = ((int)windForceMagnitude).ToString() + "km/h";
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
        Arrow.transform.localRotation = Quaternion.Lerp(Arrow.transform.localRotation, Quaternion.LookRotation(WindForce), 0.1f);

    }
}
