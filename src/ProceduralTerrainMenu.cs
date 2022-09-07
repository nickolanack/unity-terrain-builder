//C# Example (LookAtPoint.cs)
using UnityEngine;
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
    public int textureDirt=3;
    public int textureSand=4;
    public int textureSandAlt=5;
    public int textureGravel=6;



    public bool enableDetail=true;

    public Terrain neighbour;

     public void Reset()
    {
        Terrain t=GetComponent<Terrain>();
        TerrainEdit editor=new TerrainEdit();
        
        if(enableHeight){
            editor.ResetHeight(t, 0);
            editor.ResetHeight(neighbour, 0);
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


        if(enableHeight){
            GenerateHeight();
        }

        if(enableWater){
            GenerateWater();
        }


        if(enableTexture){
            GenerateTextures();
        }


        if(enableDetail){
            GenerateDetail();
        }

    }


    public void ApplyScriptableObject(ProceduralGraphObject flow){


       Debug.Log(flow.StartDatas[0]);


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



    public void GenerateTextures(){

        Terrain t=GetComponent<Terrain>();
        TerrainEdit editor=new TerrainEdit();
        Vector3 pos=editor.AtCenter(t);


       
        StyleMap heightMap=StyleMap.ForTerrainTexture(t)
            .SetValues(editor.GetHeight(t)) //auto resizes
            .Normalize();


         
        editor.DrawTexture(pos, t,  new TerrainStyle[] {new TerrainStyle(heightMap, textureDirt)});
        
       
        StyleMap beachMask=StyleMap.ForTerrainTexture(t)
            .SetValues(heightMap.Get())
            .MaskLessThanTerrainHeight(waterLevel+6f, t)
            .Blur(4, 2);


        StyleMap landMask=StyleMap.ForTerrainTexture(t)
            .SetValues(beachMask.Get())
            .Invert();
         


        StyleMap sand=StyleMap.ForTerrainTexture(t)
            .SetValues(beachMask.Get());

        StyleMap sand2=StyleMap.ForTerrainTexture(t)
            .SetValues(beachMask.Get())
            .Mult(
                StyleMap.ForTerrainTexture(t)
                    .AddPerlinNoise(0.5f, 0.8f, -0.2f)
                    .AddPerlinNoise(3f, 0.8f, -0.2f)
                    .Normalize()
                    .Blur(2, 1)
            );

        editor.DrawTexture(pos, t,  new TerrainStyle[] {
            new TerrainStyle(sand, textureSand),
            new TerrainStyle(sand2, textureSandAlt)
        });


        
       StyleMap altGrass= StyleMap.ForTerrainTexture(t)
            .AddPerlinNoise(0.5f, 1f, -0.4f)
            .Add( 
                StyleMap.ForTerrainTexture(t)
                    .AddPerlinNoise(2f, 1f, -0.3f)
            )
            .Add( 
                StyleMap.ForTerrainTexture(t)
                    .AddPerlinNoise(10, 1f, -0.3f)
            )
            .Normalize()
            .Mult(landMask);
        




        
        StyleMap weedGrass= StyleMap.ForTerrainTexture(t)
            .AddPerlinNoise(7, 0.2f, -0.1f)
            .AddPerlinNoise(5, 0.2f, -0.1f);
   

        StyleMap slopeMask=StyleMap.ForTerrainTexture(t)
            .Then(delegate(StyleMap slope){
                slope.SetValues(editor.GetSlope(t, slope.GetWidth(), slope.GetHeight()));
            })
            .Normalize();
        
        StyleMap gravel=StyleMap.ForTerrainTexture(t)
            //.SetValues(1)
            .AddPerlinNoise(7, 0.3f, 0)
            .AddPerlinNoise(2, 0.3f, 0)
            .Add(0.3f)
            //.Normalize()
            .Mult(slopeMask);

                
         


        // Vector3 pos=editor.AtCenter(t);
         editor.DrawTexture(pos, t,  new TerrainStyle[] {
            new TerrainStyle(weedGrass, textureWeedGrass), 
            new TerrainStyle(altGrass,textureAltGrass),
            new TerrainStyle(gravel,textureGravel),
            
            /*, tex2, tex3*/});


    }



    public void GenerateHeight(){

        Terrain t=GetComponent<Terrain>();


        int x=t.terrainData.heightmapResolution*Random.Range(0, 1000);
        int y=t.terrainData.heightmapResolution*Random.Range(0, 1000);


        GenerateTerrainHeight(t, x, y);
        GenerateTerrainHeight(neighbour, x-1000*t.terrainData.heightmapResolution, y);

    }


    void GenerateTerrainHeight(Terrain t, int x, int y){
         
        
        TerrainEdit editor=new TerrainEdit();
    


        StyleMap style=StyleMap.ForTerrainHeight(t)
                    .SetNoiseOffset(x, y)
                    .AddPerlinNoise(scale0, 1f, 0f)
                    .Then(delegate(StyleMap map){

                        StyleMap.ForTerrainHeight(t)
                            .SetNoiseOffset(x, y)
                            .AddPerlinNoise(scale1, 1f, - flattenBottom)
                            .Normalize()
                            .Then(delegate(StyleMap tmp){
                                 map.Mult(tmp)
                                    .Scale(ratio);
                            })
                            .Scale(1-ratio)
                            .Then(delegate(StyleMap tmp){
                                 map.Add(tmp);
                            });
                    })
                    .ScaleTerrainHeight(height, t);
                    // .Add(
                    //     StyleMap.ForTerrainHeight(t)
                    //         .AddPerlinNoise(3f, 1f, 0)
                    //         .Mult(
                    //             StyleMap.ForTerrainHeight(t)
                    //                 .AddPerlinNoise(25f, 1f, -0.7f)
                    //                 .Normalize()
                    //         )
                    //         .ScaleTerrainHeight(height/5, t)
                    // )
                    //.Blur(5,2)
                    

                    //.Get()




        Vector3 pos=editor.AtCenter(t);
        editor.DrawHeight(pos, t, new TerrainStyle[] {
            new TerrainStyle(style)
        });
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