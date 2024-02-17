using AssetBundleFramework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using ZM.AssetFrameWork;

namespace GameFramework.AssetBundleFramework
{
    /// <summary>
    /// 打包类型
    /// </summary>
    public enum BuildType
    {
        /// <summary>
        /// AB 包
        /// </summary>
        AssetBundle,

        /// <summary>
        /// 热更补丁包
        /// </summary>
        HotPatch,
    }

    public class AssetBundleBuildCompiler
    {
        private static BundleModuleData buildModuleData; // 资源模块配置数据
        private static BuildType buildType; // 打包类型
        private static string hotPatchVersion; // 热更补丁版本
        private static string hotUpdateNotice; // 热更公告
        private static BundleModuleEnum bundleModuleEnum; // 打包模块枚举类型

        private static List<string> allAssetBundlePathList = new List<string>(); // 所有 AssetBundle 文件路径列表

        // 所有文件夹的 AssetBundle 字典
        // key - 父文件夹名  value - 所有子文件夹的路径列表
        private static Dictionary<string, List<string>> allFoldersAssetBundleDic = new Dictionary<string, List<string>>();

        // 所有预制体夹的 AssetBundle 字典
        // key - 父文件夹名  value - 所有子文件夹的路径列表
        private static Dictionary<string, List<string>> allPrefabsAssetBundleDic = new Dictionary<string, List<string>>();

        /// <summary>
        /// AssetBundle 文件输出路径
        /// </summary>
        private static string bundleOutputPath
        { get { return Application.dataPath + "/../AssetBundle/" + bundleModuleEnum + "/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/"; } }

        /// <summary>
        /// 框架内部的Resources路径
        /// </summary>
        private static string resourcesPath
        { get { return Application.dataPath + "/ZMAssetsFrame/Resources/"; } }

        /// <summary>
        /// 初始化打包器
        /// </summary>
        /// <param name="moduleData">资源模块配置数据</param>
        /// <param name="buildType">打包类型</param>
        /// <param name="hotPatchVersion">热更补丁版本</param>
        /// <param name="updateNotice">热更新公告</param>
        public static void Initialize(BundleModuleData moduleData, BuildType buildType = BuildType.AssetBundle, string hotPatchVersion = "0", string updateNotice = "")
        {
            // 每次初始化前先清空缓存 防止数据残留下次打包
            allAssetBundlePathList.Clear();
            allFoldersAssetBundleDic.Clear();
            allPrefabsAssetBundleDic.Clear();

            AssetBundleBuildCompiler.buildModuleData = moduleData;
            AssetBundleBuildCompiler.buildType = buildType;
            AssetBundleBuildCompiler.hotPatchVersion = hotPatchVersion;
            AssetBundleBuildCompiler.hotUpdateNotice = updateNotice;

            bundleModuleEnum = (BundleModuleEnum)Enum.Parse(typeof(BundleModuleEnum), moduleData.moduleName);

            FileHelper.DeleteFolder(bundleOutputPath);
            Directory.CreateDirectory(bundleOutputPath);
        }

        /// <summary>
        /// 打包 AssetBundle
        /// </summary>
        /// <param name="moduleData">资源模块配置数据</param>
        /// <param name="buildType">打包类型</param>
        /// <param name="hotPatchVersion">热更补丁版本</param>
        /// <param name="updateNotice">热更新公告</param>
        public static void BuildAssetBundle(BundleModuleData moduleData, BuildType buildType = BuildType.AssetBundle, string hotPatchVersion = "0", string updateNotice = "")
        {
            // 初始化打包数据
            Initialize(moduleData, buildType, hotPatchVersion, updateNotice);
            // 打包所有文件夹
            BuildAllFolder();
            // 打包父文件夹下的所有子文件夹
            BuildRootSubFolder();
            // 打包所有预制体
            BuildAllPrefabs();

            // 调用 Unity API 进行打包 AssetBundle
            BuildAllAssetBundle();
        }

        #region 打包所有的文件夹 AssetBundle

