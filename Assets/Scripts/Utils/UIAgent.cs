using UnityEngine;
using XLua;

namespace Main
{
	public class UIAgent
	{
		public static void SetChildDepth(Component parentComp, string path, int depth)
		{
			if (IsTargetNotNull(parentComp))
			{
				UIRect rect = CompAgent.FindChild<UIRect>(parentComp, path);
				SetDepth(rect, depth);
			}
		}

		public static void SetChildDepth(GameObject parentGo, string path, int depth)
		{
			SetChildDepth(parentGo.transform, path, depth);
		}

		public static void SetDepth(Component comp, int depth)
		{
			if (IsTargetNotNull(comp))
			{
				UIRect rect = comp.GetComp<UIRect>();
				if (IsTargetNotNull(rect))
				{
					if (rect is UIWidget)
					{
						(rect as UIWidget).depth = depth;
					}
					else if (rect is UIPanel)
					{
						(rect as UIPanel).depth = depth;
					}
					else
					{
						Debugger.LogError("Can't find widget or panel!", rect);
					}
				}
			}
		}

		public static void SetDepth(GameObject go, int depth)
		{
			if (IsTargetNotNull(go))
			{
				SetDepth(go.GetComponent<UIRect>(), depth);
			}
		}

		public static void SetChildAlpha(Component parentComp, string path, bool alpha)
		{
			SetChildAlpha(parentComp, path, alpha ? 1 : 0);
		}

		public static void SetChildAlpha(GameObject parentGo, string path, bool alpha)
		{
			SetChildAlpha(parentGo, path, alpha ? 1 : 0);
		}

		public static void SetAlpha(Component comp, bool alpha)
		{
			SetAlpha(comp, alpha ? 1 : 0);
		}

		public static void SetAlpha(GameObject go, bool alpha)
		{
			SetAlpha(go, alpha ? 1 : 0);
		}

		public static void SetChildAlpha(Component parentComp, string path, float alpha)
		{
			if (IsTargetNotNull(parentComp))
			{
				UIRect rect = CompAgent.FindChild<UIRect>(parentComp, path);
				SetAlpha(rect, alpha);
			}
		}

		public static void SetChildAlpha(GameObject parentGo, string path, float alpha)
		{
			if (IsTargetNotNull(parentGo))
			{
				SetChildAlpha(parentGo.transform, path, alpha);
			}
		}

		public static void SetAlpha(Component comp, float alpha)
		{
			if (IsTargetNotNull(comp))
			{
				UIRect rect = comp.GetComp<UIRect>();
				if (IsTargetNotNull(rect))
				{
					rect.alpha = alpha;
				}
			}
		}

		public static void SetAlpha(GameObject go, float alpha)
		{
			if (IsTargetNotNull(go))
			{
				SetAlpha(go.GetComponent<UIRect>(), alpha);
			}
		}

		public static void SetChildColor(Component parentComp, string path, Color color)
		{
			if (IsTargetNotNull(parentComp))
			{
				Transform child = CompAgent.FindChild(parentComp, path);
				SetColor(child, color);
			}
		}

		public static void SetChildColor(GameObject parentGo, string path, Color color)
		{
			if (IsTargetNotNull(parentGo))
			{
				SetChildColor(parentGo.transform, path, color);
			}
		}

		public static void SetColor(Component comp, Color color)
		{
			if (IsTargetNotNull(comp))
			{
				UIBasicSprite basicSprite = comp.GetComp<UIBasicSprite>();
				if (basicSprite)
				{
					basicSprite.color = color;
					return;
				}
				UILabel label = comp.GetComp<UILabel>();
				if (label)
				{
					label.color = color;
					return;
				}
				Debugger.LogError("Can't find sprite or label!", comp);
			}
		}

		public static void SetColor(GameObject go, Color color)
		{
			if (IsTargetNotNull(go))
			{
				SetColor(go.transform, color);
			}
		}

		public static void SetChildEffectColor(Component parentComp, string path, Color color)
		{
			if (IsTargetNotNull(parentComp))
			{
				UILabel label = CompAgent.FindChild<UILabel>(parentComp, path);
				SetEffectColor(label, color);
			}
		}

		public static void SetChildEffectColor(GameObject parentGo, string path, Color color)
		{
			if (IsTargetNotNull(parentGo))
			{
				SetChildEffectColor(parentGo.transform, path, color);
			}
		}

