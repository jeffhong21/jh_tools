using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


[CustomEditor(typeof(ParticleToolbox)), CanEditMultipleObjects]
public class ParticleToolBoxEditor : Editor 
{

	public ParticleToolbox m_ParticleToolbox;
	public bool showParticleSettings = true, showEmitterSettings = true, showPhysicsSettings = true;


	delegate void SettingsLayout();




	public void OnEnable(){
		// all editor has variable to get reference to instance of script
		m_ParticleToolbox = (ParticleToolbox)target;
	}


	public override void OnInspectorGUI (){


		serializedObject.Update();
		GUILayout.Space (5);

		InfoBox();
		Buttons ();

		DrawLayouts (ParticleSettings, "Particle Settings");
		DrawLayouts (TurbulenceSettings, "Turbulence Settings");


		serializedObject.ApplyModifiedProperties();

		//GUILayout.Space (0);
		//base.OnInspectorGUI ();
	}
		

	void DrawLayouts(SettingsLayout settingsLayout, string headerName){
		showParticleSettings = GUILayout.Toggle (showParticleSettings, new GUIContent(headerName), new GUIStyle ("ShurikenModuleTitle"));
		//showParticleSettings = EditorGUILayout.Foldout (showParticleSettings, new GUIContent(headerName), new GUIStyle ("ShurikenModuleTitle"));
		if (showParticleSettings) {
			GUILayout.BeginVertical (new GUIStyle("HelpBox") ); // ShurikenModuleBg
			GUILayout.Space (8);
			settingsLayout();
			GUILayout.Space (8);
			GUILayout.EndVertical ();
		}
	}


	void DrawHeaders(){
	}

	void Buttons(){

		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button (new GUIContent (" Simulate "))) {
			Debug.Log ("Simulating Particles");
		}
		if (GUILayout.Button(new GUIContent(" Stop "))) {
			Debug.Log ("Stop Simulating Particles");
		}
		EditorGUILayout.EndHorizontal();
	}

	void InfoBox(){

		Rect rect = GUILayoutUtility.GetRect (GUIContent.none, GUIStyle.none);

		//float[] particleLifetime = { m_ParticleToolbox.pStartLifetime.value1, m_ParticleToolbox.pStartLifetime.value2 };
		//float[] particleSize = { m_ParticleToolbox.pStartSize.value1, m_ParticleToolbox.pStartSize.value2 };
		//Color[] particleColor = { m_ParticleToolbox.particleColor.color1, m_ParticleToolbox.particleColor.color2 };
		string info = " Inspector Wdith:  " + rect;
			

		EditorGUILayout.HelpBox (info, MessageType.Info);

	}


	void ParticleSettings() {
		SerializedProperty particlesPerSecond = serializedObject.FindProperty ("particlesPerSecond");

		//SerializedProperty birthOffset = serializedObject.FindProperty ("birthOffset");
		//SerializedProperty looping = serializedObject.FindProperty ("looping");
		//SerializedProperty prewarm = serializedObject.FindProperty ("prewarm");
		SerializedProperty pDuration = serializedObject.FindProperty ("pDuration");
		//SerializedProperty pStartLifetime = serializedObject.FindProperty ("pStartLifetime");
		SerializedProperty pStartSize = serializedObject.FindProperty ("pStartSize");
		SerializedProperty pStartRotation = serializedObject.FindProperty ("pStartRotation");
		SerializedProperty pStartColor = serializedObject.FindProperty ("pStartColor");
		SerializedProperty pColorOverLifetime = serializedObject.FindProperty ("pColorOverLifetime");
		SerializedProperty pStartSpeed = serializedObject.FindProperty ("pStartSpeed");
		SerializedProperty pGravityModifier = serializedObject.FindProperty ("gravityModifier");
		SerializedProperty maxParticles = serializedObject.FindProperty ("maxParticles");


		SerializedProperty pStartLifetime1 = serializedObject.FindProperty ("pStartLifetime1");
		//SerializedProperty pStartLifetime2 = serializedObject.FindProperty ("pStartLifetime2");



		EditorProperties.Show(particlesPerSecond, "Particles Per Sec" );
		EditorProperties.Show(pDuration, "Duration" );

		//EditorProperties.Show(pStartLifetime,  "Lifetime" );
		EditorProperties.Show(pStartLifetime1,  "Lifetime" );

		EditorProperties.Show(pStartSize, "Size" );
		EditorProperties.Show(pStartRotation,  "Rotation" );
		//EditorProperties.Show(birthOffset,  "Birth Offset" );
		EditorProperties.Show(pStartColor,  "Color" );
		EditorProperties.Show(pColorOverLifetime,  "Color Over Lifetime" );
		EditorProperties.Show(pStartSpeed,  "Speed" );
		EditorProperties.Show(pGravityModifier, "Gravity Modifier" );
		EditorProperties.Show(maxParticles, "Max Particles" );
	}



	public void TurbulenceSettings() {

		//GUILayoutOption[] layoutOptions = { GUILayout.Width (50) }; 

		EditorGUILayout.PropertyField(serializedObject.FindProperty("velocityLifetimeX"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("velocityLifetimeY"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("velocityLifetimeZ"));

	}





	public static void AnimationCurveAttr(AnimationCurve curve, string curveLabel, bool disable) {

		Color curveColor = Color.green;
		Rect curveRect = new Rect (0, 0, 1, 1);
		curveRect.yMax = 10;
		curveRect.yMin = -10;

		curve.preWrapMode = WrapMode.ClampForever;
		curve.postWrapMode = WrapMode.ClampForever;

		EditorGUI.indentLevel += 1;
		//EditorGUI.BeginDisabledGroup (disable == disable);
		EditorGUILayout.CurveField (curveLabel, curve, curveColor, curveRect);
		//EditorGUI.EndDisabledGroup ();
		EditorGUI.indentLevel -= 1;
	}



	public static class EditorProperties {


		public static GUILayoutOption[] GUILayoutOption = new UnityEngine.GUILayoutOption[]{ GUILayout.Width (50) }; 


		public static void Show (SerializedProperty property, string label){

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.PrefixLabel(new GUIContent(label) );
			//EditorGUILayout.Slider (property, 0f, 25f, GUIContent.none );
			EditorGUILayout.PropertyField (property, GUIContent.none );
			EditorGUILayout.EndHorizontal ();

		}


		public static void Show (SerializedProperty property1, SerializedProperty property2, string label){

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.PrefixLabel(new GUIContent(label) );
			EditorGUILayout.PropertyField (property1, GUIContent.none );
			EditorGUILayout.PropertyField (property2, GUIContent.none );
			EditorGUILayout.EndHorizontal ();
		}

	}




}
	





