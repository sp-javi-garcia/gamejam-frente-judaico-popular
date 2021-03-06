﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (AudioSource))]
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
    public GameObject ExplosionPrefab;

	[SerializeField]
	List<AudioClip> Explosions;

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
        List<Zombie> zombies = ZombieSquad.Instance.Zombies;
        for (int i = 0; i < zombies.Count; ++i)
        {
            Zombie zombie = zombies[i];
            float distance = Vector3.Distance(zombie.transform.position, transform.position);
            if (distance < Range)
            {
				int lifesToKill = 0;

				if (distance < DamageRange)
				{
					lifesToKill = 1;
					// TODO: damage the zombie
				}
				if (distance < InstaKillRange)
				{
					lifesToKill = 3;
				}

                zombie.OnBeingPushed(transform.position, MaxPushForce, lifesToKill, Range);
            }
        }
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject fx = (GameObject)Instantiate(ExplosionPrefab, transform.position + Vector3.up * 2f, transform.rotation);
        // TODO: Explosion FX
		AudioSource audio = GetComponent<AudioSource>();
		AudioClip clip = Explosions[Random.Range(0, Explosions.Count)];
		audio.clip = clip;
		audio.Play();

        MineGO.SetActive(false);
        DamageZombies();
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    void IdleBehavior()
    {

    }
}
