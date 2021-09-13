using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TerrainStamp : MonoBehaviour
{
    
    public Texture2D texture;
    public float scale; //or weight
    public string textureName;
    public int layerIndex=0;


    public bool rotateWithCamera=true;
    bool loaded=false;



    public TerrainTexture.TextureModifier modifier;



    void Update(){


        if((!loaded)&&AssetLibrary.Library){
            AssetLibrary.Library.LoadTexture(textureName, delegate(Texture2D t){

                texture=t;

            });
            loaded=true;
        }

    }

    public Texture2D GetTexture(){

        return GetTerrainTexture().GetTexture();

    }

    public TerrainTexture GetTerrainTexture(){
        
        TerrainTexture t=new TerrainTexture();
        t.texture=texture;
        t.scale=scale;
        t.layerIndex=layerIndex;
        t.modifier=modifier;
        if(rotateWithCamera){
            t.rotate=Camera.main.transform.eulerAngles.y;
        }
        return t;
    }


    public float GetScale(){
        return scale;
    }

     public int GetLayerIndex(){
        return layerIndex;
    }

}
