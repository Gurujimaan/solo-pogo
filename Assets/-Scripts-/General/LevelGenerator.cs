using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;

    [Header("Level Generation Settings")]
    public List<GameObject> tilePrefabsTier0; // List of manual tower section prefabs
    public List<GameObject> tilePrefabsTier1; 
    public List<GameObject> tilePrefabsTier2;
    public float currentSpawnY = 10f;
    public int initialPoolSize = 10;     // How many total sections to keep warm in memory
    public float renderAheadDistance = 40f; // How far above player to spawn chunks
    public float despawnDistance = 20f;     // How far below player to recycle chunks

    private float sectionHeight = 10f;    // Vertical height of each tile section
    private List<List<GameObject>> tilePrefabs; // Combined list of all tiers for pooling

    [Header("Background Generation Settings")]
    public List<GameObject> bgPrefabs;
    public int currentTier = 0;
    private int bgTier = 0;

    // Pooling & Active Trackers
    private List<GameObject> pool = new List<GameObject>();
    private List<GameObject> activeSections = new List<GameObject>();

    private List<GameObject> bgPool = new List<GameObject>();
    private List<GameObject> activeBackgrounds = new List<GameObject>();

    void Start()
    {
        tilePrefabs = new List<List<GameObject>>();
        tilePrefabs.Add(tilePrefabsTier0);
        tilePrefabs.Add(tilePrefabsTier1);
        tilePrefabs.Add(tilePrefabsTier2);

        // Initialize pools disabled off-screen
        InitializePool();

        // Build initial stack around start position
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

        // Recycle sections and backgrounds far below the player
        RecycleOldSections();

        if(currentSpawnY > 100 && currentTier == 0)
        {
            currentTier = 1;
            bgTier = 1;
        }
        else if(currentSpawnY > 200 && currentTier == 1)
        {
            currentTier = 2;
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject prefab = tilePrefabs[currentTier][Random.Range(0, tilePrefabs[currentTier].Count)];
            GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
            obj.SetActive(false);
            pool.Add(obj);

            GameObject bgPrefab = bgPrefabs[currentTier];
            GameObject bgObj = Instantiate(bgPrefab, Vector3.zero, Quaternion.identity, transform);
            bgObj.SetActive(false);
            bgPool.Add(bgObj);
        }
    }

    private void SpawnNextSection()
    {
        GameObject sectionToSpawn = GetPooledObject();
        GameObject bgToSpawn = GetPooledBgObject();

        if (sectionToSpawn != null)
        {
            sectionToSpawn.transform.position = new Vector3(0, currentSpawnY, 0);
            sectionToSpawn.SetActive(true);
            activeSections.Add(sectionToSpawn);
        }

        if (bgToSpawn != null)
        {   
            bgToSpawn.transform.position = new Vector3(0, currentSpawnY, 0);
            bgToSpawn.SetActive(true);
            activeBackgrounds.Add(bgToSpawn);
        }

        if(bgTier == 1) bgTier = 2; // Switch to bgTier 2 after the first background is spawned

        currentSpawnY += sectionHeight;
    }

    private GameObject GetPooledObject()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        // Expansion fallback
        GameObject extraPrefab = tilePrefabs[currentTier][Random.Range(0, tilePrefabs[currentTier].Count)];
        GameObject newObj = Instantiate(extraPrefab, Vector3.zero, Quaternion.identity, transform);
        newObj.SetActive(false);
        pool.Add(newObj);
        return newObj;
    }

    private GameObject GetPooledBgObject()
    {
        foreach (GameObject obj in bgPool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        // Expansion fallback using bgTier index
        GameObject bgPrefab = bgPrefabs[bgTier];
        GameObject newBgObj = Instantiate(bgPrefab, Vector3.zero, Quaternion.identity, transform);
        newBgObj.SetActive(false);
        bgPool.Add(newBgObj);
        return newBgObj;
    }

    private void RecycleOldSections()
    {
        float despawnThreshold = playerTransform.position.y - despawnDistance;

        // Recycle tile sections
        for (int i = activeSections.Count - 1; i >= 0; i--)
        {
            GameObject section = activeSections[i];
            if (section.transform.position.y < despawnThreshold)
            {
                section.SetActive(false);
                activeSections.RemoveAt(i);
            }
        }

        // Recycle background sections
        for (int i = activeBackgrounds.Count - 1; i >= 0; i--)
        {
            GameObject bg = activeBackgrounds[i];
            if (bg.transform.position.y < despawnThreshold)
            {
                bg.SetActive(false);
                activeBackgrounds.RemoveAt(i);
            }
        }
    }
}