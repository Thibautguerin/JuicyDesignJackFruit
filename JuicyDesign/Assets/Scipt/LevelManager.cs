using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    #region Singleton
    private static LevelManager instance;
    
    public static LevelManager Instance { get => instance; }

    private void SingletonAwake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(this);
    }
    #endregion

    #region Ship
    [Header("GeneralVariable - Ship")]

    [SerializeField]
    private float shipSpeed = 2;
    [SerializeField]
    private int shipHp = 1;
    [SerializeField]
    private float shipCooldown = 1;
    [SerializeField]
    private float shipBulletSpeed = 2;
    [SerializeField]
    private float shipAcceleration = 4;
    [SerializeField]
    private float shipDeceleration = 4;
    [SerializeField]
    private float shipMaxRotation = 25;

    #region Geter
    public float ShipSpeed { get => shipSpeed;}
    public int ShipHp { get => shipHp;}
    public float ShipCooldown { get => shipCooldown;}
    public float ShipBulletSpeed { get => shipBulletSpeed;}
    public float ShipAcceleration { get => shipAcceleration;}
    public float ShipDeceleration { get => shipDeceleration;}
    public float ShipMaxRotation { get => shipMaxRotation;}
    #endregion
    #endregion

    #region Enemy
    [Header("GeneralVariable - Enemy")]

    [SerializeField]
    private float enemySpeed = 2;
    [SerializeField]
    private int nbEnemyInRow = 5;
    [SerializeField]
    private int nbRow = 3;
    [SerializeField]
    private float enemyBulletSpeed = 2;
    [SerializeField]
    private Vector2 enemySize = Vector2.one;
    [SerializeField]
    private float enemyYOffset = 0.5f;

    #region Geter
    public float EnemySpeed { get => enemySpeed;}
    public int NbEnemyInRow { get => nbEnemyInRow;}
    public int NbRow { get => nbRow;}
    public float EnemyBulletSpeed { get => enemyBulletSpeed;}
    public Vector2 EnemySize { get => enemySize;}
    public float EnemyYOffset { get => enemyYOffset; }
    #endregion
    #endregion

    #region Radar
    [Header("GeneralVariable - Radar")]

    [SerializeField]
    private float radarRotationSpeed = 180;
    [SerializeField]
    private float radarFadeDuration = 0.5f;

    #region Geter
    public float RadarRotationSpeed { get => radarRotationSpeed;}
    public float RadarFadeDuration { get => radarFadeDuration; }
    #endregion

    #endregion

    [Header("EffectInputs")]
    public List<ActivationInput> activationInputs = new List<ActivationInput>();

    // Start is called before the first frame update
    void Awake()
    {
        SingletonAwake();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var item in activationInputs)
        {
            if (Input.GetKeyDown(item.input))
                item.isActive = !item.isActive;
        }
    }
}

[System.Serializable]
public class ActivationInput
{
    public string name;
    public KeyCode input;
    public bool isActive = false;
}
