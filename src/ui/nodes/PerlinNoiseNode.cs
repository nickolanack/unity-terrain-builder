using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


    public class PerlinNoiseNode : BaseNode
    {
        


        Box boxContainer;
        float size=5;
        float scale=1;
        float offset=0;

        float seed=0;


        FloatField fieldSize;
        FloatField fieldScale;
        FloatField fieldOffset;

        FloatField fieldSeed;


        Image image;

        public PerlinNoiseNode() { }

        public PerlinNoiseNode(Vector2 position, ProceduralEditor editorWindow, ProceduralGraphView graphView)
        {
            base.editorWindow = editorWindow;
            base.graphView = graphView;

            StyleSheet styleSheet = Resources.Load<StyleSheet>("EventNodeStyleSheet");
            styleSheets.Add(styleSheet);

            title = "Perlin Noise";                                    // Set Name
            SetPosition(new Rect(position, defaultNodeSize));   // Set Position
            nodeGuid = Guid.NewGuid().ToString();               // Set Guid ID

            // Add standard ports.
            AddInputPort("Stylemap", Port.Capacity.Single);
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


            fieldSize=new FloatField(){
               label="Size",
               value=size
            };

            fieldScale=new FloatField(){
               label="Scale",
               value=scale
            };
            
            fieldOffset=new FloatField(){
               label="Offset",
               value=offset
            };

            fieldSeed=new FloatField(){
               label="Seed",
               value=seed
            };

            mainContainer.Add(fieldSize);
            mainContainer.Add(fieldScale);
            mainContainer.Add(fieldOffset);
            mainContainer.Add(fieldSeed);
            
            fieldSize.RegisterCallback<ChangeEvent<float>>(OnChangedEvent);
            fieldScale.RegisterCallback<ChangeEvent<float>>(OnChangedEvent);
            fieldOffset.RegisterCallback<ChangeEvent<float>>(OnChangedEvent);
            fieldSeed.RegisterCallback<ChangeEvent<float>>(OnChangedEvent);

            TogglePreviewBox();

        }




        private void OnChangedEvent(ChangeEvent<float> evt) 
        { 
           OnUpdated();
        }


        public void OnUpdated() 
        { 
            size=fieldSize.value;
            scale=fieldScale.value;
            offset=fieldOffset.value;
            seed=fieldSeed.value;
            UpdateTexture();

            foreach(BaseNode node in NextNodes()){
               ((AddNode)node).OnUpdated();  
             }

        }



        public StyleMap GetStyleMapOut(){


            StyleMap style=new StyleMap(200, 200, 0);

            style.SetNoiseOffset((int)seed*100, (int)seed*100);

            style.AddPerlinNoise(size, scale, offset);
            return style;

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



        public void UpdateTexture(){
 
            if(image==null){
                return;
            }



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

        


        public PerlinNoiseData GetNodeData()
        {
            PerlinNoiseData nodeData = new PerlinNoiseData()
            {
                NodeGuid = NodeGuid,
                Position = GetPosition().position,
                Size = size,
                Scale = scale,
                Offset = offset,
                Seed = seed
            };

            return nodeData;
        }


        public PerlinNoiseNode SetData(PerlinNoiseData data)
        {

            size=data.Size;
            scale=data.Scale;
            offset=data.Offset;
            seed=data.Seed;

            if(fieldSize!=null){
                fieldSize.value=size;
            }
            if(fieldScale!=null){
                fieldScale.value=scale;
            }
            if(fieldOffset!=null){
                fieldOffset.value=offset;
            }
            if(fieldSeed!=null){
                fieldSeed.value=seed;
            }

            UpdateTexture();

            return this;

        }
           
    }


[System.Serializable]
public class PerlinNoiseData : BaseData
{
    public float Size;
    public float Scale;
    public float Offset;
    public float Seed;


    public override StyleMap GetStyleMap(StyleMap input, ProceduralGraphObject graph){
        StyleMap style=new StyleMap(input.GetWidth(), input.GetHeight(), 0);

        style.SetNoiseOffset((int)Seed*100, (int)Seed*100);
        style.AddPerlinNoise(Size, Scale, Offset);
        return style;

     }
}


