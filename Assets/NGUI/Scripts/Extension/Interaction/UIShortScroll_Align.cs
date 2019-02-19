using UnityEngine;
using System;

public partial class UIShortScroll {

	public enum AlignType
	{
		AT_Forward,
		AT_Center,
		AT_Backward
	}

	[Space(15)]
	public bool alignEnable;
	public AlignType alignType = AlignType.AT_Center;
	public float springStrength = 8f;
	public float nextPageThreshold = 0f;
	public bool restrictEnable = true;
	public bool basedOnSoftness = false;

	public SpringPanel.OnFinished onFinished;
	public Action<int> onAlign;

	private int mAlignedIndex = int.MinValue;
	public int alignedIndex
	{
		get
		{
			return mAlignedIndex;
		}
	}

	void AlignStart()
	{
		Realign();
	}

	void OnEnable()
	{
		Realign();
		if (mScrollView)
		{
			mScrollView.onDragFinished = OnDragFinished;
		}
	}

	void OnDisable()
	{
		if (mScrollView)
		{
			mScrollView.onDragFinished -= OnDragFinished;
		}
	}

	void OnValidate()
	{
		nextPageThreshold = Mathf.Abs(nextPageThreshold);
	}

	private void OnDragFinished()
	{
		if (enabled)
		{
			Realign();
		}
	}

	[ContextMenu("Realign")]
	public void Realign()
	{
		if (!alignEnable || mTotalCount == 0 || space == 0 || mScrollView == null)
		{
			return;
		}
		AttachDragFinishedDelegate();

		Vector3 panelAlignPoint = CalculatePanelAlignPoint();
		int index = DetermineTheClosestIndex(mTrans, panelAlignPoint);
		index = NextPageThreshold(index);

		AlignOn(index, panelAlignPoint);
	}

	private void AttachDragFinishedDelegate()
	{
		mScrollView.onDragFinished = OnDragFinished;
		if (mScrollView.horizontalScrollBar != null)
		{
			mScrollView.horizontalScrollBar.onDragFinished = OnDragFinished;
		}
		if (mScrollView.verticalScrollBar != null)
		{
			mScrollView.verticalScrollBar.onDragFinished = OnDragFinished;
		}
	}

	private Vector3 CalculatePanelAlignPoint()
	{
		Vector2 clipOffset = mPanel.clipOffset;
		Vector4 clipRegion = mPanel.baseClipRegion;
		Vector2 softness = basedOnSoftness ? mPanel.clipSoftness : Vector2.zero;

		if (mScrollView.canMoveVertically)
		{
			if (alignType == AlignType.AT_Forward)
			{
				float localAlignPoint = clipRegion.w * 0.5F - softness.y + clipRegion.y + clipOffset.y;
				return localAlignPoint * Vector3.up;
			}
			else if (alignType == AlignType.AT_Backward)
			{
				float localAlignPoint = -clipRegion.w * 0.5F + softness.y + clipRegion.y + clipOffset.y;
				return localAlignPoint * Vector3.up;
			}
			return new Vector3(0, clipRegion.y + clipOffset.y, 0);
		}
		else if (mScrollView.canMoveHorizontally)
		{
			if (alignType == AlignType.AT_Forward)
			{
				float localAlignPoint = -clipRegion.z * 0.5F + softness.x + clipRegion.x + clipOffset.x;
				return localAlignPoint * Vector3.right;
			}
			else if (alignType == AlignType.AT_Backward)
			{
				float localAlignPoint = clipRegion.z * 0.5F - softness.x + clipRegion.x + clipOffset.x;
				return localAlignPoint * Vector3.right;
			}
			return new Vector3(clipRegion.x + clipOffset.x, 0, 0);
		}
		return new Vector3(clipRegion.x + clipOffset.x, clipRegion.y + clipOffset.y, 0);
	}

