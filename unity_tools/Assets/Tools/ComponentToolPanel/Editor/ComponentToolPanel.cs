using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ComponentToolPanel
{
    [CustomEditor(typeof(GameObject))]
	public class ComponentToolPanel : Editor
	{
        //  "UnityEditor.GameObjectInspector, UnityEditor" is a custom internal script that draws the GameObjects Inspector.
        const string m_GameObjectInspectorTypeName = "UnityEditor.GameObjectInspector, UnityEditor";


        Editor m_DefaultEditor;
        ReorderableList m_List;
        GameObject m_GameObject;
        Transform m_Transform;
        List<Component> m_Components;
        bool m_Foldout;




        void OnEnable(){
            m_DefaultEditor = Editor.CreateEditor(targets, Type.GetType(m_GameObjectInspectorTypeName));
            m_Components = new List<Component>();
            m_Foldout = true;
            InitializeList();
        }


		void OnDisable(){
			//This avoids leaks
            Debug.Log("Disabling Editor");
			DestroyImmediate(m_DefaultEditor); 
		} 


        //  We're overriding Unity's internal script GameObjectInspector.cs's OnHeaderGUI method.
        protected override void OnHeaderGUI(){
            //  Draw the default Unity inspectorGUI.  Other wise it will be empty.
            m_DefaultEditor.DrawHeader();
			GUILayout.Space(2);
			GUIStyle style = new GUIStyle(EditorStyles.foldout);
            
            //  Custom GUI code here

            m_Foldout = GUILayout.Toggle (m_Foldout, new GUIContent("Component Tool Panel")  , style); //ShurikenModuleTitle

            if(m_Foldout){
                m_List.DoLayoutList();
            }



            
        }


        void InitializeList()
        {
            //  Set inspector
            m_GameObject = (GameObject)target;
            m_Components = m_GameObject.GetComponents(typeof(Component)).ToList();
            //  Remove Transform component
            m_Components.RemoveAt(0);
            
            m_List = new ReorderableList(m_Components, typeof(Component), true, true, false, false);

            m_List.drawHeaderCallback = (Rect rect) => {
                DrawElements(rect, m_GameObject.transform);
            };

            m_List.drawElementCallback =  (Rect rect, int index, bool isActive, bool isFocused) => {
                DrawElements(rect, m_Components[index]);
            };

            //  What happens when the lists changes.
            m_List.onReorderCallback = (ReorderableList internalList) => {
	   			//Move Down
				for (int i = 0; i < m_Components.Count; i++){
					int _index = internalList.list.IndexOf(m_Components[i]);
					int difference = _index-i;
					if (difference>0)
						for (int j = 0; j<Mathf.Abs(difference); j++)
							ComponentUtility.MoveComponentDown(m_Components[i]);
				}
				//Move Up
				m_Components = new List<Component>(m_GameObject.GetComponents<Component>());
				m_Components.RemoveAt(0);
				for (int i = m_Components.Count-1; i >=0;i--){
					int _index = internalList.list.IndexOf(m_Components[i]);
					int difference = _index-i;
					if (difference<0)
						for (int j = 0; j<Mathf.Abs(difference); j++)
							ComponentUtility.MoveComponentUp(m_Components[i]);
				}
			};


        }


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

            //  Enable toggle.
            rect.x += 25;
            rect.y += 2;
            bool oldValue = true;
            bool newValue = oldValue;
            EditorGUI.Toggle(rect, newValue);


            //  Component name.
            rect.x += 20;
            //rect.y = originalRect.y;
            rect.width = originalRect.width - 125;
            EditorGUI.LabelField(rect,component.GetType().ToString());


            if ( ! isTransform || isRectTransform){
                //  Delete Button.
                rect.x = originalRect.width;
                rect.y += 2;
                rect.width = rect.height = 16;
                Texture buttonIcon = EditorGUIUtility.IconContent("TL Close button act").image;
                if (GUI.Button(rect, new GUIContent(buttonIcon), GUIStyle.none) ){
                    Debug.Log(string.Format("Deleting Component:  {0}", component.GetType().ToString() ) );
                }
            }


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
