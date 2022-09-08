//C# Example (LookAtPoint.cs)
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ProceduralTerrainMenu : MonoBehaviour
{


    public ProceduralGraphObject proceduralTerrainFlow;


    public bool enableHeight=true;

    [Range(10f, 80f)]
    public float height=50;

    [Range(1f, 100f)]
    public float scale0=5f;

    [Range(1f, 100f)]
    public float scale1=25f;

    [Range(0.1f, 0.9f)]
    public float ratio=0.4f;


    [Range(0.1f, 0.9f)]
    public float flattenBottom=0.3f;
    

  
    public bool enableWater=false;

   


    [Range(0f, 50f)]
    public float waterLevel=-1;


    public GameObject waterPrefab;


    public bool enableTexture=true;

    public int textureGrass=0;
    public int textureAltGrass=1;
    public int textureWeedGrass=2;
 

    public bool enableDetail=true;

    public Terrain neighbour;

     public void Reset()
    {
        Terrain t=GetComponent<Terrain>();
        TerrainEdit editor=new TerrainEdit();
        
        if(enableHeight){
            editor.ResetHeight(t, 0);
        }
        
        if(enableTexture){
            editor.ResetTextures(t);
        }


        if(enableDetail){
            editor.ResetDetails(t);
        }


        if(enableWater){
           Transform water = transform.Find("Water");
           if(water!=null){
                DestroyImmediate(water.gameObject);
           }
        }

    }

    public void ApplyProcedural()
    {


        if(proceduralTerrainFlow!=null){

            ApplyScriptableObject(proceduralTerrainFlow);
            return;
        }


        if(enableWater){
            GenerateWater();
        }


        if(enableDetail){
            GenerateDetail();
        }

    }


    public void ApplyScriptableObject(ProceduralGraphObject flow){

        TerrainEdit editor=new TerrainEdit();
        
        Terrain t= GetComponent<Terrain>();
        editor.ResetTextures(t);
        Vector3 pos=editor.AtCenter(t);

        foreach(OutputData terrainNodeData in flow.OutputDatas){

            if(flow.PortHasInputs(terrainNodeData, "HeightMap")){
                
                StyleMap height=terrainNodeData.GetStyleMap(StyleMap.ForTerrainHeight(t), flow, "HeightMap");
              
                Debug.Log("Generated Height Map");

                editor.DrawHeight(pos, t, new TerrainStyle[] {
                    new TerrainStyle(height)
                });
            }else{
                 Debug.Log("No Height Map");
            }


            List<TerrainStyle> styles=new List<TerrainStyle>();

            for(int index=0; index<=10; index++){

                string port="TerrainDetail"+index;

                if(flow.PortHasInputs(terrainNodeData, port)){
                    StyleMap texture1=terrainNodeData.GetStyleMap(StyleMap.ForTerrainTexture(t), flow, port);

                    Debug.Log("Generate Texture Map: "+port);

                    styles.Add(new TerrainStyle(texture1.FlipXY(), index));
                }

            }

            if(styles.Count>0){

                editor.DrawTexture(pos, t,  styles);
            }

        }




    }



    public void GenerateDetail(){


        Terrain t=GetComponent<Terrain>();
        TerrainEdit editor=new TerrainEdit();
        Vector3 pos=editor.AtCenter(t);



        StyleMap grassMapMask=StyleMap.ForTerrainDetail(t)
            .SetValues(editor.GetTextureLayer(t, textureGrass)); 


        StyleMap grassMapAltMask=StyleMap.ForTerrainDetail(t)
            .SetValues(editor.GetTextureLayer(t, textureAltGrass));
        

        StyleMap grassMapWeedsMask=StyleMap.ForTerrainDetail(t)
            .SetValues(editor.GetTextureLayer(t, textureAltGrass));      


        StyleMap mainGrass=new StyleMap(grassMapMask).Scatter(50);


        editor.DrawDetails(pos, t,  new TerrainStyle[] {new TerrainStyle(mainGrass, 9)}); 


    }





    public void GenerateWater(){

        Terrain t=GetComponent<Terrain>();
        TerrainEdit editor=new TerrainEdit();

        Transform water=transform.Find("Water");
        GameObject w=null;
        if(water==null){

            w=Instantiate(waterPrefab, transform);
            w.name="Water";
        }else{
            w=water.gameObject;
        }
 
        Vector3 p=t.terrainData.size/2;
        p.y=waterLevel;
        w.transform.localPosition=p;
        Vector3 s=t.terrainData.size/10;
        s.y=1;
        w.transform.localScale=s;


    }

}