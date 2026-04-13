using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject[] customerPrefabs;
    private float startDelay = 2f;
    public float spawnInterval = 5f;
    private Vector2 spawnPos = new Vector2(-8, -4);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("SpawnRandomCustomer", startDelay, spawnInterval);
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    void SpawnRandomCustomer()
    {
        int customerIndex = Random.Range(0, customerPrefabs.Length);
        Instantiate(customerPrefabs[customerIndex], spawnPos, customerPrefabs[customerIndex].transform.rotation);
    }
}
