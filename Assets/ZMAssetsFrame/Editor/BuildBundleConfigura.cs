using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

/// <summary>
/// AB 包资源配置文件
/// AB Package resource configuration file
/// </summary>
[CreateAssetMenu(fileName = "New BuildBundleConfigura", menuName = "AssetBundle", order = 4)]
public class BuildBundleConfigura : ScriptableObject
{
    /// <summary>
    /// BuildBundleConfigura 资源存放路径
    /// BuildBundleConfigura Resource storage path
    /// </summary>
    private static string buildBundleConfiguraPath = "Assets/ZMAssetsFrame/Config/BuildBundleConfigura.asset";

    public static BuildBundleConfigura instance;

    public static BuildBundleConfigura Instance
    {
        get
        {
            if (instance == null)
            {
                instance = AssetDatabase.LoadAssetAtPath<BuildBundleConfigura>(buildBundleConfiguraPath);
            }
            return instance;
        }
    }

    /// <summary>
    /// 模块资源配置
    /// Module resource allocation
    /// </summary>
    public List<BundleModuleData> assetBundleConfigList = new List<BundleModuleData>();

    /// <summary>
    /// 根据资源模块名称获取对应的资源模块数据
    /// Obtain the resource module data based on the resource module name
    /// </summary>
    /// <param name="moduleName">资源模块名称 Resource module name</param>
    /// <returns></returns>
    public BundleModuleData GetBundleModuleDataByModuleName(string moduleName)
    {
        if (assetBundleConfigList.Count == 0) return null;

        foreach (var moduleData in assetBundleConfigList)
        {
            if (string.Equals(moduleData.moduleName, moduleName))
            {
                return moduleData;
            }
        }
        return null;
    }

    /// <summary>
    /// 根据资源模块名称移除对应的资源模块数据
    /// Remove the resource module data based on the resource module name
    /// </summary>
    /// <param name="moduleName">资源模块名称 Resource module name</param>
    public void RemoveBundleModuleDataByModuleName(string moduleName)
    {
        if (assetBundleConfigList.Count == 0) return;

        for (int i = assetBundleConfigList.Count - 1; i >= 0; i--)
        {
            if (string.Equals(assetBundleConfigList[i].moduleName, moduleName))
            {
                assetBundleConfigList.Remove(assetBundleConfigList[i]);
                break;
            }
        }
    }

    /// <summary>
    /// 存储新的资源模块数据
    /// Save new resource module data
    /// </summary>
    /// <param name="moduleData">资源模块数据 Resource module data</param>
    public void SaveModuleData(BundleModuleData moduleData)
    {
        assetBundleConfigList.Add(moduleData);
        Save();
    }

    public void Save()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }
}