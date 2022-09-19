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


	private void AddToNode(UnityEngine.UIElements.VisualElement field){

		 node.mainContainer.Add(field);

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


       AddToNode(field);
	}


	/**
	 * To add a float value field simply call the following within the nods AddFields method
	 * 
	 * 		(new NodeField(this)).AddFloatValue("someFieldName", ()=>{ return someInstanceFloatVariable; }, (value)=>{ someInstanceFloatVariable=value; });
	 */

	public void AddIntegerValue(string label, ValueGetter<int> getter, ValueSetter<int> setter){

		IntegerField field=new IntegerField(){
	       label=label,
	       value=getter()
        };


        node.OnSetData+=delegate (){ 
        	field.value=getter(); 
        };

        field.RegisterCallback<ChangeEvent<int>>(delegate (ChangeEvent<int> evt){
            
            setter(field.value);
            node.UpdatedData();
        
        });


       AddToNode(field);
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

        AddToNode(field);
	}


	/**
	 * To add a toggle/bool value...
	 * 
	 * 		(new NodeField(this)).AddToggle("someFieldName", ()=>{ return someInstanceBoolVariable; }, (value)=>{ someInstanceBoolVariable=value; });
	 */

	public void AddDropDownListValue(string label, List<string> choices,  ValueGetter<string> getter, ValueSetter<string> setter){


		DropdownField field=new DropdownField(){
           label=label,
           choices=choices,
           value=getter()
        };

        node.OnSetData+=delegate (){ 
        	field.value=getter(); 
        };
        
      
        field.RegisterCallback<ChangeEvent<string>>(evt=>{

            setter(field.value);
        	node.UpdatedData();

        });

        AddToNode(field);
	}


	/**
	 * To add a toggle/bool value...
	 * 
	 * 		(new NodeField(this)).AddToggle("someFieldName", ()=>{ return someInstanceBoolVariable; }, (value)=>{ someInstanceBoolVariable=value; });
	 */

	public void AddAnimationCurveValue(string label, ValueGetter<AnimationCurve> getter, ValueSetter<AnimationCurve> setter){


		CurveField field=new CurveField(){
           label=label,
           value=getter()
        };

        node.OnSetData+=delegate (){ 
        	field.value=getter(); 
        };
        
      
        field.RegisterCallback<ChangeEvent<AnimationCurve>>(evt=>{

            setter(field.value);
        	node.UpdatedData();

        });

        AddToNode(field);
	}


}