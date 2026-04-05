mergeInto(LibraryManager.library, {
  createTCPSocket: function (bindToWifi) {
    const handleID = APGameGlobal.ALIPAYWASMSDK.Alipay_CreateTCPSocket(bindToWifi);

    var bufferSize = lengthBytesUTF8(handleID) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(handleID, buffer, bufferSize);
    return buffer;
  },

  closeTCPSocket: function (handleIDPtr) {
    var handleID = UTF8ToString(handleIDPtr);
    APGameGlobal.ALIPAYWASMSDK.Alipay_CloseTCPSocket(handleID);
  },

  connectTCPSocket: function (handleIDPtr, addressPtr, port, timeout) {
    var handleID = UTF8ToString(handleIDPtr);
    var address = UTF8ToString(addressPtr);
    APGameGlobal.ALIPAYWASMSDK.Alipay_ConnectTCPSocket(handleID, address, port, timeout);
  },

  sendTCPSocketMessage: function (handleIDPtr, msgPtr, paramPtr) {
    var handleID = UTF8ToString(handleIDPtr);
    var message = UTF8ToString(msgPtr);
    var param = UTF8ToString(paramPtr);
    APGameGlobal.ALIPAYWASMSDK.Alipay_SendTCPSocketMessage(handleID, message, param);
  },

  sendTCPSocketBuffer: function (handleIDPtr, dataPtr, dataLength, paramPtr) {
    var handleID = UTF8ToString(handleIDPtr);
    var param = UTF8ToString(paramPtr);
    APGameGlobal.ALIPAYWASMSDK.Alipay_SendTCPSocketBuffer(handleID, dataPtr, dataLength, param);
  },

  onTCPSocketMessage: function (handleIDPtr, callbackPtr) {
    var handleID = UTF8ToString(handleIDPtr);
    APGameGlobal.ALIPAYWASMSDK.Alipay_OnTCPSocketMessage(handleID, callbackPtr);
  },

  offTCPSocketMessage: function (handleIDPtr) {
    var handleID = UTF8ToString(handleIDPtr);
    APGameGlobal.ALIPAYWASMSDK.Alipay_OffTCPSocketMessage(handleID);
  },

  onTCPSocketClose: function (handleIDPtr, callbackPtr) {
    var handleID = UTF8ToString(handleIDPtr);
    var callbackID = UTF8ToString(callbackPtr);
    APGameGlobal.ALIPAYWASMSDK.Alipay_OnTCPSocketClose(handleID, callbackID);
  },

  offTCPSocketClose: function (handleIDPtr) {
    var handleID = UTF8ToString(handleIDPtr);
    APGameGlobal.ALIPAYWASMSDK.Alipay_OffTCPSocketClose(handleID);
  },

  onTCPSocketError: function (handleIDPtr, callbackPtr) {
    var handleID = UTF8ToString(handleIDPtr);
    var callbackID = UTF8ToString(callbackPtr);
    APGameGlobal.ALIPAYWASMSDK.Alipay_OnTCPSocketError(handleID, callbackID);
  },

  offTCPSocketError: function (handleIDPtr) {
    var handleID = UTF8ToString(handleIDPtr);
    APGameGlobal.ALIPAYWASMSDK.Alipay_OffTCPSocketError(handleID);
  },

  onTCPSocketConnect: function (handleIDPtr, callbackPtr) {
    var handleID = UTF8ToString(handleIDPtr);
    var callbackID = UTF8ToString(callbackPtr);
    APGameGlobal.ALIPAYWASMSDK.Alipay_OnTCPSocketConnect(handleID, callbackID);
  },

  offTCPSocketConnect: function (handleIDPtr) {
    var handleID = UTF8ToString(handleIDPtr);
    APGameGlobal.ALIPAYWASMSDK.Alipay_OffTCPSocketConnect(handleID);
  },
});
