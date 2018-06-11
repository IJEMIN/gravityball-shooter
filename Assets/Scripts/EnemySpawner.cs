using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float m_TimeBetSpawnMin = 1f;
    public float m_TimeBetSpawnMax = 4f;

    public Enemy m_EnemyPrefab;
    public Transform m_SpawnPosTransform;

    private float m_TimeBetSpawn;
    private float m_LastSpawnTime;


    void Start()
    {
        FindObjectOfType<Player>().GetComponent<LivingEntity>().OnDeath += () => enabled = false;

        m_TimeBetSpawn = Random.Range(m_TimeBetSpawnMin, m_TimeBetSpawnMax);
    }

    void Update()
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
