using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class ProceduralGraphView : GraphView
{


    private string graphViewStyleSheet = "GraphView";   // Name of the graph view style sheet.
    private ProceduralEditor editorWindow;

    private NodeSearchWindow searchWindow;

    public ProceduralGraphView(ProceduralEditor editorWindow)
    {
        this.editorWindow = editorWindow;

        // Adding the ability to zoom in and out graph view.
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        // Adding style sheet to graph view.
        StyleSheet tmpStyleSheet = Resources.Load<StyleSheet>(graphViewStyleSheet);
        styleSheets.Add(tmpStyleSheet);

        this.AddManipulator(new ContentDragger());      // The ability to drag nodes around.
        this.AddManipulator(new SelectionDragger());    // The ability to drag all selected nodes around.
        this.AddManipulator(new RectangleSelector());   // The ability to drag select a rectangle area.
        this.AddManipulator(new FreehandSelector());    // The ability to select a single node.

        // Add a visible grid to the background.
        GridBackground grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();


        AddSearchWindow();


    }



     private void AddSearchWindow()
        {
            searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            searchWindow.Configure(editorWindow, this);
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
        }



         // This is a graph view method that we override.
        // This is where we tell the graph view which nodes can connect to each other.
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();  // All the ports that can be connected to.
            Port startPortView = startPort;                 // Start port.

            ports.ForEach((port) =>
            {
                Port portView = port;

            // First we tell that it cannot connect to itself.
            // Then we tell it it cannot connect to a port on the same node.
            // Lastly we tell it a input note cannot connect to another input node and an output node cannot connect to output node.
            if (startPortView != portView && startPortView.node != portView.node && startPortView.direction != port.direction && startPortView.portColor == portView.portColor)
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts; // return all the acceptable ports.
        }



      
        // public EndNode CreateEndNode(Vector2 position)
        // {
        //     return new EndNode(position, editorWindow, this);
        // }




        public StartNode CreateStartNode(Vector2 position)
        {
            return new StartNode(position, editorWindow, this);
        }

        public PerlinNoiseNode CreatePerlinNoiseNode(Vector2 position)
        {
            return new PerlinNoiseNode(position, editorWindow, this);
        }

        public AddNode CreateAddNode(Vector2 position)
        {
            return new AddNode(position, editorWindow, this);
        }



        public void CreateNodes(ProceduralGraphObject graphObject)
        {
           

            foreach (StartData node in graphObject.StartDatas)
            {
                InitNode(CreateStartNode(node.Position).SetData(node), node);
            }

          
            foreach (PerlinNoiseData node in graphObject.PerlinNoiseDatas)
            {
                InitNode(CreatePerlinNoiseNode(node.Position).SetData(node), node);
            }


            foreach (AddData node in graphObject.AddDatas)
            {
                InitNode(CreateAddNode(node.Position).SetData(node), node);

            }
    
        }

        public void InitNode(BaseNode node, BaseData data){
            node.NodeGuid = data.NodeGuid;
            AddElement(node);
        }


        public List<SearchTreeEntry> SearchNodeTypes(){

            return new  List<SearchTreeEntry>{
                searchWindow.AddNodeSearch("Terrain Heightmap", new StartNode()),
                searchWindow.AddNodeSearch("PerlinNoise",new PerlinNoiseNode()),
                searchWindow.AddNodeSearch("Addition",new AddNode())
            };
        }


        public bool InstantiateNode(SearchTreeEntry searchTreeEntry, Vector2 position){

            switch (searchTreeEntry.userData)
            {
               
                case PerlinNoiseNode node:
                    AddElement(CreatePerlinNoiseNode(position));
                    return true;

                case StartNode node:
                    AddElement(CreateStartNode(position));
                    return true;

                case AddNode node:
                    AddElement(CreateAddNode(position));
                    return true;

                default:
                    break;
            }
            return false;


        }

}
