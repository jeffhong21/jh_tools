// Main Component Icon, Active State Toggling and "Smart Empty Object Icon Display"
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
 
// [InitializeOnLoad]
class MainComponentIcon
{
    // only show these types if showAllTypes is false
    static Type[] componentTypes = new Type[]
    {
        typeof(Light),
        typeof(BoxCollider),
        typeof(Camera),
        typeof(ParticleSystem),
        typeof(TrailRenderer),
        typeof(MeshRenderer),
        typeof(MeshFilter),
        typeof(SpriteRenderer),
        typeof(AudioSource),
        typeof(Canvas),
        typeof(CanvasGroup),
        typeof(UnityEngine.UI.Image),
        typeof(UnityEngine.UI.Button),
        typeof(UnityEngine.UI.Text),
 
        // must add the custom types here if showAllTypes is false
        // typeof(GameController),
        // typeof(PlayerController),
        // typeof(SpellScript)
    };
    static bool showActiveToggle = true;
    static bool showIndentedActiveToggle = false;
    static bool showAllTypes = true;
    static bool useFallbackIconForExcludedComponents = false;
    static bool prefabOverOthers = true; // don't show any component icon if the gameobject is a prefab, show the prefab icon instead
    static bool alwaysShowCustom = false; // will show all custom, even if they don't have custom images (will show the little script page icon)
    static bool alwaysShowPrefabContainer = true;
    static IdleObjectsView showEmptyPrefabIconInIdleObjects = IdleObjectsView.TransformIcon; // // gameobjects with no child or components
    static FallbackIcon fallbackIcon = FallbackIcon.GameObjectOrTransformsOrPrefab;
 
    // UI exceptions
    static bool canvasOverPrefab = true;
 
    static List<GameObject> allChildGOsForToggle;
    static bool toggleActiveAllChildInheritToggleObject = true; // Attention: if hold alt and clickin on object toggle, it will set all childs active state to the same of the clicked object
 
    static MainComponentIcon()
    {
        // Init
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
    }
 
    static void HierarchyItemCB(int instanceID, Rect selectionRect)
    {
        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
 
        if (obj)
        {
            Rect r = new Rect(selectionRect);
            if (showActiveToggle)
            {
                // Toggle active button
                r.x = showIndentedActiveToggle ? selectionRect.x - 28 : 2;
                r.width = 18;
                r.height = 16;
 
                EditorGUI.BeginChangeCheck();
 
                var toggleValue = GUI.Toggle(r, obj.activeSelf, "");
 
                if (toggleActiveAllChildInheritToggleObject && Event.current.alt)
                {
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (allChildGOsForToggle == null) allChildGOsForToggle = new List<GameObject>();
                        allChildGOsForToggle.Clear();
                        allChildGOsForToggle.Add(obj);
                        FetchAllChildGameObjects(obj.transform);
                        Undo.RecordObjects(allChildGOsForToggle.ToArray(), "Toggle GameObject Active");
                        allChildGOsForToggle.ForEach(child => child.SetActive(toggleValue));
                    }
                }
                else if (EditorGUI.EndChangeCheck())
                {
                    obj.SetActive(toggleValue);
                    Undo.RecordObject(obj, "Toggle " + obj.name + " Active");
                }
            }
 
            // Icon
            r.x = EditorGUIUtility.currentViewWidth - 38;
            r.width = 18;
            r.height = 18;
            var backupColor = GUI.color;
            if (!obj.activeSelf) GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 0.5f);
            var components = obj.GetComponents<Component>();
            if (components != null)
            {
                // FIRST COMPONENT IS ALWAYS TRANSFORM OR RECT TRASNFORM
                if (components.Length > 1)
                {
                    // HAS COMPONENTS
                    Type type = components[1].GetType();
                    Texture image = EditorGUIUtility.ObjectContent(null, type).image;
                    bool isInComponentTypesList = Array.Exists(componentTypes, x => x == type);
 
                    if (image == null)
                    {
                        // put your custom components icons in Assets/Editor/Icons, name it the same name of your class and change the extension below as accordingly
                        image = AssetDatabase.LoadAssetAtPath("Assets/Editor/Icons/" + type.ToString() + ".psd", typeof(Texture)) as Texture;
                    }
 
                    if (canvasOverPrefab && type == typeof(Canvas))
                    {
                        GUI.Label(r, EditorGUIUtility.IconContent("Canvas Icon"));
                    }
                    else if (prefabOverOthers && CanShowAsPrefab(obj))
                    {
                        ShowFallbackIcon(obj, r);
                    }
                    else if (!image && alwaysShowCustom)
                    {
                        GUI.Label(r, EditorGUIUtility.IconContent("cs Script Icon"));
                    }
                    else if (showAllTypes || isInComponentTypesList)
                    {
                        if (image)
                        {
                            GUI.Label(r, image);
                        }
                        else
                        {
                            ShowFallbackIcon(obj, r);
                        }
                    }
                    else if (image && useFallbackIconForExcludedComponents || alwaysShowPrefabContainer && CanShowAsPrefab(obj))
                    {
                        ShowFallbackIcon(obj, r);
                    }
                }
                else
                {
                    // IDLE OBJECTS
                    if (obj.transform.childCount == 0)
                    {
                        if (CanShowAsPrefab(obj))
                            GUI.Label(r, EditorGUIUtility.IconContent("PrefabNormal Icon"));
                        else if (showEmptyPrefabIconInIdleObjects == IdleObjectsView.WhitePrefabIcon) // no child nor components
                            GUI.Label(r, EditorGUIUtility.IconContent("Prefab Icon"));
                        else if (showEmptyPrefabIconInIdleObjects == IdleObjectsView.TransformIcon) // no child nor components
                        {
                            if (obj.GetComponent<RectTransform>())
                                GUI.Label(r, EditorGUIUtility.IconContent("RectTransform Icon"));
                            else
                                GUI.Label(r, EditorGUIUtility.IconContent("Transform Icon"));
                        }
                    }
                    // NO COMPONENTS BUT HAS CHILDREN
                    else if (obj.transform.childCount > 0)
                    {
                        if (CanShowAsPrefab(obj))
                            GUI.Label(r, EditorGUIUtility.IconContent("PrefabNormal Icon"));
                        else
                            GUI.Label(r, EditorGUIUtility.IconContent("Prefab Icon"));
 
                        GUI.Label(r, EditorGUIUtility.IconContent("Transform Icon"));
                    }
 
                }
            }
 
