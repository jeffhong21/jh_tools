using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections;
using System.Collections.Generic;


namespace JH_Tools
{

	public class ReorderComponentsEditor : EditorWindow
	{

		private GameObject m_Selection;


        private IList m_ComponentList;

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

            Debug.Log("Creating Reorderable Lists");

            m_ComponentList = new List<string>();
            UpdateReorderableList(GetComponentList(m_Selection));

            m_List = new ReorderableList(m_ComponentList, typeof(string), true, true, false, false);
            

            m_List.drawHeaderCallback = (Rect rect) => {  
                EditorGUI.LabelField(rect, string.Format("List Of Components for {0}", m_Selection.name ) ) ;
            };
            
		}


        #region ReorderableList Element Drawer
		// private void SetupElementDrawer()
		// {
		// 	m_List.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => 
		// 	{
		// 		var element = m_List.serializedProperty.GetArrayElementAtIndex(index);
		// 		rect.y += 2;

		// 		EditorGUI.PropertyField(
		// 			new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight), m_List.serializedProperty.GetArrayElementAtIndex(index), GUIContent.none);
		// 		EditorGUI.PropertyField(
		// 			new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
		// 			element.FindPropertyRelative("_contents"), GUIContent.none);

		// 		EditorGUI.PropertyField(
		// 			new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight),
		// 			element.FindPropertyRelative("CanDelete"), GUIContent.none);
		// 	};
		// }
        #endregion


        private void OnGUI()
        {
            if (m_List == null)
            {
                Debug.LogError("ReorderableList is null");
            }
            else if (m_List.list == null)
            {
                Debug.LogError("ReorderableList points to a null list");
            }

            if (ReturnSelection() != null)
            {
                GUILayout.Space( 5 );
                EditorGUILayout.BeginVertical(new GUIStyle("HelpBox"), GUILayout.Height(windowMinSize.y));

                if(ReturnSelection() != null){
                    //  Update List List.
                    UpdateReorderableList(GetComponentList(m_Selection));

                	GUILayout.Label(string.Format("Object currently selected is: {0}", ReturnSelection().name));
                    
                	//EditorGUILayout.HelpBox(DisplayComponentList(GetComponentList(m_Selection)), MessageType.Info);

                	GUILayout.Space( 5 );
                }


                if (m_List != null){
                    m_List.DoLayoutList();
                }
                
                EditorGUILayout.EndVertical();
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


		private void UpdateReorderableList(Component[] cpnts)
		{
            if (m_ComponentList != null)
            {
                m_ComponentList.Clear();
                foreach(Component c in cpnts)
                {
                    string cpntName = c.GetType().ToString();
                    m_ComponentList.Add(cpntName);
                }
                Repaint();
            }
		}





		private string DisplayComponentList(Component[] cpnts)
		{
			string componentsString = "List of Components\n";
			foreach(Component c in cpnts)
			{
				componentsString += c.GetType().ToString() + "\n";
			}
			return componentsString;
		}

	}


	struct ReorderableListParams
	{
		public bool draggable;
		public bool displayHeader;
		public bool displayAddButton;
		public bool displayRemoveButton;
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




