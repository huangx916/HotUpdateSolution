//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UITweener), true)]
public class UITweenerEditor : Editor
{
	protected bool preview;

	public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		base.OnInspectorGUI();
		DrawCommonProperties();
	}

	protected void DrawCommonProperties ()
	{
		UITweener tw = target as UITweener;

		NGUIEditorTools.SetLabelWidth(110F);
		if (DrawPreviewHeader())
		{
			NGUIEditorTools.BeginContents();
			GUI.changed = false;
			float tweenFactor = EditorGUILayout.Slider("Tween Factor", tw.tweenFactor, 0f, 1f);
			if (GUI.changed || !preview)
			{
				NGUIEditorTools.RegisterUndo("Tween Change", tw);
				tw.tweenFactor = tweenFactor;
				tw.Sample(tweenFactor, false);
				NGUITools.SetDirty(tw);
			}
			NGUIEditorTools.EndContents();

			preview = true;
		}

		if (NGUIEditorTools.DrawHeader("Tweener"))
		{
			NGUIEditorTools.BeginContents();

			bool changed = GUI.changed = false;

			UITweener.Style style = (UITweener.Style) EditorGUILayout.EnumPopup("Play Style", tw.style);
			AnimationCurve curve = EditorGUILayout.CurveField("Animation Curve", tw.animationCurve, GUILayout.Width(EditorGUIUtility.currentViewWidth - 50f), GUILayout.Height(62f));
			//UITweener.Method method = (UITweener.Method)EditorGUILayout.EnumPopup("Play Method", tw.method);
			changed |= GUI.changed;

			GUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			EditorGUILayout.BeginVertical();
			if (NGUIEditorTools.DrawHeader("Curve Data"))
			{
				GUI.changed = false;

				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(1f);
				EditorGUILayout.BeginVertical("As TextArea", GUILayout.MinHeight(18));
				GUILayout.Space(2f);
				curve = DrawCurveData(curve);
				GUILayout.Space(2f);
				EditorGUILayout.EndVertical();
				GUILayout.Space(3f);
				EditorGUILayout.EndHorizontal();

				changed |= GUI.changed;
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);

			GUI.changed = false;

			GUILayout.BeginHorizontal();
			float del = EditorGUILayout.FloatField("Start Delay", tw.delay, GUILayout.Width(170f));
			GUILayout.Label("seconds");
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			float de = EditorGUILayout.FloatField("Delay Enter Curve", tw.curveDelayEnter, GUILayout.Width(170f));
			GUILayout.Label("seconds");
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			float dur = EditorGUILayout.FloatField("Duration", tw.duration, GUILayout.Width(170f));
			dur = Mathf.Abs(dur);
			GUILayout.Label("seconds");
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			float ee = EditorGUILayout.FloatField("Early Exit Curve", tw.curveEarlyExit, GUILayout.Width(170f));
			GUILayout.Label("seconds");
			GUILayout.EndHorizontal();

			int tg = EditorGUILayout.IntField("Tween Group", tw.tweenGroup, GUILayout.Width(170f));

			NGUIEditorTools.SetLabelWidth(156f);
			bool ts = EditorGUILayout.Toggle("Ignore TimeScale", tw.ignoreTimeScale);
			bool fx = EditorGUILayout.Toggle("Use Fixed Update", tw.useFixedUpdate);
			bool ia = EditorGUILayout.Toggle("Init On Awake", tw.setBeginningOnAwake);
			bool ie = EditorGUILayout.Toggle("Init On Enable", tw.SetBeginningOnEnable);
			bool ap = EditorGUILayout.Toggle("Auto Play On Enable", tw.AutoPlayOnEnable);

			changed |= GUI.changed;
			if (changed)
			{
				NGUIEditorTools.RegisterUndo("Tween Change", tw);
				tw.animationCurve = curve;
				//tw.method = method;
				tw.style = style;
				tw.ignoreTimeScale = ts;
				tw.tweenGroup = tg;
				tw.delay = del;
				tw.curveDelayEnter = de;
				tw.duration = dur;
				tw.curveEarlyExit = ee;
				tw.useFixedUpdate = fx;
				tw.setBeginningOnAwake = ia;
				tw.SetBeginningOnEnable = ie;
				tw.AutoPlayOnEnable = ap;
				if (preview)
				{
					tw.Sample(tw.tweenFactor, false);
				}
				NGUITools.SetDirty(tw);
			}
			NGUIEditorTools.EndContents();
		}

		NGUIEditorTools.SetLabelWidth(80f);
		NGUIEditorTools.DrawEvents("On Finished", tw, tw.onFinished);
	}

	private AnimationCurve DrawCurveData(AnimationCurve curve)
	{
		Keyframe[] keyFrames = curve.keys;

		bool keyClamp = EditorPrefs.GetBool("Clamp Key", true);
		bool keyClampTemp = EditorGUILayout.Toggle("  Clamp Key", keyClamp);
		if (keyClampTemp != keyClamp)
		{
			keyClamp = keyClampTemp;
			EditorPrefs.SetBool("Clamp Key", keyClamp);
		}

		float labelWidth = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = 35F;
		for (int i = 0, length = keyFrames.Length; i < length; i++)
		{
			GUILayout.Label("  Element " + i);
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(30f);
			float time = EditorGUILayout.FloatField("time", keyFrames[i].time);
			float value = EditorGUILayout.FloatField("value", keyFrames[i].value);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(30f);
			float inTangent = EditorGUILayout.FloatField("left", keyFrames[i].inTangent);
			float outTangent = EditorGUILayout.FloatField("right", keyFrames[i].outTangent);
			EditorGUILayout.EndHorizontal();

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

		GUILayout.Space(10f);

		return new AnimationCurve(keyFrames);
	}

	/// <summary>
	/// Draw a distinctly different looking header label
	/// </summary>

	public bool DrawPreviewHeader()
	{
		string text = "Tweener Preview";

		GUILayout.Space(3f);
		if (!preview)
			GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
		GUILayout.BeginHorizontal();
		GUI.changed = false;

		text = "<b><size=11>" + text + "</size></b>";
		if (preview)
			text = "\u25BC " + text;
		else
			text = "\u25BA " + text;
		if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f)))
			preview = !preview;

		GUILayout.Space(2f);
		GUILayout.EndHorizontal();
		GUI.backgroundColor = Color.white;
		if (!preview)
			GUILayout.Space(3f);

		return preview;
	}
}
