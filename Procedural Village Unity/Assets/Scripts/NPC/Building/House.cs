using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : NPCBuilding
{
    [SerializeField] protected Transform spawnPoint = null;

    public int numSpawnedNPC = 0;

    int numNPCsToSpawn = -1;

    public void SetNPCsToSpawn(int num) {  numNPCsToSpawn = num; }

    protected override void Start()
    {
        base.Start();

        if (!spawnPoint) { spawnPoint = transform.GetChild(0).transform; }
        if (!spawnPoint) { Debug.LogError("'Spawnpoint' GameObject missing as child of House prefab"); }
    }
    public void SpawnNPCs()
    {
        if (numNPCsToSpawn <= 0) numNPCsToSpawn = maxNpcs;
        if (!spawnPoint)
        {
            Debug.LogError("Missing reference to 'Spawnpoint' GameObject");
            return;
        }
        numSpawnedNPC = numNPCsToSpawn;
        GameObject npcPrefab = SimulationManager.Instance.GetNPCPrefab();
        float offset = 0.15f;
        int n = numNPCsToSpawn - numNPCsToSpawn / 2;
        for (int i = 0; i < numNPCsToSpawn; i++)
        {
            Vector3 posOffset = transform.rotation * new Vector3(0, 0, offset * n);
            GameObject npc = Instantiate(npcPrefab, spawnPoint.position + posOffset, Quaternion.identity);

            NPCInfo info = npc.GetComponent<NPCInfo>();
            info.SetHouse(this);
            info.SetWorkPlace(SimulationManager.Instance.GetNewWorkingPlace(info, gameObject.transform.position));
            info.SetMarketPlace(SimulationManager.Instance.GetNewMarketPlace(info, info.GetWorkPlace().transform.position));

            ++n;
        }
    }
}
