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
        
    }

    [System.Serializable]
    public class NodeLinkData
    {
        public string BaseNodeGuid;
        public string BasePortName;
        public string TargetNodeGuid;
        public string TargetPortName;
    }
