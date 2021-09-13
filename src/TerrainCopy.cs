using UnityEngine;

class TerrainCopy{




	public GameObject FromPrefab(GameObject prefab){


		


		GameObject obj=Object.Instantiate(prefab);
		Terrain instance=obj.GetComponent<Terrain>();

		Terrain template=prefab.GetComponent<Terrain>();


        TerrainData terrainData = new TerrainData();
        
       
        terrainData.baseMapResolution = template.terrainData.baseMapResolution;
        terrainData.heightmapResolution = template.terrainData.heightmapResolution;
        terrainData.alphamapResolution = template.terrainData.alphamapResolution;
        terrainData.SetDetailResolution(template.terrainData.detailResolution, template.terrainData.detailResolutionPerPatch);


        Vector3 size= template.terrainData.size;// 16f;
        //size.y*=16f;
        terrainData.size=size;

        terrainData.wavingGrassSpeed = template.terrainData.wavingGrassSpeed;
        terrainData.wavingGrassAmount = template.terrainData.wavingGrassAmount;
        terrainData.wavingGrassStrength = template.terrainData.wavingGrassStrength;

        terrainData.treePrototypes=template.terrainData.treePrototypes;
        terrainData.terrainLayers=template.terrainData.terrainLayers;
        terrainData.detailPrototypes=template.terrainData.detailPrototypes;
        TerrainCollider collider=instance.gameObject.GetComponent<TerrainCollider>();
        collider.terrainData=terrainData;
        instance.terrainData=terrainData;

        return obj;

    }

	
}