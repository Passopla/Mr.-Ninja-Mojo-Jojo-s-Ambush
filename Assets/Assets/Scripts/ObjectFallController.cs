using UnityEngine;
using System.Collections;

public class ObjectFallController : MonoBehaviour
{
    [Header("Falling Object Settings")]
    [Tooltip("The narrowest X position where objects can spawn.")]
    public float spawnRangeMinX = -8f;
    [Tooltip("The widest X position where objects can spawn.")]
    public float spawnRangeMaxX = 8f;

    [Header("Missile Settings")]
    public GameObject missilePrefab;
    public float missileSpawnRate = 2f;

    [Header("Life Settings")]
    public GameObject lifePrefab;
    public float lifeSpawnRate = 5f;

    [Header("Cloud Settings")]
    public GameObject cloudPrefab;
    public float cloudSpawnRate = 30f;

    private Coroutine missileCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartSpawning()
    {
        missileCoroutine = StartCoroutine(SpawnObjectRoutine(missilePrefab, missileSpawnRate));
        StartCoroutine(SpawnObjectRoutine(lifePrefab, lifeSpawnRate));
        StartCoroutine(SpawnObjectRoutine(cloudPrefab, cloudSpawnRate));
    }

    public void SetMissileSpawnRate(float newRate)
    {
        Debug.Log("New missile spawn rate set to: " + newRate);
        missileSpawnRate = newRate;

        // Stop the old, outdated missile coroutine
        if (missileCoroutine != null)
        {
            StopCoroutine(missileCoroutine);
        }

        // Start a new missile coroutine with the updated spawn rate
        missileCoroutine = StartCoroutine(SpawnObjectRoutine(missilePrefab, missileSpawnRate));
    }

    IEnumerator SpawnObjectRoutine(GameObject objectToSpawn, float spawnRate)
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnRate);

            Vector3 spawnPosition = new Vector3(Random.Range(spawnRangeMinX, spawnRangeMaxX), 10, 0);
            Instantiate(objectToSpawn, spawnPosition, objectToSpawn.transform.rotation);
        }
    }
}
