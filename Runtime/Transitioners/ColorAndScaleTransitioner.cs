#if UNITY_IOS || UNITY_ANDROID
#define INTERACT_MOBILE
#elif UNITY_STANDALONE || UNITY_WEBGL
#define INTERACT_DESKTOP
#else
#define INTERACT_CONSOLE
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HiddenAchievement.CrossguardUi
{
	/// <summary>
	/// Handles the transition logic for tweening between Interact states for a UI component.
	/// </summary>
    [ExecuteAlways]
	public class ColorAndScaleTransitioner : AbstractTransitioner
    {
        private class ScaleTween
        {
            public RectTransform Component;
            public float StartTime;
            public Vector2 StartScale;
            public Vector2 EndScale;

            public void Set(RectTransform component, Vector2 scale)
            {
                Component = component;
                StartScale = component.localScale;
                EndScale = (Vector3)scale + Vector3.forward;
                StartTime = Time.time;
            }
        }

        private static readonly Stack<ScaleTween> _scaleTweenPool = new Stack<ScaleTween>();

        private class ComponentInfo
        {
            public string Path = string.Empty;
            public RectTransform Component = null;
            public Graphic Graphic = null;
            public CanvasRenderer Renderer = null;
            public readonly Color?[] Colors = new Color?[(int)InteractState.Count];
            public readonly float?[] Alphas = new float?[(int)InteractState.Count];
            public readonly Vector2?[] Scales = new Vector2?[(int)InteractState.Count];       
        }

        [SerializeField]
        [Tooltip("Assign the scriptable object containing this element's style.")]
        private ColorAndScaleStyle _style = default;

        private readonly List<ComponentInfo>[] _stateComponents = new List<ComponentInfo>[(int)InteractState.Count];
        private readonly List<ComponentInfo> _componentInfo = new(2);
        
        private List<ScaleTween> _activeScaleTweens;
        

        #region Public Interface
        
        public override float TransitionTime => (_style == null) ? 0 : _style.TransitionTime;

        public ColorAndScaleStyle Style
        {
            get => _style;
            set
            {
                _style = value;
                if (Application.isPlaying)
                {
                    InitializeStates();
                }
#if UNITY_EDITOR
                else
                {
                    ManualInitializeAppearance();
                }
#endif
            }
        }
        
        #endregion


        private ComponentInfo FindComponentInfo(string path)
        {
            for (int i = 0; i < _componentInfo.Count; i++)
            {
                if (_componentInfo[i].Path == path)
                {
                    return _componentInfo[i];
                }
            }

            return null;
        }

        private void ProcessStateAppearance(InteractState state, ColorAndScaleStyleEntry[] appearance)
        {
            if (appearance == null) return;

            if (_stateComponents[(int) state] == null)
            {
                List<ComponentInfo> stateComponentInfo = new List<ComponentInfo>(appearance.Length);
                _stateComponents[(int)state] = stateComponentInfo;
            }
            else
            {
                _stateComponents[(int) state].Clear();
            }

            for (int i = 0; i < appearance.Length; i++)
            {
                ComponentInfo info = ProcessStateAppearance(state, appearance[i]);
                if (info != null)
                {
                    _stateComponents[(int)state].Add(info);
                }
            }
        }

        private ComponentInfo ProcessStateAppearance(InteractState state, ColorAndScaleStyleEntry appearance)
        {
            ComponentInfo info = FindComponentInfo(appearance.ComponentPath);

            if (info == null)
            {
                RectTransform component = FindComponent(appearance.ComponentPath);
                if (component == null) return null;

                info = new ComponentInfo()
                {
                    Path = appearance.ComponentPath,
                    Component = component,
                    Renderer = component.GetComponent<CanvasRenderer>(),
                    Graphic = component.GetComponent<Graphic>()
                };
                
                _componentInfo.Add(info);
            }

            if (appearance.UseColor)
            {
                info.Colors[(int)state] = appearance.Color;
            }
            if (appearance.UseAlpha)
            {
                info.Alphas[(int)state] = appearance.Alpha;
            }
            if (appearance.UseScale)
            {
                info.Scales[(int)state] = appearance.Scale;
            }

            return info;
        }


        /// <summary>
        /// Change the visual appearance for a state flag being turned on.
        /// </summary>
        /// <param name="state">The state being turned on.</param>
        /// <param name="immediate">Whether to change the appearance immediately, or play the transition.</param>
        protected override void TransitionOn(InteractState state, bool immediate)
        {
            // Debug.Log("<color=cyan>" + name + " TransitionOn " + state + " immediate: " + immediate + "</color>");

            if (_style == null) return;
            
            immediate |= _style.TransitionTime == 0;

            int stateIndex = (int)state;

            List<ComponentInfo> infos = _stateComponents[stateIndex];

            if (infos == null) return;

            bool useColor = false;
            bool useAlpha = false;

            for (int i = 0; i < infos.Count; i++)
            {
                ComponentInfo info = infos[i];

                useColor = (info.Colors[stateIndex] != null) && !HasHigherNonNullValue(info.Colors, stateIndex);
                useAlpha = (info.Alphas[stateIndex] != null) && !HasHigherNonNullValue(info.Alphas, stateIndex);

                if (useColor && useAlpha)
                {
                    SetComponentColorAndAlpha(info, info.Colors[stateIndex].Value, info.Alphas[stateIndex].Value, immediate);
                }
                else if (useColor)
                {
                    SetComponentColor(info, info.Colors[stateIndex].Value, immediate);
                }
                else if (useAlpha)
                {
                    SetComponentAlpha(info, info.Alphas[stateIndex].Value, immediate);
                }

                if ((info.Scales[stateIndex] != null) && !HasHigherNonNullValue(info.Scales, stateIndex))
                {
                    SetComponentScale(info, info.Scales[stateIndex].Value, immediate);
                }
            }
        }

        /// <summary>
        /// Change the visual appearance for a state flag being turned off.
        /// </summary>
        /// <param name="state">The state being turned off.</param>
        /// <param name="immediate">Whether to change the appearance immediately, or play the transition.</param>
        ///  
        protected override void TransitionOff(InteractState state, bool immediate)
        {
            // Debug.Log("<color=cyan>" + name + " TransitionOff " + state + " immediate: " + immediate + "</color>");

            if (_style == null) return;
            
            if (state == InteractState.Normal) return;

            immediate |= _style.TransitionTime == 0;

            int stateIndex = (int)state;

            // Debug.Log("<color=cyan>" + name + " TransitionOff " + state + " immediate: " + immediate + "</color>");

            List<ComponentInfo> infos = _stateComponents[stateIndex];

            if (infos == null) return;

            for (int i = 0; i < infos.Count; i++)
            {
                ComponentInfo info = infos[i];

                int colorIndex = -1;
                int alphaIndex = -1;

                if ((info.Colors[stateIndex] != null) && !HasHigherNonNullValue(info.Colors, stateIndex))
                {
                    for (int j = stateIndex - 1; j >= 0; j--)
                    {
                        if ((info.Colors[j] == null) || !_stateFlags[j]) continue;
                        // Debug.Log("<color=lime>Reverting color to " + (InteractState)j + "</color>");
                        colorIndex = j;
                        break;
                    }
                }
                if ((info.Alphas[stateIndex] != null) && !HasHigherNonNullValue(info.Alphas, stateIndex))
                {
                    for (int j = stateIndex - 1; j >= 0; j--)
                    {
                        if ((info.Alphas[j] == null) || !_stateFlags[j]) continue;
                        // Debug.Log("<color=lime>Reverting alpha to " + (TweenSelectableState)j + "</color>");
                        alphaIndex = j;
                        break;
                    }
                }

                if ((colorIndex >= 0) && (alphaIndex >= 0))
                {
                    // Debug.Log("<color lime>Reverting color to " + (InteractState)colorIndex + " and alpha to " + (InteractState)alphaIndex + "</color>");
                    SetComponentColorAndAlpha(info, info.Colors[colorIndex].Value, info.Alphas[alphaIndex].Value, immediate);
                }
                else if (colorIndex >= 0)
                {
                    // Debug.Log("<color=lime>Reverting color to " + (InteractState)colorIndex + "</color>");
                    SetComponentColor(info, info.Colors[colorIndex].Value, immediate);
                }
                else if (alphaIndex >= 0)
                {
                    // Debug.Log("<color=lime>Reverting alpha to " + (InteractState)alphaIndex + "</color>");
                    SetComponentAlpha(info, info.Alphas[alphaIndex].Value, immediate);
                }

                if ((info.Scales[stateIndex] != null) && !HasHigherNonNullValue(info.Scales, stateIndex))
                {
                    for (int j = stateIndex - 1; j >= 0; j--)
                    {
                        if ((info.Scales[j] == null) || !_stateFlags[j]) continue;
                        SetComponentScale(info, info.Scales[j].Value, immediate);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Check to see if there's a non-null value that is higher in the array than the goal position.
        /// </summary>
        /// <typeparam name="T">A nullable type.</typeparam>
        /// <param name="values">An array of values that may or may not be null.</param>
        /// <param name="position">The position to check above.</param>
        /// <returns></returns>
        private bool HasHigherNonNullValue<T>(T[] values, int position)
        {
            for (int i = _stateFlags.Length - 1; i > position; i--)
            {
                if (_stateFlags[i] && (values[i] != null))
                {
                    // Debug.Log("<color=magenta>Overriden by " + ((TweenSelectableState)j) + "</color>");
                    return true;
                }
            }
            return false;           
        }

        private void SetComponentColorAndAlpha(ComponentInfo info, Color color, float alpha, bool immediate)
        {
            // Debug.Log("<color=orange>SetComponentColorAndAlpha " + info.Component.name + " color: " + color + " alpha: " + alpha + " immediate: " + immediate + "</color>");
            
#if UNITY_EDITOR
            if (info?.Graphic == null)
            {
                Debug.LogError("ColorAndScaleTransitioner.SetComponentColorAndAlpha: Component " + info?.Component?.name + " is missing a Graphic component.");
                return;
            }
#endif
            
            color.a = alpha;

            if (immediate)
            {
                // info.Renderer.SetColor(color);
                info.Graphic.CrossFadeColor(color, 0, true, true, true);
            }
            else
            {
                info.Graphic.CrossFadeColor(color, _style.TransitionTime, true, true, true);
            }
        }

        private void SetComponentColor(ComponentInfo info, Color color, bool immediate)
        {
            // Debug.Log("<color=orange>SetComponentColor " + info.Component.name + " color: " + color + " immediate: " + immediate + "</color>");

#if UNITY_EDITOR
            if (info?.Graphic == null)
            {
                Debug.LogError("ColorAndScaleTransitioner.SetComponentColor: Component " + info?.Component?.name + " is missing a Graphic component.");
                return;
            }
#endif
            if (immediate)
            {
                // color.a = info.Renderer.GetAlpha(); 
                // info.Renderer.SetColor(color);
                // info.Graphic.CrossFadeColor(color, 0, true, false, true);
                info.Graphic.CrossFadeColor(color, 0, true, false, true);
            }
            else
            {
                info.Graphic.CrossFadeColor(color, _style.TransitionTime, true, false, true);
            }
        }

        private void SetComponentAlpha(ComponentInfo info, float alpha, bool immediate)
        {
            // Debug.Log("<color=orange>SetComponentAlpha " + info.Component.name + " alpha: " + alpha + " immediate: " + immediate + "</color>");

#if UNITY_EDITOR
            if (info?.Graphic == null)
            {
                Debug.LogError("ColorAndScaleTransitioner.SetComponentAlpha: Component " + info?.Component?.name + " is missing a Graphic component.");
                return;
            }
#endif
            
            if (immediate)
            {
                // info.Renderer.SetAlpha(alpha);
                info.Graphic.CrossFadeAlpha(alpha, 0, true);
            }
            else
            {
                info.Graphic.CrossFadeAlpha(alpha, _style.TransitionTime, true);
            }
        }

        private void SetComponentScale(ComponentInfo info, Vector2 scale, bool immediate)
        {
            if (immediate)
            {
                info.Component.localScale = (Vector3)scale + Vector3.forward;
            }
            else
            {
                StartScaleTween(info, scale);
            }
        }

        private RectTransform FindComponent(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return transform as RectTransform;
            }
            
            RectTransform component = transform.Find(path) as RectTransform;

            return component;
        }

        protected override void InitializeStates()
        {
            if (_style == null)
            {
                Debug.LogWarning("TweenSelectable " + name + " is missing a Tween Selectable Style.");
            }

            if (_style == null) return;
            
            _componentInfo.Clear();
            
            ProcessStateAppearance(InteractState.Normal, _style.NormalState);

#if INTERACT_DESKTOP
            ProcessStateAppearance(InteractState.Highlighted, _style.HighlightedState);
#endif        
            ProcessStateAppearance(InteractState.Selected, _style.SelectedState);
#if !INTERACT_MOBILE
            ProcessStateAppearance(InteractState.Isolated, _style.IsolatedState);
#endif
            ProcessStateAppearance(InteractState.Pressed, _style.PressedState);
            ProcessStateAppearance(InteractState.Disabled, _style.DisabledState);
        }

        protected override void ClearAllComponents()
        {
            
            CanvasRenderer[] renderers = GetComponentsInChildren<CanvasRenderer>();

            for (int i = 0; i < renderers.Length; i++)
            {
                CanvasRenderer child = renderers[i];
                child.SetColor(Color.white);
                child.SetAlpha(1);
                child.transform.localScale = Vector3.one;
            }
        }

        
        protected override void ForceAppearance(InteractState state)
        {
            if (_style == null) return;
            ColorAndScaleStyleEntry[] appearances = null;
            switch (state)
            {
                case InteractState.Normal:
                    appearances = _style.NormalState;
                    break;
                case InteractState.Highlighted:
                    appearances = _style.HighlightedState;
                    break;
                case InteractState.Selected:
                    appearances = _style.SelectedState;
                    break;
                case InteractState.Pressed:
                    appearances = _style.PressedState;
                    break;
                case InteractState.Disabled:
                    appearances = _style.DisabledState;
                    break;
            }

            if ((appearances == null) || (appearances.Length == 0)) return;

            for (int i = 0; i < appearances.Length; i++)
            {
                ColorAndScaleStyleEntry appearance = appearances[i];
                Component component = FindComponent(appearance.ComponentPath);
                if (component == null) continue;
                if (appearance.UseColor && appearance.UseAlpha)
                {
                    Color color = appearance.Color;
                    color.a = appearance.Alpha;
                    CanvasRenderer cr = component.GetComponent<CanvasRenderer>();
                    if (cr == null) continue;
                    cr.SetColor(color);
                }
                else if (appearance.UseColor)
                {
                    CanvasRenderer cr = component.GetComponent<CanvasRenderer>();
                    if (cr == null) continue;
                    Color color = appearance.Color;
                    color.a = cr.GetAlpha();
                    cr.SetColor(color);
                }
                else if (appearance.UseAlpha)
                {
                    CanvasRenderer cr = component.GetComponent<CanvasRenderer>();
                    if (cr == null) continue;
                    cr.SetAlpha(appearance.Alpha);
                }
                if (appearance.UseScale)
                {
                    component.transform.localScale = appearance.Scale;
                }
            }
        }

        private void StartScaleTween(ComponentInfo info, Vector2 scale)
        {
            ScaleTween scaleTween = null;

            if (_activeScaleTweens == null)
            {
                _activeScaleTweens = new List<ScaleTween>();
            }
            else
            {
                for (int i = 0; i < _activeScaleTweens.Count; i++)
                {
                    if (_activeScaleTweens[i].Component == info.Component)
                    {
                        // We already have a scale tween running for this component.  Update it with the new information.
                        _activeScaleTweens[i].Set(info.Component, scale);
                        return;
                    }              
                }
            }

            scaleTween = (_scaleTweenPool.Count > 0) ? _scaleTweenPool.Pop() : new ScaleTween();
            scaleTween.Set(info.Component, scale);
            _activeScaleTweens.Add(scaleTween);
            if (_activeScaleTweens.Count == 1)
            {
                StartCoroutine(RunScaleTweens());
            }
        }

        private IEnumerator RunScaleTweens()
        {
            do
            {
                for (int i = _activeScaleTweens.Count - 1; i >= 0; i--)
                {
                    ScaleTween scaleTween = _activeScaleTweens[i];
                    float interval = Time.time - scaleTween.StartTime;
                    if (interval > _style.TransitionTime)
                    {
                        // This tween is done.
                        scaleTween.Component.localScale = scaleTween.EndScale;
                        _activeScaleTweens.RemoveAt(i);
                        _scaleTweenPool.Push(scaleTween);
                        continue;
                    }
                    // This tween is not done.  Advance the animation.
                    scaleTween.Component.localScale = Vector3.Lerp(scaleTween.StartScale, scaleTween.EndScale, interval / _style.TransitionTime);
                }
                yield return null;
            }
            while (_activeScaleTweens.Count > 0);
        }


    }
}
