using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Bullet bullet;

    private float actualHp;
    private float startCooldown = 0f;
    private bool canShoot = true;

    private void Start()
    {
        actualHp = LevelManager.Instance.ShipHp;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");

        if (horizontal != 0)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(horizontal * LevelManager.Instance.ShipSpeed, 0);
        }

        if (Input.GetKeyDown(KeyCode.Space) && canShoot)
        {
            Instantiate(bullet, transform.position, Quaternion.identity);
            canShoot = false;
            startCooldown = Time.time;
        }

        if (canShoot == false && Time.time > startCooldown + LevelManager.Instance.ShipCooldown)
        {
            canShoot = true;
        }
    }

    public void TakeDamage()
    {
        actualHp--;
    }
}
