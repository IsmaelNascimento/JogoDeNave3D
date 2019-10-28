using UnityEngine;

public class EnemyInverse : MonoBehaviour
{
    [SerializeField] private GameObject goParticleDestroyEnemy;
    [SerializeField] private float fVelocityEnemy;
    [SerializeField] private int iDamageCount = 10;
    private bool bMoveLeft = true;
    private bool bMoveRight = false;

    private void FixedUpdate()
    {
        var distanceRaycastLimitX = .5f;
        var hitLeft = Physics2D.Raycast(transform.position + new Vector3(1, 0, 0), Vector2.left, distanceRaycastLimitX);
        var hitRight = Physics2D.Raycast(transform.position + new Vector3(-1, 0, 0), Vector2.right, distanceRaycastLimitX);

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

        OffScreen.Instance.DestroyGameObjetWidth(gameObject);

        Debug.DrawRay(transform.position + new Vector3(1, 0, 0), Vector3.right * distanceRaycastLimitX, Color.green);
        Debug.DrawRay(transform.position + new Vector3(-1, 0, 0), Vector3.left * distanceRaycastLimitX, Color.green);
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
}