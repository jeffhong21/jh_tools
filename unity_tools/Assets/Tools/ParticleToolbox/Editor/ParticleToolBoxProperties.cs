using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;




[CustomPropertyDrawer (typeof(PropertyFields.TwoFloatField))]
public class TwoFloatFieldDrawer : PropertyDrawer {

	public enum Options{ 
		Constant, 
		Random 
	}
	public Options optionList;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

		SerializedProperty s_Values1 = property.FindPropertyRelative ("value1");
		SerializedProperty s_Values2 = property.FindPropertyRelative ("value2");

		int indent = EditorGUI.indentLevel;
		label = EditorGUI.BeginProperty(position, label, property);
		EditorGUI.indentLevel = 0;
		Rect contentPosition = EditorGUI.PrefixLabel (position, label);

		EditorGUI.BeginChangeCheck ();


		contentPosition.width *= 0.4f;
		float newValue1 = EditorGUI.FloatField (contentPosition, GUIContent.none, s_Values1.floatValue);
		contentPosition.x += contentPosition.width + 3f;
		float newValue2 = EditorGUI.FloatField (contentPosition, GUIContent.none, s_Values2.floatValue);

		contentPosition.x += contentPosition.width;
		contentPosition.y -= 1f;
		contentPosition.width /= 3f;
		contentPosition.height -= 2f;
		contentPosition.x += contentPosition.width/2;

		EditorGUI.EnumPopup(contentPosition, GUIContent.none, optionList, new GUIStyle("StaticDropdown") ); //new GUIStyle("StaticDropdown") 


		switch (optionList) {
		case Options.Constant:
			break;
		case Options.Random:
			break;
		}

		if (EditorGUI.EndChangeCheck ()) {
			s_Values1.floatValue = newValue1;
			s_Values2.floatValue = newValue2;

		}


		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}
}



[CustomPropertyDrawer (typeof(PropertyFields.ColorField))]
public class ColorFieldDrawer : PropertyDrawer {

	public enum Options{ 
		Constant, 
		Random 
	}
	public Options optionList;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

		SerializedProperty s_Color = property.FindPropertyRelative ("color");
		bool showEyeDropper = false, showAlpha = true, hdr = true;
		ColorPickerHDRConfig colorPickerHDRConfig = null;

		int indent = EditorGUI.indentLevel;
		label = EditorGUI.BeginProperty(position, label, property);
		EditorGUI.indentLevel = 0;
		Rect contentPosition = EditorGUI.PrefixLabel (position, label);

		EditorGUI.BeginChangeCheck ();

		// adding 3 because of the gap between two fields is 3
		contentPosition.width = contentPosition.width * 0.8f + 3f;
		Color newColor = EditorGUI.ColorField (contentPosition, GUIContent.none, s_Color.colorValue, showEyeDropper, showAlpha, hdr, colorPickerHDRConfig);


		contentPosition.x += contentPosition.width;
		contentPosition.y -= 1f;
		//  dividing by 6 because dividing by 3 with two fields and cause i'm doubling the space allocated for one field, i double 3 to 6
		contentPosition.width /= 6f;
		contentPosition.height -= 2f;
		contentPosition.x += contentPosition.width/2;

		EditorGUI.EnumPopup(contentPosition, GUIContent.none, optionList, new GUIStyle("StaticDropdown") ); //new GUIStyle("StaticDropdown") 

		if (EditorGUI.EndChangeCheck ()) {
			s_Color.colorValue = newColor;
		}

		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();

	}
}

[CustomPropertyDrawer (typeof(PropertyFields.TwoColorField))]
public class TwoColorFieldDrawer : PropertyDrawer {

	public enum Options{ 
		Constant, 
		Random 
	}
	public Options optionList;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

		SerializedProperty s_Color1 = property.FindPropertyRelative ("color1");
		SerializedProperty s_Color2 = property.FindPropertyRelative ("color2");
		bool showEyeDropper = false, showAlpha = true, hdr = true;
		ColorPickerHDRConfig colorPickerHDRConfig = null;



		int indent = EditorGUI.indentLevel;
		label = EditorGUI.BeginProperty(position, label, property);
		EditorGUI.indentLevel = 0;
		Rect contentPosition = EditorGUI.PrefixLabel (position, label);

		EditorGUI.BeginChangeCheck ();

		contentPosition.width *= 0.4f;
		Color newColor1 = EditorGUI.ColorField (contentPosition, GUIContent.none, s_Color1.colorValue, showEyeDropper, showAlpha, hdr, colorPickerHDRConfig);
		contentPosition.x += contentPosition.width + 3f;
		Color newColor2 = EditorGUI.ColorField (contentPosition, GUIContent.none, s_Color2.colorValue, showEyeDropper, showAlpha, hdr, colorPickerHDRConfig);

		contentPosition.x += contentPosition.width;
		contentPosition.y -= 1f;
		contentPosition.width /= 3f;
		contentPosition.height -= 2f;
		contentPosition.x += contentPosition.width/2;

		EditorGUI.EnumPopup(contentPosition, GUIContent.none, optionList, new GUIStyle("StaticDropdown") ); //new GUIStyle("StaticDropdown") 

		if (EditorGUI.EndChangeCheck ()) {
			s_Color1.colorValue = newColor1;
			s_Color2.colorValue = newColor2;
		}

		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();

	}
}





/*
this.label = FindStyle("ShurikenLabel");
this.numberField = FindStyle("ShurikenValue");
this.objectField = FindStyle("ShurikenObjectField");
this.effectBgStyle = FindStyle("ShurikenEffectBg");
this.emitterHeaderStyle = FindStyle("ShurikenEmitterTitle");
this.moduleHeaderStyle = FindStyle("ShurikenModuleTitle");
this.moduleBgStyle = FindStyle("ShurikenModuleBg");
this.plus = FindStyle("ShurikenPlus");
this.minus = FindStyle("ShurikenMinus");
this.line = FindStyle("ShurikenLine");
this.checkmark = FindStyle("ShurikenCheckMark");
this.minMaxCurveStateDropDown = FindStyle("ShurikenDropdown");
this.toggle = FindStyle("ShurikenToggle");
this.popup = FindStyle("ShurikenPopUp");
this.selectionMarker = FindStyle("IN ThumbnailShadow");
this.toolbarButtonLeftAlignText = new GUIStyle(FindStyle("ToolbarButton"));
this.modulePadding = new GUIStyle();
this.emitterHeaderStyle.clipping = TextClipping.Clip;
this.emitterHeaderStyle.padding.right = 0x2d;
this.toolbarButtonLeftAlignText.alignment = TextAnchor.MiddleLeft;
this.modulePadding.padding = new RectOffset(3, 3, 4, 2);
*/