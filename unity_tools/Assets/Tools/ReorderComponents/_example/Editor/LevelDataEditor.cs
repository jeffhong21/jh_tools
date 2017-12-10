using UnityEngine;  
using UnityEditor;  
using UnityEditorInternal;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor 
{  
    private ReorderableList list;

    private void OnEnable() 
    {
        list = new ReorderableList(serializedObject, 
                serializedObject.FindProperty("Waves"), 
                true, true, true, true);

        list.drawElementCallback =  
            (Rect rect, int index, bool isActive, bool isFocused) => {
            // Here we are getting the list item being drawn:
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
                //  We are using FindPropertyRelative method to find properties of the wave:
                element.FindPropertyRelative("Type"), GUIContent.none);
            //  And after that we are drawing 3 properties in one line: Type, Prefab and Count:
            EditorGUI.PropertyField(
                new Rect(rect.x + 60, rect.y, rect.width - 60 - 30, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("Prefab"), GUIContent.none);
            EditorGUI.PropertyField(
                new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("Count"), GUIContent.none);
        };


        //   List header says "Serialized Property". Let's change it to something more informative. 
        //   To do this we need to use drawHeaderCallback.
        list.drawHeaderCallback = (Rect rect) => {  
            EditorGUI.LabelField(rect, "Monster Waves");
        };


        //  We'll make that when you select an element the corresponding mob prefab is highlighted in Project panel.
        list.onSelectCallback = (ReorderableList l) => {  
            var prefab = l.serializedProperty.GetArrayElementAtIndex(l.index).FindPropertyRelative("Prefab").objectReferenceValue as GameObject;
            if (prefab)
                EditorGUIUtility.PingObject(prefab.gameObject);
        };

        //  we want to have at least one wave in our list at all times
        list.onCanRemoveCallback = (ReorderableList l) => {  
            return l.count > 1;
        };





    }



    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();

    }

    private void clickHandler(object target) {  
        var data = (WaveCreationParams)target;
        var index = list.serializedProperty.arraySize;
        list.serializedProperty.arraySize++;
        list.index = index;
        var element = list.serializedProperty.GetArrayElementAtIndex(index);
        element.FindPropertyRelative("Type").enumValueIndex = (int)data.Type;
        element.FindPropertyRelative("Count").intValue = 
            data.Type == MobWave.WaveType.Boss ? 1 : 20;
        element.FindPropertyRelative("Prefab").objectReferenceValue = 
            AssetDatabase.LoadAssetAtPath(data.Path, typeof(GameObject)) as GameObject;
        serializedObject.ApplyModifiedProperties();
    }



    private struct WaveCreationParams {  
        public MobWave.WaveType Type;
        public string Path;
    }

}