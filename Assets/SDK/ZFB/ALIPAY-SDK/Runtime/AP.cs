#if SDK_ZFB
using System;
using LitJson;
using AlipaySdk.Ad;
using UnityEngine;

namespace AlipaySdk
{
    /// <summary>
    /// 支付宝SDK简化调用后API
    /// </summary>
    public class AP
    {
        #region 设备、系统信息

        /// <summary>
        ///  判断小游戏入参或返回值、组件、属性等是否在当前版本可用。
        ///  详情链接：https://opendocs.alipay.com/mini-game/a7e1af7b_my.canIUse?pathHash=1e43471e
        ///  支付宝客户端：10.5.30版本以上使用
        /// Tip
        /// 对于没有兼容性问题的 API 出入参字段或组件属性，my.canIUse 会始终返回 false，请注意规避。
        /// 对于异步接口，[接口名].return 代表 success 回调的参数；对于同步接口，则代表接口的返回值。
        /// </summary>
        /// 
        /// <param name="schema">
        /// 如果想判断 API 是否可用，入参需要形如 ${API}.${type}.${param}.${option}：
        /// ${API} 表示 API 的名称，不包括 my. 的名称。例如：想判断 my.getFileInfo，只需传入 getFileInfo 即可。
        /// ${type} 表示 API 的调用方式，有效值为 object/return/callback。
        /// ${param} 表示参数的某一个属性名。
        /// ${option} 表示参数属性的具体属性值。
        /// 如果想判断组件是否可用，入参需要形如 ${component}.${attribute}.${option}：
        /// ${component} 表示组件名称。
        /// ${attribute} 表示组件属性名。
        /// ${option} 表示组件属性值。
        /// </param>
        /// 
        /// <returns> 当前版本是否可用，true 表示可用，false 表示不可用。</returns>
        public static bool CanIUse(string schema)
        {
            return AlipaySDK.API.CanIUse(schema);
        }

        /// <summary>
        /// 判断当前运行环境是否为支付宝小程序开发者工具（IDE）。
        ///
        /// 详情链接：https://opendocs.alipay.com/mini-game/b2b4ee2a_my.isIDE?pathHash=d6dddb68
        ///
        /// 支付宝客户端：10.5.30版本以上使用；<b>基础库：2.1.15及以上版本</b>。
        ///
        /// 返回 true 表示当前环境为 IDE，false 表示为真机或其他环境。
        /// </summary>
        /// <returns>
        /// 是否为开发者工具(IDE)环境，true 为 IDE，false 为非 IDE。
        /// </returns>
        public static bool IsIDE()
        {
            return AlipaySDK.API.IsIDE();
        }

        /// <summary>
        /// 异步获取手机系统信息
        /// 详情链接：https://opendocs.alipay.com/mini-game/08uszk?pathHash=51167f81
        /// 支付宝客户端：10.5.30版本以上使用
        /// 常见问题
        /// Q：my.getSystemInfo 中 windowHeight 和 screenHeight 有什么区别？
        /// A：screenHeight 是整个手机屏幕的高度，windowHeight 是 webview（不包括手机状态栏、小游戏标题栏和 tabBar）的高度。
        /// </summary>
        /// <param name="result">
        /// success 回调函数：
        /// 属性	类型	兼容性	描述 
        /// pixelRatio	Number	- 设备像素比 
        /// screenHeight	Number	- 屏幕高度。单位：px。 
        /// screenWidth	Number	- 屏幕宽度。单位：px。 
        /// windowWidth	Number	- 可使用窗口宽度。单位：px 
        /// windowHeight	Number	- 可使用窗口高度。 单位：px 
        /// model	String	- 手机型号。具体可参考 手机型号与 my.getSystemInfo 返回的 model 值对照表 
        /// system	String	- 系统版本 
        /// platform	String	- 客户端平台：Android，iOS / iPhone OS 
        /// language	String	- 支付宝设置的语言 枚举值	描述	兼容性 zh-Hans 简体中文 - en 英文 - zh-Hant 繁体中文（台湾） - zh-HK 繁体中文（香港） - 
        /// version	String	- 支付宝客户端版本号 
        /// isIphoneXSeries	Boolean 基础库: 2.6.0+ 是否 iphoneX 系列 
        /// pcPlatform	String 平台: 仅pc端小游戏+ pc端小游戏当前运行环境 枚举值	描述	兼容性 Browser 浏览器 -
        /// </param>
        public static void GetSystemInfo(Action<string> result)
        {
            AlipaySDK.API.GetSystemInfo(result);
        }

        /// <summary>
        /// 同步获取手机系统信息的同步接口, 返回值同GetSystemInfo的result
        ///  详情链接：https://opendocs.alipay.com/mini-game/08v96p?pathHash=0838e22e
        /// 支付宝客户端：10.5.30版本以上使用
        /// </summary>
        public static string GetSystemInfoSync()
        {
            return AlipaySDK.API.GetSystemInfoSync();
        }

        /// <summary>
        /// 获取用户当前地理位置信息，包括经纬度、省市区、街道和POI等。
        /// 文档：https://opendocs.alipay.com/mini-game/08vb7q?pathHash=fd4cc44e
        ///
        /// 支付宝客户端：10.5.30版本以上使用。
        ///
        /// type参数支持：
        /// 0 - 获取经纬度；
        /// 1 - 获取经纬度和省市区县数据；
        /// 2 - 获取经纬度、省市区县和街道数据；
        /// 3 - 获取经纬度、省市区县、街道和周边POI数据。
        ///
        /// cacheTimeout为定位结果缓存有效期（秒），默认30秒。
        ///
        /// 回调参数为JSON字符串，成功时包含经纬度、行政区域等字段，失败时包含error和errorMessage。
        /// </summary>
        /// <param name="result">定位结果回调，参数为JSON字符串，成功/失败结构参考官方文档。</param>
        /// <param name="type">定位类型（默认0）。0=经纬度，1=含省市区，2=含街道，3=含POI。</param>
        /// <param name="cacheTimeout">缓存时长（秒），默认30。</param>
        public static void GetLocation(Action<string> result, int type = 0, int cacheTimeout = 30)
        {
            AlipaySDK.API.GetLocation(result, type, cacheTimeout);
        }

        /// <summary>
        /// 获取支付宝小程序顶部菜单按钮（右上角胶囊按钮）的布局位置信息。
        ///
        /// 详情链接：https://opendocs.alipay.com/mini-game/b96ec016_my.getMenuButtonBoundingClientRect?pathHash=9952aa77
        ///
        /// 支付宝客户端：10.1.90版本以上使用；<b>基础库：2.1.15及以上版本</b>。
        ///
        /// 返回值为JSON字符串，包含胶囊按钮的宽高和在屏幕中的位置信息，包括：
        /// - width：按钮宽度（单位px）
        /// - height：按钮高度（单位px）
        /// - top：按钮上边界坐标（单位px，以屏幕左上角为原点）
        /// - bottom：按钮下边界坐标（单位px，以屏幕左上角为原点）
        /// - left：按钮左边界坐标（单位px，以屏幕左上角为原点）
        /// - right：按钮右边界坐标（单位px，以屏幕左上角为原点）
        ///
        /// <para>注意：</para>
        /// 部分历史旧版本 IDE 与 iOS 客户端，或胶囊未渲染时返回的位置信息可能为0或负数，建议兼容处理或适当延时调用。
        ///
        /// <para>返回示例：</para>
        /// <code>
        /// {
        ///   "width": 88,
        ///   "height": 32,
        ///   "top": 44,
        ///   "bottom": 76,
        ///   "left": 287,
        ///   "right": 375
        /// }
        /// </code>
        /// </summary>
        /// <returns>
        /// 返回JSON字符串，包含胶囊按钮各项布局属性，字段详见官方文档。
        /// </returns>
        public static string GetMenuButtonBoundingClientRect()
        {
            return AlipaySDK.API.GetMenuButtonBoundingClientRect();
        }

        /// <summary>
        /// 获取当前小游戏的支付宝环境变量信息。
        ///
        /// 详情链接：https://opendocs.alipay.com/mini-game/b8ba0233_my.env?pathHash=f2ab8543
        ///
        /// 支付宝客户端：10.5.30版本以上使用；<b>基础库：2.1.15及以上版本</b>。
        ///
        /// 返回值为JSON字符串，包含环境变量相关信息，比 my.getSystemInfoSync 更轻量，包括：
        /// - USER_DATA_PATH：文件系统中的用户目录（本地用户文件路径）
        /// - clientName：客户端名称简写，支付宝为 'ap'
        /// - clientVersion：客户端版本号，例如 '10.2.6'
        /// - clientEnv：客户端环境（'prod' 生产，'sandbox' 沙箱，基础库 2.7.24+ 支持）
        /// - language：客户端设置的语言（如 'zh-Hans', 'en' 等）
        /// - platform：系统名称（'iOS', 'Android', 'unknown'）
        ///
        /// <para>返回示例：</para>
        /// <code>
        /// {
        ///   "USER_DATA_PATH": "/usr/data/",
        ///   "clientName": "ap",
        ///   "clientVersion": "10.2.6",
        ///   "clientEnv": "prod",
        ///   "language": "zh-Hans",
        ///   "platform": "iOS"
        /// }
        /// </code>
        /// </summary>
        /// <returns>
        /// JSON字符串，包含当前支付宝小游戏运行环境的主要变量信息，字段详见官方文档。
        /// </returns>
        public static string env => AlipaySDK.API.GetAlipayEnv();

        /// <summary>
        /// 同步获取窗口信息。
        ///
        /// 详情链接：https://opendocs.alipay.com/mini-game/6cc95671_my.getWindowInfo?pathHash=c369f8d3
        ///
        /// 支付宝客户端：10.5.30版本以上使用；<b>基础库：2.1.31及以上版本</b>。
        ///
        /// 返回当前设备的窗口信息，包括屏幕尺寸、窗口尺寸、像素比、安全区域等。返回值为JSON字符串，典型字段包括：
        /// - pixelRatio：设备像素比
        /// - screenWidth：屏幕宽度（px）
        /// - screenHeight：屏幕高度（px）
        /// - windowWidth：可用窗口宽度（px）
        /// - windowHeight：可用窗口高度（px）
        /// - statusBarHeight：状态栏高度（px）
        /// - safeArea：竖屏正方向下的安全区域，包括left、right、top、bottom、height、width、screenTop等
        ///
        /// <para>示例数据：</para>
        /// <code>
        /// {
        ///   "pixelRatio": 3,
        ///   "screenWidth": 375,
        ///   "screenHeight": 812,
        ///   "windowWidth": 375,
        ///   "windowHeight": 724,
        ///   "statusBarHeight": 44,
        ///   "safeArea": {
        ///     "left": 0,
        ///     "right": 375,
        ///     "top": 44,
        ///     "bottom": 812,
        ///     "height": 768,
        ///     "width": 375,
        ///     "screenTop": 44
        ///   }
        /// }
        /// </code>
        /// </summary>
        /// <returns>
        /// 返回JSON字符串，包含窗口详细信息，字段见文档说明。
        /// </returns>
        public static string GetWindowInfo()
        {
            return AlipaySDK.API.GetWindowInfo();
        }

        /// <summary>
        /// 判断当前小游戏是否已被用户收藏。
        ///
        /// 详情链接：https://opendocs.alipay.com/mini-game/6d0b314f_my.isCollected?pathHash=756d2dfa
        ///
        /// 支付宝客户端：10.1.65版本以上使用；<b>基础库：2.1.15及以上版本</b>。
        ///
        /// 调用后，可获取当前企业支付宝小程序是否被添加为收藏。
        /// </summary>
        /// <param name="result">
        /// 回调。参数为JSON字符串，成功时包含 <c>isCollected</c> 字段（bool），表示是否已被收藏。失败时含 error 和 errorMessage 字段。
        /// </param>
        public static void IsCollected(Action<string> result)
        {
            AlipaySDK.API.IsCollected(result);
        }

        /// <summary>
        /// 获取系统设置（蓝牙、定位、Wi-Fi 的系统开关状态）。
        /// 文档：https://opendocs.alipay.com/mini-game/642022e8_my.getSystemSetting?pathHash=ba31c108
        ///
        /// 支付宝客户端：10.5.30版本以上使用，基础库：2.1.59及以上版本。
        ///
        /// 返回值为 JSON 字符串，字段说明：
        /// {
        ///     "bluetoothEnabled": bool, // 蓝牙系统开关
        ///     "locationEnabled": bool,  // 定位系统开关
        ///     "wifiEnabled": bool       // Wi-Fi 系统开关
        /// }
        /// </summary>
        /// <returns>系统设置结果，JSON字符串。</returns>
        public static string GetSystemSetting()
        {
            return AlipaySDK.API.GetSystemSetting();
        }

        /// <summary>
        ///  设置 设备分辨率
        /// 支付宝客户端：10.6.80版本以上使用
        /// </summary>
        /// <param name="scale">设置分辨率缩放，范围大于0，小于设备当前的pixelRatio
        ///  通过GetSystemInfo 获取当前最大的pixelRatio
        ///  </param>
        /// <returns> 返回成功信息</returns>
        public static bool SetDevicePixelRatio(float scale)
        {
            var success = AlipaySDK.API.SetDevicePixelRatio(scale);
            if (!success) return false;
            var eventSystem = UnityEngine.EventSystems.EventSystem.current ?? GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
            if (eventSystem == null)
            {
                Debug.LogError("[SetDevicePixelRatioError] 当前场景无EventSystem，分辨率设置可能无效！");
                return false;
            }

            var inputOverride = eventSystem.GetComponent<AlipayTouchInputOverride>();
            if (inputOverride == null)
            {
                inputOverride = eventSystem.gameObject.AddComponent<AlipayTouchInputOverride>();
            }
            AlipayTouchInputOverride.SetDevicePixelRatio(scale);
            return true;
        }

        /// <summary>
        /// 重启小游戏
        /// </summary>
        /// <param name="result">重启结果</param>
        public static void RestartMiniProgram(Action<string> result)
        {
            AlipaySDK.API.RestartMiniProgram(result);
        }

        #endregion

        #region 用户授权，会员信息
        /// <summary>
        /// 获取用户授权码（authCode）。
        /// 
        /// 详情链接：https://opendocs.alipay.com/mini-game/08v96k?pathHash=75b68418
        /// 
        /// 支付宝客户端：10.5.30版本以上使用。
        ///
        /// 用于引导用户授权，将其信息授权给当前小游戏，获取授权码（authCode）。
        /// 后续须由小游戏服务端使用 authCode 换取用户 ID（open_id）、昵称、头像等信息。
        ///
        /// 注意事项：
        /// - 建议不要在小游戏首屏立即调用，否则可能涉及合规问题。
        /// - 调用前应让用户充分了解授权用途，以免影响用户体验或引发合规风险。
        /// 
        /// <para>常见用途：</para>
        /// <list type="bullet">
        ///  <item>换取 open_id（支付宝会员唯一标识）。</item>
        ///  <item>服务端用 authCode 获取用户昵称、头像等信息。</item>
        /// </list>
        /// 
        /// <para>入参 scopes：</para>
        /// <list type="bullet">
        ///  <item>"auth_base"：授权获取 open_id（默认为静默授权，不弹窗，仅获取 open_id）。</item>
        ///  <item>"auth_user"：授权获取用户基础信息（会弹窗，需提前隐私申请）。</item>
        ///  <item>支持字符串或字符串数组传入scope，可一次性申请多个授权类型。</item>
        /// </list>
        ///
        /// <para>回调说明：</para>
        /// <list type="bullet">
        ///  <item>调用成功，返回 result 回调，包含 authCode、authSuccessScopes（成功的 scope 列表）、authErrorScopes（失败的 scope 和错误码映射）。</item>
        ///  <item>调用失败，回调参数含 error（错误码）、errorMessage（错误说明）。</item>
        /// </list>
        ///
        /// <para>常见错误码：</para>
        /// <list type="bullet">
        ///  <item>3：未找到授权结果，scopes无效；</item>
        ///  <item>11：用户取消授权；</item>
        ///  <item>12、13、14：网络错误或超时；</item>
        ///  <item>15：调用过于频繁被限流，需4小时后重试。</item>
        /// </list>
        ///
        /// <para>示例：</para>
        /// <example>
        /// GetAuthCode(new []{"auth_base"}, result => {
        ///    // result 是JSON，如：{"authCode":"16dcd2xxxxxxx","authSuccessScopes":["auth_base"],"authErrorScopes":{}}
        ///    // 通常需将authCode上传服务端，服务端用以获取用户open_id或更多信息。
        /// });
        /// </example>
        /// </summary>
        /// <param name="scopes">
        /// 授权类型，支持单个字符串或字符串数组。常用："auth_base"（仅获取 open_id），"auth_user"（获取用户基础信息，需隐私申请）。
        /// </param>
        /// <param name="result">
        /// 授权结果回调。参数内容为JSON字符串，包含：
        ///   - authCode: 授权码（string）
        ///   - authSuccessScopes: 成功授权的 scope 数组（string[]）
        ///   - authErrorScopes: 授权失败的 scope 及错误码（Dictionary）
        ///  失败时含有 error（错误码）、errorMessage（错误内容）。
        /// </param>
        public static void GetAuthCode(string[] scopes, Action<string> result)
        {
            AlipaySDK.API.GetAuthCode(scopes, result);
        }

        /// <summary>
        /// 获取用户头像
        /// 支付宝客户端：10.5.30版本以上使用。
        /// </summary>
        /// <param name="url">用户头像URL</param>
        /// <param name="avatarInfoResult">头像返回信息，头像图片数据 / 错误信息</param>
        public static void GetAuthAvatar(string url, Action<AlipayAvatarInfo> avatarInfoResult)
        {
            AlipaySDK.API.GetAuthAvatar(url, avatarInfoResult);
        }

