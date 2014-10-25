using UnityEngine;
using System.Collections;

public class MineTrap : MonoBehaviour
{
    public enum State
    {
        APPEARING,
        IDLE,
        EXPLODING
    }

    public float Range = 20f;
    public float DamageRange = 1.3f;
    public float InstaKillRange = 0.5f;
    public float MaxPushForce = 10f;
    public GameObject MineGO;

    State _state;

    void OnTriggerEnter(Collider other)
    {
        if (_state == State.IDLE)
        {
            Zombie zombie = other.gameObject.GetComponent<Zombie>();
            if (zombie != null)
            {
                SetExploding(zombie);
            }
        }
    }

    void Awake()
    {
        _state = State.IDLE;
    }

    public void Spawn()
    {
        SetAppearing();
    }

    // Update is called once per frame
    void Update ()
    {
        switch(_state)
        {
        case State.IDLE:

            break;
        }
    }

    void SetAppearing()
    {
        _state = State.APPEARING;
    }

    void SetExploding(Zombie zombie)
    {
        _state = State.EXPLODING;
        StartCoroutine(Explode());
    }

    void DamageZombies()
    {
        Zombie[] zombies = ZombieSquad.Instance.Zombies;
        for (int i = 0; i < zombies.Length; ++i)
        {
            Zombie zombie = zombies[i];
            float distance = Vector3.Distance(zombie.transform.position, transform.position);
            if (distance < Range)
            {
                zombie.OnBeingPushed(transform.position, MaxPushForce, Range);
                if (distance < DamageRange)
                {
                    zombie.Life -= 1;
                    // TODO: damage the zombie
                }
                if (distance < InstaKillRange)
                {
                    zombie.InstaKill();
                }
            }
        }
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(0.5f);
        // TODO: Explosion FX
        MineGO.SetActive(false);
        DamageZombies();
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    void IdleBehavior()
    {

    }
}
