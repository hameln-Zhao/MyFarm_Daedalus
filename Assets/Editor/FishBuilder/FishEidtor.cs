using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor.UIElements;
public class FishEditor : EditorWindow
{
    private FishDataList_SO container;
    private List<FishDetails> fishDataList = new List<FishDetails>();
    private VisualTreeAsset fishRowTemplate;
    private ScrollView fishDetailsSection;
    private FishDetails activeFish;
    private Sprite defaultIcon;
    private VisualElement iconPreview;
    private ListView fishListView;
    private List<int> allFieldIDs = new List<int> {0, 1, 2, 3, 4 }; 
    private int fishingToolID = 1004;
    [MenuItem("Toolkit/FishEditor")]
    public static void ShowExample()
    {
        FishEditor wnd = GetWindow<FishEditor>();
        wnd.titleContent = new GUIContent("鱼类编辑器");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        
        // 加载UI布局
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/FishBuilder/FishEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        fishRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/FishBuilder/FishRowTemplate.uxml");
        defaultIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/M Studio/Art/Items/Icons/icon_M.png");

        fishListView = root.Q<VisualElement>("ItemList").Q<ListView>("ListView");
        fishDetailsSection = root.Q<ScrollView>("ItemDetails");
        iconPreview = fishDetailsSection.Q<VisualElement>("Icon");

        root.Q<Button>("AddButton").clicked += OnAddFishClicked;
        root.Q<Button>("DeleteButton").clicked += OnDeleteClicked;
        
        LoadDataBase();
        GenerateListView();
        
        fishListView.selectionChanged += OnListSelectionChange;
    }

  #region 事件处理
    private void OnDeleteClicked()
    {
        if (activeFish == null) return;
        
        if (EditorUtility.DisplayDialog("删除鱼类", 
            $"确定要删除鱼类 '{activeFish.ItemName}' 吗?", "删除", "取消"))
        {
            fishDataList.Remove(activeFish);
            SaveFishList();
            fishListView.Rebuild();
            fishDetailsSection.visible = false;
        }
    }

    private void OnAddFishClicked()
    {
        FishDetails newFish = new FishDetails
        {
            ItemName = "新鱼类",
            ItemID = GenerateNewID(),
            ItemType = ItemType.Consumable,
            consumableType = Consumable.ConsumableType.Fish,
            rarity = Rarity.Common,
            fieldID = new int[] { 0, 1 },
            seasons = new Season[] { Season.春天, Season.夏天, Season.秋天, Season.冬天 }, // 默认全季节
            weathers = (Weather[])Enum.GetValues(typeof(Weather)),
            weightRange = new RangeFloat { Min = 1, Max = 10 }, // 默认重量范围
            level = 1,
            difficultyLevel = 1,
            healthValue = 10, // 默认健康值
            canDash = false,
            dashDuration = 1f,
            dashSpeedMultiplier = 1.5f,
            dashProbability = 0.1f,
            //通用属性设置
            CanStack = true,       // 可堆叠
            CanInBag = true,       // 可进背包
            CanWear = false,       // 不可穿戴
            CanHold = true,        // 可手持
            CanSell = true,        // 可出售
            CanUse = true,         // 可能可以使用
            CanPlace = true,       // 可能可以摆放
            CanRotate = false,     // 不可旋转
            CanInteractOnMap = false, // 不可地图交互
            CanUseInCrafting = true, // 可用于制造
            CanGift = true,        // 可赠送
            CanPickedup = true,
            CanDropped = true,
            CanCarried = true,
        };
        
        fishDataList.Add(newFish);
        SaveFishList();
        fishListView.Rebuild();
        fishListView.ScrollToItem(fishDataList.Count - 1);
    }
    
    private void OnListSelectionChange(IEnumerable<object> selectedItem)
    {
        activeFish = (FishDetails)selectedItem.FirstOrDefault();
        if (activeFish != null)
        {
            GetFishDetails();
            fishDetailsSection.visible = true;
        }
    }
    #endregion

