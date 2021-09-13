using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TerrainStamp : MonoBehaviour
{
    
    public Texture2D texture;
    public float scale; //or weight
    public string textureName;


    public bool rotateWithCamera=true;


    bool loaded=false;



    public delegate Texture2D TextureModifier(Texture2D texture);
    public TextureModifier modifier;



    void Update(){


        if((!loaded)&&AssetLibrary.Library){
            AssetLibrary.Library.LoadTexture(textureName, delegate(Texture2D t){

                texture=t;

            });
            loaded=true;
        }

    }



    public Texture2D GetTexture(){

        Texture2D copyTexture= new Texture2D(texture.width, texture.height);
        copyTexture.SetPixels(texture.GetPixels());
        copyTexture.Apply();

        
        if(modifier!=null){
            copyTexture = modifier(copyTexture);
        }


        if(rotateWithCamera){



            
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
        

            Debug.Log("rotating: "+(Camera.main.transform.eulerAngles.y-90f));
            copyTexture = RotateTexture(rotateTexture, -Camera.main.transform.eulerAngles.y-90f);
        }

        
        return copyTexture;
    }

    public float GetScale(){
        return scale;
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
 

   


}
