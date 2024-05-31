using UnityEngine;

/// <summary>
/// Clase encargada de añadir los NPCs a la escena y darles una zona donde dormir
/// </summary>
public class House : NPCBuilding
{
    /// <summary>
    /// Lugar de aparición para los NPC
    /// </summary>
    [SerializeField] protected Transform spawnPoint = null;

    /// <summary>
    /// Cuántos NPCs se spawnearán
    /// </summary>
    [SerializeField] int numNPCsToSpawn = -1;

    public void SetNPCsToSpawn(int num) {  numNPCsToSpawn = num; }

    public int GetNPCsToSpawn() { return numNPCsToSpawn; }

    /// <summary>
    /// Inicializa el edificio y escoge un número aleatorio de NPCs a añadir a escena
    /// </summary>
    protected override void Start()
    {
        type = BuildingType.HOUSE;
        base.Start();
        if (numNPCsToSpawn < 0) numNPCsToSpawn = maxNpcs;
        else numNPCsToSpawn = Random.Range(1, maxNpcs);

        if (!spawnPoint) { spawnPoint = transform.GetChild(0).transform; }
        if (!spawnPoint) { Debug.LogError("'Spawnpoint' GameObject missing as child of House prefab"); }
    }

    /// <summary>
    /// Método que instancia NPCs en escena. Se encarga conseguir un lugar de trabajo y de mercado para estos.
    /// </summary>
    public void SpawnNPCs()
    {
        if (!spawnPoint)
        {
            Debug.LogError("Missing reference to 'Spawnpoint' GameObject");
            return;
        }
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
