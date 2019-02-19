/*
 * Tencent is pleased to support the open source community by making xLua available.
 * Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
 * Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
 * http://opensource.org/licenses/MIT
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using System.Collections.Generic;
using System;
using UnityEngine;
using XLua;
using System.Reflection;
using System.Collections;
using UnityEngine.Networking;
using System.Threading;

//配置的详细介绍请看Doc下《XLua的配置.doc》
public static class GenConfig
{
	[LuaCallCSharp]
	public static List<Type> LuaCallCSharpMain
	{
		get
		{
			List<Type> luaCallCSharpList = new List<Type>();
			foreach (Type type in Assembly.Load("Assembly-CSharp").GetTypes())
			{
				if (type.IsPublic || type.IsNestedPublic)
				{
					if (string.Equals(type.Namespace, "Main"))
					{
						luaCallCSharpList.Add(type);
					}
				}
			}
			return luaCallCSharpList;
		}
	}

	[LuaCallCSharp]
	public static List<Type> LuaCallCSharpSkillz
	{
		get
		{
			List<Type> luaCallCSharpList = new List<Type>();
			foreach (Type type in Assembly.Load("Assembly-CSharp").GetTypes())
			{
				if (type.IsPublic || type.IsNestedPublic)
				{
					if (type.Namespace == null ? type.FullName.Contains("Skillz") : string.Equals(type.Namespace, "SkillzSDK"))
					{
						if (!type.FullName.Contains("SkillzAndroidPreProcessBuild") && !type.FullName.Contains("SkillzAndroidPostProcessBuild"))
						{
							luaCallCSharpList.Add(type);
						}
					}
				}
			}
			return luaCallCSharpList;
		}
	}

	[LuaCallCSharp]
	public static List<Type> LuaCallCSharpNgui = new List<Type>() {
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

	//lua中要使用到C#库的配置，比如C#标准库，或者Unity API，第三方库等。
	[LuaCallCSharp]
    public static List<Type> LuaCallCSharp = new List<Type>() {
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

				typeof(Main.Debugger),
				typeof(Main.PointerCheck),
				typeof(SimpleJson.JsonObject),
				typeof(SimpleJson.JsonArray),
				typeof(SimpleJson.SimpleJson),
			};

	[CSharpCallLua]
	public static List<Type> CSharpCallLuaMain
	{
		get
		{
			List<Type> luaCallCSharpList = new List<Type>();
			foreach (Type type in Assembly.Load("Assembly-CSharp").GetTypes())
			{
				if ((type.IsPublic || type.IsNestedPublic) && (type.IsInterface || typeof(Delegate).IsAssignableFrom(type)))
				{
					if (string.Equals(type.Namespace, "Main"))
					{
						luaCallCSharpList.Add(type);
					}
				}
			}
			return luaCallCSharpList;
		}
	}

	[CSharpCallLua]
	public static List<Type> CSharpCallLuaSkillz
	{
		get
		{
			List<Type> luaCallCSharpList = new List<Type>();
			foreach (Type type in Assembly.Load("Assembly-CSharp").GetTypes())
			{
				if ((type.IsPublic || type.IsNestedPublic) && (type.IsInterface || typeof(Delegate).IsAssignableFrom(type)))
				{
					if (type.Namespace == null ? type.FullName.Contains("Skillz") : string.Equals(type.Namespace, "SkillzSDK"))
					{
						luaCallCSharpList.Add(type);
					}
				}
			}
			return luaCallCSharpList;
		}
	}

	//C#静态调用Lua的配置（包括事件的原型），仅可以配delegate，interface
	[CSharpCallLua]
	public static List<Type> CSharpCallLua = new List<Type>() {
				typeof(Action),
				typeof(Action<int>),
				typeof(Action<byte[]>),
				typeof(Func<string, bool>),
				typeof(Func<string, string>),
				typeof(Func<LuaTable, float>),
				typeof(Func<double, double, double>),
				typeof(Action<string>),
				typeof(Action<double>),
				typeof(IEnumerator),
				typeof(UnityEngine.Events.UnityAction),

				typeof(Action<GameObject>), // OnClick and so on
				typeof(Action<GameObject, bool>), // OnPress、onEnabled、OnTooltip
				typeof(Action<GameObject, float>), // OnScroll
				typeof(Action<GameObject, Vector2>), // OnDrag
				typeof(Action<GameObject, GameObject>), // OnDrop
				typeof(Action<GameObject, KeyCode>), // OnKey

				typeof(UIPanel.OnClippingMoved), // UIPanel
                typeof(UIScrollView.OnDragNotification),
				typeof(Action<Transform, int>), // UIShortScorll

				typeof(Func<long, string>), // TweenIntText
				
				typeof(Func<string, int, string>), // Debug.TraceBack

				typeof(Func<string, LuaTable>), // Require、DoFile
				typeof(Func<string, LuaFunction>), // LoadFile
				typeof(Func<object, object[], object>), // FuncInvoke

				// BehaviourListener
				typeof(Action<LuaTable>),
				typeof(Action<LuaTable, bool>),
				typeof(Action<LuaTable, Collider>),
				typeof(Action<LuaTable, Collision>),

				// HotFix
				typeof(Action<LuaTable, object, LuaFunction>),

				// table invoke
				typeof(Action<LuaTable>),
				typeof(Action<LuaTable, object>),
				typeof(Action<LuaTable, object, object>),
				typeof(Action<LuaTable, object, object, object>),

				// make action
				typeof(Action),
				typeof(Action<object>),
				typeof(Action<object, object>),
				typeof(Action<object, object, object>),
				typeof(Action<object, object, object, object>),
				typeof(Func<object>),
				typeof(Func<object, object>),
				typeof(Func<object, object, object>),
				typeof(Func<object, object, object, object>),
				typeof(Func<object, object, object, object, object>),

				typeof(ThreadStart),
				typeof(ParameterizedThreadStart),
			};

	[GCOptimize]
	public static List<Type> GCOptimize = new List<Type>() {
				typeof(Color32),
				typeof(Rect),
				typeof(Keyframe),
			};

	//黑名单
	[BlackList]
    public static List<List<string>> BlackList = new List<List<string>>()  {
                new List<string>(){"UnityEngine.WWW", "movie"},
                new List<string>(){"UnityEngine.Texture2D", "alphaIsTransparency"},
                new List<string>(){"UnityEngine.Security", "GetChainOfTrustValue"},
                new List<string>(){"UnityEngine.CanvasRenderer", "onRequestRebuild"},
                new List<string>(){"UnityEngine.Light", "areaSize"},
                new List<string>(){"UnityEngine.AnimatorOverrideController", "PerformOverrideClipListCleanup"},
    #if !UNITY_WEBPLAYER
                new List<string>(){"UnityEngine.Application", "ExternalEval"},
    #endif
                new List<string>(){"UnityEngine.GameObject", "networkView"}, //4.6.2 not support
                new List<string>(){"UnityEngine.Component", "networkView"},  //4.6.2 not support
                new List<string>(){"System.IO.FileInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.FileInfo", "SetAccessControl", "System.Security.AccessControl.FileSecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.DirectoryInfo", "SetAccessControl", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "CreateSubdirectory", "System.String", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "Create", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"UnityEngine.MonoBehaviour", "runInEditMode"},
				new List<string>(){"UnityEngine.WWW", "threadPriority"},

				new List<string>(){"UIDrawCall", "isActive"},
				new List<string>(){"UIInput", "ProcessEvent", "UnityEngine.Event"},
				new List<string>(){"UIWidget", "showHandles"},
				new List<string>(){"UIWidget", "showHandlesWithMoveTool"},

				new List<string>(){"UnityEngine.Input", "IsJoystickPreconfigured", "System.String"},
				new List<string>(){"Main.ResourcesManager", "LoadAssetEditor"},

				new List<string>(){ "SimpleJson.SimpleJson", "TryDeserializeObject", "System.String", "System.Object&"},
			};
}
