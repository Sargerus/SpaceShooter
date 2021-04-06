using System;
using UnityEngine;

public static class GlobalVariables
{
    public static float playerSpeed { get; private set; } = 7.0f;

    public enum GameplayMode : byte
    {
        SinglePlayer = 0,
        MultiPlayers = 1
    }
    public enum PowerupType : byte 
    {
        Triple_Shot = 1,
        Speed = 2,
        Shield = 3,
    }

   public enum MainWeaponary : int
   {
       Laser = -1,
       TripleLaser = 1
   }

    public struct PowerupCountDown
    {
        public float _tripleLaser;
        public float _speed;
        public float _shield;

        public PowerupCountDown(float tripleLaser = 5.0f, float speed = 3.0f, float shield = 5.0f)
        {
            _tripleLaser = tripleLaser;
            _speed = speed;
            _shield = shield;
        }
    }
}

public class PowerupFactory
{
    private static PowerupFactory instance;
    private PowerupFactory() { }
    public static PowerupFactory getInstance()
    {
        if (instance == null)
        {
            instance = new PowerupFactory();
        }

        return instance;
    }

    public void makePowerup(GlobalVariables.PowerupType type, Transform parent = null)
    {
        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);

        GameObject prefab = (GameObject)Resources.Load("Prefabs/Powerup/" + Enum.GetName(typeof(GlobalVariables.PowerupType), type) + "_Powerup", typeof(GameObject));

        GameObject powerUp = GameObject.Instantiate(prefab, new Vector3(UnityEngine.Random.Range(-10.0f, 10.0f), 8.0f, 0), Quaternion.identity);
        powerUp.transform.parent = parent != null ? parent : null;

        Powerup powerupComponent = powerUp.GetComponent<Powerup>();        
        if(powerupComponent != null) 
        { 
            ClearPowerupFields(powerupComponent);
            switch (type)
            {
                case GlobalVariables.PowerupType.Speed:
                    {
                        powerupComponent.speed = 10.0f;
                        powerupComponent.durationInSeconds = 3.0f;                    
                    }; break;

                case GlobalVariables.PowerupType.Triple_Shot:
                    {
                        powerupComponent.mainWeraponUpgradeLevel = GlobalVariables.MainWeaponary.TripleLaser;
                        powerupComponent.durationInSeconds = 5.0f;
                    }; break;

                case GlobalVariables.PowerupType.Shield:
                    {
                        powerupComponent.spawnShield = true;
                        powerupComponent.durationInSeconds = 5.0f;
                    }; break;
            }
        }
    }

    private void ClearPowerupFields(IUpgrading powerup)
    {
        powerup.durationInSeconds = 0.0f;
        powerup.mainWeraponUpgradeLevel = GlobalVariables.MainWeaponary.Laser;
        powerup.speed = 0.0f;
        powerup.spawnShield = false;
    }
}

static class SpaceUtility
{
    public static GlobalVariables.PowerupType getRandomPowerupType()
    {
        Array enumValues = Enum.GetValues(typeof(GlobalVariables.PowerupType));

        return (GlobalVariables.PowerupType)enumValues.GetValue(UnityEngine.Random.Range(0, enumValues.Length));
    }
    public static void CalculateMoving(GameObject obj, Vector3 directionVector, float speed = .5f, float? boundX = null, float? boundY = null)
    {
        obj.transform.Translate(directionVector * speed, Space.World);

        if (boundY != null &&
           ( (directionVector.y > 0 && obj.transform.position.y > boundY) ||
            (directionVector.y < 0 && obj.transform.position.y < boundY) ) )
        {
            GameObject.Destroy(obj);
        }

        if (boundX != null &&
            ( (directionVector.x > 0 && obj.transform.position.x > boundX) ||
             (directionVector.x < 0 && obj.transform.position.x < boundX) ) )
        {
            GameObject.Destroy(obj);
        }
    }
}