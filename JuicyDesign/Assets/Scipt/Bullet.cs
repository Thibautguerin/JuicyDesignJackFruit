using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum Direction
    {
        UP,
        DOWN
    }

    public Direction direction = Direction.UP;

    void Update()
    {
        bool activeBulletEffects = false;

        foreach (var item in LevelManager.Instance.activationInputs)
        {
            if (item.name == "Bullet Effect")
                activeBulletEffects = item.isActive;
        }

        if (direction == Direction.UP)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, LevelManager.Instance.ShipBulletSpeed);
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, -LevelManager.Instance.ShipBulletSpeed);
        }

        if (activeBulletEffects)
        {
            float randRotation = Random.Range(LevelManager.Instance.ShipBulletRotSpeedMin, LevelManager.Instance.ShipBulletRotSpeedMax);
            gameObject.transform.Rotate(Vector2.up, randRotation);
            gameObject.transform.GetChild(0).GetComponent<TrailRenderer>().enabled = true;
        }
        else
        {
            gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
            gameObject.transform.GetChild(0).GetComponent<TrailRenderer>().enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Scan")
            return;
        if (collision.CompareTag("Player"))
        {
            if (direction == Direction.UP)
                return;

            Player player = collision.GetComponent<Player>();
            if (player)
            {
                player.TakeDamage();
            }
        }
        else if (collision.CompareTag("Enemy"))
        {
            if (direction == Direction.DOWN)
                return;
            Destroy(collision.gameObject);
        }

        Destroy(gameObject);
    }
}
