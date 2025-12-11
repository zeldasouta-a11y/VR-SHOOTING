using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VRShooting.Data
{
    [System.Serializable]
    public class GunData
    {
        [Header("Gun Base Settings")]
        [Tooltip("銃のモデル")]
        [SerializeField] private GameObject gunModel;
        public GameObject gunModelObject { get { return gunModel; } }
        [Tooltip("打つ球のタイプ")]
        [SerializeField] BulletType bulletType;
        public BulletType BulletType => bulletType;

        [Tooltip("銃口の位置")]
        [SerializeField] private Transform muzzlePos;
        public Transform MuzzlePos => muzzlePos;
        [Tooltip("銃のリスポーン地点")]
        [SerializeField] private Transform gunRespawnPoint;
        public Transform GunRespawnPoint => gunRespawnPoint;
        [Tooltip("マガジンの容量")]
        [SerializeField] private int magazineCapacity = 10;
        public int MagazineCapacity => magazineCapacity;
        [Tooltip("リロード定数(millis)")]
        [SerializeField] private float reloadConstant = 100;
        public float ReloadConstant => reloadConstant;


        /// <summary>
        /// フルオート設定（1秒あたりの発射数）
        /// </summary>
        [Tooltip("球を打ってから何秒後に次が打てるか")]
        [SerializeField] private float fireRate = 0.8f;
        public float FireRate => fireRate;
        [Header("フルオート設定")]
        [Tooltip("フルオート時の連射率")]
        [SerializeField] private float fullAutoFireRate = 0.1f;
        public float FullAutoFireRate => fullAutoFireRate;

        [Tooltip("フルオート時のリロード定数(millis)")]
        [SerializeField] private float fillAutoReloadConstant = 0.0f;
        public float FillAutoReloadConstant => fillAutoReloadConstant;
        [Header("Sound Settings")]
        /// <summary>
        /// 射撃音
        /// </summary>
        [Tooltip("射撃音")]
        [SerializeField] private AudioSource shootSound;
        public AudioSource ShootSound => shootSound;
        /// <summary>
        /// 空薬莢音
        /// </summary>
        [Tooltip("空薬莢音")]
        [SerializeField] private AudioSource emptySound;
        public AudioSource EmptySound => emptySound;
        [Tooltip("リロード音")]
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

        public void ModeChange(float newFireRate, int newReloadConst)
        {
            fireRate = newFireRate;
            reloadConstant = newReloadConst;
        }
    }

}
