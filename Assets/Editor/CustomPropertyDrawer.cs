using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Main;

//[CustomPropertyDrawer(typeof(CustomPropertyAttribute))]
//public class CustomPropertyAttributeDrawer : PropertyDrawer {
//	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//	{
//		CustomPropertyAttribute range = this.attribute as CustomPropertyAttribute;

//		if (property.propertyType == SerializedPropertyType.Vector2)
//		{
//			Debug.Log("float类型");
//		}
//	}
//}

[CustomPropertyDrawer(typeof(DisabledInInspectorAttribute))]
public class DisabledInInspectorDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		bool enabled = GUI.enabled;
		GUI.enabled = false;
		EditorGUI.PropertyField(position, property, label, true);
		GUI.enabled = enabled;
	}
}

[CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
public class EnumFlagsDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
	}
}

[CustomPropertyDrawer(typeof(CustomCurveAttribute))]
public class CustomCurveDrawer : PropertyDrawer {

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		int lines = EditorPrefs.GetBool("Curve Data", true) ? property.animationCurveValue.length * 3 + 4 : 2;
		return lines * EditorGUIUtility.singleLineHeight;
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		int lines = EditorPrefs.GetBool("Curve Data", true) ? 2 : 1;
		Rect curveRect = new Rect(position);
		curveRect.height = EditorGUIUtility.singleLineHeight * lines;
		property.animationCurveValue = EditorGUI.CurveField(curveRect, label, property.animationCurveValue);
		Rect headerRect = new Rect(curveRect);
		headerRect.y += EditorGUIUtility.singleLineHeight * lines;
		headerRect.height = EditorGUIUtility.singleLineHeight;
		if (DrawHeader(headerRect, "Curve Data"))
		{
			GUI.changed = false;

			Rect dataRect = new Rect(headerRect);
			dataRect.y += EditorGUIUtility.singleLineHeight;
			dataRect.height = position.height - EditorGUIUtility.singleLineHeight * 4;
			AnimationCurve curve = DrawCurveData(dataRect, property.animationCurveValue);

			if (GUI.changed)
			{
				property.animationCurveValue = curve;
			}
		}

	}

	private static bool DrawHeader(Rect position, string text, string key = null)
	{
		key = key ?? text;

		bool state = EditorPrefs.GetBool(key, true);

		if (!state)
			GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);

		if (state)
			text = "\u25BC " + text;
		else
			text = "\u25BA " + text;
		if (GUI.Button(position, text, "dragtab"))
		{
			state = !state;
			EditorPrefs.SetBool(key, state);
		}

		GUI.backgroundColor = Color.white;

		return state;
	}

	private static AnimationCurve DrawCurveData(Rect position, AnimationCurve curve)
	{
		Keyframe[] keyFrames = curve.keys;

		Rect keyClampRect = new Rect(position);
		keyClampRect.height = EditorGUIUtility.singleLineHeight;
		bool keyClamp = EditorPrefs.GetBool("Clamp Key", true);
		keyClamp = EditorGUI.Toggle(keyClampRect, "      Clamp Key", keyClamp);
		EditorPrefs.SetBool("Clamp Key", keyClamp);

		float labelWidth = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = 80F;
		for (int i = 0, length = keyFrames.Length; i < length; i++)
		{
			Rect elementRect = new Rect(position);
			elementRect.y += (i * 3 + 1) * EditorGUIUtility.singleLineHeight;
			elementRect.height = EditorGUIUtility.singleLineHeight;
			EditorGUI.LabelField(elementRect, "      Element " + i);

			Rect timeRect = new Rect(position);
			timeRect.y += (i * 3 + 2) * EditorGUIUtility.singleLineHeight;
			timeRect.height = EditorGUIUtility.singleLineHeight;
			timeRect.width *= 0.5F;
			Rect valueRect = new Rect(position);
			valueRect.y += (i * 3 + 2) * EditorGUIUtility.singleLineHeight;
			valueRect.height = EditorGUIUtility.singleLineHeight;
			valueRect.x += valueRect.width * 0.5F;
			valueRect.width *= 0.5F;
			float time = EditorGUI.FloatField(timeRect, "          time", keyFrames[i].time);
			float value = EditorGUI.FloatField(valueRect, "          value", keyFrames[i].value);

			Rect leftRect = new Rect(position);
			leftRect.y += (i * 3 + 3) * EditorGUIUtility.singleLineHeight;
			leftRect.height = EditorGUIUtility.singleLineHeight;
			leftRect.width *= 0.5F;
			Rect rightRect = new Rect(position);
			rightRect.y += (i * 3 + 3) * EditorGUIUtility.singleLineHeight;
			rightRect.height = EditorGUIUtility.singleLineHeight;
			rightRect.x += rightRect.width * 0.5F;
			rightRect.width *= 0.5F;
			float inTangent = EditorGUI.FloatField(leftRect, "          left", keyFrames[i].inTangent);
			float outTangent = EditorGUI.FloatField(rightRect, "          right", keyFrames[i].outTangent);

			if (keyClamp)
			{
				float prevTime = i > 0 ? keyFrames[i - 1].time : 0;
				float nextTime = i < length - 1 ? keyFrames[i + 1].time : float.MaxValue;
				time = Mathf.Clamp(time, prevTime, nextTime);
			}
			keyFrames[i].time = time;
			keyFrames[i].value = value;
			keyFrames[i].inTangent = inTangent;
			keyFrames[i].outTangent = outTangent;
		}
		EditorGUIUtility.labelWidth = labelWidth;

		return new AnimationCurve(keyFrames);
	}
}

