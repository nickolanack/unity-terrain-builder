using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;



    public class AddNode : BaseNode
    {
        


        

        bool normalize=false;

        string operation="add";


        public string title="Combine";

        public AddNode() :base() { }
        public AddNode(Vector2 position, ProceduralEditor editorWindow, ProceduralGraphView graphView) :base(position, editorWindow, graphView){}

        public override BaseNode Instantiate(){
            return new AddNode();
        }

        protected override void AddStyleSheets()
        {
            AddStyleSheet("MathNodeStyleSheet");
        }

        public override string GetTitle(){
            return "Arithmetic";
        }

        protected override void AddPorts()
        {
            AddInputPort("Stylemaps", Port.Capacity.Multi);
            AddOutputPort("Out", Port.Capacity.Multi);
        }

        protected override void AddFields()
        {



            (new NodeField(this)).AddToggleValue("Normalize out", ()=>{ return normalize; }, (value)=>{ normalize=value; });
            (new NodeField(this)).AddDropDownListValue("Operation", new List<string>(){"add","subtract", "multiply"}, ()=>{ return operation; }, (value)=>{ operation=value; });

            new StyleMapPreview(this);
           
        }

    
        public override BaseData GetNodeData()
        {
            AddData nodeData = new AddData()
            {
                NodeGuid = NodeGuid,
                Position = GetPosition().position,
                Normalize = normalize,
                Operation = operation
            };

            return nodeData;
        }


        public override BaseNode SetData(BaseData data)
        {

            AddData addData=(AddData) data;

            normalize=addData.Normalize;
            operation=addData.Operation;

            base.SetData();
            return this;

        }
           
    }


[System.Serializable]
public class AddData : BaseData
{

    public bool Normalize;
    public string Operation;



    public override StyleMap GetStyleMap(StyleMap input, List<StyleMap> inputs){
       
        StyleMap map= new StyleMap(input);


       

        bool first=true;
        foreach(StyleMap style in inputs){

            if(first){
                 map.Add(style);
                 first=false;
                 continue;
            }else{

                if(Operation.Equals("add")){
                    map.Add(style);
                    continue;
                } 

                if(Operation.Equals("subtract")){
                    map.Subtract(style);
                    continue;
                } 

                 if(Operation.Equals("multiply")){
                    map.Mult(style);
                    continue;
                } 
               
            }
            
        }
        if(Normalize){
            map.Normalize();
        }

        //map.Clamp(0,1);

        return map;
    }

    public override BaseNode InstantiateNode(){
        return new AddNode();
    }

    
}


