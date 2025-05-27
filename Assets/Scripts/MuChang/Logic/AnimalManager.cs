using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyFarm.AnimalFarm
{
    public class AnimalManager : Singleton<AnimalManager>
    {
        public AnimalDataList_SO AnimalData;
        private Transform animalParent;
        private List<Animal> animals = new List<Animal>();

        private void OnEnable()
        {
            // 监听放置动物事件（参数：动物ID、生成位置）
            EventHandler.PlaceAnimalEvent += OnPlaceAnimalEvent;
            // 监听每日时间推进事件
            EventHandler.GameDayEvent += OnGameDayEvent;
            // 监听施加魔素给动物的事件（参数：目标动物、魔素数值）
            EventHandler.ApplyMagicToAnimalEvent += OnApplyMagicToAnimalEvent;
        }

        private void OnDisable()
        {
            EventHandler.PlaceAnimalEvent -= OnPlaceAnimalEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
            EventHandler.ApplyMagicToAnimalEvent -= OnApplyMagicToAnimalEvent;
        }

        private void Start()
        {
            
        }

        private void OnPlaceAnimalEvent(int animalID, Vector3 spawnPosition)
        {
            AnimalDetails details = getAnimalDetails(animalID);
            if (details != null)
            {
                SpawnAnimal(details, spawnPosition);
            }
        }

        private void SpawnAnimal(AnimalDetails details, Vector3 spawnPosition)
        {
            animalParent = GameObject.FindWithTag("AnimalParent").transform;
            // 使用第一阶段的 Prefab 作为初始外观
            GameObject prefab = details.petPrefab;
            GameObject animalGO = Instantiate(prefab, spawnPosition, Quaternion.identity, animalParent);
            Animal animal = animalGO.GetComponent<Animal>();
            if (animal != null)
            {
                animal.animalDetails = details;
                animal.currentGrowthDays = 0;
                animal.currentMagicAmount = 0;
                //animal.isMutated = false;
                animal.careActionCount = 0;
                animals.Add(animal);
                animal.UpdateAppearance();
            }
        }
        

        private void OnGameDayEvent(int day, Season season)
        {
            //动物随时间增长变化
            foreach (var animal in animals)
            {
                animal.DailyGrowthUpdate();
            }
        }

        private void OnApplyMagicToAnimalEvent(Animal animal, int magicAmount)
        {
            if (animal != null)
                animal.ApplyMagic(magicAmount);
        }

        public AnimalDetails getAnimalDetails(int animalID)
        {
            return AnimalData.AnimalDetailList.Find(a=>a.AnimalID == animalID);
        }
        
    }
}

