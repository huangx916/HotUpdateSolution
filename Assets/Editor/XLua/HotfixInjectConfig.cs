/*
 * Tencent is pleased to support the open source community by making xLua available.
 * Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
 * Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
 * http://opensource.org/licenses/MIT
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using XLua;
using Main;

//配置的详细介绍请看Doc下《XLua的配置.doc》
public static class HotfixInjectConfig
{

	[Hotfix]
	public static List<Type> HotfixMain
	{
		get
		{
			List<Type> hotfixList = new List<Type>();
			foreach (Type type in Assembly.Load("Assembly-CSharp").GetTypes())
			{
				if (string.Equals(type.Namespace, "Main"))
				{
					hotfixList.Add(type);
				}
			}
			return hotfixList;
		}
	}

	[Hotfix]
	public static List<Type> HotfixSkillz
	{
		get
		{
			List<Type> hotfixList = new List<Type>();
			foreach (Type type in Assembly.Load("Assembly-CSharp").GetTypes())
			{
				if (type.Namespace == null ? type.FullName.Contains("Skillz") : string.Equals(type.Namespace, "SkillzSDK"))
				{
					if (!type.FullName.Equals("SkillzAndroidPreProcessBuild") && !type.FullName.Equals("SkillzAndroidPostProcessBuild"))
					{
						hotfixList.Add(type);
					}
				}
			}
			return hotfixList;
		}
	}

	[Hotfix]
	public static List<Type> HotfixNgui = new List<Type>() {
#region ngui
				typeof(UIButtonMessage),
				typeof(TweenOrthoSize),
				typeof(UIPanel),
				typeof(UIDragResize),
				typeof(UIForwardEvents),
				typeof(TweenHeight),
				typeof(TweenEffectColor),
				typeof(UIToggledComponents),
				typeof(Localization),
				typeof(UIButtonScale),
				typeof(TweenScale),
				typeof(TweenColor),
				typeof(UIViewport),
				typeof(UIWidget),
				typeof(SpringPosition),
				typeof(TweenIntText),
				typeof(UIDragObject),
				typeof(UIScrollView),
				typeof(UIButtonColor),
				typeof(UIEffect),
				typeof(UICenterOnClick),
				typeof(EnvelopContent),
				typeof(ByteReader),
				typeof(BMGlyph),
				typeof(UITextList),
				typeof(AnimatedAlpha),
				typeof(UIStretch),
				typeof(UIDragDropItem),
				typeof(UIKeyBinding),
				typeof(NGUIMath),
				typeof(UIPlayTween),
				typeof(UIBasicSprite),
				typeof(TweenAlpha),
				typeof(UIBlank),
				typeof(AnimatedWidget),
				typeof(UILabel),
				typeof(UIButton),
				typeof(UIDraggableCamera),
				typeof(UIFont),
				typeof(UIInputOnGUI),
				typeof(UILocalize),
				typeof(TweenPosition),
				typeof(UISnapshotPoint),
				typeof(UISoundVolume),
				typeof(TweenFOV),
				typeof(UIPlayAnimation),
				typeof(UITweener),
				typeof(UICenterOnChild),
				typeof(BMFont),
				typeof(UIRoot),
				typeof(UISavedOption),
				typeof(UIAnchor),
				typeof(TweenWidth),
				typeof(PropertyBinding),
				typeof(TweenLetters),
				typeof(UIToggle),
				typeof(UITexture),
				typeof(UIEventTrigger),
				typeof(MinMaxRangeAttribute),
				typeof(UITextureEx),
				typeof(UIDrawCall),
				typeof(UICamera),
				typeof(UIShowControlScheme),
				typeof(UIWidgetContainer),
				typeof(UITooltip),
				typeof(UIInput),
				typeof(UIPlaySound),
				typeof(UIRect),
				typeof(TweenTransform),
				typeof(TweenRotation),
				typeof(UIPopupList),
				typeof(TweenMaterialColor),
				typeof(TypewriterEffect),
				typeof(UIColorPicker),
				typeof(UISpriteData),
				typeof(UIDragDropContainer),
				typeof(PropertyReference),
				typeof(UISpecialTable),
				typeof(AnimatedColor),
				typeof(SpringPanel),
				typeof(UIProgressBar),
				typeof(BMSymbol),
				typeof(UIDragScrollView),
				typeof(UIGrid),
				typeof(NGUIDebug),
				typeof(UIDragDropRoot),
				typeof(UIAtlas),
				typeof(UIButtonRotation),
				typeof(UISpecialGrid),
				typeof(UIWrapContent),
				typeof(UIButtonOffset),
				typeof(UISpriteAnimation),
				typeof(UISpriteAnimationEx),
				typeof(EventDelegate),
				typeof(UIScrollBar),
				typeof(NGUIText),
				typeof(UIOrthoCamera),
				typeof(UIButtonActivate),
				typeof(TweenVolume),
				typeof(UIKeyNavigation),
				typeof(UIButtonKeys),
				typeof(UIToggledObjects),
				typeof(UIEventListener),
				typeof(UIGeometry),
				typeof(LanguageSelection),
				typeof(UIDragCamera),
				typeof(UIShortScroll),
				typeof(UISlider),
				typeof(UITable),
				typeof(ActiveAnimation),
				typeof(TweenFillAmount),
				typeof(TweenProgress),
				typeof(UI2DSprite),
				typeof(UIImageButton),
				typeof(UI2DSpriteAnimation),
				typeof(UISprite),
				typeof(NGUITools),
				typeof(RealTime),
				typeof(PhysicsListener),

				typeof(AnimationOrTween.Trigger),
				typeof(AnimationOrTween.Direction),
				typeof(AnimationOrTween.EnableCondition),
				typeof(AnimationOrTween.DisableCondition),
				typeof(UIButtonMessage.Trigger),
				typeof(UIWidget.Pivot),
				typeof(UIDragObject.DragEffect),
				typeof(UIScrollView.Movement),
				typeof(UIScrollView.DragEffect),
				typeof(UIScrollView.ShowCondition),
				typeof(UIButtonColor.State),
				typeof(UIStretch.Style),
				typeof(UIDragDropItem.Restriction),
				typeof(UIKeyBinding.Action),
				typeof(UIKeyBinding.Modifier),
				typeof(UIBasicSprite.Type),
				typeof(UIBasicSprite.FillDirection),
				typeof(UIBasicSprite.AdvancedType),
				typeof(UIBasicSprite.Flip),
				typeof(UILabel.Effect),
				typeof(UILabel.Overflow),
				typeof(UILabel.Crispness),
				typeof(UILabel.Modifier),
				typeof(UITweener.Method),
				typeof(UITweener.Style),
				typeof(UIRoot.Scaling),
				typeof(UIRoot.Constraint),
				typeof(UIAnchor.Side),
				typeof(PropertyBinding.UpdateCondition),
				typeof(PropertyBinding.Direction),
				typeof(TweenLetters.AnimationProperties),
				typeof(UIDrawCall.Clipping),
				typeof(UICamera.ControlScheme),
				typeof(UICamera.ClickNotification),
				typeof(UICamera.MouseOrTouch),
				typeof(UICamera.EventType),
				typeof(UIInput.InputType),
				typeof(UIInput.Validation),
				typeof(UIInput.KeyboardType),
				typeof(UIInput.OnReturnKey),
				typeof(UIPlaySound.Trigger),
				typeof(UIRect.AnchorPoint),
				typeof(UIRect.AnchorUpdate),
				typeof(UIPopupList.Position),
				typeof(UIPopupList.Selection),
				typeof(UIProgressBar.FillDirection),
				typeof(UIGrid.Arrangement),
				typeof(UIGrid.Sorting),
				typeof(EventDelegate.Parameter),
				typeof(NGUIText.Alignment),
				typeof(NGUIText.SymbolStyle),
				typeof(NGUIText.GlyphInfo),
				typeof(UIKeyNavigation.Constraint),
				typeof(UITable.Direction),
				typeof(UITable.Sorting)
#endregion
			};

	[Hotfix]
	public static List<Type> HotfixOther = new List<Type>() {
				typeof(UnityEngine.Object),
				typeof(Time),
				typeof(Vector2),
				typeof(Vector3),
				typeof(Vector4),
				typeof(Quaternion),
				typeof(Color),
				typeof(Color32),
				typeof(Physics),
				typeof(Physics2D),
				typeof(Mathf),
				typeof(UnityEngine.Random),
				typeof(Debug),
				typeof(Camera),
				typeof(RaycastHit),
				typeof(Ray),
				typeof(Ray2D),
				typeof(Bounds),
				typeof(Rect),
				typeof(GameObject),
				typeof(Component),
				typeof(Behaviour),
				typeof(MonoBehaviour),
				typeof(Transform),
				typeof(Collider),
				typeof(Collision),
				typeof(Resources),
				typeof(AssetBundle),
				typeof(AssetBundleManifest),
				typeof(TextAsset),
				typeof(Keyframe),
				typeof(AnimationCurve),
				typeof(AnimationClip),
				typeof(MonoBehaviour),
				typeof(ParticleSystem),
				typeof(Renderer),
				typeof(SkinnedMeshRenderer),
				typeof(WWW),
				typeof(Texture),
				typeof(Texture2D),
				typeof(AudioSource),
				typeof(AudioClip),
				typeof(Input),
				typeof(KeyCode),
				typeof(Application),
				typeof(SystemInfo),
				typeof(Coroutine),
				typeof(WaitForSeconds),
				typeof(WaitForSecondsRealtime),
				typeof(WaitForEndOfFrame),
				typeof(WaitForFixedUpdate),
				typeof(YieldInstruction),
				typeof(CustomYieldInstruction),
				typeof(WaitUntil),
				typeof(WaitWhile),
				typeof(AsyncOperation),
				typeof(AssetBundleRequest),
				typeof(AssetBundleCreateRequest),
				typeof(PlayerPrefs),
				typeof(LineRenderer),

				typeof(Screen),
				typeof(Resolution),
				typeof(ScreenOrientation),

				typeof(UnityWebRequest),
				typeof(UploadHandler),
				typeof(UploadHandlerRaw),
				typeof(DownloadHandler),
				typeof(DownloadHandlerBuffer),
				typeof(DownloadHandlerAssetBundle),
				typeof(DownloadHandlerAudioClip),
				typeof(DownloadHandlerScript),
				typeof(DownloadHandlerTexture),

				typeof(object),
				typeof(int),
				typeof(GC),
				typeof(Math),
				typeof(System.Random),
				typeof(Delegate),
				typeof(Enum),
				typeof(Array),
				typeof(TimeSpan),
				typeof(DateTime),
				typeof(List<int>),
				typeof(IList),
				typeof(IDictionary),
				typeof(ICollection),
				typeof(IEnumerator),
				typeof(IDisposable),
				typeof(Action<string>),
				typeof(Action),

				typeof(Debugger),
				typeof(PointerCheck),
				typeof(SimpleJson.JsonObject),
				typeof(SimpleJson.JsonArray),
				typeof(SimpleJson.SimpleJson),
			};
}
