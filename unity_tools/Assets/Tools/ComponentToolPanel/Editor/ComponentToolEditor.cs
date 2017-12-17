using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ComponentTool
{
    // [CustomEditor(typeof(GameObject))]
	public class ComponentToolPanel : Editor
	{
        const string gameObjectInspectorTypeName = "UnityEditor.GameObjectInspector, UnityEditor";
        
		private GameObject m_Selection;
        //  Base IList of Components to compare CurrentComponents too.
        private IList m_ComponentList = new List<Component>();
         // Get IList of current Component order to compare againt base ComponentList before OnReorderCallback is called.
        private IList m_CurrentComponents = new List<Component>();
		private ReorderableList m_List;




        private bool showComponentToolPanel = true;
        private Editor m_DefaultEditor;
        //  "UnityEditor.GameObjectInspector, UnityEditor" is a custom internal script that draws the GameObjects Inspector.
        private static Type type = Type.GetType("UnityEditor.GameObjectInspector, UnityEditor");


        void OnEnable(){
            m_DefaultEditor = Editor.CreateEditor(targets, Type.GetType(gameObjectInspectorTypeName));
        }


        // //  We need an empty OnIspectorGUI() else it'll print all the variables again in a list
        // public override void OnInspectorGUI(){ }


        //  We're overriding Unity's internal script GameObjectInspector.cs's OnHeaderGUI method.
        protected override void OnHeaderGUI()
        {
            //  Creating a cache of the editor.
            //CreateCachedEditor(targets, type, ref m_DefaultEditor);
            //var gameObject = target as GameObject;

            //  Draw the default Unity inspectorGUI.  Other wise it will be empty.
            m_DefaultEditor.DrawHeader();

            //  Custom GUI code here
            
            DrawLayout();

            
        }





        private void DrawLayout()
        {
            EditorGUILayout.BeginVertical();

            showComponentToolPanel = GUILayout.Toggle (showComponentToolPanel, new GUIContent("Component Tool Panel")  , new GUIStyle ("Foldout"));//ShurikenModuleTitle

            if(showComponentToolPanel)
            {
                if (ReturnSelection() != null)
                {
                    if (m_List == null || m_List.list == null)
                    {
                        try{
                            InitializeReorderableList();
                        }
                        catch{
                            Debug.LogError("Either ReorderableList is null");
                            //DebugReorderableListNull();
                        }
                    }
                    else
                    {
                        GUILayout.Space( 5 );
                        EditorGUILayout.BeginVertical(new GUIStyle("HelpBox") );

                        if(ReturnSelection() != null){
                            
                            GUILayout.Label(string.Format("Object Currently Selected Is: {0}", ReturnSelection().name), EditorStyles.boldLabel);
                        }

                        GUILayout.Space( 5 );

                        if (m_List != null && m_List.list != null){
                            m_List.DoLayoutList();
                        }

                        GUILayout.Space( 5 );

                        EditorGUILayout.EndVertical();
                    }
                }


            EditorGUILayout.EndVertical();
            }
        }




        #region ReorderableList
        //  -- Setup the lists of components.
        private void InitializeReorderableList()
        {
            m_Selection = ReturnSelection();
            //  Update the ILists with the GameObjects list of components.
            UpdateReorderableList(m_ComponentList, GetComponentList(m_Selection));
            UpdateReorderableList(m_CurrentComponents, GetComponentList(m_Selection));

            DrawReorderableList();
            DrawElementDrawer();
            OnReorderCallback();
        }


        //  -- Create the ReorderableList
        private void DrawReorderableList()
        {
            m_List = new ReorderableList(m_ComponentList, typeof(Component), true, true, false, false);

            m_List.drawHeaderCallback = (Rect rect) => {  
                EditorGUI.LabelField(rect, string.Format("List Of Components for {0}", m_Selection.name ) ) ;
            };
        }

        private void DrawElementDrawer()
        {
            m_List.drawElementCallback =  (Rect rect, int index, bool isActive, bool isFocused) => {

            Texture componentIcon = EditorGUIUtility.ObjectContent(null, m_List.list[index].GetType() ).image;
            Texture buttonIcon = EditorGUIUtility.IconContent("Toolbar Minus").image;


            GUI.Label(
                new Rect(rect.x, rect.y, 20, EditorGUIUtility.singleLineHeight),
                componentIcon
                );

            EditorGUI.LabelField(
                new Rect(rect.x + 20, rect.y, rect.width - 60, EditorGUIUtility.singleLineHeight),
                m_List.list[index].GetType().ToString()
                );

            };

        }


        //  --  Move the Components when an item is moved in the Reorderable List.
        private void OnReorderCallback()
        {
            m_List.onReorderCallback = (ReorderableList list) => {

                SortComponents(m_CurrentComponents, m_ComponentList);

                UpdateReorderableList(m_ComponentList, GetComponentList(m_Selection));
                UpdateReorderableList(m_CurrentComponents, GetComponentList(m_Selection));
            };
        }

        
        private void SortComponents(IList CurrentComponents, IList SortedComponents)
        {
            for (int Index = 0; Index < SortedComponents.Count; Index++)
            {
                Component SortedComponent = (Component)SortedComponents[Index];
                int CurrentIndex = CurrentComponents.IndexOf(SortedComponent);
                
                if (CurrentIndex < Index)
                {
                    for (int MoveIndex = CurrentIndex; MoveIndex < Index; MoveIndex++)
                        ComponentUtility.MoveComponentDown(SortedComponent);
                }
                else
                {
                    for (int MoveIndex = CurrentIndex; MoveIndex > Index; MoveIndex--)
                        ComponentUtility.MoveComponentUp(SortedComponent);
                }
            }
        }
        

        //  -- Unity Method.  Is used when selection has been changed.
        private void OnSelectionChange()
        {
            if(ReturnSelection() != null){
                //Debug.Log(string.Format("{0} is selected.", ReturnSelection().name ) );
                InitializeReorderableList();
            }
        }
        
        #endregion






		private GameObject ReturnSelection()
		{
			GameObject selection = Selection.activeGameObject;
			if (selection != null){
                m_Selection = selection;
				return selection;
			}
			else{
				return null;
			}
		}


		//  Get selected objects list of components.
		private Component[] GetComponentList(GameObject go)
		{
			Component[] cpnts = go.GetComponents(typeof(Component));
			return cpnts;
		}


        //  --  Used to update the list.  We make sure to clear it first so it doesn't keep adding.
		private void UpdateReorderableList(IList list, Component[] cpnts)
		{
            if (list != null)
            {
                list.Clear();
                foreach(Component c in cpnts)
                {   
                    if(c.GetType() != typeof(Transform) )
                    {
                        list.Add(c);
                    }
                }
            }
		}


        

		/****************************************
		 * GameObjectInspector Replication
		 * **************************************/
		public override void OnInspectorGUI(){
			m_DefaultEditor.OnInspectorGUI();
		}
		public override void ReloadPreviewInstances(){
			m_DefaultEditor.OnInspectorGUI();
		}
		public override bool HasPreviewGUI (){
			return m_DefaultEditor.HasPreviewGUI();
		}
		public override void OnPreviewSettings (){
			m_DefaultEditor.OnPreviewSettings();
		}
		// public override Texture2D RenderStaticPreview (string assetPath, Object[] subAssets, int width, int height){
		// 	return m_DefaultEditor.RenderStaticPreview(assetPath, subAssets, width, height);
		// }
		public override void OnPreviewGUI (Rect r, GUIStyle background){
			m_DefaultEditor.OnPreviewGUI(r,background);
		}
		public void OnSceneDrag(SceneView sceneView){
			m_DefaultEditor.GetType().GetMethod("OnSceneDrag").Invoke(m_DefaultEditor, new object[]{sceneView});
		}
		public void OnDestroy(){
	 		m_DefaultEditor.GetType().GetMethod("OnDestroy").Invoke(m_DefaultEditor, null);  
		}


    }    




    #region SelectionComponents
    [Serializable]
    class SelectionComponents : IList
    {
        public object[] _contents;
        private int _count;

        public SelectionComponents()
        {
            _count = 0;
        }

        // IList Members
        public int Add(object value)
        {
            if (_count < _contents.Length)
            {
                _contents[_count] = value;
                _count++;

                return (_count - 1);
            }
            else
            {
                return -1;
            }
        }

        public void Clear()
        {
            _count = 0;
        }

        public bool Contains(object value)
        {
            bool inList = false;
            for (int i = 0; i < Count; i++)
            {
                if (_contents[i] == value)
                {
                    inList = true;
                    break;
                }
            }
            return inList;
        }

        public int IndexOf(object value)
        {
            int itemIndex = -1;
            for (int i = 0; i < Count; i++)
            {
                if (_contents[i] == value)
                {
                    itemIndex = i;
                    break;
                }
            }
            return itemIndex;
        }

        public void Insert(int index, object value)
        {
            if ((_count + 1 <= _contents.Length) && (index < Count) && (index >= 0))
            {
                _count++;

                for (int i = Count - 1; i > index; i--)
                {
                    _contents[i] = _contents[i - 1];
                }
                _contents[index] = value;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return true;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Remove(object value)
        {
            RemoveAt(IndexOf(value));
        }

        public void RemoveAt(int index)
        {
            if ((index >= 0) && (index < Count))
            {
                for (int i = index; i < Count - 1; i++)
                {
                    _contents[i] = _contents[i + 1];
                }
                _count--;
            }
        }

        public object this[int index]
        {
            get
            {
                return _contents[index];
            }
            set
            {
                _contents[index] = value;
            }
        }

        // ICollection Members

        public void CopyTo(Array array, int index)
        {
            int j = index;
            for (int i = 0; i < Count; i++)
            {
                array.SetValue(_contents[i], j);
                j++;
            }
        }

        public int Count{
            get{return _count; }
        }

        public bool IsSynchronized{
            get{return false; }
        }

        // Return the current instance since the underlying store is not
        // publicly available.
        public object SyncRoot{
            get{return this; } }

        // IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            // Refer to the IEnumerator documentation for an example of
            // implementing an enumerator.
            throw new Exception("The method or operation is not implemented.");
        }

        public void PrintContents()
        {
            Console.WriteLine("List has a capacity of {0} and currently has {1} elements.", _contents.Length, _count);
            Console.Write("List contents:");
            for (int i = 0; i < Count; i++)
            {
                Console.Write(" {0}", _contents[i]);
            }
            Console.WriteLine();
        }
    }
    #endregion

}