		public static void SetEffectColor(Component comp, Color color)
		{
			if (IsTargetNotNull(comp))
			{
				UILabel label = comp.GetComp<UILabel>();
				if (IsTargetNotNull(label))
				{
					label.effectColor = color;
				}
			}
		}

		public static void SetEffectColor(GameObject go, Color color)
		{
			if (IsTargetNotNull(go))
			{
				UILabel label = go.GetComponent<UILabel>();
				SetEffectColor(label, color);
			}
		}

		public static void SetChildText(Component parentComp, string path, object text)
		{
			SetChildText(parentComp, path, text == null ? null : text.ToString());
		}

		public static void SetChildText(GameObject parentGo, string path, object text)
		{
			SetChildText(parentGo, path, text == null ? null : text.ToString());
		}

		public static void SetText(Component comp, object text)
		{
			SetText(comp, text == null ? null : text.ToString());
		}

		public static void SetText(GameObject go, object text)
		{
			SetText(go, text == null ? null : text.ToString());
		}

		public static void SetChildText(Component parentComp, string path, string text)
		{
			if (IsTargetNotNull(parentComp))
			{
				UILabel label = CompAgent.FindChild<UILabel>(parentComp, path);
				SetText(label, text);
			}
		}

		public static void SetChildText(GameObject parentGo, string path, string text)
		{
			if (IsTargetNotNull(parentGo))
			{
				SetChildText(parentGo.transform, path, text);
			}
		}

		public static void SetText(Component comp, string text)
		{
			if (IsTargetNotNull(comp))
			{
				UILabel label = comp.GetComp<UILabel>();
				if (IsTargetNotNull(label))
				{
					label.text = text;
				}
			}
		}

		public static void SetText(GameObject go, string text)
		{
			if (IsTargetNotNull(go))
			{
				SetText(go.GetComponent<UILabel>(), text);
			}
		}

		public static void SetChildFont(GameObject parentGo, string path, UIFont font)
		{
			if (IsTargetNotNull(parentGo))
			{
				SetChildFont(parentGo.transform, path, font);
			}
		}

		public static void SetChildFont(Component parentComp, string path, UIFont font)
		{
			if (IsTargetNotNull(parentComp))
			{
				UILabel label = CompAgent.FindChild<UILabel>(parentComp, path);
				SetFont(label, font);
			}
		}

		public static void SetFont(GameObject go, UIFont font)
		{
			if (IsTargetNotNull(go))
			{
				UILabel label = go.GetComponent<UILabel>();
				SetFont(label, font);
			}
		}

		public static void SetFont(Component comp, UIFont font)
		{
			if (IsTargetNotNull(comp))
			{
				UILabel label = comp.GetComp<UILabel>();
				if (IsTargetNotNull(label))
				{
					label.bitmapFont = font;
				}
			}
		}

		public static void SetChildSprite(GameObject parentGo, string path, string spriteName)
		{
			if (IsTargetNotNull(parentGo))
			{
				SetChildSprite(parentGo.transform, path, spriteName);
			}
		}

		public static void SetChildSprite(Component parentComp, string path, string spriteName)
		{
			if (IsTargetNotNull(parentComp))
			{
				UISprite sprite = CompAgent.FindChild<UISprite>(parentComp, path);
				SetSprite(sprite, spriteName);
			}
		}

		public static void SetSprite(GameObject go, string spriteName)
		{
			if (IsTargetNotNull(go))
			{
				UISprite sprite = go.GetComponent<UISprite>();
				SetSprite(sprite, spriteName);
			}
		}

		public static void SetSprite(Component comp, string spriteName)
		{
			if (IsTargetNotNull(comp))
			{
				UISprite sprite = comp.GetComp<UISprite>();
				if (IsTargetNotNull(sprite))
				{
					sprite.spriteName = spriteName;
				}
			}
		}

		public static void SetChildTexture(Component parentComp, string path, Texture mainTexture, Texture alphaTexture = null, string shaderName = null)
		{
			if (IsTargetNotNull(parentComp))
			{
				UITexture uiTexture = CompAgent.FindChild<UITexture>(parentComp, path);
				SetTexture(uiTexture, mainTexture, alphaTexture, shaderName);
			}
		}

		public static void SetChildTexture(GameObject parentGo, string path, Texture mainTexture, Texture alphaTexture = null, string shaderName = null)
		{
			if (IsTargetNotNull(parentGo))
			{
				SetChildTexture(parentGo.transform, path, mainTexture, alphaTexture, shaderName);
			}
		}

