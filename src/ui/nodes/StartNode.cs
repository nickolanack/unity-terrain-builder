using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class StartNode : BaseNode
{
    public StartNode() { }

    public StartNode(Vector2 position, ProceduralEditor editorWindow, ProceduralGraphView graphView)
    {
        base.editorWindow = editorWindow;
        base.graphView = graphView;

        StyleSheet styleSheet = Resources.Load<StyleSheet>("StartNodeStyleSheet");
        styleSheets.Add(styleSheet);

        title = "Terrain Data";
        SetPosition(new Rect(position, defaultNodeSize));
        nodeGuid = Guid.NewGuid().ToString();

        AddInputPort("HeightMap", Port.Capacity.Single);

        RefreshExpandedState();
        RefreshPorts();
    }




    public StartData GetNodeData()
    {
        StartData nodeData = new StartData()
        {
            NodeGuid = NodeGuid,
            Position = GetPosition().position,
        };

        return nodeData;
    }


    public StartNode SetData(StartData data)
    {
            return this;
    }
}



[System.Serializable]
public class StartData : BaseData
{


    


}