using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFeature : MonoBehaviour
{

    public Texture2D[] detailTextures;
    public Texture2D[] heightMaps;
    public Vector2 centerDetail;
    public Vector2[] centerHeight;

    public bool randomize=true;
    public float random=0.2f;
    public int seed=45;

    public bool adative=true;
    public bool mergeHeight=true;
    public bool autoSeed=true;

    public float[] heightMapScale=new float[1]{0.02490386f};//=0.02490386f;

    public bool addHeight=true;
    public bool addTrees=true;
    public bool addDetails=true;


    private delegate void DrawDelegate(Vector2 offset, Terrain terrain);





    Terrain GetNeighbor(int x, int y, Terrain terrain){

        Debug.Log("Find neighbor rel "+x+"x, "+y+"y");

        if(x>0&&terrain.rightNeighbor!=null){
            if(y>0){
                return terrain.rightNeighbor.topNeighbor;
            }
            if(y<0){
                return terrain.rightNeighbor.bottomNeighbor;
            }
            return terrain.rightNeighbor;

        }

        if(x<0&&terrain.leftNeighbor!=null){
            if(y>0){
                return terrain.leftNeighbor.topNeighbor;
            }
            if(y<0){
                return terrain.leftNeighbor.bottomNeighbor;
            }
            return terrain.leftNeighbor;
        }


        if(y>0&&terrain.topNeighbor!=null){

            if(x>0){
                return terrain.topNeighbor.rightNeighbor;
            }
            if(x<0){
                return terrain.topNeighbor.leftNeighbor;
            }


            return terrain.topNeighbor;
        }
        if(y<0&&terrain.bottomNeighbor!=null){

            if(x>0){
                return terrain.bottomNeighbor.rightNeighbor;
            }
            if(x<0){
                return terrain.bottomNeighbor.leftNeighbor;
            }

            return terrain.bottomNeighbor;
        }
            
        return null;
    }

    public void DrawOnTerrain(Vector3 pos, Terrain terrain){

    


        if(randomize&&autoSeed){
            seed=Random.Range(1, 10000);
        }

        if(randomize){
            Random.seed=seed;
        }


        
        if(addHeight){
            DrawHeight(pos, terrain);
        }
        

        if(addDetails){
            DrawDetails(pos, terrain);
        }




       

        if(addTrees){
            DrawTrees(pos, terrain);
        }

        if(randomize){
            Random.seed = System.Environment.TickCount;
        }

        

    }

    void DrawHeight(Vector3 pos, Terrain terrain){
        if(heightMaps.Length==0){
            return;
        }
        int index=Random.Range(0, heightMaps.Length);
    
        Texture2D heightMap=heightMaps[index];

        int width = heightMap.width;
        int height = heightMap.height;

        Vector3 detailPos=pos*terrain.terrainData.heightmapResolution;
        Vector2 p=centerHeight[index]*terrain.terrainData.heightmapResolution/terrain.terrainData.size.x;
        Vector2 offset=new Vector2((int)(detailPos.x-p.x), (int)(detailPos.z-p.y));




        DrawGroup(offset, terrain, width-1, delegate(Vector2 offset, Terrain terrain){

            float[,] heights=terrain.terrainData.GetHeights(0, 0, width, height);
            heights=UpdateHeightMap(heights, offset, heightMap, heightMapScale[index]);
            terrain.terrainData.SetHeights(0, 0, heights);
            terrain.terrainData.SyncHeightmap();

        });
            
            
    }

    void DrawDetails(Vector3 pos, Terrain terrain){

        Vector3 detailPos=pos*terrain.terrainData.detailResolution;
        Vector2 p=centerDetail*terrain.terrainData.detailResolution/terrain.terrainData.size.x;
        //NOTE why are x and z(y) flipped?
        Vector2 offset=new Vector2((int)(detailPos.x-p.x), (int)(detailPos.z-p.y));

        DrawGroup(offset, terrain, terrain.terrainData.detailWidth, delegate(Vector2 offset, Terrain terrain){
            UpdateDetailLayers(offset, detailTextures, terrain);
        });
        
    }


    
    void DrawGroup(Vector2 offset, Terrain b, int s, DrawDelegate draw){

        draw(offset, b);
        foreach(Vector2 neighbor in GetOverflowNeighbors(offset)){
            Vector2 offsetN=offset-new Vector2(neighbor.x*s, neighbor.y*s);
            Terrain nterrain=GetNeighbor((int)neighbor.x, (int)neighbor.y, b);
            if(nterrain!=null){
                draw(offsetN, nterrain);
            }
        }

    }

    float[,] UpdateHeightMap(float[,] heights, Vector2 offset, Texture2D texture, float scale){

       
        int width = texture.width;
        int height = texture.height;

        if(!adative){
            SetAll(heights, 0);
        }
            
        for (int y = 0; y < height; y++){
           for (int x = 0; x < width; x++)
            {

                int xx=(int)(x+offset.x);
                int yy=(int)(y+offset.y);

                if(xx>=0&&xx<width&&yy>=0&&yy<height){

                    if(mergeHeight){
                        heights[yy, xx]=Mathf.Max(heights[yy, xx], texture.GetPixel(x, y).r*scale);
                    }else{
                        heights[yy, xx]+=texture.GetPixel(x, y).r*scale;
                    }

                    
                }
            }
        }

        return heights;
    }

    void SetAll(float[,] map, float value){

        int width = map.GetLength(0); 
        int height = map.GetLength(1);


        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {
                map[x, y]=value;
            }
        }

    }

    void SetAll(int[,] map, int value){

        int width = map.GetLength(0); 
        int height = map.GetLength(1);


        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {
                map[x, y]=value;
            }
        }

    }


    List<Vector2> GetOverflowNeighbors(Vector2 offset){

                

        List<Vector2> list=new List<Vector2>();

        if(offset.x<0){
            list.Add(new Vector2(-1,0));

            if(offset.y<0){
                list.Add(new Vector2(-1,-1));
            }
            if(offset.y>=0){
                list.Add(new Vector2(-1,1));
            }
        }
        if(offset.y<0){
            list.Add(new Vector2(0,-1));
            if(offset.x>=0){
                list.Add(new Vector2(1,-1));
            }
        }

        if(offset.x>=0){
            list.Add(new Vector2(1,0));
            if(offset.y>=0){
                list.Add(new Vector2(1,1));
            }
        }

        if(offset.y>=0){
            list.Add(new Vector2(0,1));
        }

        foreach(Vector2 neighbor in list){
            //Debug.Log("Check Neighbor: "+neighbor);
                
        }

        return list;

    }





    

    void UpdateDetailLayers(Vector2 offset, Texture2D[] textures, Terrain terrain){

        for(int i=0;i<textures.Length; i++){
            Texture2D texture=textures[i];
            if(texture!=null){

                int[,] map = terrain.terrainData.GetDetailLayer(0, 0, terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, i);
                int width = map.GetLength(0); // read from file
                int height = map.GetLength(1); // read from file


                if(!adative){
                    SetAll(map, 0);
                }

                

                for (int y = 0; y < height; y++){
                   for (int x = 0; x < width; x++)
                    {
                        if(texture.GetPixel(x, y).r>0.5){

                            int xx=(int)(x+offset.x);
                            int yy=(int)(y+offset.y);

                            if(xx>=0&&xx<width&&yy>=0&&yy<height){
                                map[yy,xx]=1;
                            }
                        }  
                    } 
                }


                terrain.terrainData.SetDetailLayer(0, 0, i, map);

            }

        }


    }

    void DrawTrees(Vector3 terrainPos, Terrain terrain){


        if(!adative){
            terrain.terrainData.SetTreeInstances(new TreeInstance[0],false);
        }


        if(Random.Range(0, 5)<2){
            return;
        }

        TreeInstance treeInstance = new TreeInstance();

        terrainPos.x+=(Random.Range(-random, random));
        terrainPos.z+=(Random.Range(-random, random));

        treeInstance.position = terrainPos;
        treeInstance.prototypeIndex = Random.Range(0, terrain.terrainData.treePrototypes.Length);
        treeInstance.widthScale = 1f;
        treeInstance.heightScale = 1f;
        treeInstance.color = Color.red;
        treeInstance.lightmapColor = Color.white;
        treeInstance.rotation=Random.Range (0f, Mathf.PI * 2);


        TreeInstance[] treeInstances=terrain.terrainData.treeInstances;

        
        TreeInstance[] tempTreeInstances=new TreeInstance[treeInstances.Length+1];
        for(int i=0;i<treeInstances.Length;i++){
            tempTreeInstances[i]=treeInstances[i];

        }
        tempTreeInstances[treeInstances.Length]=treeInstance;
        treeInstances=tempTreeInstances;
        
         
        terrain.terrainData.SetTreeInstances(treeInstances, false);

        
        
    }


}
