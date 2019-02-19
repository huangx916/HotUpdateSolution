namespace UnityEditor
{
    using System;
	using System.Reflection;
	using UnityEngine;

    [CustomEditor(typeof(Transform)), CanEditMultipleObjects]
    internal class NGUITransformInspector : Editor
	{
		private SerializedProperty m_Position;
		private object m_RotationGUI;
		private MethodInfo m_RotationGUIFieldMethod;
		private MethodInfo m_RotationGUIOnEnableMethod;
		private SerializedProperty m_Scale;
		private SerializedProperty m_Rotation;
		private bool m_WideMode;
        private static Contents s_Contents;
		private static NGUITransformInspector s_Instance;

		private void Inspector3D()
        {
            EditorGUILayout.PropertyField(this.m_Position, s_Contents.positionContent, new GUILayoutOption[0]);

			m_RotationGUIFieldMethod.Invoke(this.m_RotationGUI, null);

            EditorGUILayout.PropertyField(this.m_Scale, s_Contents.scaleContent, new GUILayoutOption[0]);
		}

		public void OnEnable()
		{
			s_Instance = this;

			this.m_Position = base.serializedObject.FindProperty("m_LocalPosition");
			this.m_Rotation = base.serializedObject.FindProperty("m_LocalRotation");
            this.m_Scale = base.serializedObject.FindProperty("m_LocalScale");
			if (this.m_RotationGUI == null)
			{
				string unityEditorPath = EditorApplication.applicationContentsPath + "/Managed/UnityEditor.dll";
				Assembly assembly = Assembly.LoadFrom(unityEditorPath);
				Type rotationGUIType = assembly.GetType("UnityEditor.TransformRotationGUI");

				ConstructorInfo rotationGUIConstructor = rotationGUIType.GetConstructor(new Type[0]);
				this.m_RotationGUI = rotationGUIConstructor.Invoke(null);

				m_RotationGUIFieldMethod = rotationGUIType.GetMethod("RotationField", new Type[0]);
				m_RotationGUIOnEnableMethod = rotationGUIType.GetMethod("OnEnable", new Type[] { typeof(SerializedProperty), typeof(GUIContent) });
			}
			if (s_Contents == null)
			{
				s_Contents = new Contents();
			}
			m_RotationGUIOnEnableMethod.Invoke(this.m_RotationGUI, new object[] { this.m_Rotation, s_Contents.anglesContent });
		}

		public void OnDestroy()
		{
			s_Instance = null;
		}

		public override void OnInspectorGUI()
		{
			m_WideMode = EditorGUIUtility.wideMode;
			if (m_WideMode)
            {
                EditorGUIUtility.labelWidth = 80;
            }
			else
			{
				EditorGUIUtility.wideMode = true;
				EditorGUIUtility.labelWidth = 28F;
			}
			base.serializedObject.Update();
            this.Inspector3D();
			this.DrawRotation();
			this.DrawResetBtns();
			Transform target = base.target as Transform;
            Vector3 position = target.position;
            if (((Mathf.Abs(position.x) > 100000f) || (Mathf.Abs(position.y) > 100000f)) || (Mathf.Abs(position.z) > 100000f))
            {
                EditorGUILayout.HelpBox(s_Contents.floatingPointWarning, MessageType.Warning);
            }
            base.serializedObject.ApplyModifiedProperties();

			if (NGUISettings.unifiedTransform)
			{
				NGUIEditorTools.SetLabelWidth(80f);

				if (UIWidgetInspector.instance != null)
				{
					UIWidgetInspector.instance.serializedObject.Update();
					UIWidgetInspector.instance.DrawWidgetTransform();
					if (NGUISettings.minimalisticLook)
						GUILayout.Space(-4f);
					UIWidgetInspector.instance.serializedObject.ApplyModifiedProperties();
				}

				if (UIRectEditor.instance != null)
				{
					UIRectEditor.instance.serializedObject.Update();
					UIRectEditor.instance.DrawAnchorTransform();
					UIRectEditor.instance.serializedObject.ApplyModifiedProperties();
				}
			}
		}

		private void DrawRotation()
		{
			GUILayout.BeginHorizontal();
			{
				float titleWidth = Mathf.Floor(EditorGUIUtility.labelWidth);
				GUILayout.Label(s_Contents.rotationContent, GUILayout.Width(titleWidth - 5F));

				EditorGUIUtility.labelWidth = 13;
				EditorGUILayout.PropertyField(this.m_Rotation.FindPropertyRelative("x"), GUILayout.MinWidth(44));
				EditorGUILayout.PropertyField(this.m_Rotation.FindPropertyRelative("y"), GUILayout.MinWidth(44));
				EditorGUILayout.PropertyField(this.m_Rotation.FindPropertyRelative("z"), GUILayout.MinWidth(44));
				EditorGUIUtility.labelWidth = 15;
				EditorGUILayout.PropertyField(this.m_Rotation.FindPropertyRelative("w"), GUILayout.MinWidth(44));
			}
			GUILayout.EndHorizontal();
		}

		private void DrawResetBtns()
		{
			GUILayout.Space(-73);
			if (GUILayout.Button("P", GUILayout.Width(24), GUILayout.Height(15)))
			{
				if (Event.current.button == 0)
				{
					m_Position.vector3Value = Vector3.zero;
				}
				else
				{
					m_Position.vector3Value = Round(m_Position.vector3Value);
				}
			}
			if (GUILayout.Button("A", GUILayout.Width(24), GUILayout.Height(15)))
			{
				if (Event.current.button == 0)
				{
					m_Rotation.quaternionValue = Quaternion.identity;
				}
				else
				{
					m_Rotation.quaternionValue = Quaternion.Euler(Round(m_Rotation.quaternionValue.eulerAngles));
				}
			}
			if (GUILayout.Button("S", GUILayout.Width(24), GUILayout.Height(15)))
			{
				if (Event.current.button == 0)
				{
					m_Scale.vector3Value = Vector3.one;
				}
				else
				{
					m_Scale.vector3Value = Round(m_Scale.vector3Value);
				}
			}
			if (GUILayout.Button("R", GUILayout.Width(24), GUILayout.Height(15)))
			{
				if (Event.current.button == 0)
				{
					m_Rotation.quaternionValue = Quaternion.identity;
				}
				else
				{
					m_Rotation.quaternionValue = Quaternion.Euler(Round(m_Rotation.quaternionValue.eulerAngles));
				}
			}
		}

		public static Vector3 Round(Vector3 vector)
		{
			vector.x = Mathf.Round(vector.x);
			vector.y = Mathf.Round(vector.y);
			vector.z = Mathf.Round(vector.z);
			return vector;
		}

		public static NGUITransformInspector instance
		{
			get
			{
				return s_Instance;
			}
		}

		private class Contents
        {
            public string floatingPointWarning = LocalizationDatabase.GetLocalizedString("Due to floating-point precision limitations, it is recommended to bring the world coordinates of the GameObject within a smaller range.");
            public GUIContent positionContent = new GUIContent(LocalizationDatabase.GetLocalizedString("      Position"), LocalizationDatabase.GetLocalizedString("The local position of this Game Object relative to the parent."));
			public GUIContent anglesContent = new GUIContent(LocalizationDatabase.GetLocalizedString("      Angles"), LocalizationDatabase.GetLocalizedString("The local eulerAngles of this Game Object relative to the parent."));
            public GUIContent scaleContent = new GUIContent(LocalizationDatabase.GetLocalizedString("      Scale"), LocalizationDatabase.GetLocalizedString("The local scaling of this Game Object relative to the parent."));
			public GUIContent rotationContent = new GUIContent(LocalizationDatabase.GetLocalizedString("      Rotation"), LocalizationDatabase.GetLocalizedString("The local rotation of this Game Object relative to the parent."));
			public GUIContent[] subLabels = new GUIContent[] { new GUIContent("X"), new GUIContent("Y") };
        }
	}
}

