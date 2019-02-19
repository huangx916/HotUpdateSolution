using UnityEngine;

public class DisabledInInspectorAttribute : PropertyAttribute
{
	public DisabledInInspectorAttribute()
	{
	}
}

public class EnumFlagsAttribute : PropertyAttribute {
	public EnumFlagsAttribute()
	{
	}
}

public class CustomCurveAttribute : PropertyAttribute {
	public CustomCurveAttribute()
	{
	}
}

public class HexColorAttribute : PropertyAttribute
{
	public HexColorAttribute()
	{
	}
}

public class CustomNameAttribute : PropertyAttribute
{
	public GUIContent mLabel;
	public CustomNameAttribute(string label)
	{
		mLabel = new GUIContent(label);
	}
}

public class EnumCustomNameAttribute : PropertyAttribute
{
	public string mLabel;
	public int[] mOrder;

	public EnumCustomNameAttribute() : this(null) {}

	public EnumCustomNameAttribute(string label, params int[] order)
	{
		mLabel = label;
		mOrder = order;
	}
}