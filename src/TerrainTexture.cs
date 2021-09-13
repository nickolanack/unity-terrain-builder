using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTexture{



	public Texture2D texture;
    public float scale=1; //or weight
    public int layerIndex=0; //only used for terrain texture


    public float rotate=0;


    public delegate Texture2D TextureModifier(Texture2D texture);
    public TextureModifier modifier;

    public Texture2D GetTexture(){

        Texture2D copyTexture= new Texture2D(texture.width, texture.height);
        copyTexture.SetPixels(texture.GetPixels());
        copyTexture.Apply();

        
        if(modifier!=null){
            copyTexture = modifier(copyTexture);
        }


        if(Mathf.Abs(rotate)>0.01f){



            
            int size=Mathf.Max(copyTexture.width, copyTexture.height);
            size+=size/2;

            Texture2D rotateTexture= new Texture2D(size, size);
            Color[] colors=rotateTexture.GetPixels();
            for(int i=0;i<colors.Length;i++){
                colors[i]=Color.black;
            }
            rotateTexture.SetPixels(colors);
            rotateTexture.SetPixels((size-copyTexture.width)/2, (size-copyTexture.height)/2, copyTexture.width, copyTexture.height, copyTexture.GetPixels());
            rotateTexture.Apply();
        

            Debug.Log("rotating: "+(rotate-90f));
            copyTexture = RotateTexture(rotateTexture, -rotate-90f);
        }

        
        return copyTexture;
    }

    public float GetScale(){
        return scale;
    }


    public int GetLayerIndex(){
        return layerIndex;
    }
    
    



    Texture2D RotateTexture(Texture2D tex, float angle)
    {

        angle*=Mathf.Deg2Rad;

        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);
        
        
        int  x,y;
        float x1,y1, x2,y2;
 
        int w = tex.width;
        int h = tex.height;
        float x0 = rot_x (sin, cos, -w/2.0f, -h/2.0f) + w/2.0f;
        float y0 = rot_y (sin, cos, -w/2.0f, -h/2.0f) + h/2.0f;
 
        float dx_x = rot_x (sin, cos, 1.0f, 0.0f);
        float dx_y = rot_y (sin, cos, 1.0f, 0.0f);
        float dy_x = rot_x (sin, cos, 0.0f, 1.0f);
        float dy_y = rot_y (sin, cos, 0.0f, 1.0f);
       
       
        Texture2D rotImage = new Texture2D(tex.width, tex.height);

        x1 = x0;
        y1 = y0;
 
        for (x = 0; x < tex.width; x++) {
            x2 = x1;
            y2 = y1;
            for ( y = 0; y < tex.height; y++) {
                //rotImage.SetPixel (x1, y1, Color.clear);          
     
                x2 += dx_x;//rot_x(angle, x1, y1);
                y2 += dx_y;//rot_y(angle, x1, y1);
                rotImage.SetPixel ( (int)Mathf.Floor(x), (int)Mathf.Floor(y), getPixel(tex,x2, tex.height-y2));
            }
 
            x1 += dy_x;
            y1 += dy_y;
           
        }
 
        rotImage.Apply();
       return rotImage;
    }
 
    private Color getPixel(Texture2D tex, float x, float y)
    {
        Color pix;
        int x1 = (int) Mathf.Floor(x);
        int y1 = (int) Mathf.Floor(y);
 
        if(x1 > tex.width || x1 < 0 ||
           y1 > tex.height || y1 < 0) {
            pix = Color.clear;
        } else {
            pix = tex.GetPixel(x1,y1);
        }
       
        return pix;
    }
 
    private float rot_x (float sin, float cos, float x, float y) {
        return (x * cos + y * (-sin));
    }
    private float rot_y (float sin, float cos, float x, float y) {
        return (x * sin + y * cos);
    }
 



    public static TerrainTexture[] MakeTextureNoise(Terrain t){


        // TerrainTexture tex1=new TerrainTexture();
        // tex1.texture=new Texture2D(t.terrainData.alphamapResolution, t.terrainData.alphamapResolution);
        // tex1.layerIndex=1;
        // TerrainTexture.SetValue(tex1.texture, Color.black);
        // //TerrainTexture.AddPerlinNoise(texture, 5, 0.7f);
        // TerrainTexture.AddPerlinNoise(tex1.texture, 2, 0.5f);
        // TerrainTexture.AddPerlinNoise(tex1.texture, 0.5f, 0.5f);
        


        TerrainTexture tex1=new TerrainTexture();
        tex1.texture=new Texture2D(t.terrainData.alphamapResolution, t.terrainData.alphamapResolution);
        tex1.layerIndex=2;
        TerrainTexture.SetValue(tex1.texture, Color.black);
        //TerrainTexture.AddPerlinNoise(texture, 5, 0.7f);
        //TerrainTexture.AddPerlinNoise(tex2.texture, 2, 0.5f);
        TerrainTexture.AddPerlinNoise(tex1.texture, 10f, 0.7f, -0.5f);
        TerrainTexture.AddPerlinNoise(tex1.texture, 40f, 0.6f, -0.3f);
       


        TerrainTexture tex2=new TerrainTexture();
        tex2.texture=new Texture2D(t.terrainData.alphamapResolution, t.terrainData.alphamapResolution);
        tex2.layerIndex=1;
        TerrainTexture.SetValue(tex2.texture, Color.black);
       
        TerrainTexture.AddPerlinNoise(tex2.texture, 150f, 1f, -0.3f);


        return new TerrainTexture[] {tex1, tex2};


    }



    public static TerrainTexture[] MakeDetailNoise(Terrain t){


        // TerrainTexture tex1=new TerrainTexture();
        // tex1.texture=new Texture2D(t.terrainData.alphamapResolution, t.terrainData.alphamapResolution);
        // tex1.layerIndex=1;
        // TerrainTexture.SetValue(tex1.texture, Color.black);
        // //TerrainTexture.AddPerlinNoise(texture, 5, 0.7f);
        // TerrainTexture.AddPerlinNoise(tex1.texture, 2, 0.5f);
        // TerrainTexture.AddPerlinNoise(tex1.texture, 0.5f, 0.5f);
        


        TerrainTexture tex1=new TerrainTexture();
        tex1.texture=new Texture2D(t.terrainData.detailResolution, t.terrainData.detailResolution);
        tex1.layerIndex=0;
        TerrainTexture.SetValue(tex1.texture, Color.black);
        //TerrainTexture.AddPerlinNoise(texture, 5, 0.7f);
        //TerrainTexture.AddPerlinNoise(tex2.texture, 2, 0.5f);
        TerrainTexture.AddPerlinNoise(tex1.texture, 10f, 0.7f, -0.5f);
        TerrainTexture.AddPerlinNoise(tex1.texture, 40f, 0.6f, -0.3f);
        TerrainTexture.Scatter(tex1.texture, 20);


        TerrainTexture tex2=new TerrainTexture();
        tex2.texture=new Texture2D(t.terrainData.detailResolution, t.terrainData.detailResolution);
        tex2.layerIndex=1;
        TerrainTexture.SetValue(tex2.texture, Color.black);
        TerrainTexture.AddPerlinNoise(tex2.texture, 150f, 1f, -0.3f);
        TerrainTexture.Scatter(tex2.texture, 50);


        TerrainTexture tex3=new TerrainTexture();
        tex3.texture=new Texture2D(t.terrainData.detailResolution, t.terrainData.detailResolution);
        tex3.layerIndex=2;
        TerrainTexture.SetValue(tex3.texture, Color.black);
        TerrainTexture.AddPerlinNoise(tex3.texture, 150f, 1f, -0.3f);
        TerrainTexture.Scatter(tex3.texture, 50);




        return new TerrainTexture[] {tex1, tex2, tex3};


    }


    public static TerrainTexture[] MakeHeightNoise(Terrain t, float height){


        // TerrainTexture tex1=new TerrainTexture();
        // tex1.texture=new Texture2D(t.terrainData.alphamapResolution, t.terrainData.alphamapResolution);
        // tex1.layerIndex=1;
        // TerrainTexture.SetValue(tex1.texture, Color.black);
        // //TerrainTexture.AddPerlinNoise(texture, 5, 0.7f);
        // TerrainTexture.AddPerlinNoise(tex1.texture, 2, 0.5f);
        // TerrainTexture.AddPerlinNoise(tex1.texture, 0.5f, 0.5f);
        


        TerrainTexture tex2=new TerrainTexture();
        tex2.texture=new Texture2D(t.terrainData.heightmapResolution, t.terrainData.heightmapResolution);



        tex2.layerIndex=2;
        TerrainTexture.SetValue(tex2.texture, Color.black);
        //TerrainTexture.AddPerlinNoise(texture, 5, 0.7f);
        //TerrainTexture.AddPerlinNoise(tex2.texture, 2, 0.5f);
        TerrainTexture.AddPerlinNoise(tex2.texture, 10f, 0.5f, -0.5f);
        TerrainTexture.AddPerlinNoise(tex2.texture, 40f, 0.2f, -0.2f);
        TerrainTexture.Scale(tex2.texture,height/t.terrainData.heightmapScale.y);


        return new TerrainTexture[] {tex2};


    }


    static void Scale(Texture2D texture, float s){

        float max=-Mathf.Infinity;
        for (int y = 0; y < texture.height; y++){
            for (int x = 0; x < texture.width; x++)
            {
                Color color=texture.GetPixel(x, y);
                float value=color.r;
                if(value>max){
                    max=value;
                }
            }
        }

        float mult=s/max;
        for (int y = 0; y < texture.height; y++){
            for (int x = 0; x < texture.width; x++)
            {
                Color color=texture.GetPixel(x, y);
                float value=color.r;
                color.r=value*mult;
                texture.SetPixel(x, y, color);
            }
        }


     }


    static void Scatter(Texture2D texture, float scatter){

        for (int y = 0; y < texture.height; y++){
            for (int x = 0; x < texture.width; x++)
            {
                Color color=texture.GetPixel(x, y);
                float value=color.r/scatter;
                if(Random.value<value){
                    color.r=1;
                }else{
                    color.r=0;
                }
                texture.SetPixel(x, y, color);

            }
        }


    }

    static void SetValue(Texture2D texture, Color c){

        for (int y = 0; y < texture.height; y++){
            for (int x = 0; x < texture.width; x++)
            {
                texture.SetPixel(x, y, c);
            }
        }


     }

    static void AddPerlinNoise(Texture2D texture, float size, float scale, float offset){


        for (int y = 0; y < texture.height; y++){
            for (int x = 0; x < texture.width; x++)
            {

                Color color=texture.GetPixel(x, y);
                float value=color.r;


                value+=Mathf.Clamp(Mathf.PerlinNoise(x/(texture.width/size), y/(texture.height/size))*scale, 0, 1);
                color.r=value;
                texture.SetPixel(x, y, color);

            }
        }

    }


}