            GUI.color = backupColor;
        }
    }
 
    static void ShowFallbackIcon(GameObject obj, Rect r)
    {
        switch (fallbackIcon)
        {
            case FallbackIcon.TransformOrRect:
 
                if (obj.GetComponent<RectTransform>())
                    GUI.Label(r, EditorGUIUtility.IconContent("RectTransform Icon"));
                else
                    GUI.Label(r, EditorGUIUtility.IconContent("Transform Icon"));
                break;
 
            case FallbackIcon.GameObjectOrPrefab:
                if (CanShowAsPrefab(obj))
                    GUI.Label(r, EditorGUIUtility.IconContent("PrefabNormal Icon"));
                else
                    GUI.Label(r, EditorGUIUtility.IconContent("GameObject Icon"));
                break;
 
            case FallbackIcon.GameObjectOrTransformsOrPrefab:
                if (CanShowAsPrefab(obj))
                    GUI.Label(r, EditorGUIUtility.IconContent("PrefabNormal Icon"));
                else
                {
                    if (obj.GetComponents<Component>().Length > 1)
                    {
                        GUI.Label(r, EditorGUIUtility.IconContent("GameObject Icon"));
                    }
                    else
                    {
                        GUI.Label(r, EditorGUIUtility.IconContent("Transform Icon"));
                    }
                }
                break;
        }
    }
 
    static bool IsPartOfPrefab(GameObject prefab)
    {
        return PrefabUtility.GetPrefabParent(prefab) != null;
    }
 
    static bool IsPrefabParent(GameObject prefab)
    {
        return PrefabUtility.FindPrefabRoot(prefab) == prefab;
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
 
    public static void FetchAllChildGameObjects(Transform transform)
    {
        if (allChildGOsForToggle == null) allChildGOsForToggle = new List<GameObject>();
        foreach (Transform t in transform)
        {
            if (!allChildGOsForToggle.Contains(t.gameObject)) allChildGOsForToggle.Add(t.gameObject);
            FetchAllChildGameObjects(t);
        }
    }
 
    //static bool
}
 
enum FallbackIcon
{
    None,
    GameObjectOrTransformsOrPrefab,
    GameObjectOrPrefab,
    TransformOrRect
}
 
enum IdleObjectsView
{
    None,
    TransformIcon,
    WhitePrefabIcon
}