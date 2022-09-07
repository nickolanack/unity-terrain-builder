using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


    [CreateAssetMenu(menuName = "Procedural Terra/Terraform Workflow")]
    [System.Serializable]
    public class ProceduralGraphObject : ScriptableObject
    {
        public List<NodeLinkData> NodeLinkDatas = new List<NodeLinkData>();


        public List<StartData> StartDatas = new List<StartData>();
        public List<PerlinNoiseData> PerlinNoiseDatas = new List<PerlinNoiseData>();
        public List<AddData> AddDatas = new List<AddData>();
        

        public  List<BaseData> GetInputsTo(string NodeGuid){


            List<BaseData> list=new List<BaseData>();
            foreach(NodeLinkData link in NodeLinkDatas){
                if(link.TargetNodeGuid==NodeGuid){
                    list.Add(GetData(link.BaseNodeGuid));
                }
            }


            return list;


        }

        public BaseData GetData(string NodeGuid){


            foreach(StartData data in StartDatas ){
                if(data.NodeGuid==NodeGuid){
                    return data;
                }
            }

            foreach(PerlinNoiseData data in PerlinNoiseDatas ){
                if(data.NodeGuid==NodeGuid){
                    return data;
                }
            }

            foreach(AddData data in AddDatas ){
                if(data.NodeGuid==NodeGuid){
                    return data;
                }
            }




            return null;

        }


    }

    [System.Serializable]
    public class NodeLinkData
    {
        public string BaseNodeGuid;
        public string BasePortName;
        public string TargetNodeGuid;
        public string TargetPortName;
    }
