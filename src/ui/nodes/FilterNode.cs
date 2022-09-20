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
        public string filter="sobel";
        public AnimationCurve fadeCurve;

        
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
            return "Filter";
        }

        protected override void AddPorts()
        {
            AddInputPort("Stylemap", Port.Capacity.Single);
            AddOutputPort("Out", Port.Capacity.Multi);

        }

        protected override void AddFields()
        {


           

            (new NodeField(this)).AddDropDownListValue("Filter", new List<string>(){"sobel","radial-fade"}, ()=>{ return filter; }, (value)=>{ filter=value; });
            (new NodeField(this)).AddAnimationCurveValue("FadeCurve", ()=>{ return fadeCurve; }, (value)=>{ fadeCurve=value; });

            new StyleMapPreview(this);
           
        }


    

        public override BaseData GetNodeData()
        {
            FilterData nodeData = new FilterData()
            {
                NodeGuid = NodeGuid,
                Position = GetPosition().position,
                Filter = filter,
                FadeCurve = fadeCurve

            };

            return nodeData;
        }


        public override BaseNode SetData(BaseData data)
        {

            FilterData filterData=(FilterData) data;
            filter = filterData.Filter;
            fadeCurve = filterData.FadeCurve;

            return this;

        }
           
    }


[System.Serializable]
public class FilterData : BaseData
{

    public string Filter="sobel";
    public AnimationCurve FadeCurve;

    public override StyleMap GetStyleMap(StyleMap input, List<StyleMap> inputs){

        StyleMap map = new StyleMap(input);

        foreach(StyleMap style in inputs){
           
            map.Add(style); 
            break;
        }

        if(Filter.Equals("sobel")){
            return map.FilterSobel();
        }

        if(Filter.Equals("radial-fade")){

            if(FadeCurve==null){
                 FadeCurve=AnimationCurve.Linear(0, 0, 1, 1);
            }

            return map.RadialFade(FadeCurve);
        }

       return map;
        
    }

    public override BaseNode InstantiateNode(){
        return new FilterNode();
    }
}


