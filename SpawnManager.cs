using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;

    [SerializeField]
    private GameObject _enemyContainer;

    [SerializeField]
    private GameObject _powerupContainer;

    [SerializeField]
    private GameObject _obstaclesContainer;

    [SerializeField]
    private GameObject _tripleShotPowerupPrefab;

    [SerializeField]
    private GameObject _speedPowerupPrefab;

    private GameObject _asteroidPrefab;

    [SerializeField]
    private bool _stopSpawning = false;
    private bool _spawnPowerups { get; set; } = false;
    private bool _spawnEnemies { get; set; } = false;

    private bool _spawnObstacles { get; set; } = false;

    private PowerupFactory powerupFactory;

    // Start is called before the first frame update
    void Start()
    {
        powerupFactory = PowerupFactory.getInstance();
        _asteroidPrefab = (GameObject)Resources.Load("Prefabs/Enemy/Asteroid", typeof(GameObject));

        StartCoroutine(SpawnEnemies());
        StartCoroutine(SpawnPowerups());
        StartCoroutine(SpawnObstacles());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            if(!_stopSpawning && _spawnEnemies)
            {
                GameObject enemy = Instantiate(_enemyPrefab, new Vector3(Random.Range(-10.0f, 10.0f), 8.0f, 0), Quaternion.identity);
                
                if(enemy != null && _enemyContainer!= null)
                {
                    enemy.transform.parent = _enemyContainer.transform;
                }
            }

            yield return new WaitForSeconds(1.2f);
        }        
    }

    private IEnumerator SpawnPowerups()
    {
        while (true)
        {
            if(!_stopSpawning && _spawnPowerups && powerupFactory != null)
            {
                powerupFactory.makePowerup(SpaceUtility.getRandomPowerupType(), _powerupContainer.transform);
            }

            yield return new WaitForSeconds(5.0f);
        }
    }

    private IEnumerator SpawnObstacles()
    {
        while (true)
        {
            if (!_stopSpawning && _spawnObstacles)
            {
                Instantiate(_asteroidPrefab, _obstaclesContainer.transform);
            }
            yield return new WaitForSeconds(3.5f);
        }
    }

    public void OnGameOver()
    {
        _stopSpawning = true;

        foreach(Transform transform in gameObject.transform)
        {
            Destroy(transform.gameObject);
        }
    }

    public void SpawnPowerups(bool answer)
    {
        _spawnPowerups = answer;
    }

    public void SpawnEnemies(bool answer)
    {
        _spawnEnemies = answer;
    }

    public void SpawObstacles(bool answer)
    {
        _spawnObstacles = false;
    }

    public void StartDefaultSpawningLogic()
    {
        _stopSpawning = false;
        _spawnEnemies = true;
        _spawnObstacles = true;
    }
}
