using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private EnemyRow enemyRowPrefab;

    private float targetY;
    private bool goDown = false;

    public bool enemyGoDown { get => goDown;}
    public void GoDown()
    {
        goDown = true;
        targetY = transform.position.y - LevelManager.Instance.EnemyYOffset;
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector2 spawnPosition = transform.position;
        for (int i = 0; i < LevelManager.Instance.NbRow; i++)
        {
            EnemyRow instance = Instantiate(enemyRowPrefab, transform);
            instance.transform.position = spawnPosition;
            spawnPosition.y -= LevelManager.Instance.EnemySize.y;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (goDown)
        {
            float speed = LevelManager.Instance.EnemySpeed * Time.deltaTime;
            speed = Mathf.Sqrt((speed * speed) / 2);
            transform.Translate(0, -speed, 0);
            if (transform.position.y < targetY)
                goDown = false;
        }

    }
}
