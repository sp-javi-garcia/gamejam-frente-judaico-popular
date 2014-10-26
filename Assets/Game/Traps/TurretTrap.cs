using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurretTrap : MonoBehaviour
{
    public bool DealRealDamage = true;
    public float TimeBetweenAttacks = 1f;
    public float Range = 10;
    public GameObject ShootingCharacter;
    float _remainingReloadTime;
    public GameObject ReceiveDamagePrefab;
    public GameObject ShootPrefab;

	void Start ()
    {
        _remainingReloadTime = TimeBetweenAttacks;
	}

    Zombie ChooseZombieTarget()
    {
        List<Zombie> zombies = ZombieSquad.Instance.Zombies;
        List<Zombie> zombiesInRange = new List<Zombie>();
        for (int i = 0; i < zombies.Count; ++i)
        {
            float distance = Vector3.Distance(zombies[i].transform.position, transform.position);
            if (distance <= Range)
                zombiesInRange.Add(zombies[i]);
        }
        if (zombiesInRange.Count > 0)
            return zombiesInRange[Random.Range(0, zombiesInRange.Count)];
        else
            return null;
    }
	
    bool _inAttack = false;
    Zombie _currentTarget;
    IEnumerator Attack()
    {
        _currentTarget = ChooseZombieTarget();
        if (_currentTarget != null)
        {
            //Time to rotate...
            yield return new WaitForSeconds(0.3f);
            Instantiate(ShootPrefab, ShootingCharacter.transform.position + ShootingCharacter.transform.forward * 1f, ShootingCharacter.transform.rotation);
            //Shot animation
            //yield return new WaitForSeconds(0.1f);
            GameObject fx = (GameObject)Instantiate(ReceiveDamagePrefab, _currentTarget.transform.position + Vector3.up * 2f, _currentTarget.transform.rotation);
            fx.transform.parent = _currentTarget.gameObject.transform;
            if (DealRealDamage)
            {
                _currentTarget.Life -= 1;
                if (_currentTarget.Life <= 0)
                    _currentTarget.ProcessDie();
            }
            _remainingReloadTime = TimeBetweenAttacks;
            _currentTarget = null;
        }
        _inAttack = false;
    }

	void Update ()
    {
        _remainingReloadTime -= Time.deltaTime;
        if (_remainingReloadTime <= 0f && !_inAttack)
        {
            _inAttack = true;
            StartCoroutine(Attack());
        }
        if (_currentTarget != null)
        {
            Vector3 tempPosition = new Vector3(ShootingCharacter.transform.position.x, _currentTarget.transform.position.y, ShootingCharacter.transform.position.z);
            ShootingCharacter.transform.rotation = Quaternion.Lerp(ShootingCharacter.transform.rotation, Quaternion.LookRotation(_currentTarget.transform.position - tempPosition), 0.3f);
        }
	}
}