		public static void SetTexture(Component comp, Texture mainTexture, Texture alphaTexture = null, string shaderName = null)
		{
			if (IsTargetNotNull(comp))
			{
				UITexture uiTexture = comp.GetComp<UITexture>();
				if (IsTargetNotNull(uiTexture))
				{
					uiTexture.mainTexture = mainTexture;
					uiTexture.alphaTexture = alphaTexture;
					if (!string.IsNullOrEmpty(shaderName))
					{
						uiTexture.shader = Shader.Find(shaderName);
					}
				}
			}
		}

		public static void SetTexture(GameObject go, Texture mainTexture, Texture alphaTexture = null, string shaderName = null)
		{
			if (IsTargetNotNull(go))
			{
				SetTexture(go.GetComponent<UITexture>(), mainTexture, alphaTexture, shaderName);
			}
		}

		public static void SetChildIcon(Component parentComp, string path, UIAtlas atlas, string spriteName = null)
		{
			if (IsTargetNotNull(parentComp))
			{
				UISprite uiSprite = CompAgent.FindChild<UISprite>(parentComp, path);
				SetIcon(uiSprite, atlas, spriteName);
			}
		}

		public static void SetChildIcon(GameObject parentGo, string path, UIAtlas atlas, string spriteName = null)
		{
			if (IsTargetNotNull(parentGo))
			{
				SetChildIcon(parentGo.transform, path, atlas, spriteName);
			}
		}

		public static void SetIcon(Component comp, UIAtlas atlas, string spriteName = null)
		{
			if (IsTargetNotNull(comp))
			{
				UISprite uiSprite = comp.GetComp<UISprite>();
				if (IsTargetNotNull(uiSprite))
				{
					uiSprite.atlas = atlas;
					uiSprite.spriteName = spriteName;
				}
			}

		}
		public static void SetIcon(GameObject go, UIAtlas atlas, string spriteName = null)
		{
			if (IsTargetNotNull(go))
			{
				SetIcon(go.GetComponent<UISprite>(), atlas, spriteName);
			}
		}

		public static void SetChildSize(Component parentComp, string path, float width, float height)
		{
			if (IsTargetNotNull(parentComp))
			{
				Transform child = CompAgent.FindChild<Transform>(parentComp, path);
				SetSize(child, width, height);
			}
		}

		public static void SetChildSize(GameObject parentGo, string path, float width, float height)
		{
			if (IsTargetNotNull(parentGo))
			{
				SetChildSize(parentGo.transform, path, width, height);
			}
		}

		public static void SetSize(GameObject go, float width, float height)
		{
			if (IsTargetNotNull(go))
			{
				SetSize(go.transform, width, height);
			}
		}

		public static void SetSize(Component comp, float width, float height)
		{
			if (IsTargetNotNull(comp))
			{
				UIRect rect = comp.GetComp<UIRect>();
				if (IsTargetNotNull(rect))
				{
					if (rect is UIWidget)
					{
						UIWidget widget = rect as UIWidget;
						int widthInt = Mathf.RoundToInt(width);
						if (widthInt > 0)
						{
							widget.width = widthInt;
						}
						int heightInt = Mathf.RoundToInt(height);
						if (heightInt > 0)
						{
							widget.height = heightInt;
						}
					}
					else if (rect is UIPanel)
					{
						UIPanel panel = rect as UIPanel;
						Vector4 baseClipRegion = panel.baseClipRegion;
						if (width > 0)
						{
							baseClipRegion.z = width;
						}
						if (height > 0)
						{
							baseClipRegion.w = height;
						}
						panel.baseClipRegion = baseClipRegion;
					}
					else
					{
						Debugger.LogError("Can't find widget or panel!", rect);
					}
				}
			}
		}

		public static void SetChildHeight(Component parentComp, string path, float height)
		{
			SetChildSize(parentComp, path, 0, height);
		}

		public static void SetChildHeight(GameObject parentGo, string path, float height)
		{
			SetChildSize(parentGo, path, 0, height);
		}

		public static void SetHeight(GameObject go, float height)
		{
			SetSize(go, 0, height);
		}

		public static void SetHeight(Component comp, float height)
		{
			SetSize(comp, 0, height);
		}