	private int DetermineTheClosestIndex(Transform trans, Vector3 panelAlignPoint)
	{
		// Offset this value by the momentum
		Vector3 momentum = mScrollView.currentMomentum * mScrollView.momentumAmount;
		Vector3 moveDelta = NGUIMath.SpringDampen(ref momentum, 9, 2f);
		Vector3 panelAlignPointInWorld = mScrollView.transform.TransformPoint(panelAlignPoint);
		Vector3 pickingPointInWorld = panelAlignPointInWorld - moveDelta * 0.05f; // Magic number based on what "feels right"
		Vector3 pickingPoint = mTrans.InverseTransformPoint(pickingPointInWorld);
		mScrollView.currentMomentum = Vector3.zero;

		// Calculate target index.
		if (mAxis == Vector3.right)
		{
			float indexF = pickingPoint.x / space;
			if (alignType == AlignType.AT_Center)
			{
				return AdsorbIndex_HorizontalCenter(indexF);
			}
			else if ((alignType == AlignType.AT_Forward && space > 0) ||
				(alignType == AlignType.AT_Backward && space < 0))
			{
				return AdsorbIndex_HorizontalForward(indexF);
			}
			else
			{
				return AdsorbIndex_HorizontalBackward(indexF);
			}
		}
		else if (mAxis == Vector3.down)
		{
			float indexF = pickingPoint.y / -space;
			if (alignType == AlignType.AT_Center)
			{
				return AdsorbIndex_VerticalCenter(indexF);
			}
			else if ((alignType == AlignType.AT_Forward && space > 0) ||
				(alignType == AlignType.AT_Backward && space < 0))
			{
				return AdsorbIndex_VerticalForward(indexF);
			}
			else
			{
				return AdsorbIndex_VerticalBackward(indexF);
			}
		}
		return 0;
	}

	private int AdsorbIndex_HorizontalCenter(float indexF)
	{
		int index = Mathf.RoundToInt(indexF);
		if (!restrictEnable)
		{
			return index;
		}

		float indexOffset = index - indexF;
		// If spring next, compare target-item distance to last-item;
		// Else if spring previous, compare target-item distance to first-item.
		// Otherwise, align to index.
		float indexDis = Mathf.Abs(indexOffset * space);
		// float direction = Mathf.Sign(space);
		if (indexOffset < 0)
		{
			//float softness = basedOnSoftness ? mPanel.clipSoftness.x : 0;
			//float panelHalfWith = mPanel.baseClipRegion.z * 0.5F - softness;
			//float lastItemHalfWidth = (mTrans.localPosition.x - mPanel.baseClipRegion.x) - -panelHalfWith * direction ;
			//float limitDis = Mathf.Abs((mTotalCount - 1 - indexF) * space + lastItemHalfWidth - panelHalfWith * direction);
			float limitDis = Mathf.Abs((mTotalCount - 1 - indexF) * space + mTrans.localPosition.x - mPanel.baseClipRegion.x);
			if (indexDis > limitDis)
			{
				// Last-item is closest, align to index + 1.
				return index + 1;
			}
			else
			{
				// Target-item is closest, align to index.
				return index;
			}
		}
		else if (indexOffset > 0)
		{
			//float softness = basedOnSoftness ? mPanel.clipSoftness.x : 0;
			//float panelHalfWith = mPanel.baseClipRegion.z * 0.5F - softness;
			//float firstItemHalfWidth = -panelHalfWith * direction - (mTrans.localPosition.x - mPanel.baseClipRegion.x);
			//float limitDis = Mathf.Abs(-indexF * space + firstItemHalfWidth - -panelHalfWith * direction);
			float limitDis = Mathf.Abs(-indexF * space - mTrans.localPosition.x + mPanel.baseClipRegion.x);
			if (indexDis < limitDis)
			{
				// Target-item is closest, align to index.
				return index;
			}
			else
			{
				// First-item is closest, align to index - 1.
				return index - 1;
			}
		}
		return index;
	}

	private int AdsorbIndex_HorizontalForward(float indexF)
	{
		int index = Mathf.CeilToInt(indexF);
		if (!restrictEnable)
		{
			return index;
		}

		float indexOffset = (index - 0.5F) - indexF;
		// If spring next, compare target-item distance to last-item distance is unnecessary.
		if (indexOffset > 0)
		{
			// Spring next, align to index.
			return index;
		}
		// Compare target-item to last-item, witch is closest.
		float indexDis = Mathf.Abs(indexOffset * space);
		float direction = Mathf.Sign(space);
		float softness = basedOnSoftness ? mPanel.clipSoftness.x : 0;
		float panelHalfWith = mPanel.baseClipRegion.z * 0.5F - softness;
		//float lastItemHalfWidth = (mTrans.localPosition.x - mPanel.baseClipRegion.x) - -panelHalfWith * direction ;
		//float limitDis = Mathf.Abs((mTotalCount - 1 - indexF) * space + lastItemHalfWidth - (panelHalfWith + panelHalfWith) * direction);
		float limitDis = Mathf.Abs((mTotalCount - 1 - indexF) * space + mTrans.localPosition.x - mPanel.baseClipRegion.x - panelHalfWith * direction);
		if (indexDis > limitDis)
		{
			// Last-item is closest, align to index + 1.
			return index + 1;
		}
		else
		{
			// Target-item is closest, align to index.
			return index;
		}
	}

