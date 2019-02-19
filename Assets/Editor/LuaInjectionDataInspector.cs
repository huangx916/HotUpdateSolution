using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Main;

[CanEditMultipleObjects]
[CustomEditor(typeof(LuaInjectionData), true)]
public class LuaInjectionDataInspector : Editor
{
	protected LuaInjectionData m_LuaInjectionData;

	public void OnEnable()
	{
		m_LuaInjectionData = target as LuaInjectionData;
	}

	public override void OnInspectorGUI()
	{
		EditorGUIUtility.labelWidth = 80;

		GUI.changed = false;

		FieldInfo field = typeof(LuaInjectionData).GetField("m_InjectionList", BindingFlags.Instance | BindingFlags.NonPublic);
		List<Injection4> injections = field.GetValue(m_LuaInjectionData) as List<Injection4>;

		DrawArray(injections, true);

		GUILayout.BeginHorizontal();
		Undo.RecordObject(m_LuaInjectionData, "Injection.Count");
		if (GUILayout.Button("+"))
		{
			Injection4 injection = new Injection4();
			int count = injections.Count;
			if (count > 0)
			{
				injection.Type = injections[count - 1].Type;
			}
			injections.Add(injection);
		}
		GUILayout.EndHorizontal();

		field.SetValue(m_LuaInjectionData, injections);

		if (GUI.changed)
		{
			EditorUtility.SetDirty(m_LuaInjectionData);
		}
	}

	private List<T> DrawArray<T>(List<T> array, bool isDict) where T : Injection, new()
	{
		Color oldColor = GUI.backgroundColor;
		for (int index = 0, realIndex = 0; index < array.Count; index++)
		{
			string styleName = null;

			T injection = array[index] ?? new T();
			if (injection.Type == InjectionType.List || injection.Type == InjectionType.Dict)
			{
				if (injection is IFoldable)
				{
					IFoldable foldable = injection as IFoldable;
					if (foldable.IsFolded)
					{
						styleName = "As TextArea";
						GUI.backgroundColor = Color.white * 0.8F;
						GUILayout.Space(-2F);
					}
				}
			}

			if (string.IsNullOrEmpty(styleName))
			{
				GUILayout.BeginHorizontal();
			}
			else
			{
				GUILayout.BeginHorizontal(styleName);
			}

			Undo.RecordObject(m_LuaInjectionData, "Injection.Count");
			bool remove = GUILayout.Button("×", GUILayout.Width(20F), GUILayout.Height(14F));

			GUILayout.Label("Type:", GUILayout.Width(40F));
			Undo.RecordObject(m_LuaInjectionData, "Injection.Type");
			injection.Type = (InjectionType) EditorGUILayout.EnumPopup(injection.Type, GUILayout.Width(75F));
			if (injection.Type == InjectionType.List || injection.Type == InjectionType.Dict)
			{
				if (typeof(T) == typeof(Injection))
				{
					injection.Type = InjectionType.String;
				}
			}
			GUI.enabled = index > 0;
			Undo.RecordObject(m_LuaInjectionData, "Injection.Up");
			bool up = GUILayout.Button("▲", GUILayout.Width(20F), GUILayout.Height(14F));
			GUI.enabled = index < array.Count - 1;
			Undo.RecordObject(m_LuaInjectionData, "Injection.Down");
			bool down = GUILayout.Button("▼", GUILayout.Width(20F), GUILayout.Height(14F));
			GUI.enabled = true;

			if (injection.Type == InjectionType.Space)
			{
				injection.Name = null;
				injection.Value = null;
			}
			else
			{
				if (isDict)
				{
					GUILayout.Label("Name:", GUILayout.Width(40F));
					injection.Name = EditorGUILayout.TextField(injection.Name ?? "", GUILayout.MaxWidth(120F));
				}
				else
				{
					GUILayout.Label("Index:", GUILayout.Width(40F));
					GUI.enabled = false;
					EditorGUILayout.IntField(realIndex + 1, GUILayout.Width(30F));
					GUI.enabled = true;
				}

				object value = injection.Value;
				object newValue;
				if (injection.Type == InjectionType.List || injection.Type == InjectionType.Dict)
				{
					newValue = value;
					bool valueIsDict = injection.Type == InjectionType.Dict;
					if (typeof(T) == typeof(Injection4))
					{
						Injection4 inj = injection as Injection4;
						if (!(newValue is List<Injection3>))
						{
							newValue = new List<Injection3>();
						}
						inj.IsFolded = DrawArrayValue(newValue as List<Injection3>, valueIsDict, inj.IsFolded);
					}
					else if (typeof(T) == typeof(Injection3))
					{
						Injection3 inj = injection as Injection3;
						if (!(newValue is List<Injection2>))
						{
							newValue = new List<Injection2>();
						}
						inj.IsFolded = DrawArrayValue(newValue as List<Injection2>, valueIsDict, inj.IsFolded);
					}
					else if (typeof(T) == typeof(Injection2))
					{
						Injection2 inj = injection as Injection2;
						if (!(newValue is List<Injection1>))
						{
							newValue = new List<Injection1>();
						}
						inj.IsFolded = DrawArrayValue(newValue as List<Injection1>, valueIsDict, inj.IsFolded);
					}
					else if (typeof(T) == typeof(Injection1))
					{
						Injection1 inj = injection as Injection1;
						if (!(newValue is List<Injection>))
						{
							newValue = new List<Injection>();
						}
						inj.IsFolded = DrawArrayValue(newValue as List<Injection>, valueIsDict, inj.IsFolded);
					}
				}
				else
				{
					newValue = DrawValue<T>(injection.Type, value);
				}
				if ((newValue != null && value != null) ? newValue.GetHashCode() != value.GetHashCode() : newValue != value)
				{
					injection.Value = newValue;
				}
				realIndex++;
			}

			GUILayout.EndHorizontal();

			if (injection.Type == InjectionType.List || injection.Type == InjectionType.Dict)
			{
				if (injection is IFoldable)
				{
					IFoldable foldable = injection as IFoldable;
					if (foldable.IsFolded)
					{
						GUI.backgroundColor = oldColor;
						GUILayout.Space(-2F);
					}
				}
			}

			if (remove)
			{
				array.RemoveAt(index);
				index--;
			}
			else if (up)
			{
				array[index] = array[index - 1];
				array[index - 1] = injection;
			}
			else if (down)
			{
				array[index] = array[index + 1];
				array[index + 1] = injection;
			}
			else
			{
				array[index] = injection;
			}
		}
		return array;
	}

