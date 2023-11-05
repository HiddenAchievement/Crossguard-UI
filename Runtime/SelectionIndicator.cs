using System;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// 
    /// </summary>
    public class SelectionIndicator : MonoBehaviour
    {
        private static SelectionIndicator s_currentIndicator = null;
        private static RectTransform s_currentSelection = null;
        
        [SerializeField]
        [Tooltip("An element in this selection indicator which should be fitted to the target object.")]
        private RectTransform _sizer = null;

        public void OnEnable()
        {
            if (s_currentIndicator != this)
            {
                gameObject.SetActive(false);
            }

            if (_sizer == null)
            {
                _sizer = (RectTransform) transform;
            }
        }


        public void Deselect(RectTransform element)
        {
            if ((s_currentIndicator == this) && (s_currentSelection == element))
            {
                s_currentIndicator = null;
                s_currentSelection = null;
                gameObject.SetActive(false);
            }
        }
        
        public void Select(RectTransform element)
        {
            if (s_currentSelection == element) return;
            if ((s_currentIndicator != null) && (s_currentIndicator != this))
            {
                s_currentIndicator.gameObject.SetActive(false);
            }

            s_currentIndicator = this;
            s_currentSelection = element;

            UiUtilities.FitRectTransform(_sizer, element);
            
            gameObject.SetActive(true);
        }
        
    }
}
