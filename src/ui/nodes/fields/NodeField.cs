using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public class NodeField{


	BaseNode node;


	public delegate T ValueGetter<T>();
	public delegate void ValueSetter<T>(T value);

	public NodeField(BaseNode node) {

		this.node=node;

	}


	/**
	 * To add a float value field simply call the following within the nods AddFields method
	 * 
	 * 		(new NodeField(this)).AddFloatValue("someFieldName", ()=>{ return someInstanceFloatVariable; }, (value)=>{ someInstanceFloatVariable=value; });
	 */

	public void AddFloatValue(string label, ValueGetter<float> getter, ValueSetter<float> setter){

		FloatField field=new FloatField(){
	       label=label,
	       value=getter()
        };


        node.OnSetData+=delegate (){ 
        	field.value=getter(); 
        };

        field.RegisterCallback<ChangeEvent<float>>(delegate (ChangeEvent<float> evt){
            
            setter(field.value);
            node.UpdatedData();
        
        });


        node.mainContainer.Add(field);
	}


	/**
	 * To add a toggle/bool value...
	 * 
	 * 		(new NodeField(this)).AddToggle("someFieldName", ()=>{ return someInstanceBoolVariable; }, (value)=>{ someInstanceBoolVariable=value; });
	 */

	public void AddToggleValue(string label, ValueGetter<bool> getter, ValueSetter<bool> setter){


		Toggle field=new Toggle(){
           label=label,
           value=getter()
        };

        node.OnSetData+=delegate (){ 
        	field.value=getter(); 
        };
        
      
        field.RegisterCallback<ChangeEvent<bool>>(evt=>{

            setter(field.value);
        	node.UpdatedData();

        });

        node.mainContainer.Add(field);
	}


}