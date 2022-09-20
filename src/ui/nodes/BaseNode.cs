using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public abstract class BaseNode : Node
{
    protected string nodeGuid;
    protected ProceduralGraphView graphView;
    protected ProceduralEditor editorWindow;
    protected Vector2 defaultNodeSize = new Vector2(150, 150);


    public event Action OnSetData = delegate { };
    public event Action OnUpdatedData = delegate { };

    public string NodeGuid { get => nodeGuid; set => nodeGuid = value; }


    public BaseNode()
    {
       
    }



    public virtual string GetTitle(){
        return "Node Name";
    }

    public void AddStyleSheet(string name){

        StyleSheet styleSheet = Resources.Load<StyleSheet>(name);
        if(styleSheet!=null){
            styleSheets.Add(styleSheet);
        }
    }

    public BaseNode DrawNode(Vector2 position, ProceduralEditor editorWindow, ProceduralGraphView graphView) 
    {
        AddStyleSheet("NodeStyleSheet");   

        this.editorWindow = editorWindow;
        this.graphView = graphView;

        title = GetTitle();


        SetPosition(new Rect(position, defaultNodeSize));   // Set Position
        nodeGuid = Guid.NewGuid().ToString();               // Set Guid ID

        AddPorts();
        AddFields();

        RefreshExpandedState();
        RefreshPorts();

        AddStyleSheets();

        return this;
    }



    public BaseNode(Vector2 position, ProceduralEditor editorWindow, ProceduralGraphView graphView) : this()
    {
        DrawNode(position, editorWindow, graphView);
    }

    protected virtual void AddFields()
    {

    }

    protected virtual void AddStyleSheets()
    {

    }

    protected virtual void AddPorts()
    {
    }

    public Port AddOutputPort(string name, Port.Capacity capacity = Port.Capacity.Single)
    {
        Port outputPort = GetPortInstance(Direction.Output, capacity);
        outputPort.portName = name;
        outputContainer.Add(outputPort);
        return outputPort;
    }

 
    public Port AddInputPort(string name, Port.Capacity capacity = Port.Capacity.Multi)
    {
        Port inputPort = GetPortInstance(Direction.Input, capacity);
        inputPort.portName = name;
        inputContainer.Add(inputPort);
        return inputPort;
    }


    public Port GetPortInstance(Direction nodeDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(StyleMap));
    }


    public virtual void LoadValueInToField()
    {

    }

    public StyleMap GetStyleMapOut(){
        return GetNodeData().GetStyleMap(new StyleMap(200,150,0),  InputNodeStyleMaps());
    }


     public StyleMap GetStyleMapOut(StyleMap templateMap){
        return GetNodeData().GetStyleMap(templateMap,  InputNodeStyleMaps());
    }



    public abstract BaseNode Instantiate();

    public abstract BaseData GetNodeData();



    public void SetData(){
        OnSetData();
    }

    public abstract BaseNode SetData(BaseData data);

    public void UpdatedData(){
       
        OnUpdatedData();

        foreach(BaseNode node in OutputNodes()){
           node.UpdatedData();  
        }
    }


    public  List<BaseNode> InputNodes(){

        List<BaseNode> list=new List<BaseNode>();


         foreach(Edge edge in graphView.edges){
 
            if(edge.input.node==this){
                list.Add((BaseNode)edge.output.node);
            }
         }
        return list;    

    }


     public  List<StyleMap> InputNodeStyleMaps(){

        List<StyleMap> list=new List<StyleMap>();


         foreach(BaseNode node in InputNodes()){
            list.Add(node.GetStyleMapOut());
         }
        return list;    

    }

    public  List<BaseNode> OutputNodes(){

        List<BaseNode> list=new List<BaseNode>();


         foreach(Edge edge in graphView.edges){

            if(edge.output.node==this){
                list.Add((BaseNode)edge.input.node);
            }
         }
        return list;    

    }

}


[System.Serializable]
public abstract class BaseData
{
    public string NodeGuid;
    public Vector2 Position;


    private StyleMap _CacheMap;
    private String _CacheSettingsString;


    public virtual StyleMap GetStyleMap(StyleMap input, List<StyleMap> inputs){
        return input;
    }

    public StyleMap GetStyleMap(StyleMap input, ProceduralGraphObject graph, string portName){
       return graph.GetInputsTo(NodeGuid, portName)[0].GetStyleMap(input, graph);
    }




    private StyleMap GetStyleMap(StyleMap input, ProceduralGraphObject graph){

        List<StyleMap> inputs =new List<StyleMap>();
    
        //StyleMap style= new StyleMap(input.GetWidth(), input.GetHeight(), 0);
        input.SetTileXY(graph.GetTileX(), graph.GetTileY());


        foreach(BaseData data in graph.GetInputsTo(NodeGuid)){
            inputs.Add(data.GetStyleMap(input, graph));
        }

        if(graph.GetOutputsFrom(NodeGuid).Count>1){

            //If multiple nodes are connected to the output of this node then it we will store a cached
            //copy for the next request (which should occur for each output)

            if(_CacheMap!=null){
               if(_CacheSettingsString.Equals(input.SettingsID())){
                    Debug.Log("Used Cache: "+_CacheMap.ID()+" "+_CacheSettingsString);
                    return _CacheMap;
                }
                Debug.Log("Cleared Cache: "+_CacheMap.ID()+" "+input.SettingsID()+" ("+_CacheSettingsString+")");
            }
            _CacheMap=GetStyleMap(input, inputs);
            _CacheSettingsString=input.SettingsID();
            Debug.Log("Create Cache: "+_CacheMap.ID()+" "+_CacheSettingsString);
            return _CacheMap;
        }

        return GetStyleMap(input, inputs);

    }

    public abstract BaseNode InstantiateNode();


}
