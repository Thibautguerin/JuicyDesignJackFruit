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

    [Header("GeneralVariable - Ship")]

    [SerializeField]
    private float shipSpeed = 2;
    [SerializeField]
    private int shipHp = 1;
    [SerializeField]
    private float shipCooldown = 1;

    #region Geter
    public float ShipSpeed { get => shipSpeed;}
    public int ShipHp { get => shipHp;}
    public float ShipCooldown { get => shipCooldown;}
    #endregion

    [Header("GeneralVariable - Enemy")]

    [SerializeField]
    private float enemySpeed = 2;
    [SerializeField]
    private int nbEnemyInRow = 5;
    [SerializeField]
    private int nbRow = 3;

    #region Geter
    public float EnemySpeed { get => enemySpeed;}
    public int NbEnemyInRow { get => nbEnemyInRow;}
    public int NbRow { get => nbRow;}
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        SingletonAwake();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
