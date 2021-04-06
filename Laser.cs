using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 6f;
    private UIManager _canvas;
    private GameObject host { get; set; }

    private Vector3 _directionVector = Vector3.up;
    private float _YBound = 8.0f;

    // Start is called before the first frame update
    void Start()
    {
        var canvasGM = GameObject.Find("Canvas");
        if(canvasGM != null)
        {
            _canvas = canvasGM.GetComponent<UIManager>();
        }
        else { Debug.LogWarning("Laser: _canvas is null"); }
    }

    // Update is called once per frame
    void Update()
    {
        SpaceUtility.CalculateMoving(gameObject, _directionVector, _speed * Time.deltaTime, null, _YBound);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Enemy":
                {
                    IncrementHostKillCount();
                    if (_canvas != null) _canvas.UpdateScore(10);

                    Destroy(gameObject);
                } break;

            case "Asteroid":
            case "PlayersShield":
                {
                    Destroy(gameObject);
                }; break;
            case "Player":
                {
                    if (host != null && host.CompareTag("Player"))
                    {
                        
                    }
                    else{
                        Destroy(gameObject);
                    }
                };break;
        }
    }

    public void SetHost(GameObject host)
    {
        this.host = host;
    }

    public GameObject GetHost()
    {
        return this.host;
    }

    public void IncrementHostKillCount()
    {
        Player player = host != null ? host.GetComponent<Player>() : null;

        if(player != null)
        {
            player.IncrementEnemyKilled();
        }
    }

    public void SetDirectionVector(Vector3 direction)
    {
        _directionVector = direction;
        _YBound = -8.0f;
        transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
    }
}