        /// <summary>
        /// 获取支付宝会员信息（用户昵称与头像）。
        /// 
        /// 详情链接：https://opendocs.alipay.com/mini-game/08vab1?pathHash=7f335aa9
        /// 
        /// 支付宝客户端：10.5.30版本以上使用。
        ///
        /// 本接口用于在客户端获取当前支付宝用户的会员信息（昵称、头像）。
        /// <para>注意：</para>
        /// <list type="bullet">
        /// <item>调用本接口前，务必先调用 my.getAuthCode 并获取到 "auth_user" 授权，未授权将无法获取信息。</item>
        /// </list>
        /// 
        /// <para>回调说明：</para>
        /// <list type="bullet">
        /// <item>调用成功时，result 回调参数为 JSON 字符串，包含：</item>
        /// <item>- nickName：用户昵称</item>
        /// <item>- avatar：用户头像链接</item>
        /// </list>
        /// <list type="bullet">
        /// <item>调用失败时，result 回调参数通常为空或包含错误信息。</item>
        /// </list>
        ///
        /// <para>示例用法：</para>
        /// <example>
        /// GetAuthCode(new[] { "auth_user" }, codeResult => {
        ///     // 成功拿到授权码后再获取用户信息
        ///     GetAuthUserInfo(userInfoResult => {
        ///         // userInfoResult 为json字符串，例如：
        ///         // { "nickName":"张三","avatar":"https://xxx.png" }
        ///     });
        /// });
        /// </example>
        /// </summary>
        /// <param name="result">
        /// 授权结果回调。参数内容为 JSON 字符串，包括：
        ///   - nickName: 用户昵称（string）
        ///   - avatar: 用户头像链接（string）
        /// </param>
        public static void GetAuthUserInfo(Action<string> result)
        {
            AlipaySDK.API.GetAuthUserInfo(result);
        }

        /// <summary>
        /// 获取当前支付宝客户端及小游戏基础库的基础信息。
        ///
        /// 详情链接：https://opendocs.alipay.com/mini-game/8cecc33d_my.getAppBaseInfo?pathHash=c82f8ade
        ///
        /// <para>兼容性说明：</para>
        /// <list type="bullet">
        ///   <item>仅企业支付宝小程序可用。</item>
        ///   <item>支付宝客户端支持；支小宝客户端与安诊儿客户端不支持。</item>
        ///   <item>基础库版本需 >= 2.1.59。</item>
        ///   <item>如需适配多个客户端或低版本，建议自行兼容处理。</item>
        /// </list>
        ///
        /// <para>返回值说明：</para>
        /// <list type="bullet">
        ///   <item>返回值为JSON字符串，包含客户端基础信息。</item>
        ///   <item>字段 <c>SDKVersion</c>：小游戏基础库版本。</item>
        ///   <item>字段 <c>language</c>：当前支付宝设置语言。常见枚举值包括：</item>
        ///   <list type="number">
        ///     <item>zh-Hans：简体中文</item>
        ///     <item>en：英文</item>
        ///     <item>zh-Hant：繁体中文（台湾）</item>
        ///     <item>zh-Hans：繁体中文（香港）</item>
        ///   </list>
        ///   <item>字段 <c>version</c>：支付宝客户端版本号。</item>
        /// </list>
        ///
        /// <para>示例用法：</para>
        /// <example>
        /// string baseInfo = GetAppBaseInfo();
        /// // baseInfo如：{ "SDKVersion": "2.1.59", "language": "zh-Hans", "version": "10.5.32" }
        /// </example>
        ///
        /// <para>Tips：</para>
        /// <list type="bullet">
        /// <item>建议先判断 <c>SDKVersion</c> 是否达到最低需求后再做后续能力调用。</item>
        /// <item>部分客户端或低版本可能不支持该接口，注意容错兼容。</item>
        /// </list>
        /// </summary>
        /// <returns>支付宝客户端基础信息（JSON字符串），包含SDKVersion、language、version等字段。</returns>
        public static string GetAppBaseInfo()
        {
            return AlipaySDK.API.GetAppBaseInfo();
        }

        /// <summary>
        /// 获取当前支付宝 APP 的授权设置状态，包括相册、摄像头、定位、麦克风、通知等系统权限的授权情况。
        ///
        /// 详情链接：https://opendocs.alipay.com/mini-game/6336234d_my.getAppAuthorizeSetting?pathHash=f612ff5d
        ///
        /// <para>兼容性说明：</para>
        /// <list type="bullet">
        ///   <item>仅企业支付宝小程序可用。</item>
        ///   <item>基础库版本要求 ≥ 2.1.59。</item>
        ///   <item>支付宝客户端支持；支小宝客户端及安诊儿客户端不支持。</item>
        ///   <item>部分授权字段在部分平台或特定版本才可用（详见文档）。</item>
        ///   <item>如需适配多个客户端或低版本，建议做好兼容处理。</item>
        /// </list>
        ///
        /// <para>返回值说明：</para>
        /// <list type="bullet">
        ///   <item>返回值为 JSON 字符串，包含各权限的授权开关状态。</item>
        ///   <item>各字段枚举值说明（适用于所有权限）：</item>
        ///   <list type="number">
        ///     <item><c>authorized</c>：表示已获得授权，无需再次请求。</item>
        ///     <item><c>denied</c>：表示请求已被拒绝，无法再次请求（此时建议引导用户前往系统设置页手动开启权限）。</item>
        ///     <item><c>not determined</c>：尚未请求授权，会在下次调用相关权限时触发授权请求（仅 iOS 有效，且授权界面不展示开关）。</item>
        ///   </list>
        ///   <item>部分字段额外说明：</item>
        ///   <list type="bullet">
        ///     <item><c>albumAuthorized</c>：允许使用相册（仅 iOS 有效）。</item>
        ///     <item><c>bluetoothAuthorized</c>：允许使用蓝牙（安卓 10.5.50+）。</item>
        ///     <item><c>cameraAuthorized</c>：允许使用摄像头。</item>
        ///     <item><c>locationAuthorized</c>：允许定位。</item>
        ///     <item><c>locationReducedAccuracy</c>：定位精度，true 表示模糊定位（仅 iOS 有效）。</item>
        ///     <item><c>microphoneAuthorized</c>：允许使用麦克风。</item>
        ///     <item><c>notificationAuthorized</c> 等相关通知权限：允许支付宝通知、带提醒、标记、声音（部分字段仅 iOS 有效）。</item>
        ///     <item><c>overlayAuthorized</c>：允许悬浮窗（仅安卓有效）。</item>
        ///     <item><c>phoneCalendarAuthorized</c>：允许读写日历。</item>
        ///   </list>
        /// </list>
        ///
        /// <para>示例用法：</para>
        /// <example>
        /// string authSetting = GetAppAuthorizeSetting();
        /// // 返回如：{ "cameraAuthorized": "authorized", "microphoneAuthorized": "denied", ... }
        /// </example>
        ///
        /// <para>Tips：</para>
        /// <list type="bullet">
        /// <item>仅部分权限字段在特定平台/版本上有效，使用时建议判断结果字段是否存在。</item>
        /// <item>如返回 denied，需引导用户前往系统设置页手动授权。</item>
        /// <item>部分老版本、非支付宝官方客户端可能不支持该接口，应做好兼容、容错。</item>
        /// </list>
        /// </summary>
        /// <returns>支付宝客户端各权限授权状态（JSON字符串），详见返回值内容。</returns>
        public static string GetAppAuthorizeSetting()
        {
            return AlipaySDK.API.GetAppAuthorizeSetting();
        }

        #endregion

        #region 内购支付
        /// <summary>
        /// 发起小游戏内购支付请求。
        /// 
        /// 详情链接：https://opendocs.alipay.com/mini-game/0an78p?pathHash=3f1be7cb
        /// 
        /// 支付宝客户端：10.3.90版本以上使用。
        /// 
        /// <para><b>平台&版本限制：</b></para>
        /// - 支付宝客户端 10.3.90 及以上版本支持内购功能；低于该版本需屏蔽内购功能，可通过 my.getSystemInfo 获取客户端 version 字段判定。<br/>
        /// - 当前内购功能仅支持 Android 和 Harmony，iOS 用户需屏蔽该能力。
        /// 
        /// <para><b>重要说明：</b></para>
        /// - 前端可能无法获取实际支付结果，因此建议配合服务端交易状态查询接口，实际发货和到账逻辑应以服务端回调或查询为准。<br/>
        /// - 游戏币可能存在延迟到账，建议在界面为用户提供刷新余额的选项，余额查询失败时提示异常即可，不要直接显示 0 或阻止用户进入游戏。
        /// 
        /// <para><b>参数说明：</b></para>
        /// <list type="bullet">
        ///   <item><c>customId</c>：必填。游戏开发者自定义的唯一订单号，订单支付成功后通过服务端支付结果回调回传。必须具有唯一性，不传或重复传相同值会报错。</item>
        ///   <item><c>buyQuantity</c>：必填。本次购买的游戏币数量。币数量必须确保 buyQuantity * 游戏币单价 = 支付宝限定的价格档位（详见文档“购买数量说明”）。</item>
        ///   <item><c>extraInfo</c>：选填。自定义额外信息，支付结果回调时回传。转 JSON 后长度不得超过 64 位。</item>
        /// </list>
        /// 
        /// <para><b>回调结果说明：</b></para>
        /// 成功回调参数为 JSON 字符串，包含 resultCode 属性：
        /// /**
        /// *  resultCode 字段含义：
        /// *    - 9000：订单处理成功。建议以服务端回调/订单查询为准判断是否真正到账。
        /// *    - 8000：处理中，支付结果未知。请通过服务端订单查询。
        /// *    - 4000：订单处理失败，请重试。
        /// *    - 6001：用户取消支付；建议重新生成新订单进行支付。
        /// *    - 6002：网络出错；请检查网络后重试。
        /// *    - 6004：处理结果未知。请通过服务端查询。
        /// *  失败回调参数为 JSON，包含 error（错误码）、errorMessage（错误描述）。
        /// */
        /// <para>常见错误码：</para>
        /// <list type="bullet">
        ///   <item>2：接口入参无效。</item>
        ///   <item>3：未知异常，建议重试。</item>
        ///   <item>4：无权限调用（如个人小程序未开通支付）。</item>
        ///   <item>15002/15206/60001等：请求参数非法或异常，请检查参数。</item>
        ///   <item>15004/60002/60004：系统繁忙/服务异常，可稍后重试。</item>
        ///   <item>15215：未成年人充值功能受限。</item>
        ///   <item>15201：充值数量不符合规则（详见币价数量档位限制）。</item>
        ///   <item>15203：下单速度过快、重复下单。</item>
        /// </list>
        /// 
        /// <para><b>示例：</b></para>
        /// <example>
        /// RequestGamePayment(
        ///     "xxxxxxxxxxxxx",
        ///     10,
        ///     result => {
        ///         // result 形式如：
        ///         // { "resultCode":"9000" }
        ///         // 根据 resultCode 判断支付状态，并建议通过服务端核实发货结果
        ///     });
        /// </example>
        /// 
        /// <para>购买币数量需与单价搭配满足平台定价规则（详见文档“限定价格等级”部分）。</para>
        /// </summary>
        /// <param name="customId">
        /// 必填。唯一订单号，作为本次支付的唯一标识。不可重复使用。
        /// </param>
        /// <param name="buyQuantity">
        /// 必填。购买的游戏币数量。须满足平台规定的价格档位，通常 buyQuantity * 单价要等于人民币限定价格。
        /// </param>
        /// <param name="result">
        /// 支付结果回调。参数为JSON字符串，成功时包含 resultCode 字段，失败时包含 error 和 errorMessage 字段。
        /// </param>
        public static void RequestGamePayment(string customId, int buyQuantity, Action<string> result)
        {
            AlipaySDK.API.RequestGamePayment(customId, buyQuantity, result);
        }

        /// <summary>
        /// 发起小游戏内购支付请求。
        /// 
        /// 详情链接：https://opendocs.alipay.com/mini-game/0an78p?pathHash=3f1be7cb
        /// 
        /// 支付宝客户端：10.3.90版本以上使用。
        /// 
        /// <para><b>平台&版本限制：</b></para>
        /// - 支付宝客户端 10.3.90 及以上版本支持内购功能；低于该版本需屏蔽内购功能，可通过 my.getSystemInfo 获取客户端 version 字段判定。<br/>
        /// - 当前内购功能仅支持 Android 和 Harmony，iOS 用户需屏蔽该能力。
        /// 
        /// <para><b>重要说明：</b></para>
        /// - 前端可能无法获取实际支付结果，因此建议配合服务端交易状态查询接口，实际发货和到账逻辑应以服务端回调或查询为准。<br/>
        /// - 游戏币可能存在延迟到账，建议在界面为用户提供刷新余额的选项，余额查询失败时提示异常即可，不要直接显示 0 或阻止用户进入游戏。
        /// 
        /// <para><b>参数说明：</b></para>
        /// <list type="bullet">
        ///   <item><c>customId</c>：必填。游戏开发者自定义的唯一订单号，订单支付成功后通过服务端支付结果回调回传。必须具有唯一性，不传或重复传相同值会报错。</item>
        ///   <item><c>buyQuantity</c>：必填。本次购买的游戏币数量。币数量必须确保 buyQuantity * 游戏币单价 = 支付宝限定的价格档位（详见文档“购买数量说明”）。</item>
        ///   <item><c>extraInfo</c>：选填。自定义额外信息，支付结果回调时回传。转 JSON 后长度不得超过 64 位。</item>
        /// </list>
        /// 
        /// <para><b>回调结果说明：</b></para>
        /// 成功回调参数为 JSON 字符串，包含 resultCode 属性：
        /// /**
        /// *  resultCode 字段含义：
        /// *    - 9000：订单处理成功。建议以服务端回调/订单查询为准判断是否真正到账。
        /// *    - 8000：处理中，支付结果未知。请通过服务端订单查询。
        /// *    - 4000：订单处理失败，请重试。
        /// *    - 6001：用户取消支付；建议重新生成新订单进行支付。
        /// *    - 6002：网络出错；请检查网络后重试。
        /// *    - 6004：处理结果未知。请通过服务端查询。
        /// *  失败回调参数为 JSON，包含 error（错误码）、errorMessage（错误描述）。
        /// */
        /// <para>常见错误码：</para>
        /// <list type="bullet">
        ///   <item>2：接口入参无效。</item>
        ///   <item>3：未知异常，建议重试。</item>
        ///   <item>4：无权限调用（如个人小程序未开通支付）。</item>
        ///   <item>15002/15206/60001等：请求参数非法或异常，请检查参数。</item>
        ///   <item>15004/60002/60004：系统繁忙/服务异常，可稍后重试。</item>
        ///   <item>15215：未成年人充值功能受限。</item>
        ///   <item>15201：充值数量不符合规则（详见币价数量档位限制）。</item>
        ///   <item>15203：下单速度过快、重复下单。</item>
        /// </list>
        /// 
        /// <para><b>示例：</b></para>
        /// <example>
        /// RequestGamePayment(
        ///     "myOrderId202304011237",
        ///     10,
        ///     extraInfo,
        ///     result => {
        ///         // result 形式如：
        ///         // { "resultCode":"9000" }
        ///         // 根据 resultCode 判断支付状态，并建议通过服务端核实发货结果
        ///     });
        /// </example>
        /// 
        /// <para>购买币数量需与单价搭配满足平台定价规则（详见文档“限定价格等级”部分）。</para>
        /// </summary>
        /// <param name="customId">
        /// 必填。唯一订单号，作为本次支付的唯一标识。不可重复使用。
        /// </param>
        /// <param name="buyQuantity">
        /// 必填。购买的游戏币数量。须满足平台规定的价格档位，通常 buyQuantity * 单价要等于人民币限定价格。
        /// </param>
        /// <param name="extraInfo">
        /// 选填。自定义订单扩展信息（对象类型），转成Json后不超过64个字符，支付结果时回传。
        /// </param>
        /// <param name="result">
        /// 支付结果回调。参数为JSON字符串，成功时包含 resultCode 字段，失败时包含 error 和 errorMessage 字段。
        /// </param>
        public static void RequestGamePayment(string customId, int buyQuantity, JsonData extraInfo, Action<string> result)
        {
            AlipaySDK.API.RequestGamePayment(customId, buyQuantity, extraInfo, result);
        }

        /// <summary>
        /// 拉起小游戏支付面板，完成游戏平台虚拟币支付或充值。
        /// 文档：https://opendocs.alipay.com/mini-game/0hnwiw?pathHash=9d896b49
        ///
        /// 支付宝客户端：10.7.26版本以上（仅iOS，iPad与Android不支持）。
        /// 订单数量buyQuantity须为10的倍数，zoneId区分游戏大区与角色，goodsName不超过10字符，customId为业务唯一订单号。
        /// extraInfo为自定义扩展数据（推荐传入）。
        /// </summary>
        /// <param name="buyQuantity">订单购买数量，必须为10的倍数。</param>
        /// <param name="zoneId">游戏服务区id，自定义。不分区填"1"。多角色用“区ID_角色ID”。</param>
        /// <param name="goodsName">道具名称，≤10字符，仅落库。</param>
        /// <param name="customId">唯一订单号，支付成功后台回传用。</param>
        /// <param name="extraInfo">其他扩展信息（最大256字符），建议填写。</param>
        /// <param name="result">支付结果回调，参数为JSON字符串，成功返回success字段，失败时含error与errorMessage。</param>
        public static void RequestGamePayCoinPayment(int buyQuantity, string zoneId, string goodsName, string customId, JsonData extraInfo, Action<string> result)
        {
            AlipaySDK.API.RequestGamePayCoinPayment(buyQuantity, zoneId, goodsName, customId, extraInfo, result);
        }

