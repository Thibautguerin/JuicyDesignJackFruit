using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private EnemyRow enemyRowPrefab;

    private float targetY;
    private bool goDown = false;
    private List<EnemyRow> enemyRows = new List<EnemyRow>();

    public bool enemyGoDown { get => goDown;}
    public void GoDown()
    {
        goDown = true;
        targetY = transform.position.y - LevelManager.Instance.EnemyYOffset;
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector3 spawnPosition = transform.position;
        for (int i = 0; i < LevelManager.Instance.NbRow; i++)
        {
            EnemyRow instance = Instantiate(enemyRowPrefab, transform);
            enemyRows.Add(instance);
            instance.transform.position = spawnPosition;
            spawnPosition.y -= LevelManager.Instance.EnemySize.y;
        }
        StartCoroutine(ShootRoutine());
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

    IEnumerator ShootRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(LevelManager.Instance.EnemyBulletCD);
            enemyRows[enemyRows.Count - 1].Shoot();
        }
    }

    public void RemoveRow(EnemyRow rowToRemove)
    {
        enemyRows.Remove(rowToRemove);
    }
}
