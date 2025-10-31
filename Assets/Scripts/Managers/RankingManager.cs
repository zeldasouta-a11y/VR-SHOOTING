using System;
using System.IO;
using System.Linq;
using UnityEngine;
using static UnityEngine.Analytics.IAnalytic;

public class RankingManager : MonoBehaviour
{
    [SerializeField] bool isSaveJson = true;
    [SerializeField] string FileName = "Ranking";
    [SerializeField] RankingListWrapper rankingList = new();
    public RankingListWrapper RankingList => rankingList;
    private string FilePath => Path.Combine(Application.streamingAssetsPath, FileName+".json");
    public void SetEvent(GameManager game)
    {
        game.OnGameEnd += OnGameEndHandle;
    }
    private void Start()
    {
        rankingList = LoadJson();
    }
    private void OnDisable()
    {
        var game = ManagerLocator.Instance.Game;
        if(game != null)
        {
            game.OnGameEnd -= OnGameEndHandle;
        }
    }

    private void OnGameEndHandle()
    {
        if (isSaveJson)
        {
            SaveJson();
        }
        
    }

    private void SaveJson() 
    {
        var game = ManagerLocator.Instance.Game;
        if (game == null) 
        {
            Debug.Log("Game Manager is MIssing");
            return;
        }
        RankingData data = new RankingData
        {
            Time = DateTime.Now.ToString(),
            GameSeed = game.GameSeed,
            TotalScore = game.TotalScore
        };
        data.MakeDetailData(game.TargetHitDict);
        RankingListWrapper wrapper = LoadJson();
        wrapper.Rankings.Add(data);
        ExportJson(wrapper);
        //ランキングデータ更新
        rankingList = wrapper;


    }
    private void ExportJson(RankingListWrapper wrapper)
    {
        string jsonText = JsonUtility.ToJson(wrapper, true);
        string writePath = FilePath;
        File.WriteAllText(writePath, jsonText);
        Debug.Log($"ExportedJson:\n{jsonText}at{writePath}");
    }
    private RankingListWrapper LoadJson()
    {
        if (!System.IO.File.Exists(FilePath))
        {
            Debug.LogWarning("No ranking file found");
            return new RankingListWrapper();
        }

        string json = File.ReadAllText(FilePath);
        return JsonUtility.FromJson<RankingListWrapper>(json);
    }
    private void SortJson()
    {
        // スコアの降順にソート（高いほど上位）
        rankingList.Rankings = rankingList.Rankings
            .OrderByDescending(r => r.TotalScore)
            .ThenBy(r => r.Time) // 同点ならタイム順
            .ToList();
    }
}
