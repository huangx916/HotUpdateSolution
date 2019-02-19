#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
	public partial class LuaInjectionData
	{
		[ContextMenu("ResetToBeginning")]
		private void ResetToBeginning()
		{
			Fade("fadeOutTweenerList", tweener => UIAgent.SetTweenValue(tweener, 0), false);
			Fade("fadeInTweenerList", tweener => UIAgent.SetTweenValue(tweener, 0), false);
		}

		[ContextMenu("FadeIn")]
		private void FadeIn()
		{
			Fade("fadeOutTweenerList", tweener => UIAgent.SetTweenValue(tweener, 0), false);
			Fade("fadeInTweenerList", tweener => UIAgent.SetTweenValue(tweener, 0), false);
			Fade("fadeInTweenerList", tweener => UIAgent.PlayForward(tweener, true), true);
		}

		[ContextMenu("FadeOut")]
		private void FadeOut()
		{
			Fade("fadeOutTweenerList", tweener => UIAgent.SetTweenValue(tweener, 0), false);
			Fade("fadeInTweenerList", tweener => UIAgent.SetTweenValue(tweener, 1), false);
			Fade("fadeOutTweenerList", tweener => UIAgent.PlayForward(tweener, true), true);
		}

		private void Fade(string tweenerListName, Action<UITweener> action, bool delay)
		{
			if (action == null)
			{
				return;
			}

			foreach (Injection4 injection in m_InjectionList)
			{
				if (injection.Name == tweenerListName)
				{
					List<Injection3> tweenerList = injection.Value as List<Injection3>;
					if (delay && action != null && tweenerList.Count > 0)
					{
						CoroutineManager.EndOfLag(() => EachTweener(tweenerList, action), this);
					}
					else
					{
						EachTweener(tweenerList, action);
					}
					break;
				}
			}
		}

		private static void EachTweener(List<Injection3> tweenerList, Action<UITweener> action)
		{
			if (tweenerList != null)
			{
				foreach (Injection3 injection3 in tweenerList)
				{
					UITweener tweener = injection3.Value as UITweener;
					if (tweener)
					{
						action(tweener);
					}
				}
			}
		}
	}
}
#endif