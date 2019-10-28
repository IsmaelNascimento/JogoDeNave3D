using System.Collections;
using UnityEngine;

public class EnemyTracker : MonoBehaviour
{
    [SerializeField] private Transform tPrefabShot;
    [SerializeField] private GameObject goParticleDestroyEnemy;
    [SerializeField] private float fTimeReloadShot;
    [SerializeField] private float fVelocityEnemy;
    [SerializeField] private Transform tCannon;

    [Header("Points Raycast")]
    [SerializeField] private GameObject goPointLeft;
    [SerializeField] private GameObject goPointUp;
    [SerializeField] private GameObject goPointDown;

    [SerializeField] private int iDamageCount = 10;
    private bool bMoveNow;
    private bool bCheckLeft;
    private bool CollisionWall;

    private void Start()
    {
        bMoveNow = true;
        bCheckLeft = true;
        StartCoroutine(Shooting_Coroutine());
    }

    private void Update()
    {
        var player = FindObjectOfType(typeof(PlayerControl)) as PlayerControl;

        if (player != null && bMoveNow)
            transform.position = Vector3.Lerp(transform.position, player.transform.position, fVelocityEnemy * Time.deltaTime);

        if (player != null)
            tCannon.LookAt(FindObjectOfType<PlayerControl>().transform);

        OffScreen.Instance.DestroyGameObjetWidth(gameObject);
    }

    private void FixedUpdate()
    {
        var distanceRaycastLimitX = .4f;
        var distanceRaycastLimitY = 3f;
        var hitLeft = Physics2D.Raycast(goPointLeft.transform.position, Vector2.left, distanceRaycastLimitX);
        var hitUp = Physics2D.Raycast(goPointUp.transform.position, Vector2.up, distanceRaycastLimitY);
        var hitDown = Physics2D.Raycast(goPointDown.transform.position, Vector2.down, distanceRaycastLimitY);

        if (hitLeft.collider != null && hitLeft.collider.CompareTag("Wall") && bCheckLeft)
        {
            bMoveNow = false;
            CollisionWall = true;
        }

        if (hitUp.collider != null && hitUp.collider.CompareTag("Enemy"))
        {
            bMoveNow = false;
        }
        else if(hitUp.collider != null && hitUp.collider.CompareTag("Player") && (hitUp.collider != null && !hitUp.collider.CompareTag("Enemy")))
        {
            bMoveNow = true;
        }
        else if(!CollisionWall)
        {
            bCheckLeft = true;
            bMoveNow = true;
        }

        var player = FindObjectOfType(typeof(PlayerControl)) as PlayerControl;

        if ((player != null && player.transform.position.y > transform.position.y) &&
            (player != null && transform.position.y > player.transform.position.y) &&
            player.transform.position.x >= transform.position.x)
        {
            bMoveNow = true;
        }

        if (hitDown.collider != null && hitDown.collider.CompareTag("Enemy"))
            bMoveNow = false;
        else if (hitDown.collider != null && hitDown.collider.CompareTag("Player") && (hitDown.collider != null && !hitDown.collider.CompareTag("Enemy")))
            bMoveNow = true;

        Debug.DrawRay(goPointLeft.transform.position, Vector3.left * distanceRaycastLimitX, Color.green);
        Debug.DrawRay(goPointUp.transform.position, Vector3.up * distanceRaycastLimitY, Color.green);
        Debug.DrawRay(goPointDown.transform.position, Vector3.down * distanceRaycastLimitY, Color.green);
    }

    public void Dead(int damageValueShotCurrent)
    {
        iDamageCount -= damageValueShotCurrent;

        if (iDamageCount <= 0)
        {
            GameManager.Instance.AddPoints();
            Instantiate(goParticleDestroyEnemy, transform.position, Quaternion.Euler(90, 0, 0));
            Destroy(gameObject);
        }
    }

    private IEnumerator Shooting_Coroutine()
    {
        for (int i = 0; i < 10; i++)
        {
            Instantiate(tPrefabShot, tCannon.transform.position, tCannon.rotation);
            yield return new WaitForSeconds(fTimeReloadShot);
        }
    }
}