using System;
using Cinemachine;
using UnityEngine;

public class SwitchBounds : MonoBehaviour
{
    private void OnEnable()
    {
        EventHandler.BeforeSceneEvent += SwitchBoundsConfiner;
    }
    private void OnDisable()
    {
        EventHandler.BeforeSceneEvent -= SwitchBoundsConfiner;
    }
    private void SwitchBoundsConfiner()
    {
        PolygonCollider2D BoundsConfiner = GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();
        CinemachineConfiner Confiner = GetComponent<CinemachineConfiner>();
        Confiner.m_BoundingShape2D= BoundsConfiner;
        Confiner.InvalidatePathCache();
    }
}
