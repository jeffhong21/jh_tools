// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEditor;
// using UnityEditorInternal;


// namespace JH_Tools
// {
// 	public class ReorderComponentsEditor : EditorWindow
// 	{
		
// 		private ReorderableList list;

// 		public ReorderableList(  
// 			SerializedObject serializedObject, 
// 			SerializedProperty elements, 
// 			bool draggable, 
// 			bool displayHeader, 
// 			bool displayAddButton, 
// 			bool displayRemoveButton);


// 		[MenuItem("JH Tools/ Reorder Components")]
// 		private static void OpenWindow()
// 		{
// 			ReorderComponentsEditor window = (ReorderComponentsEditor)GetWindow(typeof(ReorderComponentsEditor));

// 			window.titleContent = new GUIContent("Reorder Components");
// 			window.minSize = new Vector2(300, 350);
// 			window.maxSize = new Vector2(500, 350);
// 			window.Show();

// 			//window.ShowUtility();
// 			GUI.BringWindowToFront(0);

// 		}


//         private void OnGUI()
//         {
// 			GUILayoutOption GUIHeight = GUILayout.Height(Screen.height);
//             EditorGUILayout.BeginVertical(new GUIStyle("HelpBox"), GUIHeight);

            




//             EditorGUILayout.EndVertical();
//         }



// 		private void OnEnable()
// 		{
// 			list = new ReorderableList( 
// 						serializedObject, 
// 						serializedObject.FindProperty("srcComponents"), 
// 						true, true, true, true);


// 			list.drawElementCallback =  
// 				(Rect rect, int index, bool isActive, bool isFocused) => {
// 				var element = list.serializedProperty.GetArrayElementAtIndex(index);
// 				rect.y += 2;
// 				EditorGUI.PropertyField(
// 					new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
// 					element.FindPropertyRelative("Type"), GUIContent.none);
// 				EditorGUI.PropertyField(
// 					new Rect(rect.x + 60, rect.y, rect.width - 60 - 30, EditorGUIUtility.singleLineHeight),
// 					element.FindPropertyRelative("Prefab"), GUIContent.none);
// 				EditorGUI.PropertyField(
// 					new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight),
// 					element.FindPropertyRelative("Count"), GUIContent.none);
// 			};
// 		}








// 	}

// 	[Serializable]
// 	public class TargetComponents
// 	{
// 		public List<Component> srcComponents = new List<Component>();
// 	}

// }



