using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*************************
//创建人：Jaycr
//创建时间：#CreateTime#
//描述：敌人刷新点
//*************************

public class EnemyPoint : MonoBehaviour
{
    public int number;//敌人编号
    public int enemySize;//敌人数量
    private int count;//已生成的敌人数量
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        GenerateEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            GenerateEnemy();
        }
    }

    /// <summary>
    /// 给敌人池扩容
    /// </summary>
    private void Initialize()
    {
        PoolManager.instance.EnemyPoolExpansion(number, enemySize);
    }

    /// <summary>
    /// 生成Enemy
    /// </summary>
    public void GenerateEnemy()
    {
        int needToGenerate = enemySize - count;
        for (int i = 0; i < needToGenerate; i++)
        {
            GameObject enemy = PoolManager.instance.RentEnemy(number);
            enemy.transform.position = transform.position;
            enemy.GetComponent<Enemy>().SetEnmeyPoint(this);
            enemy.SetActive(true);
            count++;
        }
    }

    public void EnemyDie()
    {
        count--;
    }
}
