using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;


public class TerrainStyle{



    public TerrainStyle(){}
    public TerrainStyle(float[,] map){
        this.map=map;
    }
    public TerrainStyle(StyleMap map){
        this.map=map.Get();
    }


    public TerrainStyle(float[,] map, int index){
        this.map=map;
        this.layerIndex=index;
    }
    public TerrainStyle(StyleMap map, int index){
        this.map=map.Get();
        this.layerIndex=index;
    }

	//private Texture2D texture;
    private float[,] map;


    public float scale=1; //or weight
    public int layerIndex=0; //only used for terrain texture


    public float rotate=0;


    public delegate float[,] TextureModifier(float[,] map, Terrain terrain);
    public TextureModifier modifier;

    public float[,] GetMap(Terrain terrain){

        float[,] copyTexture= map.Clone() as float[,];
        // copyTexture.SetPixels(texture.GetPixels());
        // copyTexture.Apply();

        
        if(modifier!=null){
            copyTexture = modifier(copyTexture, terrain);
        }


        if(Mathf.Abs(rotate)>0.01f){

            
            int size=Mathf.Max(copyTexture.GetLength(0), copyTexture.GetLength(1));
            int offset=size/2;
            size+=offset;
            offset=offset/2;

            //init
            float[,] rotateTexture= new float[size, size];
            for(int x=0;x<rotateTexture.GetLength(0);x++){
                for(int y=0;y<rotateTexture.GetLength(1);y++){
                    rotateTexture[x,y]=0f;
                }
            }

            //center array values 
            for(int x=0;x<copyTexture.GetLength(0);x++){
                for(int y=0;y<copyTexture.GetLength(1);y++){
                    rotateTexture[x+offset,y+offset]=copyTexture[x, y];
                }
            }
            return RotateMap(rotateTexture, -rotate-90f);
        }

        
        return copyTexture;
    }


    public float GetScale(){
        return scale;
    }


    public int GetLayerIndex(){
        return layerIndex;
    }
    
    public static TerrainStyle FromTexture(Texture2D tex){
       TerrainStyle t= new TerrainStyle();
       //t.texture=texture;

        Color[] colors=tex.GetPixels();
        float[,] map=new float[tex.width,tex.height];
        for(int y=0;y<tex.height;y++){
            for(int x=0;x<tex.width;x++){
                map[x, y]=colors[x+y*tex.width].r;
            }
        }

        t.map=map;

       return t;
    }



    float[,] RotateMap(float[,] map, float angle)
    {

        angle*=Mathf.Deg2Rad;

        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        int width=map.GetLength(0);
        int height=map.GetLength(1);

        float w2=width/2.0f;
        float h2=height/2.0f;
        
        
        //int  x,y;
        float x1,y1, x2,y2;
 
        float x0 = rot_x (sin, cos, -w2, -h2) + w2;
        float y0 = rot_y (sin, cos, -w2, -h2) + h2;
 
        float dx_x = rot_x (sin, cos, 1.0f, 0.0f);
        float dx_y = rot_y (sin, cos, 1.0f, 0.0f);
        float dy_x = rot_x (sin, cos, 0.0f, 1.0f);
        float dy_y = rot_y (sin, cos, 0.0f, 1.0f);
       
       
        float[,] rotImage = new float[width, height];

        x1 = x0;
        y1 = y0;
 
        for (int x = 0; x < width; x++) {
            x2 = x1;
            y2 = y1;
            for (int  y = 0; y < height; y++) {
                //rotImage.SetPixel (x1, y1, Color.clear);          
     
                x2 += dx_x;//rot_x(angle, x1, y1);
                y2 += dx_y;//rot_y(angle, x1, y1);
                rotImage[x, y]= getFloatValue(map, x2, height-y2);
            }
 
            x1 += dy_x;
            y1 += dy_y;
           
        }
 
       return rotImage;
    }
 
    private float getFloatValue(float[,] map, float x, float y)
    {
        
        int x1 = (int) Mathf.Floor(x);
        int y1 = (int) Mathf.Floor(y);

        int width=map.GetLength(0);
        int height=map.GetLength(1);

 
        if(x1 >= width || x1 < 0 ||
           y1 >= height || y1 < 0) {
           return 0f;
        } 
       
        return map[x1,y1];
        

    }
 
    private float rot_x (float sin, float cos, float x, float y) {
        return (x * cos + y * (-sin));
    }
    private float rot_y (float sin, float cos, float x, float y) {
        return (x * sin + y * cos);
    }
 



    public static TerrainStyle[] MakeTextureNoise(Terrain t){


       
        TerrainStyle tex1=new TerrainStyle();
        tex1.layerIndex=2;
        tex1.map=StyleMap.ForTerrainTexture(t)
            .AddPerlinNoise(10f, 0.7f, -0.5f)
            .AddPerlinNoise(40f, 0.6f, -0.3f)
            .Get();
       

        TerrainStyle tex2=new TerrainStyle();
        tex2.map=StyleMap.ForTerrainTexture(t)
            .AddPerlinNoise(150f, 1f, -0.3f)
            .Get();
        tex2.layerIndex=1;
     

        return new TerrainStyle[] {tex1, tex2};

    }



    public static TerrainStyle[] MakeDetailNoise(Terrain t){



        TerrainStyle tex1=new TerrainStyle();
        tex1.map=StyleMap.ForTerrainDetail(t)
            .AddPerlinNoise(10f, 0.7f, -0.5f)
            .AddPerlinNoise(40f, 0.6f, -0.3f)
            .Scatter(20)
            .Get();
        tex1.layerIndex=0;
        

        TerrainStyle tex2=new TerrainStyle();
        tex2.map=StyleMap.ForTerrainDetail(t)
            .AddPerlinNoise(150f, 1f, -0.3f)
            .Scatter(50)
            .Get();
        tex2.layerIndex=1;


        TerrainStyle tex3=new TerrainStyle();
        tex3.map=StyleMap.ForTerrainDetail(t)
            .AddPerlinNoise(150f, 1f, -0.3f)
            .Scatter(50)
            .Get();
        tex3.layerIndex=2;



        return new TerrainStyle[] {tex1, tex2, tex3};


    }


    public static TerrainStyle[] MakeHeightNoise(Terrain t, float height){



        TerrainStyle tex1=new TerrainStyle();
        tex1.map=StyleMap.ForTerrainHeight(t)
            .AddPerlinNoise(5f, 1f, 0f)
            .Then(delegate(StyleMap map){

                StyleMap.ForTerrainHeight(t)
                    .AddPerlinNoise(25f, 1f, -0.3f)
                    .Normalize()
                    .Then(delegate(StyleMap tmp){
                         map.Mult(tmp)
                            .Scale(0.4f);
                    })
                    .Scale(0.6f)
                    .Then(delegate(StyleMap tmp){
                         map.Add(tmp);
                    });
            })
            .ScaleTerrainHeight(height, t)
            .Add(
                StyleMap.ForTerrainHeight(t)
                    .AddPerlinNoise(3f, 1f, 0)
                    .Mult(
                        StyleMap.ForTerrainHeight(t)
                            .AddPerlinNoise(25f, 1f, -0.7f)
                            .Normalize()
                    )
                    .ScaleTerrainHeight(height/5, t)
            )
            .Get();






        return new TerrainStyle[] {tex1};


    }





}







