using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


    public class ClampNode : BaseNode
    {
        


        float offset=0;
        float scale=1;


        bool invert=false;
        bool clamp=true;

        float min=0;
        float max=1;




        public ClampNode() :base() { }
        public ClampNode(Vector2 position, ProceduralEditor editorWindow, ProceduralGraphView graphView) :base(position, editorWindow, graphView){}

        public override BaseNode Instantiate(){
            return new ClampNode();
        }

        protected override void AddStyleSheets()
        {
            AddStyleSheet("EventNodeStyleSheet");
        }

        public override string GetTitle(){
            return "Offset/Scale/Clamp/Invert";
        }

        protected override void AddPorts()
        {
            AddInputPort("Stylemap", Port.Capacity.Single);
            AddOutputPort("Out", Port.Capacity.Multi);

        }

        protected override void AddFields()
        {

            (new NodeField(this)).AddFloatValue("Offset", ()=>{ return offset; }, (value)=>{ offset=value; });
            (new NodeField(this)).AddFloatValue("Scale", ()=>{ return scale; }, (value)=>{ scale=value; });

            (new NodeField(this)).AddToggleValue("Invert", ()=>{ return invert; }, (value)=>{ invert=value; });

             (new NodeField(this)).AddToggleValue("Clamp", ()=>{ return clamp; }, (value)=>{ clamp=value; });

            (new NodeField(this)).AddFloatValue("Min", ()=>{ return min; }, (value)=>{ min=value; });
            (new NodeField(this)).AddFloatValue("Max", ()=>{ return max; }, (value)=>{ max=value; });
           

            new StyleMapPreview(this);
           
        }


    

        public override BaseData GetNodeData()
        {
            ClampData nodeData = new ClampData()
            {
                NodeGuid = NodeGuid,
                Position = GetPosition().position,
                Min = min,
                Max = max,
                Invert = invert,
                Clamp = clamp,
                Scale = scale,
                Offset = offset

            };

            return nodeData;
        }


        public override BaseNode SetData(BaseData data)
        {

            ClampData clampData=(ClampData) data;

            min=clampData.Min;
            max=clampData.Max;
            invert=clampData.Invert;
            scale=clampData.Scale;
            offset=clampData.Offset;
            clamp=clampData.Clamp;

            base.SetData();
            return this;

        }
           
    }


[System.Serializable]
public class ClampData : BaseData
{




    public float Offset=0;
    public float Scale=1;

    public bool Invert;

    public float Min;
    public float Max;

    public bool Clamp;



    public override StyleMap GetStyleMap(StyleMap input, List<StyleMap> inputs){




        StyleMap map = new StyleMap(input);

        foreach(StyleMap style in inputs){
           
           	map.Add(style);	

            map.Add(Offset);
            map.Mult(Scale);

            if(Invert){
                map.Invert();
            }

            if(Clamp){
                map.Clamp(Min, Max);
            }

        	return map;
        }

        return map;
    }

    public override BaseNode InstantiateNode(){
        return new ClampNode();
    }
}


