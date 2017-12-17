using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using UnityEditorInternal;

// [InitializeOnLoad]
class DrawHierarchy : Editor
{
    static Texture2D lightTexture;
    static Texture2D cameraTexture;
    static Texture2D spriteRendererTexture;
    static Texture2D audioSourceTexture;
    static Texture2D cSharpTexture;
    static Texture2D jScriptTexture;
    static Texture2D rigidbody2DTexture;
    static Texture2D boxCollider2DTexture;
    static Texture2D circleCollider2DTexture;
    static Texture2D polygonCollider2DTexture;
    static Texture2D edgeCollider2DTexture;
    static Texture2D rigidbodyTexture;
    static Texture2D boxColliderTexture;
    static Texture2D sphereColliderTexture;
    static Texture2D capsuleColliderTexture;
    static Texture2D characterControllerTexture;
    static Texture2D meshColliderTexture;
    static Texture2D particleSystemTexture;
    static Texture2D meshRendererTexture;
    static Texture2D textMeshTexture;
    static Texture2D animatorTexture;
    static Texture2D animationTexture;

    static List<int> markedLights = new List<int>();
    static List<int> markedCameras = new List<int>();
    static List<int> markedSpriteRenderers = new List<int>();
    static List<int> markedAudioSources = new List<int>();
    static List<int> markedRigidbody2Ds = new List<int>();
    static List<int> markedBoxCollider2Ds = new List<int>();
    static List<int> markedCircleCollider2Ds = new List<int>();
    static List<int> markedPolygonCollider2Ds = new List<int>();
    static List<int> markedEdgeCollider2Ds = new List<int>();
    static List<int> markedRigidbodys = new List<int>();
    static List<int> markedBoxColliders = new List<int>();
    static List<int> markedSphereColliders = new List<int>();
    static List<int> markedCapsuleColliders = new List<int>();
    static List<int> markedCharacterControllers= new List<int>();
    static List<int> markedMeshColliders = new List<int>();
//    static List<int> markedMonoBehaviours = new List<int>();

    static List<int> markedCSs = new List<int>();
    static List<int> markedJSs = new List<int>();

    static List<int> markedParticleSystems = new List<int>();
    static List<int> markedMeshRenderers = new List<int>();
    static List<int> markedTextMeshs = new List<int>();
    static List<int> markedAnimators = new List<int>();
    static List<int> markedAnimations = new List<int>();
    static List<GameObject> allGameObjects = new List<GameObject>();
    
    static DrawHierarchy ()
    {
        // Init   //
//        Debug.Log("DrawHierarchy");
        lightTexture = AssetPreview.GetMiniTypeThumbnail(typeof(Light)) as Texture2D;//AssetDatabase.LoadAssetAtPath ("Assets/Gizmos/cubelight.tga", typeof(Texture2D)) as Texture2D;
        cameraTexture = AssetPreview.GetMiniTypeThumbnail(typeof(Camera)) as Texture2D;
        spriteRendererTexture = AssetPreview.GetMiniTypeThumbnail(typeof(SpriteRenderer)) as Texture2D;
        audioSourceTexture = AssetPreview.GetMiniTypeThumbnail(typeof(AudioSource)) as Texture2D;
        cSharpTexture = EditorGUIUtility.Load("icons/d_unityeditor.gameview.png") as Texture2D;//
        jScriptTexture = EditorGUIUtility.Load("icons/generated/js script icon.asset") as Texture2D;//
        cSharpTexture = EditorGUIUtility.Load("icons/generated/cs script icon.asset") as Texture2D;//

//        EditorGUIUtility.
//        cSharpTexture = EditorGUIUtility.ObjectContent(null,typeof(MonoBehaviour)).image as Texture2D;
//        cSharpTexture = AssetPreview.GetMiniThumbnail(typeof(UnityEngine.)) as Texture2D;  
        rigidbody2DTexture = AssetPreview.GetMiniTypeThumbnail(typeof(Rigidbody2D)) as Texture2D;
        boxCollider2DTexture = AssetPreview.GetMiniTypeThumbnail(typeof(BoxCollider2D)) as Texture2D;
        circleCollider2DTexture = AssetPreview.GetMiniTypeThumbnail(typeof(CircleCollider2D)) as Texture2D;
        polygonCollider2DTexture = AssetPreview.GetMiniTypeThumbnail(typeof(PolygonCollider2D)) as Texture2D;
        edgeCollider2DTexture = AssetPreview.GetMiniTypeThumbnail(typeof(EdgeCollider2D)) as Texture2D;
        rigidbodyTexture = AssetPreview.GetMiniTypeThumbnail(typeof(Rigidbody)) as Texture2D;
        boxColliderTexture = AssetPreview.GetMiniTypeThumbnail(typeof(BoxCollider)) as Texture2D;
        sphereColliderTexture = AssetPreview.GetMiniTypeThumbnail(typeof(SphereCollider)) as Texture2D;
        capsuleColliderTexture = AssetPreview.GetMiniTypeThumbnail(typeof(CapsuleCollider)) as Texture2D;
        characterControllerTexture = AssetPreview.GetMiniTypeThumbnail(typeof(CharacterController)) as Texture2D;
        meshColliderTexture = AssetPreview.GetMiniTypeThumbnail(typeof(MeshCollider)) as Texture2D;
        particleSystemTexture = AssetPreview.GetMiniTypeThumbnail(typeof(ParticleSystem)) as Texture2D;
        meshRendererTexture = AssetPreview.GetMiniTypeThumbnail(typeof(MeshRenderer)) as Texture2D;
        textMeshTexture = AssetPreview.GetMiniTypeThumbnail(typeof(TextMesh)) as Texture2D;
        animatorTexture = AssetPreview.GetMiniTypeThumbnail(typeof(Animator)) as Texture2D;
        animationTexture = AssetPreview.GetMiniTypeThumbnail(typeof(Animation)) as Texture2D;
//        EditorApplication.update += Update;
        EditorApplication.hierarchyWindowChanged += HierarchyWindowChanged;
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        EditorApplication.RepaintHierarchyWindow();
        EditorApplication.delayCall += PlaymodeStateChanged;
//        EditorWindow myWindowInfo = EditorWindow.GetWindow(EditorWindow.focusedWindow);
//        myWindowInfo.  
    }