    private void LoadDataBase()
    {
        var containerArray = AssetDatabase.FindAssets("FishDataList_SO");
        if (containerArray.Length > 0)
        {
            var path = AssetDatabase.GUIDToAssetPath(containerArray[0]);
            container = AssetDatabase.LoadAssetAtPath(path, typeof(FishDataList_SO)) as FishDataList_SO;
            fishDataList = container.fishDataList ?? new List<FishDetails>();
        }
        
        EditorUtility.SetDirty(container);
    }
    
    private void SaveFishList()
    {
        container.fishDataList = fishDataList;
        EditorUtility.SetDirty(container);
        AssetDatabase.SaveAssets();
    }
    
    private int GenerateNewID()
    {
        return fishDataList.Count > 0 ? fishDataList.Max(fish => fish.ItemID) + 1 : 2001;
    }
    
    private void GenerateListView()
    {
        Func<VisualElement> makeItem = () => fishRowTemplate.CloneTree();

        Action<VisualElement, int> bindItem = (e, i) =>
        {
            if (i < fishDataList.Count)
            {
                if (fishDataList[i].ItemIcon != null)
                    e.Q<VisualElement>("Icon").style.backgroundImage = fishDataList[i].ItemIcon.texture;
                e.Q<Label>("Name").text = fishDataList[i] == null ? "未知鱼类" : fishDataList[i].ItemName;
                e.Q<Label>("Rarity").text = fishDataList[i].rarity.ToString();
            }
        };

        fishListView.fixedItemHeight = 60;
        fishListView.itemsSource = fishDataList;
        fishListView.makeItem = makeItem;
        fishListView.bindItem = bindItem;
        
        fishDetailsSection.visible = false;
    }
    
