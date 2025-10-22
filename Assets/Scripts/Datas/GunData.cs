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
    [Header("Sound Settings")]
    /// <summary>
    /// �m�[�}�����[�h�̔��ˉ�
    /// </summary>
    [SerializeField] private AudioSource shootSound;
    public AudioSource ShootSound => shootSound;
    /// <summary>
    /// �t���I�[�g�̎��ɂȂ炷��
    /// </summary>
    [SerializeField] private AudioSource fullAutoSound;
    public AudioSource FullAutoSound => fullAutoSound;
    /// <summary>
    /// �����[�h��
    /// </summary>
    [SerializeField] private AudioSource reloadSound;
    public AudioSource ReloadSound => reloadSound;
    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI remainText;
    public TextMeshProUGUI RemainText => remainText;
    /// <summary>
    /// �t���I�[�g���ǂ����̐^�U�l
    /// </summary>

    [Header("予備弾数(所持弾)")]
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
