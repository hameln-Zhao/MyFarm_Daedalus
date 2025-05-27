using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TimeRange))]
public class TimeRangeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 开始绘制属性
        EditorGUI.BeginProperty(position, label, property);

        // 绘制标签
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
       
        // 获取 StartTime 和 EndTime 属性
        SerializedProperty startTimeProp = property.FindPropertyRelative("StartTime");
        SerializedProperty endTimeProp = property.FindPropertyRelative("EndTime");

        // 将分钟数转换为小时和分钟
        int startTime = startTimeProp.intValue;
        int endTime = endTimeProp.intValue;

        int startHours = startTime / 60;
        int startMinutes = startTime % 60;

        int endHours = endTime / 60;
        int endMinutes = endTime % 60;

        // 设置字段宽度
        if (position.width>0)
        {
            
        
        float fieldWidth = position.width*4/7;
        Rect startRect = new Rect(position.x, position.y, fieldWidth, position.height);
        
        Rect endRect = new Rect(position.x + position.width*4/7-5, position.y, fieldWidth, position.height);
        
        // 绘制 Start Time 字段
        EditorGUI.BeginChangeCheck();
        int startInput = EditorGUI.IntField(startRect, "Start", startHours * 100 + startMinutes);
        if (EditorGUI.EndChangeCheck())
        {
            // 将输入的时间转换为分钟数
            startTimeProp.intValue = (startInput / 100) * 60 + (startInput % 100);
        }

        // 绘制 End Time 字段
        EditorGUI.BeginChangeCheck();
        int endInput = EditorGUI.IntField(endRect, "End", endHours * 100 + endMinutes);
        if (EditorGUI.EndChangeCheck())
        {
            // 将输入的时间转换为分钟数
            endTimeProp.intValue = (endInput / 100) * 60 + (endInput % 100);
        }
        }
        // 结束绘制属性
        EditorGUI.EndProperty();
    }
}