using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


public class ProceduralLoader
{
    private List<Edge> edges => graphView.edges.ToList();
    private List<BaseNode> nodes => graphView.nodes.ToList().Where(node => node is BaseNode).Cast<BaseNode>().ToList();

    private ProceduralGraphView graphView;

    public ProceduralLoader(ProceduralGraphView graphView)
    {
        this.graphView = graphView;
    }

    public void Save(ProceduralGraphObject graphObject)
    {
        SaveEdges(graphObject);
        SaveNodes(graphObject);

        EditorUtility.SetDirty(graphObject);
        AssetDatabase.SaveAssets();
    }

    public void Load(ProceduralGraphObject graphObject)
    {
        ClearGraph();
        graphView.CreateNodes(graphObject);
        ConnectNodes(graphObject);



    }

    #region Save
    private void SaveEdges(ProceduralGraphObject graphObject)
    {
        graphObject.NodeLinkDatas.Clear();

        Edge[] connectedEdges = edges.Where(edge => edge.input.node != null).ToArray();
        for (int i = 0; i < connectedEdges.Count(); i++)
        {
            BaseNode outputNode = (BaseNode)connectedEdges[i].output.node;
            BaseNode inputNode = connectedEdges[i].input.node as BaseNode;

            graphObject.NodeLinkDatas.Add(new NodeLinkData
            {
                BaseNodeGuid = outputNode.NodeGuid,
                BasePortName = connectedEdges[i].output.portName,
                TargetNodeGuid = inputNode.NodeGuid,
                TargetPortName = connectedEdges[i].input.portName,
            });
        }
    }

    private void SaveNodes(ProceduralGraphObject graphObject)
    {

        graphObject.SaveDatas(nodes);

    }


    #endregion

    #region Load

    private void ClearGraph()
    {
        edges.ForEach(edge => graphView.RemoveElement(edge));

        foreach (BaseNode node in nodes)
        {
            graphView.RemoveElement(node);
        }
    }

 

    private void ConnectNodes(ProceduralGraphObject graphObject)
    {
        // Make connection for all node.
        for (int i = 0; i < nodes.Count; i++)
        {
            List<NodeLinkData> connections = graphObject.NodeLinkDatas.Where(edge => edge.BaseNodeGuid == nodes[i].NodeGuid).ToList();

            List<Port> allOutputPorts = nodes[i].outputContainer.Children().Where(x => x is Port).Cast<Port>().ToList();

            for (int j = 0; j < connections.Count; j++)
            {
                string targetNodeGuid = connections[j].TargetNodeGuid;
                BaseNode targetNode = nodes.First(node => node.NodeGuid == targetNodeGuid);

                if (targetNode == null)
                    continue;

                foreach (Port source in allOutputPorts)
                {
                    if (source.portName == connections[j].BasePortName)
                    {
                        //LinkNodesTogether(source, (Port)targetNode.inputContainer[0]);
                        //targetNode.UpdatedData();

                        List<Port> allInputPorts = targetNode.inputContainer.Children().Where(x => x is Port).Cast<Port>().ToList();


                        foreach (Port dest in allInputPorts)
                        {
                            if (dest.portName == connections[j].TargetPortName)
                            {
                                LinkNodesTogether(source, dest);
                                targetNode.UpdatedData();
                            }
                        }


                    }
                }
            }
        }
    }

    private void LinkNodesTogether(Port outputPort, Port inputPort)
    {
        Edge tempEdge = new Edge()
        {
            output = outputPort,
            input = inputPort
        };
        tempEdge.input.Connect(tempEdge);
        tempEdge.output.Connect(tempEdge);
        graphView.Add(tempEdge);
    }

    #endregion
}