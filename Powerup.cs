using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour, IUpgrading
{
    [SerializeField]
    private float _speed = 5.0f;

    public GlobalVariables.MainWeaponary mainWeraponUpgradeLevel { get; set; }
    public float durationInSeconds { get; set; }
    public float speed { get; set; }
    public bool spawnShield { get; set; }

    [SerializeField]
    private AudioClip _audioClip;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        SpaceUtility.CalculateMoving(this.gameObject, Vector3.down, _speed * Time.deltaTime, null, -6.0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Player":
                {
                    AudioSource.PlayClipAtPoint(_audioClip, transform.position);
                    Destroy(this.gameObject);
                };break;
        }
    }
}
