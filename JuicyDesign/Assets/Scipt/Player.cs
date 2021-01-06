using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Bullet bullet;

    private float actualHp;
    private float actualSpeed;
    private float startCooldown = 0f;
    private bool canShoot = true;
        
    private float lastRotation = 0;

    private void Start()
    {
        actualHp = LevelManager.Instance.ShipHp;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        bool activeInertia = false;

        foreach (var item in LevelManager.Instance.activationInputs)
        {
            if (item.name == "Player Inertia")
                activeInertia = item.isActive;
        }

        if (activeInertia)
        {
            if (horizontal > 0)
            {
                if (actualSpeed < LevelManager.Instance.ShipSpeed)
                    actualSpeed += LevelManager.Instance.ShipAcceleration * Time.deltaTime;
            }
            else if (horizontal < 0)
            {
                if (actualSpeed > -LevelManager.Instance.ShipSpeed)
                    actualSpeed -= LevelManager.Instance.ShipAcceleration * Time.deltaTime;
            }
            else
            {
                if (actualSpeed > 0)
                    actualSpeed -= LevelManager.Instance.ShipDeceleration * Time.deltaTime;
                else if (actualSpeed < 0)
                    actualSpeed += LevelManager.Instance.ShipDeceleration * Time.deltaTime;
            }

            gameObject.transform.Rotate(0, 0, -horizontal * LevelManager.Instance.ShipMaxRotation - lastRotation);
            lastRotation = -horizontal * LevelManager.Instance.ShipMaxRotation;
        }
        else
        {
            actualSpeed = horizontal * LevelManager.Instance.ShipSpeed;
        }

        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(actualSpeed, 0);

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        actualSpeed = 0;
    }
}
