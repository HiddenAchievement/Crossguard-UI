using System.Collections;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// A standard slide-toggle.
    /// </summary>
    public class CrossSlideToggle : CrossToggleBase
    {
        [SerializeField]
        private RectTransform _fill = null;
        public RectTransform Fill
        {
            get => _fill;
            set => _fill = value;
        }
        
        
        [SerializeField]
        private float _slideTime = .15f;

        protected override void PlayEffect(bool instant)
        {
            if (_fill == null) return;

            StopAllCoroutines();
            
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Vector2 anchorMax = _fill.anchorMax;
                anchorMax.x = IsOn ? 1 : 0;
                _fill.anchorMax = anchorMax;
            }
            else
#endif
            if (IsOn)
            {
                StartCoroutine(SlideRight());
            }
            else
            {
                StartCoroutine(SlideLeft());
            }
        }

        protected virtual IEnumerator SlideRight()
        {
            Vector2 anchorMax = _fill.anchorMax;
            while ((anchorMax.x < 1f) && !Mathf.Approximately(anchorMax.x, 1f))
            {
                anchorMax.x += Time.unscaledDeltaTime / _slideTime;
                _fill.anchorMax = anchorMax;
                yield return null;
            }
        }

        protected virtual IEnumerator SlideLeft()
        {
            Vector2 anchorMax = _fill.anchorMax;
            while ((anchorMax.x > 0) && !Mathf.Approximately(anchorMax.x, 0))
            {
                anchorMax.x -= Time.unscaledDeltaTime / _slideTime;
                _fill.anchorMax = anchorMax;
                yield return null;
            }
        }
    }
}
