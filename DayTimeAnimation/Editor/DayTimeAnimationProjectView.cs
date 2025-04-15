#if UNITY_EDITOR
using PowerUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/// <summary>
/// Project Settings for DayTimeAnimation
/// 
/// path:
/// ProjectSettings/PowerUtils/Environment/DayTimeAnimation
/// </summary>
[ProjectSettingGroup(ProjectSettingGroupAttribute.POWER_UTILS + "/Environment/DayTimeAnimation")]
[SOAssetPath("Assets/PowerUtilities/DayTimeAnimation.asset")]
public class DayTimeAnimationProjectView : ScriptableObject
{
    [HelpBox]
    public string helpBox = "operate DayTimeAnimation";

    public List<DayTimeAnimationDriver> driverList = new();
    public List<DayTimeAnimationItem> itemList = new();

    [EditorButton(onClickCall = "FindDayTimeDriverAndItems")]
    [Tooltip("Find drivers and items in current scene")]
    public bool isFindDriverAndItems;

    [EditorBox("Add Components", "isAddDriver,isAddItem,isAddDaytimeMaterialUpdate,isAddDaytimeRenderSettings", boxType = EditorBoxAttribute.BoxType.HBox,propWidthRates = new[] { 0.2f,.2f,.2f ,.2f})]
    [EditorButton(onClickCall = "AddDriver")]
    [Tooltip("Add DayTimeAnimationDriver to selected gameObjects")]
    public bool isAddDriver;

    [HideInInspector]
    [EditorButton(onClickCall = "AddItem")]
    [Tooltip("Add DayTimeAnimationItem to selected gameObjects")]
    public bool isAddItem;

    [HideInInspector]
    [EditorButton(onClickCall = "AddDaytimeMaterialUpdate")]
    [Tooltip("Add DaytimeMaterialUpdate to selected gameObjects")]
    public bool isAddDaytimeMaterialUpdate;

    [HideInInspector]
    [EditorButton(onClickCall = "AddDaytimeRenderSettings")]
    [Tooltip("Add DaytimeRenderSettings to selected gameObjects")]
    public bool isAddDaytimeRenderSettings;


    public void FindDayTimeDriverAndItems()
    {
        driverList.Clear();
        itemList.Clear();

        var drivers = FindObjectsOfType<DayTimeAnimationDriver>(true);
        driverList.AddRange(drivers);

        var items = FindObjectsOfType<DayTimeAnimationItem>(true);
        itemList.AddRange(items);
    }

    public void AddDriver()
    {
        Selection.gameObjects.ForEach(go =>
        {
            go.GetOrAddComponent<DayTimeAnimationDriver>();
        });
    }

    public void AddItem()
    {
        Selection.gameObjects.ForEach(go =>
        {
            go.GetOrAddComponent<DayTimeAnimationItem>();
        });
    }

    public void AddDaytimeMaterialUpdate()
    {
        Selection.gameObjects.ForEach(go =>
        {
            go.GetOrAddComponent<DaytimeMaterialUpdate>();
        });
    }

    public void AddDaytimeRenderSettings()
    {
        Selection.gameObjects.ForEach(go =>
        {
            go.GetOrAddComponent<DaytimeRenderSettings>();
        });
    }
}
#endif