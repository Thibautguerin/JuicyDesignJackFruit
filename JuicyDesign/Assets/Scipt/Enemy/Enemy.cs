﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    private MeshRenderer meshRenderer;
    private Coroutine routine;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        Color color = meshRenderer.material.color;
        color.a = 0;
        meshRenderer.material.color = color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Scan")
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
        if (collision.tag == "Scan")
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