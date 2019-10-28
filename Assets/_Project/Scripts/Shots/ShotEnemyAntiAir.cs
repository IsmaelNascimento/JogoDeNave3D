using UnityEngine;

public class ShotEnemyAntiAir : MonoBehaviour
{
    [SerializeField] private float fVelocityShot;

    private void Update()
    {
        transform.Translate(Vector3.forward * fVelocityShot * Time.deltaTime);
        OffScreen.Instance.DestroyGameObjetWidthAndHeigth(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Wall"))
            Destroy(gameObject);
    }
}