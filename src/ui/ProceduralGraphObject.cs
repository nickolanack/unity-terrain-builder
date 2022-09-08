using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


    [CreateAssetMenu(menuName = "Procedural Terra/Terraform Workflow")]
    [System.Serializable]
    public class ProceduralGraphObject : ScriptableObject
    {
        public List<NodeLinkData> NodeLinkDatas = new List<NodeLinkData>();


        public List<OutputData> OutputDatas = new List<OutputData>();
        public List<PerlinNoiseData> PerlinNoiseDatas = new List<PerlinNoiseData>();
        public List<AddData> AddDatas = new List<AddData>();
        public List<ClampData> ClampDatas = new List<ClampData>();
        

        public  bool PortHasInputs(BaseData data, string portName){
            return PortHasInputs(data.NodeGuid, portName);
        }
        public  bool PortHasInputs(string NodeGuid, string portName){



            foreach(NodeLinkData link in NodeLinkDatas){


                if(portName!=null&&!link.TargetPortName.Equals(portName)){
                    continue;
                }
                if(link.TargetNodeGuid==NodeGuid){
                    return true;
                }
            }


            return false;


        }


        public  List<BaseData> GetInputsTo(string NodeGuid, string portName){
             List<BaseData> list=new List<BaseData>();
            foreach(NodeLinkData link in NodeLinkDatas){


                if(portName!=null&&!link.TargetPortName.Equals(portName)){
                    //Debug.Log(portName+" "+link.TargetPortName);
                    continue;
                }
                //Debug.Log(link.TargetPortName);

                if(link.TargetNodeGuid==NodeGuid){
                    list.Add(GetData(link.BaseNodeGuid));
                }
            }


            return list;
        }
        public  List<BaseData> GetInputsTo(string NodeGuid){

            return GetInputsTo(NodeGuid, null);
        
        }

        public BaseData GetData(string NodeGuid){


            foreach(OutputData data in OutputDatas ){
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

            foreach(ClampData data in ClampDatas){
                if(data.NodeGuid==NodeGuid){
                    return data;
                }
            }


           



            return null;

        }


        public void SaveDatas(List<BaseNode> nodes){


            PerlinNoiseDatas.Clear();
            OutputDatas.Clear();
            AddDatas.Clear();
            ClampDatas.Clear();
       



            nodes.ForEach(baseNode =>
            {
                switch (baseNode)
                {
                  
                    case OutputNode node:
                        OutputDatas.Add((OutputData)node.GetNodeData());
                        break;
                  
                    case PerlinNoiseNode node:
                        PerlinNoiseDatas.Add((PerlinNoiseData)node.GetNodeData());
                        break;

                    case AddNode node:
                        AddDatas.Add((AddData)node.GetNodeData());
                        break;

                    case ClampNode node:
                        ClampDatas.Add((ClampData)node.GetNodeData());
                        break;

            
     
                    default:
                        break;
                }
            });
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
