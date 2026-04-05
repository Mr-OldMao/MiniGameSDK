mergeInto(LibraryManager.library, {
  unityCallJs: function (eventId, paramJson) {
    if (typeof UnityJsBridge === 'object') {
      UnityJsBridge.handleMsgFromUnity(
        UTF8ToString(eventId),
        UTF8ToString(paramJson)
      );
    } else if (
      window.webkit &&
      typeof window.webkit.messageHandlers !== 'undefined'
    ) {
      if (
        typeof window.webkit.messageHandlers.handleMsgFromUnity === 'object'
      ) {
        window.webkit.messageHandlers.handleMsgFromUnity.postMessage({
          apiName: UTF8ToString(eventId),
          apiParam: UTF8ToString(paramJson),
        });
      } else {
        console.error('unity send message failed');
      }
    } else {
      console.error('platform not support');
    }
  },

  unityCallJsSyncSafe: function (eventId, paramJson) {
    try {
      return _unityCallJsSync(eventId, paramJson);
    } catch (e) {
      console.error(e);
      return null;
    }
  },

  unityCallJsSync: function (eventId, paramJson) {
    var res = UnityJsBridge.handleMsgFromUnitySync(
      UTF8ToString(eventId),
      UTF8ToString(paramJson)
    );
    var bufferSize = lengthBytesUTF8(res || '') + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(res, buffer, bufferSize);
    return buffer;
  },

  AlipayFree: function (ptr) {
    return _free(ptr);
  },

  GetAlipayEnv: function () {
    var res = my.env;
    var jsonString = JSON.stringify(res);
    var bufferSize = lengthBytesUTF8(jsonString || '') + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(jsonString, buffer, bufferSize);
    return buffer;
  },

  AlipayIsIDE: function () {
    var res = my.isIDE;
    return res;
  },

  AlipaySDKVersion: function () {
    var res = my.SDKVersion;
    var bufferSize = lengthBytesUTF8(res || '') + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(res, buffer, bufferSize);
    return buffer;
  },

  AlipayWriteFileSync: function (fileNamePtr, dataPtr, encodingPtr) {
    var fileName = UTF8ToString(fileNamePtr);
    var encoding = UTF8ToString(encodingPtr);
    var stringData = UTF8ToString(dataPtr);

    return APGameGlobal.ALIPAYWASMSDK.WriteFileSync(
      fileName,
      stringData,
      encoding
    );
  },

  AlipayReadFileSync: function (
    fileNamePtr,
    encodingPtr,
    positionPtr,
    lengthPtr
  ) {
    var fileName = UTF8ToString(fileNamePtr);
    var encoding = UTF8ToString(encodingPtr);
    return APGameGlobal.ALIPAYWASMSDK.ReadFileSync(
      fileName,
      encoding,
      positionPtr,
      lengthPtr
    );
  },

  AlipayWriteBinFileSync: function (fileNamePtr, dataPtr, dataLength) {
    var fileName = UTF8ToString(fileNamePtr);
    return APGameGlobal.ALIPAYWASMSDK.WriteBinFileSync(
      fileName,
      dataPtr,
      dataLength
    );
  },

  AlipayWriteBinFile: function (
    fileNamePtr,
    dataPtr,
    dataLength,
    callbackIDPtr
  ) {
    var fileName = UTF8ToString(fileNamePtr);
    var callbackID = UTF8ToString(callbackIDPtr);
    APGameGlobal.ALIPAYWASMSDK.WriteBinFile(
      fileName,
      dataPtr,
      dataLength,
      callbackID
    );
  },

  AlipayReadBinFileSync: function (fileNamePtr, positionPtr, lengthPtr) {
    var fileName = UTF8ToString(fileNamePtr);
    return APGameGlobal.ALIPAYWASMSDK.ReadBinFileSync(
      fileName,
      positionPtr,
      lengthPtr
    );
  },

  AlipayReadBinFile: function (
    fileNamePtr,
    callbackIDPtr,
    positionPtr,
    lengthPtr
  ) {
    var fileName = UTF8ToString(fileNamePtr);
    var callbackID = UTF8ToString(callbackIDPtr);
    return APGameGlobal.ALIPAYWASMSDK.ReadBinFile(
      fileName,
      callbackID,
      positionPtr,
      lengthPtr
    );
  },

  AlipayReadBinFileAsync: function (
    fileNamePtr,
    callbackPtr,
    pos,
    len,
    callbackIdPtr
  ) {
    var fileName = UTF8ToString(fileNamePtr);
    var callbackId = UTF8ToString(callbackIdPtr);
    APGameGlobal.ALIPAYWASMSDK.ReadBinFileAsync(
      fileName,
      callbackPtr,
      pos,
      len,
      callbackId
    );
  },

  GetFSStatsSync: function (path, recursive) {
    var pathStr = UTF8ToString(path);
    return APGameGlobal.ALIPAYWASMSDK.GetFSStatsSync(pathStr, recursive);
  },

  GetUserAvatarData: function (urlPtr, successCallback, errorCallback) {
    const url = UTF8ToString(urlPtr);
    APGameGlobal.ALIPAYWASMSDK.GetUserAvatarData(
      url,
      successCallback,
      errorCallback
    );
  },

  AlipayCanIUse: function (schema) {
    var param = UTF8ToString(schema);
    return Boolean(my.canIUse(param));
  },

  OnStartFeedback: function (info) {
    var param = UTF8ToString(info);
    APGameGlobal.ALIPAYWASMSDK.OnStartFeedback(param);
  },

  AlipayLogManagerDebug: function (jsonString) {
    var param = UTF8ToString(jsonString);
    APGameGlobal.ALIPAYWASMSDK.AlipayLogManagerDebug(param);
  },

  AlipayLogManagerInfo: function (jsonString) {
    var param = UTF8ToString(jsonString);
    APGameGlobal.ALIPAYWASMSDK.AlipayLogManagerInfo(param);
  },

  AlipayLogManagerLog: function (jsonString) {
    var param = UTF8ToString(jsonString);
    APGameGlobal.ALIPAYWASMSDK.AlipayLogManagerLog(param);
  },

  AlipayLogManagerWarn: function (jsonString) {
    var param = UTF8ToString(jsonString);
    APGameGlobal.ALIPAYWASMSDK.AlipayLogManagerWarn(param);
  },

  TriggerGC: function () {
    my.triggerGC();
  },

  SendMonitor: function (type, jsonString) {
    var paramStr = UTF8ToString(jsonString);
    var paramObj = JSON.parse(paramStr);
    APGameGlobal.ALIPAYWASMSDK.SendMonitor(type, paramObj);
  },

  SetDevicePixelRatio: function (scale) {
    try {
      window.devicePixelRatio = scale;
      if (window.devicePixelRatio != scale || window.isModuleRatio == true) {
        Module.devicePixelRatio = scale;
        window.isModuleRatio = true;
      }
      return true;
    } catch (error) {
      return false;
    }
  },

  glGenTextures: function (n, textures) {
    for (var i = 0; i < n; i++) {
      var texture = GLctx.createTexture();
      if (!texture) {
        GL.recordError(1282);
        while (i < n) HEAP32[(textures + i++ * 4) >> 2] = 0;
        return;
      }
      var id = GL.getNewId(GL.textures);
      texture.name = id;
      GL.textures[id] = texture;
      window._lastTextureId = id;
      HEAP32[(textures + i * 4) >> 2] = id;
    }
  },

  glBindTexture: function (target, texture) {
    window._lastBoundTexture = texture;
    GLctx.bindTexture(target, texture ? GL.textures[texture] : null);
  },
});