        /// <summary>
        /// 拉起小游戏支付面板，完成游戏平台虚拟币支付或充值。
        /// 文档：https://opendocs.alipay.com/mini-game/0hnwiw?pathHash=9d896b49
        ///
        /// 支付宝客户端：10.7.26版本以上（仅iOS，iPad与Android不支持）。
        /// 订单数量buyQuantity须为10的倍数，zoneId区分游戏大区与角色，goodsName不超过10字符，customId为业务唯一订单号。
        /// extraInfo为自定义扩展数据（推荐传入）。
        /// </summary>
        /// <param name="buyQuantity">订单购买数量，必须为10的倍数。</param>
        /// <param name="zoneId">游戏服务区id，自定义。不分区填"1"。多角色用“区ID_角色ID”。</param>
        /// <param name="goodsName">道具名称，≤10字符，仅落库。</param>
        /// <param name="customId">唯一订单号，支付成功后台回传用。</param>
        /// <param name="result">支付结果回调，参数为JSON字符串，成功返回success字段，失败时含error与errorMessage。</param>
        public static void RequestGamePayCoinPayment(int buyQuantity, string zoneId, string goodsName, string customId, Action<string> result)
        {
            AlipaySDK.API.RequestGamePayCoinPayment(buyQuantity, zoneId, goodsName, customId, result);
        }
        #endregion

        #region 广告
        /// <summary>
        /// 获取广告管理对象
        /// （支付宝：10.5.20版本以上使用）
        /// </summary>
        public static AlipayAdManager GetAdManager()
        {
            return AlipaySDK.API.GetAdManager();
        }
        #endregion

        #region 启动参数

        /// <summary>
        /// 获取本次进入小程序（可能是冷启动或热启动）的参数。
        ///
        /// 详情链接：https://opendocs.alipay.com/mini-game/1fcb94b4_my.getEnterOptionsSync?pathHash=4fb74e85
        ///
        /// 支付宝客户端：10.5.30版本以上使用；<b>基础库：2.1.15及以上版本</b>。
        ///
        /// 返回值为JSON字符串，包含本次进入小游戏时的各类参数信息，包括：
        /// - query：启动小游戏时传递的 query 参数对象。（没有则不返回）
        /// - scene：启动场景值（字符串）。
        /// - path：当前小游戏页面地址，忽略则为首页。
        /// - referrerInfo：来源信息，包括 appId（来源小游戏），extraData（来源小游戏传递的数据）。
        /// - apiCategory：API类别，"default" 或 "embedded"（基础库2.7.22+，半屏模式时返回）。
        /// </summary>
        /// <returns>
        /// 返回JSON字符串，包含本次进入小游戏的参数信息（见官方文档）。
        /// </returns>
        public static string GetEnterOptionsSync()
        {
            return AlipaySDK.API.GetEnterOptionsSync();
        }


        /// <summary>
        /// 获取启动参数
        ///（支付宝：10.5.50版本以上使用）
        /// </summary>
        public static void GetLaunchOptions(string[] options, Action<string> result)
        {
            AlipaySDK.API.GetLaunchOptions(options, result);
        }

        /// <summary>
        /// 异步获取小游戏启动参数。
        ///
        /// 详情链接：https://opendocs.alipay.com/mini-game/093j2b?pathHash=9da032b8
        ///
        /// 支付宝客户端：10.5.30版本以上使用。
        ///
        /// 本接口用于同步获取小游戏启动时的参数，包括 query 信息、启动场景、来源信息等。
        /// 常用于获取渠道参数、分享参数、跳转信息等场景。
        ///
        /// 返回值为 JSON 字符串，主要字段如下：
        /// - query：Object，小游戏启动时携带的参数对象（如通过URL schema或分享、跳转携带的参数会在此）
        /// - scene：Number，启动小游戏的场景值
        /// - referrerInfo：Object，小游戏来源信息（若从其他应用跳转进来会包含 appId 与 extraData 字段，否则为{}）
        ///
        /// <para>示例用法：</para>
        /// <example>
        /// var optionsJson = GetLaunchOptionsSync();
        /// // optionsJson 例如：
        /// // { "query": {"key1":"value1"}, "scene":1001, "referrerInfo": { "appId":"xxxx", "extraData":{ "data1":"test" } } }
        /// </example>
        ///
        /// <para>使用说明：</para>
        /// <list type="bullet">
        /// <item>1. 分享、schema 启动、navigateToMiniProgram 跳转等场景相关参数，会携带在 query 或 referrerInfo.extraData 字段中。</item>
        /// <item>2. 如需获取启动参数中的 query 字段，请注意URLDecode处理。</item>
        /// <item>3. 若从其它小程序/APP跳转进入，referrerInfo 中包含来源 appId 和 extraData。</item>
        /// </list>
        ///
        /// <para>参考 - 获取 query 示例：</para>
        /// <code>
        /// // schema: alipays://platformapi/startapp?...&query=key1%3Dvalue1%26key2%3Dvalue2
        /// var options = GetLaunchOptionsSync();
        /// // options.query = { "key1":"value1", "key2":"value2" }
        /// </code>
        ///
        /// <para>参考 - 获取 navigateToMiniProgram 携带参数：</para>
        /// <code>
        /// // my.navigateToMiniProgram({appId: "...", extraData: { data1: "test" } });
        /// var options = GetLaunchOptionsSync();
        /// // options.referrerInfo = { "appId": "...", "extraData": { "data1":"test" } }
        /// </code>
        /// </summary>
        /// <param name="result">     
        /// 返回JSON字符串，包含以下字段：
        ///   - query：object，启动query参数
        ///   - scene：number，启动场景值
        ///   - referrerInfo：object，来源信息（含 appId 和 extraData）
        ///  </param>
        public static void GetLaunchOptions(Action<string> result)
        {
            AlipaySDK.API.GetLaunchOptions(result);
        }

        /// <summary>
        /// 同步获取小游戏启动参数。
        ///
        /// 详情链接：https://opendocs.alipay.com/mini-game/093j2b?pathHash=9da032b8
        ///
        /// 支付宝客户端：10.5.30版本以上使用。
        ///
        /// 本接口用于同步获取小游戏启动时的参数，包括 query 信息、启动场景、来源信息等。
        /// 常用于获取渠道参数、分享参数、跳转信息等场景。
        ///
        /// 返回值为 JSON 字符串，主要字段如下：
        /// - query：Object，小游戏启动时携带的参数对象（如通过URL schema或分享、跳转携带的参数会在此）
        /// - scene：Number，启动小游戏的场景值
        /// - referrerInfo：Object，小游戏来源信息（若从其他应用跳转进来会包含 appId 与 extraData 字段，否则为{}）
        ///
        /// <para>示例用法：</para>
        /// <example>
        /// var optionsJson = GetLaunchOptionsSync();
        /// // optionsJson 例如：
        /// // { "query": {"key1":"value1"}, "scene":1001, "referrerInfo": { "appId":"xxxx", "extraData":{ "data1":"test" } } }
        /// </example>
        ///
        /// <para>使用说明：</para>
        /// <list type="bullet">
        /// <item>1. 分享、schema 启动、navigateToMiniProgram 跳转等场景相关参数，会携带在 query 或 referrerInfo.extraData 字段中。</item>
        /// <item>2. 如需获取启动参数中的 query 字段，请注意URLDecode处理。</item>
        /// <item>3. 若从其它小程序/APP跳转进入，referrerInfo 中包含来源 appId 和 extraData。</item>
        /// </list>
        ///
        /// <para>参考 - 获取 query 示例：</para>
        /// <code>
        /// // schema: alipays://platformapi/startapp?...&query=key1%3Dvalue1%26key2%3Dvalue2
        /// var options = GetLaunchOptionsSync();
        /// // options.query = { "key1":"value1", "key2":"value2" }
        /// </code>
        ///
        /// <para>参考 - 获取 navigateToMiniProgram 携带参数：</para>
        /// <code>
        /// // my.navigateToMiniProgram({appId: "...", extraData: { data1: "test" } });
        /// var options = GetLaunchOptionsSync();
        /// // options.referrerInfo = { "appId": "...", "extraData": { "data1":"test" } }
        /// </code>
        /// </summary>
        /// <returns>
        /// 返回JSON字符串，包含以下字段：
        ///   - query：object，启动query参数
        ///   - scene：number，启动场景值
        ///   - referrerInfo：object，来源信息（含 appId 和 extraData）
        /// </returns>
        public static string GetLaunchOptionsSync()
        {
            return AlipaySDK.API.GetLaunchOptionsSync();
        }

        #endregion

        #region 跳转
        /// <summary>
        /// 跳转到其它支付宝小程序应用。
        ///
        /// 详情链接：https://opendocs.alipay.com/mini-game/08ux3x?pathHash=d4a32a2a
        ///
        /// 支付宝客户端：10.1.10版本以上使用；<b>基础库：2.1.0及以上版本</b>。
        ///
        /// 本接口用于在当前小程序内打开/跳转到目标小程序，支持传自定义参数、query 或 extraData，实现小程序间业务联动。<br/>
        /// <b>从10.6.50版本起，跳转前会弹出“即将访问xxxx小程序”提示。</b>
        ///
        /// <para><b>参数说明：</b></para>
        /// <list type="bullet">
        /// <item><c>appId</c>（必填）：目标小程序的16位 appId。</item>
        /// <item><c>param</c>：附加参数，JSON格式，包含以下可支持的属性：
        ///   <list type="bullet">
        ///    <item>path（string，选填）：目标小程序页面路径。省略则为首页。page路径后?后的内容会作为页面query参数。</item>
        ///    <item>query（object，选填，基础库2.7.16+）：设置目标小程序的 query 数据。</item>
        ///    <item>extraData（object，选填）：附加数据，通过目标小程序的 options.referrerInfo.extraData 获取。</item>
        ///    <item>events（object，选填，基础库2.9.41+）：小程序间通信事件监听。</item>
        ///   </list>
        /// </item>
        /// <item>result：跳转回调，参数为JSON串。success时 result.eventChannel 用于与目标小程序通信（基础库2.9.41+）。</item>
        /// </list>
        ///
        /// <para><b>注意事项：</b></para>
        /// <list type="bullet">
        /// <item>1. 若目标小程序未允许跳入，或 version 不满足，可能收到错误代码30等，应引导升级或联系目标小程序设置。</item>
        /// <item>2. 若目标小程序未上架，会收到 success 回调但页面提示“暂未找到此功能”。</item>
        /// <item>3. 官方推荐通过 my.getLaunchOptionsSync 获取 query/extraData/<see cref="options.query"/>。</item>
        /// </list>
        ///
        /// <para>常见错误码：</para>
        /// <list type="bullet">
        /// <item>2：参数无效，检查参数格式。</item>
        /// <item>30：目标应用不允许被跳转。需在开放平台配置跳转白名单。</item>
        /// <item>31：目标appId无效，需16位数字。</item>
        /// <item>5001：目标应用行业规范限制，不允许跳转。</item>
        /// </list>
        ///
        /// <para>示例用法：</para>
        /// <example>
        /// var param = new JsonData();
        /// param["path"] = "pages/index/index?from=game";
        /// param["query"] = new JsonData { ["foo"] = "bar" };
        /// param["extraData"] = new JsonData { ["myData"]="value" };
        ///
        /// NavigateToMiniProgram("2021002140649424", param, result => {
        ///     // result为JSON，包括resultCode、或error、errorMessage等
        /// });
        /// </example>
        /// </summary>
        /// <param name="appId">目标小程序 appId（16位数字）。</param>
        /// <param name="param">
        /// 跳转参数，支持 path/query/extraData/events 等，与支付宝官方 my.navigateToMiniProgram 入参一致，需按照实际文档结构赋值。
        /// </param>
        /// <param name="result">
        /// 跳转结果回调。参数为JSON字符串，通常含 resultCode、eventChannel（成功时）、error、errorMessage（失败时）等字段。
        /// </param>
        public static void NavigateToMiniProgram(string appId, JsonData param, Action<string> result)
        {
            AlipaySDK.API.NavigateToMiniProgram(appId, param, result);
        }

        /// <summary>
        /// 跳转到其它支付宝小程序应用，并通过指定参数Key传输自定义参数。
        ///
        /// 详情链接：https://opendocs.alipay.com/mini-game/08ux3x?pathHash=d4a32a2a
        ///
        /// 支付宝客户端：10.1.10版本以上使用；<b>基础库： 2.1.15 及以上版本</b>。
        ///
        /// 功能同 <see cref="NavigateToMiniProgram(string, JsonData, Action{string})"/>，但可通过指定的 paramKey 字段将 param 对象以指定Key传递给目标小程序，提高参数传递灵活性。
        ///
        /// <para>参数说明：</para>
        /// <list type="bullet">
        /// <item><c>appid</c>（必填）：目标小程序appId（16位数字）。</item>
        /// <item><c>paramKey</c>（必填）：指定最终传递参数的Key（如 "query"、"extraData"、其他业务自定义key，根据目标小程序接收接口而定）。</item>
        /// <item><c>param</c>（必填）：需要传递的参数对象。</item>
        /// <item><c>result</c>：跳转结果回调。参数结构同上。</item>
        /// </list>
        /// <para>
        /// 其他行为、错误码及注意事项与 <see cref="NavigateToMiniProgram(string, JsonData, Action{string})"/> 一致。
        /// </para>
        ///
        /// <para>示例用法：</para>
        /// <example>
        /// var data = new JsonData();
        /// data["orderNo"] = "123456";
        /// NavigateToMiniProgram("2021002140649424", "extraData", data, result => {
        ///    // result为JSON，包括resultCode、或error、errorMessage等
        /// });
        /// </example>
        /// </summary>
        /// <param name="appid">目标小程序 appId（16位数字）。</param>
        /// <param name="paramKey">参数key（如 query、extraData、可自定义），按业务约定由目标小程序约定接收。</param>
        /// <param name="param">需要传递的参数对象。</param>
        /// <param name="result">
        /// 跳转结果回调。参数为JSON字符串，通常含 resultCode、eventChannel（成功时）、error、errorMessage（失败时）等字段。
        /// </param>
        public static void NavigateToMiniProgram(string appid, string paramKey, JsonData param, Action<string> result)
        {
            AlipaySDK.API.NavigateToMiniProgram(appid, paramKey, param, result);
        }
        /// <summary>
        /// 退出当前小游戏应用。
        ///
        /// 详情链接：https://opendocs.alipay.com/mini-game/0g9tcy?pathHash=7be8c0e6
        ///
        /// 支付宝客户端：10.5.30版本以上使用；<b>基础库：2.1.15及以上版本</b>。
        ///
        /// <para><b>注意：</b></para>
        /// <list type="bullet">
        /// <item>必须由<strong>点击事件</strong>触发，否则无法成功执行退出。</item>
        /// <item>部分旧版本或非支付宝终端的小游戏容器，可能不兼容本接口，需适配。</item>
        /// </list>
        ///
        /// <para>示例用法：</para>
        /// <example>
        /// // 按钮点击事件中调用
        /// ExitProgram(result => {
        ///     // result 为退出执行状态消息
        /// });
        /// </example>
        /// </summary>
        /// <param name="result">
        /// 退出结果回调。参数一般为状态码或消息字符串。
        /// </param>
        public static void ExitProgram(Action<string> result)
        {
            AlipaySDK.API.ExitProgram(result);
        }

        /// <summary>
        /// 打开指定的 H5 页面或支付宝官方链接。
        ///
        /// 详情链接：https://opendocs.alipay.com/mini-game/3a3b812f_my.ap.openURL?pathHash=96a15b14
        ///
        /// 该接口可跳转到第三方 H5 页面或支付宝官方页面，目标地址（url）必须以 https:// 或 alipays:// 开头，且在开放平台 openURL 白名单内。
        /// 若目标为全局白名单范围（如 https://render.alipay.com/p/），可直接跳转。其它 URL 须在 openURL 白名单配置通过后方可跳转。
        ///
        /// <para>注意事项：</para>
        /// <list type="bullet">
        /// <item>url 必须以 https:// 或 alipays:// 开头，否则报参数无效。</item>
        /// <item>跳转目标须在 openURL 白名单内。</item>
        /// <item>基础库 2.8.19 以下版本跳转根目录需以 "/" 结尾。</item>
        /// </list>
        ///
        /// <para>回调 result 说明：</para>
        /// <list type="bullet">
        /// <item>返回为 JSON 字符串。调用失败时含 error、errorMessage 等字段。</item>
        /// <item>常见错误码：2（参数无效），60002（跳转目标不在白名单）。</item>
        /// </list>
        ///
        /// <para>示例：</para>
        /// <code>
        /// OpenURL("https://render.alipay.com/p/xxx", result => {
        ///     // 处理跳转结果。result 为 JSON 字符串
        /// });
        /// </code>
        /// </summary>
        /// <param name="url">要跳转的目标地址，须以 https:// 或 alipays:// 开头，且在 openURL 白名单内。</param>
        /// <param name="result">
        /// 跳转回调。参数为 JSON 字符串，成功或失败信息详见官方文档。
        /// </param>
        public static void OpenURL(string url, Action<string> result)
        {
            AlipaySDK.API.OpenURL(url, result);
        }

        #endregion

        #region 分享

