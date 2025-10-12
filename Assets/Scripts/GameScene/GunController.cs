using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JetBrains.Annotations;
using System.Threading.Tasks;
using TMPro;

public class GunController : MonoBehaviour
{
    [SerializeField] int magazineCapacity = 10;
    [SerializeField] int reloadConstant = 200;
    /// <summary>
    /// フルオートかどうかの真偽値
    /// </summary>
    [SerializeField] bool isFullAuto = false;
    /// <summary>
    /// ノーマルモードの発射音
    /// </summary>
    [SerializeField] AudioSource shootSound = null;
    /// <summary>
    /// フルオートの時にならす音
    /// </summary>
    [SerializeField] AudioSource fullAutoSound = null;
    /// <summary>
    /// リロード音
    /// </summary>
    [SerializeField] AudioSource reloadSound = null;
    [SerializeField] TextMeshProUGUI remainText;
    /// <summary>
    /// 銃弾のプレハブ。
    /// 発砲した際に、このオブジェクトを弾として実体化する。
    /// </summary>
    [SerializeField]
    private GameObject m_bulletPrefab = null;
    private int bulletRemaining;

    /// <summary>
    /// 銃口の位置。
    /// 銃弾を実体化する時の位置や向きの設定などに使用する。
    /// </summary>
    [SerializeField]
    private Transform m_muzzlePos = null;
    private bool isShooting = false;
    private bool isfullAutoPlaying = false;
    public bool isAsync = false;
    private int reloadTime ;


    public void Start()
    {
        bulletRemaining = magazineCapacity;
        reloadTime = (magazineCapacity - bulletRemaining) * reloadConstant;
    }

    void Update()
    {
        if (isFullAuto && isShooting)
        {
            ShootAmmo();
            if (!isfullAutoPlaying)
            {
                fullAutoSound?.Play();
                isfullAutoPlaying = true;
            }
        }
        else
        {
            isfullAutoPlaying = false;
            fullAutoSound?.Stop();
        }
        remainText.text = bulletRemaining.ToString();
    }
    /// <summary>
    /// VRコントローラーのトリガーが握られた時に呼び出す。
    /// </summary>
    public async void Activate()
    {
        if (--bulletRemaining < 0)
        {
            reloadSound?.Play();
            if (isAsync)
            {
                await ReloadAsync(reloadTime);
            }
            else
            {
                StartCoroutine(Reload(reloadTime));
            }
            
            return;
        }
        if (!isFullAuto) shootSound?.Play();
        isShooting = true;
        ShootAmmo();
    }
    public void Deactivate()
    {
        isShooting = false;
    }

    private IEnumerator Reload(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        bulletRemaining = magazineCapacity;
    }
    private async Task ReloadAsync(float miliseconds)
    {
        await Task.Delay((int)miliseconds);
    }
    /// <summary>
    /// 銃弾を生成する。
    /// </summary>
    public void ShootAmmo()
    {
        //弾のプレハブか銃口位置が設定されていなければ処理を行わず帰る。ついでに煽る。
        if (m_bulletPrefab == null ||
            m_muzzlePos == null)
        {
            Debug.Log(" Inspector の設定が間違ってるでww m9(^Д^)ﾌﾟｷﾞｬｰ ");
            return;
        }

        //弾を生成する。
        GameObject bulletObj = Instantiate(m_bulletPrefab);

        //弾の位置を、銃口の位置と同一にする。
        bulletObj.transform.position = m_muzzlePos.position;

        //弾の向きを、銃口の向きと同一にする。
        bulletObj.transform.rotation = m_muzzlePos.rotation;

    }
}
#if UNITY_EDITOR

// エディタ上で試射できるボタン（ビルドには含まれない）
[CustomEditor(typeof(GunController))]
public class GunControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var gun = (GunController)target;

        EditorGUILayout.Space(8);
        if (GUILayout.Button("Shot (Editor)"))
        {
            gun.Activate();
        }
    }
}
#endif