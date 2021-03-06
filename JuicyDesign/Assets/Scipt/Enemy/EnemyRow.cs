﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRow : MonoBehaviour
{
    [SerializeField]
    private Enemy enemyPrefab;
    [SerializeField]
    private Bullet bullet;

    private EnemyManager enemyManager;

    private static bool moveRight = true;
    // Start is called before the first frame update
    void Start()
    {
        enemyManager = GetComponentInParent<EnemyManager>();
        Vector3 spawnPosition = transform.position;
        for (int i = 0; i < LevelManager.Instance.NbEnemyInRow; i++)
        {
            Enemy instance = Instantiate(enemyPrefab, transform);
            instance.transform.position = spawnPosition;
            spawnPosition.x += LevelManager.Instance.EnemySize.x;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float speed = LevelManager.Instance.EnemySpeed * Time.deltaTime;
        if (enemyManager.enemyGoDown)
            speed = Mathf.Sqrt((speed * speed) / 2);
        transform.Translate(moveRight ? speed : -speed, 0, 0);
    }

    private void LateUpdate()
    {
        if (transform.childCount == 0)
        {
            enemyManager.RemoveRow(this);
            Destroy(gameObject);
            return;
        }
        int childIndex = moveRight ? transform.childCount - 1 : 0;
        Vector2 position = transform.GetChild(childIndex).position;
        if(Physics2D.Raycast(position, moveRight ? Vector2.right : Vector2.left, LevelManager.Instance.EnemySpeed * Time.deltaTime, 1 << 8)){
            moveRight = !moveRight;
            enemyManager.GoDown();
        }
    }

    public void Shoot()
    {
        int random = Random.Range(0, transform.childCount);
        Transform randChild = transform.GetChild(random);
        Enemy enemy = randChild.GetComponent<Enemy>();

        if(LevelManager.Instance.activationInputs.Find(x => x.name == "Sounds").isActive)
            enemy.audioSource.PlayOneShot(enemy.shotSound);

        Bullet instance = Instantiate(bullet, randChild.position, Quaternion.Euler(0,0,180));
        instance.IsEnemyShoot();
    }
}