    private void GetFishDetails()
    {
        fishDetailsSection.MarkDirtyRepaint();
        // 定义间距变量
        const int toggleSpacing = 20; // Toggle之间的间距
        const int labelSpacing = 5;   // 复选框和文字的间距
        // 基本信息
        fishDetailsSection.Q<IntegerField>("ItemID").value = activeFish.ItemID;
        fishDetailsSection.Q<IntegerField>("ItemID").RegisterValueChangedCallback(evt =>
        {
            activeFish.ItemID = evt.newValue;
        });

        fishDetailsSection.Q<TextField>("ItemName").value = activeFish.ItemName;
        fishDetailsSection.Q<TextField>("ItemName").RegisterValueChangedCallback(evt =>
        {
            activeFish.ItemName = evt.newValue;
            fishListView.Rebuild();
        });

        // 图标处理
        iconPreview.style.backgroundImage = activeFish.ItemIcon == null ? defaultIcon.texture : activeFish.ItemIcon.texture;
        fishDetailsSection.Q<ObjectField>("ItemIcon").value = activeFish.ItemIcon;
        fishDetailsSection.Q<ObjectField>("ItemIcon").RegisterValueChangedCallback(evt =>
        {
            activeFish.ItemIcon = evt.newValue as Sprite;
            iconPreview.style.backgroundImage = activeFish.ItemIcon == null ? defaultIcon.texture : activeFish.ItemIcon.texture;
            fishListView.Rebuild();
        });
        
        fishDetailsSection.Q<ObjectField>("ItemSprite").value = activeFish.ItemOnWorldSprite;
        fishDetailsSection.Q<ObjectField>("ItemSprite").RegisterValueChangedCallback(evt =>
        {
            activeFish.ItemOnWorldSprite = evt.newValue as Sprite;
        });
        
        fishDetailsSection.Q<EnumField>("ItemType").Init(activeFish.consumableType);
        fishDetailsSection.Q<EnumField>("ItemType").value = activeFish.consumableType;
   
        // 鱼类特有属性
        fishDetailsSection.Q<EnumField>("Rarity").Init(activeFish.rarity);
        fishDetailsSection.Q<EnumField>("Rarity").value = activeFish.rarity;
        fishDetailsSection.Q<EnumField>("Rarity").RegisterValueChangedCallback(evt =>
        {
            activeFish.rarity = (Rarity)evt.newValue;
            fishListView.Rebuild();
        });
        fishDetailsSection.Q<IntegerField>("ItemLevel").value = activeFish.level;
        fishDetailsSection.Q<IntegerField>("ItemLevel").RegisterValueChangedCallback(evt =>
        {
            activeFish.level = evt.newValue;
        });
        
        fishDetailsSection.Q<IntegerField>("ItemDifficultyLevel").value = activeFish.difficultyLevel;
        fishDetailsSection.Q<IntegerField>("ItemDifficultyLevel").RegisterValueChangedCallback(evt =>
        {
            activeFish.difficultyLevel = evt.newValue;
        });
        
        fishDetailsSection.Q<IntegerField>("HealthValue").value = activeFish.healthValue;
        fishDetailsSection.Q<IntegerField>("HealthValue").RegisterValueChangedCallback(evt =>
        {
            activeFish.restoreValue = evt.newValue;
        });
        
        // 重量范围
        fishDetailsSection.Q<FloatField>("WeightMin").value = activeFish.weightRange.Min;
        fishDetailsSection.Q<FloatField>("WeightMin").RegisterValueChangedCallback(evt =>
        {
            activeFish.weightRange.Min = Mathf.Max(0.1f, evt.newValue); // 最小0.1
        });
        
        fishDetailsSection.Q<FloatField>("WeightMax").value = activeFish.weightRange.Max;
        fishDetailsSection.Q<FloatField>("WeightMax").RegisterValueChangedCallback(evt =>
        {
            activeFish.weightRange.Max = Mathf.Max(activeFish.weightRange.Min + 0.1f, evt.newValue); // 必须大于最小值
        });
        
        // 季节选择
        var seasonContainer = fishDetailsSection.Q<VisualElement>("SeasonContainer");
        seasonContainer.Clear();
        // 设置容器为水平布局
        seasonContainer.style.flexDirection = FlexDirection.Row;
        seasonContainer.style.flexWrap = Wrap.Wrap; // 允许换行
        seasonContainer.style.alignItems = Align.Center; // 垂直居中
        foreach (Season season in Enum.GetValues(typeof(Season)))
        {
            Toggle toggle = new Toggle();
            Label label = new Label(season.ToString());
            label.style.marginLeft = labelSpacing;
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            toggle.Add(label);
            toggle.style.marginRight = toggleSpacing;
            toggle.style.alignItems = Align.Center;
            toggle.style.flexDirection = FlexDirection.Row;
            toggle.value = activeFish.seasons != null && activeFish.seasons.Contains(season);
            toggle.RegisterValueChangedCallback(evt =>
            {
                var seasons = activeFish.seasons != null ? new List<Season>(activeFish.seasons) : new List<Season>();
                
                if (evt.newValue && !seasons.Contains(season))
                {
                    seasons.Add(season);
                }
                else if (!evt.newValue && seasons.Contains(season))
                {
                    seasons.Remove(season);
                }
                
                activeFish.seasons = seasons.ToArray();
            });
            
            seasonContainer.Add(toggle);
        }
        //钓鱼区域设置
        var fieldContainer = fishDetailsSection.Q<VisualElement>("FieldContainer");
        fieldContainer.Clear();
        // 设置容器为水平布局
        fieldContainer.style.flexDirection = FlexDirection.Row;
        fieldContainer.style.flexWrap = Wrap.Wrap; // 允许换行
        fieldContainer.style.alignItems = Align.Center; // 垂直居中
      
        foreach (int id in allFieldIDs)
        {
            Toggle toggle = new Toggle();
            Label label = new Label($"区域 {id}");
            label.style.marginLeft = labelSpacing;
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            toggle.Add(label);
            toggle.style.marginRight = toggleSpacing;
            toggle.style.alignItems = Align.Center;
            toggle.style.flexDirection = FlexDirection.Row;
            
            toggle.value = activeFish.fieldID != null && activeFish.fieldID.Contains(id);
            int currentID = id;

            toggle.RegisterValueChangedCallback(evt =>
            {
                var fields = activeFish.fieldID != null ? new List<int>(activeFish.fieldID) : new List<int>();

                if (evt.newValue && !fields.Contains(currentID))
                {
                    fields.Add(currentID);
                }
                else if (!evt.newValue && fields.Contains(currentID))
                {
                    fields.Remove(currentID);
                }

                activeFish.fieldID = fields.ToArray();
            });

            fieldContainer.Add(toggle);
        }
        //天气选择
        var weatherContainer = fishDetailsSection.Q<VisualElement>("WeatherContainer");
        weatherContainer.Clear();
        // 设置容器为水平布局
        weatherContainer.style.flexDirection = FlexDirection.Row;
        weatherContainer.style.flexWrap = Wrap.Wrap; // 允许换行
        weatherContainer.style.alignItems = Align.Center; // 垂直居中
        foreach (Weather weather in Enum.GetValues(typeof(Weather)))
        {
            Toggle toggle = new Toggle();
            Label label = new Label(weather.ToString());
            label.style.marginLeft = labelSpacing;
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            toggle.Add(label);
            toggle.style.marginRight = toggleSpacing;
            toggle.style.alignItems = Align.Center;
            toggle.style.flexDirection = FlexDirection.Row;
            toggle.value = activeFish.weathers != null && activeFish.weathers.Contains(weather);

            toggle.RegisterValueChangedCallback(evt =>
            {
                var weatherList = activeFish.weathers != null ? new List<Weather>(activeFish.weathers) : new List<Weather>();

                if (evt.newValue && !weatherList.Contains(weather))
                {
                    weatherList.Add(weather);
                }
                else if (!evt.newValue && weatherList.Contains(weather))
                {
                    weatherList.Remove(weather);
                }

                activeFish.weathers = weatherList.ToArray();
            });

            weatherContainer.Add(toggle);
        }
        // 钓鱼工具ID列表
        var fishingToolsContainer = fishDetailsSection.Q<VisualElement>("FishingToolsContainer");
        fishingToolsContainer.Clear();

        if (activeFish.fishingToolItemID == null || activeFish.fishingToolItemID.Length == 0)
        {
            activeFish.fishingToolItemID = new int[] { fishingToolID }; // 默认值
        }

        for (int i = 0; i < activeFish.fishingToolItemID.Length; i++)
        {
            int index = i; // 闭包捕获
            var toolID = activeFish.fishingToolItemID[i];
    
            VisualElement toolElement = new VisualElement();
            toolElement.style.flexDirection = FlexDirection.Row;
            toolElement.style.marginBottom = 5;
    
            IntegerField toolField = new IntegerField("钓鱼工具ID");
            toolField.style.width = 175;
            toolField.value = toolID;
            toolField.RegisterValueChangedCallback(evt =>
            {
                activeFish.fishingToolItemID[index] = Mathf.Max(0, evt.newValue);
            });
    
            Button removeButton = new Button(() => RemoveFishingTool(index)) { text = "删除" };
    
            toolElement.Add(toolField);
            toolElement.Add(removeButton);
            fishingToolsContainer.Add(toolElement);
        }

        Button addToolButton = new Button(AddFishingTool) { text = "添加钓鱼工具" };
        fishingToolsContainer.Add(addToolButton);
        // 出现时间范围
        // 出现时间范围
        var timeRangesContainer = fishDetailsSection.Q<VisualElement>("TimeRangesContainer");
        timeRangesContainer.Clear();

        if (activeFish.spawnTimeRanges == null || activeFish.spawnTimeRanges.Length == 0)
        {
            activeFish.spawnTimeRanges = new TimeRange[] { new TimeRange { StartTime = 600, EndTime = 1800 } };
        }

        for (int i = 0; i < activeFish.spawnTimeRanges.Length; i++)
        {
            int index = i; // 闭包捕获
            var timeRange = activeFish.spawnTimeRanges[i];
    
            VisualElement timeRangeElement = new VisualElement();
            timeRangeElement.style.flexDirection = FlexDirection.Row;
    
            IntegerField startField = new IntegerField("开始时间");
            startField.value = timeRange.StartTime;
            startField.RegisterValueChangedCallback(evt =>
            {
                activeFish.spawnTimeRanges[index].StartTime = Mathf.Clamp(evt.newValue, 0, 2359);
            });
    
            IntegerField endField = new IntegerField("结束时间");
            endField.value = timeRange.EndTime;
            endField.RegisterValueChangedCallback(evt =>
            {
                activeFish.spawnTimeRanges[index].EndTime = Mathf.Clamp(evt.newValue, 0, 2359);
            });
    
            Button removeButton = new Button(() => RemoveTimeRange(index)) { text = "删除" };
    
            timeRangeElement.Add(startField);
            timeRangeElement.Add(endField);
            timeRangeElement.Add(removeButton);
            timeRangesContainer.Add(timeRangeElement);
        }

        Button addTimeButton = new Button(AddTimeRange) { text = "添加时间范围" };
        timeRangesContainer.Add(addTimeButton);
        
        

        #region 其他通用属性
        fishDetailsSection.Q<Slider>("SellPercentage").value = activeFish.SellPercentage;
        fishDetailsSection.Q<Slider>("SellPercentage").RegisterValueChangedCallback(evt =>
        {
            activeFish.SellPercentage = evt.newValue;
        });

        fishDetailsSection.Q<IntegerField>("ItemUseRadius").value = activeFish.ItemUseRadius;
        fishDetailsSection.Q<IntegerField>("ItemUseRadius").RegisterValueChangedCallback(evt =>
        {
            activeFish.ItemUseRadius = evt.newValue;
        });
        fishDetailsSection.Q<TextField>("Description").value = activeFish.ItemDescription;
        fishDetailsSection.Q<TextField>("Description").RegisterValueChangedCallback(evt =>
        {
            activeFish.ItemDescription = evt.newValue;
        });

        fishDetailsSection.Q<IntegerField>("Price").value = activeFish.ItemPrice;
        fishDetailsSection.Q<IntegerField>("Price").RegisterValueChangedCallback(evt =>
        {
            activeFish.ItemPrice = Mathf.Max(0, evt.newValue);
        });
         fishDetailsSection.Q<Toggle>("CanPickedup").value = activeFish.CanPickedup;
        fishDetailsSection.Q<Toggle>("CanPickedup").RegisterValueChangedCallback(evt =>
        {
            activeFish.CanPickedup = evt.newValue;
        });

        fishDetailsSection.Q<Toggle>("CanDropped").value = activeFish.CanDropped;
        fishDetailsSection.Q<Toggle>("CanDropped").RegisterValueChangedCallback(evt =>
        {
            activeFish.CanDropped = evt.newValue;
        });

        fishDetailsSection.Q<Toggle>("CanCarried").value = activeFish.CanCarried;
        fishDetailsSection.Q<Toggle>("CanCarried").RegisterValueChangedCallback(evt =>
        {
            activeFish.CanCarried = evt.newValue;
        });
          fishDetailsSection.Q<Toggle>("CanStack").value = activeFish.CanStack;
        fishDetailsSection.Q<Toggle>("CanStack").RegisterValueChangedCallback(evt =>
        {
            activeFish.CanStack = evt.newValue;
        });
    
        fishDetailsSection.Q<Toggle>("CanInBag").value = activeFish.CanInBag;
        fishDetailsSection.Q<Toggle>("CanInBag").RegisterValueChangedCallback(evt =>
        {
            activeFish.CanInBag = evt.newValue;
        });
    
        fishDetailsSection.Q<Toggle>("CanWear").value = activeFish.CanWear;
        fishDetailsSection.Q<Toggle>("CanWear").RegisterValueChangedCallback(evt =>
        {
            activeFish.CanWear = evt.newValue;
        });
    
        fishDetailsSection.Q<Toggle>("CanHold").value = activeFish.CanHold;
        fishDetailsSection.Q<Toggle>("CanHold").RegisterValueChangedCallback(evt =>
        {
            activeFish.CanHold = evt.newValue;
        });
    
        fishDetailsSection.Q<Toggle>("CanSell").value = activeFish.CanSell;
        fishDetailsSection.Q<Toggle>("CanSell").RegisterValueChangedCallback(evt =>
        {
            activeFish.CanSell = evt.newValue;
        });
    
        fishDetailsSection.Q<Toggle>("CanUse").value = activeFish.CanUse;
        fishDetailsSection.Q<Toggle>("CanUse").RegisterValueChangedCallback(evt =>
        {
            activeFish.CanUse = evt.newValue;
        });
    
        fishDetailsSection.Q<Toggle>("CanPlace").value = activeFish.CanPlace;
        fishDetailsSection.Q<Toggle>("CanPlace").RegisterValueChangedCallback(evt =>
        {
            activeFish.CanPlace = evt.newValue;
        });
    
        fishDetailsSection.Q<Toggle>("CanRotate").value = activeFish.CanRotate;
        fishDetailsSection.Q<Toggle>("CanRotate").RegisterValueChangedCallback(evt =>
        {
            activeFish.CanRotate = evt.newValue;
        });
    
        fishDetailsSection.Q<Toggle>("CanInteractOnMap").value = activeFish.CanInteractOnMap;
        fishDetailsSection.Q<Toggle>("CanInteractOnMap").RegisterValueChangedCallback(evt =>
        {
            activeFish.CanInteractOnMap = evt.newValue;
        });
    
        fishDetailsSection.Q<Toggle>("CanUseInCrafting").value = activeFish.CanUseInCrafting;
        fishDetailsSection.Q<Toggle>("CanUseInCrafting").RegisterValueChangedCallback(evt =>
        {
            activeFish.CanUseInCrafting = evt.newValue;
        });
    
        fishDetailsSection.Q<Toggle>("CanGift").value = activeFish.CanGift;
        fishDetailsSection.Q<Toggle>("CanGift").RegisterValueChangedCallback(evt =>
        {
            activeFish.CanGift = evt.newValue;
        });
        #endregion 
        fishDetailsSection.Q<Toggle>("CanDash").value = activeFish.canDash;
        fishDetailsSection.Q<Toggle>("CanDash").RegisterValueChangedCallback(evt =>
        {
            activeFish.canDash = evt.newValue;
        });
        fishDetailsSection.Q<FloatField>("DashProability").value = activeFish.dashProbability;
        fishDetailsSection.Q<FloatField>("DashProability").RegisterValueChangedCallback(evt =>
        {
            activeFish.dashProbability = evt.newValue;
        });
        fishDetailsSection.Q<FloatField>("DashSpeedMul").value = activeFish.dashSpeedMultiplier;
        fishDetailsSection.Q<FloatField>("DashSpeedMul").RegisterValueChangedCallback(evt =>
        {
            activeFish.dashSpeedMultiplier = evt.newValue;
        });
        fishDetailsSection.Q<FloatField>("DashDuration").value = activeFish.dashDuration;
        fishDetailsSection.Q<FloatField>("DashDuration").RegisterValueChangedCallback(evt =>
        {
            activeFish.dashDuration = evt.newValue;
        });
    }
        
    
    private void AddTimeRange()
    {
        var newRanges = new List<TimeRange>(activeFish.spawnTimeRanges ?? new TimeRange[0]);
        newRanges.Add(new TimeRange { StartTime = 600, EndTime = 1800 });
        activeFish.spawnTimeRanges = newRanges.ToArray();
        GetFishDetails(); // 刷新UI
    }

