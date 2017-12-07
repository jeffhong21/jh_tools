using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


/// <summary>
///  This script will copy all components and add it to all the target objects.
///
///
///  - AllComponents:  Will add selected components, even if target objects has component.
///  - OnlyNew:  Will add selected components, but only if the target object does not have that component.
///  - OnlyValues:  Will copy values from selected components onto target objects components only if target has component.
///
/// </summary>

namespace CopyComponents
{
    
    public class CopyComponentsWindow : EditorWindow
    {

        private enum CopyOptions { AllComponents, OnlyNew, OnlyValues };
        

        private GameObject m_SourceObject;
        //  List structure of components on source object.  (bool, component)
        private List<ComponentToCopy> m_SourceObjComponents = new List<ComponentToCopy>();
        //  List of all the target objects
        private List<GameObject> m_TargetObjects = new List<GameObject>();

        private CopyOptions m_CopyOptions = CopyOptions.AllComponents;
        private bool m_AllComponents = true;

        //  List of componets to add and values to copy.
        private List<ComponentToAdd> m_ComponentToAdd  = new List<ComponentToAdd>();
        //  Each targets list of components to add.
        private Dictionary<GameObject, List<ComponentToAdd>> m_TargetComponentsToAdd = new Dictionary<GameObject, List<ComponentToAdd>>();


        private bool debug = true;

        #region Window Functions
        [MenuItem("JH Tools/Copy Components")]
        private static void OpenWindow()
        {
            CopyComponentsWindow window = (CopyComponentsWindow)GetWindow(typeof(CopyComponentsWindow));

            window.titleContent = new GUIContent("Copy Components Tool");
            window.minSize = new Vector2(450, 400);
            window.maxSize = new Vector2(450, 400);
            window.Show();

            //window.ShowUtility();
            GUI.BringWindowToFront(0);

        }

        // private void OnEnable()
        // {
        //     //  This update callback is called 30 times per second in the editor.  Basically its
        //     //  an Update() function you can use at edit time.
        //     EditorApplication.update -= UpdateComponentList;
        //     EditorApplication.update += UpdateComponentList;
        // }

        // private void OnDisable()
        // {
        //     EditorApplication.update -= UpdateComponentList;
        // }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();

            GUILayoutOption GUIHeight = GUILayout.Height(Screen.height - 74);

            DisplaySourceObject(GUIHeight);
            DisplayTargetObjects(GUIHeight);
            EditorGUILayout.EndHorizontal();
            DisplayCopyComponentOptions();
        }


        //  -- THE LEFT SIDE
        private void DisplaySourceObject(GUILayoutOption GUIHeight)
        {
            //  -- START OF OBJECT SELECTION PANEL  
            EditorGUILayout.BeginVertical(new GUIStyle("HelpBox"), GUIHeight);
            //  -- SOURCE GAME OBJECT TO COPY FROM
            GUILayout.Label("Sources (Copy From): ");
            m_SourceObject = (GameObject)EditorGUILayout.ObjectField(m_SourceObject, typeof(GameObject), true);

            //  -- ALL COMPONENTS FROM GAME OBJECT
            GUILayout.Space(8);
            GUILayout.Label("Components To Copy: ");


            //  -- DRAW COMPONENTS
            EditorGUILayout.BeginVertical(new GUIStyle("HelpBox"));             //  -- LIST OF TOGGLES BEGINVERTICAL
            //  Start BeginChangeCheck() so if All toggle is selected, all components are selected.
            EditorGUI.BeginChangeCheck();   

            if (m_SourceObject != null)
            {
                m_AllComponents = GUILayout.Toggle(m_AllComponents, "All");
                DisplayAllComponents(m_SourceObject, m_SourceObjComponents);

            }

            if(EditorGUI.EndChangeCheck())
            {
                // if All toggle is selected, set all components to true.
                if(m_AllComponents == true)
                {
                    for (int index = 0; index < m_SourceObjComponents.Count; index++)
                    {
                        m_SourceObjComponents[index].IsCopyComponent = true;
                    }
                }
            }
            EditorGUILayout.EndVertical();                                  //  -- LIST OF TOGGLES ENDVERTICAL




            //  -- END OBJECT SELECTION PANEL
            EditorGUILayout.EndVertical();
        }


