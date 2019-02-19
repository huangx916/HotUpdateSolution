using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class UIShortScroll : MonoBehaviour {

	public UIRect itemPrefab;
	public float space = 640;
	public int itemRadius = 1;
	public float prevRefresh = 0.1F;
	public float momentumOffset = 0;
	public UISlider progressBar;
	public UILabel itemIndexLabel;
	public bool hideBarWhenStop;

	public float Offset
	{
		get
		{
			float clipOffset = 0;
			if (mScrollView)
			{
				if (mScrollView.canMoveHorizontally)
				{
					clipOffset = mPanel.clipOffset.x;
				}
				else if (mScrollView.canMoveVertically)
				{
					clipOffset = -mPanel.clipOffset.y;
				}
			}
			return clipOffset;
		}
	}

	public int CurrentIndex
	{
		get
		{
			return Mathf.RoundToInt(Offset / space);
		}
	}

	public Transform CurrentTrans
	{
		get
		{
			if (mItems != null)
			{
				int shortIndex = (CurrentIndex + mShortCount) % mShortCount;
				return mItems[shortIndex];
			}
			else
			{
				return null;
			}
		}
	}

	/// <summary>
	/// Transform itemTrans, int itemIndex
	/// </summary>
	public Action<Transform, int> onUpdateItem;

	private bool mInited;
	private bool mRunning;

	private Transform mTrans;
	private UIScrollView mScrollView;
	private UIPanel mPanel;
	private Transform[] mItems;

	private Vector3 mAxis;
	private Vector3 mCenter;
	private int mShortCount;
	private int mCenterIndex;
	private int mTotalCount;
	private float mSize;

	private IEnumerator mUpdateCoroutine;
	private bool mShowBar;
	private float mBarAlpha = 1;

	void Awake()
	{
		if (!mInited)
		{
			Init();
		}
	}

	void Start()
	{
		MainStart();
		AlignStart();
	}

	private void MainStart()
	{
		if (mInited)
		{
			return;
		}
		foreach (Transform itemTrans in mItems)
		{
			foreach (UIWidget widget in itemTrans.GetComponentsInChildren<UIWidget>())
			{
				widget.ParentHasChanged();
			}
		}
	}

	void LateUpdate()
	{
		if (progressBar)
		{
			if (!Application.isPlaying)
				return;

			if (hideBarWhenStop && mBarAlpha != 0)
			{
				float alpha = progressBar.alpha / mBarAlpha;
				if (mShowBar)
				{
					alpha += RealTime.deltaTime * 6F;
					mShowBar = false;
				}
				else
				{
					alpha -= RealTime.deltaTime * 3F;
				}
				alpha = Mathf.Clamp01(alpha) * mBarAlpha;
				if (progressBar.alpha != alpha)
				{
					progressBar.alpha = alpha;
				}
			}
			else if (progressBar.alpha != mBarAlpha)
			{
				progressBar.alpha = mBarAlpha;
			}
		}
	}

	private void Init()
	{
		if (mInited = FindScrollView())
		{
			mSize = 0;
			if (mAxis == Vector3.right)
			{
				mSize = mPanel.width;
			}
			else if (mAxis == Vector3.down)
			{
				mSize = mPanel.height;
			}
			int radius = Mathf.RoundToInt(mSize * 0.5F / Mathf.Abs(space) + 0.5F);
			itemRadius = Mathf.Max(itemRadius, radius);

			if (itemPrefab && mItems == null)
			{
				mShortCount = (itemRadius << 1) + 1;
				mItems = new Transform[mShortCount];
				for (int index = 0, layer = mPanel.gameObject.layer; index < mShortCount; ++index)
				{
					mItems[index] = Instantiate(itemPrefab.transform) as Transform;
					mItems[index].parent = transform;
					mItems[index].localPosition = Vector3.zero;
					mItems[index].localRotation = Quaternion.identity;
					mItems[index].localScale = Vector3.one;
					mItems[index].GetComponent<UIRect>().alpha = 1;
					mItems[index].name = index.ToString();
					foreach (Transform trans in mItems[index].GetComponentsInChildren<Transform>())
					{
						trans.gameObject.layer = layer;
					}
					mItems[index].gameObject.SetActive(false);
				}
				itemPrefab.alpha = 0;
			}
		}
	}

	private string GetPath(Transform target)
	{
		string path = null;
		while (target != null)
		{
			if (string.IsNullOrEmpty(path))
			{
				path = target.name;
			}
			else
			{
				path = target.name + "/" + path;
			}
			target = target.parent;
		}
		return path;
	}

	private bool FindScrollView()
	{
		if (mScrollView == null)
		{
			mTrans = transform;

			mScrollView = NGUITools.FindInParents<UIScrollView>(gameObject);
			if (mScrollView == null)
			{
				Debug.LogWarning(GetType() + " requires " + typeof(UIScrollView) + " on a parent object in order to work", this);
				return false;
			}
			else
			{
				if (mScrollView.canMoveHorizontally)
				{
					mAxis = Vector3.right;
				}
				else if (mScrollView.canMoveVertically)
				{
					mAxis = Vector3.down;
				}

				mPanel = mScrollView.GetComponent<UIPanel>();

				Transform trans = mTrans;
				Transform scrollTrans = mPanel.cachedTransform;
				while ((trans = trans.parent) != scrollTrans)
				{
					trans.localPosition = Vector3.zero;
					trans.localRotation = Quaternion.identity;
					trans.localScale = Vector3.one;
				}
				mTrans.localRotation = Quaternion.identity;
				mTrans.localScale = Vector3.one;
			}
		}
		return true;
	}

	public void InitItems(Action<Transform> callback)
	{
		if (callback != null)
		{
			if (!mInited)
			{
				Init();
			}
			for (int index = 0; index < mShortCount; ++index)
			{
				callback(mItems[index]);
			}
		}
	}

	public void StartScroll(int count, int targetIndex = 0)
	{
		if (!mInited)
		{
			Init();
		}
		if (mInited && mItems != null)
		{
			if (mRunning)
			{
				StopScroll();
			}
			UpdateScroll(Mathf.Max(count, 0), targetIndex);
		}
	}

	public void StopScroll(Action<Transform> dispose = null, float delayUnload = 0)
	{
		if (mInited)
		{
			if (mRunning)
			{
				mPanel.onClipMove -= OnMoved;
				mRunning = false;
			}
			if (dispose != null)
			{
				for (int pageIndex = 0, pageCount = mItems.Length; pageIndex < pageCount; pageIndex++)
				{
					dispose(mItems[pageIndex]);
				}
			}
			StartCoroutine(DelayOperation(delayUnload, () => Resources.UnloadUnusedAssets()));
		}
	}

	public Transform GetItem(int index)
	{
		if (index < 0)
		{
			index = index % mShortCount + mShortCount;
		}
		return mItems[index % mShortCount];
	}

	public float RemoveAt(int removeIndex, Action<Transform> callback = null)
	{
		if (!mRunning)
		{
			return 0;
		}

		if (removeIndex < 0 || removeIndex >= mTotalCount)
		{
			return 0;
		}

		float delay = 0;

		int firstIndex = mCenterIndex - itemRadius;
		int lastIndex = mCenterIndex + itemRadius;
		if (lastIndex < mTotalCount - 1)
		{
			if (removeIndex < firstIndex)
			{
				for (int shortIndex = 0; shortIndex < mShortCount; shortIndex++)
				{
					mItems[shortIndex].localPosition -= space * mAxis;
				}
				mCenterIndex--;

				ResetViewport(-1, 0);
			}
			else if (removeIndex <= lastIndex)
			{
				int onView = IsOnView(removeIndex);
				if (onView == 0)
				{
					delay = RemoveReposition(removeIndex, lastIndex, callback, 0);
				}
				else
				{
					RemoveReposition(removeIndex, lastIndex, 0);

					if (onView < 0)
					{
						ResetViewport(-1, 0);
					}
				}
			}
		}
		else if (firstIndex > 0)
		{
			if (removeIndex < firstIndex)
			{
				for (int shortIndex = 0; shortIndex < mShortCount; shortIndex++)
				{
					mItems[shortIndex].localPosition -= space * mAxis;
				}
				mCenterIndex--;

				ResetViewport(-1, 1);
			}
			else
			{
				int onView = IsOnView(removeIndex);
				if (onView == 0)
				{
					delay = RemoveReposition(removeIndex, lastIndex, callback, 1);
				}
				else
				{
					RemoveReposition(removeIndex, lastIndex, 1);

					if (onView < 0)
					{
						ResetViewport(-1, 0);
					}
				}
			}
		}
		else
		{
			int onView = IsOnView(removeIndex);
			if (onView == 0)
			{
				delay = RemoveReposition(removeIndex, lastIndex, callback, 2);
			}
			else
			{
				RemoveReposition(removeIndex, lastIndex, 2);
			}
		}
		mTotalCount--;

		StartCoroutine(DelayOperation(delay, () => {
				mScrollView.InvalidateBounds();
				mScrollView.onDragFinished();
			}
		));

		if (progressBar)
		{
			progressBar.alpha = mTotalCount - 1;
		}

		return delay;
	}

	public float InsertAt(int insertIndex, Action<Transform> callback = null)
	{
		if (!mRunning)
		{
			return 0;
		}

		insertIndex = Mathf.Clamp(insertIndex, 0, mTotalCount);

		float delay = 0;
		int firstIndex = mCenterIndex - itemRadius;
		int lastIndex = mCenterIndex + itemRadius;
		if (insertIndex < firstIndex)
		{
			for (int shortIndex = 0; shortIndex < mShortCount; shortIndex++)
			{
				mItems[shortIndex].localPosition += space * mAxis;
			}
			mCenterIndex++;

			ResetViewport(1, 0);
		}
		else if (insertIndex <= lastIndex)
		{
			int onView = IsOnView(insertIndex);
			if (onView == 0)
			{
				if (callback != null)
				{
					Transform trans = Instantiate(itemPrefab.transform) as Transform;
					trans.parent = transform;
					trans.localPosition = insertIndex * space * mAxis;
					trans.localRotation = Quaternion.identity;
					trans.localScale = Vector3.one;
					trans.GetComponent<UIRect>().alpha = 1;
					callback(trans);
				}

				Transform insertTrans = mItems[lastIndex % mShortCount];
				for (int index = lastIndex - 1; index >= insertIndex; --index)
				{
					int shortIndex = index % mShortCount;
					int prevShortIndex = (index + 1) % mShortCount;
					Transform trans = mItems[shortIndex];
					mItems[prevShortIndex] = trans;
					TweenPosition tp = trans.GetComponent<TweenPosition>();
					if (!tp)
					{
						tp = trans.gameObject.AddComponent<TweenPosition>();
						tp.duration = 0.5F;
					}
					tp.SetStartToCurrentValue();
					tp.to = (index - 1) * space * mAxis;
					tp.ResetToBeginning();
					tp.PlayForward();
				}
				mItems[insertIndex % mShortCount] = insertTrans;
				TweenPosition insertTp = insertTrans.GetComponent<TweenPosition>();
				if (!insertTp)
				{
					insertTp = insertTrans.gameObject.AddComponent<TweenPosition>();
					insertTp.duration = 0.5F;
				}
				insertTp.SetStartToCurrentValue();
				insertTp.to = (lastIndex + 1) * space * mAxis;
				insertTp.ResetToBeginning();
				insertTp.PlayForward();
				delay = insertTp.duration;
				StartCoroutine(DelayOperation(insertTp.duration, () => {
						if (onUpdateItem != null)
						{
							onUpdateItem(insertTrans, insertIndex);
						}
						insertTrans.localPosition = insertIndex * space * mAxis;
					}
				));
			}
			else
			{
				Transform insertTrans = mItems[lastIndex % mShortCount];
				for (int index = lastIndex - 1; index >= insertIndex; --index)
				{
					int shortIndex = index % mShortCount;
					mItems[shortIndex].localPosition += space * mAxis;
					int nextShortIndex = (index + 1) % mShortCount;
					mItems[nextShortIndex] = mItems[shortIndex];
				}

				mItems[insertIndex % mShortCount] = insertTrans;
				insertTrans.localPosition = insertIndex * space * mAxis;
				insertTrans.gameObject.SetActive(true);
				if (onUpdateItem != null)
				{
					onUpdateItem(insertTrans, insertIndex);
				}

				if (onView < 0)
				{
					ResetViewport(1, 0);
				}
			}
		}
		mTotalCount++;

		StartCoroutine(DelayOperation(delay, () => {
				mScrollView.InvalidateBounds();
				mScrollView.onDragFinished();
			}
		));

		if (progressBar)
		{
			progressBar.alpha = mTotalCount - 1;
		}

		return delay;
	}

	public void UpdateCount(int count)
	{
		if (!mRunning)
		{
			return;
		}

		count = Mathf.Max(count, 0);

		List<int> updatedList = new List<int>();
		int shortCount = (itemRadius << 1) + 1;
		if (count < mTotalCount)
		{
			int maxIndex = count - 1 - itemRadius;
			int minIndex = itemRadius;
			mCenterIndex = Mathf.Min(mCenterIndex, maxIndex);
			mCenterIndex = Mathf.Max(mCenterIndex, minIndex);

			for (int index = -itemRadius; index <= itemRadius; ++index)
			{
				int pageIndex = mCenterIndex + index;
				if (pageIndex >= count)
				{
					int _shortIndex = (pageIndex + mShortCount) % mShortCount;
					mItems[_shortIndex].gameObject.SetActive(false);
					mItems[_shortIndex].localPosition = pageIndex * space * mAxis;
				}
				else
				{
					int shortIndex = pageIndex % mShortCount;
					mItems[shortIndex].gameObject.SetActive(true);
					mItems[shortIndex].localPosition = pageIndex * space * mAxis;
					if (onUpdateItem != null)
					{
						onUpdateItem(mItems[shortIndex], pageIndex);
					}
				}
			}
			mTotalCount = count;
		}
		else if (count > mTotalCount)
		{
			for (int itemIndex = mTotalCount, maxIndex = Mathf.Min(mCenterIndex + itemRadius, count - 1); itemIndex <= maxIndex; itemIndex++)
			{
				mItems[(itemIndex + shortCount) % shortCount].gameObject.SetActive(true);
			}
			mTotalCount = count;
		}

		for (int index = -itemRadius; index <= itemRadius; ++index)
		{
			int pageIndex = mCenterIndex + index;
			if (pageIndex < 0)
			{
				pageIndex += shortCount;
			}
			else if (pageIndex >= mTotalCount)
			{
				pageIndex -= shortCount;
			}
			if (pageIndex < 0 || pageIndex >= count)
			{
				mItems[(pageIndex + shortCount) % shortCount].gameObject.SetActive(false);
				continue;
			}
			int shortIndex = pageIndex % shortCount;
			if (!updatedList.Contains(shortIndex))
			{
				mItems[shortIndex].gameObject.SetActive(true);
				if (onUpdateItem != null)
				{
					onUpdateItem(mItems[shortIndex], pageIndex);
				}
			}
		}

		if (count <= 0)
		{
			Vector2 clipOffset = Vector2.zero;
			Vector3 localOffset = clipOffset - mPanel.clipOffset;
			mPanel.cachedTransform.localPosition -= localOffset;
			mPanel.clipOffset = Vector3.zero;

			UpdatePagination(0, count);
		}

		mScrollView.InvalidateBounds();
		mScrollView.onDragFinished();

		mBarAlpha = count > 1 ? 1 : 0;
	}

	public void UpdateItem(int itemIdex)
	{
		if (!mRunning)
		{
			return;
		}
		if (itemIdex < 0 || itemIdex >= mTotalCount)
		{
			return;
		}
		if (Mathf.Abs(itemIdex - mCenterIndex) > itemRadius)
		{
			return;
		}
		int shortIndex = itemIdex % mShortCount;
		mItems[shortIndex].gameObject.SetActive(true);
		onUpdateItem(mItems[shortIndex], itemIdex);
	}

	public void UpdateItems(List<int> updateList = null, List<int> exceptList = null)
	{
		if (!mRunning)
		{
			return;
		}
		for (int index = -itemRadius; index <= itemRadius; ++index)
		{
			int itemIndex = mCenterIndex + index;
			if (itemIndex < 0)
			{
				itemIndex += mShortCount;
			}
			else if (itemIndex >= mTotalCount)
			{
				itemIndex -= mShortCount;
			}
			if (itemIndex < 0 || itemIndex >= mTotalCount)
			{
				mItems[(itemIndex + mShortCount) % mShortCount].gameObject.SetActive(false);
				continue;
			}
			int shortIndex = itemIndex % mShortCount;

			if ((updateList == null || updateList.Contains(shortIndex)) && (exceptList == null || !exceptList.Contains(shortIndex)))
			{
				mItems[shortIndex].gameObject.SetActive(true);
				if (onUpdateItem != null)
				{
					onUpdateItem(mItems[shortIndex], itemIndex);
				}
			}
		}
	}

	public int Move(int offset)
	{
		if (!mRunning)
		{
			return 0;
		}

		if (offset == 0)
		{
			return CurrentIndex;
		}

		int targetIndex = CurrentIndex + offset;
		if (offset > 0)
		{
			for (int index = 0, length = targetIndex - mCenterIndex; index < length; ++index)
			{
				if (mCenterIndex < mTotalCount - 1 - itemRadius)
				{
					int shortIndex = (mCenterIndex - itemRadius) % mShortCount;
					mItems[shortIndex].localPosition = (mCenterIndex + itemRadius + 1) * space * mAxis;
					if (onUpdateItem != null)
					{
						onUpdateItem(mItems[shortIndex], mCenterIndex + itemRadius + 1);
					}
					mCenterIndex++;
				}
			}
		}
		else
		{
			for (int index = 0, length = mCenterIndex - targetIndex; index < length; ++index)
			{
				if (mCenterIndex > itemRadius)
				{
					int shortIndex = (mCenterIndex + itemRadius) % mShortCount;
					mItems[shortIndex].localPosition = (mCenterIndex - itemRadius - 1) * space * mAxis;
					if (onUpdateItem != null)
					{
						onUpdateItem(mItems[shortIndex], mCenterIndex - itemRadius - 1);
					}
					mCenterIndex--;
				}
			}
		}
		targetIndex = Mathf.Clamp(targetIndex, 0, mTotalCount - 1);
		if (alignEnable)
		{
			AlignOn(targetIndex);
		}
		return targetIndex;
	}

	private void UpdateScroll(int count, int targetIndex = 0)
	{
		mTotalCount = count;
		mBarAlpha = count > 1 ? 1 : 0;

		if (targetIndex == int.MinValue)
		{
			targetIndex = CurrentIndex;
		}
		else
		{
			targetIndex = Mathf.Min(targetIndex, count - 1);
			targetIndex = Mathf.Max(targetIndex, 0);

			int sign = space < 0 ? -1 : 1;
			float offset = Math.Abs(space) * targetIndex;
			offset = Mathf.Min(offset, Math.Abs(space) * count - mSize);
			offset = Mathf.Max(offset, 0);
			offset *= sign;
			Vector2 clipOffset = offset * mAxis;
			Vector3 localOffset = clipOffset - mPanel.clipOffset;
			mPanel.cachedTransform.localPosition -= localOffset;
			mPanel.clipOffset = offset * mAxis;

			if (alignEnable)
			{
				AlignOn(targetIndex);
			}

			UpdatePagination(offset, count);
		}

		int maxIndex = count - 1 - itemRadius;
		int minIndex = itemRadius;
		mCenterIndex = targetIndex;
		mCenterIndex = Mathf.Min(mCenterIndex, maxIndex);
		mCenterIndex = Mathf.Max(mCenterIndex, minIndex);

		for (int index = -itemRadius; index <= itemRadius; ++index)
		{
			int pageIndex = mCenterIndex + index;
			// pageIndex must be >0
			if (pageIndex >= count)
			{
				// pageIndex must be <mShortCount
				int _shortIndex = (pageIndex + mShortCount) % mShortCount;
				mItems[_shortIndex].gameObject.SetActive(false);
				mItems[_shortIndex].localPosition = pageIndex * space * mAxis;
			}
			else
			{
				int shortIndex = pageIndex % mShortCount;
				mItems[shortIndex].gameObject.SetActive(true);
				mItems[shortIndex].localPosition = pageIndex * space * mAxis;
				if (onUpdateItem != null)
				{
					onUpdateItem(mItems[shortIndex], pageIndex);
				}
			}
		}
		SortItems();

		mPanel.onClipMove += OnMoved;
		OnMoved(mPanel);

		mRunning = true;
	}

	private void OnMoved(UIPanel panel)
	{
		if (enabled)
		{
			Vector3 momentum = mScrollView.currentMomentum * mScrollView.momentumAmount;
			Vector3 moveDelta = NGUIMath.SpringDampen(ref momentum, 9f, 2f);
			float offset = 0;
			float delta = 0;
			int sign = space < 0 ? -1 : 1;
			if (mAxis == Vector3.right)
			{
				offset = mPanel.clipOffset.x;
				delta = (moveDelta * momentumOffset).x;
			}
			else if (mAxis == Vector3.down)
			{
				offset = -mPanel.clipOffset.y;
				delta = (moveDelta * momentumOffset).y;
			}

			int minIndex = mCenterIndex - itemRadius;
			int maxIndex = mCenterIndex + itemRadius;
			if (offset * sign + mSize + delta > (maxIndex + 1 - prevRefresh) * space * sign && mCenterIndex < mTotalCount - 1 - itemRadius)
			{
				while (offset * sign + mSize + delta > (maxIndex + 1 - prevRefresh) * space * sign && mCenterIndex < mTotalCount - 1 - itemRadius)
				{
					int shortIndex = (minIndex) % mShortCount;
					mItems[shortIndex].localPosition = (maxIndex + 1) * space * mAxis;
					if (onUpdateItem != null)
					{
						onUpdateItem(mItems[shortIndex], maxIndex + 1);
					}
					++mCenterIndex;
					++minIndex;
					++maxIndex;
				}
				SortItems();
				mScrollView.InvalidateBounds();
				mScrollView.RestrictWithinBounds(true);
			}
			else if (offset * sign + delta < (minIndex + prevRefresh) * space * sign && mCenterIndex > itemRadius)
			{
				while (offset * sign + delta < (minIndex + prevRefresh) * space * sign && mCenterIndex > itemRadius)
				{
					int shortIndex = (maxIndex) % mShortCount;
					mItems[shortIndex].localPosition = (minIndex - 1) * space * mAxis;
					if (onUpdateItem != null)
					{
						onUpdateItem(mItems[shortIndex], minIndex - 1);
					}
					--mCenterIndex;
					--minIndex;
					--maxIndex;
				}
				SortItems();
				mScrollView.InvalidateBounds();
				mScrollView.RestrictWithinBounds(true);
			}

			UpdatePagination(offset, mTotalCount);
		}
	}

	void OnScroll(float delta)
	{
		if (mInited)
		{
			OnMoved(mPanel);
		}
	}

	private void SortItems()
	{
		for (int index = -itemRadius; index <= itemRadius; ++index)
		{
			int shortIndex = (mCenterIndex + index) % mShortCount;
			mItems[shortIndex].SetSiblingIndex(mShortCount);
		}
	}

	private void UpdatePagination(float offset, int pageCount)
	{
		if (progressBar)
		{
			float panelSize = 0;
			if (mAxis == Vector3.right)
			{
				panelSize = mPanel.width;
			}
			else if (mAxis == Vector3.down)
			{
				panelSize = mPanel.height;
			}
			progressBar.value = offset / (pageCount * space - panelSize);
			if (hideBarWhenStop)
			{
				mShowBar = true;
			}
		}
		if (itemIndexLabel)
		{
			int page = Mathf.Clamp((int) (offset / space + 1.5F), 1, pageCount);
			itemIndexLabel.text = page.ToString();
		}
	}

	private float RemoveReposition(int removeIndex, int lastIndex, Action<Transform> callback, int flag)
	{
		float delay = 0;

		Transform removeTrans = mItems[removeIndex % mShortCount];

		if (callback != null)
		{
			Transform trans = Instantiate(removeTrans) as Transform;
			trans.parent = removeTrans.parent;
			trans.localPosition = removeTrans.localPosition;
			trans.localRotation = removeTrans.localRotation;
			trans.localScale = removeTrans.localScale;
			callback(trans);
		}

		for (int index = removeIndex + 1; index <= lastIndex; ++index)
		{
			int shortIndex = index % mShortCount;
			int prevShortIndex = (index - 1) % mShortCount;
			Transform trans = mItems[shortIndex];
			mItems[prevShortIndex] = trans;
			TweenPosition tp = trans.GetComponent<TweenPosition>();
			if (!tp)
			{
				tp = trans.gameObject.AddComponent<TweenPosition>();
				tp.duration = 0.5F;
			}
			tp.SetStartToCurrentValue();
			tp.to = (index - 1) * space * mAxis;
			tp.ResetToBeginning();
			tp.PlayForward();
			delay = tp.duration;
		}
		mItems[lastIndex % mShortCount] = removeTrans;

		if (flag == 0)
		{
			removeTrans.localPosition = (lastIndex + 1) * space * mAxis;
			if (onUpdateItem != null)
			{
				onUpdateItem(removeTrans, lastIndex);
			}
			TweenPosition removeTp = removeTrans.GetComponent<TweenPosition>();
			if (!removeTp)
			{
				removeTp = removeTrans.gameObject.AddComponent<TweenPosition>();
				removeTp.duration = 0.5F;
			}
			removeTp.SetStartToCurrentValue();
			removeTp.to = lastIndex * space * mAxis;
			removeTp.ResetToBeginning();
			removeTp.PlayForward();

			delay = removeTp.duration;
		}
		else if (flag == 1)
		{
			removeTrans.localPosition = (lastIndex - mShortCount) * space * mAxis;
			mCenterIndex--;
			if (onUpdateItem != null)
			{
				onUpdateItem(removeTrans, flag == 0 ? lastIndex : lastIndex - mShortCount);
			}
		}
		else if (flag == 2)
		{
			removeTrans.localPosition = lastIndex * space * mAxis;
			removeTrans.gameObject.SetActive(false);
		}
		return delay;
	}

	private void RemoveReposition(int removeIndex, int lastIndex, int flag)
	{
		Transform removeTrans = mItems[removeIndex % mShortCount];

		for (int index = removeIndex + 1; index <= lastIndex; ++index)
		{
			int shortIndex = index % mShortCount;
			int prevShortIndex = (index - 1) % mShortCount;

			Transform trans = mItems[shortIndex];
			mItems[prevShortIndex] = trans;
			trans.localPosition -= space * mAxis;
		}
		mItems[lastIndex % mShortCount] = removeTrans;
		if (flag == 0)
		{
			removeTrans.localPosition = lastIndex * space * mAxis;
			if (onUpdateItem != null)
			{
				onUpdateItem(removeTrans, lastIndex);
			}
		}
		else if (flag == 1)
		{
			mCenterIndex--;
			removeTrans.localPosition = (lastIndex - mShortCount) * space * mAxis;
			if (onUpdateItem != null)
			{
				onUpdateItem(removeTrans, lastIndex - mShortCount);
			}
		}
		else if (flag == 2)
		{
			removeTrans.localPosition = lastIndex * space * mAxis;
			removeTrans.gameObject.SetActive(false);
		}
	}

	private void InsertReposition(int insertIndex, int lastIndex, int flag)
	{
		Transform insertTrans = mItems[insertIndex % mShortCount];
		for (int index = lastIndex - 1; index >= insertIndex; --index)
		{
			int shortIndex = index % mShortCount;
			mItems[shortIndex].localPosition += space * mAxis;
			int nextShortIndex = (index + 1) % mShortCount;
			mItems[nextShortIndex] = mItems[shortIndex];
		}

		mItems[insertIndex % mShortCount] = insertTrans;
		insertTrans.localPosition = insertIndex * space * mAxis;
		insertTrans.gameObject.SetActive(true);
		if (onUpdateItem != null)
		{
			onUpdateItem(insertTrans, insertIndex);
		}
	}

	private void ResetViewport(int addOrRemove, int flag)
	{
		if (addOrRemove == 0)
		{
			return;
		}
		else
		{
			addOrRemove = addOrRemove > 0 ? 1 : -1;
			int index = flag == 0 ? CurrentIndex : Mathf.Min(CurrentIndex, mTotalCount - 2);
			if (alignEnable)
			{
				mPanel.cachedTransform.localPosition -= addOrRemove * space * mAxis;
				mPanel.clipOffset += addOrRemove * space * new Vector2(mAxis.x, mAxis.y);
				AlignOn(index);
			}
			else if (flag == 1)
			{
				if (addOrRemove < 0 && CurrentIndex == mTotalCount - 1)
				{
					mPanel.cachedTransform.localPosition += space * mAxis;
					mPanel.clipOffset -= space * new Vector2(mAxis.x, mAxis.y);
				}
			}
		}
	}

	private int IsOnView(int index)
	{
		int shortIndex = index % mShortCount;
		Transform trans = mItems[shortIndex];
		Bounds removeBounds = NGUIMath.CalculateRelativeWidgetBounds(mPanel.cachedTransform, trans);
		int onView = 0;
		int dir = space > 0 ? 1 : (space < 0 ? -1 : 0);
		if (mScrollView.canMoveHorizontally)
		{
			float clipOffset = mPanel.clipOffset.x;
			if (removeBounds.min.x > clipOffset + mSize * 0.5F)
			{
				onView = dir;
			}
			else if (removeBounds.max.x < clipOffset - mSize * 0.5F)
			{
				onView = -dir;
			}
		}
		else if (mScrollView.canMoveVertically)
		{
			float clipOffset = -mPanel.clipOffset.y;
			if (-removeBounds.max.y < clipOffset - mSize * 0.5F)
			{
				onView = dir;
			}
			else if (-removeBounds.min.y > clipOffset + mSize * 0.5F)
			{
				onView = -dir;
			}
		}
		return onView;
	}

	private IEnumerator DelayOperation(float delay, EventDelegate.Callback callback)
	{
		if (callback != null)
		{
			if (delay == Mathf.Epsilon)
			{
				yield return new WaitForEndOfFrame();
			}
			else if (delay != 0)
			{
				yield return new WaitForSeconds(delay);
			}
			callback();
		}
	}
}

