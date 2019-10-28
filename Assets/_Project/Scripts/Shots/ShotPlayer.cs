using UnityEngine;

public class ShotPlayer : MonoBehaviour
{
    public float fVelocityShot;
    public int iDamageValue = 10;

    private void Update ()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * fVelocityShot); // Direction Shot
        OffScreen.Instance.DestroyGameObjetWidth(gameObject, OnDestroy: () => PlayerControl.Instance.RemoveCountShot());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
            PlayerControl.Instance.RemoveCountShot();
        }
        else if (collision.gameObject.CompareTag("Enemy"))
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
            else if (collision.gameObject.GetComponent<PlasmaBomb>())
                collision.gameObject.GetComponent<PlasmaBomb>().Dead();
        }
    }
}