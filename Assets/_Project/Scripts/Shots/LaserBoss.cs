using System.Collections;
using UnityEngine;

public class LaserBoss : MonoBehaviour
{
    private void Update()
    {
        var newPosition = Camera.main.WorldToScreenPoint(transform.position);
        var widthLimit = (Screen.width / Screen.width) - 10;
        var heigthLimit = (Screen.height + 10);

        if (newPosition.x < widthLimit || newPosition.y > heigthLimit)
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerControl>())
            StartCoroutine(CollisionPlayer(collision.gameObject.GetComponent<PlayerControl>()));
    }

    private IEnumerator CollisionPlayer(PlayerControl player)
    {
        gameObject.SetActive(false);
        yield return new WaitForFixedUpdate();
        player.OnCollision();
    }
}