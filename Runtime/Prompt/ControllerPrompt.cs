using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// 
    /// </summary>
    public class ControllerPrompt : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The name for the semantic control this prompt is associated with.")]
        private string _actionName = string.Empty;
        public string ActionName => _actionName;

        [SerializeField]
        [Tooltip("The control layer that must be active for this prompt to be active.")]
        private string _layer = string.Empty;
        public string Layer => _layer;

        [SerializeField]
        [Tooltip("Optional selectable to forward button presses to.")]
        private Selectable _selectable = null;

        public Selectable Selectable
        {
            get => _selectable;
            set => _selectable = value;
        }

        [SerializeField]
        [Tooltip("The text element which should display the controller art.")]
        private TextMeshProUGUI _display = null;

        private PointerEventData _pointerEventData;
        private static Vector3[] _corners = new Vector3[4];
        
        private void Start()
        {
            ControllerPromptManager.Instance.RegisterControllerPrompt(this);
            if (_pointerEventData == null)
            {
                _pointerEventData = new PointerEventData(EventSystem.current);
            }
        }

        private void OnDestroy()
        {
            ControllerPromptManager.Instance.UnregisterControllerPrompt(this);
        }

        public void SetToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                gameObject.SetActive(false);
                return;
            }
            _display.text = token;
            gameObject.SetActive(true);
        }

        public void PressControl()
        {
            if ((_selectable == null) || !isActiveAndEnabled) return;
            // UpdatePointerEventData();
            ExecuteEvents.Execute(_selectable.gameObject, _pointerEventData, ExecuteEvents.pointerDownHandler);
        }

        public void ReleaseControl()
        {
            if ((_selectable == null) || !isActiveAndEnabled) return;
            // UpdatePointerEventData();
            ExecuteEvents.Execute(_selectable.gameObject, _pointerEventData, ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute(_selectable.gameObject, _pointerEventData, ExecuteEvents.pointerClickHandler);
        }

        /*
        private void UpdatePointerEventData()
        {
            // Click right in the middle of the selectable.
            RectTransform rectTransform = _selectable.transform as RectTransform;
            if (rectTransform == null) return;
            rectTransform.GetWorldCorners(_corners);
            Vector2 pos = Vector2.zero;
            pos.x = (_corners[0].x + _corners[2].x) * 0.5f;
            pos.y = (_corners[0].y + _corners[1].y) * 0.5f;
            _pointerEventData.position = pos;
        }
        */
    }
}
