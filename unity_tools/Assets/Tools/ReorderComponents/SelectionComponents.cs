// using UnityEngine;  
// using System;
// using System.Collections;
// using System.Collections.Generic;

#region SelectionComponent
// namespace JH_Tools
// {
//     //[Serializable]
//     class SelectionComponents : IList
//     {
//         public object[] _contents;
//         public int _count;

//         public SelectionComponents()
//         {
//             _count = 0;
//         }

//         // IList Members
//         public int Add(object value)
//         {
//             if (_count < _contents.Length)
//             {
//                 _contents[_count] = value;
//                 _count++;

//                 return (_count - 1);
//             }
//             else
//             {
//                 return -1;
//             }
//         }

//         public void Clear()
//         {
//             _count = 0;
//         }

//         public bool Contains(object value)
//         {
//             bool inList = false;
//             for (int i = 0; i < Count; i++)
//             {
//                 if (_contents[i] == value)
//                 {
//                     inList = true;
//                     break;
//                 }
//             }
//             return inList;
//         }

//         public int IndexOf(object value)
//         {
//             int itemIndex = -1;
//             for (int i = 0; i < Count; i++)
//             {
//                 if (_contents[i] == value)
//                 {
//                     itemIndex = i;
//                     break;
//                 }
//             }
//             return itemIndex;
//         }

//         public void Insert(int index, object value)
//         {
//             if ((_count + 1 <= _contents.Length) && (index < Count) && (index >= 0))
//             {
//                 _count++;

//                 for (int i = Count - 1; i > index; i--)
//                 {
//                     _contents[i] = _contents[i - 1];
//                 }
//                 _contents[index] = value;
//             }
//         }

//         public bool IsFixedSize
//         {
//             get
//             {
//                 return true;
//             }
//         }

//         public bool IsReadOnly
//         {
//             get
//             {
//                 return false;
//             }
//         }

//         public void Remove(object value)
//         {
//             RemoveAt(IndexOf(value));
//         }

//         public void RemoveAt(int index)
//         {
//             if ((index >= 0) && (index < Count))
//             {
//                 for (int i = index; i < Count - 1; i++)
//                 {
//                     _contents[i] = _contents[i + 1];
//                 }
//                 _count--;
//             }
//         }

//         public object this[int index]
//         {
//             get
//             {
//                 return _contents[index];
//             }
//             set
//             {
//                 _contents[index] = value;
//             }
//         }

//         // ICollection Members

//         public void CopyTo(Array array, int index)
//         {
//             int j = index;
//             for (int i = 0; i < Count; i++)
//             {
//                 array.SetValue(_contents[i], j);
//                 j++;
//             }
//         }

//         public int Count
//         {
//             get
//             {
//                 return _count;
//             }
//         }

//         public bool IsSynchronized
//         {
//             get
//             {
//                 return false;
//             }
//         }

//         // Return the current instance since the underlying store is not
//         // publicly available.
//         public object SyncRoot
//         {
//             get
//             {
//                 return this;
//             }
//         }

//         // IEnumerable Members

//         public IEnumerator GetEnumerator()
//         {
//             // Refer to the IEnumerator documentation for an example of
//             // implementing an enumerator.
//             throw new Exception("The method or operation is not implemented.");
//         }

//         public void PrintContents()
//         {
//             Console.WriteLine("List has a capacity of {0} and currently has {1} elements.", _contents.Length, _count);
//             Console.Write("List contents:");
//             for (int i = 0; i < Count; i++)
//             {
//                 Console.Write(" {0}", _contents[i]);
//             }
//             Console.WriteLine();
//         }
//     }

//     // This code produces output similar to the following:
//     // Populate the List:
//     // List has a capacity of 8 and currently has 8 elements.
//     // List contents: one two three four five six seven eight
//     //
//     // Remove elements from the list:
//     // List has a capacity of 8 and currently has 6 elements.
//     // List contents: one two three four five seven
//     //
//     // Add an element to the end of the list:
//     // List has a capacity of 8 and currently has 7 elements.
//     // List contents: one two three four five seven nine
//     //
//     // Insert an element into the middle of the list:
//     // List has a capacity of 8 and currently has 8 elements.
//     // List contents: one two three four number five seven nine
//     //
//     // Check for specific elements in the list:
//     // List contains "three": True
//     // List contains "ten": False
// }
#endregion



