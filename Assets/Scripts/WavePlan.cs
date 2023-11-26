using System.Collections.Generic;
using UnityEngine;

public class WavePlan : MonoBehaviour
{
    public List<int> enemiesInWaves;

    public List<int> GetEnemiesInWave(int wave)
    {
        return enemiesInWaves;
    }

    public GameObject GetEnemyToSpawn(int enemyId)
    {
        switch (enemyId)
        {
            case 1:
                return(Resources.Load<GameObject>("Prefabs/EnemyCube"));
            case 2:
                return(Resources.Load<GameObject>("Prefabs/SmallEnemyCube"));
            case 3:
                return(Resources.Load<GameObject>("Prefabs/HugeEnemyCube"));
            default:
                return null;
        }
    }
}