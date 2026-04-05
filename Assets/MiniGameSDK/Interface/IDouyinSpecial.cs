using System;
using TTSDK;
using static TTSDK.TTRank;

namespace MiniGameSDK
{
    public interface IDouyinSpecial
    {
        void OnInitCompletedCallback();

        //侧边栏
        void ShowSideBar(Action<bool> callback);

        //排行榜
        void ShowRankList();
        void SetRankListData(int value);
        void GetRankData(Action<RankData> callback);
        void GetRankData(Action<int> callback);

        //tosat
        void ShotToast(string title, string icon = "", Action complete = null, int durationMS = 1000);

        void ShowRevisitGuide(Action<bool> callback = null);

        void CheckScene();

    }
}