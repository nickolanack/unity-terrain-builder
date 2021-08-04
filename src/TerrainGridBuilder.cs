using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TerrainGridBuilder : MonoBehaviour
{
    
    public GameObject decalPrefab;
    public GameObject treePrefab;


    public static TerrainGridBuilder main;

    void Start(){

        TerrainGridBuilder.main=this;


    }


    public void DrawFeature(Vector2 pos, TerrainFeature feature){

        RaycastHit hit;
        if(!GetHit(pos, out hit)){
            return;
        }
        Terrain terrain = GetTerrainHit(hit);

        if(terrain==null){
            return;
        }

        Vector3 terrainPos = GetTerrainPosition(terrain, hit);
        feature.DrawOnTerrain(terrainPos, terrain);


    }

    
   
    public void TerrainMakeTree(Vector2 pos){

        RaycastHit hit;
        if(!GetHit(pos, out hit)){
            return;
        }
        Terrain terrain = GetTerrainHit(hit);
        Vector3 terrainPos = GetTerrainPosition(terrain, hit);
    





        TreePrototype treePrototype = new TreePrototype();
        treePrototype.prefab = treePrefab;
         
        TreePrototype[] treePrototypeCollection = new TreePrototype[1] { treePrototype };
        terrain.terrainData.treePrototypes = treePrototypeCollection;
         
        TreeInstance treeInstance = new TreeInstance();


        treeInstance.position = terrainPos;//hit.point;//new Vector3(terrainPos.x, 0, terrainPos.z);


        treeInstance.prototypeIndex = 0;
        treeInstance.widthScale = 1f;
        treeInstance.heightScale = 1f;
        treeInstance.color = Color.red;
        treeInstance.lightmapColor = Color.white;
        treeInstance.rotation=Random.Range (0f, Mathf.PI * 2);




         
        TreeInstance[] treeInstanceCollecion = new TreeInstance[1] { treeInstance };
        terrain.terrainData.SetTreeInstances(treeInstanceCollecion, false);


        Debug.Log("Tree: "+treeInstance.position);

    }


    void TerrainSetAllHeight(Terrain terrain, float height){

         int xRes = terrain.terrainData.heightmapResolution;
         int yRes = terrain.terrainData.heightmapResolution;


         float[,] heights = new float[xRes, yRes];
         
         for (int y = 0; y < yRes; y++) {
             for (int x = 0; x < xRes; x++) {
                 heights[x,y] = 0;
             }
         }

        terrain.terrainData.SetHeights(0, 0, heights);
        terrain.terrainData.SyncHeightmap();

    }


    bool GetHit(Vector2 pos, out RaycastHit hit){
        
        Debug.Log("Terrain");
        Ray ray = Camera.main.ScreenPointToRay(pos);
        return Physics.Raycast(ray, out hit);

    }

    Terrain GetTerrainHit(RaycastHit hit){
       
        return hit.transform.gameObject.GetComponent<Terrain>();
        
    }

    Vector3 GetTerrainPosition(Terrain terrain, RaycastHit hit){

       
        Vector3 point= (hit.point-hit.transform.position)/terrain.terrainData.size.x;
        Debug.Log("Terrain Point: "+point);
        return point;
    }




    void TerrainPull(Vector2 pos){

        RaycastHit hit;
        if(!GetHit(pos, out hit)){
            return;
        }


        Terrain terrain = GetTerrainHit(hit);
        Vector3 terrainPosition = GetTerrainPosition(terrain, hit)*terrain.terrainData.heightmapResolution;
    


        Debug.Log(terrain.SampleHeight(hit.point));
        Debug.Log(terrainPosition);
        terrain.SampleHeight(hit.point);

        float[,] heights=new float[1,1];
        heights[0,0]=0.001f;

        terrain.terrainData.SetHeights((int)terrainPosition.x, (int)terrainPosition.z, heights);
        terrain.terrainData.SyncHeightmap();
     
    }




}
