using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ComponentToolPanel
{
    [CustomEditor(typeof(GameObject), true)]      
    [CanEditMultipleObjects]
	public class ComponentToolPanel : Editor
	{
        //  "UnityEditor.GameObjectInspector, UnityEditor" is a custom internal script that draws the GameObjects Inspector.
        const string m_GameObjectInspectorTypeName = "UnityEditor.GameObjectInspector, UnityEditor";

		#region Component Dependencies Dictionary
        //  Some components come with other compoenets, so we need to delete components that com e with these com ponents.
		static Dictionary<Type, List<Type>> componentDependencies = new Dictionary<Type, List<Type>>(){
			{
				typeof(Camera), new List<Type>(){
					typeof(GUILayer), 
					typeof(FlareLayer)
				}
			},
			{
				typeof(MeshRenderer), new List<Type>(){
					typeof(TextMesh)
				}
			},
			{
				typeof(ParticleSystemRenderer), new List<Type>(){
					typeof(ParticleSystem)
				}
			},
			{
				typeof(SkinnedMeshRenderer), new List<Type>(){
					typeof(Cloth)
				}
			},
			{
				typeof(Rigidbody), new List<Type>(){
					typeof(HingeJoint),
					typeof(FixedJoint),
					typeof(SpringJoint),
					typeof(CharacterJoint),
					typeof(ConfigurableJoint),
					typeof(ConstantForce)
				}
			},
			{
				typeof(Rigidbody2D), new List<Type>(){
	#if UNITY_5_3 || UNITY_5_4
					typeof(FixedJoint2D),
					typeof(RelativeJoint2D),
					typeof(FrictionJoint2D),
					typeof(TargetJoint2D),
	#endif
					typeof(DistanceJoint2D),
					typeof(HingeJoint2D),
					typeof(SliderJoint2D),
					typeof(SpringJoint2D),
					typeof(WheelJoint2D),
					typeof(ConstantForce2D)
				}
			},
			{
				typeof(RectTransform), new List<Type>(){
					typeof(Canvas)
				}
			},
			{
				typeof(Canvas), new List<Type>(){
					typeof(CanvasScaler)
				}
			},   
	#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4
			{
				typeof(NetworkIdentity), new List<Type>(){
					typeof(NetworkAnimator)
				}
			},
	#endif
			{
				typeof(CanvasRenderer), new List<Type>(){
					typeof(Text),
					typeof(Image),
					typeof(RawImage)
				}
			}
		};
		#endregion

		GameObject[] m_GameObjects;
        Editor m_DefaultEditor;
        ReorderableList m_List;
        Transform m_Transform;
        List<Component> m_Components;
        bool m_Foldout;




        void OnEnable(){
            m_DefaultEditor = Editor.CreateEditor(targets, Type.GetType(m_GameObjectInspectorTypeName));
            m_Components = new List<Component>();
			m_GameObjects = new GameObject[targets.Length];
			//m_List = new ReorderableList[targets.Length];
            m_Foldout = true;
			// for (int i = 0; i < targets.Length; i++){
			// 	InitializeList(i);
			// }
			InitializeList(0);
        }


		void OnDisable(){
			//This avoids leaks
            //Debug.Log("Disabling Editor");
			DestroyImmediate(m_DefaultEditor); 
		} 


        //  We're overriding Unity's internal script GameObjectInspector.cs's OnHeaderGUI method.
        protected override void OnHeaderGUI(){
            //  Draw the default Unity inspectorGUI.  Other wise it will be empty.
            m_DefaultEditor.DrawHeader();

			// // -- Currently does not work because when the editor Disables which causes the selection order to reorder. --
			// for(int i = 0; i < Selection.gameObjects.Length; i ++){
			// 	m_GameObjects[i] = Selection.gameObjects[i];
			// }

			GUILayout.Space(2);

			GUIStyle style = new GUIStyle(EditorStyles.foldout);
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 12;


			//  Custom GUI code here
			m_Foldout = GUILayout.Toggle (m_Foldout, new GUIContent("Component Tool Panel : " + targets[0].name)  , style); //ShurikenModuleTitle

			if(m_Foldout){
				//  Setting up the rect for the controls.
				float height =m_List.elementHeight * m_Components.Count + m_List.headerHeight + 20; //  + m_GameObjects.Length * 12;   //20 = extra space for add button
				if ( m_Components.Count == 0){
					height +=m_List.elementHeight;
				}
				Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(height));
				rect.x+=11;
				rect.width-=11;

				//  Draw List
				m_List.DoList(rect);
			}
			



			// // -- Currently does not work because when the editor Disables which causes the selection order to reorder. --
			// //  Selected Objects.  
			// if(m_Foldout && m_GameObjects.Length > 1){
			// 	EditorGUILayout.BeginVertical();
			// 	//  Need to setup rect for controls.  Otherwise it starts to spill over on top of the transform component
			// 	float height = EditorGUIUtility.singleLineHeight * m_GameObjects.Length + 12; 
			// 	Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(height));
			// 	rect.y -= 12;
			// 	EditorGUI.LabelField(rect, "Selected Objects");
			// 	for(int i = 1; i < m_GameObjects.Length; i ++){
			// 		rect.y += 20;//EditorGUIUtility.singleLineHeight;
			// 		rect.height = 18;//EditorGUIUtility.singleLineHeight;
			// 		rect.width = EditorGUIUtility.currentViewWidth / 2 - 4;
			// 		Color oldColor = GUI.color;
			// 		GUI.color = Color.grey;
			// 		//EditorGUILayout.LabelField(GUIContent.none,EditorStyles.helpBox, GUILayout.Height(18), GUILayout.Width(50) );
			// 		EditorGUI.LabelField(rect, GUIContent.none, EditorStyles.helpBox );
			// 		GUI.color = oldColor;
			// 		//rect = GUILayoutUtility.GetLastRect();
			// 		EditorGUI.LabelField(rect,new GUIContent(m_GameObjects[i].name + string.Format(" : [ {0} ] ", i), EditorGUIUtility.ObjectContent(m_GameObjects[i], m_GameObjects[i].GetType()).image));
			// 	}
			// 	EditorGUILayout.EndVertical();
			// }

        }

		//  Initializes the reorderable list.
        void InitializeList(int index)
        {
            //  Set inspector
            GameObject gameObject =  m_GameObjects[index] = (GameObject)targets[index];
            m_Components = gameObject.GetComponents(typeof(Component)).ToList();
            //  Remove Transform component
            m_Components.RemoveAt(0);
            
            m_List = new ReorderableList(m_Components, typeof(Component), true, true, false, false);

            m_List.drawHeaderCallback = (Rect rect) => {
                DrawElements(rect, gameObject.transform);
            };

            m_List.drawElementCallback =  (Rect rect, int _index, bool isActive, bool isFocused) => {
                DrawElements(rect, m_Components[_index]);
            };

            //  What happens when the lists changes.
            m_List.onReorderCallback = (ReorderableList internalList) => {
	   			//Move Down
				for (int i = 0; i < m_Components.Count; i++){
					int listIndex = internalList.list.IndexOf(m_Components[i]);
					int difference = listIndex - i;
					if (difference>0)
						for (int j = 0; j<Mathf.Abs(difference); j++)
							ComponentUtility.MoveComponentDown(m_Components[i]);
				}
				//Move Up
				m_Components = new List<Component>(gameObject.GetComponents<Component>());
				m_Components.RemoveAt(0);
				for (int i = m_Components.Count-1; i >=0;i--){
					int listIndex = internalList.list.IndexOf(m_Components[i]);
					int difference = listIndex - i;
					if (difference<0)
						for (int j = 0; j<Mathf.Abs(difference); j++)
							ComponentUtility.MoveComponentUp(m_Components[i]);
				}
			};


        }

		//  Draws what the list looks like.
        void DrawElements(Rect originalRect, Component component){

            Rect rect = new Rect(originalRect);

            bool isRectTransform = component is RectTransform;
            bool isTransform = component is Transform || isRectTransform;

			if (isTransform){
				rect.x+=14;
				rect.y-=1;
			}

            //  Component Icon
            rect.x += 5;
            rect.width = rect.height = 20;
            Texture componentIcon = EditorGUIUtility.ObjectContent(null,component.GetType() ).image;
            EditorGUI.LabelField(rect, new GUIContent(componentIcon) );


			//Enable/Disable Toggle Handler
            rect.x += 25;
            rect.y += 2;
            if(EditorUtility.GetObjectEnabled(component) > -1){
				bool oldValue = EditorUtility.GetObjectEnabled(component)==1?true:false;
				bool newValue = EditorGUI.Toggle(rect, oldValue);
				if (oldValue != newValue){
					Component[] _targets = GetTargetComponents(component, GetTargetComponentMode.AllowMultiComponent | GetTargetComponentMode.AllowMultiGameObject | GetTargetComponentMode.ExcludeDifferentTypes | GetTargetComponentMode.IncludeTransforms);
					for (int i = 0; i < _targets.Length; i++){
						Undo.RecordObject(_targets[i], (newValue?"Enable" : "Disable ")+_targets[i].GetType().Name);
						EditorUtility.SetObjectEnabled(_targets[i], newValue);
					}
				}
			}


            //  Component name.
            rect.x += 20;
            //rect.y = originalRect.y;
            rect.width = originalRect.width - 125;
            EditorGUI.LabelField(rect,component.GetType().ToString());



			//Remove Button
            if ( ! isTransform || isRectTransform){
                rect.x = originalRect.width;
                rect.y += 1;
                rect.width = rect.height = 16;
                Texture buttonIcon = EditorGUIUtility.IconContent("TL Close button act").image;
                if (GUI.Button(rect, new GUIContent(buttonIcon), GUIStyle.none) ){
                    Debug.Log(string.Format("Deleting Component:  {0}", component.GetType().ToString() ) );

					Component[] _targets = GetTargetComponents(component, GetTargetComponentMode.AllowMultiComponent | GetTargetComponentMode.AllowMultiGameObject | GetTargetComponentMode.ExcludeDifferentTypes);
					for (int i = 0; i < _targets.Length; i++){
						string dependants = GetComponentDependants(_targets[i]);
						if (dependants == string.Empty){
							Undo.SetCurrentGroupName("Remove "+_targets[i].GetType().Name);
							Undo.DestroyObjectImmediate(_targets[i]);
						}
						else
							EditorUtility.DisplayDialog("Can't remove component", "Can't remove "+ _targets[i].GetType().Name+" because "+dependants+" depends on it","Ok");
					}
					EditorGUIUtility.ExitGUI();
				}
				if (isRectTransform)
					rect.y+=2;
			}
			if (isTransform)
				rect.y-=1;


        }






		/****************************************
		 * Utilities
		 * **************************************/

		//Utility method for getting components dependants
		string GetComponentDependants(Component component){
			Type type = component.GetType();
			Component[] components = component.GetComponents<Component>();
			HashSet<string> dependants = new HashSet<string>();
			bool hasBuiltInDependants = componentDependencies.Keys.Contains(type);
			for (int i = 1; i < components.Length; i++){
				if (components[i] == component)
					continue;
				if (components[i] == null)
					continue;

				//First, check if any other built-in attached component depends on it
				if (hasBuiltInDependants && componentDependencies[type].Contains(components[i].GetType()))
					dependants.Add(components[i].GetType().Name);

				//Then, find [RequireComponent] Attributes
				RequireComponent[] attribute = (RequireComponent[])components[i].GetType().GetCustomAttributes(typeof(RequireComponent), true);
				if (attribute.Length == 0)
					continue;
				if (type.IsAssignableFrom(attribute[0].m_Type0) || (attribute[0].m_Type1 != null && type.IsAssignableFrom(attribute[0].m_Type1)) || (attribute[0].m_Type2 != null && type.IsAssignableFrom(attribute[0].m_Type2)))
					dependants.Add(components[i].GetType().Name);
			}

			//And convert to string for better display 
			string text = string.Empty;
			for (int i = 0; i < dependants.Count; i++){
				text+=dependants.ToList()[i];
				if (i != dependants.Count-1)
					text+=", ";
			}
			return text;
		}

        

		[Flags]
		enum GetTargetComponentMode{
			None = 0,
			IncludeTransforms = 1,
			ExcludeDifferentTypes = 2,
			AllowMultiComponent = 4,
			AllowMultiGameObject = 8,
		}


		Component[] GetTargetComponents(Component mainTarget, GetTargetComponentMode mode){
			List<Component> targets = new List<Component>();
			if (mode == GetTargetComponentMode.None)
				return targets.ToArray();

			bool multiComponent = (Event.current.control || Event.current.command) && ((mode & GetTargetComponentMode.AllowMultiComponent)==GetTargetComponentMode.AllowMultiComponent);
			bool multiGameObject = Event.current.shift && ((mode & GetTargetComponentMode.AllowMultiGameObject) == GetTargetComponentMode.AllowMultiGameObject);


            // if (m_GameObjects != mainTarget.gameObject && !multiGameObject)
            //     return;

            // "<=" made on purpose in order to include Transforms if needed
            for (int j = 0; j <= m_Components.Count; j++){
                Component target;
                try{
                    target = m_Components[j];
                }catch{
                    if ((mode & GetTargetComponentMode.IncludeTransforms) != GetTargetComponentMode.IncludeTransforms)
                        break;
                    target = m_Transform;
                }

                if (target == null)
                    continue;
                if (mainTarget.GetType() != target.GetType() && ((mode & GetTargetComponentMode.ExcludeDifferentTypes) == GetTargetComponentMode.ExcludeDifferentTypes))
                    continue;
                if (!multiComponent && mainTarget.GetType() != target.GetType())
                    continue;
                targets.Add(target);
                if (!multiComponent)
                    break;
            }
			

			return targets.ToArray();
		}





		/****************************************
		 * GameObjectInspector Replication
		 * **************************************/
        //  We need an empty OnIspectorGUI() else it'll print all the variables again in a list
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


}
