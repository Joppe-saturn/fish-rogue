using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class WaterLoot : MonoBehaviour
{
    [System.Serializable]
    public class Fish
    {
        public int weight;
        public GameObject room;
    }

    [System.Serializable]
    public class Enemy
    {
        public int weight;
        public GameObject enemies;
    }

    [System.Serializable]
    public class Tresure
    {
        public int weight;
        public Item loot;
    }

    [System.Serializable]
    public class Loot
    {
        public int fishWeight;
        public List<Fish> fishList;
        public int enemyWeight;
        public List<Enemy> enemiesList;
        public int tresureWeight;
        public List<Tresure> tresuresList;
    }

    [SerializeField] private Loot loottable;
    [SerializeField] public int chanceToCatch;
    private GameObject roomSpawner;
    private List<GameObject> spawnedRooms = new List<GameObject>();
    [SerializeField] private int chanceToPreRoom;
    private GameObject player;
    [SerializeField] private float algieTime;
    private InventoryManager inventoryManager;

    private void Start()
    {
        roomSpawner = FindFirstObjectByType<InventoryManager>().transform.parent.transform.GetChild(2).gameObject;
        player = FindFirstObjectByType<PlayerMovement>().gameObject;
        inventoryManager = FindFirstObjectByType<InventoryManager>();
    }

    public void GetFish(Transform bait)
    {
        StartCoroutine(inventoryManager.SpreadAlgie(algieTime));
        int r = Random.Range(0, loottable.fishWeight + loottable.tresureWeight + loottable.enemyWeight);
        
        if (r < loottable.fishWeight)
        {
            SpawnFish();
        }
        else if (r - loottable.fishWeight < loottable.enemyWeight)
        {
            SpawnEnemy(bait);
        } 
        else
        {
            SpawnTresure(bait);
        }

        FishingRot fishing = FindFirstObjectByType<FishingRot>();
        bait.transform.position = FindFirstObjectByType<PlayerMovement>().transform.position;
        StartCoroutine(fishing.RetriefBait());
    }

    private void SpawnFish()
    {
        float weightTotal = 0;

        if (Random.Range(0, chanceToPreRoom) > 2 && spawnedRooms.Count > 0)
        {
            weightTotal = spawnedRooms.Count;

            float r = Random.Range(0f, weightTotal);
            int fishInstance = 0;

            for (int i = 0; i < spawnedRooms.Count; i++)
            {
                if (r < loottable.fishList[i].weight)
                {
                    fishInstance = i;
                    break;
                }
                else
                {
                    r --;
                }
            }

            player.transform.position = spawnedRooms[fishInstance].transform.position;
        } 
        else
        {
            for (int i = 0; i < loottable.fishList.Count; i++)
            {
                weightTotal += loottable.fishList[i].weight;
            }

            float r = Random.Range(0f, weightTotal);
            int fishInstance = 0;

            for (int i = 0; i < loottable.fishList.Count; i++)
            {
                if (r < loottable.fishList[i].weight)
                {
                    fishInstance = i;
                    break;
                }
                else
                {
                    r -= loottable.fishList[i].weight;
                }
            }

            roomSpawner.transform.position += new Vector3(1000, 0, 0);
            GameObject room = Instantiate(loottable.fishList[fishInstance].room, roomSpawner.transform.position, Quaternion.identity);
            player.transform.position = room.transform.position;
            room.GetComponent<Room>().previousRoom = gameObject;
            spawnedRooms.Add(room);
        }
    }

    private void SpawnEnemy(Transform bait)
    {
        int weightTotal = 0;

        for (int i = 0; i < loottable.enemiesList.Count; i++)
        {
            weightTotal += loottable.enemiesList[i].weight;
        }

        int r = Random.Range(0, weightTotal);
        int enemyInstance = 0;

        for (int i = 0; i < loottable.enemiesList.Count; i++)
        {
            if(r < loottable.enemiesList[i].weight)
            {
                enemyInstance = i;
                break;
            } else
            {
                r -= loottable.enemiesList[i].weight;
            }
        }

        Instantiate(loottable.enemiesList[enemyInstance].enemies, bait.position, Quaternion.identity);
    }

    private void SpawnTresure(Transform bait)
    {
        int weightTotal = 0;

        for (int i = 0; i < loottable.tresuresList.Count; i++)
        {
            weightTotal += loottable.tresuresList[i].weight;
        }

        int r = Random.Range(0, weightTotal);
        int tresureInstance = 0;

        for (int i = 0; i < loottable.tresuresList.Count; i++)
        {
            if (r < loottable.tresuresList[i].weight)
            {
                tresureInstance = i;
                break;
            }
            else
            {
                r -= loottable.tresuresList[i].weight;
            }
        }

        Instantiate(loottable.tresuresList[tresureInstance].loot.item, bait.position, Quaternion.identity);
    }
}
