using System.Collections;
using UnityEngine;
using DG.Tweening;

public class CannonMain : MonoBehaviour
{
    private static CannonMain m_Instance;
    public static CannonMain Instance
    {
        get
        {
            if(m_Instance == null)
            {
                m_Instance = FindObjectOfType(typeof(CannonMain)) as CannonMain;
            }

            return m_Instance;
        }
    }

    [Header("Setup")]
    [SerializeField] private float fTimeLoadingLaser = 4;
    [SerializeField] private float fTimeDurationLaser = 1;
    [SerializeField] private GameObject goFxLoadingLaser;
    [SerializeField] private GameObject goLaserShot;
    [SerializeField] private Transform tBodyUp;
    [SerializeField] private Transform tBodyDown;
    [SerializeField] public bool bActiveCannonMain = false;


    public void Update()
    {
        if (!transform.parent.GetComponentInChildren<EnemyAntiAir>())
            bActiveCannonMain = true;

        WeaponCannonMain();
    }

    private void WeaponCannonMain()
    {
        if (bActiveCannonMain)
        {
            tBodyUp.DOMoveY(tBodyUp.TransformPoint (new Vector3(0, - 0.28f, 0)).y, 1f);
            tBodyDown.DOMoveY(tBodyDown.TransformPoint(new Vector3(0, 0.28f, 0)).y, 1f).OnComplete(() => 
            {
                if(gameObject.activeInHierarchy)
                    StartCoroutine(LaserShot_Coroutine());
            });
        }
    }

    public IEnumerator LaserShot_Coroutine()
    {
        if (GameManager.Instance.CanControlPlayer)
        {
            goFxLoadingLaser.SetActive(true);
            yield return new WaitForSeconds(fTimeLoadingLaser);
            goFxLoadingLaser.SetActive(false);
            goLaserShot.SetActive(true);
            yield return new WaitForSeconds(fTimeDurationLaser);
            goLaserShot.SetActive(false);
            StopAllCoroutines();
            StartCoroutine(LaserShot_Coroutine());
        }
    }
}