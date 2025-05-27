//绘制 Property
using System;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SceneNameAttribute))]//针对某一个属性进行绘制
public class SceneNameDrawer : PropertyDrawer
{
    //拿到所有场景
    private int sceneIndex = -1;
    private GUIContent[] sceneNames;
    private readonly string[] scenePathSplit = { "/", ".unity" };
    /// <summary>
    /// 会覆盖原来的Inspector面板内容
    /// </summary>
    /// <param name="position">包括宽高 xy</param> 
    /// <param name="property">对应标记了SceneName的Property</param>
    /// <param name="label"></param>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        base.OnGUI(position, property, label);
        if (EditorBuildSettings.scenes.Length==0)//判断buildSettings里有没有场景
        {
            return;
        }

        if (sceneIndex==-1)//在选择之后index就会改变 这样只会初始化一次
        {
            GetSceneNameArray(property);
        }

        var tempIndex = sceneIndex;
        sceneIndex=EditorGUI.Popup(position, label, sceneIndex, sceneNames); //弹出式列表
        if (tempIndex!=sceneIndex)//如果有变化 就赋值
        {
            property.stringValue = sceneNames[sceneIndex].text;//传递选择的名字给property
        }
       
    }

    private void GetSceneNameArray(SerializedProperty property)
    {
        var scenes = EditorBuildSettings.scenes;//获取所有场景 建立数组
        //初始化数组
        sceneNames = new GUIContent[scenes.Length];
        for (int i = 0; i < sceneNames.Length; i++)
        {
            string path = scenes[i].path;
            string[] splitPath = path.Split(scenePathSplit, StringSplitOptions.RemoveEmptyEntries);//模式是清除空格
            string sceneName = "";
            if (splitPath.Length>0)
            {
                sceneName = splitPath[splitPath.Length - 1];//得到的数组结果可能是 Asset Scene 01_Filed 所以取最后一个即可
            }
            else
            {
                sceneName = "(Deleted Scene)";
            }

            sceneNames[i] = new GUIContent(sceneName);
        }
        //如果build里面没有场景
        if (sceneNames.Length==0)
        {
            sceneNames = new[] { new GUIContent("Check Your Build Settings") };
        }
        //特性初始化一般都是空的 起码需要有初始的东西 默认是第一个元素
        if (!string.IsNullOrEmpty(property.stringValue))//如果不是空的
        {
            bool nameFound = false;
            for (int i = 0; i < sceneNames.Length; i++)
            {
                if (sceneNames[i].text==property.stringValue)//判断已经输入的文本内容对不对
                {
                    sceneIndex = i;
                    nameFound = true;
                    break;
                }   
            }
            if (nameFound==false)
            {
                sceneIndex = 0;
            }
        }
        else//是空
        {
            sceneIndex = 0;
        }
        //更新UI值
        property.stringValue = sceneNames[sceneIndex].text;//传递选择的名字给property
    }
}

