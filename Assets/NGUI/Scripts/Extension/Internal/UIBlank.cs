//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Blank UI but makes a draw call.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI Blank")]
public class UIBlank : UIWidget
{

	/// <summary>
	/// Whether to force drawing while the widget is visible.
	/// </summary>

	public override bool forceDraw
	{
		get
		{
			return true;
		}
	}
}
