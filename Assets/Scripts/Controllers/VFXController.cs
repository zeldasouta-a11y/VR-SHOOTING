using System.Collections;
using UnityEngine;

public class VFXController : MonoBehaviour,IHitReceiver
{

    [Tooltip("壊れる瞬間のSE（1クリップでOK）")]
    [SerializeField] private AudioClip breakSfxClip;
    [Header("SFX (simple)")]
    [SerializeField] private AudioSource sfx;                 // ここに同じオブジェクトのAudioSourceを割り当て（未設定ならAwakeで自動追加）
    [Tooltip("汎用の破片SE（1つでも可）。TargetData.BreakSfxClipが設定されていればそちらを優先。")]
    [SerializeField] private AudioClip[] debrisClips;
    [Range(0f, 0.3f)][SerializeField] private float sfxDelay = 0.07f;   // 銃声と少しズラす
    [SerializeField] private bool addDistanceDelay = false;               // 距離による音の伝搬遅延
    [Range(0.5f, 1.5f)][SerializeField] private float distanceDelayScale = 1f;
    [Range(0f, 2f)][SerializeField] private float pitchJitterSemitones = 0.5f;
    [Range(0.7f, 1f)][SerializeField] private float volumeJitterMin = 0.9f;
    private Canvas canvas;
    private void Awake()
    {
        if (!sfx) sfx = GetComponent<AudioSource>();
        if (!sfx) sfx = gameObject.AddComponent<AudioSource>();
        sfx.playOnAwake = false;
        sfx.spatialBlend = 1f;
        sfx.rolloffMode = AudioRolloffMode.Logarithmic;
        sfx.minDistance = 2f;
        sfx.maxDistance = 50f;
        sfx.dopplerLevel = 0f;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    //当たった時
    public void OnHitNotify()
    {
        Debug.Log("Hit Receive");
        this.gameObject.SetActive(true);
        SpawnBreakFx();
        PlayBreakSfx();
    }
    // 視覚：Break VFXを生成
    private void SpawnBreakFx()
    {

        Vector3 pos = transform.position;

        var r = GetComponentInChildren<Renderer>();
        if (r) pos = r.bounds.center;

    }

    // 音：TargetDataの個別SEを優先。なければdebrisClipsから再生。
    private void PlayBreakSfx()
    {
        AudioClip clip = null;
        if(breakSfxClip != null)
            clip = breakSfxClip;
        else if (debrisClips != null && debrisClips.Length > 0)
            clip = debrisClips[Random.Range(0, debrisClips.Length)];

        if (clip == null) return;
        StartCoroutine(PlaySfxWithDelay(clip));
    }

    private IEnumerator PlaySfxWithDelay(AudioClip clip)
    {
        float delay = sfxDelay;

        if (addDistanceDelay)
        {
            Transform listener = (canvas && canvas.worldCamera) ? canvas.worldCamera.transform
                                   : (Camera.main ? Camera.main.transform : null);
            if (listener)
            {
                float dist = Vector3.Distance(listener.position, transform.position);
                delay += (dist / 343f) * distanceDelayScale; // 音速おおよそ343m/s
            }
        }

        if (delay > 0f) yield return new WaitForSeconds(delay);

        float semi = Random.Range(-pitchJitterSemitones, pitchJitterSemitones);
        sfx.pitch = Mathf.Pow(2f, semi / 12f);

        float vol = Random.Range(volumeJitterMin, 1f);
        sfx.PlayOneShot(clip, vol);

        sfx.pitch = 1f;
    }
}