		public static void SetChildTweenValue(Component parentComp, string path, bool value, int group = 0)
		{
			SetChildTweenValue(parentComp, path, value ? 1 : 0, group);
		}

		public static void SetChildTweenValue(GameObject parentGo, string path, bool value, int group = 0)
		{
			SetChildTweenValue(parentGo, path, value ? 1 : 0, group);
		}

		public static void SetTweenValue(Component comp, bool tweenFactor, int group = 0)
		{
			SetTweenValue(comp, tweenFactor ? 1 : 0, group);
		}

		public static void SetTweenValue(GameObject go, bool tweenFactor, int group = 0)
		{
			SetTweenValue(go, tweenFactor ? 1 : 0, group);
		}

		public static void SetChildTweenValue(Component parentComp, string path, float value, int group = 0)
		{
			if (IsTargetNotNull(parentComp))
			{
				Transform child = CompAgent.FindChild(parentComp, path);
				SetTweenValue(child, value, group);
			}
		}

		public static void SetChildTweenValue(GameObject parentGo, string path, float value, int group = 0)
		{
			if (IsTargetNotNull(parentGo))
			{
				SetChildTweenValue(parentGo.transform, path, value, group);
			}
		}

		public static void SetTweenValue(Component comp, float tweenFactor, int group = 0)
		{
			if (IsTargetNotNull(comp))
			{
				if (comp is UITweener)
				{
					UITweener tweener = comp as UITweener;
					if (group == -1 || tweener.tweenGroup == group)
					{
						tweener.tweenFactor = tweenFactor;
						tweener.Sample(tweenFactor, true);
						tweener.enabled = false;
					}
				}
				else
				{
					foreach (UITweener tweener in comp.GetComponents<UITweener>())
					{
						if (group == -1 || tweener.tweenGroup == group)
						{
							tweener.tweenFactor = tweenFactor;
							tweener.Sample(tweenFactor, true);
							tweener.enabled = false;
						}
					}
				}
			}
		}

		public static void SetTweenValue(GameObject go, float tweenFactor, int group = 0)
		{
			if (IsTargetNotNull(go))
			{
				foreach (UITweener tweener in go.GetComponents<UITweener>())
				{
					SetTweenValue(tweener, tweenFactor, group);
				}
			}
		}

		public static float PlayChildTweener(Component parentComp, string path, int isForward, bool restart = false, int group = 0)
		{
			if (isForward > 0)
			{
				return PlayChildForward(parentComp, path, restart, group);
			}
			else
			{
				return PlayChildReverse(parentComp, path, restart, group);
			}
		}

		public static float PlayChildTweener(GameObject parentGo, string path, int isForward, bool restart = false, int group = 0)
		{
			if (isForward > 0)
			{
				return PlayChildForward(parentGo, path, restart, group);
			}
			else
			{
				return PlayChildReverse(parentGo, path, restart, group);
			}
		}

		public static float PlayTweener(Component comp, int isForward, bool restart = false, int group = 0)
		{
			if (isForward > 0)
			{
				return PlayForward(comp, restart, group);
			}
			else
			{
				return PlayReverse(comp, restart, group);
			}
		}

		public static float PlayTweener(GameObject go, int isForward, bool restart = false, int group = 0)
		{
			if (isForward > 0)
			{
				return PlayForward(go, restart, group);
			}
			else
			{
				return PlayReverse(go, restart, group);
			}
		}

		public static float PlayChildTweener(Component parentComp, string path, bool isForward, bool restart = false, int group = 0)
		{
			if (isForward)
			{
				return PlayChildForward(parentComp, path, restart, group);
			}
			else
			{
				return PlayChildReverse(parentComp, path, restart, group);
			}
		}

		public static float PlayChildTweener(GameObject parentGo, string path, bool isForward, bool restart = false, int group = 0)
		{
			if (isForward)
			{
				return PlayChildForward(parentGo, path, restart, group);
			}
			else
			{
				return PlayChildReverse(parentGo, path, restart, group);
			}
		}

		public static float PlayTweener(Component comp, bool isForward, bool restart = false, int group = 0)
		{
			if (isForward)
			{
				return PlayForward(comp, restart, group);
			}
			else
			{
				return PlayReverse(comp, restart, group);
			}
		}

		public static float PlayTweener(GameObject go, bool isForward, bool restart = false, int group = 0)
		{
			if (isForward)
			{
				return PlayForward(go, restart, group);
			}
			else
			{
				return PlayReverse(go, restart, group);
			}
		}

