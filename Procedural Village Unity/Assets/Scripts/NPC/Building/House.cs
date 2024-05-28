using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : NPCBuilding
{
    [SerializeField] protected Transform spawnPoint = null;

    public int numSpawnedNPC = 0;

    protected override void Start()
    {
        base.Start();

        if (!spawnPoint) { spawnPoint = transform.GetChild(0).transform; }
        if (!spawnPoint) { Debug.LogError("'Spawnpoint' GameObject missing as child of House prefab"); }
    }
    public void SpawnNPCs(int num)
    {
        if (!spawnPoint)
        {
            Debug.LogError("Missing reference to 'Spawnpoint' GameObject");
            return;
        }
        numSpawnedNPC = num;
        GameObject npcPrefab = SimulationManager.Instance.GetNPCPrefab();
        float offset = 0.15f;
        int n = num - num / 2;
        for (int i = 0; i < num; i++)
        {
            Vector3 posOffset = transform.rotation * new Vector3(0, 0, offset * n);
            GameObject npc = Instantiate(npcPrefab, spawnPoint.position + posOffset, Quaternion.identity);

            NPCInfo info = npc.GetComponent<NPCInfo>();
            info.SetHouse(this);
            info.SetWorkPlace(SimulationManager.Instance.GetNewWorkingPlace(info, gameObject.transform.position));
            info.SetLeisurePlace(SimulationManager.Instance.GetNewLeisurePlace(info, gameObject.transform.position));

            ++n;
        }
    }
}
