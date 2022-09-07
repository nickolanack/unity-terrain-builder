using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TerrainEdit{



    public bool additive=true;
    public bool mergeHeight=false;


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


    public float[,] GetHeight(Terrain terrain){
        return terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
    }


    public float[,] GetSlope(Terrain terrain){
        return GetSlope(terrain, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
    }
    public float[,] GetSlope(Terrain terrain, int width, int height){
        float[,] map=new float[width, height];
        for (int y = 0; y < height; y++){
           for (int x = 0; x < width; x++)
            {
                map[x, y]=terrain.terrainData.GetSteepness(y/(float)height,x/(float)width);
            }
        }
        return map;
    }

    
    public void DrawHeight(Vector3 pos, Terrain terrain, TerrainStyle[] stamps){

        
        foreach(TerrainStyle stamp in stamps){ //selected.GetComponent<TerrainStamp>();
            float[,] map=stamp.GetMap(terrain);

            if(map==null){
                continue;
            }

            int size=terrain.terrainData.heightmapResolution;

            Vector2 offset=CalcTextureShift(pos, terrain.terrainData.heightmapResolution, map);


            DrawTerrainTiles(offset, terrain, size-1, map.GetLength(0), delegate(Vector2 shift, Terrain terrain){
                UpdateHeightMap(shift, map, stamp.GetScale(), terrain);
            });

        }
            
            
    }



    public float[,] GetTextureLayer(Terrain terrain, int layerId){
        float[,,] layers= terrain.terrainData.GetAlphamaps(0, 0, terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight);
        float[,] map=new  float[terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight];


        for (int y = 0; y < terrain.terrainData.alphamapHeight; y++){
           for (int x = 0; x < terrain.terrainData.alphamapWidth; x++)
            {
                map[x, y]=layers[x,y,layerId];
            }
        }

        return map;

    }





    public void DrawTexture(Vector3 pos, Terrain terrain, TerrainStyle[] stamps){

        
 
        foreach(TerrainStyle stamp in stamps){
        
            float[,] map=stamp.GetMap(terrain);

            if(map==null){
                continue;
            }

            Vector2 offset=CalcTextureShift(pos, terrain.terrainData.alphamapResolution, map);

            DrawTerrainTiles(offset, terrain, terrain.terrainData.alphamapWidth, map.GetLength(0), delegate(Vector2 shift, Terrain terrain){
                UpdateAlphaMap(shift, map, stamp.GetLayerIndex(), stamp.GetScale(), terrain);
            });
                
            
        }
    }


    


    public void DrawDetails(Vector3 pos, Terrain terrain, TerrainStyle[] stamps){




        //Vector3 detailPos=pos*terrain.terrainData.detailResolution;
       
        for(int i=0;i<terrain.terrainData.detailPrototypes.Length; i++){

    

            foreach(TerrainStyle stamp in stamps){
                float[,] map=stamp.GetMap(terrain);
                if(map==null){
                    continue;
                }

                Vector2 offset=CalcTextureShift(pos, terrain.terrainData.detailResolution, map);

                DrawTerrainTiles(offset, terrain, terrain.terrainData.detailWidth, map.GetLength(0), delegate(Vector2 shift, Terrain terrain){
                    UpdateDetailLayer(shift, map, stamp.GetLayerIndex(), terrain);
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



    Vector2 CalcTextureShift(Vector3 pos, float resolution, float[,] map){

          Vector3 detailPos=pos*resolution;
          Vector2 p=new Vector2(map.GetLength(0)/2, map.GetLength(1)/2);//centerDetail*terrain.terrainData.detailResolution/terrain.terrainData.size.x;
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





    void UpdateHeightMap(Vector2 shift, float[,] map, float scale, Terrain terrain){

       
        float[,] heights=terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
        //int width = texture.width;
        //int height = texture.height;

        int width=heights.GetLength(0);
        int height=heights.GetLength(1);


        
        if(!additive){
            SetAll(heights, 0);
        }


        loopTexture(width, height, shift, map, delegate(int x, int y, int xh, int yh){ 
            float value=map[xh, yh];
            
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


    public void ResetTerrain(Terrain t){
        ResetTextures(t);
        ResetHeight(t, 0);
        ResetDetails(t);
    }


    public void ResetHeight(Terrain terrain, float value){  


           float[,] heights=terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
            //int width = texture.width;
            //int height = texture.height;

            int width=heights.GetLength(0);
            int height=heights.GetLength(1);


            for (int y = 0; y < height; y++){
               for (int x = 0; x < width; x++)
                {

                    heights[x, y]=value;
                }
            }
            
            terrain.terrainData.SetHeights(0, 0, heights);
            terrain.terrainData.SyncHeightmap();
            
        }



    void UpdateAlphaMap(Vector2 shift, float[,] map, int layer, float weight, Terrain terrain){

     
        //Note: when running in editor mode terrain.terrainData.alphamapTextureCount returns 1!
        float[,,] m= terrain.terrainData.GetAlphamaps(0,0, 1, 1);
        int alphamapTextureCount=m.GetLength(2);


        int width = terrain.terrainData.alphamapWidth;
        int height = terrain.terrainData.alphamapHeight;



        int x0=Mathf.Max(0,(int)shift.x);
        int y0=Mathf.Max(0,(int)shift.y);

        int x1=Mathf.Min(width, (int)(map.GetLength(0)+shift.x));
        int y1=Mathf.Min(height, (int)(map.GetLength(1)+shift.y));


        float[,,] values=terrain.terrainData.GetAlphamaps(x0, y0, x1-x0, y1-y0);


        loopTexture(width, height, shift, map, delegate(int x, int y, int xh, int yh){


            float value=map[xh, yh]*weight;


            for (int l = 0; l < alphamapTextureCount; l++)
            {
                values[x-x0,y-y0,l]*=(1-value);
                //values[0,0,l]*=(1-value);
                //Debug.Log(l+": "+values[0,0,l]);
            }
            values[x-x0,y-y0,layer]+=value;
            //values[0,0,layer]+=value;
            //Debug.Log("*"+layer+": "+values[0,0,layer]);

            //terrain.terrainData.SetAlphamaps(x, y, values);


        });

        terrain.terrainData.SetAlphamaps(x0, y0, values);

    }


    public void ResetTextures(Terrain terrain){  


        //Note: when running in editor mode terrain.terrainData.alphamapTextureCount returns 1!
        float[,,] m= terrain.terrainData.GetAlphamaps(0,0, 1, 1);
        int alphamapTextureCount=m.GetLength(2);


        float[,,] map = new float[terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight, alphamapTextureCount];
        for (int y = 0; y < terrain.terrainData.alphamapHeight; y++){
           for (int x = 0; x < terrain.terrainData.alphamapWidth; x++)
            {

                 map[x, y, 0]=1;
                for (int l =1; l <alphamapTextureCount; l++)
                {
                    map[x, y, l]=0;
                } 
            }
        }
        
        terrain.terrainData.SetAlphamaps(0, 0, map);
        
    }

    protected delegate void Loop(int x, int y, int tx, int ty);
    void loopTexture(int width, int height, Vector2 shift, float[,] map, Loop loop){



        int x0=Mathf.Max(0,(int)shift.x);
        int y0=Mathf.Max(0,(int)shift.y);

        int x1=Mathf.Min(width, (int)(map.GetLength(0)+shift.x));
        int y1=Mathf.Min(height, (int)(map.GetLength(1)+shift.y));

            

        for (int y = y0; y < y1; y++){
           for (int x = x0; x < x1; x++)
            {

                int xh=(int)(x-shift.x);
                int yh=(int)(y-shift.y);

                loop(x, y, xh, yh);
            }
        }


    }


    
    void UpdateDetailLayer(Vector2 shift, float[,] map, int layer, Terrain terrain){

        int[,] detail = terrain.terrainData.GetDetailLayer(0, 0, terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, layer);
        int width = detail.GetLength(0); // read from file
        int height = detail.GetLength(1); // read from file


        if(!additive){
            SetAll(detail, 0);
        }
        
        loopTexture(width, height, shift, map, delegate(int x, int y, int xh, int yh){ 

            if(map[xh, yh]>0.5){ 

                //NOTE: why do y and x need to be flipped!! I added this to fix placement after testing
                detail[y,x]=1 ;      
            } 
        });

        

        terrain.terrainData.SetDetailLayer(0, 0, layer, detail);


    }


    public void ResetDetails(Terrain terrain){




        int[,] map = new int[terrain.terrainData.detailWidth, terrain.terrainData.detailHeight];
        int width = map.GetLength(0); // read from file
        

        for(int l=0;l<terrain.terrainData.detailPrototypes.Length;l++){
            terrain.terrainData.SetDetailLayer(0, 0, l, map);
        }


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