		public static float PlayChildForward(Component parentComp, string path, bool restart = false, int group = 0)
		{
			if (IsTargetNotNull(parentComp))
			{
				Transform child = CompAgent.FindChild<Transform>(parentComp, path);
				return PlayForward(child, restart, group);
			}
			return 0;
		}

		public static float PlayChildForward(GameObject parentGo, string path, bool restart = false, int group = 0)
		{
			if (IsTargetNotNull(parentGo))
			{
				return PlayChildForward(parentGo.transform, path, restart, group);
			}
			return 0;
		}

		public static float PlayForward(Component comp, bool restart = false, int group = 0)
		{
			if (IsTargetNotNull(comp))
			{
				if (comp is UITweener)
				{
					UITweener tweener = comp as UITweener;
					if (group == -1 || tweener.tweenGroup == group)
					{
						if (restart)
						{
							tweener.tweenFactor = 0;
							tweener.Sample(0, true);
							tweener.enabled = false;
						}
						tweener.PlayForward();
						return tweener.delay + tweener.duration;
					}
				}
				else
				{
					float time = 0;
					foreach (UITweener tweener in comp.GetComponents<UITweener>())
					{
						if (group == -1 || tweener.tweenGroup == group)
						{
							if (restart)
							{
								tweener.tweenFactor = 0;
								tweener.Sample(0, true);
								tweener.enabled = false;
							}
							tweener.PlayForward();
							time = Mathf.Max(time, tweener.delay + tweener.duration);
						}
					}
					return time;
				}
			}
			return 0;
		}

		public static float PlayForward(GameObject go, bool restart = false, int group = 0)
		{
			if (IsTargetNotNull(go))
			{
				float time = 0;
				foreach (UITweener tweener in go.GetComponents<UITweener>())
				{
					if (group == -1 || tweener.tweenGroup == group)
					{
						time = PlayForward(tweener, restart, group);
					}
				}
				return time;
			}
			return 0;
		}

		public static float PlayChildReverse(Component parentComp, string path, bool restart = false, int group = 0)
		{
			if (IsTargetNotNull(parentComp))
			{
				Transform child = CompAgent.FindChild<Transform>(parentComp, path);
				return PlayReverse(child, restart, group);
			}
			return 0;
		}

		public static float PlayChildReverse(GameObject parentGo, string path, bool restart = false, int group = 0)
		{
			if (IsTargetNotNull(parentGo))
			{
				return PlayChildReverse(parentGo.transform, path, restart, group);
			}
			return 0;
		}

		public static float PlayReverse(Component comp, bool restart = false, int group = 0)
		{
			if (IsTargetNotNull(comp))
			{
				if (comp is UITweener)
				{
					UITweener tweener = comp as UITweener;
					if (group == -1 || tweener.tweenGroup == group)
					{
						if (restart)
						{
							tweener.tweenFactor = 1;
							tweener.Sample(1, true);
							tweener.enabled = false;
						}
						tweener.PlayReverse();
						return tweener.delay + tweener.duration;
					}
				}
				else
				{
					float time = 0;
					foreach (UITweener tweener in comp.GetComponents<UITweener>())
					{
						if (group == -1 || tweener.tweenGroup == group)
						{
							if (restart)
							{
								tweener.tweenFactor = 1;
								tweener.Sample(1, true);
								tweener.enabled = false;
							}
							tweener.PlayReverse();
							time = Mathf.Max(time, tweener.delay + tweener.duration);
						}
					}
					return time;
				}
			}
			return 0;
		}

		public static float PlayReverse(GameObject go, bool restart = false, int group = 0)
		{
			if (IsTargetNotNull(go))
			{
				float time = 0;
				foreach (UITweener tweener in go.GetComponents<UITweener>())
				{
					if (group == -1 || tweener.tweenGroup == group)
					{
						time = PlayReverse(tweener, restart, group);
					}
				}
				return time;
			}
			return 0;
		}

		public static void SetChildProgress(Component parentComp, string path, float value)
		{
			if (IsTargetNotNull(parentComp))
			{
				UIProgressBar bar = CompAgent.FindChild<UIProgressBar>(parentComp, path);
				SetProgress(bar, value);
			}
		}