	private object DrawValue<T>(InjectionType type, object value) where T : Injection, new()
	{
		switch (type)
		{
			case InjectionType.String:
				GUILayout.Label("Value:", GUILayout.Width(40F));
				Undo.RecordObject(m_LuaInjectionData, "Injection.Value");
				value = EditorGUILayout.TextArea(JsonParser.ObjectToString(value ?? ""));
				break;
			case InjectionType.Int:
				GUILayout.Label("Value:", GUILayout.Width(40F));
				Undo.RecordObject(m_LuaInjectionData, "Injection.Value");
				value = EditorGUILayout.IntField(JsonParser.ObjectToInt(value));
				break;
			case InjectionType.Float:
				GUILayout.Label("Value:", GUILayout.Width(40F));
				Undo.RecordObject(m_LuaInjectionData, "Injection.Value");
				value = EditorGUILayout.FloatField(JsonParser.ObjectToFloat(value));
				break;
			case InjectionType.Boolean:
				GUILayout.Label("Value:", GUILayout.Width(40F));
				Undo.RecordObject(m_LuaInjectionData, "Injection.Value");
				value = EditorGUILayout.Toggle(JsonParser.ObjectToBool(value));
				break;
			case InjectionType.Curve:
				AnimationCurve curve = value as AnimationCurve;
				GUILayout.Label("Value:", GUILayout.Width(40F));
				Undo.RecordObject(m_LuaInjectionData, "Injection.Value");
				value = EditorGUILayout.CurveField(curve ?? new AnimationCurve());
				break;
			case InjectionType.Color:
				Color color = (Color) value;
				GUILayout.Label("Value:", GUILayout.Width(40F));
				Undo.RecordObject(m_LuaInjectionData, "Injection.Value");
				value = EditorGUILayout.ColorField(color);
				break;
			case InjectionType.Vector2:
				Vector2 vector2 = (Vector2) value;
				GUILayout.Label("Value:", GUILayout.Width(40F));
				Undo.RecordObject(m_LuaInjectionData, "Injection.Value");
				value = EditorGUILayout.Vector2Field("", vector2);
				break;
			case InjectionType.Vector3:
				Vector3 vector3 = (Vector3) value;
				GUILayout.Label("Value:", GUILayout.Width(40F));
				Undo.RecordObject(m_LuaInjectionData, "Injection.Value");
				value = EditorGUILayout.Vector3Field("", vector3);
				break;
			case InjectionType.Vector4:
				Vector4 vector4 = (Vector4) value;
				GUILayout.Label("Value:", GUILayout.Width(40F));
				Undo.RecordObject(m_LuaInjectionData, "Injection.Value");
				value = EditorGUILayout.Vector4Field("", vector4);
				break;
			case InjectionType.Object:
				Object obj = value as Object;
				GUILayout.Label("Value:", GUILayout.Width(40F));
				Undo.RecordObject(m_LuaInjectionData, "Injection.Value");
				value = EditorGUILayout.ObjectField(obj, typeof(Object), true);
				break;
			case InjectionType.GameObject:
				GameObject go = null;
				if (value is GameObject)
				{
					go = value as GameObject;
				}
				else if (value is Component)
				{
					go = (value as Component).gameObject;
				}
				GUILayout.Label("Value:", GUILayout.Width(40F));
				Undo.RecordObject(m_LuaInjectionData, "Injection.Value");
				value = EditorGUILayout.ObjectField(go, typeof(GameObject), true);
				break;
			case InjectionType.Transform:
				Transform trans = null;
				if (value is Transform)
				{
					trans = value as Transform;
				}
				else if (value is Component)
				{
					trans = (value as Component).transform;
				}
				else if (value is GameObject)
				{
					trans = (value as GameObject).transform;
				}
				GUILayout.Label("Value:", GUILayout.Width(40F));
				Undo.RecordObject(m_LuaInjectionData, "Injection.Value");
				value = EditorGUILayout.ObjectField(trans, typeof(Transform), true);
				break;
			case InjectionType.Behaviour:
				Behaviour behaviour = null;
				if (value is Behaviour)
				{
					behaviour = value as Behaviour;
				}
				else if (value is Component)
				{
					behaviour = (value as Component).GetComponent<Behaviour>();
				}
				else if (value is GameObject)
				{
					behaviour = (value as GameObject).GetComponent<Behaviour>();
				}
				GUILayout.Label("Value:", GUILayout.Width(40F));
				Undo.RecordObject(m_LuaInjectionData, "Injection.Value");
				Behaviour currentBehaviour = EditorGUILayout.ObjectField(behaviour, typeof(Behaviour), true) as Behaviour;
				if (currentBehaviour)
				{
					Behaviour[] behaviours = currentBehaviour.GetComponents<Behaviour>();
					int behaviourCount = behaviours.Length;
					string[] behaviourNames = new string[behaviourCount];
					int[] behaviourIndexs = new int[behaviourCount];
					for (int index = 0; index < behaviourCount; index++)
					{
						behaviourNames[index] = index + "." + behaviours[index].GetType().Name;
						behaviourIndexs[index] = index;
					}

					Undo.RecordObject(m_LuaInjectionData, "Injection.Value");
					int currentIndex = behaviours.IndexOf(currentBehaviour);
					int behaviourIndex = EditorGUILayout.IntPopup(currentIndex, behaviourNames, behaviourIndexs);
					if (behaviourIndex != currentIndex)
					{
						currentBehaviour = behaviours[behaviourIndex];
					}
				}
				value = currentBehaviour;
				break;
			case InjectionType.OtherComp:
				Component comp = null;
				if (value is Component)
				{
					comp = value as Component;
				}
				else if (value is GameObject)
				{
					comp = (value as GameObject).GetComponent<Component>();
				}
				GUILayout.Label("Value:", GUILayout.Width(40F));
				Undo.RecordObject(m_LuaInjectionData, "Injection.Value");
				Component currentComp = EditorGUILayout.ObjectField(comp, typeof(Component), true) as Component;
				if (currentComp)
				{
					Component[] components = currentComp.GetComponents<Component>();
					List<Component> compList = new List<Component>();
					foreach (Component component in components)
					{
						if (!(component is Behaviour))
						{
							compList.Add(component);
						}
					}
					int compCount = compList.Count;
					if (compCount > 0)
					{
						string[] compNames = new string[compCount];
						int[] compIndexs = new int[compCount];
						for (int index = 0; index < compCount; index++)
						{
							compNames[index] = index + "." + compList[index].GetType().Name;
							compIndexs[index] = index;
						}
						int currentIndex = compList.IndexOf(currentComp);
						if (currentIndex != -1)
						{
							Undo.RecordObject(m_LuaInjectionData, "Injection.Value");
							int behaviourIndex = EditorGUILayout.IntPopup(currentIndex, compNames, compIndexs);
							if (behaviourIndex != currentIndex)
							{
								currentComp = compList[behaviourIndex];
							}
						}
						else
						{
							currentComp = compList[0];
						}
					}
					else
					{
						currentComp = null;
					}
				}
				value = currentComp;
				break;
		}
		return value;
	}

	private bool DrawArrayValue<T>(List<T> array, bool isDict, bool isFolded = false) where T : Injection, new()
	{
		GUILayout.Label("Size:", GUILayout.Width(40F));
		GUI.enabled = false;
		EditorGUILayout.IntField(array.Count);
		GUI.enabled = true;

		if (!isFolded)
		{
			Undo.RecordObject(m_LuaInjectionData, "Injection.Append");
			if (GUILayout.Button("+", GUILayout.Width(20F), GUILayout.Height(14F)))
			{
				T t = new T();
				int count = array.Count;
				if (count > 0)
				{
					t.Type = array[count - 1].Type;
				}
				array.Add(t);
			}
		}

		Undo.RecordObject(m_LuaInjectionData, "Injection.Fold");
		if (GUILayout.Button(isFolded ? "\u25C4" : "\u25BC", GUILayout.Width(20F), GUILayout.Height(14F)))
		{
			isFolded = !isFolded;
		}
		if (!isFolded)
		{
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Space(20F);
			GUILayout.BeginVertical();
			DrawArray(array, isDict);
			GUILayout.EndVertical();
		}

		return isFolded;
	}
}
