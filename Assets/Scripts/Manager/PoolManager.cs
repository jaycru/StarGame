using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*************************
//创建人：Jaycr
//创建时间：#CreateTime#
//描述：对象池的管理器
//*************************

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;
    
    private Pool<GameObject> bulletPool;
    private Pool<GameObject>[] enemyPools;

    public GameObject bulletPrefab;
    public GameObject[] enemyPrefabs;

    public GameObject bulletContainer;
    public GameObject enemyContainer;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        Initialize();
    }

    private void Initialize()
    {
        bulletPool = new Pool<GameObject>(() => Instantiate(bulletPrefab));
        enemyPools = new Pool<GameObject>[enemyPrefabs.Length];
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            GameObject prefab = enemyPrefabs[i];
            enemyPools[i] = new Pool<GameObject>(() => Instantiate(prefab));
        }
    }
    
    /// <summary>
    /// 租借子弹
    /// </summary>
    /// <returns></returns>
    public GameObject RentBullet()
    {
        GameObject bullet = bulletPool.Rent();
        bullet.SetActive(false);
        return bullet;
    }
    /// <summary>
    /// 归还子弹
    /// </summary>
    /// <param name="bullet"></param>
    public void ReturnBullet(GameObject bullet)
    {
        bullet.transform.SetParent(bulletContainer.transform);
        bulletPool.Return(bullet);
    }
    
    /// <summary>
    /// 给指定敌人池扩容
    /// </summary>
    /// <param name="i">敌人池编号</param>
    /// <param name="size">要扩充的容量</param>
    public void EnemyPoolExpansion(int i, int size)
    {
        enemyPools[i].AddMaxSize(size);
    }
    /// <summary>
    /// 租借敌人
    /// </summary>
    /// <param name="i">敌人编号i</param>
    public GameObject RentEnemy(int i)
    {
        return enemyPools[i].Rent();
    }

    /// <summary>
    /// 归还敌人
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="i">敌人编号</param>
    public void ReturnEnemy(GameObject enemy, int i)
    {
        enemy.transform.SetParent(enemyContainer.transform.GetChild(i));
        enemyPools[i].Return(enemy);
    }
}
