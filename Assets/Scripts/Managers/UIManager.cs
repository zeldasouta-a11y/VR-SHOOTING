using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject overLayPanel;
    [SerializeField] Image panelImage;
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timeLimitText;

    private int score => ManagerLocator.Instance.Game.TotalScore;
    private static WaitForSeconds wait1s = new WaitForSeconds(1);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (overLayPanel == null) { Debug.LogError("overLayPanel is not Exist"); }
        if(resultText == null) { Debug.LogError("result Txt is not Exist"); }
        if(scoreText == null) { Debug.LogError("score Text is not Exist"); }
        if(timeLimitText == null) { Debug.LogError("time Limit Text is not Exist"); }
    }

    public void SetEvent(GameManager game,PhaseManager phase)
    {
        game.OnGameEnd += ShowResult;
        game.OnGameStart += OnGameStart;
        phase.OnPhaseChanged += OnPhaseChangedHable;
    }
    private void OnDisable()
    {
        var game = ManagerLocator.Instance.Game;
        var phase = ManagerLocator.Instance.Phase;
        if(game != null)
        {
            game.OnGameStart -= OnGameStart;
            game.OnGameEnd -= ShowResult;
        }
        if(phase != null)
        {
            phase.OnPhaseChanged -= OnPhaseChangedHable;
        }
    }

    public void UpdateUI(float limitTimer)
    {
        if (timeLimitText != null)
        {
            timeLimitText.text = limitTimer.ToString("n1");
        }

        if (scoreText != null)
        {
            
            scoreText.text = "Score: " + score.ToString();
        }
    }
    private void ShowResult()
    {
        timeLimitText.text = "End!";
        panelImage.color = new Color(255,255,255,100);
        resultText.text = "";
        resultText.text += "スコア: " + score.ToString() + "\n";
        foreach (var item in ManagerLocator.Instance.Game.TargetHitDict)
        {
            if (item.Key == "TutorialTerget") continue;
            resultText.text += item.Key + ": " + item.Value.ToString() + "\n";
        };
        overLayPanel.SetActive(true);
    }
    private void OnGameStart()
    {
        overLayPanel.SetActive(false);
        timeLimitText.text = "Start!";
    }
    private void OnPhaseChangedHable(PhaseSettingData data)
    {
        switch(data.gamePhase)
        {
            case PhaseState.Tutorial:
                timeLimitText.text = "Tutorial";
                panelImage.color = new Color(255,255, 255, 0);
                resultText.text = "Tutorial";
                overLayPanel.SetActive(true);
                StartCoroutine(PanelFade());
                break;
            default:
                break;
        }
    }
    private IEnumerator PanelFade()
    {
        yield return wait1s;
        overLayPanel.SetActive(false);
        resultText.text = "";
    }
}
