using UnityEngine;
using System;

public class Settings
{ 
   //透明相关
   public const float itemFadeDuration = 0.35f;
   public const float targetAlpha = 0.45f;
 
   //时间相关
   public const float secondThreshold = 0.01f;//数值越小时间越快
   public const int secondHold = 59;
   public const int minuteHold = 59;
   public const int hourHold = 23;
   public const int dayHold = 10;
   public const int seasonHold = 3;
   public const int monthHold = 4;
   
   //Transition
   public const float fadeCanvasDuration = 1f;
   
   //Player
   public const float holdHeight = 1.5f;
   public const float floatingDistance = 5f;//鱼漂出现位置的系数
   public const float fallGravity=-3.5f;
   //Animator
   public const float holdImgTime = 0.5f;
   
   //灯光
   public static TimeSpan morningTime = new TimeSpan(5,0,0);
   public static TimeSpan nightTime = new TimeSpan(19,0,0);
   public const float lightChangeDuration = 6f;
   
   //钓鱼
   public const float ThrowDuration = 1f;
   public const float maxDistance = 5f;//最大位移系数
   public const float speedY = 3f;//初始运动的速度
   public const float speedX = 3f;//初始运动的速度
   public const float fallAcceleration = 30f;//初始运动的速度
   public const float totalRotation = 720;//具体旋转圈数
}
