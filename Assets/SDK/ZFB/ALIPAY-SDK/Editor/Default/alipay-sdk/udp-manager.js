import APGameGlobal from './global';
import { generateUniqueHandleID } from './utils';

const longLiveUDPObjects = {};
const udpMessageCallbacks = {};
const udpCloseCallbacks = {};
const udpErrorCallbacks = {};
const udpListeningCallbacks = {};

const AlipayUDPManager = {
  Alipay_CreateUDPSocket: function (
    broadcast = false,
    multicast = false,
    bindToWifi = false
  ) {
    const handleID = generateUniqueHandleID(); // 生成唯一的handleID
    const options = {
      broadcast: Boolean(broadcast),
      multicast: Boolean(multicast),
      bindToWifi: Boolean(bindToWifi),
    };
    const socket = my.createUDPSocket(options);
    longLiveUDPObjects[handleID] = socket;
    return handleID;
  },

  Alipay_CloseUDPSocket: function (handleID) {
    if (longLiveUDPObjects[handleID]) {
      longLiveUDPObjects[handleID].close();
      delete longLiveUDPObjects[handleID];
    }
  },

  Alipay_BindUDPSocket: function (handleID, port, broadcast = false) {
    const socket = longLiveUDPObjects[handleID];
    if (socket) {
      const options = {
        port,
        broadcast: Boolean(broadcast),
      };
      return socket.bind(options);
    }
    return null;
  },

  Alipay_SendUDPSocketMessage: function (handleID, message, param) {
    const paramObj = JSON.parse(param);
    const { address, port } = paramObj;
    const socket = longLiveUDPObjects[handleID];
    if (socket) {
      socket.send({
        address: address,
        port: port,
        message: message,
      });
    } else {
      console.error('UDPSocket not found for handleID:', handleID);
    }
  },

  Alipay_SendUDPSocketBuffer: function (handleID, dataPtr, dataLength, param) {
    const paramObj = JSON.parse(param);
    const { address, port } = paramObj;
    const socket = longLiveUDPObjects[handleID];
    if (socket) {
      const messageData = {
        address: address,
        port: port,
        message: APGameGlobal.Module.HEAPU8.slice(dataPtr, dataPtr + dataLength)
          .buffer,
      };

      if (paramObj.length && paramObj.offset) {
        messageData.offset = paramObj.offset;
        messageData.length = paramObj.length;
      }
      socket.send(messageData);
    } else {
      console.error('UDPSocket not found for handleID:', handleID);
    }
  },

  Alipay_OnUDPSocketMessage: function (handleID, callback) {
    const socket = longLiveUDPObjects[handleID];

    if (socket) {
      const callbackKey = handleID + '_message';

      udpMessageCallbacks[callbackKey] = (messageEvent) => {
        const { remoteInfo, message } = messageEvent;

        let uint8Message;
        if (Array.isArray(message)) {
          try {
            uint8Message = new Uint8Array(message);
          } catch (e) {
            console.error('Failed to convert message to Uint8Array:', e);
            return;
          }
        } else if (message instanceof ArrayBuffer) {
          uint8Message = new Uint8Array(message);
        } else if (message instanceof Uint8Array) {
          uint8Message = message;
        } else {
          console.error('Invalid message type:', message);
          return;
        }

        const remoteInfoJson = JSON.stringify(remoteInfo);
        const remoteInfoPtr = APGameGlobal.Module._malloc(
          APGameGlobal.Module.lengthBytesUTF8(remoteInfoJson) + 1
        );
        APGameGlobal.Module.stringToUTF8(
          remoteInfoJson,
          remoteInfoPtr,
          APGameGlobal.Module.lengthBytesUTF8(remoteInfoJson) + 1
        );

        const messageBuffer = APGameGlobal.Module._malloc(
          uint8Message.byteLength
        );
        APGameGlobal.Module.HEAPU8.set(uint8Message, messageBuffer);

        const handleIDPtr = APGameGlobal.Module._malloc(
          APGameGlobal.Module.lengthBytesUTF8(handleID) + 1
        );
        APGameGlobal.Module.stringToUTF8(
          handleID,
          handleIDPtr,
          APGameGlobal.Module.lengthBytesUTF8(handleID) + 1
        );

        APGameGlobal.Module.dynCall_viiii(
          callback,
          handleIDPtr,
          remoteInfoPtr,
          messageBuffer,
          uint8Message.byteLength
        );

        APGameGlobal.Module._free(messageBuffer);
        APGameGlobal.Module._free(remoteInfoPtr);
        APGameGlobal.Module._free(handleIDPtr);
      };
      socket.onMessage(udpMessageCallbacks[callbackKey]);
    }
  },

  Alipay_OffUDPSocketMessage: function (handleID) {
    const socket = longLiveUDPObjects[handleID];
    if (socket) {
      const callbackKey = handleID + '_message';
      if (udpMessageCallbacks[callbackKey]) {
        socket.offMessage(udpMessageCallbacks[callbackKey]);
        delete udpMessageCallbacks[callbackKey];
      }
    }
  },

  Alipay_OnUDPSocketClose: function (handleID, callbackID) {
    const socket = longLiveUDPObjects[handleID];
    if (socket) {
      const callbackKey = handleID + '_close';
      udpCloseCallbacks[callbackKey] = (event) => {
        var resultString = JSON.stringify({
          messageId: callbackID,
          result: event,
        });
        APGameGlobal.Module.SendMessage(
          'AlipayBridge',
          'ReceiveMessageFromJS',
          resultString
        );
      };
      socket.onClose(udpCloseCallbacks[callbackKey]);
    }
  },

  Alipay_OffUDPSocketClose: function (handleID) {
    const socket = longLiveUDPObjects[handleID];
    if (socket) {
      const callbackKey = handleID + '_close';
      if (udpCloseCallbacks[callbackKey]) {
        socket.offClose(udpCloseCallbacks[callbackKey]);
        delete udpCloseCallbacks[callbackKey];
      }
    }
  },

  Alipay_OnUDPSocketError: function (handleID, callbackID) {
    const socket = longLiveUDPObjects[handleID];
    if (socket) {
      const callbackKey = handleID + '_error';
      udpErrorCallbacks[callbackKey] = (event) => {
        var resultString = JSON.stringify({
          messageId: callbackID,
          result: event,
        });
        APGameGlobal.Module.SendMessage(
          'AlipayBridge',
          'ReceiveMessageFromJS',
          resultString
        );
      };
      socket.onError(udpErrorCallbacks[callbackKey]);
    }
  },

  Alipay_OffUDPSocketError: function (handleID) {
    const socket = longLiveUDPObjects[handleID];
    if (socket) {
      const callbackKey = handleID + '_error';
      if (udpErrorCallbacks[callbackKey]) {
        socket.offError(udpErrorCallbacks[callbackKey]);
        delete udpErrorCallbacks[callbackKey];
      }
    }
  },

  Alipay_OnUDPSocketListening: function (handleID, callbackID) {
    const socket = longLiveUDPObjects[handleID];
    if (socket) {
      const callbackKey = handleID + '_listening';
      udpListeningCallbacks[callbackKey] = (event) => {
        var resultString = JSON.stringify({
          messageId: callbackID,
          result: event,
        });
        APGameGlobal.Module.SendMessage(
          'AlipayBridge',
          'ReceiveMessageFromJS',
          resultString
        );
      };
      socket.onListening(udpListeningCallbacks[callbackKey]);
    }
  },

  Alipay_OffUDPSocketListening: function (handleID) {
    const socket = longLiveUDPObjects[handleID];
    if (socket) {
      const callbackKey = handleID + '_listening';
      if (udpListeningCallbacks[callbackKey]) {
        socket.offListening(udpListeningCallbacks[callbackKey]);
        delete udpListeningCallbacks[callbackKey];
      }
    }
  },
};

export default AlipayUDPManager;
