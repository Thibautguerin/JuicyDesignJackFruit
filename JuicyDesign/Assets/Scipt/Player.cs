using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Bullet bullet;
    public Image imageCooldown;

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

    private bool activeSubmarine = false;
    private bool activeShake = false;
    private bool activeSounds = false;
    private bool activeRadar = false;
    private bool activeJuicyVFX = false;
    private Transform radar;

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

        motorAudioSource = transform.GetChild(0).GetComponent<AudioSource>();
        motorAudioSource.loop = true;
        motorAudioSource.clip = motorSound;

        radar = transform.GetChild(3);
        radar.parent = null;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");

        foreach (var item in LevelManager.Instance.activationInputs)
        {
            if (item.name == "Submarine")
                activeSubmarine = item.isActive;
            if (item.name == "Sounds")
                activeSounds = item.isActive;
            if (item.name == "Shake")
                activeShake = item.isActive;
            if (item.name == "Radar")
                activeRadar = item.isActive;
            if (item.name == "Juicy VFX")
                activeJuicyVFX = item.isActive;
        }

        transform.GetChild(2).gameObject.SetActive(activeJuicyVFX);
        if (radar != null)
        {
            radar.gameObject.SetActive(activeRadar);
            radar.position = transform.position;
        }

        if (activeSounds)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
            if (!motorAudioSource.isPlaying)
                motorAudioSource.Play();
        }
        else
        {
            audioSource.Stop();
            motorAudioSource.Stop();
        }

        if (canMove)
        {
            if (activeSubmarine)
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
            if (activeSounds)
                audioSource.PlayOneShot(shotSound);
            if (activeSubmarine)
            {
                imageCooldown.DOFillAmount(1, LevelManager.Instance.ShipCooldown).OnComplete(() =>
                {
                    imageCooldown.fillAmount = 0f;
                });
            }
            if (activeShake)
                LevelManager.Instance.CameraAnimator.SetTrigger("Shooting");
            if (activeSubmarine)
            {
                transform.DOMoveY(transform.position.y - 0.2f, 0.18f).OnComplete(() =>
                {
                    transform.DOMoveY(transform.position.y + 0.2f, 0.15f);
                });
            }
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
        if (activeSounds)
            audioSource.PlayOneShot(destructionSound);
        actualHp--;
        if (actualHp <= 0)
        {
            canMove = false;
            Destroy(radar.gameObject);
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            if (activeSubmarine)
            {
                transform.DOScale(0.01f, 2).OnComplete(() =>
                {
                    Destroy(gameObject);
                    LevelManager.Instance.gameObject.AddComponent(typeof(AudioListener));
                });
            }
            else
            {
                if (activeSounds)
                {
                    GetComponent<MeshRenderer>().enabled = false;
;                   Destroy(gameObject, 2);
                }
                else
                    Destroy(gameObject);
                LevelManager.Instance.gameObject.AddComponent(typeof(AudioListener));
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        actualSpeed = 0;
    }
}