        /// <summary>
        /// 设置小游戏页面的分享信息。
        /// 
        /// 详情链接：https://opendocs.alipay.com/mini-game/08vab2?pathHash=24ae6fef
        ///
        /// 支付宝客户端：10.5.30版本以上使用。
        ///
        /// 通过注册页面/游戏的分享内容，用户点击分享按钮（或调用 my.showSharePanel 展示分享面板）时，将弹出本接口设置的标题、参数等信息。
        ///
        /// <para><b>参数说明：</b></para>
        /// <list type="bullet">
        /// <item><c>title</c>：分享卡片标题，必填。例如“小游戏”。</item>
        /// <item><c>param</c>：扩展参数（JSON对象），可设置如分享描述（desc）、图片（imageUrl）、分享参数（query）等，具体字段如下：
        /// <list type="bullet">
        /// <item><c>desc</c>（string，选填）：分享描述。</item>
        /// <item><c>imageUrl</c>（string，选填）：分享卡片图片链接。</item>
        /// <item><c>query</c>（string，选填）：分享参数，目标用户打开后通过 my.getLaunchOptionsSync().query 获取。</item>
        /// </list></item>
        /// <item><c>result</c>：分享面板被唤起/回调的通知。参数为JSON串，典型字段包括 success、fail 或 complete 标志以及状态内容。</item>
        /// </list>
        ///
        /// <para>常见用法：</para>
        /// <example>
        /// OnShareAppMessage(
        ///   "小游戏",
        ///   new JsonData { ["desc"] = "这个小游戏真好玩", ["imageUrl"] = "https://xxx.png", ["query"] = "foo=bar" },
        ///   result => {
        ///     // result为json，如 {"success":true}
        ///   }
        /// );
        /// </example>
        ///
        /// <para>Tips：</para>
        /// <list type="bullet">
        /// <item>1. 分享时附加的 query 字段，将在其他用户通过分享进入小游戏时由 my.getLaunchOptionsSync().query 获取。</item>
        /// <item>2. 推荐将设置分享信息的代码放在页面 onShow 或相应生命周期回调中。</item>
        /// </list>
        /// </summary>
        /// <param name="title">分享标题，必填。</param>
        /// <param name="param">
        /// 分享参数，JSON对象。支持字段：desc（描述），imageUrl（图片地址），query（启动参数）等，详见支付宝官方文档。
        /// </param>
        /// <param name="result">
        /// 分享面板打开或分享行为后的回调，参数为json，包含 success、fail、complete 等状态内容。
        /// </param>
        public static void OnShareAppMessage(string title, JsonData param, Action<string> result)
        {
            AlipaySDK.API.OnShareAppMessage(title, param, result);
        }
        /// <summary>
        /// 唤起支付宝分享面板，触发小程序的分享功能。
        ///
        /// 详情链接：https://opendocs.alipay.com/mini-game/08unfz?pathHash=df7da857
        ///
        /// 支付宝客户端：10.5.30版本以上使用；<b>基础库：2.1.15及以上版本</b>。
        ///
        /// 本接口用于在小游戏中调用后主动唤起支付宝的分享面板，用户可进行分享操作。<br/>
        /// 分享卡片内容、参数建议通过注册 <see cref="OnShareAppMessage"/> 实现自定义配置。
        ///
        /// <para>常用场景：</para>
        /// <list type="bullet">
        /// <item>自定义分享按钮点击回调中调用，提升用户分享体验。</item>
        /// </list>
        ///
        /// <para>回调说明：</para>
        /// <list type="bullet">
        /// <item>result：返回分享面板弹出后的回调，参数为JSON字符串，包含如 success、fail、complete 字段及其状态。</item>
        /// </list>
        ///
        /// <para>示例用法：</para>
        /// <example>
        /// // 先注册分享内容
        /// OnShareAppMessage("小游戏", new JsonData { ["desc"] = "这个小游戏真好玩" }, null);
        ///
        /// // 按钮点击事件中唤起分享
        /// ShowSharePanel(result => {
        ///     // result 为 json，如 {"success":true}
        /// });
        /// </example>
        ///
        /// <para>注意：</para>
        /// <list type="bullet">
        /// <item>本接口仅支持企业支付宝小程序、个人支付宝小程序；不支持支小宝、安诊儿客户端及开发者工具。</item>
        /// <item>实际分享内容和参数应通过 <see cref="OnShareAppMessage"/> 注入。</item>
        /// </list>
        ///
        /// 相关文档：my.onShareAppMessage
        /// </summary>
        /// <param name="result">
        /// 分享面板打开及分享行为的回调，参数为json字符串，典型字段包括 success、fail、complete 等状态。
        /// </param>
        public static void ShowSharePanel(Action<string> result)
        {
            AlipaySDK.API.ShowSharePanel(result);
        }
        #endregion

        #region 文件
        /// <summary>
        /// 获取文件系统管理对象
        /// </summary>
        public static AlipayFSManager GetFileSystemManager()
        {
            return AlipaySDK.API.GetFileSystemManager();
        }
        #endregion

        #region 剪切板

        /// <summary>
        /// 设置剪贴板内容数据。
        ///
        /// 详情链接：https://opendocs.alipay.com/mini-game/08uiqa?pathHash=f968cde9
        ///
        /// 支付宝客户端：10.5.30版本以上使用。
        ///
        /// 本接口用于将指定文本内容写入系统剪贴板。（仅企业支付宝小程序支持，个人小程序不支持）
        ///
        /// <para>参数说明：</para>
        /// <list type="bullet">
        ///   <item>text：要写入剪贴板的文本内容，必填。</item>
        /// </list>
        ///
        /// <para>回调说明：</para>
        /// <list type="bullet">
        ///   <item>result：参数为JSON字符串，成功时通常为空对象，失败时包含 error、errorMessage 字段。</item>
        /// </list>
        ///
        /// <para>常见错误码：</para>
        /// <list type="bullet">
        ///   <item>2：接口参数无效。请检查 text 是否为有效非空字符串。</item>
        ///   <item>4：permission denied。仅企业小游戏可用，个人主体报此错。</item>
        /// </list>
        ///
        /// <para>示例用法：</para>
        /// <example>
        /// SetClipboard("需要复制的内容", result => {
        ///   // result 形如 {} 成功，或者 { "error":4, "errorMessage":"permission denied" } 失败
        /// });
        /// </example>
        ///
        /// <para>Tips：</para>
        /// <list type="bullet">
        /// <item>1. 仅企业支付宝小程序支持。</item>
        /// <item>2. 如需读取剪贴板内容，请使用 <see cref="GetClipboard"/>。</item>
        /// </list>
        /// </summary>
        /// <param name="text">要写入剪贴板的文本内容，必填。</param>
        /// <param name="result">
        /// 回调方法。参数为JSON字符串，成功时为空对象，失败时含 error, errorMessage 字段。
        /// </param>
        public static void SetClipboard(string text, Action<string> result)
        {
            AlipaySDK.API.SetClipboard(text, result);
        }

        /// <summary>
        /// 获取剪贴板内容数据。
        ///
        /// 详情链接：https://opendocs.alipay.com/mini-game/08ug7c?pathHash=ed9be046
        ///
        /// 支付宝客户端：10.5.30版本以上使用。
        ///
        /// 本接口用于获取当前剪贴板中的文本信息（仅支持企业支付宝小程序，个人小程序无法使用）。
        ///
        /// <para>回调说明：</para>
        /// <list type="bullet">
        ///   <item>result：回调参数为JSON字符串。当获取成功时，字段 text 表示剪贴板内容；失败时含 error、errorMessage 等。</item>
        /// </list>
        ///
        /// <para>常见错误码及处理：</para>
        /// <list type="bullet">
        /// <item>4：permission denied，当前小游戏为个人主体，无法使用。</item>
        /// <item>30：iOS 16 及以上，用户拒绝跨应用粘贴授权。</item>
        /// <item>60000：系统禁止获取剪贴板内容，用户关闭了剪贴板权限（可提醒用户前往 设置 > 通用 > 剪贴板设置 打开权限）。</item>
        /// <item>2001-2003：用户拒绝授权。如有需要，可提醒/引导用户前往小游戏右上角“胶囊按钮--设置--权限开关”手动开启。</item>
        /// <item>2004：支付宝版本低于10.3.90 且iOS 16+，直接报错。注意兼容性。</item>
        /// </list>
        ///
        /// <para>示例用法：</para>
        /// <example>
        /// GetClipboard(result => {
        ///   // result形如 { "text": "剪贴内容" }，如失败则含 error、errorMessage
        /// });
        /// </example>
        ///
        /// <para>Tips：</para>
        /// <list type="bullet">
        /// <item>1. 仅企业支付宝小程序支持。</item>
        /// <item>2. 实际参数和回调结构详见官方文档。</item>
        /// </list>
        /// </summary>
        /// <param name="result">
        /// 回调方法。参数为JSON字符串，成功时字段text为剪贴板内容，失败时含error、errorMessage等。
        /// </param>
        public static void GetClipboard(Action<string> result)
        {
            AlipaySDK.API.GetClipboard(result);
        }
        #endregion

        #region 震动

        /// <summary>
        /// 触发设备较短时间（约40ms）振动。
        /// 文档：https://opendocs.alipay.com/mini-game/08v96s?pathHash=38e92cdc
        ///
        /// 支付宝客户端：10.5.30版本以上使用。
        ///
        /// 仅在 iPhone 7/7 Plus 及以上机型及部分 Android 设备生效。
        /// </summary>
        /// <param name="result">振动结果回调，参数为 JSON 字符串。</param>
        public static void VibrateShort(Action<string> result)
        {
            AlipaySDK.API.VibrateShort(result);
        }

        /// <summary>
        /// 触发设备较长时间（约400ms）振动。
        /// 文档：https://opendocs.alipay.com/mini-game/08ul5b?pathHash=3250b452
        ///
        /// 支付宝客户端：10.5.30版本以上使用。
        /// 仅在 iPhone 7/7 Plus 及以上机型及部分 Android 设备生效。
        /// </summary>
        /// <param name="result">振动结果回调，参数为 JSON 字符串。</param>
        public static void VibrateLong(Action<string> result)
        {
            AlipaySDK.API.VibrateLong(result);
        }

        #endregion

        #region 屏幕亮度
        /// <summary>
        /// 设置是否保持屏幕常亮，仅对当前小游戏生效，离开小游戏后失效。
        /// 文档：https://opendocs.alipay.com/mini-game/08ung3?pathHash=6864c6e0
        ///
        /// 支付宝客户端：10.5.30版本以上使用。
        ///
        /// 回调参数为 JSON 字符串。
        /// </summary>
        /// <param name="keepScreenOn">是否保持屏幕常亮。</param>
        /// <param name="result">设置结果回调，参数为 JSON 字符串。</param>
        public static void SetKeepScreenOn(bool keepScreenOn, Action<string> result)
        {
            AlipaySDK.API.SetKeepScreenOn(keepScreenOn, result);
        }

        /// <summary>
        /// 设置屏幕亮度。
        /// 文档：https://opendocs.alipay.com/mini-game/08v96q?pathHash=87764190
        ///
        /// 支付宝客户端：10.5.30版本以上使用。
        ///
        /// brightness 取值范围为 0~1。
        /// 回调参数为 JSON 字符串，成功或失败时均会执行回调。
        /// </summary>
        /// <param name="brightness">需要设置的屏幕亮度，范围0~1。</param>
        /// <param name="result">设置结果回调，参数为 JSON 字符串。</param>
        public static void SetScreenBrightness(float brightness, Action<string> result)
        {
            AlipaySDK.API.SetScreenBrightness(brightness, result);
        }

        /// <summary>
        /// 获取屏幕亮度。
        /// 文档：https://opendocs.alipay.com/mini-game/08v5ix?pathHash=ecd3a0c1
        ///
        /// 支付宝客户端：10.5.30版本以上使用。
        ///
        /// 回调参数为 JSON 字符串，包含 brightness 字段（取值范围 0~1）。
        /// </summary>
        /// <param name="result">亮度结果回调，参数为包含 brightness 字段的 JSON 字符串。</param>
        public static void GetScreenBrightness(Action<string> result)
        {
            AlipaySDK.API.GetScreenBrightness(result);
        }
        #endregion

        #region 网络
        /// <summary>
        /// 小游戏发起 HTTPS 网络请求。
        /// 文档：https://opendocs.alipay.com/mini-game/08uy1c?pathHash=79945623
        ///
        /// 支付宝客户端：10.5.30版本以上使用。
        ///
        /// 支持自定义 HTTP 头、请求方式、数据、超时与返回格式。仅支持 HTTPS 协议，目标域名需在服务器域名白名单内。
        ///
        /// 回调参数为 JSON 字符串，包含服务器返回的数据和状态等。
        /// </summary>
        /// <param name="url">目标服务器 HTTPS 地址。</param>
        /// <param name="result">请求结果回调，参数为 JSON 字符串。</param>
        /// <param name="headers">可选，自定义请求头（默认 'content-type': 'application/json'）。</param>
        /// <param name="method">可选，请求方法（默认"GET"，支持GET、POST等）。</param>
        /// <param name="data">可选，请求参数，对象或字符串。</param>
        /// <param name="timeout">可选，请求超时时间，单位ms，默认30000。</param>
        /// <param name="dataType">可选，期望返回格式，默认"json"，支持"json"、"text"、"base64"、"arraybuffer"。</param>
        public static void Request(string url, Action<string> result, JsonData headers = null, string method = "GET", object data = null, int timeout = 30000, string dataType = "json")
        {
            AlipaySDK.API.Request(url, result, headers, method, data, timeout, dataType);
        }

        /// <summary>
        /// 小游戏发起 HTTPS 网络请求。
        /// 文档：https://opendocs.alipay.com/mini-game/08uy1c?pathHash=79945623
        ///
        /// 支付宝客户端：10.5.30版本以上使用。
        ///
        /// 支持自定义请求方式、数据格式、超时、Header等。仅支持 HTTPS 协议，且目标域名需配置在服务器域名白名单。
        ///
        /// 回调参数为 JSON 字符串，包括 data、status、headers 等数据。
        /// </summary>
        /// <param name="url">请求目标服务器的 HTTPS 地址。</param>
        /// <param name="result">请求结果回调，参数为 JSON 字符串。</param>
        /// <param name="param">可选，请求参数（如 method、data、headers、timeout等）。</param>
        public static void Request(string url, Action<string> result, JsonData param = null)
        {
            AlipaySDK.API.Request(url, result, param);
        }

        /// <summary>
        /// 下载文件资源到本地
        /// </summary>
        /// <param name="url">下载文件地址</param>
        /// <param name="header">HTTP 请求 Header</param>
        /// <param name="result"></param>
        public static void DownloadFile(string url, JsonData header, Action<string> result)
        {
            AlipaySDK.API.DownloadFile(url, header, result);
        }

        /// <summary>
        /// 下载指定文件资源到本地。
        /// 文档：https://opendocs.alipay.com/mini-game/08uo7r?pathHash=940da447
        ///
        /// 支付宝客户端：10.5.30版本以上使用。
        ///
        /// 支持 HTTPS 下载文件，支持指定保存路径及自定义 Header。
        /// 不指定 filePath 时，文件将存为本地临时文件。
        /// 回调参数为 JSON 字符串，包含保存路径、状态码等信息。
        /// </summary>
        /// <param name="url">下载文件的 HTTPS 地址。</param>
        /// <param name="result">下载结果回调，参数为 JSON 字符串。</param>
        /// <param name="header">可选，HTTP 请求 Header。</param>
        /// <param name="filePath">可选，指定文件保存路径，不指定则保存为本地临时文件。</param>
        /// <param name="timeout">可选，超时时间（毫秒），默认60000。</param>
        public static void DownloadFile(string url, Action<string> result, JsonData header = null, string filePath = null, int timeout = 60000)
        {
            AlipaySDK.API.DownloadFile(url, result, header, filePath, timeout);
        }

        /// <summary>
        /// 上传本地文件到开发者服务器。
        /// 文档：https://opendocs.alipay.com/mini-game/08v38n?pathHash=25c7e4a1
        ///
        /// 支付宝客户端：10.5.30版本以上使用。
        ///
        /// 通过 HTTPS POST 请求，将本地文件（如图片、音视频等）上传到指定服务器。服务端需支持 multipart/form-data 格式。
        ///
        /// fileType 参数建议统一传入 "image" 以兼容低版本支付宝客户端。
        ///
        /// 回调参数为JSON字符串，成功时包含服务器返回的数据、状态码等，失败时含 error 和 errorMessage。
        /// </summary>
        /// <param name="url">开发者服务器地址（需在服务器域名白名单内，HTTPS）。</param>
        /// <param name="filePath">本地文件路径。</param>
        /// <param name="fileName">表单字段名（key），服务器端据此获取文件内容。</param>
        /// <param name="fileType">文件类型。建议传"image"以最大兼容。</param>
        /// <param name="result">上传结果回调，参数为JSON字符串。</param>
        /// <param name="header">可选，HTTP请求Header。</param>
        /// <param name="formData">可选，附加form表单数据。</param>
        public static void UploadFile(string url, string filePath, string fileName, string fileType, Action<string> result, JsonData header = null, JsonData formData = null)
        {
            AlipaySDK.API.UploadFile(url, filePath, fileName, fileType, result, header, formData);
        }
        #endregion

