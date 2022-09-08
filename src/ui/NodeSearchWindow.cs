using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;


public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private ProceduralEditor editorWindow;
    private ProceduralGraphView graphView;

    private Texture2D iconImage;

    public void Configure(ProceduralEditor editorWindow, ProceduralGraphView graphView)
    {
        this.editorWindow = editorWindow;
        this.graphView = graphView;

        // Icon image that we kinda don't use.
        // However use it to create space left of the text.
        // TODO: find a better way.
        iconImage = new Texture2D(1, 1);
        iconImage.SetPixel(0, 0, new Color(0, 0, 0, 0));
        iconImage.Apply();
    }


    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> tree = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent("Terra Editor"),0),
            new SearchTreeGroupEntry(new GUIContent("Terra Node"),1),
        };


        tree.AddRange(graphView.SearchNodeTypes());


        return tree;
    }

    public SearchTreeEntry AddNodeSearch(string name, BaseNode baseNode)
    {
        SearchTreeEntry tmp = new SearchTreeEntry(new GUIContent(name, iconImage))
        {
            level = 2,
            userData = baseNode
        };

        return tmp;
    }

    public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        // Get mouse position on the screen.
        Vector2 mousePosition = editorWindow.rootVisualElement.ChangeCoordinatesTo(
            editorWindow.rootVisualElement.parent, context.screenMousePosition - editorWindow.position.position);

        // Now we use mouse position to calculator where it is in the graph view.
        Vector2 graphMousePosition = graphView.contentViewContainer.WorldToLocal(mousePosition);

        return CheckForNodeType(searchTreeEntry, graphMousePosition);
    }

    private bool CheckForNodeType(SearchTreeEntry searchTreeEntry, Vector2 position)
    {
        return graphView.InstantiateNode(searchTreeEntry, position);
    }
}