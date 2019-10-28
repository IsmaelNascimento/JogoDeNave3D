using UnityEngine;

public class ChargeShot : MonoBehaviour
{
    public float fVelocityShot;
    public int iDamageValue = 20;

    private void Start()
    {
        PlayerControl.Instance.CanChargeShotMore = false;
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * fVelocityShot); // Direction Shot
        OffScreen.Instance.DestroyGameObjetWidth(gameObject, OnDestroy:() => PlayerControl.Instance.CanChargeShotMore = true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            PlayerControl.Instance.RemoveCountShot();
            Destroy(gameObject);

            if (collision.gameObject.GetComponent<EnemyControl>())
                collision.gameObject.GetComponent<EnemyControl>().Dead(iDamageValue);
            else if (collision.gameObject.GetComponent<EnemyAntiAir>())
                collision.gameObject.GetComponent<EnemyAntiAir>().Dead(iDamageValue);
            else if (collision.gameObject.GetComponent<EnemyTracker>())
                collision.gameObject.GetComponent<EnemyTracker>().Dead(iDamageValue);
            else if (collision.gameObject.GetComponent<EnemyCargoShip>())
                collision.gameObject.GetComponent<EnemyCargoShip>().Dead(iDamageValue);
            else if (collision.gameObject.GetComponent<BossOne>())
                collision.gameObject.GetComponent<BossOne>().Dead(iDamageValue);
        }
    }
}