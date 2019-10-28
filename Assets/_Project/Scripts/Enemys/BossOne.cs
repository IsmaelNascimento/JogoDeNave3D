using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossOne : MonoBehaviour
{
    [SerializeField] private int iDamageCount = 600;
    [SerializeField] private GameObject goParticleDestroyEnemy;
    [SerializeField] private GameObject goModelBoss;

    public void Dead(int damageValueShotCurrent)
    {
        iDamageCount -= damageValueShotCurrent;
        print("ShotBoss Dead");

        if (iDamageCount <= 0)
        {
            GameManager.Instance.AddPoints(100);
            goModelBoss.SetActive(false);
            goParticleDestroyEnemy.SetActive(true);
            var timeDestruction = goParticleDestroyEnemy.GetComponentInChildren<ParticleSystem>().main.duration;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<ShotPlayer>())
        {
            print("ShotBoss");
            Dead(collision.gameObject.GetComponent<ShotPlayer>().iDamageValue);
        }

        if (collision.gameObject.GetComponent<ChargeShot>())
            Dead(collision.gameObject.GetComponent<ChargeShot>().iDamageValue);

        if (collision.gameObject.GetComponent<ChargeShot>() ||
            collision.gameObject.GetComponent<ShotPlayer>())
            print("Shot on Boss");
    }
}
