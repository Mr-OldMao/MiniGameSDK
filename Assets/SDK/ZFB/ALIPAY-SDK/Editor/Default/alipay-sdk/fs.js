import APGameGlobal from './global';
const AlipayFS = {
  WriteFileSync: function (fileName, stringData, encoding) {
    var fs = my.getFileSystemManager();
    var res = fs.writeFileSync({
      filePath: fileName,
      data: stringData,
      encoding: encoding,
    });
    var jsonString = JSON.stringify(res);
    var bufferSize = APGameGlobal.Module.lengthBytesUTF8(jsonString || '') + 1;
    var buffer = APGameGlobal.Module._malloc(bufferSize);
    APGameGlobal.Module.stringToUTF8(jsonString, buffer, bufferSize);
    return buffer;
  },

  ReadFileSync: function (fileName, encoding, positionPtr, lengthPtr) {
    var fs = my.getFileSystemManager();
    var params = {
      filePath: fileName,
      encoding: encoding,
    };
    if (positionPtr > 0) {
      params.position = positionPtr;
    }
    if (lengthPtr > 0) {
      params.length = lengthPtr;
    }
    var res = fs.readFileSync(params);

    if (res && res.data) {
      var jsonString = JSON.stringify(res);
      var bufferSize =
        APGameGlobal.Module.lengthBytesUTF8(jsonString || '') + 1;
      var buffer = APGameGlobal.Module._malloc(bufferSize);
      APGameGlobal.Module.stringToUTF8(jsonString, buffer, bufferSize);
      return buffer;
    }
  },

  WriteBinFileSync: function (fileName, dataPtr, dataLength) {
    var fs = my.getFileSystemManager();

    var data = APGameGlobal.Module.HEAPU8.subarray(
      dataPtr,
      dataPtr + dataLength
    );
    var arrayBuffer = data.buffer.slice(
      data.byteOffset,
      data.byteOffset + dataLength
    );
    var res = fs.writeFileSync({
      filePath: fileName,
      data: arrayBuffer,
    });
    var jsonString = JSON.stringify(res);
    var bufferSize = APGameGlobal.Module.lengthBytesUTF8(jsonString || '') + 1;
    var buffer = APGameGlobal.Module._malloc(bufferSize);
    APGameGlobal.Module.stringToUTF8(jsonString, buffer, bufferSize);
    return buffer;
  },

  ReadBinFileSync: function (fileName, positionPtr, lengthPtr) {
    var fs = my.getFileSystemManager();
    var params = {
      filePath: fileName,
    };
    if (positionPtr > 0) {
      params.position = positionPtr;
    }
    if (lengthPtr > 0) {
      params.length = lengthPtr;
    }
    var res = fs.readFileSync(params);
    if (res && res.data) {
      var byteArray = new Uint8Array(res.data);
      var length = byteArray.length;
      var bufferPtr = APGameGlobal.Module._malloc(length + 4);
      APGameGlobal.Module.HEAP32[bufferPtr >> 2] = length;
      APGameGlobal.Module.HEAPU8.set(byteArray, bufferPtr + 4);
      return bufferPtr;
    } else {
      var jsonString = JSON.stringify(res);
      var errorBufferSize = APGameGlobal.Module.lengthBytesUTF8(jsonString) + 1;
      var errorBufferPtr = APGameGlobal.Module._malloc(errorBufferSize);
      APGameGlobal.Module.stringToUTF8(
        jsonString,
        errorBufferPtr,
        errorBufferSize
      );
      return errorBufferPtr;
    }
  },

  WriteBinFile: function (fileName, dataPtr, dataLength, callbackID) {
    var fs = my.getFileSystemManager();
    var data = APGameGlobal.Module.HEAPU8.subarray(
      dataPtr,
      dataPtr + dataLength
    );
    var arrayBuffer = data.buffer.slice(
      data.byteOffset,
      data.byteOffset + dataLength
    );
    fs.writeFile({
      filePath: fileName,
      data: arrayBuffer,
      success: function (res) {
        var resultString = JSON.stringify({
          messageId: callbackID,
          result: res,
        });
        APGameGlobal.Module.SendMessage(
          'AlipayBridge',
          'ReceiveMessageFromJS',
          resultString
        );
      },
      fail: function (err) {
        var resultString = JSON.stringify({
          messageId: callbackID,
          result: err,
        });
        APGameGlobal.Module.SendMessage(
          'AlipayBridge',
          'ReceiveMessageFromJS',
          resultString
        );
      },
    });
  },

  ReadBinFile: function (fileName, callbackID, positionPtr, lengthPtr) {
    var fs = my.getFileSystemManager();
    var resultString;
    var params = {
      filePath: fileName,
    };
    if (positionPtr > 0) {
      params.position = positionPtr;
    }
    if (lengthPtr > 0) {
      params.length = lengthPtr;
    }
    fs.readFile({
      ...params,
      success: function (res) {
        if (res && res.data) {
          var byteArray = new Uint8Array(res.data);
          resultString = JSON.stringify({
            messageId: callbackID,
            result: byteArray,
          });
        } else {
          resultString = JSON.stringify({
            messageId: callbackID,
            result: null,
          });
        }
        APGameGlobal.Module.SendMessage(
          'AlipayBridge',
          'ReceiveMessageFromJS',
          resultString
        );
      },
      fail: function (err) {
        resultString = JSON.stringify({
          messageId: callbackID,
          result: err,
        });
        APGameGlobal.Module.SendMessage(
          'AlipayBridge',
          'ReceiveMessageFromJS',
          resultString
        );
      },
    });
  },

  ReadBinFileAsync: function (
    fileName,
    callbackPtr,
    position,
    length,
    callbackId
  ) {
    var fs = my.getFileSystemManager();
    var params = { filePath: fileName };
    if (position > 0) params.position = position;
    if (length > 0) params.length = length;

    fs.readFile({
      ...params,
      success: function (res) {
        let uint8Message = new Uint8Array(res.data);
        let dataLen = uint8Message.byteLength;
        let bufferPtr = APGameGlobal.Module._malloc(dataLen);
        APGameGlobal.Module.HEAPU8.set(uint8Message, bufferPtr);

        let fileNamePtr = APGameGlobal.Module._malloc(
          APGameGlobal.Module.lengthBytesUTF8(fileName) + 1
        );
        APGameGlobal.Module.stringToUTF8(
          fileName,
          fileNamePtr,
          APGameGlobal.Module.lengthBytesUTF8(fileName) + 1
        );

        let callbackIdPtr = APGameGlobal.Module._malloc(
          APGameGlobal.Module.lengthBytesUTF8(callbackId) + 1
        );
        APGameGlobal.Module.stringToUTF8(
          callbackId,
          callbackIdPtr,
          APGameGlobal.Module.lengthBytesUTF8(callbackId) + 1
        );

        APGameGlobal.Module.dynCall_viiiii(
          callbackPtr,
          fileNamePtr,
          bufferPtr,
          dataLen,
          0,
          callbackIdPtr
        );
      },
      fail: function (err) {
        let fileNamePtr = APGameGlobal.Module._malloc(
          APGameGlobal.Module.lengthBytesUTF8(fileName) + 1
        );
        APGameGlobal.Module.stringToUTF8(
          fileName,
          fileNamePtr,
          APGameGlobal.Module.lengthBytesUTF8(fileName) + 1
        );
        let errmsg = typeof err === 'object' ? JSON.stringify(err) : err + '';
        let errmsgPtr = APGameGlobal.Module._malloc(
          APGameGlobal.Module.lengthBytesUTF8(errmsg) + 1
        );
        APGameGlobal.Module.stringToUTF8(
          errmsg,
          errmsgPtr,
          APGameGlobal.Module.lengthBytesUTF8(errmsg) + 1
        );
        let callbackIdPtr = APGameGlobal.Module._malloc(
          APGameGlobal.Module.lengthBytesUTF8(callbackId) + 1
        );
        APGameGlobal.Module.stringToUTF8(
          callbackId,
          callbackIdPtr,
          APGameGlobal.Module.lengthBytesUTF8(callbackId) + 1
        );

        APGameGlobal.Module.dynCall_viiiii(
          callbackPtr,
          fileNamePtr,
          0,
          0,
          errmsgPtr,
          callbackIdPtr
        );
      },
    });
  },

  GetFSStatsSync: function (pathStr, recursive) {
    var fs = my.getFileSystemManager();
    var res = fs.statSync({
      path: pathStr,
      recursive: Boolean(recursive),
    });
    var jsonString = JSON.stringify(res);
    var bufferSize = APGameGlobal.Module.lengthBytesUTF8(jsonString) + 1;
    var buffer = APGameGlobal.Module._malloc(bufferSize);
    APGameGlobal.Module.stringToUTF8(jsonString, buffer, bufferSize);
    return buffer;
  },
};

export default AlipayFS;
