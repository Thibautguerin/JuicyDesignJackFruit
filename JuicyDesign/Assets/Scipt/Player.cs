using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public Bullet bullet;

    private float actualHp;
    private float actualSpeed;
    private float startCooldown = 0f;
    private bool canShoot = true;

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

            gameObject.transform.rotation = Quaternion.Euler(0, 180-horizontal * LevelManager.Instance.ShipMaxRotationY, horizontal * LevelManager.Instance.ShipMaxRotationZ);
        }
        else
        {
            actualSpeed = horizontal * LevelManager.Instance.ShipSpeed;
            gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(actualSpeed, 0);

        if (Input.GetKeyDown(KeyCode.Space) && canShoot)
        {
            LevelManager.Instance.CameraAnimator.SetTrigger("Shooting");
            transform.DOMoveY(transform.position.y - 0.7f, 0.18f).OnComplete(() =>
            {
                transform.DOMoveY(transform.position.y + 0.7f, 0.08f);
            });
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
        if (actualHp <= 0)
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        actualSpeed = 0;
    }
}
