using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StyleMap{


	public delegate void StyleMapDelegate(StyleMap t1);

	private float[,] map;


    public int randomXOffset=-1;
    public int randomYOffset=-1;

	public StyleMap(int width, int height):this(width, height, 0){

	}


  


    public StyleMap RandomizeNoiseOffset(){


        randomXOffset=GetWidth()*Random.Range(0, 1000);
        randomYOffset=GetHeight()*Random.Range(0, 1000);

        return this;

    }


     public StyleMap SetNoiseOffset(int x, int y){


        randomXOffset=x;
        randomYOffset=y;

        return this;

    }


	public StyleMap(int width, int height, float value){

		map=new float[width, height];
        
		SetValues(value);

	}

	public float[,] Get(){
		return map;
	}

    public float GetAt(int x, int y){
        return map[x,y];
    }

	public StyleMap(float[,] map){

		this.map=map;
		
	}

    public StyleMap(StyleMap styleMap){

        map=new float[styleMap.GetWidth(), styleMap.GetHeight()];
        this.SetValues(styleMap.Get());
        
    }


    public int GetWidth(){
        return map.GetLength(0);
    }
    public int GetHeight(){
        return map.GetLength(1);
    }


	public StyleMap Then(StyleMapDelegate cb){
		cb(this);
		return this;
	}

	public StyleMap Add(StyleMap map2){
    	return Add(map2.Get());
    }




    public StyleMap Subtract(StyleMap map2){

        return Subtract(map2.Get());
    }

    public StyleMap Subtract(float value){

        int width=GetWidth();
        int height=GetHeight();

        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {
                map[x, y]=Mathf.Max(0,map[x, y]-value);             
            }
        }

        return this;
    }



    public StyleMap Clamp(float min, float max){

        int width=GetWidth();
        int height=GetHeight();

        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {
                map[x, y]=Mathf.Max(min, Mathf.Min(map[x, y], max));             
            }
        }

        return this;
    }


     public StyleMap Subtract(float[,] map2){

        int width=GetWidth();
        int height=GetHeight();

        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {
                map[x, y]=Mathf.Max(0,map[x, y]-map2[x , y]);             
            }
        }

        return this;
    }

    public StyleMap Add(float value){

        int width=GetWidth();
        int height=GetHeight();

        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {
                map[x, y]=Mathf.Min(1,map[x, y]+value);             
            }
        }

        return this;
    }
    public StyleMap Add(float[,] map2){

        int width=GetWidth();
        int height=GetHeight();

        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {
                map[x, y]=map[x, y]+map2[x, y];                
            }
        }

        return this;
    }

    public StyleMap Mult(StyleMap map2){
    	return Mult(map2.Get());
    }

    public StyleMap Mult(float[,] map2){

        int width=GetWidth();
        int height=GetHeight();

        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {
            
                float value=map[x, y]*map2[x, y];
                map[x, y]=value;
                
            }
        }

        return this;

    }

    public StyleMap Mult(float v){

        int width=GetWidth();
        int height=GetHeight();

        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {
            
                float value=map[x, y]*v;
                map[x, y]=value;
                
            }
        }

        return this;

    }

    public StyleMap Normalize(){

    	return Scale(1);
    }



    public StyleMap Scale(float scale){


        int width=GetWidth();
        int height=GetHeight();

        float max=-Mathf.Infinity;
        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {
                float value=map[x,y];
                if(value>max){
                    max=value;
                }
            }
        }

        float mult=scale/max;
        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {
                float value=map[x,y];
                map[x, y]= value*mult;
            }
        }

        return this;

     }


    public StyleMap MaskLessThan(float value){

        int width=GetWidth();
        int height=GetHeight();

        Debug.Log("Mask Less Than: "+value);


        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {
                
                if(map[x,y]<=value){
                    map[x, y]=1;
                }else{
                    map[x, y]=0;
                }
            }
        }

        return this;


    }


    public StyleMap MaskLessThanTerrainHeight(float height, Terrain t){
        return MaskLessThan(height/(float)t.terrainData.heightmapScale.y);
    }


    public StyleMap Invert(){
        int width=GetWidth();
        int height=GetHeight();


        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {
                map[x, y]=1-map[x, y];
            }
        }

        return this;
    }




    // public StyleMap MaskMoreThan(float value){



    // }


    public StyleMap ScaleTerrainHeight(float height, Terrain t){
    	return Scale(height/t.terrainData.heightmapScale.y);
    }


    public StyleMap Scatter(float scatter){

        int width=GetWidth();
        int height=GetHeight();


        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {
                float value=map[x,y]/scatter;
                if(Random.value<value){
                    value=1;
                }else{
                    value=0;
                }
                map[x, y]=value;
            }
        }

        return this;

    }

    public StyleMap RadialFadeOuter(float length){


    	int width=GetWidth();
        int height=GetHeight();

        int cx=width/2;
		int cy=height/2;

		float maxSqr=Mathf.Min(cx*cx,cy*cy);

        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {

		        float dx=x-cx;
		        float dy=y-cy;

		        float distSqr=dx*dx+dy*dy;
		        

		        float value=map[x,y];

                value*=Mathf.Min(1, (maxSqr-distSqr)/(maxSqr*length*2));
                map[x,y]=value;


            }
        }

        return this;
    }


    public StyleMap Blur(int radius, int strength){

    	this.map=(new MapBlur()).Blur(map, radius, strength);


    	return this;
    }

    public StyleMap RadialFadeInner(float length){


    	int width=GetWidth();
        int height=GetHeight();

        int cx=width/2;
		int cy=height/2;

		float maxSqr=cx*cx+cy*cy;

        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {



		        float dx=x-cx;
		        float dy=y-cy;

		        float distSqr=dx*dx+dy*dy;
		        

		        float value=map[x,y];
		        value*=Mathf.Min(1, distSqr/(maxSqr*length));
                map[x,y]=value;


            }
        }

        return this;
    }


    public StyleMap SquareFadeEdges(float length){


    	int width=GetWidth();
        int height=GetHeight();


        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {

		        float value=map[x,y];
	
		        value*=Mathf.Min(1, x/(width*length));
                value*=Mathf.Min(1, (width-x)/(width*length));

                value*=Mathf.Min(1, y/(height*length));
                value*=Mathf.Min(1, (height-y)/(height*length));

                map[x,y]=value;

            }
        }

        return this;
    }

    public StyleMap SetValues(float[,] map2){


        int w=map2.GetLength(0);
        int h=map2.GetLength(1);

        int width=GetWidth();
        int height=GetHeight();


        if(w!=width||h!=height){
            map2=(new MapResize()).Resize(map2, width, height);
        }




        for (int y = 0; y < width; y++){
            for (int x = 0; x < height; x++)
            {

                map[x, y]=map2[x,y];
            }
        }

        return this;
    }


    public StyleMap Resize(int width, int height){
        map=(new MapResize()).Resize(map, width, height);
        return this;
    }



    public StyleMap SetValues(float value){

        for (int y = 0; y < GetHeight(); y++){
            for (int x = 0; x < GetWidth(); x++)
            {
                map[x, y]=value;
            }
        }

        return this;
     }


    public StyleMap FlipXY(){

        int width=GetWidth();
        int height=GetHeight();

        float[,] map2=new float[height, width];
         for (int y = 0; y < GetHeight(); y++){
            for (int x = 0; x < GetWidth(); x++)
            {
                map2[y, x]=map[x, y];
            }
        }

        map=map2;
        return this;


    }

    public StyleMap AddPerlinNoise(float size, float scale, float offset){


        int width=GetWidth();
        int height=GetHeight();

        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {

                float value=map[x,y];
                value+=Mathf.Clamp((Mathf.PerlinNoise(((x+randomXOffset)*100.0f)/(width*size), ((y+randomYOffset)*100.0f)/(height*size))+offset)*scale, 0, 1);
                map[x, y]=value;

            }
        }

        return this;

    }


    public static StyleMap ForTerrainTexture(Terrain t){

    	return new StyleMap(t.terrainData.alphamapResolution, t.terrainData.alphamapResolution);
    }

    public static StyleMap ForTerrainDetail(Terrain t){

    	return new StyleMap(t.terrainData.detailResolution, t.terrainData.detailResolution);
    }

    public static StyleMap ForTerrainHeight(Terrain t){

    	return (new StyleMap(t.terrainData.heightmapResolution, t.terrainData.heightmapResolution)).RandomizeNoiseOffset();
    }



}