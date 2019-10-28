using UnityEngine;

public class ShotEnemy : MonoBehaviour
{
    [SerializeField] private float fVelocityShot;

    private void Update()
    {
        transform.Translate(Vector3.back * fVelocityShot * Time.deltaTime); // Direction Player
        OffScreen.Instance.DestroyGameObjetWidthAndHeigth(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Wall"))
            Destroy(gameObject);
    }
}