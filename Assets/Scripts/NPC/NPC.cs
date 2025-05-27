using System;
using System.Collections;
using System.Collections.Generic;
using MyFarm.Dialogue;
using UnityEngine;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(BoxCollider2D))]
public class NPC : MonoBehaviour
{
    [SceneName]
    public string npcSceneName;
    public GameObject npcPrefab;
    private void Update()
    {
        if (SceneManager.GetSceneAt(SceneManager.sceneCount - 1).name==npcSceneName)
        {
            npcPrefab.SetActive(true);
        }
        else
        {
            npcPrefab.SetActive(false);
        }
    }
    
   



}
