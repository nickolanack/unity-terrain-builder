using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;


public class MapResize{
    
   
    
    public float[,] Resize(float[,] map, int width, int height){


        if(width>map.GetLength(0)&&height>map.GetLength(1)){
            return Expand(map, width, height);
        }

    	return ResizeY(ResizeX(map, width), height);

    }


    float[,] Expand(float[,] map, int width, int height){


        float[,] mout = new float[width, height];


        float scaleY=map.GetLength(1)/(float)height;
        float scaleX=map.GetLength(0)/(float)width;

        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {

                mout[x,y]=map[(int)(x*scaleX), (int)(y*scaleY)];
            }
        }

        return mout;

    }



     float[,] ResizeY(float[,] map, int height){

    	
        if(height>map.GetLength(1)){
    		throw new Exception("Cannot up-sample");
    	}

    	int width=map.GetLength(0);

        float[,] mout = new float[width, height];

        float scaleY=map.GetLength(1)/(float)height;
        int scaleYFloor=(int)(scaleY);
        float ry=scaleY%scaleYFloor;



        float avg=0;

        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {

                avg=0;
                
                for(int sy=0; sy<scaleYFloor;sy++){
                    avg+=map[x,scaleYFloor*y+sy];
                }

                if(y+scaleYFloor<height){
                	avg+=map[x,scaleYFloor*y+scaleYFloor]*ry;
                	mout[x,y]=avg/scaleY;
            	}else{
            		mout[x,y]=avg/scaleYFloor;
            	}

            }
        }

        return mout;


    }



     float[,] ResizeX(float[,] map, int width){

    	if(width>map.GetLength(0)){
    		throw new Exception("Cannot up-sample");
    	}

        int height=map.GetLength(1);
        float[,] mout = new float[width, height];


        float scaleX=map.GetLength(0)/(float)width;
        int scaleXFloor=(int)(scaleX);
        float rx=scaleX%scaleXFloor;


        Debug.Log("scaleX:"+scaleX+", scaleXFloor:"+scaleXFloor+", rx:"+rx);

        float avg=0;

        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++)
            {

                avg=0;
                
                for(int sx=0;sx<scaleXFloor;sx++){
                    avg+=map[scaleXFloor*x+sx,y];
                }

                if(x+scaleXFloor<width){
               
            		avg+=map[scaleXFloor*x+scaleXFloor,y]*rx;
            		mout[x,y]=avg/scaleX;
            	
            	}else{
            		mout[x,y]=avg/scaleXFloor;
            	}

            }
        }


        return mout;


    }

    
}