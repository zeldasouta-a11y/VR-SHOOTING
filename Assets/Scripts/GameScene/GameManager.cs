using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] Vector3 minPosition = new Vector3(-15, 0, 20);
    [SerializeField] Vector3 maxPosition = new Vector3(15, 10, 50);
    [SerializeField] private int totalScore = 0;
    [SerializeField] private float createDuration = 1.0f;
    [SerializeField] private CreateTargetManager _createTargetManager;
    [SerializeField] TextMeshProUGUI scoreText
    ;
    private float timer = 0.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < createDuration )
        {
            timer += Time.deltaTime;
        }
        else
        {
            Vector3 pos = new Vector3(Random.Range(minPosition.x, maxPosition.x), Random.Range(minPosition.y, maxPosition.y), Random.Range(minPosition.z, maxPosition.z));
            _createTargetManager.CreateInstanceAndSetCameraAndScripts(0, pos);
            timer = 0.0f;
        }
    }
    public void AddScore(int point)
    {
        totalScore += point;
        if (scoreText != null)
        {
            scoreText.text = "Score: " + totalScore.ToString();
        }
        Debug.Log("Score: " + totalScore);
    }

}
