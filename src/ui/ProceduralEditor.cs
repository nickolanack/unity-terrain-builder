using System.Collections;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


public class ProceduralEditor : EditorWindow
    {
        private ProceduralGraphObject currentGraphObject;                           // Current open dialouge container in dialogue editor window.
        private ProceduralGraphView graphView;                                            // Reference to GraphView Class.


         private ProceduralLoader saveAndLoad;  

        private Label containerLabel;                                          // Name of the current open dialouge container.
        private string graphViewStyleSheet = "Editor"; // Name of the graph view style sheet.



        // Callback attribute for opening an asset in Unity (e.g the callback is fired when double clicking an asset in the Project Browser).
        // Read More https://docs.unity3d.com/ScriptReference/Callbacks.OnOpenAssetAttribute.html
        [OnOpenAsset(0)]
        public static bool ShowWindow(int instanceId, int line)
        {
            UnityEngine.Object item = EditorUtility.InstanceIDToObject(instanceId); // Find Unity Object with this instanceId and load it in.

            if (item is ProceduralGraphObject)    // Check if item is a DialogueContainerSO Object.
            {
                ProceduralEditor window = (ProceduralEditor)GetWindow(typeof(ProceduralEditor));    // Make a unity editor window of type ProceduralEditor.
                window.titleContent = new GUIContent("Terra Workflow Editor");                                        // Name of editor window.
                window.currentGraphObject = item as ProceduralGraphObject;                                  // The DialogueContainerSO we will load in to editor window.
                window.minSize = new Vector2(500, 250);                                                         // Starter size of the editor window.
                window.Load();                                                                                  // Load in DialogueContainerSO data in to the editor window.
            }

            return false;   // we did not handle the open.
        }

        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
            Load();
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(graphView);
        }


        private void ConstructGraphView()
        {
            // Make the DialogueGraphView and Stretch it to the same size as the Parent.
            // Add it to the DialogueEditorWindow.
            graphView = new ProceduralGraphView(this);
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);

            saveAndLoad = new ProceduralLoader(graphView);

        }


        private void GenerateToolbar()
        {
            // Find and load the styleSheet for graph view.
            StyleSheet styleSheet = Resources.Load<StyleSheet>(graphViewStyleSheet);
            // Add the styleSheet for graph view.
            rootVisualElement.styleSheets.Add(styleSheet);

            Toolbar toolbar = new Toolbar();

            // Save button.
            {
                Button saveBtn = new Button()
                {
                    text = "Save"
                };
                saveBtn.clicked += () =>
                {
                    Save();
                };
                toolbar.Add(saveBtn);
            }

            // Load button.
            {
                Button loadBtn = new Button()
                {
                    text = "Load"
                };
                loadBtn.clicked += () =>
                {
                    Load();
                };
                toolbar.Add(loadBtn);
            }

            {
                containerLabel = new Label("");
                toolbar.Add(containerLabel);
                containerLabel.AddToClassList("containerLabel");
            }

            rootVisualElement.Add(toolbar);
        }

        private void Load()
        {
          if (currentGraphObject != null)
            {
                containerLabel.text = "Name:   " + currentGraphObject.name;
                saveAndLoad.Load(currentGraphObject);
            }
        }

        private void Save()
        {
           if (currentGraphObject != null)
            {
                saveAndLoad.Save(currentGraphObject);
            }
        }

    }