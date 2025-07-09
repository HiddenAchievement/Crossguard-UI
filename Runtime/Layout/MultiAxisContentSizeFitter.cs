using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// A modified version of ContentSizeFitter that allows a container to expand in
    /// two directions.
    /// </summary>
    [AddComponentMenu("Layout/Multi-Axis Content Size Fitter")]
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class MultiAxisContentSizeFitter : UIBehaviour, ILayoutSelfController
    {
         public enum FitMode
        {
            Unconstrained,
            MinSize,
            PreferredSize
        }

        [SerializeField]
        protected FitMode _horizontalFit = FitMode.Unconstrained;

        public FitMode HorizontalFit
        {
            get => _horizontalFit;
            set { if (SetStruct(ref _horizontalFit, value)) SetDirty(); }
        }

        [SerializeField]
        protected FitMode _verticalFit = FitMode.Unconstrained;
        public FitMode VerticalFit
        {
            get => _verticalFit;
            set { if (SetStruct(ref _verticalFit, value)) SetDirty(); }
        }

        [SerializeField]
        protected float m_maxWidth = 0;
        public float MaxWidth
        {
            get => m_maxWidth;
            set { if (SetStruct(ref m_maxWidth, value)) SetDirty(); }
        }

        [SerializeField]
        protected float m_maxHeight = 0;
        public float MaxHeight
        {
            get => m_maxHeight;
            set { if (SetStruct(ref m_maxHeight, value)) SetDirty(); }
        }

        [System.NonSerialized]
        private RectTransform _rt;
        private RectTransform _rectTransform
        {
            get
            {
                if (_rt == null)
                {
                    _rt = GetComponent<RectTransform>();
                }
                return _rt;
            }
        }

        private DrivenRectTransformTracker _tracker;

        protected MultiAxisContentSizeFitter()
        { }

        #region Unity Lifetime calls

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        protected override void OnDisable()
        {
            _tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(_rectTransform);
            base.OnDisable();
        }

        #endregion

        protected override void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }

        private void HandleSelfFittingAlongAxis(int axis)
        {
            FitMode fitting = (axis == 0 ? HorizontalFit : VerticalFit);

            if (fitting == FitMode.Unconstrained)
            {
                // Keep a reference to the tracked transform, but don't control its properties:
                _tracker.Add(this, _rectTransform, DrivenTransformProperties.None);
                return;
            }

            float max = (axis == 0 ? m_maxWidth : m_maxHeight);
            max = (max == 0 ? float.MaxValue : max);

            _tracker.Add(this, _rectTransform, (axis == 0 ? DrivenTransformProperties.SizeDeltaX : DrivenTransformProperties.SizeDeltaY));

            // Set size to min or preferred size
            if (fitting == FitMode.MinSize)
                _rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, Mathf.Min(LayoutUtility.GetMinSize(_rt, axis), max));
            else
                _rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, Mathf.Min(LayoutUtility.GetPreferredSize(_rt, axis), max));
        }

        public virtual void SetLayoutHorizontal()
        {
            _tracker.Clear();
            HandleSelfFittingAlongAxis(0);
        }

        public virtual void SetLayoutVertical()
        {
            HandleSelfFittingAlongAxis(1);
        }

        protected void SetDirty()
        {
            if (!IsActive()) return;

            LayoutRebuilder.MarkLayoutForRebuild(_rectTransform);
        }

        public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            if (currentValue.Equals(newValue)) return false;

            currentValue = newValue;
            return true;
        }

    #if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }
    #endif
    }
}