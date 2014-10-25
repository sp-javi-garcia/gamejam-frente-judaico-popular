using UnityEngine;
using System.Collections;

public class HumanBase : MonoBehaviour
{
    public int RequiredZombies;

    bool _zombiesArrived = false;
    void OnTriggerEnter(Collider other)
    {
        if (!_zombiesArrived)
        {
            _zombiesArrived = true;
            Zombie zombie = other.gameObject.GetComponent<Zombie>();
            if (zombie != null)
            {
                GameController.Instance.ZombiesArrived();
            }
        }
    }
}
