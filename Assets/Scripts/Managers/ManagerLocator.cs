using UnityEngine;

public class ManagerLocator : MonoBehaviour
{
    //AllManager AcsessPoint
    public static ManagerLocator Instance { get; private set; }
    [field:SerializeField] public GameManager Game { get; private set; }
    [field:SerializeField] public GunManager Gun { get; private set; }
    [field:SerializeField] public BulletManager Bullet { get; private set; }
    [field:SerializeField] public CreateTargetManager CreateTarget { get; private set; }
    [field:SerializeField] public PhaseManager Phase { get; private set; }
    [field:SerializeField] public UIManager UI { get; private set; }
    [field:SerializeField] public RankingManager Ranking { get; private set; }
    [field:SerializeField] public VRInputManager Input { get;private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        InjectDependencies();
    }
    //Event購読関数
    private void InjectDependencies() 
    {
        Game.SetEvent(Phase);
        Phase.SetEvent(Game);
        CreateTarget.SetEvent(Phase,Game);
        UI.SetEvent(Game, Phase);
        Ranking.SetEvent(Game);
        Input.SetEvent(Game);
    }
    //Game Create Memo
    //Structure:
    //  ManagerLocator(static) - Managers - Controllers - Datas.
    //* ManagerLocator is static class and asosiated with all managers.
    //* Managers are controlling own item data, player data, game data, etc. 
    //* Controllers are executing own items.
    //* Datas are basis for all items.
    //

}
