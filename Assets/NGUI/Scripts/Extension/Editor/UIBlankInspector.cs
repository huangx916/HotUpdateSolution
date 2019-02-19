//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Inspector class used to edit UIEffects.
/// </summary>

[CanEditMultipleObjects]
[CustomEditor(typeof(UIBlank), true)]
public class UIBlankInspector : UIWidgetInspector
{

	/// <summary>
	/// Draw all the custom properties.
	/// </summary>

	protected override void DrawCustomProperties()
	{
		DrawAlpha(serializedObject, mWidget);
		if (!NGUISettings.unifiedTransform)
		{
			DrawInspectorProperties(serializedObject, mWidget, false, this);
		}
	}
}
