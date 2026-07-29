using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;

    [Header("Level Generation Settings")]
    public List<GameObject> tilePrefabs; // List of manual tower section prefabs
    public float sectionHeight = 10f;    // Vertical height of each tile section
    public int initialPoolSize = 10;     // How many total sections to keep warm in memory
    public float renderAheadDistance = 40f; // How far above player to spawn chunks
    public float despawnDistance = 20f;     // How far below player to recycle chunks

    // Pooling & Active Trackers
    private List<GameObject> pool = new List<GameObject>();
    private List<GameObject> activeSections = new List<GameObject>();
    private float currentSpawnY = 0f;

    void Start()
    {
        // 2. Initialize the Object Pool disabled off-screen
        InitializePool();

        // 3. Build initial stack around start position
        while (currentSpawnY < renderAheadDistance)
        {
            SpawnNextSection();
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Check if we need to spawn new sections ahead of the player
        if (playerTransform.position.y + renderAheadDistance > currentSpawnY)
        {
            SpawnNextSection();
        }

        // Recycle sections far below the player
        RecycleOldSections();
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            // Pick a random section prefab to pre-warm
            GameObject prefab = tilePrefabs[Random.Range(0, tilePrefabs.Count)];
            GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    private void SpawnNextSection()
    {
        GameObject sectionToSpawn = GetPooledObject();

        if (sectionToSpawn != null)
        {
            sectionToSpawn.transform.position = new Vector3(0, currentSpawnY, 0);
            sectionToSpawn.SetActive(true);

            activeSections.Add(sectionToSpawn);
            currentSpawnY += sectionHeight;
        }
    }

    private GameObject GetPooledObject()
    {
        // Find an inactive object in the pool
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        // Expansion fallback: If player climbs faster than pool size, expand pool smoothly
        GameObject extraPrefab = tilePrefabs[Random.Range(0, tilePrefabs.Count)];
        GameObject newObj = Instantiate(extraPrefab, Vector3.zero, Quaternion.identity, transform);
        newObj.SetActive(false);
        pool.Add(newObj);
        return newObj;
    }

    private void RecycleOldSections()
    {
        for (int i = activeSections.Count - 1; i >= 0; i--)
        {
            GameObject section = activeSections[i];

            // Check if section center is below player despawn threshold
            if (section.transform.position.y < playerTransform.position.y - despawnDistance)
            {
                section.SetActive(false);
                activeSections.RemoveAt(i);
            }
        }
    }
}