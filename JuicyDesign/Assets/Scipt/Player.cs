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
    private bool canMove = true;

    private AudioClip shotSound;
    private AudioClip destructionSound;
    private AudioClip movementSound;
    private AudioClip motorSound;

    private AudioSource audioSource;
    private AudioSource motorAudioSource;

    private void Start()
    {
        actualHp = LevelManager.Instance.ShipHp;
        audioSource = GetComponent<AudioSource>();

        foreach (var item in LevelManager.Instance.sounds)
        {
            if (item.name == "Shot Sound")
                shotSound = item.sound;
            if (item.name == "Destruction Sound")
                destructionSound = item.sound;
            if (item.name == "Movement Sound")
                movementSound = item.sound;
            if (item.name == "Motor Sound")
                motorSound = item.sound;
        }

        audioSource.loop = true;
        audioSource.clip = movementSound;
        audioSource.Play();

        motorAudioSource = transform.GetChild(0).GetComponent<AudioSource>();
        motorAudioSource.loop = true;
        motorAudioSource.clip = motorSound;
        motorAudioSource.Play();
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

        if (canMove)
        {
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

                gameObject.transform.rotation = Quaternion.Euler(0, 180 - horizontal * LevelManager.Instance.ShipMaxRotationY, horizontal * LevelManager.Instance.ShipMaxRotationZ);
            }
            else
            {
                actualSpeed = horizontal * LevelManager.Instance.ShipSpeed;
                gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(actualSpeed, 0);
        }

        if (Input.GetKeyDown(KeyCode.Space) && canShoot && canMove)
        {
            audioSource.PlayOneShot(shotSound);
            LevelManager.Instance.CameraAnimator.SetTrigger("Shooting");
            transform.DOMoveY(transform.position.y - 0.2f, 0.18f).OnComplete(() =>
            {
                transform.DOMoveY(transform.position.y + 0.2f, 0.15f);
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
        audioSource.PlayOneShot(destructionSound);
        actualHp--;
        if (actualHp <= 0)
        {
            canMove = false;
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            transform.DOScale(0.01f, 2).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        actualSpeed = 0;
    }
}
