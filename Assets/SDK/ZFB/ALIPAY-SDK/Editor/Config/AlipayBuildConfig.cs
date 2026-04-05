using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlipayBuildConfig : ScriptableObject
{
	public AlipayProjectCfg AlipayProjectCfg;

	public CompileOptions CompileOptions;

}

[Serializable]
public class AlipayProjectCfg
{
	/// <summary>
	/// 游戏appid
	/// </summary>
	public string AppID;

	/// <summary>
	/// 资源加载方式
	/// </summary>
	public string DataFileLoadType;

	/// <summary>
	/// 资源CDN地址
	/// </summary>
	public string CDN;

	/// <summary>
	/// 小游戏项目名
	/// </summary>
	public string ProjectName;

	/// <summary>
	/// 导出路径(绝对路径)
	/// </summary>
	public string DerivedPath = string.Empty;

	/// <summary>
	/// 游戏内存大小(MB)
	/// </summary>
	public int MemorySize = 512;

	/// <summary>
	/// 游戏方向
	/// </summary>
	public AlipayScreenOrientation Orientation = AlipayScreenOrientation.Portrait;

	/// <summary>
	/// 加载页图片
	/// </summary>
	public Texture2D LoadingImage;

	/// <summary>
	/// 自定义bundle中的hash长度
	/// </summary>
	public int BundleHashLength = 32;

	/// <summary>
	/// 判定为下载bundle的路径标识符，此路径下的下载，会自动缓存
	/// </summary>
	public string CacheableFileIdentifier = "StreamingAssets;";

	/// <summary>
	/// 命中路径标识符的情况下，并不是所有文件都有必要缓存，过滤下不需要缓存的文件
	/// </summary>
	public string ExcludeFileIdentifier = "json;";

	/// <summary>
	/// 首包开启压缩资源
	/// </summary>
	public bool UseDataZip = true;

	/// <summary>
	/// 首包开启BR压缩资源
	/// </summary>
	public bool UseDataBrotli = false;

	/// <summary>
	/// 拼接在DATA_CDN和首包资源文件名的路径
	/// </summary>
	public string DataFileSubPath = string.Empty;

	/// <summary>
	/// Unity编辑器2022.3.56 、 团结1.5.x 及以上版本需要开启此选项适配音频
	/// 需要适配Load Type : CompressedInMemory、Steaming 两种格式可以开启此选项
	/// </summary>
	[Tooltip("Unity编辑器2022.3.56 、 团结1.5.x 及以上版本需要开启此选项适配音频；需要适配Load Type : CompressedInMemory、Steaming 两种格式可以开启此选项；")]
	public bool ReplaceUntiyAudioJS = false;
}

public enum AlipayScreenOrientation
{
	Portrait,
	Landscape,
	LandscapeLeft,
	LandscapeRight
}