	private int AdsorbIndex_HorizontalBackward(float indexF)
	{
		int index = Mathf.FloorToInt(indexF);
		if (!restrictEnable)
		{
			return index;
		}

		float indexOffset = indexF - (index + 0.5F);
		// If spring previous, compare target-item distance to first-item distance is unnecessary.
		if (indexOffset > 0)
		{
			// Spring previous, align to index.
			return index;
		}
		// Compare target-item to first-item, witch is closest.
		float indexDis = Mathf.Abs(indexOffset * space);
		float direction = Mathf.Sign(space);
		float softness = basedOnSoftness ? mPanel.clipSoftness.x : 0;
		float panelHalfWith = mPanel.baseClipRegion.z * 0.5F - softness;
		//float firstItemHalfWidth = -panelHalfWith * direction - (mTrans.localPosition.x - mPanel.baseClipRegion.x);
		//float limitDis = Mathf.Abs(-indexF * space + firstItemHalfWidth - -(panelHalfWith + panelHalfWith) * direction);
		float limitDis = Mathf.Abs(-indexF * space - mTrans.localPosition.x + mPanel.baseClipRegion.x + panelHalfWith * direction);
		if (indexDis < limitDis)
		{
			// Target-item is closest, align to index.
			return index;
		}
		else
		{
			// First-item is closest, align to index - 1.
			return index - 1;
		}
	}

	private int AdsorbIndex_VerticalCenter(float indexF)
	{
		int index = Mathf.RoundToInt(indexF);
		if (!restrictEnable)
		{
			return index;
		}

		float indexOffset = index - indexF;
		// If spring next, compare target-item distance to last-item;
		// Else if spring previous, compare target-item distance to first-item.
		// Otherwise, align to index.
		float indexDis = Mathf.Abs(indexOffset * -space);
		// float direction = Mathf.Sign(-space);
		if (indexOffset < 0)
		{
			//float softness = basedOnSoftness ? mPanel.clipSoftness.y : 0;
			//float panelHalfHeight = mPanel.baseClipRegion.w * 0.5F - softness;
			//float lastItemHalfHeight = (mTrans.localPosition.y - mPanel.baseClipRegion.y) - -panelHalfHeight * direction ;
			//float limitDis = Mathf.Abs((mTotalCount - 1 - indexF) * -space + lastItemHalfHeight - panelHalfHeight * direction);
			float limitDis = Mathf.Abs((mTotalCount - 1 - indexF) * -space + mTrans.localPosition.y - mPanel.baseClipRegion.y);
			if (indexDis > limitDis)
			{
				// Last-item is closest, align to index + 1.
				return index + 1;
			}
			else
			{
				// Target-item is closest, align to index.
				return index;
			}
		}
		else if (indexOffset > 0)
		{
			//float softness = basedOnSoftness ? mPanel.clipSoftness.y : 0;
			//float panelHalfHeight = mPanel.baseClipRegion.w * 0.5F - softness;
			//float firstItemHalfHeight = -panelHalfHeight * direction - (mTrans.localPosition.y - mPanel.baseClipRegion.y);
			//float limitDis = Mathf.Abs(-indexF * -space + firstItemHalfHeight - -panelHalfHeight * direction);
			float limitDis = Mathf.Abs(-indexF * -space - mTrans.localPosition.y + mPanel.baseClipRegion.y);
			if (indexDis < limitDis)
			{
				// Target-item is closest, align to index.
				return index;
			}
			else
			{
				// First-item is closest, align to index - 1.
				return index - 1;
			}
		}
		return index;
	}

