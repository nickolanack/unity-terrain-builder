using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public class BaseNode : Node
{
    protected string nodeGuid;
    protected ProceduralGraphView graphView;
    protected ProceduralEditor editorWindow;
    protected Vector2 defaultNodeSize = new Vector2(200, 250);

    public string NodeGuid { get => nodeGuid; set => nodeGuid = value; }

    public BaseNode()
    {
        StyleSheet styleSheet = Resources.Load<StyleSheet>("NodeStyleSheet");
        styleSheets.Add(styleSheet);
    }




    public Port AddOutputPort(string name, Port.Capacity capacity = Port.Capacity.Single)
    {
        Port outputPort = GetPortInstance(Direction.Output, capacity);
        outputPort.portName = name;
        outputContainer.Add(outputPort);
        return outputPort;
    }

    /// <summary>
    /// Add a port to the inputContainer.
    /// </summary>
    /// <param name="name">The name of port.</param>
    /// <param name="capacity">Can it accept multiple or a single one.</param>
    /// <returns>Get the port that was just added to the inputContainer.</returns>
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




    public  List<BaseNode> PreviousNodes(){

        List<BaseNode> list=new List<BaseNode>();


         foreach(Edge edge in graphView.edges){
 
            if(edge.input.node==this){
                list.Add((BaseNode)edge.output.node);
            }
         }
        return list;    

    }

    public  List<BaseNode> NextNodes(){

        List<BaseNode> list=new List<BaseNode>();


         foreach(Edge edge in graphView.edges){

            if(edge.output.node==this){
                list.Add((BaseNode)edge.input.node);
            }
         }
        return list;    

    }

}



// using System.Collections;
        // using System.Collections.Generic;
        // using UnityEngine;


        [System.Serializable]
        public class BaseData
        {
            public string NodeGuid;
            public Vector2 Position;
        }