    static void PlaymodeStateChanged( )
    {
        HierarchyWindowChanged();

    }

    static void HierarchyWindowChanged()
    {
//        if(EditorApplication.isPlaying)
//            return;
        if(Event.current == null)
        {
            Debug.Log("Don't Exist");
        }
        else 
        {
            Debug.Log("Do Exist");
        }



        allGameObjects.Clear();
        allGameObjects.AddRange(Resources.FindObjectsOfTypeAll (typeof(GameObject)) as GameObject[]);

        ClearLists(markedLights,markedCameras,markedSpriteRenderers,markedAudioSources,markedBoxCollider2Ds,
                markedCircleCollider2Ds,markedPolygonCollider2Ds,markedEdgeCollider2Ds,markedRigidbody2Ds,
                markedBoxColliders, markedSphereColliders, markedCapsuleColliders, markedCharacterControllers,
                markedMeshColliders, markedRigidbodys, markedCSs, markedJSs, markedParticleSystems, 
                markedMeshRenderers, markedTextMeshs, markedAnimators, markedAnimations);

        foreach (GameObject g in allGameObjects) 
        {
            Component[] components = g.GetComponents(typeof(Component));

            for (int i = 0; i < components.Length; i++) 
            {
                if(components[i] is MonoBehaviour)
                {
                    MonoScript script = MonoScript.FromMonoBehaviour( (components[i] as MonoBehaviour) );
                    string path = AssetDatabase.GetAssetPath( script );
                    string extension = path.Substring(path.Length-3,3);
                    if(extension == ".cs")
                    {
                        markedCSs.Add(g.GetInstanceID());
                        i = components.Length;
                    }
                    else if(extension == ".js")
                    {
                        markedJSs.Add(g.GetInstanceID());
                        i = components.Length;
                    }
                }
                else
                {
                }
            }
            int gameObjId = g.GetInstanceID ();
            if (g.GetComponent<Animator> () != null)
                markedAnimators.Add(gameObjId);

            if (g.GetComponent<Animation> () != null)
                markedAnimations.Add(gameObjId);

            if (g.GetComponent<TextMesh> () != null)
                markedTextMeshs.Add(gameObjId);

            if (g.GetComponent<MeshRenderer> () != null)
                markedMeshRenderers.Add(gameObjId);

            if (g.GetComponent<ParticleSystem> () != null)
                markedParticleSystems.Add(gameObjId);

            if (g.GetComponent<Light> () != null)
                markedLights.Add(gameObjId);
            
            if (g.GetComponent<Camera> () != null)
                markedCameras.Add(gameObjId);
            
            if (g.GetComponent<SpriteRenderer> () != null)
                markedSpriteRenderers.Add(gameObjId);
            
            if (g.GetComponent<AudioSource> () != null)
                markedAudioSources.Add(gameObjId);
            
            if (g.GetComponent<Rigidbody2D> () != null)
                markedRigidbody2Ds.Add(gameObjId);
            
            if (g.GetComponent<BoxCollider2D> () != null)
                markedBoxCollider2Ds.Add(gameObjId);

            if (g.GetComponent<CircleCollider2D> () != null)
                markedCircleCollider2Ds.Add(gameObjId);

            if (g.GetComponent<PolygonCollider2D> () != null)
                markedPolygonCollider2Ds.Add(gameObjId);

            if (g.GetComponent<EdgeCollider2D> () != null)
                markedEdgeCollider2Ds.Add(gameObjId);

            if (g.GetComponent<Rigidbody> () != null)
                markedRigidbodys.Add(gameObjId);
            
            if (g.GetComponent<BoxCollider> () != null)
                markedBoxColliders.Add(gameObjId);

            if (g.GetComponent<SphereCollider> () != null)
                markedSphereColliders.Add(gameObjId);

            if (g.GetComponent<CapsuleCollider> () != null)
                markedCapsuleColliders.Add(gameObjId);

            if (g.GetComponent<CharacterController> () != null)
                markedCharacterControllers.Add(gameObjId);

            if (g.GetComponent<MeshCollider> () != null)
                markedMeshColliders.Add(gameObjId);
            
        }

    }