	private int AdsorbIndex_VerticalForward(float indexF)
	{
		int index = Mathf.CeilToInt(indexF);
		if (!restrictEnable)
		{
			return index;
		}

		float indexOffset = (index - 0.5F) - indexF;
		// If spring next, compare target-item distance to last-item distance is unnecessary.
		if (indexOffset > 0)
		{
			// Spring next, align to index.
			return index;
		}
		// Compare target-item to last-item, witch is closest.
		float indexDis = Mathf.Abs(indexOffset * -space);
		float direction = Mathf.Sign(-space);
		float softness = basedOnSoftness ? mPanel.clipSoftness.y : 0;
		float panelHalfHeight = mPanel.baseClipRegion.w * 0.5F - softness;
		//float lastItemHalfHeight = (mTrans.localPosition.y - mPanel.baseClipRegion.y) - -panelHalfHeight * direction ;
		//float limitDis = Mathf.Abs((mTotalCount - 1 - indexF) * -space + lastItemHalfHeight - (panelHalfHeight + panelHalfHeight) * direction);
		float limitDis = Mathf.Abs((mTotalCount - 1 - indexF) * -space + mTrans.localPosition.y - mPanel.baseClipRegion.y - panelHalfHeight * direction);
		if (indexDis > limitDis)
		{
			// Last-item is closest, align to index + 1.
			return index + 1;
		}
		else
		{
			// Target-item is closest, align to index.
			return index;
		}
	}

	private int AdsorbIndex_VerticalBackward(float indexF)
	{
		int index = Mathf.FloorToInt(indexF);
		if (!restrictEnable)
		{
			return index;
		}

		float indexOffset = indexF - (index + 0.5F);
		// If spring previous, compare target-item distance to first-item distance is unnecessary.
		if (indexOffset > 0)
		{
			// Spring previous, align to index.
			return index;
		}
		// Compare target-item to first-item, witch is closest.
		float indexDis = Mathf.Abs(indexOffset * -space);
		float direction = Mathf.Sign(-space);
		float softness = basedOnSoftness ? mPanel.clipSoftness.y : 0;
		float panelHalfHeight = mPanel.baseClipRegion.w * 0.5F - softness;
		//float firstItemHalfHeight = -panelHalfHeight * direction - (mTrans.localPosition.y - mPanel.baseClipRegion.y);
		//float limitDis = Mathf.Abs(-indexF * -space + firstItemHalfHeight - -(panelHalfHeight + panelHalfHeight) * direction);
		float limitDis = Mathf.Abs(-indexF * -space - mTrans.localPosition.y + mPanel.baseClipRegion.y + panelHalfHeight * direction);
		if (indexDis < limitDis)
		{
			// Target-item is closest, align to index.
			return index;
		}
		else
		{
			// First-item is closest, align to index - 1.
			return index - 1;
		}
	}

	private int NextPageThreshold(int index)
	{
		// If we're still on the same object
		if (mAlignedIndex == index)
		{
			// If we have a touch in progress and the next page threshold set
			if (nextPageThreshold > 0f && UICamera.currentTouch != null)
			{
				Vector2 totalDelta = UICamera.currentTouch.totalDelta;

				float delta = 0f;

				switch (mScrollView.movement)
				{
					case UIScrollView.Movement.Horizontal:
						delta = totalDelta.x;
						break;
					case UIScrollView.Movement.Vertical:
						delta = -totalDelta.y;
						break;
					default:
						delta = totalDelta.magnitude;
						break;
				}

				// if negative direction
				if (space < 0)
				{
					delta = -delta;
				}

				if (delta > nextPageThreshold)
				{
					// Prev item
					--index;
				}
				else if (delta < -nextPageThreshold)
				{
					// Next item
					++index;
				}
			}
		}
		return index;
	}

	private Vector3 GetTargetIndexPoint(int index)
	{
		Vector4 panelRegion = mPanel.baseClipRegion;
		Vector2 softness = basedOnSoftness ? mPanel.clipSoftness : Vector2.zero;
		Vector3 panelSize = new Vector3(panelRegion.z - softness.x - softness.x, -panelRegion.w - softness.y - softness.y, 0);

		float direction = space > 0 ? 1 : -1;
		float offsetRatio = 0;
		Vector3 offset = Vector3.zero;
		switch (alignType)
		{
			case AlignType.AT_Forward:
				if (index == (direction > 0 ? 0 : mTotalCount - 1))
				{
					// This offset is ends-item half with
					offset = -panelSize * 0.5F - direction * (mTrans.localPosition - new Vector3(panelRegion.x, panelRegion.y, 0));
				}
				else
				{
					// This offset is item half with
					offsetRatio = direction * - 0.5F;
				}
				break;
			case AlignType.AT_Backward:
				if (index == (direction < 0 ? 0 : mTotalCount - 1))
				{
					// This offset is ends-item half with
					offset = panelSize * 0.5F + direction * (mTrans.localPosition - new Vector3(panelRegion.x, panelRegion.y, 0));
				}
				else
				{
					// This offset is item half with
					offsetRatio = direction * 0.5F;
				}
				break;
		}
		return (index + offsetRatio) * space * mAxis + offset + mTrans.localPosition;
	}

