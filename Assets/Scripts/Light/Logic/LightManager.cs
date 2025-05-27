using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    private LightControl[] sceneLights;
    private LightShift currentLightShift;
    private Season currentSeason;
    private float timeDifference;

    private void OnEnable()
    {
        EventHandler.AfterSceneEvent += OnAfterSceneEvent;
        EventHandler.LightShiftChangeEvent += OnLightShiftChangeEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneEvent -= OnAfterSceneEvent;
        EventHandler.LightShiftChangeEvent -= OnLightShiftChangeEvent;
    }

    private void OnLightShiftChangeEvent(Season season, LightShift lightShift, float timeDifference)
    {
        currentSeason = season;
        this.timeDifference = timeDifference;
        if(currentLightShift != lightShift)
        {
            currentLightShift = lightShift;
            foreach (var light in sceneLights)
            {
                //lightControl 改变灯光的方法
                light.ChangeLightShift(currentSeason,currentLightShift,timeDifference);
            }
        }
        
    }

    private void OnAfterSceneEvent()
    {
        sceneLights = FindObjectsOfType<LightControl>();
        foreach (var light in sceneLights)
        {
            //lightControl 改变灯光的方法
            light.ChangeLightShift(currentSeason,currentLightShift,timeDifference);
        }
    }
}
