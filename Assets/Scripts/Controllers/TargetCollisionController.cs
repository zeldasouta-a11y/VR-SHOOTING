using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TargetCollisionController : MonoBehaviour
{
    [SerializeField] GameObject pointCanvas;
    [SerializeField] Canvas canvas;
    [SerializeField] TextMeshProUGUI hittext;
    [HideInInspector] GameObject targetModel;
    [SerializeField]
    TargetData targetDatas;
    Vector3 moving;
    private readonly WaitForSeconds fixedUpdate = new WaitForSeconds(1f);
    private bool isFixedUpdate = false;
    private float time = 0f;

    /// <summary>
    /// �K���A�C���X�^���X�쐬����ɌĂԂ���
    /// </summary>
    /// <param name="score"></param>
    /// <param name="time"></param>
    public void Init(TargetData _data,GameObject Model ,Camera targetCamera)
    {
        targetDatas = _data;
        targetModel = Model;
        canvas.worldCamera = targetCamera;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (pointCanvas == null)
        {
            Debug.LogError("PointCanvas is not assigned in the inspector.");
        }
        if (targetDatas.TargetModel == null)
        {
            Debug.LogError("TargetModel is not assigned in the inspector.");
        }
        moving = targetDatas.MoveVector;
        pointCanvas.SetActive(false);
        if (targetDatas.IsVanish) 
        {
            Destroy(this.gameObject, targetDatas.VanishTime);
        }
    }
    void Update()
    {
        if (!targetDatas.IsMovable) return;
        if (time > targetDatas.MoveDurtation)
        {
            time = 0f;
            if (targetDatas.IsUFOMove)
            {
                int rotationX = Random.Range(0, 360);
                int rotationY = Random.Range(0, 360);
                int rotationZ = Random.Range(0, 360);
                moving = Quaternion.Euler(rotationX,rotationY,rotationZ) * targetDatas.MoveVector;
            }
            else if (targetDatas.IsPendulumMove)
            {
                moving *= -1;
            }
        }
        this.gameObject.transform.localPosition += moving * Time.deltaTime;
        time += Time.deltaTime;
    }
    private IEnumerator Moveing()
    {
        isFixedUpdate = true;
        yield return fixedUpdate;
        isFixedUpdate = false;
    }
    void OnTriggerEnter(Collider collision)
    {
        string objecttag = collision.gameObject.tag;
        if (objecttag == "bullet")
        {

            OnHitUI();
            ManagerLocator.Instance.Game.AddScore(targetDatas.HitScore,targetDatas.ModelName);
            
            Destroy(this.gameObject,3.0f);
        }
    }
    private void OnHitUI() 
    {
        if (targetModel != null) targetModel.gameObject.SetActive(false);
        if (hittext == null)
        {
            hittext = pointCanvas.AddComponent<TextMeshProUGUI>();
        }
        hittext.text = targetDatas.HitScore.ToString();
        pointCanvas.gameObject.SetActive(true);
    }
}
