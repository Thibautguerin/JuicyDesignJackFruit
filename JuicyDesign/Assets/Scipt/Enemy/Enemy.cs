using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    private MeshRenderer meshRenderer;
    private Coroutine routine;
    private bool radarState;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
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
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Scan" && LevelManager.Instance.activationInputs.Find(x => x.name == "Radar").isActive)
        {
            if (routine != null)
            {
                StopCoroutine(routine);
                routine = null;
            }
            Color color = meshRenderer.material.color;
            color.a = 1;
            meshRenderer.material.color = color;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Scan" && LevelManager.Instance.activationInputs.Find(x => x.name == "Radar").isActive)
        {
            Color color = meshRenderer.material.color;
            color.a = 1;
            meshRenderer.material.color = color;
            routine = StartCoroutine(fadeRoutine());
        }
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

        }
        routine = null;
    }

}
