using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 驱动时间 UI 的脚本：显示 12 小时制时间(如 11:00am)、星期(如 星期三)、季节(如 冬)、日期(如 12月23日)
/// 挂到包含这些 TMP_Text 的对象(通常是 Canvas 下某个 Panel) 上，并在 Inspector 里拖拽引用。
/// </summary>
public class TimeUIController : MonoBehaviour
{
    [Header("引用 TMP_Text 组件")] 
    public TMP_Text timeText;      // 11:00am
    public TMP_Text weekdayText;   // 星期三
    public TMP_Text seasonText;    // 冬
    public TMP_Text dateText;      // 12月23日

    // 缓存，避免重复赋值
    private int currentHour = -1;
    private int currentMinute = -1;
    private int currentDay = -1;
    private int currentMonth = -1;
    private int currentYear = -1;
    private Season currentSeason;

    private static readonly string[] weekNames =
    {
        "星期一","星期二","星期三","星期四","星期五","星期六","星期日"
    };

    private void OnEnable()
    {
        EventHandler.GameMinuteEvent += OnGameMinuteEvent;          // 分钟变化
        EventHandler.GameDateEvent += OnGameDateEvent;              // 时/日/月/年/季节变化
    }

    private void OnDisable()
    {
        EventHandler.GameMinuteEvent -= OnGameMinuteEvent;
        EventHandler.GameDateEvent -= OnGameDateEvent;
    }

    private void Start()
    {
        // 初始拉一次数据（TimeManager.Start 会先广播一次事件，这里可不做额外处理）
    }

    private void OnGameMinuteEvent(int minute, int hour)
    {
        if (minute == currentMinute && hour == currentHour) return;
        currentMinute = minute;
        currentHour = hour;
        UpdateTimeText(hour, minute);
    }

    private void OnGameDateEvent(int hour, int day, int month, int year, Season season)
    {
        bool timeChanged = hour != currentHour; // 有时 GameDateEvent 会伴随时间也变化
        bool dateChanged = day != currentDay || month != currentMonth || year != currentYear;
        bool seasonChanged = season != currentSeason;

        currentHour = hour;
        currentDay = day;
        currentMonth = month;
        currentYear = year;
        currentSeason = season;

        if (timeChanged) UpdateTimeText(hour, currentMinute); // currentMinute 已由分钟事件维护, 但第一次也可能是 -1
        if (dateChanged) UpdateDateTexts();
        if (seasonChanged) UpdateSeasonText();
    }

    private void UpdateTimeText(int hour24, int minute)
    {
        if (minute < 0) return; // 还没收到分钟事件
        int hour12 = hour24 % 12;
        if (hour12 == 0) hour12 = 12;
        string suffix = hour24 < 12 ? "am" : "pm";
        // 分钟补零
        string timeStr = $"{hour12}:{minute:00}{suffix}";
        if (timeText != null && timeText.text != timeStr)
            timeText.text = timeStr;
    }

    private void UpdateDateTexts()
    {
        // 星期计算：从第 1 年 1 月 1 日(游戏开始) 作为星期一
        // 总天数 = (year-1)*每年月数*每天数 + (month-1)*每天数 + (day-1)
        int daysPerMonth = Settings.dayHold;          // day 1..dayHold
        int monthsPerYear = Settings.monthHold;       // month 1..monthHold
        int totalOffset = (currentYear - 1) * monthsPerYear * daysPerMonth + (currentMonth - 1) * daysPerMonth + (currentDay - 1);
        int weekIndex = totalOffset % 7; // 0 = 星期一

        if (weekdayText != null)
        {
            string weekStr = weekNames[weekIndex];
            if (weekdayText.text != weekStr) weekdayText.text = weekStr;
        }

        if (dateText != null)
        {
            string dateStr = $"{currentMonth}月{currentDay}日"; // 12月23日
            if (dateText.text != dateStr) dateText.text = dateStr;
        }
    }

    private void UpdateSeasonText()
    {
        if (seasonText == null) return;
        // Season 枚举本身是中文(春天/夏天/秋天/冬天)，如只想显示首字可取 substring
        string fullName = currentSeason.ToString(); // 例: "春天"
        string display = fullName.Length > 1 ? fullName.Substring(0, 1) : fullName; // 例: "春"
        // 若你希望显示整词(春天)则用 fullName
        if (seasonText.text != display)
            seasonText.text = display;
    }
}

