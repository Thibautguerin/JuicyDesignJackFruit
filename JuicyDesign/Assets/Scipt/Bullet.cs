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

    private MeshRenderer meshRenderer;
    private MeshRenderer visibleRender;
    private Coroutine routine;
    private bool radarState;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        visibleRender = transform.GetChild(1).GetComponent<MeshRenderer>();
    }

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
            if(direction == Direction.UP)
                gameObject.transform.Rotate(Vector2.up, randRotation);
            else
                gameObject.transform.Rotate(Vector2.down, randRotation);
            gameObject.transform.GetChild(0).GetComponent<TrailRenderer>().enabled = true;
        }
        else
        {
            if(direction == Direction.UP)
                gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
            else
                gameObject.transform.rotation = Quaternion.Euler(0,0,180);
            gameObject.transform.GetChild(0).GetComponent<TrailRenderer>().enabled = false;
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
                LevelManager.Instance.CameraAnimator.SetTrigger("SubmarineHit");
            }
        }
       else if (collision.CompareTag("Enemy"))
        {
            if (direction == Direction.DOWN)
                return;
            Destroy(collision.gameObject);
            LevelManager.Instance.CameraAnimator.SetTrigger("ShipHit");
        }

        Destroy(gameObject);
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
        routine = null;
    }
}
