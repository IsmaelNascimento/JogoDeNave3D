using UnityEngine;

public class MoveGameObject : MonoBehaviour
{
    [SerializeField] private float fVelocityMoviment;
    [SerializeField] public bool bCanMovement;

    private void Start()
    {
        bCanMovement = false;
    }

    private void Update()
    {
        if(bCanMovement && !GameManager.Instance.BossNow)
            transform.Translate(Vector3.right * Time.deltaTime * fVelocityMoviment);

        if (transform.position.x >= 303f)
            GameManager.Instance.BossNow = true;
    }
}