using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Enemy m_EnemyPrefab;
    private float m_LastSpawnTime;
    public Transform m_SpawnPosTransform;

    private float m_TimeBetSpawn;
    public float m_TimeBetSpawnMax = 4f;
    public float m_TimeBetSpawnMin = 1f;


    private void Start()
    {
        FindObjectOfType<Player>().GetComponent<LivingEntity>().OnDeath += () => enabled = false;

        m_TimeBetSpawn = Random.Range(m_TimeBetSpawnMin, m_TimeBetSpawnMax);
    }

    private void Update()
    {
        if (Time.time >= m_LastSpawnTime + m_TimeBetSpawn)
        {
            m_LastSpawnTime = Time.time;
            m_TimeBetSpawn = Random.Range(m_TimeBetSpawnMin, m_TimeBetSpawnMax);

            var enemyInstance = Instantiate(m_EnemyPrefab, m_SpawnPosTransform.position, m_SpawnPosTransform.rotation);

            enemyInstance.Setup(3f, 50, 100, Color.green);
        }
    }
}