        //  -- THE RIGHT SIDE
        private void DisplayTargetObjects(GUILayoutOption GUIHeight)
        {
            //  -- START OF OBJECT SELECTION PANEL  
            EditorGUILayout.BeginVertical(new GUIStyle("HelpBox"), GUIHeight);
            //  -- TARGETS TO 
            GUILayout.Label("Targets (Paste To): ");

            DisplayTargetObjects();


            //  -- ALL THE BUTTONS FOR ADDING OR REMOVING OBJECTS
            int buttonGUIHeight = 24;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(" + ", new GUILayoutOption[] { GUILayout.Height(buttonGUIHeight) }))
            {
                Debug.Log("Add " + Selection.activeGameObject + " target objects");
                AddTargetObjects();
            }

            if (GUILayout.Button(" - ", new GUILayoutOption[] { GUILayout.Height(buttonGUIHeight) }))
            {
                Debug.Log("Remove " + Selection.activeGameObject + " from target objects list");
                RemoveTargetObjects();
            }

            if (GUILayout.Button("Clear Objects", new GUILayoutOption[] { GUILayout.Height(buttonGUIHeight) }))
            {
                ClearTargetObjects();
                Debug.Log("Clear All Objects");

            }

            EditorGUILayout.EndHorizontal();



            //  -- END OBJECT SELECTION PANEL
            EditorGUILayout.EndVertical();

        }

        //  -- THE BUTTONS
        private void DisplayCopyComponentOptions()
        {
            EditorGUILayout.BeginVertical();

            m_CopyOptions = (CopyOptions)EditorGUILayout.EnumPopup(new GUIContent("Copy Options: "), m_CopyOptions);

            if(debug)
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Copy Components", GUILayout.Height(24))){
                    CopyComponentsToTarget();
                }
                if (GUILayout.Button("PRINT DEBUG", GUILayout.Height(24))){
                    DebugMessage();
                }

                EditorGUILayout.EndHorizontal();
            }
            else{
                if (GUILayout.Button("Copy Components", GUILayout.Height(24))){
                    CopyComponentsToTarget();
                }
            }


            EditorGUILayout.EndVertical();

        }


        private void UpdateComponentList()
        {
            if (m_SourceObject != null)
            {
                //  -- Go through the source object list of components.
                for (int index = 0; index < m_SourceObject.GetComponents<Component>().Length; index++)
                {
                    //  -- Check to see if list contains the component.  Otherwise it will keep on adding.
                    Component srcComponent = m_SourceObject.GetComponents<Component>()[index];
                    ComponentToCopy srcComponentToCopy = new ComponentToCopy(true, srcComponent);

                    if (m_SourceObjComponents.Contains(srcComponentToCopy) == false)
                    {
                        m_SourceObjComponents.Add(srcComponentToCopy);
                    }

                
                }
                // Repaint();
            }
        }


        // Displays the all components of the source game object
        private void DisplayAllComponents(GameObject srcObject, List<ComponentToCopy> componentsToCopy)
        {
            UpdateComponentList();

            for (int index = 0; index < srcObject.GetComponents<Component>().Length; index++)
            {
                string componentName = componentsToCopy[index].ComponentName;
                // bool isCopy = GUILayout.Toggle(componentsToCopy[index].IsCopyComponent, componentName);
                componentsToCopy[index].IsCopyComponent = GUILayout.Toggle(componentsToCopy[index].IsCopyComponent, componentName);
            }

        }


        // Displays the all the target objects ObjectField UI
        private void DisplayTargetObjects()
        {
            for (int i = 0; i < m_TargetObjects.Count; i++)
            {
                m_TargetObjects[i] = (GameObject)EditorGUILayout.ObjectField(m_TargetObjects[i], typeof(GameObject), true);
            }
        }


        private void AddTargetObjects()
        {
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                if (!m_TargetObjects.Contains(Selection.gameObjects[i]))
                {
                    m_TargetObjects.Add(Selection.gameObjects[i]);
                }
            }
            Repaint();
        }


        private void RemoveTargetObjects()
        {
            //  Remove targets from selection.
            if(Selection.gameObjects.Length > 0){
                foreach (GameObject targetObj in Selection.gameObjects){
                    //  If selection is in targetObjects
                    if (m_TargetObjects.Contains(targetObj)){
                        m_TargetObjects.Remove(targetObj);
                    }
                    //  If not remove the last index.
                    else{
                        m_TargetObjects.RemoveAt(m_TargetObjects.Count - 1);
                    }
                }
            }
            //  Remove the last target.
            else{
                if (m_TargetObjects.Count > 0){
                    m_TargetObjects.RemoveAt(m_TargetObjects.Count - 1);
                }
            }

            Repaint();
        }


        private void ClearTargetObjects()
        {
            m_TargetObjects.Clear();
            Repaint();
        }
        #endregion


        //  -- ADD ALL SELECTED COMPONEENTS ONTO TARGET OBJECTS.
        private void CopyComponentsToTarget()
        {
            //  Add all checked components into a new list and get their values.
            m_ComponentToAdd = GetAllComponentsToCopy(m_SourceObject, m_SourceObjComponents);
            //  Get List of targets and the components to add to each target.
            m_TargetComponentsToAdd = GetListOfComponentsToCopyForTarget(m_TargetObjects, m_ComponentToAdd);

            //  Loop through each target and add components.
            foreach(GameObject target in m_TargetObjects)
            {
                List<ComponentToAdd> targetComponentsToAdd = m_TargetComponentsToAdd[target];


                //  Add components.
                for(int i = 0; i < targetComponentsToAdd.Count; i ++)
                {
                    Component componentToAdd = targetComponentsToAdd[i].Component;
                    FieldInfo[] fieldAttrs = targetComponentsToAdd[i].FieldAttrs;
                    Component newComponent = target.AddComponent(componentToAdd.GetType());

                    if (fieldAttrs.Length > 0)
                    {
                        //  Add values to newly added component.
                        foreach(FieldInfo field in fieldAttrs)
                        {
                            //  If you are using private serialized variables.  Remove if something goes wrong.
                            if(field.IsPublic || field.GetCustomAttributes(typeof(SerializeField),true).Length != 0)
                            {
                                //  Get the value from the component that was added.
                                object value = field.GetValue(componentToAdd);
                                //  Set the values of the newly added component.
                                field.SetValue(newComponent, value);
                            }
                        }
                    }
                    else
                    {
                        UnityEditorInternal.ComponentUtility.CopyComponent(componentToAdd);
                        UnityEditorInternal.ComponentUtility.PasteComponentValues(newComponent);
                    }



                }
            }
        }


        //  Get all checked components and put into list.  ComponentToAdd class will get all values.
        private List<ComponentToAdd> GetAllComponentsToCopy(GameObject sourceObject, List<ComponentToCopy> componentsToCopy)
        {
            List<ComponentToAdd> componentsToAdd = new List<ComponentToAdd>();
            if (sourceObject != null){
                for (int k = 0; k < componentsToCopy.Count; k++)
                {
                    if(componentsToCopy[k].IsCopyComponent)
                    {
                        //  Component from the source object
                        Component srcComponentToAdd = componentsToCopy[k].Component;
                        // if(srcComponentToAdd.GetType().ToString() == "ParticleSystem"){
                        //     continue;
                        // }
                        ComponentToAdd componentToAdd = new ComponentToAdd(srcComponentToAdd);
                        componentToAdd.SetFieldAttr(srcComponentToAdd.GetType());
                        componentsToAdd.Add(componentToAdd);
                    }
                }
            }
            return componentsToAdd;
        }


        //  Returns a dictionary of target gameObjects and its componentsToCopy.
        private Dictionary<GameObject, List<ComponentToAdd>> GetListOfComponentsToCopyForTarget(List<GameObject> targetObjects, List<ComponentToAdd> sourceComponentsToAdd)
        {
            Dictionary<GameObject, List<ComponentToAdd>> targetComponentsToAdd = new Dictionary<GameObject, List<ComponentToAdd>>();
            List<ComponentToAdd> componentsToAdd = sourceComponentsToAdd;

            //  Loop through each target object and get each target objects components.
            for (int i = 0; i < targetObjects.Count; i++ )
            {
                GameObject target = targetObjects[i];

                //  Temporary hack way to be able to check if any of the targets components are in componentsToAdd (components to add to target).
                //  Instead of comparing the component, I'm comparing component.GetType() cause it returns the components name.
                Component[] _targetComponents = target.GetComponents<Component>();          //  Temporary array to hold the targets components.
                Type[] targetComponents = new Type[target.GetComponents<Component>().Length];

                for (int k = 0; k < target.GetComponents<Component>().Length; k ++)
                {
                    targetComponents[k] = _targetComponents[k].GetType();
                    // Debug.Log("Target components: " + targetComponents[k]);
                }

                if (m_CopyOptions == CopyOptions.OnlyNew || m_CopyOptions == CopyOptions.OnlyValues)
                {
                    //  Loop through list of componentsToCopy and check if target object contains that component.
                    for(int k = 0; k < componentsToAdd.Count; k ++)
                    {
                        //  Check if target contains the a component that is to be added.
                        if(targetComponents.Contains(componentsToAdd[k].Component.GetType()) )     // Using System.Linq Contain Method to check if source component is in targetComponents array.
                        {
                            //Debug.Log(string.Format("{0} contains the components:  {1}.  Removing",target, componentsToAdd[k].Component.GetType().ToString() ));
                            //  Remove from list of components to add to target.
                            componentsToAdd.Remove(componentsToAdd[k]);
                        }
                    }
                }

                //  Prints all the targets components.
                PrintComponentList( "List of Targets components", targetComponents);
                //  Prints all the components that need to be added to target.
                PrintComponentList( "List of Components to add to Target", componentsToAdd);

                //  Add target and its list of componentsToAdd.
                targetComponentsToAdd.Add(target, componentsToAdd);
            }
            return targetComponentsToAdd;
        }


        private void PrintComponentList(string header, List<ComponentToAdd> list)
        {
            string debugInfo = header + "\n";
            for(int k = 0; k < list.Count; k ++){
                debugInfo += list[k].Component.GetType().ToString() + "\n";
            }
            Debug.Log( debugInfo );
        }

        private void PrintComponentList(string header, List<ComponentToCopy> list)
        {
            string debugInfo = header + "\n";
            for(int k = 0; k < list.Count; k ++){
                debugInfo += list[k].Component.GetType().ToString() + "\n";
            }
            Debug.Log( debugInfo );
        }

        private void PrintComponentList(string header, Type[] list)
        {
            string debugInfo = header + "\n";
            for(int k = 0; k < list.Length; k ++){
                debugInfo += list[k].ToString() + "\n";
            }
            Debug.Log( debugInfo );
        }


        private void DebugMessage()
        {
            // if (m_SourceObject != null)
            // {
                
            //     //  Add all checked components into a new list and get their values.
            //     m_ComponentToAdd = GetAllComponentsToCopy(m_SourceObject, m_SourceObjComponents);
            //     //  Get List of targets and the components to add to each target.
            //     m_TargetComponentsToAdd = GetListOfComponentsToCopyForTarget(m_TargetObjects, m_ComponentToAdd);

            //     //  Loop through each target and add components.
            //     foreach(GameObject target in m_TargetObjects)
            //     {
            //         ComponentToAdd[] targetComponentsToAdd = m_TargetComponentsToAdd[target].ToArray();

            //         string componentInfo = string.Format("Target Name:  {0}\n", target);

            //         Debug.Log(m_TargetComponentsToAdd[target].Count);
            //         //  Add components.
            //         // for(int i = 0; i < targetComponentsToAdd.Length; i ++)
            //         // {
            //         //     Component componentToAdd = targetComponentsToAdd[i].Component;
            //         //     FieldInfo[] fieldAttrs = targetComponentsToAdd[i].FieldAttrs;

            //         //     componentInfo += componentToAdd.GetType().ToString() + "\n";
            //         // }


            //         Debug.Log(componentInfo);
            //     }

            // }

            // SourceObjComponents();

            Debug.Log("Source Components: " + m_SourceObjComponents.Count);
            Debug.Log("Components To Add: " + m_ComponentToAdd.Count);


        }


        // private void SourceObjComponents()
        // {
        //     string info = "Componets:\n";
        //     if (m_SourceObject != null)
        //     {
        //         //  -- Go through the source object list of components.
        //         for (int index = 0; index < m_SourceObject.GetComponents<Component>().Length; index++)
        //         {
        //             //  -- Check to see if list contains the component.  Otherwise it will keep on adding.
        //             Component srcComponent = m_SourceObject.GetComponents<Component>()[index];
        //             ComponentToCopy srcComponentToCopy = new ComponentToCopy(true, srcComponent);

        //             if (m_SourceObjComponents.Contains(srcComponentToCopy) == false)
        //             {
        //                 m_SourceObjComponents.Add(srcComponentToCopy);
        //                 info += srcComponentToCopy.ComponentName.ToString() + "\n";
        //             }
        //         }
        //     }


        //     Debug.Log(info);
        //     Debug.Log(m_SourceObjComponents.Count);



        // }


        // private void _UpdateComponentList()
        // {
        //     if (m_SourceObject != null)
        //     {
        //         //  -- Go through the source object list of components.
        //         for (int index = 0; index < m_SourceObject.GetComponents<Component>().Length; index++)
        //         {
        //             //  -- Check to see if list contains the component.  Otherwise it will keep on adding.
        //             Component srcComponent = m_SourceObject.GetComponents<Component>()[index];
        //             ComponentToCopy srcComponentToCopy = new ComponentToCopy(true, srcComponent);

        //             if (m_SourceObjComponents.Contains(srcComponentToCopy) == false)
        //             {
        //                 m_SourceObjComponents.Add(srcComponentToCopy);
        //             }
        //         }
        //     }
        // }





    }
}


