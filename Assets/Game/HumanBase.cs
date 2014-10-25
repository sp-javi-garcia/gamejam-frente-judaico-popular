using UnityEngine;
using System.Collections;

public class HumanBase : MonoBehaviour
{
    public int RequiredZombies;

    bool _zombiesArrived = false;
    void OnTriggerEnter(Collider other)
    {
        Zombie zombie = other.gameObject.GetComponent<Zombie>();
        if (zombie != null)
        {
            if (!_zombiesArrived)
            {
                _zombiesArrived = true;
                GameController.Instance.ZombiesArrived();
            }
            // TODO: make the zombie attack or something
            //zombie.EatBrain();
        }
    }
}
