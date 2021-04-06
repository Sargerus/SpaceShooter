using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;

    private GameObject _explosion;

    private float _rotateDirection;

    private Vector3 directionVector;

    private int _hp = 2;

    private void Awake()
    {
        _explosion = (GameObject)Resources.Load("Prefabs/Damage/Explosion", typeof(GameObject));
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector3 buf = Vector3.zero;
        transform.position = new Vector3(Random.Range(-10.0f, 10.0f), 8.0f, 0);
        _rotateDirection = Random.value < 0.5f ? -1 : 1;

        if (transform.position.x <= 0)
        {
            buf = new Vector3(Random.Range(0.1f, 10.0f), 0, 0) - transform.position;
        }
        else
        {
            buf = new Vector3(Random.Range(-10.0f, 0), 0, 0) - transform.position;
        }

        directionVector.x = buf.x; directionVector.y = buf.y; directionVector.z = buf.z;
        directionVector.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        SpaceUtility.CalculateMoving(gameObject, directionVector, _speed * Time.deltaTime, directionVector.x <= 0 ? -10.0f : 10.0f, null);//, -8.0f
        transform.Rotate(new Vector3(0, 0, _rotateDirection), 60.0f * Time.deltaTime, Space.World);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Asteroid":
            case "PlayersShield":
            case "Player":
                {
                    Damage(_hp);
                }; break;

            case "Enemy":
            case "Laser":
                {
                    Damage(1);
                };break;
        }
    }

    private void Damage(int points)
    {
        _hp -= points;

        if (_hp <= 0)
        {
            

            Destroy(GetComponent<SpriteRenderer>());
            Destroy(GetComponent<CircleCollider2D>());
            GetComponent<AudioSource>().Play();
            Instantiate(_explosion, transform.position, Quaternion.identity, transform);
           
            Destroy(gameObject, 2.4f);
        }
    }
}
