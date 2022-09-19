using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


    public class PerlinNoiseNode : BaseNode
    {
        


        
        float size=5;
        float scale=1;
        float offset=0;

        float seed=0;

        int octaves=0;

        string mergeOperation="add-all"; 
        AnimationCurve octaveCurve;

        bool normalize;

    
        public PerlinNoiseNode() :base() { }
        public PerlinNoiseNode(Vector2 position, ProceduralEditor editorWindow, ProceduralGraphView graphView) :base(position, editorWindow, graphView){}

        public override BaseNode Instantiate(){
            return new PerlinNoiseNode();
        }


        protected override void AddStyleSheets()
        {
            AddStyleSheet("PerlinNoiseStyleSheet");
        }

        public override string GetTitle(){
            return "Perlin Noise";
        }


        protected override void AddPorts()
        {
            AddOutputPort("Out", Port.Capacity.Multi);
        }

        protected override void AddFields()
        {
           

            octaveCurve=AnimationCurve.Constant(0, 1, 1.0f);

            (new NodeField(this)).AddFloatValue("Size", ()=>{ return size; }, (value)=>{ size=value; });
            (new NodeField(this)).AddFloatValue("Scale", ()=>{ return scale; }, (value)=>{ scale=value; });
            (new NodeField(this)).AddFloatValue("Offset", ()=>{ return offset; }, (value)=>{ offset=value; });
            (new NodeField(this)).AddFloatValue("Seed", ()=>{ return seed; }, (value)=>{ seed=value; });
            (new NodeField(this)).AddIntegerValue("Octaves", ()=>{ return octaves; }, (value)=>{ octaves=value; });
            (new NodeField(this)).AddDropDownListValue("Merge Operation", new List<string>(){"add", "multiply", "mask-add"}, ()=>{ return mergeOperation; }, (value)=>{ mergeOperation=value; });

            (new NodeField(this)).AddAnimationCurveValue("Octave Scale Curve", ()=>{ return octaveCurve; }, (value)=>{ octaveCurve=value; });


            (new NodeField(this)).AddToggleValue("Normalize", ()=>{ return normalize; }, (value)=>{ normalize=value; });
           

            new StyleMapPreview(this);
         
        }

       


        public void OnUpdated() 
        { 
            base.UpdatedData();
        }



        public override BaseData GetNodeData()
        {
            PerlinNoiseData nodeData = new PerlinNoiseData()
            {
                NodeGuid = NodeGuid,
                Position = GetPosition().position,
                Size = size,
                Scale = scale,
                Offset = offset, 
                Octaves = octaves,
                MergeOperation = mergeOperation,
                OctaveCurve = octaveCurve,
                Seed = seed,
                Normalize = normalize
            };

            return nodeData;
        }

        public override BaseNode SetData(BaseData data)
        {

            PerlinNoiseData perlinNoiseData=(PerlinNoiseData) data;


            size=perlinNoiseData.Size;
            scale=perlinNoiseData.Scale;
            offset=perlinNoiseData.Offset;
            seed=perlinNoiseData.Seed;
            octaves=perlinNoiseData.Octaves;
            mergeOperation=perlinNoiseData.MergeOperation;
            octaveCurve=perlinNoiseData.OctaveCurve;
            normalize=perlinNoiseData.Normalize;

            base.SetData();
            return this;

        }
           
    }


[System.Serializable]
public class PerlinNoiseData : BaseData
{
    public float Size;
    public float Scale;
    public float Offset;
    public float Seed;
    public int Octaves;
    public AnimationCurve OctaveCurve;
    public string MergeOperation="add";
    public bool Normalize;


    private float OctaveScale(int i){

        if(OctaveCurve==null){
            return 1;
        }

        float v= OctaveCurve.Evaluate(i/(Octaves+1.0f));

        //Debug.Log("Curve("+(i/(Octaves+1))+"): "+v);

        return v;


        // if(MergeOperation.Equals("multiply")){
        //     return 1.0f;
        // }

        // return 1.0f/(Octaves+1);

    }


    private float OctaveSize(int i){
        return Mathf.Pow(2,i);
    }


    public override StyleMap GetStyleMap(StyleMap input, List<StyleMap> inputs){

        StyleMap style=new StyleMap(input);

        style.SetSeed((int)Seed*100);
 
        for(int i=0;i<=Octaves;i++){
            if(MergeOperation.Equals("multiply")&&i>0){
                style.MultPerlinNoise(OctaveSize(i)*Size, Scale*OctaveScale(i), Offset);
                continue;
            }

            if(MergeOperation.Equals("mask-add")&&i>0){
                style.MultPerlinNoise(OctaveSize(i)*Size, 1, Offset);
                //continue; //also want to add after each octave
            }
            style.AddPerlinNoise(OctaveSize(i)*Size, Scale*OctaveScale(i), Offset);
        }
        if(Normalize){
            style.Normalize();
        }
        return style;

     }

     public override BaseNode InstantiateNode(){
        return new PerlinNoiseNode();
    }
}


