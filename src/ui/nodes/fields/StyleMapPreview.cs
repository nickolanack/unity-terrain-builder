using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

using Unity.EditorCoroutines.Editor;


public class StyleMapPreview{




	public BaseNode node;
	Image image;
	Box boxContainer;



    private EditorCoroutine queuUpdateCoroutine;



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

        private Color colorValue(float v){

            if(v>2f){
                return Color.magenta;
             }
             if(v>1f){
                return Color.Lerp(Color.white, Color.magenta, 2-v);
             }

             if(v<-1f){
                return Color.cyan;
             }

             if(v<0f){
                return Color.Lerp(Color.black, Color.cyan, 2+v);
             }



             return new Color(v, v, v);
    

        }

        public void UpdateTexture(){

            /**
             * Implement basic throttling/de-bounce to prevent excessive rendering while editing values, especially AnimationCurves
             */

            if(queuUpdateCoroutine!=null){
               EditorCoroutineUtility.StopCoroutine(queuUpdateCoroutine);
            }

            IEnumerator queuUpdate=_DelayUpdateTexture(1.0f);
            queuUpdateCoroutine=EditorCoroutineUtility.StartCoroutine(queuUpdate, this);

        }


        private IEnumerator _DelayUpdateTexture(float delay){

            yield return new EditorWaitForSeconds(delay);
            _UpdateTexture();

        }

        private void _UpdateTexture(){


    
 
            if(image==null){
                return;
            }


            Texture2D texture = new Texture2D(200, 150);

            StyleMap template=new StyleMap(200,150, 0);

            template.SetPerlinConst(100);

			StyleMap style=node.GetStyleMapOut(template);


            for (int y = 0; y < 150; y++){
               for (int x = 0; x < 200; x++)
                {
                     float v=style.GetAt(x,y);

                    texture.SetPixel(x, y, colorValue(v));
                     
                   
                }
            }

            texture.Apply();
            image.image=texture;

        }

        public void Show(){
        	TogglePreviewBox();
        }

}
