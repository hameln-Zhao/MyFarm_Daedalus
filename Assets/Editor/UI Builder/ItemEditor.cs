using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemEditor : EditorWindow
{
    private ItemContainer_SO container;
    private List<ItemDetails> itemList = new List<ItemDetails>();
    private VisualTreeAsset itemRowTemplate;
    private ScrollView itemDetailsSection;
    private ItemDetails activeItem;
    private Sprite defaultIcon;
    private VisualElement iconPreview;
    private ListView itemListView;
    
    // 修改：使用DropdownField代替RadioButtonGroup
    private DropdownField typeDropdown;
    private Dictionary<string, ItemType> dropdownOptions = new Dictionary<string, ItemType>()
    {
        {"装备", ItemType.Equipment},
        {"消耗品", ItemType.Consumable}
        // 可以继续添加其他类型
    };

    [MenuItem("Toolkit/ItemEditor")]
    public static void ShowExample()
    {
        ItemEditor wnd = GetWindow<ItemEditor>();
        wnd.titleContent = new GUIContent("物品编辑器");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Builder/ItemEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        itemRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Builder/ItemRowTemplate.uxml");
        defaultIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/M Studio/Art/Items/Icons/icon_M.png");

        itemListView = root.Q<VisualElement>("ItemList").Q<ListView>("ListView");
        itemDetailsSection = root.Q<ScrollView>("ItemDetails");
        iconPreview = itemDetailsSection.Q<VisualElement>("Icon");
        
     
        typeDropdown = root.Q<DropdownField>("ItemTypeDropdown");
        typeDropdown.choices = dropdownOptions.Keys.ToList();
        typeDropdown.index = 0; // 默认选择第一个选项
        LoadDataBase();
        typeDropdown.RegisterValueChangedCallback(OnItemTypeChanged);
      
        root.Q<Button>("AddButton").clicked += OnAddItemClicked;
        root.Q<Button>("DeleteButton").clicked += OnDeleteClicked;
        
        GenerateListView();
        itemListView.selectionChanged += OnListSelectionChange;
    }
  #region 事件处理
    private void OnDeleteClicked()
    {
        if (activeItem == null) return;
        
        itemList.Remove(activeItem);
        SaveCurrentList();
        itemListView.Rebuild();
        itemDetailsSection.visible = false;
    }

    private void OnAddItemClicked()
    {
        // 根据下拉框当前选中的类型创建对应物品
        ItemDetails newItem = dropdownOptions[typeDropdown.value] switch
        {
            ItemType.Equipment => new Equipment(),
            ItemType.Consumable => new Consumable(),
            _ => new ItemDetails()
        };
        
        newItem.ItemName = "新物品";
        newItem.ItemID = GenerateNewID();
        newItem.ItemType = dropdownOptions[typeDropdown.value];
        itemList.Add(newItem);
        SaveCurrentList();
        itemListView.Rebuild();
    }
    
    private void OnItemTypeChanged(ChangeEvent<string> evt)
    {
        // 当下拉框选择变化时加载对应类型的物品列表
        LoadCurrentList();
        GenerateListView();
        itemListView.Rebuild();
        itemListView.selectedIndex = -1;
        itemDetailsSection.visible = false;
    }
    
    private void OnListSelectionChange(IEnumerable<object> selectedItem)
    {
        // 根据当前下拉框的值转换activeItem为具体子类
        activeItem = dropdownOptions[typeDropdown.value] switch
        {
            ItemType.Equipment => (Equipment)selectedItem.FirstOrDefault() ?? new Equipment(),
            ItemType.Consumable => (Consumable)selectedItem.FirstOrDefault() ?? new Consumable(),
            _ => new ItemDetails()
        };
        GetItemDetails();
        itemDetailsSection.visible = true;
    }
    #endregion

    private void LoadDataBase()
    {
        var containerArray = AssetDatabase.FindAssets("ItemContainer_SO");
        if (containerArray.Length > 0)
        {
            var path = AssetDatabase.GUIDToAssetPath(containerArray[0]);
            container = AssetDatabase.LoadAssetAtPath(path, typeof(ItemContainer_SO)) as ItemContainer_SO;
        }
        
        LoadCurrentList();
        EditorUtility.SetDirty(container);
    }
    
    private void LoadCurrentList()
    {
        var selectedType = dropdownOptions[typeDropdown.value];
        itemList = selectedType switch
        {
            ItemType.Equipment => container.equipments?.ItemDetailsList.Cast<ItemDetails>().ToList() ?? new List<ItemDetails>(),
            ItemType.Consumable => container.consumables?.ItemDetailsList.Cast<ItemDetails>().ToList() ?? new List<ItemDetails>(),
            _ => new List<ItemDetails>()
        };
    }
    
    private void SaveCurrentList()
    {
        var selectedType = dropdownOptions[typeDropdown.value];
    
        switch (selectedType)
        {
            case ItemType.Equipment:
                if (container.equipments == null)
                {
                    container.equipments = ScriptableObject.CreateInstance<EquipmentData_SO>();
                    AssetDatabase.AddObjectToAsset(container.equipments, container);
                    EditorUtility.SetDirty(container.equipments); // 新增
                }
                container.equipments.ItemDetailsList = itemList.OfType<Equipment>().ToList();
                EditorUtility.SetDirty(container.equipments); // 新增
                break;
            
            case ItemType.Consumable:
                if (container.consumables == null)
                {
                    container.consumables = ScriptableObject.CreateInstance<ConsumableData_SO>();
                    AssetDatabase.AddObjectToAsset(container.consumables, container);
                    EditorUtility.SetDirty(container.consumables); // 新增
                }
                container.consumables.ItemDetailsList = itemList.OfType<Consumable>().ToList();
                EditorUtility.SetDirty(container.consumables); // 新增
                break;
        }
    
        EditorUtility.SetDirty(container);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(); // 新增：确保所有更改被刷新
    }
    private int GenerateNewID()
    {
        return itemList.Count > 0 ? itemList.Max(item => item.ItemID) + 1 : 1001;
    }
    private void GenerateListView()
    {
        Func<VisualElement> makeItem = () => itemRowTemplate.CloneTree();

        Action<VisualElement, int> bindItem = (e, i) =>
        {
            if (i < itemList.Count)
            {
                if (itemList[i].ItemIcon != null)
                    e.Q<VisualElement>("Icon").style.backgroundImage = itemList[i].ItemIcon.texture;
                e.Q<Label>("Name").text = itemList[i] == null ? "NO ITEM" : itemList[i].ItemName;
            }
        };

        itemListView.fixedItemHeight = 50;  //根据需要高度调整数值
        itemListView.itemsSource = itemList;
        itemListView.makeItem = makeItem;
        itemListView.bindItem = bindItem;
        
        //右侧信息面板不可见
        itemDetailsSection.visible = false;
    }

    private void GetItemDetails()
    {
        itemDetailsSection.MarkDirtyRepaint();
        
        itemDetailsSection.Q<IntegerField>("ItemID").value = activeItem.ItemID;
        itemDetailsSection.Q<IntegerField>("ItemID").RegisterValueChangedCallback(evt =>
        {
            activeItem.ItemID = evt.newValue;
        });

        itemDetailsSection.Q<TextField>("ItemName").value = activeItem.ItemName;
        itemDetailsSection.Q<TextField>("ItemName").RegisterValueChangedCallback(evt =>
        {
            activeItem.ItemName = evt.newValue;
            itemListView.Rebuild();
        });

        iconPreview.style.backgroundImage = activeItem.ItemIcon == null ? defaultIcon.texture : activeItem.ItemIcon.texture;
        itemDetailsSection.Q<ObjectField>("ItemIcon").value = activeItem.ItemIcon;
        itemDetailsSection.Q<ObjectField>("ItemIcon").RegisterValueChangedCallback(evt =>
        {
            Sprite newIcon = evt.newValue as Sprite;
            activeItem.ItemIcon = newIcon;

            iconPreview.style.backgroundImage = newIcon == null ? defaultIcon.texture : newIcon.texture;
            itemListView.Rebuild();
        });
        
        itemDetailsSection.Q<ObjectField>("ItemSprite").value = activeItem.ItemOnWorldSprite;
        itemDetailsSection.Q<ObjectField>("ItemSprite").RegisterValueChangedCallback(evt =>
        {
            activeItem.ItemOnWorldSprite = (Sprite)evt.newValue;
        });
        
        
        //TODO：可以在这里加 针对每种物品的不同属性响应
        //由于加了下拉框逻辑 这里的就只显示 更细节的Type
        switch (activeItem)
        {
            case Equipment equipment:
                itemDetailsSection.Q<EnumField>("ItemType").Init(equipment.equipmentType);
                itemDetailsSection.Q<EnumField>("ItemType").label = "EquipmentType";
                itemDetailsSection.Q<EnumField>("ItemType").value = equipment.equipmentType;
                break;
            case Consumable consumable:
                itemDetailsSection.Q<EnumField>("ItemType").Init(consumable.consumableType);
                itemDetailsSection.Q<EnumField>("ItemType").label = "ConsumableType";
                itemDetailsSection.Q<EnumField>("ItemType").value = consumable.consumableType;
                break;
        }
        
        itemDetailsSection.Q<EnumField>("ItemType").RegisterValueChangedCallback(evt =>
        {
            // 根据当前activeItem的实际类型处理
            switch (activeItem)
            {
                case Equipment equipment:
                    equipment.equipmentType = (Equipment.EquipmentType)evt.newValue;
                    break;
            
                case Consumable consumable:
                    consumable.consumableType = (Consumable.ConsumableType)evt.newValue;
                    break;
            }
        });
        itemDetailsSection.Q<TextField>("Description").value = activeItem.ItemDescription;
        itemDetailsSection.Q<TextField>("Description").RegisterValueChangedCallback(evt =>
        {
            activeItem.ItemDescription = evt.newValue;
        });

        itemDetailsSection.Q<IntegerField>("ItemUseRadius").value = activeItem.ItemUseRadius;
        itemDetailsSection.Q<IntegerField>("ItemUseRadius").RegisterValueChangedCallback(evt =>
        {
            activeItem.ItemUseRadius = evt.newValue;
        });
        
        itemDetailsSection.Q<IntegerField>("Price").value = activeItem.ItemPrice;
        itemDetailsSection.Q<IntegerField>("Price").RegisterValueChangedCallback(evt =>
        {
            activeItem.ItemPrice = evt.newValue;
        });

        itemDetailsSection.Q<Slider>("SellPercentage").value = activeItem.SellPercentage;
        itemDetailsSection.Q<Slider>("SellPercentage").RegisterValueChangedCallback(evt =>
        {
            activeItem.SellPercentage = evt.newValue;
        });
        itemDetailsSection.Q<Toggle>("CanPickedup").value = activeItem.CanPickedup;
        itemDetailsSection.Q<Toggle>("CanPickedup").RegisterValueChangedCallback(evt =>
        {
            activeItem.CanPickedup = evt.newValue;
        });

        itemDetailsSection.Q<Toggle>("CanDropped").value = activeItem.CanDropped;
        itemDetailsSection.Q<Toggle>("CanDropped").RegisterValueChangedCallback(evt =>
        {
            activeItem.CanDropped = evt.newValue;
        });

        itemDetailsSection.Q<Toggle>("CanCarried").value = activeItem.CanCarried;
        itemDetailsSection.Q<Toggle>("CanCarried").RegisterValueChangedCallback(evt =>
        {
            activeItem.CanCarried = evt.newValue;
        });
          itemDetailsSection.Q<Toggle>("CanStack").value = activeItem.CanStack;
        itemDetailsSection.Q<Toggle>("CanStack").RegisterValueChangedCallback(evt =>
        {
            activeItem.CanStack = evt.newValue;
        });
    
        itemDetailsSection.Q<Toggle>("CanInBag").value = activeItem.CanInBag;
        itemDetailsSection.Q<Toggle>("CanInBag").RegisterValueChangedCallback(evt =>
        {
            activeItem.CanInBag = evt.newValue;
        });
         itemDetailsSection.Q<Toggle>("CanWear").value = activeItem.CanWear;
        itemDetailsSection.Q<Toggle>("CanWear").RegisterValueChangedCallback(evt =>
        {
            activeItem.CanWear = evt.newValue;
        });
    
        itemDetailsSection.Q<Toggle>("CanHold").value = activeItem.CanHold;
        itemDetailsSection.Q<Toggle>("CanHold").RegisterValueChangedCallback(evt =>
        {
            activeItem.CanHold = evt.newValue;
        });
    
        itemDetailsSection.Q<Toggle>("CanSell").value = activeItem.CanSell;
        itemDetailsSection.Q<Toggle>("CanSell").RegisterValueChangedCallback(evt =>
        {
            activeItem.CanSell = evt.newValue;
        });
    
        itemDetailsSection.Q<Toggle>("CanUse").value = activeItem.CanUse;
        itemDetailsSection.Q<Toggle>("CanUse").RegisterValueChangedCallback(evt =>
        {
            activeItem.CanUse = evt.newValue;
        });
    
        itemDetailsSection.Q<Toggle>("CanPlace").value = activeItem.CanPlace;
        itemDetailsSection.Q<Toggle>("CanPlace").RegisterValueChangedCallback(evt =>
        {
            activeItem.CanPlace = evt.newValue;
        });
    
        itemDetailsSection.Q<Toggle>("CanRotate").value = activeItem.CanRotate;
        itemDetailsSection.Q<Toggle>("CanRotate").RegisterValueChangedCallback(evt =>
        {
            activeItem.CanRotate = evt.newValue;
        });
    
        itemDetailsSection.Q<Toggle>("CanInteractOnMap").value = activeItem.CanInteractOnMap;
        itemDetailsSection.Q<Toggle>("CanInteractOnMap").RegisterValueChangedCallback(evt =>
        {
            activeItem.CanInteractOnMap = evt.newValue;
        });
    
        itemDetailsSection.Q<Toggle>("CanUseInCrafting").value = activeItem.CanUseInCrafting;
        itemDetailsSection.Q<Toggle>("CanUseInCrafting").RegisterValueChangedCallback(evt =>
        {
            activeItem.CanUseInCrafting = evt.newValue;
        });
    
        itemDetailsSection.Q<Toggle>("CanGift").value = activeItem.CanGift;
        itemDetailsSection.Q<Toggle>("CanGift").RegisterValueChangedCallback(evt =>
        {
            activeItem.CanGift = evt.newValue;
        });
    }
 }
