using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*************************
//创建人：Jaycr
//创建时间：#CreateTime#
//描述：敌人的代码
//*************************

public class Enemy : MonoBehaviour
{
    public int number;//敌人编号
    private EnemyPoint enemyPoint;//敌人刷新点
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        Invoke("Die", 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Die()
    {
        PoolManager.instance.ReturnEnemy(gameObject, number);
        gameObject.SetActive(false);
        enemyPoint.EnemyDie();
    }

    public void SetEnmeyPoint(EnemyPoint enemyPoint)
    {
        this.enemyPoint = enemyPoint;
    }
}