[CustomPropertyDrawer(typeof(HexColorAttribute))]
public class HexColorDrawer : PropertyDrawer
{
	private const float mLabelWidth = 60;
	private string mEncodedColor;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		Rect colorFieldPos = new Rect(position.x, position.y, position.width - mLabelWidth, position.height);
		EditorGUI.PropertyField(colorFieldPos, property, label, true);

		GUI.SetNextControlName("Encoded Color");
		Rect stringFieldPos = new Rect(position.x + position.width - mLabelWidth, position.y, mLabelWidth, position.height);
		mEncodedColor = EditorGUI.TextField(stringFieldPos, "", mEncodedColor);
		if (GUI.GetNameOfFocusedControl() == "Encoded Color")
		{
			if (Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.Used)
			{
				property.colorValue = ConvertExt.ParseColor24(mEncodedColor, 0);
			}
		}
		else
		{
			mEncodedColor = ConvertExt.EncodeColor24(property.colorValue);
		}
	}
}

[CustomPropertyDrawer(typeof(CustomNameAttribute))]
public class CustomNameDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		Type type = fieldInfo.FieldType;
		bool isCollection = typeof(ICollection).IsAssignableFrom(type);
		if (isCollection)
		{
			GUIContent displayLabel = (attribute as CustomNameAttribute).mLabel;
			string displayLabelStr = displayLabel.text;
			string[] strs = label.text.Split(' ');
			int strLength = strs.Length;
			if (strLength > 1)
			{
				displayLabelStr += " " + strs[strs.Length - 1];
				EditorGUI.PropertyField(position, property, new GUIContent(displayLabelStr), true);
			}
			else
			{
				EditorGUI.PropertyField(position, property, displayLabel, true);
			}
		}
		else
		{
			EditorGUI.PropertyField(position, property, (attribute as CustomNameAttribute).mLabel, true);
		}
	}

}

[CustomPropertyDrawer(typeof(EnumCustomNameAttribute))]
public class EnumCustomNameDrawer : CustomNameDrawer {

