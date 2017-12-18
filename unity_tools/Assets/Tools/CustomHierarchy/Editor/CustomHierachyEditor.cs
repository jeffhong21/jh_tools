using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
 
namespace JH_Tools
{
	[InitializeOnLoad]
	class CustomHierachyEditor
	{
		// only show these types if showAllTypes is false
		static Type[] componentTypes = new Type[]
		{
			typeof(Light),
			typeof(Camera),
			typeof(ParticleSystem),
			typeof(TrailRenderer),
			typeof(MeshRenderer),
			typeof(MeshFilter),
			typeof(SpriteRenderer),
			typeof(AudioSource),
			typeof(Canvas),
			typeof(CanvasGroup),
			typeof(UnityEngine.EventSystems.EventSystem),
			typeof(UnityEngine.UI.Image),
			typeof(UnityEngine.UI.Button),
			typeof(UnityEngine.UI.Text),
		};


		static bool showActiveToggle = true;

		static List<GameObject> allChildGOsForToggle;
		static bool toggleActiveAllChildInheritToggleObject = true; // Attention: if hold alt and clickin on object toggle, it will set all childs active state to the same of the clicked object


		static CustomHierachyEditor ()
		{
			EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
		}



		static void HierarchyWindowItemOnGUI (int instanceID, Rect selectionRect)
		{
			UnityEngine.Object obj = EditorUtility.InstanceIDToObject(instanceID);
			if (obj == false)
				return;
			GameObject go = EditorUtility.InstanceIDToObject (instanceID) as GameObject;
			Rect rect = new Rect (selectionRect); 

			//  - NEEDS ATTENTION.  DUCT TAPED TOGETHER.
			//  Highlight GameObject
			if(EditorPrefs.GetInt(go.GetInstanceID() + "Highlight") == 1){
				GUIStyle style = new GUIStyle(); 
				style.normal.textColor = Color.black;

				Rect highlightRect = new Rect (selectionRect); 
				highlightRect.x = 1;
				highlightRect.width = EditorGUIUtility.currentViewWidth - 4;
				highlightRect.height = EditorGUIUtility.singleLineHeight;

				Color oldColor = GUI.color;
				GUI.color = Color.magenta;
				EditorGUI.LabelField(highlightRect, GUIContent.none, EditorStyles.helpBox );
				GUI.color = oldColor;
				highlightRect.x = selectionRect.x;
				highlightRect.y = selectionRect.y + 1.5f;
				EditorGUI.LabelField(highlightRect,new GUIContent(go.name) , style);
			}


			//  -- Icon 
			rect.x = 2;
			rect.width = rect.height = 18;

			var components = go.GetComponents<Component>();
			if(components != null){
				//  If game object has more than transform
				if(components.Length > 1){
					//  See if a gameobject contains a certain type from componentTypes.
					Type type = null;;
					foreach(Component c in components){
						if(componentTypes.Contains(c.GetType() ) ){
							type = c.GetType();
							continue;
						}
					}
					if(type == null){
						type = components[1].GetType();
					}
					
					Texture icon = AssetPreview.GetMiniTypeThumbnail(type);
					bool isInComponentTypesList = Array.Exists(componentTypes, x => x == type);

					if (type == typeof(Canvas)){
						GUI.Label(rect, EditorGUIUtility.IconContent("Canvas Icon") );
					}
					else if(IsDisconnectedPrefabInstance(go)){
						GUI.Label(rect, EditorGUIUtility.IconContent("Prefab Icon"));
					}
					else if(CanShowAsPrefab(go)){
						GUI.Label(rect, EditorGUIUtility.IconContent("PrefabNormal Icon"));
					}
					else if(isInComponentTypesList){
						GUI.Label(rect, new GUIContent(icon) );
					}
					else{
						GUI.Label(rect, new GUIContent(AssetPreview.GetMiniTypeThumbnail(typeof(GameObject) ) ) );  
					}
				}
				//  If object is empty object.
				else{
					if(CanShowAsPrefab(go)){
						GUI.Label(rect, EditorGUIUtility.IconContent("PrefabNormal Icon"));
					}
					else{
						GUI.Label(rect, new GUIContent(AssetPreview.GetMiniTypeThumbnail(typeof(GameObject) ) ) );  
					}
				}
			}




			//  -- Warning Icon if something is missing. ** Need to add a way to check for missing dependencies.
			if (IsDisconnectedPrefabInstance(go) && IsRootTransform(go)){
				rect.x = EditorGUIUtility.labelWidth + EditorGUIUtility.currentViewWidth / 15;
				rect.y -= 2;
				rect.width = rect.height = 24;
				GUI.Label(rect, EditorGUIUtility.IconContent("console.warnicon.sml") );
			}



			//  -- Set the Enable/Disable of the gameobject to GUI Toggle.
			rect.x = EditorGUIUtility.currentViewWidth - 28;
			rect.y = selectionRect.y;
			rect.width = rect.height = 18;

			EditorGUI.BeginChangeCheck();
			
			bool toggleValue = GUI.Toggle(rect, go.activeSelf, string.Empty);
			//  If alt key is also pressed.
			if (toggleActiveAllChildInheritToggleObject && Event.current.alt){
				if (EditorGUI.EndChangeCheck()){
					if (allChildGOsForToggle == null) allChildGOsForToggle = new List<GameObject>();
					allChildGOsForToggle.Clear();
					allChildGOsForToggle.Add(go);
					FetchAllChildGameObjects(go.transform);
					Undo.RecordObjects(allChildGOsForToggle.ToArray(), "Toggle GameObject Active");
					allChildGOsForToggle.ForEach(child => child.SetActive(toggleValue));
				}
			}
			else if (EditorGUI.EndChangeCheck()){
				go.SetActive(toggleValue);
				Undo.RecordObject(obj, "Toggle " + obj.name + " Active");
			}


			//  - NEEDS ATTENTION.
			//  --  Special component buttons (ParticleSystem, Audio, MeshRender)
			// if(go.GetComponent<MeshRenderer>()){
			// 	rect.x -= 15;
			// 	Texture icon = AssetPreview.GetMiniTypeThumbnail(typeof(MeshRenderer));
			// 	bool toggleMesh = GUI.Toggle(rect, go.GetComponent<MeshRenderer>().enabled, new GUIContent(icon));
			// }


			//  Markers
			if(EditorPrefs.GetInt(go.GetInstanceID() + "G") == 1){
				rect.x -= 15;
				GUI.Label(rect, "G");
			}


		}


