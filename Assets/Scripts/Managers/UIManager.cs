using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRShooting.Data;
namespace VRShooting.Manager
{
    public class UIManager : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] GameObject overLayPanel;
        [SerializeField] Image panelImage;
        [SerializeField] TextMeshProUGUI resultText;
        [SerializeField] TextMeshProUGUI scoreText;
        [SerializeField] TextMeshProUGUI timeLimitText;

        private int score => ManagerLocator.Instance.Game.TotalScore;
        private static WaitForSeconds wait05s = new WaitForSeconds(0.5f);
        private static WaitForSeconds wait1s = new WaitForSeconds(1);
        private string[] dotArray = new string[4] { "", ".", "..", "..." };
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (overLayPanel == null) { Debug.LogError("overLayPanel is not Exist"); }
            if (resultText == null) { Debug.LogError("result Txt is not Exist"); }
            if (scoreText == null) { Debug.LogError("score Text is not Exist"); }
            if (timeLimitText == null) { Debug.LogError("time Limit Text is not Exist"); }
        }

        public void SetEvent(GameManager game, PhaseManager phase)
        {
            game.OnGameEnd += ShowResult;
            game.OnGameStart += OnGameStart;
            phase.OnPhaseChanged += OnPhaseChangedHable;
        }
        private void OnDisable()
        {
            var game = ManagerLocator.Instance.Game;
            var phase = ManagerLocator.Instance.Phase;
            if (game != null)
            {
                game.OnGameStart -= OnGameStart;
                game.OnGameEnd -= ShowResult;
            }
            if (phase != null)
            {
                phase.OnPhaseChanged -= OnPhaseChangedHable;
            }
        }

        public void UpdateTimerUI(float limitTimer)
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
        public void UpdateTimerUI(string stringText)
        {
            if (timeLimitText != null)
            {
                timeLimitText.text = stringText;
            }
            if (scoreText != null)
            {
                scoreText.text = "Score: " + score.ToString();
            }

        }
        private void ShowResult(bool isgameComplete)
        {
            if (!isgameComplete)
            {
                return;
            }
            timeLimitText.text = "End!";
            panelImage.color = new Color(255, 255, 255, 100);
            resultText.text = "";
            resultText.text += "スコア: " + score.ToString() + "\n";
            foreach (var item in ManagerLocator.Instance.Game.TargetHitDict)
            {
                if (item.Key == "TutorialTerget" || string.IsNullOrEmpty(item.Key)) continue;
                resultText.text += item.Key + ": " + item.Value.ToString() + "\n";
            }
            ;
            overLayPanel.SetActive(true);
        }
        private void OnGameStart()
        {
            overLayPanel.SetActive(false);
            timeLimitText.text = "Start!";
        }
        private void OnPhaseChangedHable(PhaseSettingData data)
        {
            switch (data.gamePhase)
            {
                case PhaseState.Tutorial:
                    panelImage.color = new Color(255, 255, 255, 0);
                    resultText.text = "Tutorial";
                    overLayPanel.SetActive(true);
                    StartCoroutine(PanelFade());
                    break;
                default:
                    break;
            }
        }
        public void StartLoading(float waitTime, string endMassage = "")
        {
            StartCoroutine(NowLoading(waitTime, endMassage));
        }
        private IEnumerator NowLoading(float waitTime, string endMessage = "")
        {
            int dotCount = 0;
            for (float i = 0; i <= waitTime; i += 0.5f)
            {
                dotCount %= dotArray.Length;
                UpdateTimerUI("loading" + dotArray[dotCount++]);
                yield return wait05s;
            }
            UpdateTimerUI(endMessage);
        }
        private IEnumerator PanelFade()
        {
            yield return wait1s;
            overLayPanel.SetActive(false);
            resultText.text = "";
        }
    }

}
