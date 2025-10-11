using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VRStartButton : MonoBehaviour
{
    public Button startButton;

    void Start()
    {
        // ボタンを自動で取得
        if (startButton == null)
            startButton = GetComponent<Button>();

        // ボタンクリックイベントを設定
        if (startButton != null)
            startButton.onClick.AddListener(OnStartButtonClicked);
    }

    public void OnStartButtonClicked()
    {
        Debug.Log("Start Button Pressed! Loading VRgeme_ver1...");

        // GameSceneに遷移
        SceneManager.LoadScene("VRgeme_ver1");
    }
}