	private Vector3 AlignRestrict(Vector3 indexPosition)
	{
		if (mAxis == Vector3.right)
		{
			if (alignType == AlignType.AT_Center)
			{
				// Restrict both ends
				return Restrict_HorizontalCenter(indexPosition);
			}
			else if ((alignType == AlignType.AT_Forward && space > 0) ||
				(alignType == AlignType.AT_Backward && space < 0))
			{
				// Restrict last
				return Restrict_HorizontalForward(indexPosition);
			}
			else
			{
				// Restrict first
				return Restrict_HorizontalBackward(indexPosition);
			}
		}
		else if (mAxis == Vector3.down)
		{
			if (alignType == AlignType.AT_Center)
			{
				// Restrict both ends
				return Restrict_VerticalCenter(indexPosition);
			}
			else if ((alignType == AlignType.AT_Forward && space > 0) ||
				(alignType == AlignType.AT_Backward && space < 0))
			{
				// Restrict last
				return Restrict_VerticalForward(indexPosition);
			}
			else
			{
				// Restrict first
				return Restrict_VerticalBackward(indexPosition);
			}
		}
		return -mTrans.localPosition;
	}

	private Vector3 Restrict_HorizontalCenter(Vector3 indexPosition)
	{
		float direction = Mathf.Sign(space);

		//float softness = basedOnSoftness ? mPanel.clipSoftness.x : 0;
		//float panelHalfWith = mPanel.baseClipRegion.z * 0.5F - softness;
		//float lastItemHalfWidth = (mTrans.localPosition.x - mPanel.baseClipRegion.x) - -panelHalfWith * direction;
		//float firstItemHalfWidth = -panelHalfWith * direction - (mTrans.localPosition.x - mPanel.baseClipRegion.x);
		//float itemsWidth = (mTotalCount - 1) * space + lastItemHalfWidth - firstItemHalfWidth;
		//float widthDifference = itemsWidth * direction - (panelHalfWith + panelHalfWith);
		float widthDifference = ((mTotalCount - 1) * space + (mTrans.localPosition.x - mPanel.baseClipRegion.x) * 2) * direction;
		if (widthDifference < 0)
		{
			indexPosition.x = (mTotalCount - 1) * 0.5F * space + mTrans.localPosition.x - mPanel.baseClipRegion.x;
			return indexPosition;
		}

		//float firstRestrictPos = firstItemHalfWidth + mTrans.localPosition.x - -panelHalfWith * direction;
		float firstRestrictPos = mPanel.baseClipRegion.x;
		if (indexPosition.x * direction < firstRestrictPos* direction)
		{
			indexPosition.x = firstRestrictPos;
			return indexPosition;
		}

		//float lastRestrictPos = (mTotalCount - 1) * space + lastItemHalfWidth + mTrans.localPosition.x - panelHalfWith * direction;
		float lastRestrictPos = (mTotalCount - 1) * space + mTrans.localPosition.x * 2 - mPanel.baseClipRegion.x;
		if (indexPosition.x * direction > lastRestrictPos * direction)
		{
			indexPosition.x = lastRestrictPos;
			return indexPosition;
		}

		return indexPosition;
	}

	private Vector3 Restrict_HorizontalForward(Vector3 indexPosition)
	{
		float direction = Mathf.Sign(space);
		float softness = basedOnSoftness ? mPanel.clipSoftness.x : 0;
		float panelHalfWith = mPanel.baseClipRegion.z * 0.5F - softness;
		//float lastItemHalfWidth = (mTrans.localPosition.x - mPanel.baseClipRegion.x) - -panelHalfWith * direction;
		//float lastRestrictPos = (mTotalCount - 1) * space + lastItemHalfWidth + mTrans.localPosition.x - (panelHalfWith + panelHalfWith) * direction;
		float lastRestrictPos = (mTotalCount - 1) * space + mTrans.localPosition.x * 2 - mPanel.baseClipRegion.x - panelHalfWith * direction;
		//float firstItemHalfWidth = -panelHalfWith * direction - (mTrans.localPosition.x - mPanel.baseClipRegion.x);
		//float firstRestrictPos = firstItemHalfWidth + mTrans.localPosition.x;
		float firstRestrictPos = mPanel.baseClipRegion.x - panelHalfWith * direction;
		float temp = Mathf.Min(indexPosition.x * direction, lastRestrictPos * direction);
		indexPosition.x = Mathf.Max(temp, firstRestrictPos * direction) * direction;
		return indexPosition;
	}

