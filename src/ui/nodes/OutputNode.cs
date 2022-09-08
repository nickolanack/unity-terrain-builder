using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class OutputNode : BaseNode
{
    public OutputNode() { }

    public OutputNode(Vector2 position, ProceduralEditor editorWindow, ProceduralGraphView graphView) :base(position, editorWindow, graphView,  "Terrain Data")
    {
        AddStyleSheet("OutputNodeStyleSheet");
    }


    protected override void AddPorts()
    {

        AddInputPort("HeightMap", Port.Capacity.Single);

        AddInputPort("TerrainDetail0", Port.Capacity.Single);

        AddInputPort("TerrainDetail1", Port.Capacity.Single);
        AddInputPort("TerrainDetail2", Port.Capacity.Single);
        AddInputPort("TerrainDetail3", Port.Capacity.Single);
        AddInputPort("TerrainDetail4", Port.Capacity.Single);
        AddInputPort("TerrainDetail5", Port.Capacity.Single);
        AddInputPort("TerrainDetail6", Port.Capacity.Single);
        AddInputPort("TerrainDetail7", Port.Capacity.Single);

    }

    public override BaseData GetNodeData()
    {
        OutputData nodeData = new OutputData()
        {
            NodeGuid = NodeGuid,
            Position = GetPosition().position,
        };

        return nodeData;
    }


    public OutputNode SetData(OutputData data)
    {

            base.SetData();
            return this;
    }
}



[System.Serializable]
public class OutputData : BaseData
{


     public override StyleMap GetStyleMap(StyleMap input, List<StyleMap> inputs){
       return inputs[0];
     }


}