mergeInto(LibraryManager.library, {
  createUDPSocket: function (broadcast, multicast, bindToWifi) {
    const handleID = APGameGlobal.ALIPAYWASMSDK.Alipay_CreateUDPSocket(
      broadcast,
      multicast,
      bindToWifi
    );

    var bufferSize = lengthBytesUTF8(handleID) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(handleID, buffer, bufferSize);
    return buffer; 
  },

  closeUDPSocket: function (handleIDPtr) {
    var handleID = UTF8ToString(handleIDPtr);
    APGameGlobal.ALIPAYWASMSDK.Alipay_CloseUDPSocket(handleID);
  },

  bindUDPSocket: function (handleIDPtr, port, broadcast) {
    var handleID = UTF8ToString(handleIDPtr);
    return APGameGlobal.ALIPAYWASMSDK.Alipay_BindUDPSocket(
      handleID,
      port,
      Boolean(broadcast)
    );
  },

  sendUDPSocketMessage: function (handleIDPtr, msgPtr, paramPtr) {
    var handleID = UTF8ToString(handleIDPtr);
    var message = UTF8ToString(msgPtr);
    var param = UTF8ToString(paramPtr);
    APGameGlobal.ALIPAYWASMSDK.Alipay_SendUDPSocketMessage(
      handleID,
      message,
      param
    );
  },

  sendUDPSocketBuffer: function (handleIDPtr, dataPtr, dataLength, paramPtr) {
    var handleID = UTF8ToString(handleIDPtr);
    var param = UTF8ToString(paramPtr);
    APGameGlobal.ALIPAYWASMSDK.Alipay_SendUDPSocketBuffer(
      handleID,
      dataPtr,
      dataLength,
      param
    );
  },

  onUDPSocketMessage: function (handleIDPtr, callbackPtr) {
    var handleID = UTF8ToString(handleIDPtr);
    APGameGlobal.ALIPAYWASMSDK.Alipay_OnUDPSocketMessage(handleID, callbackPtr);
  },

  offUDPSocketMessage: function (handleIDPtr) {
    var handleID = UTF8ToString(handleIDPtr);
    APGameGlobal.ALIPAYWASMSDK.Alipay_OffUDPSocketMessage(handleID);
  },

  onUDPSocketClose: function (handleIDPtr, callbackPtr) {
    var handleID = UTF8ToString(handleIDPtr);
    var callbackID = UTF8ToString(callbackPtr);
    APGameGlobal.ALIPAYWASMSDK.Alipay_OnUDPSocketClose(handleID, callbackID);
  },

  offUDPSocketClose: function (handleIDPtr) {
    var handleID = UTF8ToString(handleIDPtr);
    APGameGlobal.ALIPAYWASMSDK.Alipay_OffUDPSocketClose(handleID);
  },

  onUDPSocketError: function (handleIDPtr, callbackPtr) {
    var handleID = UTF8ToString(handleIDPtr);
    var callbackID = UTF8ToString(callbackPtr);
    APGameGlobal.ALIPAYWASMSDK.Alipay_OnUDPSocketError(handleID, callbackID);
  },

  offUDPSocketError: function (handleIDPtr) {
    var handleID = UTF8ToString(handleIDPtr);
    APGameGlobal.ALIPAYWASMSDK.Alipay_OffUDPSocketError(handleID);
  },

  onUDPSocketListening: function (handleIDPtr, callbackPtr) {
    var handleID = UTF8ToString(handleIDPtr);
    var callbackID = UTF8ToString(callbackPtr);
    APGameGlobal.ALIPAYWASMSDK.Alipay_OnUDPSocketListening(handleID, callbackID);
  },

  offUDPSocketListening: function (handleIDPtr) {
    var handleID = UTF8ToString(handleIDPtr);
    APGameGlobal.ALIPAYWASMSDK.Alipay_OffUDPSocketListening(handleID);
  },
});
