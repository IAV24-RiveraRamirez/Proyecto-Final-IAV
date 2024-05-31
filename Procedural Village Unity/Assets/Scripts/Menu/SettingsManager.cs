using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    public bool toogleWater = true;
    public bool randomOffsets = true;
    public int nOctaves = 0;
    public int terrainDepth = 0;
    public float perlinScale = 0;
    public int nHouses = 0;
    public float maxVillageSeparation = 0;
    public int maxHousesHeightDiff = 0;
    public float maxTreeHeightSpawn = 0;
    public float treeSpawnInterval = 0;
    public float treeSpawnMinValue = 0;
    public float maxHouseAngle = 0;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void toggleWater() { toogleWater = !toogleWater; }
    public void toggleRandomOffsets() {  randomOffsets = !randomOffsets; }
    public void setNOctaves(float v) { nOctaves = (int)v; }
    public void setTerrainDepth(float v) { terrainDepth = (int)v; }
    public void setPerlinScale(float v) { perlinScale = v; }
    public void setNHouses(float v) { nHouses = (int)v; }
    public void setMaxVillageSeparation(float v) { maxVillageSeparation = v; }
    public void setMaxHousesHDiff(float v) { maxHousesHeightDiff = (int)v; }
    public void setMaxTreeHeightSpawn(float v) { maxTreeHeightSpawn = v; }
    public void setTreeSpawnInterval(float v) { treeSpawnInterval = v; }
    public void setTreenMinSpawnValue(float v) { treeSpawnMinValue = v; }

    public void setMaxHouseAngle(float v) { maxHouseAngle = v; }
}