		public static void FetchAllChildGameObjects(Transform transform)
		{
			if (allChildGOsForToggle == null) allChildGOsForToggle = new List<GameObject>();
			foreach (Transform t in transform){
				if ( !allChildGOsForToggle.Contains(t.gameObject)) allChildGOsForToggle.Add(t.gameObject);
				FetchAllChildGameObjects(t);
			}
		}

		static bool IsRootTransform(GameObject go){
			return go.transform == go.transform.root;
		}

		static bool IsPartOfPrefab(GameObject prefab){
			return PrefabUtility.GetPrefabParent(prefab) != null;
		}
	
		static bool IsPrefabParent(GameObject prefab){
			return PrefabUtility.FindPrefabRoot(prefab) == prefab;
		}

		static bool IsPrefabInstance(GameObject prefab){
			return PrefabUtility.GetPrefabParent(prefab) != null && PrefabUtility.GetPrefabObject(prefab.transform) != null;
		}

		static bool IsPrefabOriginal(GameObject prefab){
			return PrefabUtility.GetPrefabParent(prefab) == null && PrefabUtility.GetPrefabObject(prefab.transform) != null;
		}

		static bool IsDisconnectedPrefabInstance(GameObject prefab){
			return PrefabUtility.GetPrefabParent(prefab) != null && PrefabUtility.GetPrefabObject(prefab.transform) == null;
		}

		static bool CanShowAsPrefab(GameObject prefab)
		{
			if (IsPrefabParent(prefab) &&
				IsPartOfPrefab(prefab) &&
				PrefabUtility.GetPrefabType(prefab) == PrefabType.PrefabInstance ||
				PrefabUtility.GetPrefabType(prefab) == PrefabType.ModelPrefabInstance)
			{
				return true;
			}

			return false;
		}





		[MenuItem("GameObject/Marker/GameManager", false, 12)]
		static void AddGameManagerMarker()
		{
			foreach(UnityEngine.Object o in Selection.gameObjects){
				if(EditorPrefs.GetInt(o.GetInstanceID() + "G") == 0){
					EditorPrefs.SetInt(o.GetInstanceID() + "G", 1);
				}
				else{
					EditorPrefs.SetInt(o.GetInstanceID() + "G", 0);
				}
			}
		}

		[MenuItem("GameObject/Highlight Object", false, 13)]
		static void HighlightObject()
		{
			foreach(UnityEngine.Object o in Selection.gameObjects){
				if(EditorPrefs.GetInt(o.GetInstanceID() + "Highlight") == 0){
					EditorPrefs.SetInt(o.GetInstanceID() + "Highlight", 1);
				}
				else{
					EditorPrefs.SetInt(o.GetInstanceID() + "Highlight", 0);
				}
			}
		}



		static void MouseClick(GameObject go, Rect rect)
		{
			if (Event.current != null && rect.Contains(Event.current.mousePosition)
				&& Event.current.button == 0 && Event.current.type <= EventType.mouseUp)
			{
				if (go)
				{
					Vector2 mousePosition = Event.current.mousePosition;
					EditorUtility.DisplayPopupMenu(new Rect(mousePosition.x, mousePosition.y, 0, 0), "Window/Test",null);
					Debug.Log(string.Format("Object Selected is {0}\nRect X:  {1}\nRect Y:  {2}\nRect Width:  {3}\nRect Height:  {4}", go.name, rect.x, rect.y, rect.width, rect.height)  );
					//Event.current.Use();
				}			
			}
		}

	}
}



