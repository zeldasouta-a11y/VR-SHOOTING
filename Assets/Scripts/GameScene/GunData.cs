using TMPro;
using UnityEngine;


[System.Serializable]
public class GunData 
{
    //������Unity�֘A�̃I�u�W�F�N�g������������ƃG���[���o�܂�
    [SerializeField] private GameObject gunModel;
    public GameObject gunModelObject {get { return gunModel; }}
    /// <summary>
    /// �e�e�̃v���n�u�B
    /// ���C�����ۂɁA���̃I�u�W�F�N�g��e�Ƃ��Ď��̉�����B
    /// </summary>
    [SerializeField] private GameObject bulletPrefab ;
    public GameObject BulletPrefab => bulletPrefab;
    /// <summary>
    /// �e���̈ʒu
    /// </summary>
    [SerializeField] private Transform muzzlePos;
    public Transform MuzzlePos => muzzlePos;
    [SerializeField] private int magazineCapacity  = 10;
    public int MagazineCapacity => magazineCapacity;
    [SerializeField] private int reloadConstant = 100;
    public int ReloadConstant => reloadConstant;
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
    [SerializeField] private TextMeshProUGUI remainText;
    public TextMeshProUGUI RemainText => remainText;
    /// <summary>
    /// �t���I�[�g���ǂ����̐^�U�l
    /// </summary>
    [SerializeField] bool isFullAuto = false;
    public bool IsFullAuto
    {
        get { return isFullAuto; }
        set { isFullAuto = value; }
    }

    }
