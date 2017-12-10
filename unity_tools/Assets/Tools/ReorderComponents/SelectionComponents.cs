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


//  using System;
//  using System.Collections.Generic;
//  using System.Linq;
//  using UnityEngine;
//  using UnityEditor;
//  using UnityEngine.Networking;
 
//  public class ComponentsSorter : ScriptableObject
//  {
//      private class ComponentComparer : IComparer<Component>
//      {
//         private Type[] TypesOrder;

//         public ComponentComparer(List<Component> Components)
//         {
//             SetComponentOrder(Components);
//         }


//         private void SetComponentOrder(List<Component> Components)
//         {
//             TypesOrder = new Type[Components.Count];

//             for(int i = 0; i < Components.Count; i ++)
//             {
//                 TypesOrder[i] = Components.GetType();
//             }

//         }

 
//          private Int32 GetIndex(Component Component)
//          {
//              var Type = Component.GetType();
 
//              Type BestMatch = typeof(UnityEngine.Object);
//              var BestIndex = Int32.MaxValue;
//              for (int Index = 0; Index < TypesOrder.Length; Index++)
//              {
//                  // If we found the exact type in the list, then this is the right index.
//                  var TypeOrder = TypesOrder[Index];
//                  if (Type == TypeOrder)
//                      return Index;
 
//                  // If we found a parent, then we switch to its place if it is more
//                  // "recent" (in the inheritance tree) than previously found parents.
//                  if (Type.IsSubclassOf(TypeOrder))
//                  {
//                      if (TypeOrder.IsSubclassOf(BestMatch))
//                      {
//                          BestMatch = TypeOrder;
//                          BestIndex = Index;
//                      }
//                  }
//              }
 
//              return BestIndex;
//          }
 
//          public int Compare(Component First, Component Second)
//          {
//              return Comparer<Int32>.Default.Compare(GetIndex(First), GetIndex(Second));
//          }
//      }
 


//      [MenuItem("Edit/Sort Components %&a")]
//      private static void SortComponents()
//      {

//         // -- Used before onReorderCallback to get the order of the selected objects current list of components order.
//         var GameObject = Selection.activeGameObject;
//         //  List of components before onReorderCallback.
//         var CurrentComponents = GameObject.GetComponents<Component>().Where(Component => Component.GetType() != typeof (Transform)).ToList();
    
//         //  List of components after onReorderCallback.
//         var SortedComponents = GameObject.GetComponents<Component>().Where(Component => Component.GetType() != typeof(Transform)).ToList();
//         SortedComponents.Sort(new ComponentComparer(CurrentComponents));
 

//         //  Used after onReorderCallback to arrange the components
//         for (var Index = 0; Index < SortedComponents.Count; Index++)
//         {
//             var SortedComponent = SortedComponents[Index];
//             //var Components = GameObject.GetComponents<Component>().Where(Component => Component.GetType() != typeof (Transform)).ToList();
//             var CurrentIndex = CurrentComponents.IndexOf(SortedComponent);
            
//             if (CurrentIndex < Index)
//             {
//                 for (var MoveIndex = CurrentIndex; MoveIndex < Index; MoveIndex++)
//                     UnityEditorInternal.ComponentUtility.MoveComponentDown(SortedComponent);
//             }
//             else
//             {
//                 for (var MoveIndex = CurrentIndex; MoveIndex > Index; MoveIndex--)
//                     UnityEditorInternal.ComponentUtility.MoveComponentUp(SortedComponent);
//             }
//         }
//      }
//  }