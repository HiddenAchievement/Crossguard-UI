using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// Add this to a container object, when you want it to shrink to fit its contents (if it either has a content
    /// size fitter on it, or it's inside a Layout Group in the hierarchy). One of the primary use cases for this is
    /// buttons that resize to fit the text inside of them.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class ShrinkwrapLayoutElement : UIBehaviour, ILayoutElement, ILayoutGroup
    {
        [SerializeField]
        [Tooltip("The padding to place around the content.")]
        private RectOffset _padding;

        [SerializeField]
        [Tooltip("Whether to shrinkwrap along the vertical axis.")]
        private bool _fitVertical;

        [SerializeField]
        [Tooltip("Whether to shrinkwrap along the horizontal axis.")]
        private bool _fitHorizontal;

        [SerializeField]
        [Tooltip("The content we're fitting this element to, typically a child of this element.")]
        private RectTransform _content;

        [SerializeField]
        private TextAnchor _contentAlignment = TextAnchor.MiddleCenter;
        
        private DrivenRectTransformTracker _tracker;
        
        private Vector2 _contentMinSize = Vector2.zero;
        private Vector2 _contentPreferredSize = Vector2.zero;
        private Vector2 _contentFlexibleSize = Vector2.zero;
        
        private Vector2 _totalMinSize = Vector2.zero;
        private Vector2 _totalPreferredSize = Vector2.zero;
        private Vector2 _totalFlexibleSize = Vector2.zero;

        private RectTransform _rectTransform;
        
        public void CalculateLayoutInputHorizontal()
        {
            _tracker.Clear();
            CalcAlongAxis(0);
        }

        public void CalculateLayoutInputVertical()
        {
            CalcAlongAxis(1);
        }

        private void CalcAlongAxis(int axis)
        {
            float combinedPadding = axis == 0 ? _padding.horizontal : _padding.vertical;

            _contentMinSize[axis] = LayoutUtility.GetMinSize(_content, axis);
            _contentPreferredSize[axis] = LayoutUtility.GetPreferredSize(_content, axis);
            _contentFlexibleSize[axis] = LayoutUtility.GetFlexibleSize(_content, axis);
            
            _totalMinSize[axis] = combinedPadding + _contentMinSize[axis];
            _totalPreferredSize[axis] = Mathf.Max(_totalMinSize[axis], combinedPadding + _contentPreferredSize[axis]);
            _totalFlexibleSize[axis] = _contentFlexibleSize[axis];
        }

        public float minWidth => _totalMinSize[0];
        public float preferredWidth => _totalPreferredSize[0];
        public float flexibleWidth => _totalFlexibleSize[0];
        
        public float minHeight => _totalMinSize[1];
        public float preferredHeight => _totalPreferredSize[1];
        public float flexibleHeight => _totalFlexibleSize[1];

        public int layoutPriority => 0;
        
        public void SetLayoutHorizontal()
        {
            SetContentAlongAxis(0);
        }

        public void SetLayoutVertical()
        {
            SetContentAlongAxis(1);
        }

        private void SetContentAlongAxis(int axis)
        {
            if (_rectTransform == null)
            {
                _rectTransform = (RectTransform)transform;
            }
            
            float size = _rectTransform.rect.size[axis];
            float innerSize = size - (axis == 0 ? _padding.horizontal : _padding.vertical);
            float requiredSpace = Mathf.Clamp(innerSize, _contentMinSize[axis], _contentFlexibleSize[axis] > 0 ? size : _contentPreferredSize[axis]);
            requiredSpace += axis == 0 ? _padding.horizontal : _padding.vertical;
            float availableSpace  = _rectTransform.rect.size[axis];
            float surplusSpace = availableSpace - requiredSpace;
            // ReSharper disable once PossibleLossOfFraction
            float alignmentOnAxis = axis == 0 ? ((int)_contentAlignment % 3) * 0.5f : ((int)_contentAlignment / 3) * 0.5f;
            float startOffset = (axis == 0 ? _padding.left : _padding.top) + surplusSpace * alignmentOnAxis;
            //if (axis == 1)
            { 
                Debug.Log($"axis: {axis} size: {size} innerSize: {innerSize} requiredSpace: {requiredSpace} availableSpace: {availableSpace} surplusSpace: {surplusSpace} alignmentOnAxis: {alignmentOnAxis} startOffset: {startOffset}");
            }
            
            _tracker.Add(this, _content,
                DrivenTransformProperties.Anchors |
                (axis == 0 ?
                    (DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.SizeDeltaX) :
                    (DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.SizeDeltaY)
                )
            );
            
            _content.anchorMin = Vector2.up;
            _content.anchorMax = Vector2.up;

            Vector2 sizeDelta = _content.sizeDelta;
            sizeDelta[axis]    = requiredSpace;
            _content.sizeDelta = sizeDelta;

            Vector2 anchoredPosition = _content.anchoredPosition;
            anchoredPosition[axis] = (axis == 0) ? (startOffset + requiredSpace * _content.pivot[axis]) : (-startOffset - requiredSpace * (1f - _content.pivot[axis]));
            _content.anchoredPosition = anchoredPosition;
        }
        
#if UNITY_EDITOR
        private Vector2 _contentSize;
        private void Update()
        {
            if (Application.isPlaying)
                return;
            
            // If children size change in editor, update layout (case 945680 - Child GameObjects in a Horizontal/Vertical Layout Group don't display their correct position in the Editor)
            bool dirty = false;
            
            if (_content != null && _content.sizeDelta != _contentSize)
            {
                dirty = true;
                _contentSize = _content.sizeDelta;
            }
            if (dirty)
            {
                LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
            }
        }
#endif
    }
}