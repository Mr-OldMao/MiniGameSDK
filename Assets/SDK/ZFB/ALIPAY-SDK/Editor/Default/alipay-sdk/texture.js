import APGameGlobal from './global';
APGameGlobal.USED_TEXTURE_COMPRESSION = false;

const downloadedTextures = {};
const downloadingTextures = {};
const downloadFailedTextures = {};
let hasCheckSupportedExtensions = false;

const pngPath =
  window.unityNamespace.unityColorSpace && window.unityNamespace.unityColorSpace === "Linear" ? "lpng" : "png";
const assetPath = `${(window.unityNamespace.DATA_CDN || "").replace(/\/$/, "")}/Assets`;

const mod = {
  getRemoteImageFile(path, width, height) {
    let textureFormat = APGameGlobal.TextureCompressedFormat;
    if (!textureFormat || (textureFormat === "dds" && (width % 4 !== 0 || height % 4 !== 0))) {
      mod.downloadFile(path, width, height);
    } else {
      mod.requestFile(path, width, height, textureFormat);
    }
  },
  reTryRemoteImageFile(path, width, height) {
    const cid = path;
    if (!downloadFailedTextures[cid]) {
      downloadFailedTextures[cid] = {
        count: 0,
        path,
        width,
        height,
      };
    }
    if (downloadFailedTextures[cid].count > 4) {
      return;
    }
    setTimeout(() => {
      mod.getRemoteImageFile(path, width, height);
    }, Math.pow(2, downloadFailedTextures[cid].count) * 250);
    downloadFailedTextures[cid].count++;
  },
  requestFile(path, width, height, format) {
    const cid = path;
    const url = `${assetPath.replace(/\/$/, "")}/Textures/${format}/${width}/${path}.txt`;

    var xhr = new(window.unityNamespace.UnityLoader.UnityCache.XMLHttpRequest || XMLHttpRequest)();
    xhr.isFetchApi = true;
    xhr.open("GET", url, true);
    xhr.responseType = "arraybuffer";
    xhr.onload = function () {
      const res = xhr;
      if (res.status === 200) {
        downloadedTextures[cid] = {
          data: res.response,
        };
        downloadingTextures[cid].forEach((v) => v());
        delete downloadingTextures[cid];
        delete downloadFailedTextures[cid];
        delete downloadedTextures[cid];
      } else {
        var message = "[Texture] download fail, url:" + url + ", status:" + res.status;
        APGameGlobal.ALIPAYWASMSDK.SendMonitor(2, {
          type: 'unity_texture_download_fail',
          'message': message
        });
        mod.reTryRemoteImageFile(path, width, height);
      }
    };
    xhr.onerror = function (e) {
      const res = xhr;
      var message = "[Texture] download fail, url:" + url + ", status:" + res.status;
      APGameGlobal.ALIPAYWASMSDK.SendMonitor(2, {
        type: 'unity_texture_download_fail',
        'message': message
      });
      mod.reTryRemoteImageFile(path, width, height);
    };
    xhr.send(null);
  },
  downloadFile(path, width, height) {
    const cid = path;
    const url = `${assetPath.replace(/\/$/, "")}/Textures/${pngPath}/${width}/${path}.png`;

    const image = my.createImage();
    image.crossOrigin = "";
    image.src = url;
    image.onload = function () {
      downloadedTextures[cid] = {
        data: image,
      };
      downloadingTextures[cid].forEach((v) => v());
      delete downloadingTextures[cid];
      delete downloadFailedTextures[cid];
      delete downloadedTextures[cid];
    };
    image.onerror = function (err) {
      var message = "[Texture] download fail, url:" + url + ", status:" + JSON.stringify(err);
      APGameGlobal.ALIPAYWASMSDK.SendMonitor(2, {
        type: 'unity_texture_download_fail',
        'message': message
      });
      mod.reTryRemoteImageFile(path, width, height);
    };
  },
  APDownloadTexture(path, width, height, callback) {
    const width4m = width % 4;
    if (width4m !== 0) {
      width += 4 - width4m;
    }

    const cid = path;
    if (!cid) {
      // 可能由于瘦身资源发起的下载此处将直接忽略
      return;
    }
    if (downloadingTextures[cid]) {
      downloadingTextures[cid].push(callback);
    } else {
      downloadingTextures[cid] = [callback];
      mod.getRemoteImageFile(path, width, height);
    }
  },
  APSetSupportedExtensions(exts) {
    if (hasCheckSupportedExtensions) {
      return;
    }
    const list = exts;
    APGameGlobal.TextureCompressedFormat = "";
    if (list.indexOf("WEBGL_compressed_texture_s3tc") !== -1) {
      APGameGlobal.TextureCompressedFormat = "dds";
    }
    if (list.indexOf("WEBGL_compressed_texture_etc") !== -1) {
      APGameGlobal.TextureCompressedFormat = "etc2";
    }
    if (list.indexOf("WEBGL_compressed_texture_astc") !== -1) {
      APGameGlobal.TextureCompressedFormat = "astc";
    }
    console.log("Support Compressed Texture:", APGameGlobal.TextureCompressedFormat);
    try {
      UnityJsBridge.handleMsgFromUnity('localLog', JSON.stringify({
        'level': 'info',
        'message': '[Texture] Support Compressed Texture:' + APGameGlobal.TextureCompressedFormat,
        '__invokeType__': 'async',
        '__callBackId__': '-1'
      }));
    } catch (error) {}
    hasCheckSupportedExtensions = true;
  },
};

APGameGlobal.APDownloadTexture = mod.APDownloadTexture;
APGameGlobal.APSetSupportedExtensions = mod.APSetSupportedExtensions;
APGameGlobal.DownloadedTextures = downloadedTextures;
APGameGlobal.TextureCompressedFormat = ""; // 支持的压缩格式

const listener = function (res) {
  console.log("onNetworkStatusChange" + JSON.stringify(res));
  APGameGlobal.ALIPAYWASMSDK.SendMonitor(1, {
    type:'unity_texture_network',
    'message': '[Texture] onNetworkStatusChange:' + JSON.stringify(res)
  });

  if (res.isConnected) {
    Object.keys(downloadFailedTextures).forEach((key) => {
      const v = downloadFailedTextures[key];
      if (v.count > 4) {
        mod.getRemoteImageFile(v.path, v.width, v.height);
      }
    });
  }
};
my.onNetworkStatusChange(listener);