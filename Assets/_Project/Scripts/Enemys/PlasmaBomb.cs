using System.Collections;
using UnityEngine;

public class PlasmaBomb : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private bool bMoveUp = true;
    [SerializeField] private float fVelocityEnemy = 1f;
    [SerializeField] private float fDistanceDetectionForPlayer = 7f;
    [SerializeField] private float fDistanceDetectionForMove = 7f;
    [Tooltip("Tempo da duração da explosao em segundos")] [SerializeField] private float fTimeOfExplosion = 1;
    [Tooltip("Tempo para começar explosao em segundos")] [SerializeField] private float fTimeForExplosion = 2;
    public ParticleSystem psFxExplosion;

    private bool bCanMove = false;
    public bool InExplosion { get; set; }
    private float fTimerForStartExplosion = 0;

    private void Start()
    {
        var mainFxExplosion = psFxExplosion.main;
        mainFxExplosion.duration = fTimeOfExplosion;
    }

    private void Update()
    {
        if (bCanMove)
        {
            if (bMoveUp)
                transform.Translate(Vector3.up * fVelocityEnemy * Time.deltaTime);
            else
                transform.Translate(Vector3.down * fVelocityEnemy * Time.deltaTime);
        }

        DetectPlayer();
        OffScreen.Instance.DestroyGameObjetWidth(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            StartCoroutine(FxExplosion_Coroutine());
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject, 5f);
    }

    private void DetectPlayer()
    {
        if (PlayerControl.Instance != null)
        {
            var distanceForPlayer = Vector3.Distance(PlayerControl.Instance.gameObject.transform.position, transform.position);
            var walls = GameObject.FindGameObjectsWithTag("Wall");

            if (distanceForPlayer < fDistanceDetectionForMove)
                bCanMove = true;

            if (distanceForPlayer < fDistanceDetectionForPlayer)
            {
                fTimerForStartExplosion += Time.deltaTime;

                if (fTimerForStartExplosion >= fTimeForExplosion)
                    StartCoroutine(FxExplosion_Coroutine());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Wall"))
            StartCoroutine(FxExplosion_Coroutine());
    }

    public void Dead()
    {
        StartCoroutine(FxExplosion_Coroutine());
    }

    private IEnumerator FxExplosion_Coroutine()
    {
        GetComponent<MeshRenderer>().enabled = false;
        InExplosion = true;
        psFxExplosion.gameObject.SetActive(true);
        yield return new WaitForSeconds(fTimeOfExplosion);
        InExplosion = false;
        Destroy(gameObject);
    }
}