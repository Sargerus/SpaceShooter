using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.0f;

    private Animator _anim;
    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        if(_anim == null)
        {
            Debug.LogWarning("Enemy: _anim is null");
        }

        transform.position = new Vector3(Random.Range(-10.0f, 10.0f), 8.0f, 0);

        StartCoroutine(Shoot());
    }

    private IEnumerator Shoot()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            Random.InitState(System.DateTime.Now.Millisecond);
            if (Random.value > 0.5f)
            {
                break;
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                break;
            }
        }

        if(GetComponent<BoxCollider2D>() != null)
        {
            GameObject laserPrefab = (GameObject)Resources.Load("Prefabs/Projectile/Laser", typeof(GameObject));
            Bounds size = GetComponent<SpriteRenderer>().bounds;

            GameObject laser = Instantiate(laserPrefab, transform.position + new Vector3(0, -1.5f, 0), Quaternion.identity, transform);
            laser.GetComponent<Laser>().SetDirectionVector(Vector3.down);
        }
    }

    // Update is called once per frame
    void Update()
    {
        SpaceUtility.CalculateMoving(this.gameObject, Vector3.down, _speed * Time.deltaTime, null, -6.0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
       switch (other.tag)
        {
            case "Player":
            case "Laser":
            case "Asteroid":
            case "PlayersShield":
                {
                    Destroy(GetComponent<BoxCollider2D>());
                    GetComponent<AudioSource>().Play();
                    if (_anim != null) _anim.SetTrigger("OnEnemyDeath");                   
                    Destroy(gameObject, 2.8f);
                }; break;
        }        
    }
}
