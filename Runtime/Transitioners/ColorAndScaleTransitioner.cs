#if UNITY_IOS || UNITY_ANDROID
#define INTERACT_MOBILE
#elif UNITY_STANDALONE || UNITY_WEBGL
#define INTERACT_DESKTOP
#else
#define INTERACT_CONSOLE
#endif

using System.Collections.Generic;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

namespace HiddenAchievement.CrossguardUi
{
	/// <summary>
	/// Handles the transition logic for tweening between Interact states for a UI component.
	/// </summary>
    [ExecuteAlways]
	public class ColorAndScaleTransitioner : AbstractTransitioner
    {
        private class ComponentInfo
        {
            public string Path = string.Empty;
            public RectTransform Component;
            public CanvasRenderer Renderer;
            public readonly Color?[] Colors = new Color?[(int)InteractState.Count];
            public readonly float?[] Alphas = new float?[(int)InteractState.Count];
            public readonly Vector2?[] Scales = new Vector2?[(int)InteractState.Count];
        }

        [SerializeField]
        [Tooltip("Assign the scriptable object containing this element's style.")]
        private ColorAndScaleStyle _style;

        private readonly List<ComponentInfo>[] _stateComponents = new List<ComponentInfo>[(int)InteractState.Count];
        private readonly List<ComponentInfo> _componentInfo = new(2);

        #region Public Interface
        
        /// <inheritdoc />
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
                _stateComponents[(int)state] = new List<ComponentInfo>(appearance.Length);
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
                Transform component = UiUtilities.FindComponent(transform, appearance.ComponentPath);
                if (component == null) return null;

                info = new ComponentInfo
                {
                    Path = appearance.ComponentPath,
                    Component = (RectTransform)component,
                    Renderer = component.GetComponent<CanvasRenderer>(),
                    // Graphic = component.GetComponent<Graphic>()
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


        /// <inheritdoc />
        protected override void TransitionOn(InteractState state, bool immediate)
        {
            // Debug.Log("<color=cyan>" + name + " TransitionOn " + state + " immediate: " + immediate + "</color>");

            if (_style == null) return;
            
            immediate |= _style.TransitionTime == 0;

            int stateIndex = (int)state;

            List<ComponentInfo> infos = _stateComponents[stateIndex];

            if (infos == null) return;
            
            for (int i = 0; i < infos.Count; i++)
            {
                ComponentInfo info = infos[i];

                bool useColor = (info.Colors[stateIndex] != null) && !HasHigherNonNullValue(info.Colors, stateIndex);
                bool useAlpha = (info.Alphas[stateIndex] != null) && !HasHigherNonNullValue(info.Alphas, stateIndex);

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

        /// <inheritdoc />
        protected override void TransitionOff(InteractState state, bool immediate)
        {
            // Debug.Log("<color=cyan>" + name + " TransitionOff " + state + " immediate: " + immediate + "</color>");

            if (_style == null) return;
            
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
                    Debug.Assert(info.Colors[colorIndex].HasValue);
                    Debug.Assert(info.Alphas[alphaIndex].HasValue);
                    SetComponentColorAndAlpha(info, info.Colors[colorIndex].Value, info.Alphas[alphaIndex].Value, immediate);
                }
                else if (colorIndex >= 0)
                {
                    // Debug.Log("<color=lime>Reverting color to " + (InteractState)colorIndex + "</color>");
                    Debug.Assert(info.Colors[colorIndex].HasValue);
                    SetComponentColor(info, info.Colors[colorIndex].Value, immediate);
                }
                else if (alphaIndex >= 0)
                {
                    // Debug.Log("<color=lime>Reverting alpha to " + (InteractState)alphaIndex + "</color>");
                    Debug.Assert(info.Alphas[alphaIndex].HasValue);
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
            if (info?.Renderer == null)
            {
                Debug.LogError($"ColorAndScaleTransitioner.SetComponentColorAndAlpha: Component {info?.Component.name} is missing a Graphic component.");
                return;
            }
#endif
            
            color.a = alpha;

            if (immediate)
            {
                info.Renderer.SetColor(color);
            }
            else
            {
                LMotion.Create(info.Renderer.GetColor(), color, _style.TransitionTime)
                    .BindToColor(info.Renderer)
                    .AddTo(info.Renderer.gameObject);
            }
        }

        private void SetComponentColor(ComponentInfo info, Color color, bool immediate)
        {
            // Debug.Log("<color=orange>SetComponentColor " + info.Component.name + " color: " + color + " immediate: " + immediate + "</color>");

#if UNITY_EDITOR
            if (info?.Renderer == null)
            {
                Debug.LogError("ColorAndScaleTransitioner.SetComponentColor: Component " + info?.Component.name + " is missing a Graphic component.");
                return;
            }
#endif
            if (immediate)
            {
                color.a = info.Renderer.GetColor().a;
                info.Renderer.SetColor(color);
            }
            else
            {
                LMotion.Create(info.Renderer.GetColor(), color, _style.TransitionTime)
                    .BindToColorNoAlpha(info.Renderer)
                    .AddTo(info.Renderer.gameObject);;
            }
        }

        private void SetComponentAlpha(ComponentInfo info, float alpha, bool immediate)
        {
            // Debug.Log("<color=orange>SetComponentAlpha " + info.Component.name + " alpha: " + alpha + " immediate: " + immediate + "</color>");

#if UNITY_EDITOR
            if (info?.Renderer == null)
            {
                Debug.LogError("ColorAndScaleTransitioner.SetComponentAlpha: Component " + info?.Component.name + " is missing a Graphic component.");
                return;
            }
#endif
            if (immediate)
            {
                // info.Renderer.SetAlpha(alpha);
                Color color = info.Renderer.GetColor();
                color.a = alpha;
                info.Renderer.SetColor(color);
            }
            else
            {
                LMotion.Create(info.Renderer.GetColor().a, alpha, _style.TransitionTime)
                    .BindToColorA(info.Renderer)
                    .AddTo(info.Renderer.gameObject);;
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
                LMotion.Create((Vector2)info.Component.localScale, scale, _style.TransitionTime)
                    .BindToLocalScaleXY(info.Component)
                    .AddTo(info.Component.gameObject);;
            }
        }
        
        /// <inheritdoc />
        protected override void InitializeStates()
        {
            if (_style == null)
            {
                Debug.LogWarning($"ColorAndScaleTransitioner {name} is missing a Style.", gameObject);
                return;
            }
            
            _componentInfo.Clear();
            
            ProcessStateAppearance(InteractState.Normal, _style.NormalState);

#if INTERACT_DESKTOP
            ProcessStateAppearance(InteractState.Highlighted, _style.HighlightedState);
#endif        
            ProcessStateAppearance(InteractState.Selected, _style.SelectedState);
#if !INTERACT_MOBILE
            ProcessStateAppearance(InteractState.Isolated, _style.IsolatedState);
#endif
            ProcessStateAppearance(InteractState.Checked, _style.CheckedState);
            ProcessStateAppearance(InteractState.Pressed, _style.PressedState);
            ProcessStateAppearance(InteractState.Disabled, _style.DisabledState);
        }

        /// <inheritdoc />
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
        
        /// <inheritdoc />
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
                case InteractState.Checked:
                    appearances = _style.CheckedState;
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
                Component component = UiUtilities.FindComponent(transform, appearance.ComponentPath);
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
    }
}
