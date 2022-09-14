using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


    public class ClampNode : BaseNode
    {
        


        

        float min=-1;
        float max=1;
        bool prenormalize=true;
        bool invert=false;
        bool scale=true;

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
            return "Clamp/Scale";
        }

        protected override void AddPorts()
        {
            AddInputPort("Stylemap", Port.Capacity.Single);
            AddOutputPort("Out", Port.Capacity.Multi);

        }

        protected override void AddFields()
        {


            (new NodeField(this)).AddFloatValue("Min", ()=>{ return min; }, (value)=>{ min=value; });
            (new NodeField(this)).AddFloatValue("Max", ()=>{ return max; }, (value)=>{ max=value; });
            (new NodeField(this)).AddToggleValue("Pre-normalize", ()=>{ return prenormalize; }, (value)=>{ prenormalize=value; });
            (new NodeField(this)).AddToggleValue("Invert", ()=>{ return invert; }, (value)=>{ invert=value; });
            (new NodeField(this)).AddToggleValue("Scale", ()=>{ return scale; }, (value)=>{ scale=value; });

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
                Prenormalize = prenormalize, 
                Invert = invert,
                Scale = scale
            };

            return nodeData;
        }


        public override BaseNode SetData(BaseData data)
        {

            ClampData clampData=(ClampData) data;

            min=clampData.Min;
            max=clampData.Max;
            prenormalize=clampData.Prenormalize;
            invert=clampData.Invert;
            scale=clampData.Scale;

            base.SetData();
            return this;

        }
           
    }


[System.Serializable]
public class ClampData : BaseData
{

    public float Min;
    public float Max;

    public bool Prenormalize;
    public bool Invert;
    public bool Scale;


    public override StyleMap GetStyleMap(StyleMap input, List<StyleMap> inputs){




        StyleMap map = new StyleMap(input.GetWidth(), input.GetHeight(), 0);

        foreach(StyleMap style in inputs){
           
           	map.Add(style);	


           	if(!Scale){

    	 		if(Prenormalize){
        	 		map.Normalize();
        	 	}

        	 	if(Invert){
	        	 	map.Invert();
	        	}

    	 		map.Clamp(Min, Max);

    	 		return map;
    	 	}



           	if(Prenormalize){
        		map.Scale(Max-Min);
        	}else{
        		map.Mult(Max-Min);
        	}
        	map.Add(Min);


        	if(Invert){
        	 	map.Invert();
        	}


            map.Clamp(0, 1);
        	return map;
        }

        return map;
    }

    public override BaseNode InstantiateNode(){
        return new ClampNode();
    }
}


