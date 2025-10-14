using TMPro;
using UnityEngine;


[System.Serializable]
public class GunData 
{
    //ここでUnity関連のオブジェクトを初期化するとエラーが出ます
    [SerializeField] private GameObject gunModel;
    public GameObject gunModelObject {get { return gunModel; }}
    /// <summary>
    /// 銃弾のプレハブ。
    /// 発砲した際に、このオブジェクトを弾として実体化する。
    /// </summary>
    [SerializeField] private GameObject bulletPrefab ;
    public GameObject BulletPrefab => bulletPrefab;
    /// <summary>
    /// 銃口の位置
    /// </summary>
    [SerializeField] private Transform muzzlePos;
    public Transform MuzzlePos => muzzlePos;
    [SerializeField] private int magazineCapacity  = 10;
    public int MagazineCapacity => magazineCapacity;
    [SerializeField] private int reloadConstant = 100;
    public int ReloadConstant => reloadConstant;
    /// <summary>
    /// ノーマルモードの発射音
    /// </summary>
    [SerializeField] private AudioSource shootSound;
    public AudioSource ShootSound => shootSound;
    /// <summary>
    /// フルオートの時にならす音
    /// </summary>
    [SerializeField] private AudioSource fullAutoSound;
    public AudioSource FullAutoSound => fullAutoSound;
    /// <summary>
    /// リロード音
    /// </summary>
    [SerializeField] private AudioSource reloadSound;
    public AudioSource ReloadSound => reloadSound;
    [SerializeField] private TextMeshProUGUI remainText;
    public TextMeshProUGUI RemainText => remainText;
    /// <summary>
    /// フルオートかどうかの真偽値
    /// </summary>
    [SerializeField] bool isFullAuto = false;
    public bool IsFullAuto
    {
        get { return isFullAuto; }
        set { isFullAuto = value; }
    }

    }
