using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UIWidget))]
[RequireComponent(typeof(UIProgressBar))]
public class UIProgressColor : MonoBehaviour {

	[Serializable]
	public struct ProgressColor
	{
		public float progress;
		public Color color;
	}

	public List<ProgressColor> colors = new List<ProgressColor>();

	private UIProgressBar mBar;

	void Awake()
	{
		Init();
	}

	[ContextMenu("Init")]
	private void Init()
	{
		colors.Sort(ProgressSort);

		mBar = GetComponent<UIProgressBar>();
		EventDelegate.Add(mBar.onChange, OnProgressChange);
	}

	private int ProgressSort(ProgressColor pc1, ProgressColor pc2)
	{
		float delta = pc1.progress - pc2.progress;
		if (delta < 0)
		{
			return -1;
		}
		if (delta > 0)
		{
			return 1;
		}
		return 0;
	}

	[ContextMenu("Apply")]
	private void OnProgressChange()
	{
		int count = colors.Count;
		if (count > 0 && mBar.foregroundWidget)
		{
			float value = mBar.value;
			int floorIndex = 0;
			for (int index = count - 1; index >= 0; index--)
			{
				if (colors[index].progress <= value)
				{
					floorIndex = index;
					break;
				}
			}
			int ceilIndex = count - 1;
			for (int index = 0; index < count; index++)
			{
				if (colors[index].progress >= value)
				{
					ceilIndex = index;
					break;
				}
			}
			Color color;
			if (floorIndex == ceilIndex)
			{
				color = colors[floorIndex].color;
			}
			else
			{
				float deltaProgress = colors[ceilIndex].progress - colors[floorIndex].progress;
				float percent = (value - colors[floorIndex].progress) / deltaProgress;
				color = Color.Lerp(colors[floorIndex].color, colors[ceilIndex].color, percent);
			}
			mBar.foregroundWidget.color = color;
		}
	}
}
