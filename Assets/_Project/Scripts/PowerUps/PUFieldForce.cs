using UnityEngine;

public class PUFieldForce : MonoBehaviour
{
    [SerializeField] private GameObject m_GameObjectChild;
    [SerializeField] private int iNumbersFieldForces = 3;

    private void Awake()
    {
        m_GameObjectChild.AddComponent<ScriptGameObjectChild>();
        m_GameObjectChild.GetComponent<ScriptGameObjectChild>().iNumbersFieldForces = iNumbersFieldForces;
    }
}

public class ScriptGameObjectChild : MonoBehaviour
{
    public int iNumbersFieldForces;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerControl>())
        {
            collision.gameObject.GetComponent<PlayerControl>().CollisionsAvailableForceField += iNumbersFieldForces;
            Destroy(gameObject);
        }
    }
}