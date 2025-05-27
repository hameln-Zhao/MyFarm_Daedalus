using System;
using System.Collections.Generic;
using UnityEngine;

public class FishingLineManager: Singleton<FishingLineManager>
{
    [Header("References")]
    [SerializeField] private GameObject lineSegmentPrefab; // 拖入你创建的线段预制体
    [SerializeField] private Transform rodTip; // 鱼竿尖端位置
    [SerializeField] private Transform floatTransform; // 鱼漂的Transform
    
    [Header("Line Settings")]
    [SerializeField] private Color lineColor = Color.white;

    [SerializeField] private float lineThickness;//鱼线的宽度
    [SerializeField] private int segmentsPerUnit; // 每单位长度多少个线段
    [SerializeField] private float frequency; //波动的频率
    [SerializeField] private float amplitude; //波动的相位
    
    private List<GameObject> lineSegments = new List<GameObject>();
    private bool isLineActive = false;

    private void OnEnable()
    {
        EventHandler.FinishFishing += OnFinishFishing;
    }



    private void OnDisable()
    {
        EventHandler.FinishFishing -= OnFinishFishing;
    }

    private void Update()
    {
        if (isLineActive && floatTransform != null)
        {
            UpdateLinePosition(rodTip.position, floatTransform.position);
            
            // 根据钓鱼状态添加效果
            if (FishManager.Instance.fishGameState == FishState.WaitForFish)
            {
                AddWavingEffect(amplitude ,frequency); // 轻微晃动
            }
            else if (FishManager.Instance.fishGameState == FishState.Playing)
            {
                // 上钩时绷紧效果 - 不需要额外处理，因为UpdateLinePosition已经保持直线
            }
        }
    }

    // 创建新线段
    public void CreateNewLine(Transform startPoint, Transform endPoint)
    {
        rodTip = startPoint;
        floatTransform = endPoint;
        isLineActive = true;
        UpdateLinePosition(startPoint.position, endPoint.position);
    }

    // 更新线段位置
    private void UpdateLinePosition(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        int segmentCount = Mathf.Max(1, Mathf.CeilToInt(distance * segmentsPerUnit));
        
        // 调整线段数量
        while (lineSegments.Count < segmentCount)
        {
            GameObject segment = Instantiate(lineSegmentPrefab, transform);
            lineSegments.Add(segment);
        }
        while (lineSegments.Count > segmentCount)
        {
            Destroy(lineSegments[lineSegments.Count - 1]);
            lineSegments.RemoveAt(lineSegments.Count - 1);
        }
        
        // 更新每个线段的位置和旋转
        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)segmentCount;
            Vector3 position = Vector3.Lerp(start, end, t);
            GameObject segment = lineSegments[i];
            
            segment.transform.position = position;
            segment.transform.right = direction.normalized;
            segment.transform.localScale = new Vector3(distance / segmentCount, lineThickness, 1f);
            
            // 设置颜色
            SpriteRenderer renderer = segment.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = lineColor;
            }
        }
    }

    // 添加晃动效果
    private void AddWavingEffect(float amplitude, float frequency)
    {
        for (int i = 0; i < lineSegments.Count; i++)
        {
            GameObject segment = lineSegments[i];
            float offset = Mathf.Sin(Time.time * frequency + i * 0.3f) * amplitude;
            segment.transform.position += Vector3.up * offset;
        }
    }
    private void OnFinishFishing()
    {
        ClearLine();
    }
    // 清除线段
    public void ClearLine()
    {
        foreach (var segment in lineSegments)
        {
            Destroy(segment);
        }
        lineSegments.Clear();
        isLineActive = false;
    }
}