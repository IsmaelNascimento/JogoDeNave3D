using UnityEngine;

public class EnemyCargoShip : MonoBehaviour
{
    public float fVelocityMovimentEnemy;
    [SerializeField] private GameObject goParticleDestroyEnemy;
    [SerializeField] private GameObject goPowerUp;
    [SerializeField] private int iDamageCount = 10;

    private void Update()
    {
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
            Instantiate(goPowerUp, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}