        #region 电量
        /// <summary>
        /// 异步获取当前设备的电量和充电状态。
        ///
        /// 详情链接：https://opendocs.alipay.com/mini-game/08v5iw?pathHash=a071934e
        ///
        /// 支付宝客户端：10.1.38 及以上版本；<b>基础库：2.1.15 及以上版本</b>。
        ///
        /// 回调参数为 JSON 字符串，主要字段：
        /// - level：当前设备电量（Number）
        /// - isCharging：当前设备是否在充电中（Boolean）
        ///
        /// <para>返回示例：</para>
        /// <code>
        /// {
        ///   "level": 85,
        ///   "isCharging": false
        /// }
        /// </code>
        /// </summary>
        /// <param name="result">
        /// 回调，参数为 JSON 字符串，包含当前电池电量和充电状态信息。
        /// </param>
        public static void GetBatteryInfo(Action<string> result)
        {
            AlipaySDK.API.GetBatteryInfo(result);
        }
        /// <summary>
        /// 同步获取当前设备的电量和充电状态。
        ///
        /// 详情链接：https://opendocs.alipay.com/mini-game/08umce?pathHash=85a2fd13
        ///
        /// 支付宝客户端：10.1.38 及以上版本；<b>基础库：2.1.15 及以上版本</b>。
        ///
        /// 返回值为 JSON 字符串，主要字段：
        /// - level：当前设备电量（Number）
        /// - isCharging：当前设备是否在充电中（Boolean）
        ///
        /// <para>返回示例：</para>
        /// <code>
        /// {
        ///   "level": 85,
        ///   "isCharging": false
        /// }
        /// </code>
        /// </summary>
        /// <returns>
        /// JSON 字符串，包含当前电池电量和充电状态信息。
        /// </returns>
        public static string GetBatteryInfoSync()
        {
            return AlipaySDK.API.GetBatteryInfoSync();
        }
        #endregion

        #region 性能
        /// <summary>
        /// 修改小游戏渲染帧率（FPS）。
        ///
        /// 详情链接：https://opendocs.alipay.com/mini-game/093y9b?pathHash=61ad93c1
        ///
        /// 可设置 requestAnimationFrame 的回调频率，默认帧率为 60FPS。
        /// 支持设置范围为 1 ~ 60 帧每秒。
        /// </summary>
        /// <param name="fps">
        /// 渲染帧率，取值范围为 1~60。
        /// </param>
        public static void SetPreferredFramesPerSecond(int fps)
        {
            AlipaySDK.API.SetPreferredFramesPerSecond(fps);
        }

        /// <summary>
        /// 尝试加快触发 JS 对象垃圾回收（Garbage Collection）。
        /// 文档：https://opendocs.alipay.com/mini-game/091cfp?pathHash=ca20ca34
        ///
        /// 支付宝客户端：10.5.30版本以上使用。
        /// 实际GC时机由JS引擎控制，调用该方法不能保证立即回收内存。
        /// </summary>
        public static void TriggerGC()
        {
            AlipaySDK.API.TriggerGC();
        }
        #endregion

        #region 版本管理

        /// <summary>
        /// 获取全局唯一的小游戏版本更新管理器。
        ///
        /// 详情链接：https://opendocs.alipay.com/mini-game/377c53c5_my.getUpdateManager?pathHash=12f3e30e
        ///
        /// 支付宝客户端：10.5.30版本以上使用；iOS 需10.6.70及以上版本；<b>基础库：2.1.15及以上版本</b>。
        ///
        /// 通过该接口可获得 UpdateManager 实例，用于监听和管理小游戏的版本更新升级流程。
        /// 仅支持企业支付宝小程序。
        ///
        /// <para>用法示例：</para>
        /// <code>
        /// GetUpdateManager(manager => {
        ///     // manager: AlipayUpdateManager 实例
        ///     // 可调用 manager.OnCheckForUpdate / manager.OnUpdateReady 等方法监听更新流程
        /// });
        /// </code>
        ///
        /// </summary>
        /// <param name="result">
        /// 回调返回 <see cref="AlipayUpdateManager"/> 实例，可用于后续监听和处理小游戏的版本更新流程。
        /// </param>
        public static void GetUpdateManager(Action<AlipayUpdateManager> result)
        {
            AlipaySDK.API.GetUpdateManager(result);
        }

        /// <summary>
        /// 获取当前运行环境的支付宝小游戏基础库版本号。
        ///
        /// 详情链接：https://opendocs.alipay.com/mini-game/7c6a7c5b_my.SDKVersion?pathHash=85b373c5
        ///
        /// 基础库是支付宝小游戏的运行时框架，不同版本客户端和不同时间点对应的基础库版本可能不同。
        /// 可通过该接口获取当前运行环境（IDE 或真机）中的基础库版本号（如："1.24.11"）。
        ///
        /// 返回值示例： "1.24.11"
        /// </summary>
        /// <returns>
        /// 当前运行环境的基础库版本号（字符串），格式如 "1.24.11"。
        /// </returns>
        public static string SDKVersion()
        {
            return AlipaySDK.API.SDKVersion();
        }

        /// <summary>
        /// 获取当前小游戏的版本信息。
        ///
        /// 详情链接：https://opendocs.alipay.com/mini-game/d310584a_my.getAccountInfoSync?pathHash=7d14f744
        ///
        /// 支付宝客户端：10.2.60版本以上使用；<b>基础库：2.1.15及以上版本</b>。
        ///
        /// 可同步获取当前小游戏运行环境（正式版、体验版或开发版）及对应的版本号、appId等信息。
        ///
        /// 返回值为JSON字符串，主要字段：
        /// - miniProgram：小游戏版本信息
        ///   - appId：小游戏 appId
        ///   - envVersion：当前运行版本（"release"=正式版，"trial"=体验版，"develop"=开发版）
        ///   - version：小游戏版本号
        /// - plugin：插件版本信息（仅在插件环境下返回）
        ///   - appId：插件 appId
        ///   - version：插件版本号
        /// </summary>
        /// <returns>
        /// 返回JSON字符串，包含小游戏及插件的版本与身份信息，字段见官方文档详细说明。
        /// </returns>
        public static string GetAccountInfoSync()
        {
            return AlipaySDK.API.GetAccountInfoSync();
        }
        #endregion

        #region 舆情日志上报

        /// <summary>
        /// 打印支付宝Native日志
        /// （支付宝：10.5.56版本以上使用）
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="logMessage"></param>
        public static void LocalLog(LogType logType, string logMessage)
        {
            AlipaySDK.API.LocalLog(logType, logMessage);
        }

        /// <summary>
        /// 获取日志管理器对象。
        /// </summary>
        /// <returns></returns>
        public static APLogManager GetLogManager()
        {
            return AlipaySDK.API.GetLogManager();
        }

        /// <summary>
        /// 数据会在上报舆情平台之后，关联在舆情中的额外信息。
        /// </summary>
        /// <param name="callback"></param>
        public static void OnStartFeedback(Func<string> callback)
        {
            AlipaySDK.API.OnStartFeedback(callback);
        }
        #endregion

        #region 首页功能

        /// <summary>
        /// 禁用当前页面通用菜单中的“添加到桌面”功能。
        ///
        /// 详情链接：https://opendocs.alipay.com/mini-game/e6bed875_my.hideAddToDesktopMenu?pathHash=8b8c08cd
        ///
        /// 支付宝客户端：10.5.30版本以上使用；<b>基础库：2.1.57及以上版本</b>。
        ///
        /// 调用后，用户点击小游戏右上角菜单时，“添加到桌面”按钮会置灰，并显示“当前页面不可添加到桌面”。
        /// </summary>
        /// <param name="result">
        /// 回调，参数为JSON字符串，调用成功时为空对象，失败时含 error 和 errorMessage 字段。
        /// </param>
        public static void HideAddToDesktopMenu(Action<string> result)
        {
            AlipaySDK.API.HideAddToDesktopMenu(result);
        }

        /// <summary>
        /// 判断游戏中心是否能够添加到支付宝首页。
        ///
        /// 详情链接：https://opendocs.alipay.com/mini-game/0h3s3d?pathHash=6f222867
        ///
        /// 支付宝客户端：10.5.30版本以上使用；<b>基础库：2.1.57及以上版本</b>。
        ///
        /// 本接口仅支持企业支付宝小程序。调用该接口，可判断当前小游戏的“游戏中心”是否能添加到支付宝首页“我的小程序”栏。
        ///
        /// <para>回调说明：</para>
        /// <list type="bullet">
        ///   <item>result：回调参数为JSON字符串。字段 <c>canAddAppToMyApps</c>（bool）表示首页能否添加。</item>
        ///   <item>如果有错误则含 error、errorMessage。</item>
        /// </list>
        ///
        /// <para>常见错误码：</para>
        /// <list type="bullet">
        ///   <item>60001：疲劳度检测不通过</item>
        ///   <item>60002：此应用已经在首页了</item>
        ///   <item>60005：达到添加次数上限</item>
        /// </list>
        ///
        /// <para>示例用法：</para>
        /// <example>
        /// CanAddGameCenterToMyApps(result => {
        ///     // result如 { "canAddAppToMyApps": true }
        ///     // 若已经添加、频率受限或达次数上限，则含有 error 和 errorMessage 字段
        /// });
        /// </example>
        ///
        /// <para>Tips：</para>
        /// <list type="bullet">
        /// <item>仅企业支付宝小程序可用，个人小程序及部分特殊客户端不支持。</item>
        /// <item>推荐先用 canIUse('canAddGameCenterToMyApps') 检查API能力。</item>
        /// </list>
        /// </summary>
        /// <param name="result">
        /// 回调方法。参数为JSON字符串，字段 canAddAppToMyApps 表示是否可添加，失败时包含 error、errorMessage。
        ///</param>
        public static void CanAddGameCenterToMyApps(Action<string> result)
        {
            AlipaySDK.API.CanAddGameCenterToMyApps(result);
        }

        /// <summary>
        /// 添加游戏中心到支付宝首页“我的小程序”栏。
        ///
        /// 详情链接：https://opendocs.alipay.com/mini-game/0h3t12?pathHash=00d4a2f4
        ///
        /// 支付宝客户端：10.5.30版本以上使用；<b>基础库：2.1.57及以上版本</b>。
        ///
        /// 调用该接口，用于将当前小游戏的“游戏中心”添加到支付宝客户端首页的“我的小程序”列表，方便用户快速访问。<br/>
        /// 本接口仅支持企业支付宝小程序。调用前建议通过 <see cref="CanAddGameCenterToMyApps"/> 检查是否允许添加。
        ///
        /// <para>回调说明：</para>
        /// <list type="bullet">
        ///   <item>result：回调参数为JSON字符串。字段 <c>addAppToMyApps</c>（bool）表示是否已成功添加。</item>
        ///   <item>失败时result中包含 error（错误码）和 errorMessage（错误原因）。</item>
        /// </list>
        ///
        /// <para>常见错误码：</para>
        /// <list type="bullet">
        ///   <item>60001：疲劳度检测不通过</item>
        ///   <item>60002：此应用已经在首页了</item>
        ///   <item>60005：达到添加次数上限</item>
        /// </list>
        ///
        /// <para>示例用法：</para>
        /// <example>
        /// AddGameCenterToMyApps(result => {
        ///     // result如 { "addAppToMyApps": true }
        ///     // 添加失败时包含 error 和 errorMessage 字段
        /// });
        /// </example>
        ///
        /// <para>Tips：</para>
        /// <list type="bullet">
        /// <item>仅企业支付宝小程序可用。</item>
        /// <item>建议调用前先用 <see cref="CanAddGameCenterToMyApps"/> 判断是否可添加，避免无效请求。</item>
        /// <item>部分客户端及老版本不支持，需兼容检查。</item>
        /// </list>
        /// </summary>
        /// <param name="result">添加结果回调。参数为JSON字符串，字段 addAppToMyApps 表示是否添加成功，失败时包含 error、errorMessage。</param>
        public static void AddGameCenterToMyApps(Action<string> result)
        {
            AlipaySDK.API.AddGameCenterToMyApps(result);
        }

        public static void CanAddAppToMyApps(string type, Action<string> result)
        {
            AlipaySDK.API.CanAddAppToMyApps(type, result);
        }

        public static void AddAppToMyApps(string type, Action<string> result)
        {
            AlipaySDK.API.AddAppToMyApps(type, result);
        }

        #endregion

        #region 内存不足警告

        /// <summary>
        /// 监听支付宝小游戏内存不足告警事件。
        /// 文档：https://opendocs.alipay.com/mini-game/2b435749_my.onMemoryWarning?pathHash=51ce35c1
        ///
        /// 基础库：2.1.51及以上版本；
        /// 支付宝客户端：
        ///   - iOS 10.7.26及以上
        ///   - Android 10.1.35及以上
        ///
        /// 支持主体：企业支付宝小程序。
        ///
        /// 返回参数(JSON字符串)：
        /// {
        ///   "level": int // 系统内存告警等级（仅Android提供，iOS无此字段）
        /// }
        /// level取值（仅Android）：
        ///   5  -- TRIM_MEMORY_RUNNING_MODERATE（内存警告：中等）
        ///   10 -- TRIM_MEMORY_RUNNING_LOW     （内存警告：低）
        ///   15 -- TRIM_MEMORY_RUNNING_CRITICAL（内存警告：临界）
        ///
        /// 示例返回：{"level":10}
        /// </summary>
        /// <param name="result">
        /// 内存告警回调，参数为JSON字符串，字段level仅Android端有。
        /// </param>
        public static void OnMemoryWarning(Action<string> result)
        {
            AlipaySDK.API.OnMemoryWarning(result);
        }

        /// <summary>
        /// 移除内存不足告警事件的监听函数。
        /// 文档：https://opendocs.alipay.com/mini-game/2203821c_my.offMemoryWarning?pathHash=acaac634
        ///
        ///  支付宝客户端：10.5.30版本以上使用； 基础库：2.1.51及以上版本；
        ///
        /// 参数说明：
        /// - result：需移除的监听函数（即传入 OnMemoryWarning 的 Action 实例）；
        ///           如果不传，则移除全部已注册的内存不足监听。
        ///
        /// 示例用法：
        /// var listener = (string resJson) => { /* 处理内存告警 */ };
        /// OnMemoryWarning(listener);
        /// OffMemoryWarning(listener); // 移除指定listener
        /// OffMemoryWarning();         // 移除全部监听
        /// </summary>
        /// <param name="result">
        /// 要移除的内存告警监听函数（与 OnMemoryWarning 注册时的 Action 实例对应）。可为 null，若为 null 则移除所有监听。
        /// </param>
        public static void OffMemoryWarning(Action<string> result = null)
        {
            AlipaySDK.API.OffMemoryWarning(result);
        }
        #endregion

        #region 音频

        /// <summary>
        ///  创建Alipay内部音频对象
        /// </summary>
        public static AlipayAudioContext CreateInnerAudioContext()
        {
            return AlipaySDK.API.CreateInnerAudioContext();
        }

        #endregion

        #region 设备-授权设置

        /// <summary>
        /// 获取用户对当前小游戏各项功能的授权状态。
        /// 文档：https://opendocs.alipay.com/mini-game/08ul59?pathHash=abccd9ff
        ///
        /// 支付宝客户端：10.5.30版本以上使用，基础库：2.1.15及以上版本。
        ///
        /// 返回值字段说明(JSON字符串)：
        /// {
        ///   "authSetting": {
        ///     "location": bool,        // 地理位置授权
        ///     "album": bool,           // 相册读取授权
        ///     "writePhotosAlbum": bool,// 相册写入授权
        ///     "camera": bool,          // 摄像头授权
        ///     "alipaysports": bool,    // 运动数据授权
        ///     "phoneNumber": bool,     // 手机号授权
        ///     "aliaddress": bool,      // 收货地址授权
        ///     "userInfo": bool,        // 会员基础信息（昵称和头像）
        ///     "audioRecord": bool      // 麦克风授权
        ///   },
        ///   "subscriptionsSetting": {   // 订阅消息设置，withSubscriptions为true时返回
        ///     "mainSwitch": bool,                    // 订阅消息总开关
        ///     "itemSettings": { [templateId]: "accept"|"reject" } // 每一项订阅消息订阅状态
        ///   }
        /// }
        /// </summary>
        /// <param name="result">获取的授权状态，JSON字符串。</param>
        public static void GetSetting(Action<string> result)
        {
            AlipaySDK.API.GetSetting(result);
        }

        /// <summary>
        /// 获取用户对当前小游戏各项功能的授权状态。
        /// 文档：https://opendocs.alipay.com/mini-game/08ul59?pathHash=abccd9ff
        ///
        /// 支付宝客户端：10.5.30版本以上使用，基础库：2.1.15及以上版本。
        ///
        /// 返回值字段说明(JSON字符串)：
        /// {
        ///   "authSetting": {
        ///     "location": bool,        // 地理位置授权
        ///     "album": bool,           // 相册读取授权
        ///     "writePhotosAlbum": bool,// 相册写入授权
        ///     "camera": bool,          // 摄像头授权
        ///     "alipaysports": bool,    // 运动数据授权
        ///     "phoneNumber": bool,     // 手机号授权
        ///     "aliaddress": bool,      // 收货地址授权
        ///     "userInfo": bool,        // 会员基础信息（昵称和头像）
        ///     "audioRecord": bool      // 麦克风授权
        ///   },
        ///   "subscriptionsSetting": { // 订阅消息设置，withSubscriptions参数为true时返回
        ///     "mainSwitch": bool,      // 订阅消息总开关（true为开启）
        ///     "itemSettings": { [templateId]: "accept"|"reject" } // 每项消息模板的订阅状态
        ///   }
        /// }
        /// </summary>
        /// <param name="withSubscriptions">
        /// 是否获取用户订阅消息的订阅状态。true: 获取订阅消息相关授权；false: 不获取（默认）。
        /// </param>
        /// <param name="result">
        /// 获取的授权状态，JSON字符串（结构如上）。
        /// </param>
        public static void GetSetting(bool withSubscriptions, Action<string> result)
        {
            AlipaySDK.API.GetSetting(withSubscriptions, result);
        }

