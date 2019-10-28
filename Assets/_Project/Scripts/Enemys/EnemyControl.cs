using System.Collections;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    public float fVelocityMovimentEnemy;
    [SerializeField] private GameObject goParticleDestroyEnemy;
    [SerializeField] private Transform tPrefabShot;
    [SerializeField] private float fDistanceZigZag;
    public bool bZigZag;
    public bool bCanShoot;
    [SerializeField] private float fTimeReloadShot;
    private float fLimitDistanceMin;
    private float fLimitDistanceMax;
    private bool bMoveUp;
    private bool bMoveDown;
    [SerializeField] private int iDamageCount = 10;

    private void Start()
    {
        fLimitDistanceMax = transform.position.y + fDistanceZigZag;
        fLimitDistanceMin = transform.position.y - fDistanceZigZag;
        bMoveUp = true;
    }

    private void Update()
    {
        if (bZigZag)
            StartZigZag();
        else
            transform.Translate(Vector3.left * fVelocityMovimentEnemy * Time.deltaTime);

        OffScreen.Instance.DestroyGameObjetWidth(gameObject);
    }

    public void Dead(int damageValueShotCurrent)
    {
        iDamageCount -= damageValueShotCurrent;

        if(iDamageCount <= 0)
        {
            GameManager.Instance.AddPoints();
            Instantiate(goParticleDestroyEnemy, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void OnBecameVisible()
    {
        if (bCanShoot)
            StartCoroutine(Shooting_Coroutine());
    }

    private void StartZigZag()
    {
        transform.Translate(Vector3.left * fVelocityMovimentEnemy * Time.deltaTime);

        if (bMoveUp)
            transform.Translate(Vector3.up * fVelocityMovimentEnemy * Time.deltaTime);

        if (bMoveDown)
            transform.Translate(Vector3.down * fVelocityMovimentEnemy * Time.deltaTime);

        if (transform.position.y >= fLimitDistanceMax)
        {
            bMoveUp = false;
            bMoveDown = true;
        }
        
        if (transform.position.y <= (fLimitDistanceMin))
        {
            bMoveUp = true;
            bMoveDown = false;
        }
    }

    private IEnumerator Shooting_Coroutine()
    {
        while (!PlayerControl.bEnemyAirCanShot)
            yield return null;

        Instantiate(tPrefabShot, transform.position, Quaternion.Euler(0, 90, 0));
        yield return new WaitForSeconds(fTimeReloadShot);
        StartCoroutine(Shooting_Coroutine());
    }
}