        /// <summary>
        /// 打包所有的文件夹 AssetBundle
        /// </summary>
        public static void BuildAllFolder()
        {
            if (buildModuleData.singleFolderPathArr == null || buildModuleData.singleFolderPathArr.Length == 0) return;

            for (int i = 0; i < buildModuleData.singleFolderPathArr.Length; i++)
            {
                // 替换斜杠
                string path = buildModuleData.singleFolderPathArr[i].bundlePath.Replace(@"\", "/");

                // 判断资源包是否已经存在
                if (!IsRepeatBundleFile(path))
                {
                    // 添加到列表中缓存
                    allAssetBundlePathList.Add(path);
                    // 获取 AB 包名 : "资源模块名" + "_" + "ab包名"
                    string bundleName = GenerateBundleName(buildModuleData.singleFolderPathArr[i].abName);

                    // 把需要打包的所有 AB 包资源模块 添加到字典中存储
                    if (!allFoldersAssetBundleDic.ContainsKey(bundleName))
                    {
                        allFoldersAssetBundleDic.Add(bundleName, new List<string> { path });
                    }
                    else
                    {
                        allFoldersAssetBundleDic[bundleName].Add(path);
                    }
                }
                else
                {
                    Debug.LogError("RepeatBundleFile:" + path);
                }
            }
        }

        /// <summary>
        /// 是否是重复的 Bundle 文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        private static bool IsRepeatBundleFile(string path)
        {
            if (allAssetBundlePathList.Count > 0)
            {
                foreach (string item in allAssetBundlePathList)
                {
                    // 判断 AB 包是否已经存在了
                    if (string.Equals(item, path) || item.Contains(path) || path.EndsWith(".cs"))
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (path.EndsWith(".cs"))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 生成 AB 包名
        /// </summary>
        /// <param name="abName">ab包名</param>
        /// <returns></returns>
        private static string GenerateBundleName(string abName)
        {
            // "资源模块名" + "_" + "ab包名" 保证打出来的包不一样名字
            return bundleModuleEnum.ToString() + "_" + abName;
        }

        #endregion

        #region 打包父文件夹下的所有子文件夹

        /// <summary>
        /// 打包父文件夹下的所有子文件夹
        /// </summary>
        public static void BuildRootSubFolder()
        {
            // 检测父文件夹是否配置
            if (buildModuleData.rootFolderPathArr == null || buildModuleData.rootFolderPathArr.Length <= 0) return;

            for (int i = 0; i < buildModuleData.rootFolderPathArr.Length; i++)
            {
                // 获取配置父文件夹路径
                string path = buildModuleData.rootFolderPathArr[i] + "/";
                // 获取父文件夹下所有子文件夹路径
                string[] folderArr = Directory.GetDirectories(path);
                // 遍历子文件夹
                foreach (string item in folderArr)
                {
                    // 获取子文件夹路径 并替换斜杠
                    path = item.Replace(@"\", "/");
                    // 获取子文件夹名字作为 AB 包名
                    int nameIndex = path.LastIndexOf('/') + 1;
                    string bundleName = GenerateBundleName(path.Substring(nameIndex));
                    // 判断 AB 包是否已经存在 剔除冗余
                    if (!IsRepeatBundleFile(path))
                    {
                        allAssetBundlePathList.Add(path);

                        if (!allFoldersAssetBundleDic.ContainsKey(bundleName))
                        {
                            allFoldersAssetBundleDic.Add(bundleName, new List<string> { path });
                        }
                        else
                        {
                            allFoldersAssetBundleDic[bundleName].Add(path);
                        }
                    }
                    else
                    {
                        Debug.LogError("RepeatBundle File FolderPath:" + path);
                    }

                    // 处理子文件夹资源的代码
                    // 获取子文件夹下所有的文件
                    string[] filePathArr = Directory.GetFiles(path, "*");
                    foreach (string filePath in filePathArr)
                    {
                        // 过滤 .meta 结尾的文件
                        if (!filePath.EndsWith(".meta"))
                        {
                            string abFilePath = filePath.Replace(@"\", "/");
                            if (!IsRepeatBundleFile(abFilePath))
                            {
                                allAssetBundlePathList.Add(abFilePath);

                                if (!allFoldersAssetBundleDic.ContainsKey(bundleName))
                                {
                                    allFoldersAssetBundleDic.Add(bundleName, new List<string> { abFilePath });
                                }
                                else
                                {
                                    allFoldersAssetBundleDic[bundleName].Add(abFilePath);
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region 打包指定文件夹下的所有预制体

        /// <summary>
        /// 打包指定文件夹下的所有预制体
        /// </summary>
        public static void BuildAllPrefabs()
        {
            if (buildModuleData.prefabPathArr == null || buildModuleData.prefabPathArr.Length <= 0) return;

            // 获取所有预制体的 GUID
            string[] guidArr = AssetDatabase.FindAssets("t:Prefab", buildModuleData.prefabPathArr);
            for (int i = 0; i < guidArr.Length; i++)
            {
                // 通过预制体的 GUID 获取预制体的路径
                string filePath = AssetDatabase.GUIDToAssetPath(guidArr[i]);
                // 获取 AssetBundle 名称
                string bundleName = GenerateBundleName(Path.GetFileNameWithoutExtension(filePath));
                // 如果 AB 包不存在 就计算打包数据
                if (!allAssetBundlePathList.Contains(filePath))
                {
                    // 获取预制体的所有依赖项
                    string[] dependenciesArr = AssetDatabase.GetDependencies(filePath);
                    List<string> dependenciesList = new List<string>();
                    for (int j = 0; j < dependenciesArr.Length; j++)
                    {
                        string path = dependenciesArr[j];
                        // 过滤冗余文件
                        if (!IsRepeatBundleFile(path))
                        {
                            allAssetBundlePathList.Add(path);
                            dependenciesList.Add(path);
                        }
                        else
                        {
                            if (!path.EndsWith(".cs"))
                                Debug.Log("RepeatDependenciesFile Has Packaged:" + path);
                        }
                    }

                    if (!allPrefabsAssetBundleDic.ContainsKey(bundleName))
                    {
                        allPrefabsAssetBundleDic.Add(bundleName, dependenciesList);
                    }
                    else
                    {
                        Debug.LogError("RepeatPrefabName(There are prefabs repeats under the current module) Name:" + bundleName);
                    }
                }
            }
        }

        #endregion

        #region 打包 AssetBundle

        /// <summary>
        /// 打包 AssetBundle
        /// </summary>
        public static void BuildAllAssetBundle()
        {
            // 修改所有要打包的文件的 AssetBundleName
            ModifyAllFileBundleName();
            // 生成一份 AssetBundle 配置
            WriteAssetBundleConfig();

            // 调用 UnityAPI 打包 AssetBundle
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(bundleOutputPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
            if (!manifest)
            {
                EditorUtility.DisplayProgressBar("BuildAssetBundle!", "BuildAssetBundle Failed!", 1);
                Debug.LogError("AssetBundle Build Failed!");
            }
            else
            {
                Debug.Log("AssetBundle Build Success!");
                // 删除所有的 ManifestFile 文件
                DeleteAllAssetBundleManifestFile();
                // 加密所有的 AssetBundle
                EncryptAllAssetBundle();
            }

            ModifyAllFileBundleName(true);
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// 生成 AssetBundle 配置文件
        /// </summary>
        public static void WriteAssetBundleConfig()
        {
            BundleConfig config = new BundleConfig();
            config.bundleInfoList = new List<BundleInfo>();
            // 所有 AssetBundle 文件字典 key-文件路径 value-AssetBundleName
            Dictionary<string, string> allAssetBundleFilePathDic = new Dictionary<string, string>();
            // 获取到工程中所有的 AssetBundleName
            string[] allAssetBundleArr = AssetDatabase.GetAllAssetBundleNames();
            foreach (var bundleName in allAssetBundleArr)
            {
                // 获取指定 AssetBundleName 下的所有文件路径
                string[] bundleFileArr = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);

                foreach (var filePath in bundleFileArr)
                {
                    if (!filePath.EndsWith(".cs"))
                    {
                        allAssetBundleFilePathDic.Add(filePath, bundleName);
                    }
                }
            }

            // 计算 AssetBundle 数据 生成 AssetBundle 配置文件
            foreach (var item in allAssetBundleFilePathDic)
            {
                // 获取文件路径
                string filePath = item.Key;
                if (!filePath.EndsWith(".cs"))
                {
                    BundleInfo info = new BundleInfo();
                    info.path = filePath;
                    info.bundleName = item.Value;
                    info.assetName = Path.GetFileName(filePath);
                    info.crc = Crc32.GetCrc32(filePath);
                    info.bundleDependenciesList = new List<string>();

                    string[] dependencie = AssetDatabase.GetDependencies(filePath);
                    foreach (var dependenciePath in dependencie)
                    {
                        // 如果依赖项不是当前的这个文件 以及依赖项不是cs脚本 就进行处理
                        if (!dependenciePath.Equals(filePath) && dependenciePath.EndsWith(".cs"))
                        {
                            if (allAssetBundleFilePathDic.TryGetValue(dependenciePath, out string assetBundleName))
                            {
                                // 如果依赖项已经包好这个 AssetBundle 就不进行处理 否则添加进依赖项
                                if (!info.bundleDependenciesList.Contains(assetBundleName))
                                {
                                    info.bundleDependenciesList.Add(assetBundleName);
                                }
                            }
                        }
                    }

                    config.bundleInfoList.Add(info);
                }
            }

            // 生成 AssetBundle 配置文件
            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            // json 生成路径
            string bundleConfigPath = Application.dataPath + "/" + bundleModuleEnum.ToString().ToLower() + "assetbundleconfig.json";
            using (var writer = File.CreateText(bundleConfigPath))
            {
                writer.Write(json);
                writer.Dispose();
                writer.Close();
            }
            // 修改 AssetBundle 配置文件的 AssetBundleName
            AssetImporter importer = AssetImporter.GetAtPath(bundleConfigPath.Replace(Application.dataPath, "Assets"));
            if (importer != null)
            {
                importer.assetBundleName = bundleModuleEnum.ToString().ToLower() + "bundleconfig.unity";
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 修改或清空 AssetBundle
        /// </summary>
        /// <param name="clear">是否需要清空修改过的文件</param>
        private static void ModifyAllFileBundleName(bool clear = false)
        {
            int i = 0;
            // 修改所有文件夹下 AssetBundle Name
            foreach (var item in allFoldersAssetBundleDic)
            {
                i++;
                EditorUtility.DisplayProgressBar("Modify AssetBundle Name", "Name:" + item.Key, i * 1.0f / allFoldersAssetBundleDic.Count);
                foreach (string path in item.Value)
                {
                    AssetImporter importer = AssetImporter.GetAtPath(path);
                    if (importer != null)
                    {
                        importer.assetBundleName = (clear ? "" : item.Key + ".unity");
                    }
                }
            }

            i = 0;
            // 修改所有预制体的 AssetBundle Name
            foreach (var item in allPrefabsAssetBundleDic)
            {
                i++;
                foreach (string path in item.Value)
                {
                    EditorUtility.DisplayProgressBar("Modify AssetBundle Name", "Name:" + item.Key, i * 1.0f / allPrefabsAssetBundleDic.Count);
                    AssetImporter importer = AssetImporter.GetAtPath(path);
                    if (importer != null)
                    {
                        importer.assetBundleName = (clear ? "" : item.Key + ".unity");
                    }
                }
            }

            // 移除未使用的 AssetBundleName
            if (clear)
            {
                string bundleConfigPath = Application.dataPath + "/" + bundleModuleEnum.ToString().ToLower() + "assetbundleconfig.json";
                AssetImporter importer = AssetImporter.GetAtPath(bundleConfigPath.Replace(Application.dataPath, "Assets"));
                if (importer != null)
                {
                    importer.assetBundleName = "";
                }

                AssetDatabase.RemoveUnusedAssetBundleNames();
            }
        }

        #endregion

        /// <summary>
        /// 删除所有 AssetBundle 自动生成的清单文件
        /// </summary>
        private static void DeleteAllAssetBundleManifestFile()
        {
            string[] filePathArr = Directory.GetFiles(bundleOutputPath);

            foreach (var path in filePathArr)
            {
                // 删除 ".manifest" 结尾的文件
                if (path.EndsWith(".manifest"))
                {
                    File.Delete(path);
                }
            }
        }

        /// <summary>
        /// 加密所有的 AssetBundle
        /// </summary>
        private static void EncryptAllAssetBundle()
        {
            // 获取文件夹信息
            DirectoryInfo directoryInfo = new DirectoryInfo(bundleOutputPath);
            // 获取文件夹下所有的文件信息
            FileInfo[] fileInfoArr = directoryInfo.GetFiles("*", SearchOption.AllDirectories);

            for (int i = 0; i < fileInfoArr.Length; i++)
            {
                EditorUtility.DisplayProgressBar("加密文件", "Name:" + fileInfoArr[i].Name, i * 1.0f / fileInfoArr.Length);
                // 加密路径 加密密钥
                AES.AESFileEncrypt(fileInfoArr[i].FullName, "sztu");
            }
            EditorUtility.ClearProgressBar();
            Debug.Log("AssetBundle Encrypt Finished!");
        }

        public static void CopyAssetBundleToStreamingAssets(BundleModuleData moduleData, bool showTips = true)
        {
            bundleModuleEnum = (BundleModuleEnum)Enum.Parse(typeof(BundleModuleEnum), moduleData.moduleName);

            // 获取目标文件夹信息
            DirectoryInfo directoryInfo = new DirectoryInfo(bundleOutputPath);
            // 获取文件夹下所有的文件信息
            FileInfo[] fileInfoArr = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
            // AssetBundle 内嵌的目标文件夹
            string streamingAssetsPath = Application.streamingAssetsPath + "/AssetBundle/" + bundleModuleEnum + "/";
            // 防止上一次资源残留
            FileHelper.DeleteFolder(streamingAssetsPath);
            Directory.CreateDirectory(streamingAssetsPath);

            List<BuiltinBundleInfo> bundleInfoList = new List<BuiltinBundleInfo>();

            // 把 AssetBundle 文件拷贝到 streamingAssetsPath
            for (int i = 0; i < fileInfoArr.Length; i++)
            {
                EditorUtility.DisplayProgressBar("内嵌资源中", "Name:" + fileInfoArr[i].Name, i * 1.0f / fileInfoArr.Length);
                File.Copy(fileInfoArr[i].FullName, streamingAssetsPath + fileInfoArr[i].Name);
                // 生成内嵌资源文件信息
                BuiltinBundleInfo info = new BuiltinBundleInfo()
                {
                    fileName = fileInfoArr[i].Name,
                    md5 = MD5.GetMd5FromFile(fileInfoArr[i].FullName),
                    size = fileInfoArr[i].Length / 1024,
                };
                //info.fileName = fileInfoArr[i].Name;
                //info.md5 = MD5.GetMd5FromFile(fileInfoArr[i].FullName);
                //info.size = fileInfoArr[i].Length / 1024;
                bundleInfoList.Add(info);
            }

            // 生成配置文件json数据 并写入Resources文件夹
            string json = JsonConvert.SerializeObject(bundleInfoList, Formatting.Indented);

            if (!Directory.Exists(resourcesPath))
            {
                Directory.CreateDirectory(resourcesPath);
            }

            FileHelper.WriteFile(resourcesPath + bundleModuleEnum + "info.json", Encoding.UTF8.GetBytes(json));
            AssetDatabase.Refresh();

            EditorUtility.ClearProgressBar();
            if (showTips)
            {
                EditorUtility.DisplayDialog("内嵌操作", "内嵌资源完成 Path:" + streamingAssetsPath, "确认");
            }
            Debug.Log("AssetsBundle Copy to StreamingAssets Finish!");
        }
    }
}