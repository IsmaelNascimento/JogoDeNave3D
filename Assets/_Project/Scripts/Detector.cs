using UnityEngine;

public class Detector : MonoBehaviour
{
    [Header("Setup Detector")]
    public int id;
    [SerializeField] private Transform tTarget;
    [SerializeField] private float fDistanceMin = 10f;

    [Space(15)][Header("GameObjects")]
    [SerializeField] private GameObject[] goObjects;

    private void Start()
    {
        if (tTarget == null)
            tTarget = FindObjectOfType<PlayerControl>().transform;

        ActiveOrDisableObjects(false);
    }

    private void Update()
    {
        if (tTarget == null)
            return;

        var distance = Vector3.Distance(tTarget.position, transform.position);

        if (distance < fDistanceMin)
            ActiveOrDisableObjects(true);
    }

    private void ActiveOrDisableObjects(bool actived)
    {
        for (int i = 0; i < goObjects.Length; i++)
        {
            if(actived)
            {
                if(goObjects[i] != null)
                    goObjects[i].SetActive(true);
            }
            else
            {
                if (goObjects[i] != null)
                    goObjects[i].SetActive(false);
            }
        }
    }
}