using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour
{

    private MeshRenderer meshRenderer;
    private MeshRenderer visibleRender;
    private Coroutine routine;
    private bool radarState;
    private bool goRight = true;

    [SerializeField]
    private float maxXOffset = 0.5f;
    [SerializeField]
    private float offsetSpeed = 0.1f;

    [HideInInspector]
    public AudioClip shotSound;
    [HideInInspector]
    public AudioSource audioSource;

    private AudioClip destructionSound;
    private AudioClip radarSound;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        visibleRender = transform.GetChild(0).GetComponent<MeshRenderer>();
        StartCoroutine(DirectionRoutine());

        audioSource = GetComponent<AudioSource>();

        foreach (var item in LevelManager.Instance.sounds)
        {
            if (item.name == "Shot Sound")
                shotSound = item.sound;
            if (item.name == "Destruction Sound")
                destructionSound = item.sound;
            if (item.name == "Radar Sound")
                radarSound = item.sound;
        }
    }

    private void Update()
    {
        bool b = LevelManager.Instance.activationInputs.Find(x => x.name == "Radar").isActive;
        if(radarState!= b)
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

        transform.Translate((goRight ? offsetSpeed : -offsetSpeed) * Time.deltaTime, 0, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Scan" && LevelManager.Instance.activationInputs.Find(x => x.name == "Radar").isActive)
        {
            audioSource.PlayOneShot(radarSound, 0.4f);
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

    public void Destruction()
    {
        audioSource.PlayOneShot(destructionSound);
        transform.DORotate(new Vector3(0, 180, 180), 1.5f);
        transform.DOScale(0.01f, 2).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    IEnumerator fadeRoutine()
    {
        float maxTime = LevelManager.Instance.RadarFadeDuration;
        float timer = 0;
        while (timer< maxTime)
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

    IEnumerator DirectionRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            float xOffset = LevelManager.Instance.EnemySize.x * transform.GetSiblingIndex() - transform.localPosition.x;
            if (goRight && xOffset > 0 || !goRight && xOffset < 0)
            {
                float random = Random.Range(0f, 1f);
                if (random > 1 - Mathf.Abs(xOffset) / maxXOffset)
                    goRight = !goRight;
            }
        }
    }

}
