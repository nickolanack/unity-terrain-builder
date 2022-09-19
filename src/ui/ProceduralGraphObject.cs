using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;


    [CreateAssetMenu(menuName = "Procedural Terra/Terraform Workflow")]
    [System.Serializable]
    public class ProceduralGraphObject : ScriptableObject
    {
        public List<NodeLinkData> NodeLinkDatas = new List<NodeLinkData>();


        public List<OutputData> OutputDatas = new List<OutputData>();
        public List<PerlinNoiseData> PerlinNoiseDatas = new List<PerlinNoiseData>();
        public List<AddData> AddDatas = new List<AddData>();
        public List<ClampData> ClampDatas = new List<ClampData>();
        public List<FilterData> FilterDatas = new List<FilterData>();
        

        public  List<List<BaseData>> GetLists(){
            return new List<List<BaseData>>(){
                
                OutputDatas.Cast<BaseData>().ToList(), 
                PerlinNoiseDatas.Cast<BaseData>().ToList(), 
                AddDatas.Cast<BaseData>().ToList(), 
                ClampDatas.Cast<BaseData>().ToList(),
                FilterDatas.Cast<BaseData>().ToList()

            };
        }


        int tileX=0;
        int tileY=0;

        public void SetTileXY(int x, int y){
            tileX=x;
            tileY=y;
        }


        public int GetTileX(){
            return tileX;
        }

        public int GetTileY(){
            return tileY;
        }




        private  void ClearLists(){
            PerlinNoiseDatas.Clear();
            OutputDatas.Clear();
            AddDatas.Clear();
            ClampDatas.Clear();
            FilterDatas.Clear();
        }


        public void SaveDatas(List<BaseNode> nodes){


           ClearLists();



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

                     case FilterNode node:
                        FilterDatas.Add((FilterData)node.GetNodeData());
                        break;

                    default:
                        break;
                }
            });
        }


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


            BaseData data=null;
            if((data=GetData(NodeGuid, GetLists()))!=null){

                return data;
            }

        
            return null;

        }

        private BaseData GetData(string NodeGuid, List<List<BaseData>> lists){       

            foreach(List<BaseData> list in lists ){

                foreach(BaseData data in list ){
                    if(data.NodeGuid==NodeGuid){
                        return data;
                    }
                }
            }

            return null;

        }


        public List<BaseData> AllNodes(){     

            List<BaseData> result=new List<BaseData>();  
            foreach(List<BaseData> list in GetLists() ){
                result=result.Concat(list).ToList();  
            }

            return result;

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
