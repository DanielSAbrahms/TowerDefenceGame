using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavePlan : MonoBehaviour
{
    public List<int> enemiesInWaves;
    
    public List<int> GetEnemiesInWave(int wave)
    {
        return enemiesInWaves;
    }
}
