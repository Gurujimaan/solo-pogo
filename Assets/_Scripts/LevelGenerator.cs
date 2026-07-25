using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{ 
    [Header("References")]
    public GameObject playerPrefab;

    [Header("Level Generation Settings")]
    public int levelWidth = 10;
    public int levelHeight = 10;
    public List<GameObject> tilePrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