	private Vector3 Restrict_HorizontalBackward(Vector3 indexPosition)
	{
		float direction = Mathf.Sign(space);
		float softness = basedOnSoftness ? mPanel.clipSoftness.x : 0;
		float panelHalfWith = mPanel.baseClipRegion.z * 0.5F - softness;
		//float firstItemHalfWidth = -panelHalfWith * direction - (mTrans.localPosition.x - mPanel.baseClipRegion.x);
		//float firstRestrictPos = firstItemHalfWidth + mTrans.localPosition.x - -(panelHalfWith + panelHalfWith) * direction;
		float firstRestrictPos = mPanel.baseClipRegion.x + panelHalfWith * direction;
		//float lastItemHalfWidth = (mTrans.localPosition.x - mPanel.baseClipRegion.x) - -panelHalfWith * direction;
		//float lastRestrictPos = (mTotalCount - 1) * space + lastItemHalfWidth + mTrans.localPosition.x;
		float lastRestrictPos = (mTotalCount - 1) * space + mTrans.localPosition.x * 2 - mPanel.baseClipRegion.x + panelHalfWith * direction;
		float temp = Mathf.Max(indexPosition.x * direction, firstRestrictPos * direction);
		indexPosition.x = Mathf.Min(temp, lastRestrictPos * direction) * direction;
		return indexPosition;
	}

	private Vector3 Restrict_VerticalCenter(Vector3 indexPosition)
	{
		float direction = Mathf.Sign(-space);

		//float softness = basedOnSoftness ? mPanel.clipSoftness.y : 0;
		//float panelHalfHeight = mPanel.baseClipRegion.w * 0.5F - softness;
		//float lastItemHalfHeight = (mTrans.localPosition.y - mPanel.baseClipRegion.y) - -panelHalfHeight * direction;
		//float firstItemHalfHeight = -panelHalfHeight * direction - (mTrans.localPosition.y - mPanel.baseClipRegion.y);
		//float itemsHeight = (mTotalCount - 1) * -space + lastItemHalfHeight - firstItemHalfHeight;
		//float heightDifference = itemsHeight * direction - mPanel.baseClipRegion.w;
		float heightDifference = ((mTotalCount - 1) * -space + (mTrans.localPosition.y - mPanel.baseClipRegion.y) * 2) * direction;
		if (heightDifference < 0)
		{
			indexPosition.y = (mTotalCount - 1) * 0.5F * -space + mTrans.localPosition.y;
			return indexPosition;
		}

		//float firstRestrictPos = firstItemHalfHeight + mTrans.localPosition.y - -panelHalfHeight * direction;
		float firstRestrictPos = mPanel.baseClipRegion.y;
		if (indexPosition.y * direction < firstRestrictPos * direction)
		{
			indexPosition.y = firstRestrictPos;
			return indexPosition;
		}

		//float lastRestrictPos = (mTotalCount - 1) * -space + lastItemHalfHeight + mTrans.localPosition.y - panelHalfHeight * direction;
		float lastRestrictPos = (mTotalCount - 1) * -space + mTrans.localPosition.y * 2 - mPanel.baseClipRegion.y;
		if (indexPosition.y * direction > lastRestrictPos * direction)
		{
			indexPosition.y = lastRestrictPos;
			return indexPosition;
		}

		return indexPosition;
	}

