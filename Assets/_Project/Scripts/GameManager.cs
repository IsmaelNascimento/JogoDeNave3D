using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private static GameManager m_Instance;
    public static GameManager Instance
    {
        get
        {
            if(m_Instance == null)
            {
                m_Instance = FindObjectOfType(typeof(GameManager)) as GameManager;
            }

            return m_Instance;
        }
    }    

    [Header("Points")]
    [SerializeField] private Text tTextPoints;
    [SerializeField] private int iCountPoints;
    private int points = 0;

    [Header("Life")]
    public Text tTextLife;
    [SerializeField] private GameObject goScreenGameOver;

    [Header("EnemysAir")]
    [SerializeField] private Transform tPrefabEnemy;
    [SerializeField] private float fTimeCreateEnemy;
    [SerializeField] private Transform tInstantiateEnemys;
    private float fTimerCreateEnemys = 0;
    [Header("EnemysAir")]
    [SerializeField] private Transform tPrefabEnemyTracker;
    [SerializeField] private Transform tPointUp;
    [SerializeField] private Transform tPointDown;
    [SerializeField] private float fTimeCreateEnemyTracker = 0;
    [Header("EnemysCargoShip")]
    [SerializeField] private Transform tPrefabEnemyCargoShip;
    [SerializeField] private float fTimeCreateEnemyCargoShip = 0;
    [Header("EnemysAntiAir")]
    [SerializeField]
    private Transform tPrefabEnemyAntiAir;
    [SerializeField] private float fTimeCreateEnemyAntiAir = 0;
    [Header("Sounds")]
    public AudioSource asMusicGameplay;
    [Header("For teste")]
    [SerializeField] private bool bNoCreateEnemys = false;

    //Variables Controllers
    private bool bCanControlPlayer;
    public bool CanControlPlayer
    {
        get
        {
            return bCanControlPlayer;
        }
        set
        {
            bCanControlPlayer = value;

            if (bCanControlPlayer && BossNow && CannonMain.Instance.bActiveCannonMain)
                StartCoroutine(CannonMain.Instance.LaserShot_Coroutine());
        }
    }
    public bool CanCreateEnemys { get; set; }
    public bool bAuxCanCreateEnemys = true;
    private bool bCreateEnemyCargoShipNow = true;
    private bool bCreateEnemyAntiAir = true;
    public bool BossNow { get; set; }

    private void Start()
    {
        tTextLife.text = string.Format("Life: {0}", PlayerControl.Instance.Life);
    }

    private void Update()
    {
        if (bNoCreateEnemys)
            return;

        fTimerCreateEnemys += Time.deltaTime;

        if (CanCreateEnemys)
        {
            if (fTimerCreateEnemys > fTimeCreateEnemy)
            {
                CreateEnemys();
                fTimerCreateEnemys = 0;
            }
        }
        else
            fTimerCreateEnemys = 0;

        TriggerCreateEnemysCargoShip();
        TriggerCreateEnemysAntiAir();
    }

    private void CreateEnemys()
    {
        var enemy = Instantiate(tPrefabEnemy, tInstantiateEnemys.position, Quaternion.identity);
        enemy.GetComponent<EnemyControl>().bCanShoot = PlayerControl.bAuxCanShot;
        enemy.GetComponent<EnemyControl>().bZigZag = PlayerControl.bAuxCanZigZag;
    }

    public void AddPoints(int countPoints = 0)
    {
        if(countPoints != 0)
            points += countPoints;
        else
            points += iCountPoints;

        tTextPoints.text = string.Format("Points: {0}", points);
    }

    #region UIs

    public void GameOver()
    {
        goScreenGameOver.SetActive(true);
    }

    public void OnButtonClickRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #endregion

    public void CreateEnemysTracker()
    {
        StartCoroutine(CreateEnemysTracker_Coroutine());
    }

    private void TriggerCreateEnemysCargoShip()
    {
        var triggersInScene = FindObjectsOfType(typeof(PartScene)) as PartScene[];
        var player = FindObjectOfType(typeof(PlayerControl)) as PlayerControl;

        if (player == null)
            return;

        foreach (PartScene trigger in triggersInScene)
        {
            var distance = Vector3.Distance(trigger.gameObject.transform.position, player.transform.position);

            if (distance < 10f && trigger.PartCurrent == PartScene.Part.CreateEnemyCargoShip && bCreateEnemyCargoShipNow && CanControlPlayer)
            {
                bCreateEnemyCargoShipNow = false;
                StartCoroutine(CreateEnemyCargoShip_Coroutine());
            }
        }
    }

    private void TriggerCreateEnemysAntiAir()
    {
        var triggersInScene = FindObjectsOfType(typeof(PartScene)) as PartScene[];
        var player = FindObjectOfType(typeof(PlayerControl)) as PlayerControl;

        if (player == null)
            return;

        foreach (var trigger in triggersInScene)
        {
            var distance = Vector3.Distance(trigger.gameObject.transform.position, player.transform.position);

            if (distance < 10f && trigger.PartCurrent == PartScene.Part.CreateEnemyAntiAir && bCreateEnemyAntiAir && CanControlPlayer)
            {
                bCreateEnemyAntiAir = false;
                StartCoroutine(CreateEnemyAntiAir_Coroutine());
            }
        }
    }

    #region Coroutines

    private IEnumerator CreateEnemysTracker_Coroutine()
    {
        for (int i = 0; i < 3; i++)
        {
            Instantiate(tPrefabEnemyTracker, tPointUp.position, Quaternion.Euler(0, 0, 90));
            Instantiate(tPrefabEnemyTracker, tPointDown.position, Quaternion.Euler(0, 0, 90));
            yield return new WaitForSeconds(fTimeCreateEnemyTracker);
        }
    }

    private IEnumerator CreateEnemyCargoShip_Coroutine()
    {
        for (int i = 0; i < 3; i++)
        {
            Instantiate(tPrefabEnemyCargoShip, tInstantiateEnemys.position, Quaternion.identity);
            yield return new WaitForSeconds(fTimeCreateEnemyTracker);
        }
    }

    private IEnumerator CreateEnemyAntiAir_Coroutine()
    {
        for (int i = 0; i < 10; i++)
        {
            Instantiate(tPrefabEnemyAntiAir, tInstantiateEnemys.position, Quaternion.identity);
            yield return new WaitForSeconds(fTimeCreateEnemyAntiAir);
        }
    }

    #endregion
}