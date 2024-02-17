using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BundleTools
{
    /// <summary>
    /// AB 包名枚举类存放路径
    /// </summary>
    private static string bundleModuleEnumFilePath = Application.dataPath + "/ZMAssetsFrame/Config/BundleModuleEnum.cs";

    /// <summary>
    /// 根据 AB 包名 生成对应的枚举名
    /// </summary>
    [MenuItem("GameTool/GenerateBundleModuleEnum")]
    public static void GenerateBundleModuleEnum()
    {
        string namespaceName = "AssetBundleFramework"; // 命名空间
        string enumClassName = "BundleModuleEnum"; // 脚本名

        // 如果存在该脚本 就删除掉 然后刷新
        if (File.Exists(bundleModuleEnumFilePath))
        {
            File.Delete(bundleModuleEnumFilePath);
            AssetDatabase.Refresh();
        }

        using (var writer = File.CreateText(bundleModuleEnumFilePath))
        {
            writer.WriteLine("/* ------------------------------------");
            writer.WriteLine("/* Title:AssetBundle模块类");
            writer.WriteLine("/* Data:" + System.DateTime.Now);
            writer.WriteLine("/* Description: Represents each module which is used to download an load");
            writer.WriteLine("/* 描述: 表示用于加载和下载的模块枚举名类");
            writer.WriteLine("------------------------------------ */");
            writer.WriteLine();

            writer.WriteLine($"namespace {namespaceName}");
            writer.WriteLine("{");
            {
                // 获取所有的资源模块
                List<BundleModuleData> moduleList = BuildBundleConfigura.Instance.assetBundleConfigList;

                if (moduleList == null || moduleList.Count <= 0) return;

                writer.WriteLine("\t" + $"public enum {enumClassName}");
                writer.WriteLine("\t" + "{");
                {
                    writer.WriteLine("\t\tNone,");

                    for (int i = 0; i < moduleList.Count; i++)
                    {
                        writer.WriteLine("\t\t" + moduleList[i].moduleName + ",");
                    }
                }
                writer.WriteLine("\t" + "}");
            }
            writer.WriteLine("}");
            writer.Close();
        }

        AssetDatabase.Refresh();
    }
}