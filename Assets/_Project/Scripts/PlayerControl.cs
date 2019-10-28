using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour
{
    private static PlayerControl m_Instance;
    public static PlayerControl Instance
    {
        get
        {
            if(m_Instance == null)
            {
                m_Instance = FindObjectOfType<PlayerControl>();
            }

            return m_Instance;
        }
    }

    [Header("Setup Ship")]
    [SerializeField] private bool bShotAutomatic;
    [SerializeField] private int iLife;
    public int Life
    {
        get
        {
            return iLife;
        }
        set
        {
            iLife = value;

            GameManager.Instance.tTextLife.text = string.Format("Life: {0}", iLife);

            if (iLife < 1)
                Dead();
        }
    }
    [SerializeField] private int iCountShot;
    [SerializeField] private float fTimeReloadShot;
    [SerializeField] private float fVelocityMovimentShip;
    [SerializeField] private float fVelocityMovimentShipTouch;
    [SerializeField] private float fTimeInvincible;
    [Space(10)]

    [SerializeField] private Transform tPrefabShot;
    [SerializeField] private Transform tWeaponShipUp;
    [SerializeField] private Transform tWeaponShipCenter;
    [SerializeField] private Transform tWeaponShipDown;
    public GameObject goParticleDestroyEnemy;
    [SerializeField] private AudioSource asInputShip;

    [Header("Moviment Touch")]
    [SerializeField] private Transform tParticleTouch;
    private bool bCollisionNow;
    public static int iAuxCountShot;
    private bool bCanShot;

    private bool bInvincible;
    public static bool bEnemyAirCanShot;
    public static bool bAuxCanZigZag;
    public static bool bAuxCanShot;
    private bool bCreateEnemyTrackerNow;
    private float fTimer = 0;
    public bool TripleShot { get; set; }

    [Header("Power ups")]
    private bool bFieldForceIsActived = false;
    private int iCollisionsAvailableForceField;
    public int CollisionsAvailableForceField
    {
        get
        {
            return iCollisionsAvailableForceField;
        }
        set
        {
            iCollisionsAvailableForceField = value;

            if(iCollisionsAvailableForceField != 0)
            {
                bFieldForceIsActived = true;
                goFxFieldForce.SetActive(true);
            }
            else
            {
                bFieldForceIsActived = false;
                goFxFieldForce.SetActive(false);
            }
        }
    }
    [SerializeField] private GameObject goFxFieldForce;
    public float VelocityShotPlayer { get; set; }

    [Header("Charge Shot")]
    [SerializeField] private float fTimeFillMin = 1;
    [SerializeField] private float fTimeFillMax = 2;
    public bool CanChargeShotMore { get; set; }
    private bool bCanChargeShotNow = false;
    [SerializeField] private Transform tPrefabChargeShot;
    [SerializeField] private GameObject tFxLoadingChargeShot;
    private float fTimerChargeShot = 0;


    private void Awake()
    {
        goFxFieldForce.AddComponent<FieldForce>();
    }

    private void Start()
    {
        CanChargeShotMore = true;
        bEnemyAirCanShot = true;
        bCreateEnemyTrackerNow = true;
        iAuxCountShot = 0;
        bCanShot = true;
        bAuxCanZigZag = false;
        bAuxCanShot = false;
        VelocityShotPlayer = tPrefabShot.GetComponent<ShotPlayer>().fVelocityShot;
        TripleShot = false;
    }

    private void Update()
    {
        if (GameManager.Instance.CanControlPlayer)
        {
            gameObject.GetComponentInChildren<MeshRenderer>().enabled = true;
            MovimentWithKeyboard();
            MovimentWithTouch();
            Shot();
        }

        if (bShotAutomatic && GameManager.Instance.CanControlPlayer)
            StartCoroutine(Shot_Coroutine());

        if (bInvincible)
        {
            fTimer += Time.deltaTime;

            if (fTimer >= fTimeInvincible)
            {
                bInvincible = false;
                fTimer = 0;
            }
            else
                return;
        }

        DetectCollision();
        ChargeShot();
    }

    private void MovimentWithKeyboard()
    {
        var worldToScreen = Camera.main.WorldToScreenPoint(transform.position);

        if (Input.GetKey(KeyCode.W)) // Up
        {
            if (worldToScreen.y < (Screen.height - 20))
            {
                transform.Translate(Vector3.up * Time.deltaTime * fVelocityMovimentShip);
            }
        }

        if (Input.GetKey(KeyCode.S)) // Down 
        {
            if (worldToScreen.y > 20)
            {
                transform.Translate(Vector3.down * Time.deltaTime * fVelocityMovimentShip);
            }
        }

        if (Input.GetKey(KeyCode.A)) // Left
        {
            if (worldToScreen.x > 20)
            {
                transform.Translate(Vector3.left * Time.deltaTime * fVelocityMovimentShip);
            }
        }

        if (Input.GetKey(KeyCode.D)) // Right
        {
            if (worldToScreen.x < (Screen.width / 2))
            {
                transform.Translate(Vector3.right * Time.deltaTime * fVelocityMovimentShip);
            }
        }
    }

    private void MovimentWithTouch()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var touchCurrent = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var newPosition = new Vector3(touchCurrent.x, touchCurrent.y, transform.position.z);
            tParticleTouch.position = newPosition;
            tParticleTouch.GetComponent<ParticleSystem>().Play();
            transform.DOMove(newPosition, fVelocityMovimentShipTouch);
        }
    }

    private void Shot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Shot_Coroutine());
        }
    }

    public void AnimationStart()
    {
        StartCoroutine(AnimationStart_Coroutine());
    }

    public void RemoveCountShot()
    {
        if (iAuxCountShot > 0)
        {
            iAuxCountShot--;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Wall"))
            OnCollision();

        if (collision.gameObject.tag.Equals("Enemy"))
            OnCollision(collision.gameObject);

        if (collision.gameObject.GetComponent<LaserBoss>())
        {
            collision.gameObject.SetActive(false);
            OnCollision();
        }
    }

    private void DetectCollision()
    {
        var allEnemys = GameObject.FindGameObjectsWithTag("Enemy");
        var enemys = FindObjectsOfType(typeof(EnemyControl)) as EnemyControl[];
        var enemysAntiAir = FindObjectsOfType(typeof(EnemyAntiAir)) as EnemyAntiAir[];
        var enemysTracker = FindObjectsOfType(typeof(EnemyTracker)) as EnemyTracker[];
        var enemysCargoShip = FindObjectsOfType(typeof(EnemyCargoShip)) as EnemyCargoShip[];
        var plasmaBombs = FindObjectsOfType(typeof(PlasmaBomb)) as PlasmaBomb[];
        var partsScene = FindObjectsOfType(typeof(PartScene)) as PartScene[];
        //PowerUps
        var powerUpsIncrementVelocityShotPlayer = FindObjectsOfType(typeof(PUIncrementVelocityShotPlayer)) as PUIncrementVelocityShotPlayer[];
        var powerUpsTripleShot = FindObjectsOfType(typeof(PUTripleShot)) as PUTripleShot[];

        #region Enemys

        foreach (var enemy in enemys)
        {
            var distance = Vector3.Distance(enemy.gameObject.transform.position, transform.position);

            if (distance < 1.01f)
            {
                OnCollision(enemy.gameObject);
            }
        }

        foreach (var enemy in enemysAntiAir)
        {
            var distance = Vector3.Distance(enemy.gameObject.transform.position, transform.position);

            if (distance < 1.01f)
            {
                OnCollision(enemy.gameObject);
            }
        }

        foreach (var enemy in enemysTracker)
        {
            var distance = Vector3.Distance(enemy.gameObject.transform.position, transform.position);

            if (distance < 1.01f)
            {
                OnCollision(enemy.gameObject);
            }
        }

        foreach (var enemy in enemysCargoShip)
        {
            var distance = Vector3.Distance(enemy.gameObject.transform.position, transform.position);

            if (distance < 1.01f)
            {
                OnCollision(enemy.gameObject);
            }
        }

        foreach (var bomb in plasmaBombs)
        {
            var distance = Vector3.Distance(bomb.gameObject.transform.position, transform.position);

            if(((bomb.InExplosion && distance < 1.5f) || distance < 1.01f))
            {
                print("bFieldForceIsActived:: " + bFieldForceIsActived);
                OnCollision(bomb.gameObject, bFieldForceIsActived);
            }
        }

        #endregion

        #region Detectores

        foreach (PartScene part in partsScene)
        {
            var distance = Vector3.Distance(part.gameObject.transform.position, transform.position);
            
            if (distance < 10f && part.PartCurrent == PartScene.Part.Part1)
            {
                bAuxCanZigZag = true;
            }
            else if(distance < 10f && part.PartCurrent == PartScene.Part.Part2)
            {
                bAuxCanZigZag = true;
                bAuxCanShot = true;
            }
            else if (distance < 20 && part.PartCurrent == PartScene.Part.Part3)
            {
                GameManager.Instance.bAuxCanCreateEnemys = false;
                GameManager.Instance.CanCreateEnemys = false;
                print(GameManager.Instance.bAuxCanCreateEnemys);
                print(GameManager.Instance.CanCreateEnemys);
            }

            if (distance < 15f && part.PartCurrent == PartScene.Part.Part4 && bCreateEnemyTrackerNow)
            {
                bCreateEnemyTrackerNow = false;
                GameManager.Instance.CreateEnemysTracker();
            }
        }

        #endregion

        if (bCollisionNow)
        {
            if (enemys.Length < 1 && enemysCargoShip.Length < 1)
            {
                gameObject.GetComponent<BoxCollider2D>().enabled = true;
                transform.DOMove(Camera.main.ScreenToWorldPoint(new Vector3((Screen.width / 10), (Screen.height / 2), 20)), 1f)
                    .OnComplete(() =>
                    {
                        StartCoroutine(OnCollision_Coroutine());
                    });
            }
            else
            {
                for (int i = 0; i < enemys.Length; i++)
                    enemys[i].fVelocityMovimentEnemy += 1;

                for (int i = 0; i < enemysCargoShip.Length; i++)
                    enemysCargoShip[i].fVelocityMovimentEnemy += 1;

                for (int i = 0; i < enemysCargoShip.Length; i++)
                    enemysCargoShip[i].fVelocityMovimentEnemy += 1;

                for (int i = 0; i < enemysAntiAir.Length; i++)
                {
                    if (enemysAntiAir[i].CanMoviment && enemysAntiAir[i].MovimentLeft)
                        enemysAntiAir[i].fVelocityEnemy += 1;
                }
            }
        }
    }

    public void OnCollision(GameObject enemy = null, bool fieldForceIsActived = false)
    {
        tFxLoadingChargeShot.SetActive(false);

        if (enemy != null)
            Destroy(enemy.gameObject);

        if (fieldForceIsActived)
            return;

        DOTween.KillAll();
        gameObject.SetActive(false);
        bEnemyAirCanShot = false;
        transform.parent.GetComponent<MoveGameObject>().bCanMovement = false;
        CollisionsAvailableForceField = 0;
        GameManager.Instance.CanControlPlayer = false;
        GameManager.Instance.CanCreateEnemys = false;
        Instantiate(goParticleDestroyEnemy, transform.position, Quaternion.identity);
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3((-200), (Screen.height / 2), 20));
        bCollisionNow = true;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.SetActive(true);
        Life--;
    }

    public void Dead()
    {
        var shotsPlayer = FindObjectsOfType(typeof(ShotPlayer)) as ShotPlayer[];

        for (int i = 0; i < shotsPlayer.Length; i++)
            Destroy(shotsPlayer[i]);

        Destroy(gameObject);
        GameManager.Instance.GameOver();
    }

    private void ChargeShot()
    {
        if (Input.GetButton("Fire1") && CanChargeShotMore)
        {
            fTimerChargeShot += Time.deltaTime;

            if(fTimerChargeShot >= fTimeFillMin)
            {
                tFxLoadingChargeShot.SetActive(true);
                bShotAutomatic = false;
            }

            if (fTimerChargeShot >= fTimeFillMax)
                bCanChargeShotNow = true;
        }

        if (Input.GetButtonUp("Fire1") && bCanChargeShotNow && CanChargeShotMore)
        {
            fTimerChargeShot = 0;
            bCanChargeShotNow = false;
            tFxLoadingChargeShot.SetActive(false);
            Instantiate(tPrefabChargeShot, tWeaponShipCenter.position, Quaternion.Euler(0, 90, 0));
            bShotAutomatic = true;
        }
        else if(Input.GetButtonUp("Fire1") && fTimerChargeShot != 2f)
        {
            bShotAutomatic = true;
            fTimerChargeShot = 0;
            tFxLoadingChargeShot.SetActive(false);
        }
    }

    #region Coroutines

    private IEnumerator AnimationStart_Coroutine()
    {
        asInputShip.Play();
        transform.DOMoveX(Camera.main.ScreenToWorldPoint(new Vector3((Screen.width - 100), (Screen.height / 2), 20)).x, (asInputShip.clip.length / 2));

        yield return new WaitForSeconds((asInputShip.clip.length / 2));

        transform.DOMove(Camera.main.ScreenToWorldPoint(new Vector3((Screen.width / 20), (Screen.height / 2), 20)), (asInputShip.clip.length / 2)).OnComplete(() => 
        {
            asInputShip.Stop();
            GameManager.Instance.CanCreateEnemys = true;
            GameManager.Instance.CanControlPlayer = true;
            GameManager.Instance.asMusicGameplay.Play();
            transform.parent.GetComponent<MoveGameObject>().bCanMovement = true;
        });
    }

    private IEnumerator OnCollision_Coroutine()
    {
        for (int i = 0; i < 3; i++)
        {
            gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
            yield return new WaitForSeconds(.3f);
            gameObject.GetComponentInChildren<MeshRenderer>().enabled = true;
        }

        GameManager.Instance.CanControlPlayer = true;

        if (GameManager.Instance.bAuxCanCreateEnemys)
            GameManager.Instance.CanCreateEnemys = true;

        bCollisionNow = false;
        bEnemyAirCanShot = true;
        transform.parent.GetComponent<MoveGameObject>().bCanMovement = true;
    }

    private IEnumerator Shot_Coroutine()
    {
        if (bCanShot)
        {
            if (TripleShot)
            {
                var shotDiagonalUp = Instantiate(tPrefabShot, tWeaponShipUp.position, Quaternion.Euler(315, 90, 0));
                var shotCenter = Instantiate(tPrefabShot, tWeaponShipCenter.position, Quaternion.Euler(0, 90, 0));
                var shotDiagonalDown = Instantiate(tPrefabShot, tWeaponShipDown.position, Quaternion.Euler(45, 90, 0));
                shotCenter.GetComponent<ShotPlayer>().fVelocityShot = VelocityShotPlayer;
                shotDiagonalUp.GetComponent<ShotPlayer>().fVelocityShot = VelocityShotPlayer;
                shotDiagonalDown.GetComponent<ShotPlayer>().fVelocityShot = VelocityShotPlayer;

            }
            else
            {
                var shot = Instantiate(tPrefabShot, tWeaponShipCenter.position, Quaternion.Euler(0, 90, 0));
                shot.GetComponent<ShotPlayer>().fVelocityShot = VelocityShotPlayer;
            }

            iAuxCountShot++;
        }

        bCanShot = false;
        yield return new WaitForSeconds(fTimeReloadShot);

        if (iAuxCountShot < iCountShot && !bShotAutomatic)
            bCanShot = true;
        else
            bCanShot = true;

        StopAllCoroutines();
    }
    
    #endregion
}

public class FieldForce : MonoBehaviour
{
    private void Update()
    {
        CollisionFieldForce();
    }

    private void CollisionFieldForce()
    {
        var allEnemys = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in allEnemys)
        {
            var distance = Vector3.Distance(enemy.gameObject.transform.position, transform.position);

            if (PlayerControl.Instance.CollisionsAvailableForceField != 0 && distance < 2f)
            {
                Destroy(enemy);
                Instantiate(PlayerControl.Instance.goParticleDestroyEnemy, transform.position, Quaternion.identity);
                PlayerControl.Instance.CollisionsAvailableForceField--;
            }
        }
    }
}