        /// <summary>
        /// 打开支付宝小游戏设置界面，允许用户管理已授权的功能权限。
        /// 文档：https://opendocs.alipay.com/mini-game/08v6w8?pathHash=a46faef9
        ///
        /// 仅显示小游戏已请求过的权限项。
        /// 设置界面关闭时，会返回用户最新的授权结果。
        ///
        /// 支付宝客户端：10.5.30及以上版本，基础库：2.1.15及以上版本。
        ///
        /// 返回值字段说明(JSON字符串)：
        /// {
        ///   "authSetting": {
        ///     "location": bool,         // 地理位置授权状态
        ///     "album": bool,            // 相册读取授权状态
        ///     "writePhotosAlbum": bool, // 相册写入授权状态
        ///     "camera": bool,           // 摄像头授权状态
        ///     "alipaysports": bool,     // 运动数据授权状态
        ///     "phoneNumber": bool,      // 手机号授权状态
        ///     "aliaddress": bool,       // 收货地址授权状态
        ///     "userInfo": bool,         // 会员基础信息（昵称、头像）
        ///     "audioRecord": bool       // 麦克风授权状态
        ///   }
        ///   // 若需订阅消息授权信息，请使用重载方法并指定withSubscriptions参数
        /// }
        /// </summary>
        /// <param name="result">
        /// 设置完成后的授权状态，JSON字符串（结构如上）。
        /// </param>
        public static void OpenSetting(Action<string> result)
        {
            AlipaySDK.API.OpenSetting(result);
        }

        /// <summary>
        /// 打开支付宝小游戏设置界面，允许用户管理已授权的功能权限，并可获取订阅消息的授权状态。
        /// 文档：https://opendocs.alipay.com/mini-game/08v6w8?pathHash=a46faef9
        ///
        /// 仅显示小游戏已请求过的权限项。
        /// 设置界面关闭时，会返回用户最新的授权结果。
        ///
        /// 支付宝客户端：10.5.30及以上版本，基础库：2.1.15及以上版本。
        ///
        /// <paramref name="withSubscriptions"/>为true时，可同时获取用户订阅消息授权状态（支付宝客户端10.5.10+）。
        ///
        /// 返回值字段说明(JSON字符串)：
        /// {
        ///   "authSetting": {
        ///     "location": bool,         // 地理位置授权状态
        ///     "album": bool,            // 相册读取授权状态
        ///     "writePhotosAlbum": bool, // 相册写入授权状态
        ///     "camera": bool,           // 摄像头授权状态
        ///     "alipaysports": bool,     // 运动数据授权状态
        ///     "phoneNumber": bool,      // 手机号授权状态
        ///     "aliaddress": bool,       // 收货地址授权状态
        ///     "userInfo": bool,         // 会员基础信息（昵称、头像）
        ///     "audioRecord": bool       // 麦克风授权状态
        ///   },
        ///   "subscriptionsSetting": {   // withSubscriptions参数为true时返回
        ///     "mainSwitch": bool,                        // 订阅消息总开关（true为启用）
        ///     "itemSettings": { 
        ///         [templateId]: "accept"|"reject"        // 各消息模板id的订阅状态："accept"为同意，"reject"为拒绝
        ///     }
        ///   }
        /// }
        /// </summary>
        /// <param name="withSubscriptions">
        /// 是否同时获取用户订阅消息的订阅状态（true:获取，false:不获取；默认false）。
        /// </param>
        /// <param name="result">
        /// 设置完成后的授权状态，JSON字符串（结构如上）。
        public static void OpenSetting(bool withSubscriptions, Action<string> result)
        {
            AlipaySDK.API.OpenSetting(withSubscriptions, result);
        }
        #endregion

        #region 交互提示
        /// <summary>
        /// 显示弱提示（Toast）弹窗，可自定义内容、类型、显示时长等。
        /// 文档：https://opendocs.alipay.com/mini-game/08uufv?pathHash=477000e1
        ///
        /// 支付宝客户端、支小宝客户端、安诊儿客户端均支持。
        ///
        /// 适用主体：企业支付宝小程序、个人支付宝小程序。
        ///
        /// 【能力说明】
        /// - 支持设置提示内容、图标类型（none/success/fail/exception/loading）、显示时长等。
        /// - 10.3.20+支持 image 参数（需绝对路径，优先级高于 type），支持 mask 蒙层防止穿透。
        /// - content 内容可用 "\n" 或 "\r\n" 实现换行（真机有效）。
        /// - 弹窗执行后立即回调 result，不会等待 toast 消失。
        ///
        /// 【参数说明】
        /// - content：显示内容（可为空，可换行）
        /// - type：图标类型（none/success/fail/exception/loading），默认 none
        /// - duration：显示时长，单位 ms，默认 3000
        ///
        /// 【回调返回参数】
        /// result 回调参数为 JSON 字符串，包含操作结果（如有）。
        ///
        /// 【常见错误】
        /// - INVALID_PARAM（错误码2）：入参不合法，需检查参数有效性。
        ///
        /// </summary>
        /// <param name="content">
        /// 弱提示内容（支持换行，可为空）。
        /// </param>
        /// <param name="type">
        /// 图标类型，none(无)/success/fail/exception/loading，默认 none。
        /// </param>
        /// <param name="duration">
        /// 显示时长（毫秒），默认 3000。
        /// </param>
        /// <param name="result">
        /// 弹窗显示结果回调，参数为 JSON 字符串（通常仅通知展示成功）。
        /// </param>
        public static void ShowToast(string content, string type, int duration, Action<string> result)
        {
            AlipaySDK.API.ShowToast(content, type, duration, result);
        }

        /// <summary>
        /// 隐藏弱提示（Toast）弹窗。
        /// 文档：https://opendocs.alipay.com/mini-game/08v27u?pathHash=a3ffa2a9
        ///
        /// 支付宝客户端、支小宝客户端、安诊儿客户端均支持。
        ///
        /// 适用主体：企业支付宝小程序、个人支付宝小程序。
        ///
        /// 【能力说明】
        /// - 主动隐藏当前显示的 toast 弹窗，无内容参数。
        /// - 回调函数返回操作结果，无特定字段。
        ///
        /// 【兼容与已知问题】
        /// - 支持所有客户端，但在支付宝 iOS 10.3.20~10.3.70（不含）无效。
        ///
        /// </summary>
        /// <param name="result">
        /// Toast隐藏操作结果回调，参数为 JSON 字符串（通常仅用于通知操作已完成）。
        /// </param>
        public static void HideToast(Action<string> result)
        {
            AlipaySDK.API.HideToast(result);
        }

        /// <summary>
        /// 弹出警告框（Alert），仅含一个按钮，用户确认即关闭弹窗。
        /// 文档：https://opendocs.alipay.com/mini-game/08uzg6?pathHash=264bb228
        ///
        /// 支付宝客户端：10.5.30版本以上使用；
        /// 支小宝和安诊儿客户端不支持；
        ///
        /// 适用主体：企业支付宝小程序、个人支付宝小程序。
        ///
        /// 【能力说明】
        /// - 支持设置标题、内容、按钮文案（自定义内容支持换行符 \n）。
        /// - 不支持图片/富文本样式，不支持副屏调用。
        ///
        /// 【参数说明】
        /// - title：警告框标题（可为空）
        /// - content：警告框内容（可为空，支持 \n 换行，不支持 html 语法）
        /// - buttonText：按钮文字（默认"确定"）
        ///
        /// 【回调返回参数】
        /// 参数为JSON字符串（如无内容，一般仅用于通知已点击按钮）。
        ///
        /// </summary>
        /// <param name="title">
        /// 警告框标题，可为空。
        /// </param>
        /// <param name="content">
        /// 警告框内容，可为空，支持换行符（\n 或 \r\n）。
        /// </param>
        /// <param name="buttonText">
        /// 按钮文字，默认“确定”。
        /// </param>
        /// <param name="result">
        /// 弹窗结果回调，参数为 JSON 字符串（一般用于通知点击按钮）。
        /// </param>
        public static void Alert(string title, string content, string buttonText, Action<string> result)
        {
            AlipaySDK.API.Alert(title, content, buttonText, result);
        }

        /// <summary>
        /// 显示提示确认框（带标题、内容、按钮文案自定义）。
        /// 文档：https://opendocs.alipay.com/mini-game/08v0wv?pathHash=0c674184
        ///
        /// 支付宝客户端/支小宝客户端/安诊儿客户端均支持。
        ///
        /// 适用主体：企业支付宝小程序、个人支付宝小程序。
        ///
        /// 【能力说明】
        /// - 弹出确认框，支持自定义标题、内容、确认/取消按钮文案及颜色。
        /// - 用户点击“确定”时回调参数 confirm=true，点击“取消”时回调参数 confirm=false。
        ///
        /// 【参数说明】
        /// - title：确认框标题（可为空）。
        /// - content：确认框内容（可为空）。
        /// - okButtonText：确认按钮文字（默认"确定"）。
        /// - cancelButtonText：取消按钮文字（默认"取消"）。
        ///
        /// 【回调返回参数】
        /// 参数为 JSON 字符串：
        ///   confirm：bool，点击“确定”为 true，点击“取消”为 false。
        ///
        /// </summary>
        /// <param name="title">
        /// 确认框标题，可为空。
        /// </param>
        /// <param name="content">
        /// 确认框显示内容，可为空。
        /// </param>
        /// <param name="okButtonText">
        /// “确定”按钮文案，默认“确定”。
        /// </param>
        /// <param name="cancelButtonText">
        /// “取消”按钮文案，默认“取消”。
        /// </param>
        /// <param name="result">
        /// 弹窗结果回调，参数为 JSON 字符串，包含字段 confirm（true/false）。
        /// </param>
        public static void Confirm(string title, string content, string okButtonText, string cancelButtonText, Action<string> result)
        {
            AlipaySDK.API.Confirm(title, content, okButtonText, cancelButtonText, result);
        }

        /// <summary>
        /// 弹出对话框，接收用户文本输入（带标题、内容、占位、按钮自定义等）。
        /// 文档：https://opendocs.alipay.com/mini-game/08ux3y?pathHash=9ddbcd3b
        ///
        /// 支付宝客户端：10.1.10版本以上使用；
        /// 基础库：1.7.2及以上版本；
        ///
        /// 适用主体：企业支付宝小程序、个人支付宝小程序。
        ///
        /// 【能力说明】
        /// - 可弹出带标题、内容、占位、自定义按钮文案和颜色的输入框弹窗，支持左中右对齐，输入框默认值可设置。
        /// - 若用户点击确认，则 ok=true；点击取消，则 ok=false。
        ///
        /// 【参数说明】
        /// - title：对话框标题（可为空）。
        /// - message：提示内容（可为空，默认"请输入内容"）。
        /// - placeholder：输入框提示文案（可为空）。
        /// - align：内容对齐方式，"left"/"center"/"right"，默认"center"（Android 10.3.30+）。
        /// - okButtonText：确认按钮文字（默认"确定"）。
        /// - cancelButtonText：取消按钮文字（默认"取消"）。
        ///
        /// 【回调返回参数】
        /// 参数为 JSON 字符串：
        ///   ok：bool，点击确认为 true，点击取消为 false；
        ///   inputValue：string，点击确认为输入框内容，点击取消为 ""。
        ///
        /// </summary>
        /// <param name="title">
        /// 对话框标题，可为空。
        /// </param>
        /// <param name="message">
        /// 提示内容，可为空，默认为"请输入内容"。
        /// </param>
        /// <param name="placeholder">
        /// 输入框内的提示文案，可为空。
        /// </param>
        /// <param name="align">
        /// 内容对齐方式："left"、"center"、"right"，默认"center"（Android 10.3.30+）。
        /// </param>
        /// <param name="okButtonText">
        /// 确认按钮文案，默认"确定"。
        /// </param>
        /// <param name="cancelButtonText">
        /// 取消按钮文案，默认"取消"。
        /// </param>
        /// <param name="result">
        /// 弹窗结果回调，参数为 JSON 字符串（包含 ok、inputValue 字段）。
        /// </param>
        public static void Prompt(string title, string message, string placeholder, string align, string okButtonText, string cancelButtonText, Action<string> result)
        {
            AlipaySDK.API.Prompt(title, message, placeholder, align, okButtonText, cancelButtonText, result);
        }
        #endregion

        #region  设备-网络状态

        /// <summary>
        /// 获取当前网络类型及状态。
        /// 文档：https://opendocs.alipay.com/mini-game/08ujw5?pathHash=613f1df1
        ///
        /// 支付宝客户端：9.6.8版本以上使用；
        /// 基础库：2.1.15及以上版本；
        ///
        /// 适用主体：企业支付宝小程序。
        ///
        /// 回调参数（JSON字符串）：
        /// {
        ///     "networkAvailable": bool,    // 网络是否可用
        ///     "networkType": string,       // 当前网络类型。可取值：UNKNOWN、NOTREACHABLE、WIFI、3G、2G、4G、WWAN、5G
        ///     "signalStrength": int,       // 信号强度（单位 dbm），仅安卓且网络为 WiFi 时返回，支付宝10.5.16+
        ///     "hasSystemProxy": bool       // 设备是否使用网络代理，支付宝10.5.16+
        /// }
        ///
        /// 注意事项：
        /// - 安卓用户需将支付宝客户端的“获取手机信息”授权设置为“始终允许”，否则 networkType 可能为 UNKNOWN。
        /// </summary>
        /// <param name="result">
        /// 调用结果回调，返回JSON字符串，格式如上，含网络类型、可用性等信息
        public static void GetNetworkType(Action<string> result)
        {
            AlipaySDK.API.GetNetworkType(result);
        }

        /// <summary>
        /// 监听网络状态变化事件。
        /// 文档：https://opendocs.alipay.com/mini-game/08ung1?pathHash=32a4f746
        ///
        /// 支付宝客户端：10.5.30版本以上使用；
        /// 基础库：2.1.15及以上版本；
        ///
        /// 适用主体：企业支付宝小程序、个人支付宝小程序。
        ///
        /// 回调参数（JSON字符串）：
        /// {
        ///     "isConnected": bool,    // 当前是否有网络连接
        ///     "networkType": string   // 网络类型。可取值：UNKNOWN、NOTREACHABLE、WWAN、WIFI、2G、3G、4G、5G
        /// }
        ///
        /// 注意事项：
        /// 建议使用真机预览来进行调试。切换网络时可能会导致调试中断。
        /// </summary>
        /// <param name="listener">
        /// 网络状态变化事件的回调，参数为JSON字符串，包含当前连接状态与网络类型等信息。
        /// </param>
        public static void OnNetworkStatusChange(Action<string> onAction)
        {
            AlipaySDK.API.OnNetworkStatusChange(onAction);
        }

        /// <summary>
        /// 移除网络状态变化事件的监听函数。
        /// 文档：https://opendocs.alipay.com/mini-game/08ug7b?pathHash=69d8e21b
        ///
        /// 支付宝客户端：10.5.30版本以上使用；
        /// 基础库：2.1.15及以上版本；
        ///
        /// 适用主体：企业支付宝小程序、个人支付宝小程序。
        ///
        /// 参数说明：
        /// - offAction：需要移除的监听函数（与 OnNetworkStatusChange 注册时的 Action 实例对应）；
        ///   如果为 null，则移除全部已注册的网络状态变化监听。
        ///
        /// 示例用法：
        /// var listener = (string resJson) => { /* 处理网络变化 */ };
        /// OnNetworkStatusChange(listener);
        /// OffNetworkStatusChange(listener); // 移除指定监听
        /// OffNetworkStatusChange();         // 移除全部监听
        /// </summary>
        /// <param name="offAction">
        /// 要移除的网络状态变化监听函数。与 OnNetworkStatusChange 注册时的 Action 实例对应。
        /// 可为 null，若为 null 则移除所有监听。
        /// </param>
        public static void OffNetworkStatusChange(Action<string> offAction = null)
        {
            AlipaySDK.API.OffNetworkStatusChange(offAction);
        }
        #endregion

        #region 游戏圈

        /// <summary>
        /// 创建游戏圈按钮---异步
        /// </summary>
        /// <param name="param"></param>
        /// <param name="result"></param>
        public static void CreateGameClubButton(CreateGameClubButtonParam param, Action<GameClubButton> result)
        {
            AlipaySDK.API.CreateGameClubButton(param, result);
        }

        /// <summary>
        /// 创建游戏圈按钮，用户点击后会跳转到小游戏的游戏圈页面。
        /// 文档：https://opendocs.alipay.com/mini-game/0dc2pc?pathHash=8883de78
        ///
        /// 支付宝客户端：10.5.90版本以上使用；
        /// 基础库：2.1.15及以上版本；
        ///
        /// 适用主体：企业支付宝小程序。
        ///
        /// 【主要参数说明】
        /// - type：必填，按钮类型。"text"（文本样式）或 "image"（图片样式）。
        /// - text：type="text"时必填，按钮文案内容。
        /// - image：type="image"时可填，自定义按钮背景图（仅支持网络图片，优先级高于icon）。
        /// - icon：type="image"且image未配置时有效，可选枚举："blue"、"white"、"dark"、"light"。
        /// - openlink：可选，圈子ID（指定跳转到某圈子）。
        /// - style：必填，按钮样式对象。包括 left、top、width、height（位置与尺寸，int），
        ///   以及 backgroundColor、borderColor、borderWidth、borderRadius、color、fontSize、textAlign等（颜色和文本样式，hex与string）。
        ///
        /// 【返回值】
        /// GameClubButton对象。可用于后续隐藏、销毁等操作。
        ///
        /// 【使用注意】
        /// - type与样式配置需匹配，详细样式与枚举参见官方文档说明。
        /// - 若image字段配置，则icon字段无效。
        ///
        /// </summary>
        /// <param name="param">
        /// 创建按钮的参数，详见 CreateGameClubButtonParam。type、style为必填；其他参数按业务需求选择性填写。
        /// </param>
        /// <returns>
        /// GameClubButton 游戏圈按钮对象。可用于控制按钮显示与销毁。
        /// </returns>
        public static GameClubButton CreateGameClubButton(CreateGameClubButtonParam param)
        {
            return AlipaySDK.API.CreateGameClubButton(param);
        }

