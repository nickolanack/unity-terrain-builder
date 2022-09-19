using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StyleMap{


	public delegate void StyleMapDelegate(StyleMap t1);

	private float[,] map;


    public int seedIndexOffsetX=0;
    public int seedIndexOffsetY=0;

    public float perlinConst=150.0f;

    public int seedValue=0;

	public StyleMap(int width, int height):this(width, height, 0){

	}  


  


    public StyleMap RandomizeSeed(){


        seedValue=GetWidth()*Random.Range(0, 1000);
       
        return this;

    }


    public StyleMap SetPerlinConst(float c){
        perlinConst=c;
        return this;
    }

    public float GetPerlinConst(){
        return perlinConst;
    }


    public StyleMap SetSeed(int s){


        seedValue=s;

        return this;

    }


    public StyleMap SetTileXY(int x, int y){

        SetSeedIndexOffset(x*GetWidth(), y*GetHeight());

        return this;
    }


     public StyleMap SetSeedIndexOffset(int x, int y){


        seedIndexOffsetX=x;
        seedIndexOffsetY=y;

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
        this.SetPerlinConst(styleMap.GetPerlinConst());
        //this.SetSeed(styleMap.seedValue);

        SetSeedIndexOffset(styleMap.seedIndexOffsetX, styleMap.seedIndexOffsetY);
        
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
                map[x, y]=map[x, y]-value;             
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
                map[x, y]=map[x, y]-map2[x , y];             
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
                map[x, y]=map[x, y]+value;             
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

    	int width=GetWidth();
        int height=GetHeight();

        float max=-Mathf.Infinity;
        float min=Mathf.Infinity;
        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {
                float value=map[x,y];
                if(value>max){
                    max=value;
                }
                if(value<min){
                    min=value;
                }
            }
        }
        float offset=-min;
        float mult=1.0f/(max-min);
        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {
                float value=map[x,y];
                map[x, y]= Mathf.Max(0, Mathf.Min((value-min)*mult, 1));
            }
        }

        return this;
    }


    /**
     * @deprecated confusing use Add/Mult/Subtract/Normalize
     */
    public StyleMap Scale(float scale){


        return Mult(scale);

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


    public StyleMap RadialFade(AnimationCurve curve){


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
                value*=curve.Evaluate(Mathf.Max(0.0f, Mathf.Min((maxSqr-distSqr)/maxSqr,1.0f)));
                map[x,y]=value;


            }
        }

        return this;
    }


    /**
     * @deprecated
     */
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

                value*=Mathf.Max(0.0f, Mathf.Min((maxSqr-distSqr)/maxSqr,1.0f));
                map[x,y]=value;


            }
        }

        return this;
    }


    public StyleMap Blur(int radius, int strength){

    	this.map=(new MapBlur()).Blur(map, radius, strength);


    	return this;
    }

    /**
     * @deprecated
     */
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




        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {

                map[x, y]=map2[x, y];
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


        Debug.Log("Perlin tile: x"+seedIndexOffsetX+", y"+seedIndexOffsetY);

        int width=GetWidth();
        int height=GetHeight();

        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {

                float value=map[x,y];

                float perlin=Mathf.PerlinNoise(((x+seedIndexOffsetX+seedValue)*perlinConst)/(width*size), ((y+seedIndexOffsetY+seedValue)*perlinConst)/(height*size));
                float noise=(perlin+offset)*scale;

                value+=noise;
                map[x, y]=value;

            }
        }

        return this;

    }
    public StyleMap MultPerlinNoise(float size, float scale, float offset){

        Debug.Log("Perlin tile: x"+seedIndexOffsetX+", y"+seedIndexOffsetY);

        int width=GetWidth();
        int height=GetHeight();

        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {

                float value=map[x,y];

                float perlin=Mathf.PerlinNoise(((x+seedIndexOffsetX+seedValue)*perlinConst)/(width*size), ((y+seedIndexOffsetY+seedValue)*perlinConst)/(height*size));
                float noise=(perlin+offset)*scale;

                value*=noise;
                map[x, y]=value;

            }
        }

        return this;

    }


    public StyleMap FilterSobel(){


        int width=GetWidth();
        int height=GetHeight();

        float[,] map2=new float[width, height];

        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {

                float slope=0;
                int count=0;
                for(int i=y-1; i<=y+1; i++){
                    if(i>=0&&i<height){
                        for(int j=x-1; j<=x+1; j++){
                            if(j>=0&&j<width&&(i!=x||j!=y)){
                                slope =Mathf.Max(slope, Mathf.Abs(map[j,i] - map[x, y]));
                                count++;
                            }
                        }
                    }
                }

                //Debug.Log("x:"+x+", y:"+y+"= "+slope+" ("+count+")");

                map2[x, y]=slope;
                
            }
        }

        map=map2;

        return this;

    }


    public static StyleMap ForTerrainTexture(Terrain t){

    	return new StyleMap(t.terrainData.alphamapResolution, t.terrainData.alphamapResolution);
    }

    public static StyleMap ForTerrainDetail(Terrain t){

    	return new StyleMap(t.terrainData.detailResolution, t.terrainData.detailResolution);
    }

    public static StyleMap ForTerrainHeight(Terrain t){

    	return (new StyleMap(t.terrainData.heightmapResolution, t.terrainData.heightmapResolution));
    }



}