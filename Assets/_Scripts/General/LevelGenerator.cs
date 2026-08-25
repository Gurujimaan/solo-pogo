using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;

    [Header("Level Generation Settings")]
    public List<GameObject> tilePrefabs; // List of manual tower section prefabs
    public float currentSpawnY = 10f;
    public float sectionHeight = 10f;    // Vertical height of each tile section
    public int initialPoolSize = 10;     // How many total sections to keep warm in memory
    public float renderAheadDistance = 40f; // How far above player to spawn chunks
    public float despawnDistance = 20f;     // How far below player to recycle chunks

    [Header("Background Generation Settings")]
    public List<GameObject> bgPrefabs;
    public int bgTier = 0;

    // Pooling & Active Trackers
    private List<GameObject> pool = new List<GameObject>();
    private List<GameObject> activeSections = new List<GameObject>();

    private List<GameObject> bgPool = new List<GameObject>();
    private List<GameObject> activeBackgrounds = new List<GameObject>();

    void Start()
    {
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
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject prefab = tilePrefabs[Random.Range(0, tilePrefabs.Count)];
            GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
            obj.SetActive(false);
            pool.Add(obj);

            GameObject bgPrefab = bgPrefabs[bgTier];
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
            Debug.Log("nope");
            bgToSpawn.transform.position = new Vector3(0, currentSpawnY, 0);
            bgToSpawn.SetActive(true);
            activeBackgrounds.Add(bgToSpawn);
        }

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
        GameObject extraPrefab = tilePrefabs[Random.Range(0, tilePrefabs.Count)];
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