        /// <summary>
        /// 获取当前用户在游戏圈的相关数据指标（如加入时间、禁言状态、当天互动数等）。
        /// 文档：https://opendocs.alipay.com/mini-game/0dbbmk?pathHash=63975db4
        ///
        /// 支付宝客户端：10.5.30版本以上使用；
        /// 基础库：2.1.15及以上版本；
        ///
        /// 适用主体：企业支付宝小程序。
        ///
        /// 【参数要求】
        /// - dataTypeList：必填，需获取的指标类型编号数组，每个编号含义如下：
        ///   1 —— 用户最新加入游戏圈的时间（秒级Unix时间戳）
        ///   3 —— 用户禁言状态（0正常，1禁言）
        ///   4 —— 当天点赞贴子数
        ///   5 —— 当天评论贴子数
        ///   6 —— 当天发表贴子数
        ///   7 —— 当天发表视频贴子数
        ///   8 —— 当天点赞官方贴数
        ///   9 —— 当天评论官方贴数
        ///   10 —— 当天访问圈子的时长
        ///   11 —— 当天浏览圈子的贴子数量
        ///
        /// 【回调返回参数】
        /// 回调参数为JSON字符串，格式示例：
        /// {
        ///   "data": [
        ///     { "dataType": 1, "value": 1666666666 },
        ///     { "dataType": 3, "value": 0 }
        ///     // ...可能有多个
        ///   ]
        /// }
        ///
        /// 【典型错误码】
        /// - 1：请求参数非法，检查入参是否正确；
        /// - 2：系统繁忙或未创建游戏圈，稍后重试；
        /// - 3：交互数据查询限流，需联系技术支持。
        ///
        /// </summary>
        /// <param name="dataTypeList">
        /// 需要获取的游戏圈数据类型编号数组，每项为1、3、4、5、6、7、8、9、10、11之一。
        /// </param>
        /// <param name="result">
        /// 查询结果回调，返回JSON字符串，含所有请求类型的结果。
        /// </param>
        public static void GetGameClubData(int[] dataTypeList, Action<string> result)
        {
            AlipaySDK.API.GetGameClubData(dataTypeList, result);
        }
        #endregion

        #region  键盘

        /// <summary>
        /// 显示支付宝小游戏输入键盘。
        /// 文档：https://opendocs.alipay.com/mini-game/08ug7d?pathHash=eae21012
        ///
        /// 支付宝客户端：10.5.30版本以上使用；
        /// 基础库：2.1.15及以上版本；
        ///
        /// 适用主体：企业支付宝小程序。
        ///
        /// 回调参数（JSON字符串）：
        /// {
        ///     "success": bool,    // 显示键盘操作是否成功
        ///     ...                 // 可能包含其他扩展信息
        /// }
        ///
        /// 相关API：my.hideKeyboard
        /// </summary>
        /// <param name="defaultValue">键盘输入框的默认值（可选）。</param>
        /// <param name="maxLength">输入内容允许的最大长度（可选，0为不限制）。</param>
        /// <param name="multiple">是否为多行输入（可选，true为多行输入，false为单行，默认为单行）。</param>
        /// <param name="confirmHold">点击确认按钮时键盘是否收起（可选，false时收起，true时不收起）。</param>
        /// <param name="confirmType">
        /// 确认按钮类型（可选），影响键盘右下角按钮文案。可选值："done"（完成）、"next"（下一个）、"search"（搜索）、"go"（前往）、"send"（发送）。默认为"done"。
        /// </param>
        /// <param name="result">接口调用结果回调，参数为JSON字符串，参见上述返回格式。</param>
        public static void ShowKeyboard(string defaultValue, int maxLength, bool multiple, bool confirmHold, string confirmType, Action<string> result)
        {
            AlipaySDK.API.ShowKeyboard(defaultValue, maxLength, multiple, confirmHold, confirmType, result);
        }

        /// <summary>
        /// 隐藏支付宝小游戏输入键盘。
        /// 文档：https://opendocs.alipay.com/mini-game/08ujw6?pathHash=f000b254
        ///
        /// 支付宝客户端：10.5.30版本以上使用；
        /// 基础库：2.1.15及以上版本；
        ///
        /// 适用主体：企业支付宝小程序、个人支付宝小程序。
        ///
        /// 回调参数（JSON字符串）：
        /// 调用成功/失败等结果信息。
        ///
        /// 注意事项：
        /// 在 Android 上，input 组件的 focus 事件中直接调用 my.hideKeyboard() 可能不生效。如有必要，请适当延时后再调用。
        /// </summary>
        /// <param name="result">接口调用结果回调，返回JSON字符串，包含隐藏键盘操作的相关信息。</param>
        public static void HideKeyboard(Action<string> result)
        {
            AlipaySDK.API.HideKeyboard(result);
        }

        /// <summary>
        /// 监听支付宝小游戏键盘输入事件。
        /// 文档：https://opendocs.alipay.com/mini-game/08v7q6?pathHash=d8f89e0c
        ///
        /// 支付宝客户端：10.5.30版本以上使用；
        /// 基础库：2.1.15及以上版本；
        ///
        /// </summary>
        /// <param name="result">
        /// 键盘输入事件的回调，参数为JSON字符串，包含当前输入内容（字段名value）。
        /// </param>
        public static void OnKeyboardInput(Action<string> result)
        {
            AlipaySDK.API.OnKeyboardInput(result);
        }

        /// <summary>
        /// 监听用户点击支付宝小游戏键盘 Confirm 按钮事件。
        /// 文档：https://opendocs.alipay.com/mini-game/08ung2?pathHash=4fa18417
        ///
        /// 支付宝客户端：10.5.30版本以上使用；
        /// 基础库：2.1.15及以上版本；
        ///
        /// 适用主体：企业支付宝小程序。
        /// </summary>
        /// <param name="result">
        /// 键盘 Confirm 按钮事件的回调，参数为JSON字符串，包含当前输入内容（字段名value）。
        /// </param>
        public static void OnKeyboardConfirm(Action<string> result)
        {
            AlipaySDK.API.OnKeyboardConfirm(result);
        }

        /// <summary>
        /// 监听支付宝小游戏键盘收起事件。
        /// 文档：https://opendocs.alipay.com/mini-game/08umcf?pathHash=817ef34e
        ///
        /// 支付宝客户端：10.5.30版本以上使用；
        /// 基础库：2.1.15及以上版本；
        ///
        /// 适用主体：企业支付宝小程序。
        /// </summary>
        /// <param name="result">
        /// 键盘收起事件的回调，参数为JSON字符串，包含当前输入内容（字段名value）。
        /// </param>
        public static void OnKeyboardComplete(Action<string> result)
        {
            AlipaySDK.API.OnKeyboardComplete(result);
        }

        /// <summary>
        /// 取消监听支付宝小游戏键盘输入事件。
        /// 文档：https://opendocs.alipay.com/mini-game/08v6w9?pathHash=5d68e7fc
        ///
        /// 支付宝客户端：10.5.30版本以上使用；
        /// 基础库：2.1.15及以上版本；
        ///
        /// 适用主体：企业支付宝小程序。
        ///
        /// 参数说明：
        /// - result：要移除的输入事件监听函数（与 OnKeyboardInput 注册时的 Action 实例对应）；
        /// </summary>
        /// <param name="result">
        /// 要移除的键盘输入监听函数 
        /// </param>
        public static void OffKeyboardInput(Action<string> result)
        {
            AlipaySDK.API.OffKeyboardInput(result);
        }

        /// <summary>
        /// 取消监听用户点击支付宝小游戏键盘 Confirm 按钮事件。
        /// 文档：https://opendocs.alipay.com/mini-game/08v6wa?pathHash=8aae58a9
        ///
        /// 支付宝客户端：10.5.30版本以上使用；
        /// 基础库：2.1.15及以上版本；
        ///
        /// 适用主体：企业支付宝小程序。
        ///
        /// 参数说明：
        /// - result：要移除的 Confirm 按钮事件监听函数（与 OnKeyboardConfirm 注册时的 Action 实例对应）；
        /// </summary>
        /// <param name="result">
        /// 要移除的 Confirm 按钮事件监听函数 
        /// </param>
        public static void OffKeyboardConfirm(Action<string> result)
        {
            AlipaySDK.API.OffKeyboardConfirm(result);
        }

        /// <summary>
        /// 取消监听支付宝小游戏键盘收起事件。
        /// 文档：https://opendocs.alipay.com/mini-game/08vab6?pathHash=85e72531
        ///
        /// 支付宝客户端：10.5.30版本以上使用；
        /// 基础库：2.1.15及以上版本；
        ///
        /// 适用主体：企业支付宝小程序。
        ///
        /// 参数说明：
        /// - result：要移除的键盘收起事件监听函数（必须为之前通过 OnKeyboardComplete 注册的 Action 实例）
        ///
        /// </summary>
        /// <param name="result">
        /// 要移除的键盘收起事件监听函数（与 OnKeyboardComplete 注册时的 Action 实例对应）
        /// </param>
        public static void OffKeyboardComplete(Action<string> result)
        {
            AlipaySDK.API.OffKeyboardComplete(result);
        }
        #endregion

        #region PC键鼠

        /// <summary>
        /// 监听键盘按键弹起（KeyUp）事件，仅适用于 PC 平台。
        /// <para>
        /// 功能描述：
        /// 通过传入监听函数，可捕获键盘按键弹起事件，获取按键信息。
        /// </para>
        /// <para>
        /// 回调参数（JSON字符串）：
        /// {
        ///     "key": string,        // 按键名（参考 Web KeyEvent key 属性）
        ///     "code": string,       // 按键代码（参考 Web KeyEvent code 属性）
        ///     "timeStamp": number   // 事件触发时间戳
        /// }
        /// </para>
        /// </summary>
        /// <param name="listener">
        /// 键盘按键弹起事件的监听回调。参数为 JSON 字符串，具体内容参见上述格式。
        /// </param>
        public static void OnKeyUp(Action<string> listener)
        {
            AlipaySDK.API.OnKeyUp(listener);
        }

        /// <summary>
        /// 监听键盘按键按下（KeyDown）事件，仅适用于 PC 平台。
        /// <para>
        /// 功能描述：
        /// 通过传入监听函数，可捕获键盘按键按下事件，获取按键信息。
        /// </para>
        /// <para>
        /// 回调参数（JSON字符串）：
        /// {
        ///     "key": string,        // 按键名（参考 Web KeyEvent key 属性）
        ///     "code": string,       // 按键代码（参考 Web KeyEvent code 属性）
        ///     "timeStamp": number   // 事件触发时间戳
        /// }
        /// </para>
        /// </summary>
        /// <param name="listener">
        /// 键盘按键按下事件的监听回调。参数为 JSON 字符串，具体内容参见上述格式。
        /// </param>
        public static void OnKeyDown(Action<string> listener)
        {
            AlipaySDK.API.OnKeyDown(listener);
        }

        /// <summary>
        /// 监听鼠标滚轮事件。
        /// 功能描述：
        /// 通过传入监听函数，可捕获鼠标滚轮滚动事件，获取滚动量及鼠标坐标等信息。
        /// </para>
        /// <para>
        /// 回调参数（JSON字符串）：
        /// {
        ///     "deltaX": number,    // 滚轮 X 轴方向滚动量
        ///     "deltaY": number,    // 滚轮 Y 轴方向滚动量
        ///     "deltaZ": number,    // 滚轮 Z 轴方向滚动量
        ///     "x": number,         // 鼠标所在位置横坐标
        ///     "y": number,         // 鼠标所在位置纵坐标
        ///     "timeStamp": number  // 事件触发时的时间戳
        /// }
        /// </para>
        /// </summary>
        /// <param name="listener">
        /// 鼠标滚轮事件的监听回调。回调参数为 JSON 字符串，具体内容参见上述格式。
        /// </param>
        public static void OnWheel(Action<string> listener)
        {
            AlipaySDK.API.OnWheel(listener);
        }

        /// <summary>
        /// 取消监听鼠标滚轮事件。
        /// </summary>
        /// <param name="listener">
        /// 需要移除的监听回调，需与添加时传入的参数保持一致。
        /// </param>
        public static void OffWheel(Action<string> listener)
        {
            AlipaySDK.API.OffWheel(listener);
        }

        #endregion

        #region 存储 Storage

        /// <summary>
        /// 保存数据到本地缓存（按 key 存储，原内容会被覆盖）。
        /// 文档：https://opendocs.alipay.com/mini-game/08uvoz?pathHash=4f723567
        ///
        /// 支付宝客户端：10.5.30版本以上使用；
        /// 基础库：2.1.15及以上版本；
        ///
        /// 适用主体：企业支付宝小程序、个人支付宝小程序。
        ///
        /// 【能力说明】
        /// - 可存储任意类型（string、object等，内部自动序列化），单条 key 最大 200KB，总体最多 10MB。
        /// - 数据按小程序 appId 和支付宝账号隔离，web-view/插件场景也会隔离，卸载客户端或删除小程序时数据会被清理。
        /// - 推荐异步使用以避免页面阻塞，若超限或参数非法会触发错误回调。
        ///
        /// 【参数要求】
        /// - key：必填，不为空字符串。
        /// - data：必填，支持任意类型，建议200KB以内。
        ///
        /// 【回调返回参数】
        /// 回调参数为 JSON 字符串，成功/失败均有详细信息。常见错误：
        ///   - 2：必填参数为空（key 或 data）
        ///   - 12：存储空间超限（超过10MB）
        ///   - 14：单条 data 超 200KB
        ///
        /// 【相关接口】
        /// my.getStorage / my.getStorageInfo / my.removeStorage / my.clearStorage 支持查、删、清；
        ///
        /// </summary>
        /// <param name="key">
        /// 存储的数据 key，必填，不接受空字符串。
        /// </param>
        /// <param name="data">
        /// 存储的数据内容，任意类型（如 string、object），单次最大200KB。
        /// </param>
        /// <param name="result">
        /// 存储操作结果回调，JSON字符串格式，成功/失败详见官方文档。
        /// </param>
        public static void SetStorage(string key, string data, Action<string> result)
        {
            AlipaySDK.API.SetStorage(key, data, result);
        }

        /// <summary>
        /// 删除本地缓存中的指定 key 对应的数据。
        /// 文档：https://opendocs.alipay.com/mini-game/08umc7?pathHash=c1f8a149
        ///
        /// 支付宝客户端：10.5.30版本以上使用；
        /// 基础库：2.1.15及以上版本；
        ///
        /// 适用主体：企业支付宝小程序、个人支付宝小程序。
        ///
        /// 【能力说明】
        /// - 仅删除指定 key 的缓存内容，其他 key 数据不受影响。
        /// - 参数 key 必填，且必须为有效字符串，不能为 null 或空串。
        ///
        /// 【回调返回参数】
        /// 回调参数为 JSON 字符串，包含成功/失败信息等。
        /// 常见错误：
        ///   - 2：接口参数无效（key 不合法或缺失），请填写正确的 key 值。
        ///
        /// </summary>
        /// <param name="key">
        /// 要删除的缓存 key，必填且为非空有效字符串。
        /// </param>
        /// <param name="result">
        /// 删除操作结果回调，返回 JSON 字符串，包含成功或失败信息。
        /// </param>
        public static void RemoveStorage(string key, Action<string> result)
        {
            AlipaySDK.API.RemoveStorage(key, result);
        }

        /// <summary>
        /// 获取当前小程序本地缓存的整体信息（key列表、已用空间、总空间）。
        /// 文档：https://opendocs.alipay.com/mini-game/08ux40?pathHash=f6d29a66
        ///
        /// 支付宝客户端：10.5.30版本以上使用；
        /// 基础库：2.1.15及以上版本；
        ///
        /// 适用主体：企业支付宝小程序、个人支付宝小程序。
        ///
        /// 【功能说明】
        /// - 返回当前小程序所有本地缓存 key 的列表，当前占用空间（KB）、最大可用空间（KB）。
        /// - 如需读取具体内容，请使用 my.getStorage。
        ///
        /// 【回调返回参数】
        /// StorageInfo，其中：
        ///   keys：string[]，当前所有缓存 key。
        ///   currentSize：int，当前占用空间（单位KB）。
        ///   limitSize：int，最大可用空间（单位KB）。
        ///
        /// </summary>
        /// <param name="result">
        /// 查询结果回调，返回 StorageInfo 对象，包括缓存 key 列表、当前/最大空间等信息。
        /// </param>
        public static void GetStorageInfo(Action<StorageInfo> result)
        {
            AlipaySDK.API.GetStorageInfo(result);
        }

        /// <summary>
        /// 从本地缓存中异步获取指定 key 对应的内容。
        /// 文档：https://opendocs.alipay.com/mini-game/08v38p?pathHash=5352291f
        ///
        /// 支付宝客户端：10.5.30版本以上使用；
        /// 基础库：2.1.15及以上版本；
        ///
        /// 适用主体：企业支付宝小程序、个人支付宝小程序。
        ///
        /// 【能力说明】
        /// - 支持异步获取本地缓存指定 key 的内容，若 key 不存在，返回结果的 data 字段为 null（不触发错误回调）。
        /// - 异步缓存存取不能保证操作顺序，若有依赖逻辑请务必串联回调或改用同步API（my.getStorageSync）。
        ///
        /// 【参数要求】
        /// - key：必填，欲获取内容的 key，不可为空字符串。
        ///
        /// 【回调返回参数】
        /// - 回调参数为 JSON 字符串，主要字段：
        ///   - data：key 对应的内容（如 key 不存在则为 null）。
        ///
        /// </summary>
        /// <param name="key">
        /// 需要获取的本地缓存 key，必填且为有效字符串。
        /// </param>
        /// <param name="result">
        /// 查询结果回调，返回 JSON 字符串（字段 data 为取到的内容，若 key 不存在则为 null）。
        /// </param>
        public static void GetStorage(string key, Action<string> result)
        {
            AlipaySDK.API.GetStorage(key, result);
        }

