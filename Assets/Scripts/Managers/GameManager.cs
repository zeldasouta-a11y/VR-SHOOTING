using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;


namespace VRShooting.Manager
{
    public enum GameState { Idle, Playing, Paused, Ended }
    public enum Tutorial { Use, Skip };

    //UnityEvent Inspectror�Őݒ�\
    [System.Serializable]

    public class GameManager : MonoBehaviour
    {



        [Header("Seed")]
        [SerializeField] bool isCustomSeed = true;
        [EnableIf("isCustomSeed", hideWhenFalse: false)]
        [SerializeField] int gameSeed = 12345;
        public int GameSeed => gameSeed;

        [Header("BGM Setting")]
        [SerializeField] AudioSource fullAutoBGM;
        [SerializeField] AudioSource enddingBGM;
        [Header("other")]
        [EnableIf("isFullAutoMode", hideWhenFalse: false)]
        [SerializeField] private int totalScore = 0;
        public int TotalScore => totalScore;
        [SerializeField] float fullAutoDuration = 35.0f;
        [Header("チュートリアルが有効かどうか")]
        [SerializeField] Tutorial tutorial;
        public Tutorial Tutorial => tutorial;
        Dictionary<string, int> targetHitCount = new Dictionary<string, int>();
        public IReadOnlyDictionary<string, int> TargetHitDict => targetHitCount;
        private GameState gamestate = GameState.Idle;
        public GameState State => gamestate;
        private bool isFullAutoMode = false;


        //System Event
        public event Action<bool> OnFullAutoChanged;
        public event Action OnGameStart;
        //正常終了 true,中断はfalse
        public event Action<bool> OnGameEnd;
        public event Action OnHit;
        private void Start()
        {
            StartGame();
        }
        private void OnDisable()
        {
            var phase = ManagerLocator.Instance.Phase;
            if (phase != null)
            {
                phase.OnAllPhaseEnd -= GameEnd;
            }
        }
        public void SetEvent(PhaseManager phase)
        {
            phase.OnAllPhaseEnd += GameEnd;
        }
        [OnInspectorButton("Game Restart", true)]
        public void GameRestart()
        {
#if UNITY_EDITOR
            Debug.Log("GameRestart !");
#endif
            //ゲーム中断処理
            if (gamestate != GameState.Ended)
            {
                gamestate = GameState.Ended;
                OnGameEnd?.Invoke(false);
            }
            enddingBGM.Stop();
            //1フレーム待ってから実行(厳密にはdeltaTime)
            Invoke(nameof(StartGame), Time.deltaTime);
        }
        [OnInspectorButton("", true)]
        public void StartFullAuto()
        {
            if (!isFullAutoMode)
                StartCoroutine(FullAutoMode());
        }
        private IEnumerator FullAutoMode()
        {
            isFullAutoMode = true;
            fullAutoBGM.Play();
            OnFullAutoChanged?.Invoke(true);
            yield return new WaitForSeconds(fullAutoDuration);
            fullAutoBGM.Stop();
            OnFullAutoChanged?.Invoke(false);
            isFullAutoMode = false;
        }

        private void StartGame()
        {
#if UNITY_EDITOR
            Debug.Log("[GameManager]GameStart!");
#endif
            if (!isCustomSeed)
            {
                long tick = DateTime.Now.Ticks;
                gameSeed = (int)tick;
            }
            //多重スタート禁止
            if (gamestate == GameState.Playing)
            {
                return;
            }
            totalScore = 0;
            targetHitCount.Clear();
            gamestate = GameState.Playing;
            OnGameStart?.Invoke();
        }

        public void GameEnd()
        {
#if UNITY_EDITOR
            Debug.Log("GameEnd!");
#endif
            gamestate = GameState.Ended;
            enddingBGM.Play();
            fullAutoBGM?.Stop();
            OnGameEnd?.Invoke(true);
        }
        
        public void AddScore(int point, string name)
        {
            if (!ManagerLocator.Instance.Phase.IsIgnoreScoreMode)
            {
                totalScore += point;
                if (targetHitCount.TryGetValue(name, out int count))
                {
                    targetHitCount[name] = count + 1;
                }
                else
                {
                    targetHitCount[name] = 1;
                }
            }


            OnHit?.Invoke();
        }
    }

}
