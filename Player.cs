using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private static int _playersCount; 
    //---POWERUP COUNTDOWN VARIABLES---//
    private GlobalVariables.PowerupCountDown _powerupCountDown;
    private GameObject _damagePrefab;

    private Animator _animator;
    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private GameObject _tripleShotPrefab;

    [SerializeField]
    private int _enemyKilled { get; set; }

    private GameObject _fireAreaObject;

    private UIManager _canvas;

    private bool AmIShooting = false;

    

    private Vector2 moveDirectionVec;

    private SpawnManager _spawnManager;
    private Game_Manager _gameManager;

    [SerializeField]
    private AudioClip _laserSoundClip;
    private AudioSource _audioSource;

    //---PROPERTIES OF SHIP---///
    private GlobalVariables.MainWeaponary mainWeaponLevel = 0;
    private float _fireRate = 0.33f;
    private float _speed = 7.0f;
    private int _lives = 4;
    [SerializeField]
    private float _canFire = -1.0f;

    private FireDamageArea _fireDamageArea;

    private List<GameObject> _damagePoints;
    private List<GameObject> _damagaSource;

    struct FireDamageArea
    {
        public bool left; //false
        public bool right; //true

        public bool GetSide()
        {
            bool answer = false;

            if (left) { right = true; answer = true; }
            else if (right) { left = true; answer = false; }
            else { left = true; }

            if(left && right)
            {
                left = false;
                right = false;
            }

            return answer;
        }
    }

    private void Awake()
    {
        _damagePrefab = (GameObject)Resources.Load("Prefabs/Damage/Fire_Damage", typeof(GameObject));
        SetUpPlayersControl(++_playersCount);
    }

    // Start is called before the first frame update
    void Start()
    {
        var canvasGM = GameObject.Find("Canvas");
        if (canvasGM != null)
        {
            _canvas = canvasGM.GetComponent<UIManager>();
        }
        else Debug.LogWarning("Player: _canvas is null");

        var gameManagerGM = GameObject.Find("GameManager").GetComponent<Game_Manager>();
        if(gameManagerGM != null)
        {
            _gameManager = gameManagerGM.GetComponent<Game_Manager>();
        }
        else Debug.LogWarning("Player: _gameManager is null");

        var spawManagerGM = GameObject.Find("Spawn_Manager");
        if(spawManagerGM != null)
        {
            _spawnManager = spawManagerGM.GetComponent<SpawnManager>();
        }
        else Debug.LogWarning("Player: _spawnManager is null");

        _audioSource = GetComponent<AudioSource>();
        if(_audioSource == null)
        {
            Debug.LogWarning("Player: _audioSource is null");
        }
        else { _audioSource.clip = _laserSoundClip; }

        _animator = GetComponent<Animator>();
        if(_animator == null)
        {
            Debug.LogWarning("Player: _animator is null");
        }
                
        _damagePoints = new List<GameObject>();
        _damagaSource = new List<GameObject>();
        _fireDamageArea = new FireDamageArea();
        _powerupCountDown = new GlobalVariables.PowerupCountDown(0.0f);

        foreach (Transform child in transform)
        {
            if (child.name == "FireArea")
            {
                _fireAreaObject = child.gameObject;
                break;
            }
        }

        //SetUpPlayersControl(++_playersCount);

        StartCoroutine(CountKillsBeforePowerups());
        StartCoroutine(PowerUpCountTime());
        StartCoroutine(ClearDamageSources());
    }

    private IEnumerator ClearDamageSources()
    {
        while (true)
        {
            _damagaSource.Clear();
            yield return new WaitForSeconds(2.0f);
        }
    }

    private void SetUpPlayersControl(int playerNumber)
    {
        GetComponent<PlayerInput>().SwitchCurrentActionMap("Player" + playerNumber);

        //--Move--
        GetComponent<PlayerInput>().currentActionMap.FindAction("Move").performed += Player_performed_Move;
        GetComponent<PlayerInput>().currentActionMap.FindAction("Move").canceled += Player_canceled_Move;
        //--Shoot--
        GetComponent<PlayerInput>().currentActionMap.FindAction("Shoot").started += Player_started_Shoot;
        GetComponent<PlayerInput>().currentActionMap.FindAction("Shoot").canceled += Player_canceled_Shoot;
    }

    private IEnumerator CountKillsBeforePowerups()
    {
        while (true)
        {
            if (_enemyKilled > 3)
            {
                if (_spawnManager == null) continue;
                _spawnManager.SpawnPowerups(true);
                yield break;
            }
            yield return new WaitForSeconds(.5f);
        }
    }

    private IEnumerator PowerUpCountTime()
    {
        float decrement = 0.5f;
        while (true)
        {
            if (_powerupCountDown._tripleLaser > 0)
            {
                _powerupCountDown._tripleLaser -= decrement;
            }
            else mainWeaponLevel = GlobalVariables.MainWeaponary.Laser;

            if (_speed > GlobalVariables.playerSpeed)
            {
                _powerupCountDown._speed -= decrement;
            }
            else _speed = GlobalVariables.playerSpeed;

            if (_powerupCountDown._shield > 0)
            {
                _powerupCountDown._shield -= decrement;
            }
            else
            {
                _powerupCountDown._shield--;
                foreach (Transform transform in transform)
                {
                    if (transform.CompareTag("PlayersShield"))
                    {
                        Destroy(transform.gameObject);
                    }
                }

            }
            yield return new WaitForSeconds(decrement);
        }
    }

    private void Player_canceled_Shoot(InputAction.CallbackContext obj)   
    {
        AmIShooting = false;
    }

    private void Player_started_Shoot(InputAction.CallbackContext context)
    {
        AmIShooting = true;
    }

    private void Player_canceled_Move(InputAction.CallbackContext context)
    {
        moveDirectionVec.Set(0, 0);

        if (_animator == null) return;

        if (!_animator.GetBool("IsLeftIdle"))
        {
            _animator.SetBool("IsLeftIdle", true);
        }
        if (!_animator.GetBool("IsRightIdle"))
        {
            _animator.SetBool("IsRightIdle", true);
        }
            
        _animator.SetBool("IsMovingLeft", false);
        _animator.SetBool("IsMovingRight", false);
    }

    private void Player_performed_Move(InputAction.CallbackContext context)
    {
        moveDirectionVec = context.ReadValue<Vector2>();

        if (_animator == null) return;

        if(moveDirectionVec.x < 0)
        {
            _animator.SetBool("IsLeftIdle", false);
            _animator.SetBool("IsMovingLeft", true);
        }

        if (moveDirectionVec.x > 0)
        {
            _animator.SetBool("IsRightIdle", false);
            _animator.SetBool("IsMovingRight", true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.timeScale != 0)
        {
            CalcualteMovement();
            Shoot();
        }
    }

    void CalcualteMovement()
    {
        transform.Translate(new Vector3(moveDirectionVec.x, moveDirectionVec.y, 0).normalized * _speed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x < -11.0f)
        {
            transform.position = new Vector3(11.0f, transform.position.y, 0);
        }
        else if (transform.position.x > 11.0f)
        {
            transform.position = new Vector3(-11.0f, transform.position.y, 0);
        }
    }

    private void Shoot()
    {
        if ( (Time.time <= _canFire) || AmIShooting == false) return;

        _canFire = Time.time + _fireRate;

        if(mainWeaponLevel == GlobalVariables.MainWeaponary.TripleLaser)
        {
            GameObject tripleLaser = Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);

            foreach(Transform tr in tripleLaser.transform)
            {
                tr.GetComponent<Laser>().SetHost(gameObject);
            }
        }
        else
        {
           GameObject laser = Instantiate(_laserPrefab, transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity);
           laser.GetComponent<Laser>().SetHost(gameObject);
        }

        _audioSource.Play();       
    }

    public void Damage(int points, GameObject damageDealer)
    {
        if (_damagaSource.Contains(damageDealer))
        {
            return;
        }
        else { _damagaSource.Add(damageDealer); }

        _lives -= points;

        _canvas.UpdateLives(_lives);

        if (_lives <= 0)
        {
            if(Game_Manager.GetGameplayMode().Equals(GlobalVariables.GameplayMode.SinglePlayer)
                || ( Game_Manager.GetGameplayMode().Equals(GlobalVariables.GameplayMode.MultiPlayers) && _playersCount < 1 )) 
            {
                if (_spawnManager != null) _spawnManager.OnGameOver();
                if (_gameManager != null) _gameManager.GameOver();
                if (_canvas != null) _canvas.UIGameOver();
            }

            _playersCount--;
            Destroy(gameObject);
        }

        Bounds size = _fireAreaObject.GetComponent<BoxCollider2D>().bounds;

        for (var i = 0; i < points; i++)
        {
            Vector3 newPos = _fireDamageArea.GetSide() ? new Vector3(UnityEngine.Random.Range(size.center.x, size.max.x),
                                                                     UnityEngine.Random.Range(size.min.y, size.max.y), 0)
                                                        : new Vector3(UnityEngine.Random.Range(size.min.x, size.center.x),
                                                                      UnityEngine.Random.Range(size.min.y, size.max.y), 0);

            _damagePoints.Add(Instantiate(_damagePrefab, newPos, Quaternion.identity, transform));
        }
    }

    public void IncrementEnemyKilled()
    {
        _enemyKilled++;
    }
 
    private void OnTriggerEnter2D(Collider2D collision)
    {
       switch (collision.tag)
       {
            case "Enemy":
            case "Laser":
                {
                    Laser laser = collision.gameObject.GetComponent<Laser>();

                    if (laser != null && laser.GetHost() != null
                        && laser.GetHost().CompareTag("Player"))
                    {
                        break;
                    }

                    Damage(1, collision.gameObject);
                }; break;
            case "Asteroid":
                {
                    Damage(2, collision.gameObject);
                };break;
            case "Powerup":
                {
                    UpgradeShip(collision.GetComponent<Powerup>());
                }; break;
       }
    }

    private void UpgradeShip(IUpgrading upgrade)
    {
        if(upgrade.mainWeraponUpgradeLevel >= mainWeaponLevel)
        {
            mainWeaponLevel = upgrade.mainWeraponUpgradeLevel;
            _powerupCountDown._tripleLaser += upgrade.durationInSeconds;
        }

        if(upgrade.speed > 0)
        {
            _speed += upgrade.speed;

            if(_speed > 10.0f)
            {
                _speed = 10.0f;
            }
            _powerupCountDown._speed += upgrade.durationInSeconds;
        }

        if (upgrade.spawnShield)
        {
            if(_powerupCountDown._shield < 0)
            {
                GameObject shieldPrefab = (GameObject)Resources.Load("Prefabs/Shield/Shield", typeof(GameObject));
                GameObject blueShield = Instantiate(shieldPrefab, transform.position, Quaternion.identity);
                _powerupCountDown._shield = 0;
                blueShield.transform.parent = transform;
            }

            _powerupCountDown._shield += upgrade.durationInSeconds;
        }
    }

    private void OnDestroy()
    {
        _playersCount = 0;
    }
}
