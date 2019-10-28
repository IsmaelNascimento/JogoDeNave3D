using UnityEngine;

public class PUIncrementVelocityShotPlayer : MonoBehaviour
{
    public float fIncrementVelocityShotPlayer;
    [SerializeField] private GameObject m_GameObjectChild;

    private void Start()
    {
        m_GameObjectChild.AddComponent<ScriptGameObjectChild2>();
    }
}

public class ScriptGameObjectChild2 : MonoBehaviour
{
    public int iNumbersFieldForces;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerControl>())
        {
            var puIncrementVelocityShotPlayer = GetComponentInParent<PUIncrementVelocityShotPlayer>();
            collision.gameObject.GetComponent<PlayerControl>().VelocityShotPlayer += puIncrementVelocityShotPlayer.fIncrementVelocityShotPlayer;
            Destroy(gameObject);
        }
    }
}