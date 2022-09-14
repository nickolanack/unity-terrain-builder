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
           

            (new NodeField(this)).AddFloatValue("Size", ()=>{ return size; }, (value)=>{ size=value; });
            (new NodeField(this)).AddFloatValue("Scale", ()=>{ return scale; }, (value)=>{ scale=value; });
            (new NodeField(this)).AddFloatValue("Offset", ()=>{ return offset; }, (value)=>{ offset=value; });
            (new NodeField(this)).AddFloatValue("Seed", ()=>{ return seed; }, (value)=>{ seed=value; });
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
                Seed = seed
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
    public bool Normalize;


    public override StyleMap GetStyleMap(StyleMap input, List<StyleMap> inputs){

        StyleMap style=new StyleMap(input.GetWidth(), input.GetHeight(), 0);

        style.SetSeed((int)Seed*100);
        style.AddPerlinNoise(Size, Scale, Offset);
        if(Normalize){
                style.Normalize();
        }
        return style;

     }

     public override BaseNode InstantiateNode(){
        return new PerlinNoiseNode();
    }
}


