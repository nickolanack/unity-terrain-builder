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



    


        public void CreateNodes(ProceduralGraphObject graphObject)
        {
           



            foreach (BaseData node in graphObject.AllNodes())
            {
                
                InitNode(
                    node.InstantiateNode().DrawNode(
                        node.Position, editorWindow, this
                    ).SetData(node), 
                    node);
                //InitNode(CreateOutputNode(node.Position).SetData(node), node);
            }

          
           

        
        }

        public void InitNode(BaseNode node, BaseData data){
            node.NodeGuid = data.NodeGuid;
            AddElement(node);
        }


        public List<SearchTreeEntry> SearchNodeTypes(){

            return new  List<SearchTreeEntry>{
                searchWindow.AddNodeSearch(new OutputNode()),
                searchWindow.AddNodeSearch(new PerlinNoiseNode()),
                searchWindow.AddNodeSearch(new AddNode()),
                searchWindow.AddNodeSearch(new ClampNode()),
                searchWindow.AddNodeSearch(new FilterNode()),
            };
        }


        public bool InstantiateNode(SearchTreeEntry searchTreeEntry, Vector2 position){


            BaseNode node=(BaseNode)searchTreeEntry.userData;
            AddElement(node.Instantiate().DrawNode(position, editorWindow, this));

            return true;



        }

}
