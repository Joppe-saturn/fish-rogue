using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [System.Serializable]
    public class EnemieSpawns
    {
        public GameObject enemy;
        public Vector3 enemySpawn;
    }

    [System.Serializable]
    public class ExitPoint
    {
        public Vector3 scale;
        public Vector3 position;
    }

    [SerializeField] private List<EnemieSpawns> spawnEnemies;
    [SerializeField] private ExitPoint exitPointExit;

    public GameObject previousRoom;

    private List<GameObject> enemyTotal = new List<GameObject>();

    private GameObject player;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>().gameObject;

        for(int i = 0; i < spawnEnemies.Count; i++)
        {
            enemyTotal.Add(Instantiate(spawnEnemies[i].enemy, transform.position + spawnEnemies[i].enemySpawn, Quaternion.identity));
        }
    }

    private void Update()
    {
        bool canLeave = true;
        
        for(int i = 0; i < spawnEnemies.Count; i++)
        {
            if (enemyTotal[i] != null)
            {
                canLeave = false;
                break;
            }
        }

        if (canLeave)
        {
            Vector3 exitPos = exitPointExit.position + transform.position - exitPointExit.scale / 2.0f;
            if (player.transform.position.x > exitPos.x && player.transform.position.y > exitPos.y && player.transform.position.z > exitPos.z)
            {
                if (player.transform.position.x < exitPos.x + exitPointExit.scale.x && player.transform.position.y < exitPos.y + exitPointExit.scale.y && player.transform.position.z < exitPos.z + exitPointExit.scale.z)
                {
                    player.transform.position = previousRoom.transform.position;
                }
            }
        }
    }
}
