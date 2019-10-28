using UnityEngine;

public class PUTripleShot : MonoBehaviour
{
    [SerializeField] private GameObject m_GameObjectChild;

    private void Start()
    {
        m_GameObjectChild.AddComponent<ScriptGameObjectChild3>();
    }
}

public class ScriptGameObjectChild3 : MonoBehaviour
{
    public int iNumbersFieldForces;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerControl>())
        {
            collision.gameObject.GetComponent<PlayerControl>().TripleShot = true;
            Destroy(gameObject);
        }
    }
}