        /// <summary>
        /// 异步清除当前小程序的所有本地数据缓存。
        /// 文档：https://opendocs.alipay.com/mini-game/08uiq7?pathHash=7ac8c8b0
        ///
        /// 支付宝客户端：10.5.30版本以上使用；
        /// 基础库：2.1.15及以上版本；
        ///
        /// 适用主体：企业支付宝小程序、个人支付宝小程序。
        ///
        /// 【能力说明】
        /// - 清除所有通过 my.setStorage 等存储的本地数据，操作不可逆。
        /// - 仅影响当前小程序的数据，不影响其他小程序或支付宝其他数据。
        ///
        /// 【回调返回参数】
        /// - 回调参数为 JSON 字符串，包含操作结果等信息。
        ///
        /// </summary>
        /// <param name="result">
        /// 清除完成后回调，参数为 JSON 字符串，包含清除结果等信息。
        /// </param>
        public static void ClearStorage(Action<string> result)
        {
            AlipaySDK.API.ClearStorage(result);
        }

        /// <summary>
        /// 将数据存储在本地缓存中指定的 key 中的 同步 接口，会覆盖掉原来该 key 对应的数据。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool SetStorageSync(string key, string data)
        {
            return AlipaySDK.API.SetStorageSync(key, data);
        }

        public static bool SetIntStorageSync(string key, int data)
        {
            return AlipaySDK.API.SetIntStorageSync(key, data);
        }

        public static bool SetFloatStorageSync(string key, float data)
        {
            return AlipaySDK.API.SetFloatStorageSync(key, data);
        }

        /// <summary>
        /// 同步删除缓存数据的接口。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool RemoveStorageSync(string key)
        {
            return AlipaySDK.API.RemoveStorageSync(key);
        }

        /// <summary>
        /// 同步获取缓存数据的接口
        /// </summary>
        /// <returns></returns>
        public static StorageInfo GetStorageInfoSync()
        {
            return AlipaySDK.API.GetStorageInfoSync();
        }

        /// <summary>
        /// 同步获取缓存数据的接口
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetStorageSync(string key)
        {
            return AlipaySDK.API.GetStorageSync(key);
        }

        /// <summary>
        ///  清除本地数据缓存的同步接口
        /// </summary>
        /// <returns></returns>
        public static bool ClearStorageSync()
        {
            return AlipaySDK.API.ClearStorageSync();
        }
        #endregion

        #region 触摸事件
        /// <summary>
        /// 监听开始触摸事件
        /// </summary>
        public static void OnTouchStart(Action<OnTouchListenerResult> onTouchStartResult)
        {
            AlipaySDK.API.OnTouchStart(onTouchStartResult);
        }
        /// <summary>
        /// 取消监听开始触摸事件
        /// </summary>
        public static void OffTouchStart(Action<OnTouchListenerResult> onTouchStartResult = null)
        {
            AlipaySDK.API.OffTouchStart(onTouchStartResult);
        }
        /// <summary>
        /// 监听触点移动事件
        /// </summary>
        public static void OnTouchMove(Action<OnTouchListenerResult> onTouchMoveResult)
        {
            AlipaySDK.API.OnTouchMove(onTouchMoveResult);
        }
        /// <summary>
        /// 取消监听触点移动事件
        /// </summary>
        public static void OffTouchMove(Action<OnTouchListenerResult> onTouchMoveResult = null)
        {
            AlipaySDK.API.OffTouchMove(onTouchMoveResult);
        }
        /// <summary>
        /// 监听触点失效事件
        /// </summary>
        public static void OnTouchCancel(Action<OnTouchListenerResult> onTouchCancelResult)
        {
            AlipaySDK.API.OnTouchCancel(onTouchCancelResult);
        }
        /// <summary>
        /// 取消监听触点失效事件
        /// </summary>
        public static void OffTouchCancel(Action<OnTouchListenerResult> onTouchCancelResult = null)
        {
            AlipaySDK.API.OffTouchCancel(onTouchCancelResult);
        }
        /// <summary>
        /// 监听触摸结束事件
        /// </summary>
        public static void OnTouchEnd(Action<OnTouchListenerResult> onTouchEndResult)
        {
            AlipaySDK.API.OnTouchEnd(onTouchEndResult);
        }
        /// <summary>
        /// 取消监听触摸结束事件
        /// </summary>
        public static void OffTouchEnd(Action<OnTouchListenerResult> onTouchEndResult = null)
        {
            AlipaySDK.API.OffTouchEnd(onTouchEndResult);
        }
        #endregion

        #region Socket

        /// <summary>
        /// 创建Socket对象
        /// </summary>
        /// <returns></returns>
        public static AlipaySocket CreateSocket()
        {
            return AlipaySDK.API.CreateSocket();
        }

        /// 如果游戏使用 TCP 进行网络通信，在 Unity WebGL 中开发者需要使用 WebSocket 进行替代。也可以使用 SDK 中的 TCP 相关 API。
        /// 文档：https://opendocs.alipay.com/mini-game/0gs1y7?pathHash=dc3cb908
        ///
        /// </summary>
        /// <param name="bindToWifi"></param>
        /// <returns></returns>
        public static AlipayTCPSocket CreateAlipayTCPSocket(bool bindToWifi = false)
        {
            return AlipaySDK.API.CreateAlipayTCPSocket(bindToWifi);
        }

        /// <summary>
        /// 如果游戏使用 UDP 进行网络通信，在 Unity WebGL 中开发者需要使用 SDK 提供的 API 进行替代。
        /// 文档：https://opendocs.alipay.com/mini-game/0gs1y7?pathHash=dc3cb908
        /// 
        /// 注意UDP功能需要权限挂载
        /// </summary>
        /// <param name="broadcast"></param>
        /// <param name="multicast"></param>
        /// <param name="bindToWifi"></param>
        /// <returns></returns>
        public static AlipayUDPSocket CreateAlipayUDPSocket(bool broadcast = false, bool multicast = false, bool bindToWifi = false)
        {
            return AlipaySDK.API.CreateAlipayUDPSocket(broadcast, multicast, bindToWifi);
        }
        #endregion

        #region 游戏行为数据上报

        /// <summary>
        /// 报告游戏加载完成---直到用户可操作算加载完成
        /// 1. 如果有选服界面，手动点击选择服务器开始游戏的情况（选服页面上报）
        /// 2. 有选服界面，不需要手动点击，自动选服进入的情况（游戏主场景上报）
        /// 3. 无选服界面，默认进入游戏算加载完成 （游戏主场景上报）
        /// 
        /// 文档：https://opendocs.alipay.com/mini-game/0hq9qx?pathHash=2797daf8
        /// 支付宝客户端：10.5.70版本以上使用；
        /// 基础库：2.1.15及以上版本；
        /// </summary>
        public static void ReportLoadingCompleted()
        {
            AlipaySDK.API.ReportLoadingCompleted();
        }

        /// <summary>
        /// 初次启动游戏时，默认授权或主动授权完成时进行上报
        /// 
        /// 文档：https://opendocs.alipay.com/mini-game/0hq9qz?pathHash=c9d9e725
        /// 支付宝客户端：10.5.70版本以上使用；
        /// 基础库：2.1.15及以上版本；
        /// </summary>
        public static void ReportAuthorized()
        {
            AlipaySDK.API.ReportAuthorized();
        }

        /// <summary>
        /// 报告游戏角色创建完成
        /// 
        /// 文档：https://opendocs.alipay.com/mini-game/0hqsvx?pathHash=8d16108c
        /// 支付宝客户端：10.5.70版本以上使用；
        /// 基础库：2.1.15及以上版本；
        /// </summary>
        /// <param name="initial">
        ///  boolean initial  :  表示是否是初次创建。
        ///         true 游戏角色初次创建。
        ///         false 游戏角色已创建过。
        /// </param>
        public static void ReportGameCharacterCreated(bool initial)
        {
            AlipaySDK.API.ReportGameCharacterCreated(initial);
        }

        /// <summary>
        /// 报告成功进入游戏游玩界面
        /// 
        /// 文档：https://opendocs.alipay.com/mini-game/0hqsvx?pathHash=c3266722
        /// 支付宝客户端：10.5.70版本以上使用；
        /// 基础库：2.1.15及以上版本；
        /// </summary>
        public static void ReportGamePlay()
        {
            AlipaySDK.API.ReportGamePlay();
        }

        /// <summary>
        /// 小游戏通用埋点上报接口
        /// 
        /// 文档：https://opendocs.alipay.com/mini-game/0hqsvx?pathHash=c3266722
        /// 支付宝客户端：10.5.70版本以上使用；
        /// 基础库：2.1.15及以上版本；
        /// </summary>
        /// <param name="eventName">埋点上报的事件名称。</param>
        /// <param name="eventData">埋点上报的事件数据，自定义object。</param>、
        public static void ReportCustomEvent(string eventId, JsonData extData = null)
        {
            AlipaySDK.API.ReportCustomEvent(eventId, extData);
        }
        #endregion

        #region 特殊API
        public static string GetUnitySwitch()
        {
            return AlipaySDK.API.GetUnitySwitch();
        }

        public static string GetSwitch(JsonData args)
        {
            return AlipaySDK.API.GetSwitch(args);
        }

        public static void Rpc(string operationType, JsonData rpcRequestData, Action<string> result)
        {
            AlipaySDK.API.Rpc(operationType, rpcRequestData, result);
        }

        public static void GetDowngradeResult(string bizId, int scene, Action<string> result, JsonData ext = null)
        {
            AlipaySDK.API.GetDowngradeResult(bizId, scene, result, ext);
        }

        public static void RemoteLog(string bizType, string seedId, Action<string> result, JsonData param = null)
        {
            AlipaySDK.API.RemoteLog(bizType, seedId, result, param);
        }

        public static void HandleLoggingAction(string actionType, Action<string> result, JsonData param = null)
        {
            AlipaySDK.API.HandleLoggingAction(actionType, result, param);
        }

        public static void StartBizService(string name, JsonData param, Action<string> result)
        {
            AlipaySDK.API.StartBizService(name, param, result);
        }

        /// <summary>
        /// （已下线）隐藏当前页面的收藏按钮
        /// </summary>
        [Obsolete("已下线，游戏上线后会自动隐藏收藏按钮")]
        public static void HideFavoriteMenu(Action<string> result)
        {
            AlipaySDK.API.HideFavoriteMenu(result);
        }
        #endregion

        #region 图片
        /// <summary>
        /// 从本地相册选择图片或使用相机拍照。
        /// </summary>
        /// <param name="result"></param>
        /// <param name="count">最多可选择的图片张数</param>
        /// <param name="sizeType">图片类型</param>
        /// <param name="sourceType">选择图片的来源</param>
        /// <param name="highQuality">是否启用高画质模式。此参数仅供安卓使用。高画质模式使用系统相机，画质好，速度也相对较慢；非高画质模式直接从视频流获取快照，速度更快，但不适合对防抖和对焦要求较高的场景。</param>
        /// <param name="useFrontCamera">是否默认使用前置摄像头</param>
        public static void ChooseImage(Action<string> result, int count = 1, string[] sizeType = null, string[] sourceType = null, bool highQuality = false, bool useFrontCamera = false)
        {
            AlipaySDK.API.ChooseImage(result, count, sizeType, sourceType, highQuality, useFrontCamera);
        }

        /// <summary>
        /// 预览图片。
        /// </summary>
        /// <param name="urls">需要预览的图片链接列表支持网络 url、 本地临时文件、本地缓存文件、 本地用户文件
        /// 注意：iOS 端从 10.2.70 开始支持本地用户文件</param>
        /// <param name="result"></param>
        /// <param name="enableSavePhoto">是否支持长按下载图片</param>
        /// <param name="enableShowPhotoDownload">是否在右下角显示下载入口</param>
        /// <param name="current">当前显示图片的索引值</param>
        public static void PreviewImage(string[] urls, Action<string> result, bool enableSavePhoto = false, bool enableShowPhotoDownload = false, int current = 0)
        {
            AlipaySDK.API.PreviewImage(urls, result, enableSavePhoto, enableShowPhotoDownload, current);
        }

        /// <summary>
        /// 保存图片到系统相册
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="result"></param>
        /// <param name="hideToast"></param>
        public static void SaveImageToPhotosAlbum(string filePath, Action<string> result, bool hideToast = false)
        {
            AlipaySDK.API.SaveImageToPhotosAlbum(filePath, result, hideToast);
        }
        #endregion

        #region 订阅

        /// <summary>
        /// 唤起客户端小游戏消息订阅界面，允许用户选择订阅指定模板的消息（一次最高3个）。
        /// 文档：https://opendocs.alipay.com/mini-game/7780eac9_my.requestSubscribeMessage?pathHash=172f65ac
        ///
        /// 支付宝客户端：10.5.30版本以上使用；
        /// 基础库：2.7.10及以上版本；
        ///
        /// 适用主体：企业支付宝小程序、个人支付宝小程序。
        ///
        /// 【功能说明】
        /// - 此接口将拉起一个消息订阅面板，模板依据 entityIds 列表指定（ID需在商家平台提前配置领取）。
        /// - 只支持一次性模板id和长期性模板id中的一种混合，不可混用；一次最多3个模板id。
        /// - 若用户操作“总是保持以上选择/不再询问”，以后相同场景将不再弹出面板；用户可在“胶囊按钮-设置-消息管理”中重新更改。
        ///
        /// 【参数要求】
        /// - entityIds为必填数组，长度1~3，且模板类型不能混合，否则接口报错。
        /// - thirdTypeAppId仅服务商代调用场景下需传。
        ///
        /// 【回调返回参数】
        /// {
        ///   "behavior": string, // 订阅操作结果，subscribe：订阅成功，cancel/失败则触发fail回调
        ///   "show": bool,       // 是否弹出了订阅面板
        ///   "keep": bool,       // （一次性订阅）是否"总是保持以上选择，不再询问"
        ///   "refuse": bool,     // （长期订阅）是否"拒绝，不再询问"
        ///   "result": {
        ///      "subscribedEntityIds": string[],    // 成功订阅的模板ID列表
        ///      "unsubscribedEntityIds": string[]   // 未订阅的模板ID列表
        ///   }
        /// }
        ///
        /// 【订阅限制与常见错误】
        /// - 超过3个模板id、模板id不合法、类型混用、授权窗口已弹出/调用太频繁等会导致失败（详见文档错误码）。
        /// - Android 基础库低于2.7.15，web-view页面不能拉起订阅界面。
        ///
        /// 【服务端补充说明】
        /// - 服务端可通过alipay.open.app.messagetemplate.subscribe.query或my.getSetting接口获知订阅状态。
        ///
        /// </summary>
        /// <param name="entityIds">
        /// 需订阅的消息模板ID数组（1~3个，类型不可混用）。
        /// </param>
        /// <param name="result">
        /// 订阅完成后的回调，返回JSON字符串，详见回调返回参数说明。
        /// </param>
        /// <param name="thirdTypeAppId">
        /// 模板小游戏appid，仅服务商代调用场景下需填写，普通场景请传null或不填。
        /// </param>
        public static void RequestSubscribeMessage(string[] entityIds, Action<string> result, string thirdTypeAppId = null)
        {
            AlipaySDK.API.RequestSubscribeMessage(entityIds, result, thirdTypeAppId);
        }

        /// <summary>
        /// 取消当前用户已订阅的消息模板。
        /// 文档：https://opendocs.alipay.com/mini-game/d83e894e_my.unsubscribeMessage?pathHash=fe612761
        ///
        /// 支付宝客户端：10.5.30版本以上使用；
        /// 基础库：2.1.15及以上版本；
        ///
        /// 适用主体：企业支付宝小程序。
        ///
        /// 【功能说明】
        /// 传入要取消订阅的消息模板ID列表（entityIds），可一次性取消 1~3 个模板的订阅。
        /// 模板ID必须为同一类型（全部为一次性或全部为长期性），不能混用。
        ///
        /// 【参数要求】
        /// - entityIds：必填，包含 1~3 个有效消息模板ID，类型不可混用。
        ///
        /// 【回调返回参数】
        /// 回调参数为JSON字符串，内容依场景而定。
        ///
        /// 【典型错误码】
        /// - 203：参数无效，请检查entityIds是否有效、个数是否在1~3之间。
        /// - 106002：模板列表混合了一次性/长期模板，不允许混用。
        /// - 106008：模板ID不属于当前小程序，请校验后重试。
        /// - 10：系统异常，建议联系支付宝技术支持。
        ///
        /// </summary>
        /// <param name="entityIds">
        /// 需要取消订阅的消息模板ID数组（1~3个，且类型不可混用）。
        /// </param>
        /// <param name="result">
        /// 取消订阅的回调，返回JSON字符串，内容参考官方文档。
        /// </param>
        public static void UnsubscribeMessage(string[] entityIds, Action<string> result)
        {
            AlipaySDK.API.UnsubscribeMessage(entityIds, result);
        }
        #endregion

        #region 排行榜---邀请制
        public static void SetImRankData(string rankId, int dataType, string value, int updateType, int priority, Action<string> result, JsonData extraInfo = null)
        {
            AlipaySDK.API.SetImRankData(rankId, dataType, value, updateType, priority, result, extraInfo);
        }

        public static void GetImRankList(string rankId, Action<string> result, string headImg = null, string scorePrefix = null, string scoreSuffix = null, JsonData extraInfo = null)
        {
            AlipaySDK.API.GetImRankList(rankId, result, headImg, scorePrefix, scoreSuffix, extraInfo);
        }
        #endregion
    }
}

#endif