// using UnityEngine;
// using UnityEditor;
// using UnityEditorInternal;
// using System.Collections.Generic;
 
// public class PrefabLibraryWindowMain : EditorWindow {
 
//     const int TOP_PADDING = 2;
//     static Vector2 w_WindowMinSize = Vector2.one * 300.0f;
//     static Rect w_HelpRect = new Rect(0.0f, 0.0f, 300.0f, 100.0f);
//     static Rect w_ListRect = new Rect(Vector2.zero, w_WindowMinSize);
 
//     string Product = "Scene Doors";
//     string Version = "1.5";
 
//     public int toolbarSelection = 0;
//     public string[] toolbarStrings = new string[] {"Library", "Settings", "About"};
 
//     SerializedObject pl_Categories = null;
//     ReorderableList pl_CategoriesList = null;
 
//     public IList<string> categoriesStrings;
 
//     Color headerColor = new Color(0.65f, 0.65f, 0.65f, 1);
//     //Color backgroundColor = new Color(0.75f, 0.75f, 0.75f);
 
//     [MenuItem("Tools/Labyrith/Prefab Library")]
//     public static void Init()
//     {
//         PrefabLibraryWindowMain window = EditorWindow.GetWindow<PrefabLibraryWindowMain>(false, "Prefab Library");
//         window.minSize = w_WindowMinSize;
//     }
 
//     private void OnEnable()
//     {
//         pl_CategoriesList = new ReorderableList(pl_Categories, categoriesStrings, true, true, true, true);
//         pl_CategoriesList.drawHeaderCallback = (rect) => EditorGUILayout.LabelField("Categories");
//         pl_CategoriesList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
//         {
//             rect.y += TOP_PADDING;
//             rect.height = EditorGUIUtility.singleLineHeight;
//             EditorGUI.PropertyField(rect, pl_CategoriesList.serializedProperty.GetArrayElementAtIndex(index));
//         };
//     }
 
//     void OnInspectorUpdate()
//     {
//         Repaint();
//     }
 
//     void OnGUI()
//     {
//         EditorStyles.label.wordWrap = true;
 
//         GUI.skin.label.wordWrap = true;
 
//         if (!EditorGUIUtility.isProSkin)
//         {
//             headerColor = new Color(165 / 255f, 165 / 255f, 165 / 255f, 1);
//         }
//         else
//         {
//             headerColor = new Color(41 / 255f, 41 / 255f, 41 / 255f, 1);
//         }
 
//         toolbarSelection = GUILayout.Toolbar(toolbarSelection, toolbarStrings);
 
//         if(toolbarSelection == 1)
//         {
//             EditorGUILayout.BeginHorizontal(); //Begin Whole Window
//             EditorGUILayout.BeginVertical(GUILayout.Width(200)); //Begin Sidebar
//             EditorGUILayout.BeginHorizontal(); //Begin Right
 
//             GUILayout.Label("About Scene Doors", EditorStyles.boldLabel);
 
//             pl_Categories.Update();
//             pl_CategoriesList.DoList(w_ListRect);
//             pl_Categories.ApplyModifiedProperties();
 
//             EditorGUILayout.EndHorizontal(); //End
//         }
//     }
 
//     public bool DrawHeaderTitle(string title, bool foldoutProperty, Color backgroundColor)
//     {
 
//         GUILayout.Space(0);
 
//         GUI.Box(new Rect(1, GUILayoutUtility.GetLastRect().y + 4, position.width, 27), "");
//         EditorGUI.DrawRect(new Rect(GUILayoutUtility.GetLastRect().x, GUILayoutUtility.GetLastRect().y + 5f, position.width + 1, 25f), headerColor);
//         GUILayout.Space(4);
 
//         GUILayout.Label(title, EditorStyles.largeLabel);
//         GUI.color = Color.clear;
//         if (GUI.Button(new Rect(0, GUILayoutUtility.GetLastRect().y - 4, position.width, 27), ""))
//         {
//             foldoutProperty = !foldoutProperty;
//         }
//         GUI.color = Color.white;
//         return foldoutProperty;
//     }
// }