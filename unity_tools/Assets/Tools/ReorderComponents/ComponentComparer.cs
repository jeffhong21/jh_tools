 using System;
 using System.Collections.Generic;
 using System.Linq;
 using UnityEngine;
 using UnityEditor;
 using UnityEngine.Networking;
 

namespace JH_Tools
{
    public class ComponentComparer : ScriptableObject, IComparer<Component>
    {
        private Type[] TypesOrder;

        public ComponentComparer(List<Component> Components)
        {
            SetComponentOrder(Components);
        }


        private void SetComponentOrder(List<Component> Components)
        {
            TypesOrder = new Type[Components.Count];

            for(int i = 0; i < Components.Count; i ++)
            {
                TypesOrder[i] = Components.GetType();
            }

        }


        private Int32 GetIndex(Component Component)
        {
            var Type = Component.GetType();

            Type BestMatch = typeof(UnityEngine.Object);
            var BestIndex = Int32.MaxValue;
            for (int Index = 0; Index < TypesOrder.Length; Index++)
            {
                // If we found the exact type in the list, then this is the right index.
                var TypeOrder = TypesOrder[Index];
                if (Type == TypeOrder)
                    return Index;

                // If we found a parent, then we switch to its place if it is more
                // "recent" (in the inheritance tree) than previously found parents.
                if (Type.IsSubclassOf(TypeOrder))
                {
                    if (TypeOrder.IsSubclassOf(BestMatch))
                    {
                        BestMatch = TypeOrder;
                        BestIndex = Index;
                    }
                }
            }

            return BestIndex;
        }

        public int Compare(Component First, Component Second)
        {
            return Comparer<Int32>.Default.Compare(GetIndex(First), GetIndex(Second));
        }

    // [MenuItem("Edit/Sort Components %&a")]
    // private static void SortComponents()
    // {
    //     var GameObject = Selection.activeGameObject;
    //     var SortedComponents = GameObject.GetComponents<Component>()
    //         .Where(Component => Component.GetType() != typeof(Transform)).ToList();
    //     //SortedComponents.Sort(new ComponentComparer());

    //     for (var Index = 0; Index < SortedComponents.Count; Index++)
    //     {
    //         var SortedComponent = SortedComponents[Index];
    //         var Components = GameObject.GetComponents<Component>().Where(Component => Component.GetType() != typeof (Transform)).ToList();
    //         var CurrentIndex = Components.IndexOf(SortedComponent);
    //         if (CurrentIndex < Index)
    //         {
    //             for (var MoveIndex = CurrentIndex; MoveIndex < Index; MoveIndex++)
    //                 UnityEditorInternal.ComponentUtility.MoveComponentDown(SortedComponent);
    //         }
    //         else
    //         {
    //             for (var MoveIndex = CurrentIndex; MoveIndex > Index; MoveIndex--)
    //                 UnityEditorInternal.ComponentUtility.MoveComponentUp(SortedComponent);
    //         }
    //     }
    // }


    }



}