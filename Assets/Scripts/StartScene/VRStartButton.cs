using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VRStartButton : MonoBehaviour
{
    public Button startButton;

    void Start()
    {
        // �{�^���������Ŏ擾
        if (startButton == null)
            startButton = GetComponent<Button>();

        // �{�^���N���b�N�C�x���g��ݒ�
        if (startButton != null)
            startButton.onClick.AddListener(OnStartButtonClicked);
    }

    public void OnStartButtonClicked()
    {
        Debug.Log("Start Button Pressed! Loading VRgeme_ver1...");

        // GameScene�ɑJ��
        SceneManager.LoadScene("VRgeme_ver1");
    }
}
