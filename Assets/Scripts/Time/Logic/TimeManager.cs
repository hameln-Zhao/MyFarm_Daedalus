using System;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    private int gameSecond, gameMinute, gameHour, gameDay,gameMonth,gameYear;
    private Season gameSeason = Season.春天;
    private int monthInSeason = 1;
    private bool gameClockPause;
    private float tikTime;
    public int GameTimeByMinutes;
    //灯光时间差
    private float timeDifference;

    public TimeSpan GameTime => new TimeSpan(gameHour, gameMinute, gameSecond);
    //awake》优于 enable

    protected override void Awake()
    {
        base.Awake();
        newGameTime();
    }

    private void Start()
    {
        EventHandler.CallGameMinuteEvent(gameMinute,gameHour);
        EventHandler.CallGameDateEvent(gameHour,gameDay,gameMonth,gameYear,gameSeason);
        //切换灯光
        EventHandler.CallLightShiftChangeEvent(gameSeason,GetCurrentLightShift(),timeDifference);
    }

    private void Update()
    {
        if (!gameClockPause)
        {
            tikTime += Time.deltaTime;
            if (tikTime>=Settings.secondThreshold)
            {
                tikTime -= Settings.secondThreshold;
                UpdateGameTime();
            }
        }

        if (Input.GetKey(KeyCode.T))
        {
            for (int i = 0; i < 60; i++)
            {
                UpdateGameTime();
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            gameDay++;
            EventHandler.CallGameDayEvent(gameDay,gameSeason);
            EventHandler.CallGameDateEvent(gameHour,gameDay,gameMonth,gameYear,gameSeason);
        }
        GameTimeByMinutes=gameHour*60+gameMinute;
    }
    
    private void newGameTime()
    {
        gameSecond = 0;
        gameMinute = 0;
        gameHour = 7;
        gameDay = 1;
        gameMonth = 1;
        gameYear = 1;
        gameSeason = Season.春天;
    }
    private void UpdateGameTime()
    {
        //Debug.Log(GameTime);
        gameSecond++;
        if (gameSecond> Settings.secondHold)
        {
            gameMinute++;
            gameSecond = 0;
            if (gameMinute>Settings.minuteHold)
            {
                gameHour++;
                gameMinute = 0;
                if (gameHour>Settings.hourHold)
                {
                    gameDay++;
                    gameHour = 0;
                    if (gameDay > Settings.dayHold)
                    {
                        gameMonth++;
                        gameDay = 1;//日期没有0
                        if (gameMonth>Settings.monthHold)
                        {
                            gameMonth = 1;
                        }

                        monthInSeason--;
                        if (monthInSeason==0)
                        {
                            monthInSeason = 3;
                            int seasonNumber = (int)gameSeason;
                            seasonNumber++;
                            if (seasonNumber>Settings.seasonHold)
                            {
                                seasonNumber = 0;
                                gameYear++;
                            }
                            gameSeason=(Season)seasonNumber;
                            if (gameYear>3)
                            {
                                //时间到了
                                gameYear = 1;
                            }
                        }
                        //每天刷新地图和农作物生长
                        EventHandler.CallGameDayEvent(gameDay,gameSeason);
                    }
                   
                }
                EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth,gameYear,gameSeason );
            }
            
            EventHandler.CallGameMinuteEvent(gameMinute,gameHour);
            //切换灯光
            EventHandler.CallLightShiftChangeEvent(gameSeason,GetCurrentLightShift(),timeDifference);
            
        }
    }

    private LightShift GetCurrentLightShift()
    {
        if (GameTime >= Settings.morningTime && GameTime < Settings.nightTime)
        {
            timeDifference = (float)(GameTime - Settings.morningTime).TotalMinutes;
            //Debug.Log("白天："+timeDifference);
            return LightShift.Morning;
        }

        if (GameTime < Settings.morningTime || GameTime >= Settings.nightTime)
        {
            timeDifference = Mathf.Abs((float)(GameTime - Settings.nightTime).TotalMinutes);
            //Debug.Log("晚上："+timeDifference);
            return LightShift.Night;
        }
        return LightShift.Morning;
    }
}
