﻿using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public const float SM_EXECUTE_RATE = 1F;

    [Header("Params")]
    [SerializeField]
    private float maxHP;

    [SerializeField]
    private string enemyKey;

    [Header("State distances")]
    [SerializeField]
    private float warningDistance = 50F;

    [SerializeField]
    private float attackDistance = 25F;

    [Header("Shoot")]
    [SerializeField]
    private float timeToShoot = 2F;

    [SerializeField]
    private Transform spawnLocation;

    [SerializeField]
    private float shootForce = 20F;

    private float currentHP;

    private Guid guid;

    private EnemyStateMachine stateMachine;

    public float TimeToShoot { get => timeToShoot; }
    public float WarningDistance { get => warningDistance; }
    public float AttackDistance { get => attackDistance; }
    public Transform SpawnLocation { get => spawnLocation; }
    public float ShootForce { get => shootForce; }

    private void Start()
    {
        if (PersistentData.Instance.LoadEnemyDeath(enemyKey))
        {
            Destroy(gameObject);
        }
        else
        {
            currentHP = maxHP;
            stateMachine = new EnemyStateMachine(this);
            InvokeRepeating("ExecuteSM", 0F, SM_EXECUTE_RATE);
        }
    }

    [ContextMenu("SetEnemyID")]
    private void SetEnemyID()
    {
        if (!Guid.TryParse(enemyKey.ToString(), out guid))
        {
            guid = Guid.NewGuid();
            enemyKey = guid.ToString();
        }
    }

    private void ExecuteSM()
    {
        stateMachine.Execute(Vector3.Distance(transform.position, PlayerController.Instance.transform.position));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Bullet")))
        {
            currentHP -= 1;
            PlayerController.Instance.UpdateScore();

            if (currentHP <= 0)
            {
                if (!string.IsNullOrEmpty(enemyKey))
                {
                    PersistentData.Instance.SaveEnemyDeath(enemyKey);
                }

                Destroy(gameObject);
            }
        }
    }
}