using System;
using System.IO;
using AlipaySdk.Editor.CompressionTool;

public class AlipayAssetCompressionCore
{
    private const string webglminFolderName = "webgl-min";

    /// <summary>
    /// 压缩纹理资源处理
    /// </summary>
    /// <param name="completeCallback"> 流程执行结束后产生的回调Action，回调形参1(bool)为是否执行成功，形参2(string)异常时返回的错误提示信息</param>
    /// <param name="bundleDir">输入的bundle目录</param>
    /// <param name="outputDir"> bundle资源处理后的存放路径</param>
    /// <param name="debug"> 调试模式 true:仅生成ASTC false:全量模式(ASTC、DXT5、ETC2、PNG-min)</param>
    /// <param name="force">强制全部重新生成，默认采用增量模式</param>
    public static void CompressText(Action<bool, string> completeCallback, string bundleDir = null,
           string outputDir = null, bool debug = false, bool force = false)
    {

        string errorMsg = string.Empty;
        string inputFolderPath = string.IsNullOrEmpty(bundleDir) ? Path.Combine(AlipayUtil.GetAlipayBuildConfig().AlipayProjectCfg.DerivedPath, "webgl", "StreamingAssets") : bundleDir;
        string outputRootPath =
              Path.Combine(string.IsNullOrEmpty(outputDir) ? AlipayUtil.GetAlipayBuildConfig().AlipayProjectCfg.DerivedPath : outputDir,
                  webglminFolderName);
        if (string.IsNullOrEmpty(inputFolderPath) || string.IsNullOrEmpty(outputRootPath))
        {
            errorMsg = "输入/输出路径不能为空！";
        }

        if (!string.IsNullOrEmpty(errorMsg))
        {
            completeCallback(false, errorMsg);
            return;
        }
        AlipayAssetsTextTools.CompressText(completeCallback, inputFolderPath, outputRootPath, debug, force);
    }

    public static void CompressTextAsync(Action<bool, string> completeCallback, string bundleDir = null,
            string outputDir = null, bool debug = false, bool force = false, IProgress<ProgressReport> progress = null)
    {
        string errorMsg = string.Empty;
        string inputFolderPath = string.IsNullOrEmpty(bundleDir) ? Path.Combine(AlipayUtil.GetAlipayBuildConfig().AlipayProjectCfg.DerivedPath, "webgl", "StreamingAssets") : bundleDir;
        string outputRootPath =
              Path.Combine(string.IsNullOrEmpty(outputDir) ? AlipayUtil.GetAlipayBuildConfig().AlipayProjectCfg.DerivedPath : outputDir,
                  webglminFolderName);
        if (string.IsNullOrEmpty(inputFolderPath) || string.IsNullOrEmpty(outputRootPath))
        {
            errorMsg = "输入/输出路径不能为空！";
        }

        if (!string.IsNullOrEmpty(errorMsg))
        {
            completeCallback(false, errorMsg);
            return;
        }
        AlipayAssetsTextTools.CompressTextAsync(completeCallback, inputFolderPath, outputRootPath, debug, force, progress);
    }
}
