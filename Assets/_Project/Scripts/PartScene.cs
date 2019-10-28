using UnityEngine;

public class PartScene : MonoBehaviour
{
    [SerializeField] private Part m_PartCurrent;

    public Part PartCurrent
    {
        get
        {
            return m_PartCurrent;
        }

        set
        {
            m_PartCurrent = value;
        }
    }

    public enum Part
    {
        Part1,
        Part2,
        Part3,
        Part4,
        CreateEnemyCargoShip,
        CreateEnemyAntiAir
    }
}