		public static void SetChildProgress(GameObject parentGo, string path, float value)
		{
			if (IsTargetNotNull(parentGo))
			{
				SetChildProgress(parentGo.transform, path, value);
			}
		}

		public static void SetProgress(Component comp, float value)
		{
			if (IsTargetNotNull(comp))
			{
				UIProgressBar bar = comp.GetComp<UIProgressBar>();
				if (IsTargetNotNull(bar))
				{
					bar.value = value;
				}
			}
		}

		public static void SetProgress(GameObject go, float value)
		{
			if (IsTargetNotNull(go))
			{
				UIProgressBar bar = go.GetComponent<UIProgressBar>();
				SetProgress(bar, value);
			}
		}

		public static void SetChildBtnEnabled(Component parentComp, string path, bool enabled)
		{
			if (IsTargetNotNull(parentComp))
			{
				UIButton bar = CompAgent.FindChild<UIButton>(parentComp, path);
				SetBtnEnabled(bar, enabled);
			}
		}

		public static void SetChildBtnEnabled(GameObject parentGo, string path, bool enabled)
		{
			if (IsTargetNotNull(parentGo))
			{
				SetChildBtnEnabled(parentGo.transform, path, enabled);
			}
		}

		public static void SetBtnEnabled(Component comp, bool enabled)
		{
			if (IsTargetNotNull(comp))
			{
				UIButton btn = comp.GetComp<UIButton>();
				if (IsTargetNotNull(btn))
				{
					btn.isEnabled = enabled;
				}
			}
		}

		public static void SetBtnEnabled(GameObject go, bool enabled)
		{
			if (IsTargetNotNull(go))
			{
				UIButton bar = go.GetComponent<UIButton>();
				SetBtnEnabled(bar, enabled);
			}
		}

		public static void ClearChildClick(Component parentComp, string path, string listenerTag = null)
		{
			if (IsTargetNotNull(parentComp))
			{
				Transform trans = CompAgent.FindChild<Transform>(parentComp, path);
				ClearClick(trans, listenerTag);
			}
		}

		public static void ClearChildClick(GameObject parentGo, string path, string listenerTag = null)
		{
			if (IsTargetNotNull(parentGo))
			{
				ClearChildClick(parentGo.transform, path, listenerTag);
			}
		}

		public static void ClearClick(Component comp, string listenerTag = null)
		{
			if (IsTargetNotNull(comp))
			{
				ClearClick(comp.gameObject, listenerTag);
			}
		}

		public static void ClearClick(GameObject go, string listenerTag = null)
		{
			if (IsTargetNotNull(go))
			{
				if (listenerTag == null)
				{
					UIEventListener.Get(go).onClick = null;
				}
				else
				{
					UIEventListener.Get(go, listenerTag).onClick = null;
				}
			}
		}

		/// <summary>
		/// if action is null, please use ClearClick instead
		/// </summary>
		public static void SetChildOnClick(Component parentComp, string path, params System.Action[] actions)
		{
			if (IsTargetNotNull(parentComp))
			{
				Transform trans = CompAgent.FindChild<Transform>(parentComp, path);
				SetOnClick(trans, actions);
			}
		}

		/// <summary>
		/// if action is null, please use ClearClick instead
		/// </summary>
		public static void SetChildOnClick(GameObject parentGo, string path, params System.Action[] actions)
		{
			if (IsTargetNotNull(parentGo))
			{
				SetChildOnClick(parentGo.transform, path, actions);
			}
		}

		/// <summary>
		/// if action is null, please use ClearClick instead
		/// </summary>
		public static void SetOnClick(Component comp, params System.Action[] actions)
		{
			if (IsTargetNotNull(comp))
			{
				SetOnClick(comp.gameObject, actions);
			}
		}

		/// <summary>
		/// if action is null, please use ClearClick instead
		/// </summary>
		public static void SetOnClick(GameObject go, params System.Action[] actions)
		{
			if (IsTargetNotNull(go))
			{
				UIEventListener.Get(go).onClick = gameObj =>
				{
					foreach (System.Action action in actions)
					{
						if (action != null)
						{
							action();
						}
					}
				};
			}
		}

		/// <summary>
		/// if action is null, please use ClearClick instead
		/// </summary>
		public static void SetChildOnClick(Component parentComp, string path, params System.Action<GameObject>[] actions)
		{
			if (IsTargetNotNull(parentComp))
			{
				Transform trans = CompAgent.FindChild<Transform>(parentComp, path);
				SetOnClick(trans, actions);
			}
		}

