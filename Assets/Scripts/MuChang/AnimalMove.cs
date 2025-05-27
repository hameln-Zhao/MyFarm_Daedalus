using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;

public class AnimalMove : MonoBehaviour
{
    [Header("动物移动")]
    private Vector2 moveDirection;        // 移动方向
    private Farm farmArea;     // 牧场范围检测脚本
    private float timer;
    private float stopTimer;
    private bool isMoving;
    public float changeDirectionTime = 2f;
    public float moveSpeed = 2f;          // 移动速度
    public float stopTimeMin = 1f;
    public float stopTimeMax = 3f;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        // 获取牧场范围检测脚本组件
        farmArea = GetComponentInParent<Farm>();
        //初始方向
        SetRandomDirection();
        stopTimer = Random.Range(stopTimeMin, stopTimeMax);
    }

    void FixedUpdate()
    {
        AnimalMovement();
    }

    private void AnimalMovement()
    {
        float Speed;
        if (!isMoving)
        {
            Speed = 0;
            animator.SetFloat("Speed",Speed);
            stopTimer -= Time.deltaTime;
            if (stopTimer <= 0)
            {
                isMoving = true;
                SetRandomDirection();
                stopTimer = Random.Range(stopTimeMin, stopTimeMax);
            }
        }
        else
        {
            timer += Time.deltaTime;
            if (timer >= changeDirectionTime)
            {
                float randomStopChance = Random.Range(0f, 1f);
                if (randomStopChance < 0.2f)
                {
                    isMoving = false;
                    timer = 0f;
                }
                else
                {
                    SetRandomDirection();
                }
            }

            Speed = moveSpeed;
            animator.SetFloat("Speed",Speed);
            Vector2 newPosition = (Vector2)transform.position + moveDirection * moveSpeed * Time.deltaTime;
            if(farmArea.IsInFarmArea(newPosition))
                transform.position = newPosition;
            else
                SetRandomDirection();
        }
    }
    
    private void SetRandomDirection()
    {
        // 生成一个随机的移动方向
        moveDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
}