    static void ClearLists(params List<int>[] lists)
    {
        for (int i = 0; i < lists.Length; i++) 
        {
            lists[i].Clear();
        }
    }
    
    static void HierarchyWindowItemOnGUI (int instanceID, Rect selectionRect)
    {
        if(EditorApplication.isPlaying)
            return;

        GameObject me = null;
        for (int i = 0; i < allGameObjects.Count; i++)
        {
            if(allGameObjects[i].GetInstanceID() == instanceID)
                me = allGameObjects[i];
        }
        if(me == null)
            return;
        Rect r = new Rect (selectionRect); 
        //r.x = r.width + (me.HeritageCount() * 14);
        r.width = 18;

        me.SetActive(GUI.Toggle(r,me.activeSelf,GUIContent.none));
        r.x -= 15;

        r = ShowType(r, instanceID, markedCSs, cSharpTexture);
        r = ShowType(r, instanceID, markedJSs, jScriptTexture);
        r = ShowType(r, instanceID, markedBoxCollider2Ds, boxCollider2DTexture);
        r = ShowType(r, instanceID, markedCircleCollider2Ds, boxCollider2DTexture);
        r = ShowType(r, instanceID, markedPolygonCollider2Ds, polygonCollider2DTexture);
        r = ShowType(r, instanceID, markedEdgeCollider2Ds, edgeCollider2DTexture);
        r = ShowType(r, instanceID, markedRigidbody2Ds, rigidbody2DTexture);
        r = ShowType(r, instanceID, markedMeshColliders, meshColliderTexture);
        r = ShowType(r, instanceID, markedCharacterControllers, characterControllerTexture);
        r = ShowType(r, instanceID, markedCapsuleColliders, characterControllerTexture);
        r = ShowType(r, instanceID, markedSphereColliders, sphereColliderTexture);
        r = ShowType(r, instanceID, markedBoxColliders, boxColliderTexture);
        r = ShowType(r, instanceID, markedRigidbodys, rigidbodyTexture);
        r = ShowType(r, instanceID, markedAudioSources, audioSourceTexture);
        r = ShowType(r, instanceID, markedSpriteRenderers, spriteRendererTexture);
        r = ShowType(r, instanceID, markedMeshRenderers, meshRendererTexture);
        r = ShowType(r, instanceID, markedTextMeshs, textMeshTexture);
        r = ShowType(r, instanceID, markedAnimations, animationTexture);
        r = ShowType(r, instanceID, markedAnimators, animatorTexture);
        r = ShowType(r, instanceID, markedParticleSystems, particleSystemTexture);
        r = ShowType(r, instanceID, markedLights, lightTexture);
        r = ShowType(r, instanceID, markedCameras, cameraTexture);
    }

    static Rect ShowType(Rect rect, int instanceID ,List<int> gameObjectIds, Texture texture)
    {
        
        if (gameObjectIds.Contains (instanceID)) 
        {
            GUI.Label (rect, texture); 
            rect.x -= 20;
        }
        
        return rect;
    }
    
}