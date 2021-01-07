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
    public GameObject explosion;

    private MeshRenderer meshRenderer;
    private MeshRenderer visibleRender;
    private Coroutine routine;
    private bool radarState;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject enemy;

    private AudioClip radarSound;
    private AudioClip alarmSound;
    private AudioSource audioSource;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        visibleRender = transform.GetChild(0).GetComponent<MeshRenderer>();
        if (LevelManager.Instance.activationInputs.Find(x => x.name == "Radar").isActive || !LevelManager.Instance.activationInputs.Find(x => x.name == "Juicy VFX").isActive)
        {
            enemy.SetActive(false);
            player.SetActive(false);
        }

        audioSource = GetComponent<AudioSource>();
        foreach (var item in LevelManager.Instance.sounds)
        {
            if (item.name == "Radar Sound")
                radarSound = item.sound;
            if (item.name == "Alarm Sound")
                alarmSound = item.sound;
        }
    }

    void Update()
    {
        if (!audioSource.enabled && LevelManager.Instance.activationInputs.Find(x => x.name == "Sounds").isActive)
            audioSource.enabled = true;
        else if (audioSource.enabled && !LevelManager.Instance.activationInputs.Find(x => x.name == "Sounds").isActive)
            audioSource.enabled = false;
        if(!LevelManager.Instance.activationInputs.Find(x => x.name == "Juicy VFX").isActive)
        {
            enemy.SetActive(false);
            player.SetActive(false);
        }

        if (direction == Direction.UP)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, LevelManager.Instance.ShipBulletSpeed);
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, -LevelManager.Instance.ShipBulletSpeed);
        }

        bool b = LevelManager.Instance.activationInputs.Find(x => x.name == "Radar").isActive;
        if (radarState != b)
        {
            if (routine != null)
                StopCoroutine(routine);
            radarState = b;
            Color color = meshRenderer.material.color;
            color.a = b ? 0 : 1;
            meshRenderer.material.color = color;
            color = visibleRender.material.color;
            color.a = b ? 0 : 1;
            visibleRender.material.color = color;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Scan" && LevelManager.Instance.activationInputs.Find(x => x.name == "Radar").isActive)
        {
            if (LevelManager.Instance.activationInputs.Find(x => x.name == "Sounds").isActive)
            {
                if (direction != Direction.UP)
                    audioSource.PlayOneShot(radarSound);
                if (direction != Direction.UP && (transform.position - collision.transform.position).magnitude <= 3)
                    collision.transform.parent.GetComponent<AudioSource>().PlayOneShot(alarmSound);
            }
            if (routine != null)
            {
                StopCoroutine(routine);
                routine = null;
            }
            Color color = meshRenderer.material.color;
            color.a = 1;
            meshRenderer.material.color = color;
            color = visibleRender.material.color;
            color.a = 1;
            visibleRender.material.color = color;
            if (!LevelManager.Instance.activationInputs.Find(x => x.name == "Juicy VFX").isActive)
            {
                if (direction == Direction.UP)
                    player.SetActive(true);
                else
                    enemy.SetActive(true);
            }
            return;
        }
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
                if(LevelManager.Instance.activationInputs.Find(x => x.name == "Shake").isActive)
                    LevelManager.Instance.CameraAnimator.SetTrigger("SubmarineHit");
            }
        }
       else if (collision.CompareTag("Enemy"))
        {
            if (direction == Direction.DOWN)
                return;
            collision.GetComponent<Enemy>().Destruction();
            if (LevelManager.Instance.activationInputs.Find(x => x.name == "Shake").isActive)
                LevelManager.Instance.CameraAnimator.SetTrigger("ShipHit");
        }

        Destroy(gameObject);

        if (LevelManager.Instance.activationInputs.Find(x => x.name == "Juicy VFX").isActive)
        {
            Vector3 position = transform.position + Vector3.forward * -1;
            GameObject instance = Instantiate(explosion, position, Quaternion.identity);
            Destroy(instance, 1.5f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Scan" && LevelManager.Instance.activationInputs.Find(x => x.name == "Radar").isActive)
        {
            Color color = meshRenderer.material.color;
            color.a = 1;
            meshRenderer.material.color = color;
            color = visibleRender.material.color;
            color.a = 1;
            visibleRender.material.color = color;
            routine = StartCoroutine(fadeRoutine());
        }
    }

    public void IsEnemyShoot()
    {
        if (LevelManager.Instance.activationInputs.Find(x => x.name == "Juicy VFX").isActive)
        {
            enemy.SetActive(true);
            player.SetActive(false);
        }
        direction = Direction.DOWN;
    }

    IEnumerator fadeRoutine()
    {
        float maxTime = LevelManager.Instance.RadarFadeDuration;
        float timer = 0;
        while (timer < maxTime)
        {
            timer += 0.1f;
            yield return new WaitForSeconds(0.1f);
            Color color = meshRenderer.material.color;
            color.a = Mathf.Lerp(0, 1, 1 - (timer / maxTime));
            meshRenderer.material.color = color;
            color = visibleRender.material.color;
            color.a = Mathf.Lerp(0, 1, 1 - (timer / maxTime));
            visibleRender.material.color = color;
        }
        enemy.SetActive(false);
        player.SetActive(false);
        routine = null;
    }
}
