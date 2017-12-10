using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;


namespace JH_Tools
{


	public class ReorderComponentsEditor : EditorWindow
	{
		private GameObject m_GameObject;
		private SerializedObject m_SerializedObject;
		private ReorderableList m_List;

		//private List<SourceComponent> m_ComponentList = new List<SourceComponent>();
		//private List<SourceComponent> m_ComponentList;
		private ComponentData m_ComponentData;


		private static Vector2 windowMinSize = new Vector2(300, 450);
		private static Rect listRect = new Rect(Vector2.zero, windowMinSize);

		[MenuItem("JH Tools/Reorder Components")]
		private static void OpenWindow()
		{
			ReorderComponentsEditor window = (ReorderComponentsEditor)GetWindow(typeof(ReorderComponentsEditor));
			window.titleContent = new GUIContent("Reorder Components");
			window.minSize = windowMinSize;
			window.Show();

			GUI.BringWindowToFront(0);

		}


		private void OnEnable()
		{
			m_ComponentData = FindObjectOfType<ComponentData>();

			// if (m_GameObject == null){
			// 	return;
			// }

			//UpdateReorderableList(GetComponentList(m_GameObject));

			CreateReorderableList();
			SetupHeaderDrawer();
			SetupElementDrawer();
		}


		private void CreateReorderableList()
		{
			m_SerializedObject = new SerializedObject(m_ComponentData);
			m_List = new ReorderableList(m_SerializedObject, m_SerializedObject.FindProperty("ComponentList"), true, true, false, false);
		}


		private void SetupHeaderDrawer()
		{
			m_List.drawHeaderCallback = (Rect rect) => {  
				EditorGUI.LabelField(rect, string.Format("List Of Components for" ) ) ;
			};
		}


		private void SetupElementDrawer()
		{
			m_List.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => 
			{
				var element = m_List.serializedProperty.GetArrayElementAtIndex(index);
				rect.y += 2;


				EditorGUI.PropertyField(
					new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
					element.FindPropertyRelative("ComponentName"), GUIContent.none);

				EditorGUI.PropertyField(
					new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight),
					element.FindPropertyRelative("CanDelete"), GUIContent.none);
			};
		}


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
			else
			{
				GUILayout.Space( 5 );
				EditorGUILayout.BeginVertical(new GUIStyle("HelpBox"), GUILayout.Height(windowMinSize.y));

				if( DisplaySelection() != null){
					GUILayout.Label(string.Format("Object currently selected is:  {0}", DisplaySelection().name));
					// EditorGUILayout.HelpBox(DisplayComponentList(), MessageType.Info);
					GUILayout.Space( 5 );
					
				}
				//m_SerializedObject.Update();
				m_List.DoLayoutList();
				//m_SerializedObject.ApplyModifiedProperties();
				

				EditorGUILayout.EndVertical();
			}



        }

		//  Get selected object.
		private GameObject DisplaySelection()
		{
			GameObject selection = Selection.activeGameObject;
			if (selection != null){
				return selection;
			}
			else{
				return null;
			}
		}

		//  Get selected objects list of components.
		private Component[] GetComponentList(GameObject go)
		{
			Component[] components = go.GetComponents(typeof(Component));
			return components;
		}


		// private void UpdateReorderableList(Component[] components)
		// {
		// 	m_ComponentList.Clear();
		// 	foreach(Component c in components)
		// 	{
		// 		SourceComponent srcComponent = new SourceComponent();
		// 		srcComponent.ComponentName = c.GetType().ToString();

		// 		m_ComponentList.Add(srcComponent);
		// 	}
		// }


		// private string DisplayComponentList()
		// {
		// 	string componentsString = "List of Components\n";
		// 	foreach(SourceComponent sc in m_ComponentList)
		// 	{
		// 		componentsString += sc.ComponentName + "\n";
		// 	}

		// 	return componentsString;
		// }

	}


    // [System.Serializable]
    // public struct SourceComponent
    // {
    //     public string ComponentName;
    //     public bool CanDelete;
    // }

	// [System.Serializable]
	// public class ComponentData
	// {
	// 	public List<SourceComponent> ComponentList = new List<SourceComponent>();
	// }

}




