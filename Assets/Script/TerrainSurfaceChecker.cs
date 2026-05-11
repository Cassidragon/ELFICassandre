using UnityEngine;

public static class TerrainSurfaceChecker
{

    public static int GetMainTexture(Vector3 worldPos, Terrain terrain)
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;

        float percentX = (worldPos.x - terrainPos.x) / terrainData.size.x;
        float percentZ = (worldPos.z - terrainPos.z) / terrainData.size.z;

        int mapX = Mathf.RoundToInt(percentX * (terrainData.alphamapWidth - 1));
        int mapZ = Mathf.RoundToInt(percentZ * (terrainData.alphamapHeight - 1));

        mapX = Mathf.Clamp(mapX, 0, terrainData.alphamapWidth - 1);
        mapZ = Mathf.Clamp(mapZ, 0, terrainData.alphamapHeight - 1);

        float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

        float maxMix = 0;
        int maxIndex = 0;

        for (int i = 0; i < splatmapData.GetLength(2); i++)
        {
            if (splatmapData[0, 0, i] > maxMix)
            {
                maxIndex = i;
                maxMix = splatmapData[0, 0, i];
            }
        }

        return maxIndex;
    }
}
