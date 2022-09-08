using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


    public class AddNode : BaseNode
    {
        


        

        bool normalize=false;
        bool subtract=false;

        public AddNode() { }

        public AddNode(Vector2 position, ProceduralEditor editorWindow, ProceduralGraphView graphView) :base(position, editorWindow, graphView, "Addition/Subtraction")
        {
            
            AddStyleSheet("EventNodeStyleSheet");
           
        }

        protected override void AddPorts()
        {
            AddInputPort("Stylemaps", Port.Capacity.Multi);
            AddOutputPort("Out", Port.Capacity.Multi);
        }

        protected override void AddFields()
        {


            (new NodeField(this)).AddToggleValue("Normalize", ()=>{ return normalize; }, (value)=>{ normalize=value; });
            (new NodeField(this)).AddToggleValue("Subract", ()=>{ return subtract; }, (value)=>{ subtract=value; });
            new StyleMapPreview(this);
           
        }

    
        public override BaseData GetNodeData()
        {
            AddData nodeData = new AddData()
            {
                NodeGuid = NodeGuid,
                Position = GetPosition().position,
                Normalize = normalize,
                Subtract = subtract
            };

            return nodeData;
        }


        public AddNode SetData(AddData data)
        {

            normalize=data.Normalize;
            subtract=data.Subtract;

            base.SetData();
            return this;

        }
           
    }


[System.Serializable]
public class AddData : BaseData
{

    public bool Normalize;
    public bool Subtract;



    public override StyleMap GetStyleMap(StyleMap input, List<StyleMap> inputs){
       
        StyleMap map= new StyleMap(input.GetWidth(), input.GetHeight(), 0);


       

        bool sub=false;
        foreach(StyleMap style in inputs){

            if(sub){
                 Debug.Log(NodeGuid+" Subtract map");
                 map.Subtract(style);
            }else{
                Debug.Log(NodeGuid+" Add map");
                map.Add(style);
                if(Subtract){
                    sub=true;
                } 
            }
            
        }
        if(Normalize){
            map.Normalize();
        }

        map.Clamp(0,1);

        return map;
    }

    
}


