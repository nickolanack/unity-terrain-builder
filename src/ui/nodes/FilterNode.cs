using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


    public class FilterNode : BaseNode
    {
        


        
        public bool filterSobel=true;

        
        public FilterNode() :base() { }
        public FilterNode(Vector2 position, ProceduralEditor editorWindow, ProceduralGraphView graphView) :base(position, editorWindow, graphView){}

        public override BaseNode Instantiate(){
            return new FilterNode();
        }
     
        protected override void AddStyleSheets()
        {
            AddStyleSheet("EventNodeStyleSheet");
        }

        public override string GetTitle(){
            return "Sobel Filter";
        }

        protected override void AddPorts()
        {
            AddInputPort("Stylemap", Port.Capacity.Single);
            AddOutputPort("Out", Port.Capacity.Multi);

        }

        protected override void AddFields()
        {


            //(new NodeField(this)).AddFloatValue("Filter Type", ()=>{ return filter; }, (value)=>{ filter=value; });
            

            new StyleMapPreview(this);
           
        }


    

        public override BaseData GetNodeData()
        {
            FilterData nodeData = new FilterData()
            {
                NodeGuid = NodeGuid,
                Position = GetPosition().position,
     
            };

            return nodeData;
        }


        public override BaseNode SetData(BaseData data)
        {

            FilterData filterData=(FilterData) data;

            base.SetData();
            return this;

        }
           
    }


[System.Serializable]
public class FilterData : BaseData
{


    public bool FilterSobel=true;
    public override StyleMap GetStyleMap(StyleMap input, List<StyleMap> inputs){

        StyleMap map = new StyleMap(input.GetWidth(), input.GetHeight(), 0);

        foreach(StyleMap style in inputs){
           
            map.Add(style); 
            break;
        }

       return map.FilterSobel().Normalize();

        
    }

    public override BaseNode InstantiateNode(){
        return new FilterNode();
    }
}