		/// <summary>
		/// if action is null, please use ClearClick instead
		/// </summary>
		public static void SetChildOnClick(GameObject parentGo, string path, params System.Action<GameObject>[] actions)
		{
			if (IsTargetNotNull(parentGo))
			{
				SetChildOnClick(parentGo.transform, path, actions);
			}
		}

		/// <summary>
		/// if action is null, please use ClearClick instead
		/// </summary>
		public static void SetOnClick(Component comp, params System.Action<GameObject>[] actions)
		{
			if (IsTargetNotNull(comp))
			{
				SetOnClick(comp.gameObject, actions);
			}
		}

		/// <summary>
		/// if action is null, please use ClearClick instead
		/// </summary>
		public static void SetOnClick(GameObject go, params System.Action<GameObject>[] actions)
		{
			if (IsTargetNotNull(go))
			{
				UIEventListener listener = UIEventListener.Get(go);
				listener.onClick = null;
				foreach (System.Action<GameObject> action in actions)
				{
					listener.onClick += action;
				}
			}
		}

		/// <summary>
		/// if action is null, please use ClearPress instead
		/// </summary>
		public static void SetChildOnPress(Component parentComp, string path, params System.Action<GameObject, bool>[] actions)
		{
			if (IsTargetNotNull(parentComp))
			{
				Transform trans = CompAgent.FindChild<Transform>(parentComp, path);
				SetOnPress(trans, actions);
			}
		}

		/// <summary>
		/// if action is null, please use ClearPress instead
		/// </summary>
		public static void SetChildOnPress(GameObject parentGo, string path, params System.Action<GameObject, bool>[] actions)
		{
			if (IsTargetNotNull(parentGo))
			{
				SetChildOnPress(parentGo.transform, path, actions);
			}
		}

		/// <summary>
		/// if action is null, please use ClearPress instead
		/// </summary>
		public static void SetOnPress(Component comp, params System.Action<GameObject, bool>[] actions)
		{
			if (IsTargetNotNull(comp))
			{
				SetOnPress(comp.gameObject, actions);
			}
		}

		/// <summary>
		/// if action is null, please use ClearPress instead
		/// </summary>
		public static void SetOnPress(GameObject go, params System.Action<GameObject, bool>[] actions)
		{
			if (IsTargetNotNull(go))
			{
				UIEventListener listener = UIEventListener.Get(go);
				listener.onPress = null;
				foreach (System.Action<GameObject, bool> action in actions)
				{
					listener.onPress += action;
				}
			}
		}

		/// <summary>
		/// if action is null, please use ClearDrag instead
		/// </summary>
		public static void SetChildOnDrag(Component parentComp, string path, params System.Action<GameObject, Vector2>[] actions)
		{
			if (IsTargetNotNull(parentComp))
			{
				Transform trans = CompAgent.FindChild<Transform>(parentComp, path);
				SetOnDrag(trans, actions);
			}
		}

		/// <summary>
		/// if action is null, please use ClearDrag instead
		/// </summary>
		public static void SetChildOnDrag(GameObject parentGo, string path, params System.Action<GameObject, Vector2>[] actions)
		{
			if (IsTargetNotNull(parentGo))
			{
				SetChildOnDrag(parentGo.transform, path, actions);
			}
		}

		/// <summary>
		/// if action is null, please use ClearDrag instead
		/// </summary>
		public static void SetOnDrag(Component comp, params System.Action<GameObject, Vector2>[] actions)
		{
			if (IsTargetNotNull(comp))
			{
				SetOnDrag(comp.gameObject, actions);
			}
		}

		/// <summary>
		/// if action is null, please use ClearDrag instead
		/// </summary>
		public static void SetOnDrag(GameObject go, params System.Action<GameObject, Vector2>[] actions)
		{
			if (IsTargetNotNull(go))
			{
				UIEventListener listener = UIEventListener.Get(go);
				listener.onDrag = null;
				foreach (System.Action<GameObject, Vector2> action in actions)
				{
					listener.onDrag += action;
				}
			}
		}

		private static bool IsTargetNull(Object obj, string message = null)
		{
			return PointerCheck.IsNull(obj, message ?? "Object is null!");
		}

		private static bool IsTargetNotNull(Object obj, string message = null)
		{
			return !IsTargetNull(obj, message);
		}
	}
}