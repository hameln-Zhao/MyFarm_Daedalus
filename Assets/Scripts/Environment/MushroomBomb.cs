using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomBomb : MonoBehaviour
{
    public Sprite[] animationFrames; // 放入帧图
    public float chargeDuration = 3f; // 蓄力时间
    public float explosionDuration = 1f; // 爆炸时间

    public SpriteRenderer sr;
    public Coroutine currentRoutine;
    public bool playerInside = false;
    private Transform player;
    
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = animationFrames[0];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player Entered");
            playerInside = true;
            player = collision.transform;
            if(currentRoutine == null)
                currentRoutine = StartCoroutine(ChargeAndExplode());
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInside = false;
            if (currentRoutine != null)
            {
                StopCoroutine(currentRoutine);
                currentRoutine = null;
                sr.sprite = animationFrames[0];
            }
        }
    }

    IEnumerator ChargeAndExplode()
    {
        float t = 0f;
        while (t < chargeDuration)
        {
            if (!playerInside) yield break;

            int frameIndex = Mathf.FloorToInt((t / chargeDuration) * 5);
            frameIndex = Mathf.Clamp(frameIndex, 0, 4);
            sr.sprite = animationFrames[frameIndex];
            t += Time.deltaTime;
            yield return null;
        }

        if (playerInside)
        {
            yield return StartCoroutine(PlayExplosion());
            Debug.Log("受到伤害");
            Destroy(gameObject);
        }
    }
    
    IEnumerator PlayExplosion() {
        // 播放帧 6~8（index 5~7）
        float frameTime = explosionDuration / 3f;
        for (int i = 5; i <= 7; i++) {
            sr.sprite = animationFrames[i];
            yield return new WaitForSeconds(frameTime);
        }
    }
}
