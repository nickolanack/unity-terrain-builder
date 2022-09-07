using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


    public class AddNode : BaseNode
    {
        


        Box boxContainer;
        

        bool normalize=false;


        Image image;

        public AddNode() { }

        public AddNode(Vector2 position, ProceduralEditor editorWindow, ProceduralGraphView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            StyleSheet styleSheet = Resources.Load<StyleSheet>("EventNodeStyleSheet");
            styleSheets.Add(styleSheet);

            title = "Addition";                                    // Set Name
            SetPosition(new Rect(position, defaultNodeSize));   // Set Position
            nodeGuid = Guid.NewGuid().ToString();               // Set Guid ID

            // Add standard ports.
            AddInputPort("Stylemaps", Port.Capacity.Multi);
            AddOutputPort("Out", Port.Capacity.Multi);

            AddFields();

            RefreshExpandedState();
            RefreshPorts();
        }

        private void AddFields()
        {
           
            ToolbarMenu menu = new ToolbarMenu();
            menu.text = "...";

            menu.menu.AppendAction("Toggle preview", new Action<DropdownMenuAction>(x => TogglePreviewBox()));
            titleContainer.Add(menu);

           	TogglePreviewBox();

        }


        public void OnUpdated() 
        { 
          
            UpdateTexture();
        }

        public void TogglePreviewBox(){

          	 if(boxContainer!=null){
    
                mainContainer.Remove(boxContainer);
                boxContainer=null;
                image=null;
                return;
            }

            boxContainer = new Box();
            boxContainer.AddToClassList("PreviewBox");
            mainContainer.Add(boxContainer);

           
            image=new Image();
            UpdateTexture();
        
           //Image img = GetNewImage("ImagePreview", "ImagePreviewLeft");
            boxContainer.Add(image);

        }


        public StyleMap GetStyleMapOut(){


        	 StyleMap map=new StyleMap(200,200,0);

        	 foreach(BaseNode node in PreviousNodes()){
        	 	map.Add(((PerlinNoiseNode)node).GetStyleMapOut());	
        	 }
        	
             if(normalize){
        	   map.Normalize();
             }
        	
        	 return map;

        }



        public void UpdateTexture(){
 
     		 Texture2D texture = new Texture2D(200, 200);

            StyleMap style=GetStyleMapOut();

            for (int y = 0; y < 200; y++){
               for (int x = 0; x < 200; x++)
                {
                     float c=style.GetAt(x,y);
                     texture.SetPixel(x, y, new Color(c, c, c));
                }
            }

            texture.Apply();


            image.image=texture;
        }

        


        public AddData GetNodeData()
        {
            AddData nodeData = new AddData()
            {
                NodeGuid = NodeGuid,
                Position = GetPosition().position,

            };

            return nodeData;
        }


        public AddNode SetData(AddData data)
        {

            normalize=data.Normalize;
           
            UpdateTexture();

            return this;

        }
           
    }


[System.Serializable]
public class AddData : BaseData
{

    public bool Normalize;


    public override StyleMap GetStyleMap(StyleMap input, ProceduralGraphObject graph){


        StyleMap style= new StyleMap(input.GetWidth(), input.GetHeight(), 0);

        foreach(BaseData data in graph.GetInputsTo(NodeGuid)){
            style.Add(data.GetStyleMap(input, graph));
        }
        if(Normalize){
            style.Normalize();
        }

        return style;
    }
}


