using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public class AssetBundleModuleConfigWindow : OdinEditorWindow
{
    [PropertySpace(spaceAfter: 5, spaceBefore: 5), Required("Please enter a resource module name(请输入资源模块名称)"), GUIColor(0.3f, 0.8f, 0.8f, 1f)]
    [LabelText("ModuleName")] public string moduleName;  // 资源模块名称

    #region Tab Tooltip

    [ReadOnly, HideLabel, DisplayAsString, TabGroup("Prefab Bundle(预制体包)")]
    public string prefabTabel = "Each prefab in this folder generates a separate AssetBundle";

    [ReadOnly, HideLabel, DisplayAsString, TabGroup("Prefab Bundle(预制体包)")]
    public string prefabTabel_cn = "该文件夹下的所有预制体都会单独生成一个AssetBundle";

    [ReadOnly, HideLabel, DisplayAsString, TabGroup("SubFolder Bundle(文件夹子包)")]
    public string rootFolderSubBundle = "All subfolders in this folder generate an AssetBundle individually";

    [ReadOnly, HideLabel, DisplayAsString, TabGroup("SubFolder Bundle(文件夹子包)")]
    public string rootFolderSubBundle_cn = "该文件夹下的所有子文件夹都会单独生成一个AssetBundle";

    [ReadOnly, HideLabel, DisplayAsString, TabGroup("Single Patch Bundle(单个补丁包)")]
    public string prefabTabelSingleBundle = "The specified folder generates a separate AssetBundle";

    [ReadOnly, HideLabel, DisplayAsString, TabGroup("Single Patch Bundle(单个补丁包)")]
    public string prefabTabelSingleBundle_cn = "指定的文件夹会单独生成一个AssetBundle";

    #endregion

    [FolderPath, LabelText("Prefab Bundle Path(预制体资源路径配置)"), TabGroup("Prefab Bundle(预制体包)")]
    public string[] prefabPathArr = new string[] { "Path..." };

    [FolderPath, LabelText("SubFolder Bundle Path(文件夹子包路径配置)"), TabGroup("SubFolder Bundle(文件夹子包)")]
    public string[] rootFolderPathArr = new string[] { };

    [LabelText("Single Patch Bundle Path(单个补丁包路径配置)"), TabGroup("Single Patch Bundle(单个补丁包)")]
    public BundleFileInfo[] singleFolderPathArr = new BundleFileInfo[] { };

    /// <summary>
    /// 打开模块资源配置窗口
    /// </summary>
    /// <param name="moduleName">模块资源名</param>
    public static void ShowModuleConfigWindow(string moduleName)
    {
        AssetBundleModuleConfigWindow window = GetWindowWithRect<AssetBundleModuleConfigWindow>(new Rect(0, 0, 600, 600));
        window.titleContent = new GUIContent("Module Config Window");
        window.Show();

        // 更新窗口数据
        // Update window data
        BundleModuleData moduleData = BuildBundleConfigura.Instance.GetBundleModuleDataByModuleName(moduleName);

        if (moduleData != null)
        {
            window.moduleName = moduleData.moduleName;
            window.prefabPathArr = moduleData.prefabPathArr;
            window.rootFolderPathArr = moduleData.rootFolderPathArr;
            window.singleFolderPathArr = moduleData.singleFolderPathArr;
        }
    }

    /// <summary>
    /// 存储模块资源配置按钮
    /// Storage module resource configuration button
    /// </summary>
    [OnInspectorGUI]
    public void DrawSaveConfiguraButton()
    {
        GUILayout.BeginArea(new Rect(0, 510, 600, 200));
        // 绘制删除当前资源配置的按钮
        // Draws a button to delete the current resource configuration
        if (GUILayout.Button("Delete Configuration", GUILayout.Height(47)))
        {
            DeleteConfiguration();
        }
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(0, 555, 600, 200));
        // 绘制保存当前资源配置的按钮
        // Draws a button to save the current resource configuration
        if (GUILayout.Button("Save Configuration", GUILayout.Height(47)))
        {
            SaveConfiguration();
        }
        GUILayout.EndArea();
    }

    /// <summary>
    /// 删除当前资源配置
    /// Delete the current resource configuration
    /// </summary>
    public void DeleteConfiguration()
    {
        BuildBundleConfigura.Instance.RemoveBundleModuleDataByModuleName(moduleName);
        EditorUtility.DisplayDialog("Successfully Delete!", $"{moduleName} Configuration Has Deleted", "Confirm");
        Close();
        AssetBundleBuildWindow.ShowAssetBundleWindow();
    }

    /// <summary>
    /// 保存当前资源配置
    /// Save the current resource configuration
    /// </summary>
    public void SaveConfiguration()
    {
        if (string.IsNullOrEmpty(moduleName))
        {
            EditorUtility.DisplayDialog("Fail To Save!", "ModuleName Cannot Be Empty", "Confirm");
            return;
        }

        BundleModuleData moduleData = BuildBundleConfigura.Instance.GetBundleModuleDataByModuleName(moduleName);
        if (moduleData == null)
        {
            // 添加新的模块资源配置
            // Add a new module resource configuration
            moduleData = new BundleModuleData()
            {
                moduleName = this.moduleName,
                prefabPathArr = this.prefabPathArr,
                rootFolderPathArr = this.rootFolderPathArr,
                singleFolderPathArr = this.singleFolderPathArr,
            };
            BuildBundleConfigura.Instance.SaveModuleData(moduleData);
        }
        else
        {
            moduleData.moduleName = this.moduleName;
            moduleData.prefabPathArr = this.prefabPathArr;
            moduleData.rootFolderPathArr = this.rootFolderPathArr;
            moduleData.singleFolderPathArr = this.singleFolderPathArr;
        }

        EditorUtility.DisplayDialog("Save Successfully!", $"{moduleName} Configuration Has Saved", "Confirm");
        Close();
        AssetBundleBuildWindow.ShowAssetBundleWindow();
    }
}