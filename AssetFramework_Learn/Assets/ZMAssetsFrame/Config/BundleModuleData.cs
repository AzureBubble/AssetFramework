using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// AssetBundle 模块资源数据结构类
/// AssetBundle module resource data structure class
/// </summary>
[System.Serializable]
public class BundleModuleData
{
    /// <summary>
    /// AssetBundle 模块ID
    /// AssetBundle ID of the module
    /// </summary>
    public long bundleID;

    /// <summary>
    /// AssetBundle 模块名字
    /// AssetBundle Module name
    /// </summary>
    public string moduleName;

    /// <summary>
    /// 是否打包
    /// Whether to pack
    /// </summary>
    [HideInInspector] public bool isBuild;

    /// <summary>
    /// 上一次点击按钮的时间
    /// The last time the button was clicked
    /// </summary>
    [HideInInspector] public float lastClickBtnTime;

    /// <summary>
    /// 预制体资源路径配置
    /// refabricated resource path
    /// </summary>
    public string[] prefabPathArr;

    /// <summary>
    /// 文件夹子包路径配置
    /// Configure the folder path
    /// </summary>
    public string[] rootFolderPathArr;

    /// <summary>
    /// 单个补丁包路径配置
    /// Single patch package path
    /// </summary>
    public BundleFileInfo[] singleFolderPathArr;
}

[System.Serializable]
public class BundleFileInfo
{
    /// <summary>
    /// AB 包名
    /// AB Name
    /// </summary>
    [LabelText("AB Name")] public string abName;

    /// <summary>
    /// AB 包路径
    /// AB Path
    /// </summary>
    [LabelText("AB Path"), FolderPath] public string bundlePath;
}