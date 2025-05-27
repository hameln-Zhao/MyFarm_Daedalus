using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour
{
    public RectTransform dayNightImage;
    public Image seasonImage;
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI timeText;
    public Sprite[] seasonSprites;

    private void OnEnable()
    {
        EventHandler.GameMinuteEvent += OnGameMinuteEvent;
        EventHandler.GameDateEvent += OnGameDateEvent;
    }

    private void OnDisable()
    {
        EventHandler.GameMinuteEvent -= OnGameMinuteEvent;
        EventHandler.GameDateEvent -= OnGameDateEvent;
    }

    private void OnGameDateEvent(int hour, int day, int month, int year, Season season)
    {
        dateText.text=year+"年"+month.ToString("00")+"月"+day.ToString("00")+"日";
        seasonImage.sprite = seasonSprites[(int)season];
        DayAndNightRotate(hour);
    }

    private void OnGameMinuteEvent(int minute, int hour)
    {
        timeText.text=hour.ToString("00") + ":" + minute.ToString("00");
    }

    private void DayAndNightRotate(int hour)
    {
        var target = new Vector3(0, 0, hour * 15-90);//应该是黑天开始
        dayNightImage.DORotate(target, 1f,RotateMode.Fast);
    }
    
}
