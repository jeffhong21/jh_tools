using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JH_Tools
{
	public class ReorderComponentsEditor : EditorWindow
	{

		private GameObject m_Selection;
        //  Base IList of Components to compare CurrentComponents too.
        private IList m_ComponentList = new List<Component>();
         // Get IList of current Component order to compare againt base ComponentList before OnReorderCallback is called.
        private IList m_CurrentComponents = new List<Component>();
		private ReorderableList m_List;


		#region Window

		private static Vector2 windowMinSize = new Vector2(300, 450);
		private static Rect listRect = new Rect(Vector2.zero, windowMinSize);

		[MenuItem("JH Tools/Reorder Components")]
		private static void OpenWindow()
		{
			ReorderComponentsEditor window = (ReorderComponentsEditor)GetWindow(typeof(ReorderComponentsEditor));
			window.titleContent = new GUIContent("Reorder Components");
			window.minSize = windowMinSize;
			window.Show();

		}



		#endregion


		private void OnEnable()
		{
            m_Selection = ReturnSelection();

            if (m_Selection == null){
                return;
            }

            InitializeReorderableList();
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
            
            //  Used after onReorderCallback to arrange the components
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
        #endregion


        //  -- Unity Method.  Is used when selection has been changed.
        private void OnSelectionChange()
        {
            if(ReturnSelection() != null){
                //Debug.Log(string.Format("{0} is selected.", ReturnSelection().name ) );
                InitializeReorderableList();
            }
            // else{
            //     Debug.Log("New Selection");
            // }
        }


        private void OnGUI()
        {
            if (ReturnSelection() != null)
            {
                if (m_List == null || m_List.list == null)
                {
                    try{
                        InitializeReorderableList();
                    }
                    catch{
                        DebugReorderableListNull();
                    }
                }
                else
                {
                    GUILayout.Space( 5 );
                    EditorGUILayout.BeginVertical(new GUIStyle("HelpBox"), GUILayout.Height(windowMinSize.y));

                    if(ReturnSelection() != null){
                        GUILayout.Label(string.Format("Object currently selected is: {0}", ReturnSelection().name));
                    }

                    GUILayout.Space( 5 );

                    if (m_List != null && m_List.list != null){
                        m_List.DoLayoutList();
                    }

                    GUILayout.Space( 5 );

                    if(ReturnSelection() != null){
                        EditorGUILayout.HelpBox(DisplayListOfComponents(m_ComponentList), MessageType.Info);
                    }

                    EditorGUILayout.EndVertical();
                }
            }
            
        }





		//  Get selected object.
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


		private void UpdateReorderableList(IList list, Component[] cpnts)
		{
            if (list != null)
            {
                list.Clear();
                foreach(Component c in cpnts)
                {   
                    if(c.GetType() != typeof(Transform) )
                    {
                        //string cpntName = c.GetType().ToString();
                        //list.Add(cpntName);
                        list.Add(c);
                    }
                }
                // Repaint();
            }
		}


		private string DisplayListOfComponents(IList cpnts)
		{
			string componentsString = "List of Components\n";

            for(int i = 0; i < cpnts.Count; i ++)
            {
                componentsString += cpnts[i].ToString() + "\n";
            }

			return componentsString;
		}

        
        private void PrintListOfComponents(string header, IList components)
        {   
            string list = header + "\n";
            for(int i = 0; i < components.Count; i++)
            {
                list += components[i] + "\n";
            }
            Debug.Log(list);
        }

        private void DebugReorderableListNull()
        {
            if (m_List == null)
            {
                Debug.LogError("Either ReorderableList is null");
            }
            if (m_List.list == null)
            {
                Debug.LogError("ReorderableList points to a null list");
            }
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

        public int Count
        {
            get
            {
                return _count;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        // Return the current instance since the underlying store is not
        // publicly available.
        public object SyncRoot
        {
            get
            {
                return this;
            }
        }

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

    // This code produces output similar to the following:
    // Populate the List:
    // List has a capacity of 8 and currently has 8 elements.
    // List contents: one two three four five six seven eight
    //
    // Remove elements from the list:
    // List has a capacity of 8 and currently has 6 elements.
    // List contents: one two three four five seven
    //
    // Add an element to the end of the list:
    // List has a capacity of 8 and currently has 7 elements.
    // List contents: one two three four five seven nine
    //
    // Insert an element into the middle of the list:
    // List has a capacity of 8 and currently has 8 elements.
    // List contents: one two three four number five seven nine
    //
    // Check for specific elements in the list:
    // List contains "three": True
    // List contains "ten": False
    #endregion


}




