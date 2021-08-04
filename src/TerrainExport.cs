using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using UnityEngine.Rendering;


public class TerrainExport : MonoBehaviour
{



    public bool autoExport=false;

    void Start(){

        if(!autoExport){
            return;
        }

        Terrain terrain=gameObject.GetComponent<Terrain>();
        if(terrain!=null){
            ExportTerrain(terrain);
        }

    }


    public void ExportTerrain(Vector2 pos){

        RaycastHit hit;
        if(!GetHit(pos, out hit)){
            return;
        }
        Terrain terrain = GetTerrainHit(hit);
        if(terrain!=null){
            ExportTerrain(terrain);
        }


    }


    public void ExportTerrain(Terrain terrain){


        ExportDetailLayers(terrain);

        ExportAlphaLayers(terrain);
        
        ExportHeight(terrain);


        Debug.Log(terrain.terrainData.treePrototypes);
        foreach(TreePrototype tree in terrain.terrainData.treePrototypes){
            Debug.Log(tree);
        }

        foreach(TreeInstance tree in terrain.terrainData.treeInstances){
            Debug.Log(tree);
        }


    }


    void ExportHeight(Terrain terrain){

        int resolution=terrain.terrainData.heightmapResolution;

        float[,] map=terrain.terrainData.GetHeights(0, 0, resolution, resolution);

        float max=0f;
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                if(map[x,y]>max){
                    max=map[x,y];
                }
            }
        }
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
               
                map[x,y]=map[x,y]/max;
                
            }
        }


        SaveFile(ImageConversion.EncodeToPNG(ToTexture(map)), "heightmap-"+max+".png");


    }


    void ExportDetailLayers(Terrain terrain){

        Debug.Log(terrain.terrainData.detailPrototypes);
        foreach(DetailPrototype detail in terrain.terrainData.detailPrototypes){
            Debug.Log(detail);
        }

        for(int i=0; i<terrain.terrainData.detailPrototypes.Length; i++){

            int[,] map=terrain.terrainData.GetDetailLayer(0, 0,  terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, i);

            SaveFile(ImageConversion.EncodeToPNG(ToTexture(map)), "detail-"+i+".png");

        }

    }


    void ExportAlphaLayers(Terrain terrain){
        Debug.Log(terrain.terrainData.alphamapLayers);
        //foreach(SplatPrototype spat in terrain.terrainData.splatPrototypes){
        { 
            float[,,] maps = terrain.terrainData.GetAlphamaps(0, 0, terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight);

            Texture2D[] texture = new Texture2D[terrain.terrainData.alphamapLayers];
            for (int l = 0; l < terrain.terrainData.alphamapLayers; l++)
            {
                texture[l]=new Texture2D(terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight, TextureFormat.RGBA32, false);
            }

            for (int y = 0; y < terrain.terrainData.alphamapHeight; y++)
            {
                for (int x = 0; x < terrain.terrainData.alphamapWidth; x++)
                {
                    for (int l = 0; l < terrain.terrainData.alphamapLayers; l++)
                    {
                        //Debug.Log("ColorAlpha: "+maps[x,y,l]);
                        texture[l].SetPixel(x, y, new Color(maps[x,y,l]*256.0f,  maps[x,y,l]*256.0f,  maps[x,y,l]*256.0f));
                    }
                }
            }

            for (int l = 0; l < terrain.terrainData.alphamapLayers; l++)
            {
                SaveFile(ImageConversion.EncodeToPNG(texture[l]), "texture-"+l+".png");
            }


        }
    }


    void SaveFile(byte[] bytes, string filename){
        string path=Application.dataPath + "/"+filename;
        Debug.Log("write file at: "+path);

        File.WriteAllBytes(path, bytes);
    }

  
    Texture2D ToTexture(float[,] map){



        int width = map.GetLength(0); // read from file
        int height = map.GetLength(1); // read from file
        Texture2D texture = new Texture2D (width, height, TextureFormat.RGBA32, false);

        float max=0;

        for (int y = 0; y < height; y++){
           for (int x = 0; x < width; x++)
           {
                if(map[x,y]>max){
                    max=map[x,y];
                    Debug.Log("Heightmap layer value["+x+","+y+"]: "+max);
                    
                }
              texture.SetPixel(x, y, new Color(map[x,y],  map[x,y], map[x,y]));
           }
       }

       return texture;
           

    }

    Texture2D ToTexture(int[,] map){



        int width = map.GetLength(0); // read from file
        int height = map.GetLength(1); // read from file
        Texture2D texture = new Texture2D (width, height, TextureFormat.RGBA32, false);

        float max=0;

        for (int y = 0; y < height; y++){
           for (int x = 0; x < width; x++)
           {
                if(map[x,y]>max){
                    max=map[x,y];
                    //Debug.Log("Detail layer value["+x+","+y+"]: "+max);
                    
                }
              texture.SetPixel(x, y, new Color(map[x,y],  map[x,y], map[x,y]));
           }
       }

       return texture;
           

    }

    bool GetHit(Vector2 pos, out RaycastHit hit){
        
        Debug.Log("Terrain");
        Ray ray = Camera.main.ScreenPointToRay(pos);
        return Physics.Raycast(ray, out hit);

    }

    Terrain GetTerrainHit(RaycastHit hit){
       
        
        return hit.transform.gameObject.GetComponent<Terrain>();
        
    }

   
}
