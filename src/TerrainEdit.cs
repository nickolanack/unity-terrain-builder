using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TerrainEdit{



    public bool additive=true;
    public bool mergeHeight=true;


    public delegate void TerrainHit(Vector3 terrainPosition, Terrain terrain);
    public delegate Terrain NeighborResolver(int x, int y, Terrain terrain);

    /**
     * a function that returns the neighbor of terrain, given x,y (tile offset from terrain)
     * for example neighborResolver(1,0,t) should return the tile 1 to the right of t
     */
    public NeighborResolver neighborResolver;



    public Vector3 AtCenter(Terrain t){
        return new Vector3(0.5f, 0f, 0.5f);
    }


    public void AtScreenPos(Vector2 pos, TerrainHit callback){


    	RaycastHit hit;
        if(!GetHit(pos, out hit)){
            return;
        }
        Terrain terrain = GetTerrainHit(hit);

        if(terrain==null){
            return;
        }

        Vector3 terrainPos = GetTerrainPosition(terrain, hit);
        callback(terrainPos, terrain);

    }





     bool GetHit(Vector2 pos, out RaycastHit hit){
        
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




    public void DrawHeight(Vector3 pos, Terrain terrain, TerrainTexture[] stamps){

        
        foreach(TerrainTexture stamp in stamps){ //selected.GetComponent<TerrainStamp>();
            Texture2D texture=stamp.GetTexture();

            if(texture==null){
                continue;
            }

            int size=terrain.terrainData.heightmapResolution;

            Vector2 offset=CalcTextureShift(pos, terrain.terrainData.heightmapResolution, texture);


            DrawTerrainTiles(offset, terrain, size-1, texture.width, delegate(Vector2 shift, Terrain terrain){
                UpdateHeightMap(shift, texture, stamp.GetScale(), terrain);
            });

        }
            
            
    }


    public void DrawTexture(Vector3 pos, Terrain terrain, TerrainTexture[] stamps){

        
 
        foreach(TerrainTexture stamp in stamps){
        
            Texture2D texture=stamp.GetTexture();

            if(texture==null){
                continue;
            }

            Vector2 offset=CalcTextureShift(pos, terrain.terrainData.alphamapResolution, texture);

            DrawTerrainTiles(offset, terrain, terrain.terrainData.alphamapWidth, texture.width, delegate(Vector2 shift, Terrain terrain){
                UpdateAlphaMap(shift, texture, stamp.GetLayerIndex(), stamp.GetScale(), terrain);
            });
                
            
        }
    }


    


    public void DrawDetails(Vector3 pos, Terrain terrain, TerrainTexture[] stamps){




        //Vector3 detailPos=pos*terrain.terrainData.detailResolution;
       
        for(int i=0;i<terrain.terrainData.detailPrototypes.Length; i++){

    

            foreach(TerrainTexture stamp in stamps){
                Texture2D texture=stamp.GetTexture();
                if(texture==null){
                    continue;
                }

                Vector2 offset=CalcTextureShift(pos, terrain.terrainData.detailResolution, texture);

                DrawTerrainTiles(offset, terrain, terrain.terrainData.detailWidth, texture.width, delegate(Vector2 shift, Terrain terrain){
                    UpdateDetailLayer(shift, texture, stamp.GetLayerIndex(), terrain);
                });
                    
                
            }
        }

        
        
    }


     public void DrawTrees(Vector3 terrainPos, Terrain terrain){


        if(!additive){
            terrain.terrainData.SetTreeInstances(new TreeInstance[0],false);
        }



        TreeInstance treeInstance = new TreeInstance();

        //terrainPos.x+=(Random.Range(-random, random));
        //terrainPos.z+=(Random.Range(-random, random));

        treeInstance.position = terrainPos;
        treeInstance.prototypeIndex = Random.Range(0, terrain.terrainData.treePrototypes.Length);
        treeInstance.widthScale = 1f;
        treeInstance.heightScale = 1f;
        treeInstance.color = Color.white;
        treeInstance.lightmapColor = Color.white;
        treeInstance.rotation=Random.Range (0f, Mathf.PI * 2);


        TreeInstance[] treeInstances=terrain.terrainData.treeInstances;

        
        TreeInstance[] tempTreeInstances=new TreeInstance[treeInstances.Length+1];
        for(int i=0;i<treeInstances.Length;i++){
            tempTreeInstances[i]=treeInstances[i];

        }
        tempTreeInstances[treeInstances.Length]=treeInstance;
        treeInstances=tempTreeInstances;
        
         
        terrain.terrainData.SetTreeInstances(treeInstances, true);
        terrain.terrainData.SyncHeightmap();
        
        
    }



    Vector2 CalcTextureShift(Vector3 pos, float resolution, Texture2D texture){

          Vector3 detailPos=pos*resolution;
          Vector2 p=new Vector2(texture.width/2, texture.height/2);//centerDetail*terrain.terrainData.detailResolution/terrain.terrainData.size.x;
          return new Vector2((int)(detailPos.x-p.x), (int)(detailPos.z-p.y));
    }


    delegate void DrawDelegate(Vector2 offset, Terrain terrain);
    void DrawTerrainTiles(Vector2 offset, Terrain b, int terrainSize, int textureSize, DrawDelegate draw){


        Debug.Log("Draw at: "+offset+" terrain size: "+terrainSize+" texture size: "+textureSize);

        draw(offset, b);
        foreach(Vector2 neighbor in GetOverflowNeighbors(offset, terrainSize, textureSize)){
            Vector2 offsetN=offset-new Vector2(neighbor.x*terrainSize, neighbor.y*terrainSize);
            Terrain nterrain=ResolveNeighborTerrain((int)neighbor.x, (int)neighbor.y, b);
            if(nterrain!=null){
                draw(offsetN, nterrain);
            }
        }

    }

    Terrain ResolveNeighborTerrain(int x, int y, Terrain terrain){

        if(neighborResolver!=null){
        	return neighborResolver(x, y, terrain);
        }	

        return null; //no overflow.
    }





    void UpdateHeightMap(Vector2 shift, Texture2D texture, float scale, Terrain terrain){

       
        float[,] heights=terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
        //int width = texture.width;
        //int height = texture.height;

        int width=heights.GetLength(0);
        int height=heights.GetLength(1);


        
        if(!additive){
            SetAll(heights, 0);
        }


        loopTexture(width, height, shift, texture, delegate(int x, int y, int xh, int yh){ 
            float value=texture.GetPixel(xh, yh).r;
            
            value*=scale;

            if(mergeHeight){
                value=Mathf.Max(heights[y, x], value);
                heights[y, x]=0;
            }
                
            heights[y, x]+=value;
        });


        terrain.terrainData.SetHeights(0, 0, heights);
        terrain.terrainData.SyncHeightmap();

    }




    void UpdateAlphaMap(Vector2 shift, Texture2D texture, int layer, float weight, Terrain terrain){

        float[,,] map = new float[terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight,terrain.terrainData.alphamapTextureCount];

        //TODO: use map. and write entire map back at end

        int width = map.GetLength(0); // read from file
        int height = map.GetLength(1); // read from file



        int x0=Mathf.Max(0,(int)shift.x);
        int y0=Mathf.Max(0,(int)shift.y);

        int x1=Mathf.Min(width, (int)(texture.width+shift.x));
        int y1=Mathf.Min(height, (int)(texture.height+shift.y));


        float[,,] values=terrain.terrainData.GetAlphamaps(x0, y0, x1-x0, y1-y0);


        loopTexture(width, height, shift, texture, delegate(int x, int y, int xh, int yh){

            //float[,,] values=terrain.terrainData.GetAlphamaps(x, y, 1, 1);

            float value=texture.GetPixel(xh, yh).r*weight;


            for (int l = 0; l < terrain.terrainData.alphamapTextureCount; l++)
            {
                values[x-x0,y-y0,l]*=(1-value);
                //values[0,0,l]*=(1-value);
                //Debug.Log(l+": "+values[0,0,l]);
            }
            values[x-x0,y-y0,layer]+=value;
            //values[0,0,layer]+=value;
            Debug.Log("*"+layer+": "+values[0,0,layer]);

            //terrain.terrainData.SetAlphamaps(x, y, values);


        });

        terrain.terrainData.SetAlphamaps(x0, y0, values);

    }

    protected delegate void Loop(int x, int y, int tx, int ty);
    void loopTexture(int width, int height, Vector2 shift, Texture2D texture, Loop loop){



        int x0=Mathf.Max(0,(int)shift.x);
        int y0=Mathf.Max(0,(int)shift.y);

        int x1=Mathf.Min(width, (int)(texture.width+shift.x));
        int y1=Mathf.Min(height, (int)(texture.height+shift.y));

            

        for (int y = y0; y < y1; y++){
           for (int x = x0; x < x1; x++)
            {

                int xh=(int)(x-shift.x);
                int yh=(int)(y-shift.y);

               //if(InRange(xh, texture.width)&&InRange(yh, texture.height)){} // not needed since x, and y are clamped
                loop(x, y, xh, yh);
            }
        }


    }


    
    void UpdateDetailLayer(Vector2 shift, Texture2D texture, int layer, Terrain terrain){

        int[,] map = terrain.terrainData.GetDetailLayer(0, 0, terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, layer);
        int width = map.GetLength(0); // read from file
        int height = map.GetLength(1); // read from file


        if(!additive){
            SetAll(map, 0);
        }
        
        loopTexture(width, height, shift, texture, delegate(int x, int y, int xh, int yh){ 

            if(texture.GetPixel(xh, yh).r>0.5){ 

                //NOTE: why do y and x need to be flipped!! I added this to fix placement after testing
                map[y,x]=1 ;      
            } 
        });

        

        terrain.terrainData.SetDetailLayer(0, 0, layer, map);


    }

   

   





    // bool InRange(int x, int width){
    //     return x>0&&x<width;
    // }

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


    List<Vector2> GetOverflowNeighbors(Vector2 offset, int tileSize, int textureSize){

                
        int x0=(int)Mathf.Floor(offset.x/tileSize);
        int x1=(int)Mathf.Floor((offset.x+textureSize)/tileSize);

        int y0=(int)Mathf.Floor(offset.y/tileSize);
        int y1=(int)Mathf.Floor((offset.y+textureSize)/tileSize);

        Debug.Log("x["+x0+","+x1+"] y["+y0+","+y1+"]");






        List<Vector2> list=new List<Vector2>();


        for (int y = y0; y <= y1; y++){
            for (int x = x0; x <= x1; x++)
            {
                if(!(x==0&&y==0)){
                    list.Add(new Vector2(x,y));
                }
            }
        }

        return list;

    }


}