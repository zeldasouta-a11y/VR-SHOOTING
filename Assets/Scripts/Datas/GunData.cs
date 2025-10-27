using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GunData
{
    [Header("Gun Base Settings")]
    [SerializeField] private GameObject gunModel;
    public GameObject gunModelObject { get { return gunModel; } }
    [SerializeField] BulletType bulletType;
    public BulletType BulletType => bulletType;

    //[SerializeField] private GameObject bulletPrefab;
    //public GameObject BulletPrefab => bulletPrefab;
    //[SerializeField] BulletData bulletData;
    //public BulletData BulletData => bulletData;
    [SerializeField] private Transform muzzlePos;
    public Transform MuzzlePos => muzzlePos;
    [SerializeField] private int magazineCapacity = 10;
    public int MagazineCapacity => magazineCapacity;

    [SerializeField] private float reloadConstant = 100;
    public float ReloadConstant => reloadConstant;

  
    // フルオート設定（1秒あたりの発射数）
    [SerializeField] private float fireRate = 0.8f;
    public float FireRate => fireRate;
    [Header("フルオート設定")]
    [SerializeField] private float fullAutoFireRate = 0.1f;
    public float FullAutoFireRate => fullAutoFireRate;

    [SerializeField] private float fillAutoReloadConstant = 0.0f;
    public float FillAutoReloadConstant => fillAutoReloadConstant;
    [Header("Sound Settings")]
    /// <summary>
    /// 射撃音
    /// </summary>
    [SerializeField] private AudioSource shootSound;
    public AudioSource ShootSound => shootSound;
    /// <summary>
    /// リロード音
    /// </summary>
    [SerializeField] private AudioSource reloadSound;
    public AudioSource ReloadSound => reloadSound;
    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI remainText;
    public TextMeshProUGUI RemainText => remainText;
    /// <summary>
    /// 予備弾
    /// </summary>

    [Header("予備弾数(所持弾)")]
    [SerializeField] bool isInfiniteAmmo = false;
    public bool IsInfiniteAmmo => isInfiniteAmmo;
    [SerializeField] private int reserveAmmo = 700;
    public int ReserveAmmo 
    {
        get => reserveAmmo;
        set => reserveAmmo = value;
    }

    [SerializeField] private TextMeshProUGUI reloadText;
    public TextMeshProUGUI ReloadText => reloadText;

    [SerializeField] private Image reloadProgress;
    public Image ReloadProgress => reloadProgress;

    public void ModeChange(float newFireRate,int newReloadConst)
    {
        fireRate = newFireRate;
        reloadConstant = newReloadConst;
    }
}