    private void RemoveTimeRange(int index)
    {
        if (activeFish.spawnTimeRanges == null || index < 0 || index >= activeFish.spawnTimeRanges.Length)
            return;
            
        var newRanges = new List<TimeRange>(activeFish.spawnTimeRanges);
        newRanges.RemoveAt(index);
        activeFish.spawnTimeRanges = newRanges.ToArray();
        GetFishDetails(); // 刷新UI
    }
    private void AddFishingTool()
    {
        var newTools = new List<int>(activeFish.fishingToolItemID ?? new int[0]);
        newTools.Add(1001); // 默认添加1001
        activeFish.fishingToolItemID = newTools.ToArray();
        GetFishDetails(); // 刷新UI
    }

    private void RemoveFishingTool(int index)
    {
        if (activeFish.fishingToolItemID == null || index < 0 || index >= activeFish.fishingToolItemID.Length)
            return;
        
        var newTools = new List<int>(activeFish.fishingToolItemID);
        newTools.RemoveAt(index);
        activeFish.fishingToolItemID = newTools.ToArray();
        GetFishDetails(); // 刷新UI
    }
    // 将整数时间(如600)格式化为显示字符串(如"06:00")
    private string FormatTimeDisplay(int time)
    {
        int hours = time / 100;
        int minutes = time % 100;
        return $"{hours:D2}:{minutes:D2}";
    }
}