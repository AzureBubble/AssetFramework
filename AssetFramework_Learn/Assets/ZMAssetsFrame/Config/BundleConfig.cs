using System.Collections.Generic;

[System.Serializable]
public class BundleConfig
{
    /// <summary>
    /// 所有 AssetBundle 的信息列表
    /// </summary>
    public List<BundleInfo> bundleInfoList;
}

/// <summary>
/// AssetBundle 信息
/// </summary>
[System.Serializable]
public class BundleInfo
{
    /// <summary>
    /// 资源文件路径
    /// </summary>
    public string path;

    /// <summary>
    /// Crc
    /// </summary>
    public uint crc;

    /// <summary>
    /// AB 包名
    /// </summary>
    public string bundleName;

    /// <summary>
    /// 资源名称
    /// </summary>
    public string assetName;

    /// <summary>
    /// 资源依赖项
    /// </summary>
    public List<string> bundleDependenciesList;
}

/// <summary>
/// 内嵌的 AssetBundle 的信息
/// </summary>
public class BuiltinBundleInfo
{
    /// <summary>
    /// 文件名
    /// </summary>
    public string fileName;

    /// <summary>
    /// MD5加密码 校验文件(当前解压的模块 没有开启热更)
    /// </summary>
    public string md5;

    /// <summary>
    /// 文件大小
    /// </summary>
    public float size;
}