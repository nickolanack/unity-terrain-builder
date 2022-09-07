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



    public TerrainStyle.TextureModifier modifier;
    

    public delegate void TextureLoader(string textureName, TextureLoadedEvent callback);
    public delegate void TextureLoadedEvent(Texture2D texture);
    public TextureLoader loadTexture;



    void Update(){


        if((!loaded)&&loadTexture!=null){
           loadTexture(textureName, delegate(Texture2D t){

                texture=t;

            });
            loaded=true;
        }

    }


    public TerrainStyle GetTerrainTexture(){
        
        TerrainStyle t=TerrainStyle.FromTexture(texture);
        //t.(texture);
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
