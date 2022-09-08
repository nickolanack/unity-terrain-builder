using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;



public class StyleMapPreview{




	public BaseNode node;
	Image image;
	Box boxContainer;


	 public StyleMapPreview(BaseNode node) {
	 	this.node=node;

        node.OnSetData+=()=>UpdateTexture();
        node.OnUpdatedData+=()=>UpdateTexture();


        AddMenu();
        Show();

	 }

	 public void AddMenu(){

	 	 ToolbarMenu menu = new ToolbarMenu();
         menu.text = "...";


         menu.menu.AppendAction("Toggle preview", new Action<DropdownMenuAction>(x => TogglePreviewBox()));
         node.titleContainer.Add(menu);

	 }

	 public void TogglePreviewBox(){

          
            if(boxContainer!=null){
    
                node.mainContainer.Remove(boxContainer);
                boxContainer=null;
                image=null;
                return;
            }

            boxContainer = new Box();
            boxContainer.AddToClassList("PreviewBox");
            node.mainContainer.Add(boxContainer);

           

            image=new Image();
            
            UpdateTexture();

            boxContainer.Add(image);

        }


        public void UpdateTexture(){
 
            if(image==null){
                return;
            }



            Texture2D texture = new Texture2D(200, 150);
			StyleMap style=node.GetStyleMapOut();


            for (int y = 0; y < 150; y++){
               for (int x = 0; x < 200; x++)
                {
                     float c=style.GetAt(x,y);
                     texture.SetPixel(x, y, new Color(c, c, c));
                }
            }

            texture.Apply();
            image.image=texture;

        }

        public void Show(){
        	TogglePreviewBox();
        }

}
