import APGameGlobal from './alipay-sdk/global';
import unityNamespace from './unity-namespace';
import './alipay-sdk/index';

const ELaunchEventType = {
  uninitialized: 'uninitialized',
  /** 插件启动 */
  launchPlugin: 'launchPlugin',
  /** 下载wasm代码 */
  loadWasm: 'loadWasm',
  /** wasm代码编译 */
  compileWasm: 'compileWasm',
  /** 下载首资源包 */
  loadAssets: 'loadAssets',
  /** 引擎初始化 */
  prepareGame: 'prepareGame',
};
const loaderOptions = {
  streamingAssetsUrl: '',
  data: {
    url: '',
    zipUrl: '',
    brUrl: '',
    size: 0,
    md5: '',
    subpackage: '',
    path: '',
    loadFromSubpackage: true,
  },
  loadingPageConfig: {
    /**
     * !!注意：修改设计宽高和缩放模式后，需要修改文字和进度条样式。默认设计尺寸为667*375
     */
    scaleMode: 'centerCrop', // 缩放模式（默认全屏）
    designWidth: 0, // 设计宽度（需要修改设计尺寸时更改）
    designHeight: 0, // 设计高度（需要修改设计尺寸时更改
    textConfig: {
      firstStartText: '首次加载请耐心等待', // 首次启动时提示文案
      downloadingText: ['正在加载资源'], // 加载阶段循环展示的文案
      compilingText: '编译中', // 编译阶段文案
      initText: '初始化中', // 初始化阶段文案
      completeText: '开始游戏', // 初始化完成
      textDuration: 1500, // 当downloadingText有多个文案时，每个文案展示时间
      // 文字样式
      style: {
        bottom: 64,
        height: 24,
        width: 240,
        lineHeight: 24,
        color: '#ffffff',
        fontSize: 12,
      },
    },
    // 进度条样式
    barConfig: {
      style: {
        width: 240,
        height: 24,
        padding: 2,
        bottom: 64,
        backgroundColor: '#07C160', // 已加载的进度条颜色
        defaultBackgroundColor: '#802b2b2b', // 未加载的进度条颜色
      },
    },
    materialConfig: {
      backgroundImage: '', // 背景图片，使用小游戏包内图片；（必传）
    },
  },
  customEnv: {
    unityNamespace,
  },
  onLaunchProgress(e) {
    // e: ILaunchEvent
    // interface ILaunchEvent {
    //   type: ELaunchEventType;
    //   data: {
    //     /** 阶段耗时 */
    //     costTimeMs: number;
    //     /** 总耗时 */
    //     runTimeMs: number;
    //     /** 当前是否处于前台，onShow/onHide */
    //     isVisible: boolean;
    //   };
    // }
    APGameGlobal.onLaunchProgress(e);
    if (e.type === ELaunchEventType.launchPlugin) {
      // 整个游戏开始启动时发送
    }
    if (e.type === ELaunchEventType.loadWasm) {
      // 开始下载 wasm 代码包，会多次回调
    }
    if (e.type === ELaunchEventType.compileWasm) {
      // 开始编译 wasm 主包代码
      APGameGlobal.Module = window.unityNamespace.Module;
    }
    if (e.type === ELaunchEventType.loadAssets) {
      // data 文件下载完成后，发送通知
      // 游戏插入自定义的预加载代码
    }
    if (e.type === ELaunchEventType.prepareGame) {
      // 游戏开始初始化，callMain
    }
  },
};
async function main() {
  await my.loadUnity(loaderOptions);
}
main().catch((err) => {
  console.error(err);
});