	private Vector3 Restrict_VerticalForward(Vector3 indexPosition)
	{
		float direction = Mathf.Sign(-space);
		float softness = basedOnSoftness ? mPanel.clipSoftness.y : 0;
		float panelHalfHeight = mPanel.baseClipRegion.w * 0.5F - softness;
		//float lastItemHalfHeight = (mTrans.localPosition.y - mPanel.baseClipRegion.y) - -panelHalfHeight * direction ;
		//float lastRestrictPos = (mTotalCount - 1) * space + lastItemHalfHeight + mTrans.localPosition.y - (panelHalfHeight + panelHalfHeight) * direction;
		float lastRestrictPos = (mTotalCount - 1) * -space + mTrans.localPosition.y * 2 - mPanel.baseClipRegion.y - panelHalfHeight * direction;
		//float firstItemHalfHeight = -panelHalfHeight * direction - (mTrans.localPosition.y - mPanel.baseClipRegion.y);
		//float firstRestrictPos = firstItemHalfHeight + mTrans.localPosition.y;
		float firstRestrictPos = mPanel.baseClipRegion.y - panelHalfHeight * direction;
		float temp = Mathf.Min(indexPosition.y * direction, lastRestrictPos * direction);
		indexPosition.y = Mathf.Max(temp, firstRestrictPos * direction) * direction;
		return indexPosition;
	}

	private Vector3 Restrict_VerticalBackward(Vector3 indexPosition)
	{
		float direction = Mathf.Sign(-space);
		float softness = basedOnSoftness ? mPanel.clipSoftness.y : 0;
		float panelHalfHeight = mPanel.baseClipRegion.w * 0.5F - softness;
		//float firstItemHalfHeight = -panelHalfHeight * direction - (mTrans.localPosition.y - mPanel.baseClipRegion.y);
		//float firstRestrictPos = firstItemHalfHeight + mTrans.localPosition.y - -(panelHalfHeight + panelHalfHeight) * direction;
		float firstRestrictPos = mPanel.baseClipRegion.y + panelHalfHeight * direction;
		//float lastItemHalfHeight = (mTrans.localPosition.y - mPanel.baseClipRegion.y) - -panelHalfHeight * direction;
		//float lastRestrictPos = (mTotalCount - 1) * -space + lastItemHalfHeight + mTrans.localPosition.y;
		float lastRestrictPos = (mTotalCount - 1) * -space + mTrans.localPosition.y * 2 - mPanel.baseClipRegion.y + panelHalfHeight * direction;
		float temp = Mathf.Max(indexPosition.y * direction, firstRestrictPos * direction);
		indexPosition.y = Mathf.Min(temp, lastRestrictPos * direction) * direction;
		return indexPosition;
	}

	private void AlignOn(int index, Vector3 panelAlignPoint, bool immediately = false)
	{
		if (index != int.MinValue && mScrollView != null && mPanel != null)
		{
			index = Mathf.Clamp(index, 0, mTotalCount - 1);
			Vector3 indexPosition = GetTargetIndexPoint(index);

			if (restrictEnable)
			{
				indexPosition = AlignRestrict(indexPosition);
			}

			mAlignedIndex = index;

			// Figure out the difference between the chosen child and the panel's align in local coordinates
			Vector3 localOffset = indexPosition - panelAlignPoint;

			// Offset shouldn't occur if blocked
			if (!mScrollView.canMoveHorizontally)
				localOffset.x = 0f;
			if (!mScrollView.canMoveVertically)
				localOffset.y = 0f;
			localOffset.z = 0f;

			// Spring the panel to this calculated position
			if (immediately || !Application.isPlaying)
			{
				mPanel.cachedTransform.localPosition -= localOffset;

				Vector4 co = mPanel.clipOffset;
				co.x += localOffset.x;
				co.y += localOffset.y;
				mPanel.clipOffset = co;
			}
			else
			{
				SpringPanel.Begin(mPanel.cachedGameObject,
					mPanel.cachedTransform.localPosition - localOffset, springStrength).onFinished = onFinished;
			}
		}
		else
			mAlignedIndex = int.MinValue;

		// Notify the listener
		if (onAlign != null)
			onAlign(mAlignedIndex);
	}

	/// <summary>
	/// Stop align the panel.
	/// </summary>
	public void StopAlign(bool immediately = false)
	{
		int index = int.MinValue;
		AlignOn(index, immediately);
	}

	/// <summary>
	/// Align the panel on the specified index.
	/// </summary>
	public void AlignOn(int index, bool immediately = false)
	{
		if (mScrollView != null && mPanel != null)
		{
			if (index != int.MinValue && alignEnable)
			{
				Vector3 panelAlignPoint = CalculatePanelAlignPoint();
				AlignOn(index, panelAlignPoint, immediately);
			}
			else
			{
				mAlignedIndex = int.MinValue;
				SpringPanel sp = mPanel.GetComponent<SpringPanel>();
				if (sp) sp.enabled = false;
			}
		}
	}
}

