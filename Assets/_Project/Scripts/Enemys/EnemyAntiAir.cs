using System.Collections;
using UnityEngine;

public class EnemyAntiAir : MonoBehaviour
{
    [SerializeField] private bool bMoviment;
    public bool CanMoviment { get { return bMoviment; } }
    [SerializeField] private Transform tCannon;
    [SerializeField] private Transform tPrefabShot;
    [SerializeField] private GameObject goParticleDestroyEnemy;
    [SerializeField] private float fTimeReloadShot;
    [SerializeField] private float fDistancePlayer = 7;
    [SerializeField] private Transform tPositionInstantiateBullet;
    public float fVelocityEnemy;
    [SerializeField] private bool bMovimentLeft = false;
    public bool MovimentLeft { get { return bMovimentLeft; } }
    [SerializeField] private int iDamageCount = 10;

    private bool bMoveLeft = true;
    private bool bMoveRight = false;

    private void Start()
    {
        StartCoroutine(Shooting_Coroutine());
    }

    private void Update()
    {
        var player = FindObjectOfType(typeof(PlayerControl)) as PlayerControl;

        if (player != null)
            tCannon.LookAt(FindObjectOfType<PlayerControl>().transform);

        if (CanMoviment)
            Moviment();

        OffScreen.Instance.DestroyGameObjetWidth(gameObject);
    }

    private void Moviment()
    {
        if (MovimentLeft)
            transform.Translate(Vector3.left * fVelocityEnemy * Time.deltaTime);
        else
        {
            var distanceRaycastLimitX = 1f;
            var hitLeft = Physics2D.Raycast(transform.position + new Vector3(-1, 0, 0), Vector2.left, distanceRaycastLimitX);
            var hitRight = Physics2D.Raycast(transform.position + new Vector3(1, 0, 0), Vector2.right, distanceRaycastLimitX);

            if (bMoveLeft)
                transform.Translate(Vector3.right * fVelocityEnemy * Time.deltaTime);
            else
                transform.Translate(Vector3.left * fVelocityEnemy * Time.deltaTime);

            if (hitLeft.collider != null && hitLeft.collider.CompareTag("Wall") && bMoveLeft)
            {
                bMoveLeft = false;
                bMoveRight = true;
            }
            else if (hitRight.collider != null && hitRight.collider.CompareTag("Wall") && bMoveRight)
            {
                bMoveLeft = true;
                bMoveRight = false;
            }

            Debug.DrawRay(transform.position + new Vector3(distanceRaycastLimitX, 0, 0), Vector3.right * distanceRaycastLimitX, Color.green);
            Debug.DrawRay(transform.position + new Vector3(-distanceRaycastLimitX, 0, 0), Vector3.left * distanceRaycastLimitX, Color.green);
        }
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

    private IEnumerator Shooting_Coroutine()
    {
        var player = FindObjectOfType(typeof(PlayerControl)) as PlayerControl;
        var distance = 0f;

        if (player != null)
            distance = Vector3.Distance(player.gameObject.transform.position, transform.position);

        if (GameManager.Instance.CanControlPlayer && distance < fDistancePlayer)
            Instantiate(tPrefabShot, tPositionInstantiateBullet.position, tCannon.transform.rotation);

        yield return new WaitForSeconds(fTimeReloadShot);

        StartCoroutine(Shooting_Coroutine());
    }
}