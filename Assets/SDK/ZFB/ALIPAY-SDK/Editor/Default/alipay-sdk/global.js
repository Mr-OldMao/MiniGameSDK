 const APGameGlobal = {};

 APGameGlobal.onLaunchProgress = function (e) {
   if (e.type === 'launchPlugin') {}
   if (e.type === 'loadWasm') {}
   if (e.type === 'compileWasm') {}
   if (e.type === 'loadAssets') {}
   if (e.type === 'prepareGame') {
     APGameGlobal.ALIPAYWASMSDK.SendMonitor(1, {
       type: 'used_unity_texture_compression',
       used: APGameGlobal.USED_TEXTURE_COMPRESSION,
     });
   }
 };

 export default APGameGlobal;