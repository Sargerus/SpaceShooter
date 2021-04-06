using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField]
    private GameObject _skyPrefab;
    private float _skyPrefabMinY; // keep it positive

    private GameObject _skyGM1;
    private GameObject _skyGM2;

    private float _speed = 1.0f;

    //private int _targetFrameRate = 60;
    //
    //// Start is called before the first frame update
    //void Awake()
    //{
    //    QualitySettings.vSyncCount = 0;
    //    Application.targetFrameRate = _targetFrameRate;
    //}
    //
    //// Update is called once per frame
    //void Update()
    //{
    //    if(Application.targetFrameRate > _targetFrameRate)
    //    {
    //        Application.targetFrameRate = _targetFrameRate;
    //    }
    //}

    public void Start()
    {
        _skyGM1 = Instantiate(_skyPrefab, Vector3.zero, Quaternion.identity);

        if(_skyGM1 != null)
        {
           SpriteRenderer spriteRenderer = _skyGM1.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                Bounds size = spriteRenderer.bounds;

                _skyPrefabMinY = size.min.y;
                if (_skyPrefabMinY < 0) _skyPrefabMinY *= -1;
            }
        }

        _skyGM2 = Instantiate(_skyPrefab, new Vector3(0, _skyPrefabMinY * 2, 0), Quaternion.identity);
    }

    public void Update()
    {
        if (_skyGM1 != null && _skyGM1.transform.position.y < -_skyPrefabMinY * 2) Destroy(_skyGM1.gameObject);
        if (_skyGM2 != null && _skyGM2.transform.position.y < -_skyPrefabMinY * 2) Destroy(_skyGM2.gameObject);

        if (_skyGM1 != null)
        {
            SpaceUtility.CalculateMoving(_skyGM1, Vector3.down, _speed * Time.deltaTime, null, -_skyPrefabMinY * 2);
        }
        if (_skyGM2 != null)
        {
            SpaceUtility.CalculateMoving(_skyGM2, Vector3.down, _speed * Time.deltaTime, null, -_skyPrefabMinY * 2);
        }

        if(_skyGM1 == null)
        {
            _skyGM1 = Instantiate(_skyPrefab, new Vector3(0, _skyPrefabMinY * 2, 0), Quaternion.identity);
        }

        if(_skyGM2 == null)
        {
            _skyGM2 = Instantiate(_skyPrefab, new Vector3(0, _skyPrefabMinY * 2, 0), Quaternion.identity);
        }
    }
}