	private Dictionary<string, string> mCustomEnumNameDict = new Dictionary<string, string>();

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (property.propertyType == SerializedPropertyType.Enum)
		{
			Type type = fieldInfo.FieldType;
			bool isCollection = typeof(ICollection).IsAssignableFrom(type);
			if (isCollection)
			{
				Type[] argumentTypes = type.GetGenericArguments();
				if (argumentTypes.Length <= 0)
				{
					argumentTypes = new Type[] { type.GetElementType() };
				}
				foreach (Type argumentType in argumentTypes)
				{
					if (typeof(Enum).IsAssignableFrom(argumentType))
					{
						Type enumType = argumentType;
						OnEnumGUI(position, property, label, enumType, true);
						return;
					}
				}
			}
			else
			{
				Type enumType = type;
				OnEnumGUI(position, property, label, enumType, false);
				return;
			}
		}
		base.OnGUI(position, property, label);
	}

	private void OnEnumGUI(Rect position, SerializedProperty property, GUIContent label, Type enumType, bool isCollection)
	{
		string[] enumNames = property.enumNames;
		int length = enumNames.Length;

		CacheCustomNames(enumType, enumNames);

		EditorGUI.BeginChangeCheck();

		EnumCustomNameAttribute enumCustomNameAttribute = attribute as EnumCustomNameAttribute;
		int[] order = enumCustomNameAttribute.mOrder;
		int[] realIndexs = order.Length == length ? GetRealIndexs(order) : GetDefaultIndexs(length);

		string[] customNames = new string[length];
		for (int i = 0; i < length; ++i)
		{
			string enumName = enumNames[realIndexs[i]];
			customNames[i] = mCustomEnumNameDict.ContainsKey(enumName) ? mCustomEnumNameDict[enumName] : enumName;
		}
		int index = GetCurrentIndex(realIndexs, property.enumValueIndex);
		if (index == -1 && property.enumValueIndex != -1)
		{
			SortingError(position, property, label);
			return;
		}
		string displayLabel = GetDisplayLabel(label.text, isCollection);
		index = EditorGUI.Popup(position, displayLabel, index, customNames);
		if (EditorGUI.EndChangeCheck())
		{
			if (index >= 0)
				property.enumValueIndex = realIndexs[index];
		}
	}

	private string GetDisplayLabel(string label, bool isCollection)
	{
		string displayLabel = (attribute as EnumCustomNameAttribute).mLabel;
		if (displayLabel == null)
		{
			return label;
		}

		if (isCollection)
		{
			string[] strs = label.Split(' ');
			int strLength = strs.Length;
			if (strLength > 1)
			{
				return displayLabel + " " + strs[strLength - 1];
			}
		}

		return displayLabel;
	}

	private void CacheCustomNames(Type enumType, string[] enumNames)
	{
		foreach (string enumName in enumNames)
		{
			FieldInfo field = enumType.GetField(enumName);
			if (field != null)
			{
				object[] attrs = field.GetCustomAttributes(typeof(EnumCustomNameAttribute), false);
				if (attrs.Length > 0)
				{
					mCustomEnumNameDict[enumName] = (attrs[0] as EnumCustomNameAttribute).mLabel;
				}
			}
		}
	}

	private int[] GetRealIndexs(int[] order)
	{
		int length = order.Length;
		int[] indexs = new int[length];
		for (int i = 0; i < length; ++i)
		{
			int index = 0;
			for (int j = 0; j < length; ++j)
			{
				if (order[i] > order[j])
				{
					++index;
				}
			}
			indexs[i] = index;
		}
		return indexs;
	}

	private int[] GetDefaultIndexs(int length)
	{
		int[] indexs = new int[length];
		for (int i = 0; i < length; ++i)
		{
			indexs[i] = i;
		}
		return indexs;
	}

	private int GetCurrentIndex(int[] realIndexs, int realIndex)
	{
		int length = realIndexs.Length;
		for (int i = 0; i < length; ++i)
		{
			if (realIndexs[i] == realIndex)
			{
				return i;
			}
		}
		return -1;
	}

	private void SortingError(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.PropertyField(position, property, new GUIContent(label.text + " (sorting error)"), true);
		EditorGUI.EndProperty();
	}
}