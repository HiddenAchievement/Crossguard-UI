#if CROSS_REVAMP

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using LitMotion;
using LitMotion.Extensions;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// A Dropdown slightly customized to work with TweenTransitioner.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class CrossRevampDropdown : CrossSelectable, IPointerClickHandler, ISubmitHandler, ICancelHandler
    {
        protected class CrossDropdownItem : MonoBehaviour, ICancelHandler, ISelectHandler
        {
            [SerializeField]
            private TMP_Text _text;
            [SerializeField]
            private Image _image;
            [SerializeField]
            private RectTransform _rectTransform;
            [SerializeField]
            private CrossRevampToggle _crossToggle;

            public TMP_Text Text { get => _text; set => _text = value; }
            public Image Image { get => _image; set => _image = value;
            }
            public RectTransform RectTransform { get => _rectTransform; set => _rectTransform = value; }
            public CrossRevampToggle Toggle { get => _crossToggle; set => _crossToggle = value; }
            
            public CrossRevampDropdown Dropdown { get; set; }
            
            public void OnCancel(BaseEventData eventData)
            {
                if (Dropdown != null)
                {
                    Dropdown.Hide();
                }
            }

            public void OnSelect(BaseEventData eventData)
            {
                if (Dropdown == null) return;
                Dropdown.OnItemSelected(this);
            }
        }
        
        [Serializable]
        public class DropdownEvent : UnityEvent<int> { }

        // Template used to create the dropdown.
        [SerializeField]
        private RectTransform _template;

        /// <summary>
        /// The Rect Transform of the template for the dropdown list.
        /// </summary>
        public RectTransform Template { get => _template; set { _template = value; RefreshShownValue(); } }

        // Text to be used as a caption for the current value. It's not required, but it's kept here for convenience.
        [SerializeField]
        private TMP_Text _captionText;

        /// <summary>
        /// The Text component to hold the text of the currently selected option.
        /// </summary>
        public TMP_Text CaptionText { get => _captionText; set { _captionText = value; RefreshShownValue(); } }

        [SerializeField]
        private Image _captionImage;

        /// <summary>
        /// The Image component to hold the image of the currently selected option.
        /// </summary>
        public Image CaptionImage { get => _captionImage; set { _captionImage = value; RefreshShownValue(); } }

        [SerializeField]
        private Graphic _placeHolder;

        /// <summary>
        /// The placeholder Graphic component. Shown when no option is selected.
        /// </summary>
        public Graphic PlaceHolder { get => _placeHolder; set { _placeHolder = value; RefreshShownValue(); } }

        [Space]

        [SerializeField]
        private TMP_Text _itemText;

        /// <summary>
        /// The Text component to hold the text of the item.
        /// </summary>
        public TMP_Text ItemText { get => _itemText; set { _itemText = value; RefreshShownValue(); } }

        [SerializeField]
        private Image _itemImage;

        /// <summary>
        /// The Image component to hold the image of the item
        /// </summary>
        public Image ItemImage { get => _itemImage; set { _itemImage = value; RefreshShownValue(); } }

        [Space]

        [SerializeField]
        private int _value;
        public int Value { get => _value; set => SetValue(value); }

        [Space]

        // Items that will be visible when the dropdown is shown.
        // We box this into its own class so we can use a Property Drawer for it.
        [SerializeField]
        private TMP_Dropdown.OptionDataList _options = new TMP_Dropdown.OptionDataList();

        public List<TMP_Dropdown.OptionData> Options
        {
            get => _options.options;
            set { _options.options = value; RefreshShownValue(); }
        }

        [Space]

        // Notification triggered when the dropdown changes.
        [SerializeField]
        private DropdownEvent _onValueChanged = new();
        
        public DropdownEvent OnValueChanged { get => _onValueChanged; set => _onValueChanged = value; }

        [SerializeField]
        private UnityEvent _onDropdownOpened = new();
        
        public UnityEvent OnDropdownOpened { get => _onDropdownOpened; set => _onDropdownOpened = value; }
        
        [SerializeField]
        private float _alphaFadeSpeed = 0.15f;

        /// <summary>
        /// The time interval at which a drop down will appear and disappear
        /// </summary>
        public float AlphaFadeSpeed { get => _alphaFadeSpeed; set => _alphaFadeSpeed = value;
        }

        private GameObject _dropdown;
        private GameObject _blocker;
        private readonly List<CrossDropdownItem> _items = new();

        private bool _validTemplate;
        private Coroutine _destroyCoroutine;

        private static readonly TMP_Dropdown.OptionData s_noOptionData = new();

        public bool IsExpanded => _dropdown != null;


        // ============================================================================================
        
        private ScrollRect _scrollRect;
        private CanvasGroup _listCanvasGroup;
        private Coroutine _fadeCoroutine;
        
        
        protected CrossRevampDropdown() {}

#if UNITY_EDITOR
        protected override void Reset()
        {
            // base.Reset();
            transition = Transition.None;
        }
#endif
        protected override void Awake()
        {
            // m_AlphaTweenRunner = new TweenRunner<FloatTween>();
            // m_AlphaTweenRunner.Init(this);

            if (_captionImage)
                _captionImage.enabled = (_captionImage.sprite != null);

            if (_template)
                _template.gameObject.SetActive(false);

            base.Awake();
        }
        
        protected override void Start()
        {
            base.Start();

            RefreshShownValue();
        }
        
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (!IsActive())
                return;

            RefreshShownValue();
        }
#endif
        
        protected override void OnDisable()
        {
            //Destroy dropdown and blocker in case user deactivates the dropdown when they click an option (case 935649)
            ImmediateDestroyDropdownList();

            if (_blocker != null)
                DestroyBlocker(_blocker);

            _blocker = null;

            base.OnDisable();
        }
        
        /// <summary>
        /// Handling for when the dropdown is initially 'clicked'. Typically shows the dropdown
        /// </summary>
        /// <param name="eventData">The associated event data.</param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!interactable) return;
            Show();
            EnsureSelectedItemVisible();
        }
        
        /// <summary>
        /// Handling for when the dropdown is selected and a submit event is processed. Typically shows the dropdown
        /// </summary>
        /// <param name="eventData">The associated event data.</param>
        public void OnSubmit(BaseEventData eventData)
        {
            if (!interactable) return;
            Show();
            EnsureSelectedItemVisible();
        }
        
        /// <summary>
        /// This will hide the dropdown list.
        /// </summary>
        /// <remarks>
        /// Called by a BaseInputModule when a Cancel event occurs.
        /// </remarks>
        /// <param name="eventData">The associated event data.</param>
        public virtual void OnCancel(BaseEventData eventData)
        {
            Hide();
        }

        protected virtual void OnItemSelected(CrossDropdownItem item)
        {
            if (_scrollRect == null)
            {
                _scrollRect = item.GetComponentInParent<ScrollRect>();
            }

            if (_scrollRect.content.rect.width > 0)
            {
                EnsureItemVisible(item);
            }
            else
            {
                StartCoroutine(EnsureItemVisibleEventually(item));
            }
        }
        
        protected void EnsureSelectedItemVisible()
        {
            GameObject go = EventSystem.current.currentSelectedGameObject;
            if (go == null) return;
            CrossDropdownItem item = go.GetComponent<CrossDropdownItem>();
            if (item == null) return;
            OnItemSelected(item);
        }
        
        
        protected virtual void EnsureItemVisible(CrossDropdownItem item)
        {
            UiUtilities.EnsureItemVisible(_scrollRect, (RectTransform)item.transform);
        }

        private IEnumerator EnsureItemVisibleEventually(CrossDropdownItem item)
        {
            while (_scrollRect.content.rect.width < 0)
            {
                yield return null;
            }
            EnsureItemVisible(item);
        }
        
        // ============================================================================================

        /// <summary>
        /// Set index number of the current selection in the Dropdown without invoking onValueChanged callback.
        /// </summary>
        /// <param name="input">The new index for the current selection.</param>
        public void SetValueWithoutNotify(int input)
        {
            SetValue(input, false);
        }

        void SetValue(int value, bool sendCallback = true)
        {
            if (Application.isPlaying && (value == _value || Options.Count == 0))
                return;

            _value = Mathf.Clamp(value, _placeHolder ? -1 : 0, Options.Count - 1);
            RefreshShownValue();

            if (sendCallback)
            {
                // Notify all listeners
                UISystemProfilerApi.AddMarker("Dropdown.value", this);
                _onValueChanged.Invoke(_value);
            }
        }
        
        /// <summary>
        /// Refreshes the text and image (if available) of the currently selected option.
        /// </summary>
        /// <remarks>
        /// If you have modified the list of options, you should call this method afterwards to ensure that the visual state of the dropdown corresponds to the updated options.
        /// </remarks>
        public void RefreshShownValue()
        {
            TMP_Dropdown.OptionData data = s_noOptionData;

            if (Options.Count > 0 && _value >= 0)
                data = Options[Mathf.Clamp(_value, 0, Options.Count - 1)];

            if (_captionText)
            {
                _captionText.text = data is { text: not null } ? data.text : "";
            }

            if (_captionImage)
            {
                _captionImage.sprite  = data?.image;
                _captionImage.enabled = (_captionImage.sprite != null);
            }

            if (_placeHolder)
            {
                _placeHolder.enabled = Options.Count == 0 || _value == -1;
            }
        }
        
        /// <summary>
        /// Add multiple options to the options of the Dropdown based on a list of OptionData objects.
        /// </summary>
        /// <param name="options">The list of OptionData to add.</param>
        /// <remarks>
        /// See AddOptions(List&lt;string&gt; options) for code example of usages.
        /// </remarks>
        public void AddOptions(List<TMP_Dropdown.OptionData> options)
        {
            this.Options.AddRange(options);
            RefreshShownValue();
        }

        public void AddOptions(List<string> options)
        {
            for (int i = 0; i < options.Count; i++)
                this.Options.Add(new TMP_Dropdown.OptionData(options[i]));

            RefreshShownValue();
        }
        
        /// <summary>
        /// Add multiple image-only options to the options of the Dropdown based on a list of Sprites.
        /// </summary>
        /// <param name="options">The list of Sprites to add.</param>
        /// <remarks>
        /// See AddOptions(List&lt;string&gt; options) for code example of usages.
        /// </remarks>
        public void AddOptions(List<Sprite> options)
        {
            for (int i = 0; i < options.Count; i++)
                this.Options.Add(new TMP_Dropdown.OptionData(options[i]));

            RefreshShownValue();
        }
        
        
        /// <summary>
        /// Clear the list of options in the Dropdown.
        /// </summary>
        public void ClearOptions()
        {
            Options.Clear();
            _value = _placeHolder ? -1 : 0;
            RefreshShownValue();
        }
        
        private void SetupTemplate()
        {
            _validTemplate = false;

            if (!_template)
            {
                Debug.LogError("The dropdown template is not assigned. The template needs to be assigned and must have a child GameObject with a Toggle component serving as the item.", this);
                return;
            }

            GameObject templateGo = _template.gameObject;
            templateGo.SetActive(true);
            CrossRevampToggle itemCrossToggle = _template.GetComponentInChildren<CrossRevampToggle>();

            _validTemplate = true;
            if (!itemCrossToggle || itemCrossToggle.transform == Template)
            {
                _validTemplate = false;
                Debug.LogError("The dropdown template is not valid. The template must have a child GameObject with a Toggle component serving as the item.", Template);
            }
            else if (!(itemCrossToggle.transform.parent is RectTransform))
            {
                _validTemplate = false;
                Debug.LogError("The dropdown template is not valid. The child GameObject with a Toggle component (the item) must have a RectTransform on its parent.", Template);
            }
            else if (ItemText != null && !ItemText.transform.IsChildOf(itemCrossToggle.transform))
            {
                _validTemplate = false;
                Debug.LogError("The dropdown template is not valid. The Item Text must be on the item GameObject or children of it.", Template);
            }
            else if (ItemImage != null && !ItemImage.transform.IsChildOf(itemCrossToggle.transform))
            {
                _validTemplate = false;
                Debug.LogError("The dropdown template is not valid. The Item Image must be on the item GameObject or children of it.", Template);
            }

            if (!_validTemplate)
            {
                templateGo.SetActive(false);
                return;
            }

            CrossDropdownItem item = itemCrossToggle.gameObject.AddComponent<CrossDropdownItem>();
            item.Text = _itemText;
            item.Image = _itemImage;
            item.Toggle = itemCrossToggle;
            item.RectTransform = (RectTransform)itemCrossToggle.transform;

            // Find the Canvas that this dropdown is a part of
            Canvas parentCanvas = null;
            Transform parentTransform = _template.parent;
            while (parentTransform != null)
            {
                parentCanvas = parentTransform.GetComponent<Canvas>();
                if (parentCanvas != null)
                    break;

                parentTransform = parentTransform.parent;
            }

            Canvas popupCanvas = GetOrAddComponent<Canvas>(templateGo);
            popupCanvas.overrideSorting = true;
            popupCanvas.sortingOrder = 30000;

            // If we have a parent canvas, apply the same raycasters as the parent for consistency.
            if (parentCanvas != null)
            {
                Component[] components = parentCanvas.GetComponents<BaseRaycaster>();
                for (int i = 0; i < components.Length; i++)
                {
                    Type raycasterType = components[i].GetType();
                    if (templateGo.GetComponent(raycasterType) == null)
                    {
                        templateGo.AddComponent(raycasterType);
                    }
                }
            }
            else
            {
                GetOrAddComponent<GraphicRaycaster>(templateGo);
            }
            
            GetOrAddComponent<CanvasGroup>(templateGo);
            templateGo.SetActive(false);

            _validTemplate = true;
        }
        
        private static T GetOrAddComponent<T>(GameObject go) where T : Component
        {
            T comp = go.GetComponent<T>();
            if (!comp)
                comp = go.AddComponent<T>();
            return comp;
        }
        
                /// <summary>
        /// Show the dropdown.
        ///
        /// Plan for dropdown scrolling to ensure dropdown is contained within screen.
        ///
        /// We assume the Canvas is the screen that the dropdown must be kept inside.
        /// This is always valid for screen space canvas modes.
        /// For world space canvases we don't know how it's used, but it could be e.g. for an in-game monitor.
        /// We consider it a fair constraint that the canvas must be big enough to contain dropdowns.
        /// </summary>
        public void Show()
        {
            if (_destroyCoroutine != null)
            {
                StopCoroutine(_destroyCoroutine);
                ImmediateDestroyDropdownList();
            }

            if (!IsActive() || !IsInteractable() || _dropdown != null)
                return;

            // Get root Canvas.
            var list = ListPool<Canvas>.Fetch();
            gameObject.GetComponentsInParent(false, list);
            if (list.Count == 0)
                return;

            Canvas rootCanvas = list[list.Count - 1];
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].isRootCanvas)
                {
                    rootCanvas = list[i];
                    break;
                }
            }

            ListPool<Canvas>.Return(list);

            if (!_validTemplate)
            {
                SetupTemplate();
                if (!_validTemplate)
                    return;
            }

            _template.gameObject.SetActive(true);

            // popupCanvas used to assume the root canvas had the default sorting Layer, next line fixes (case 958281 - [UI] Dropdown list does not copy the parent canvas layer when the panel is opened)
            _template.GetComponent<Canvas>().sortingLayerID = rootCanvas.sortingLayerID;

            // Instantiate the drop-down template
            _dropdown = CreateDropdownList(_template.gameObject);
            _dropdown.name = "Dropdown List";
            _dropdown.SetActive(true);

            // Make drop-down RectTransform have same values as original.
            RectTransform dropdownRectTransform = _dropdown.transform as RectTransform;
            Debug.Assert(dropdownRectTransform);
            dropdownRectTransform.SetParent(_template.transform.parent, false);

            // Get Canvas Group for animating.
            _listCanvasGroup = _dropdown.GetComponent<CanvasGroup>();
            
            // Instantiate the drop-down list items

            // Find the dropdown item and disable it.
            CrossDropdownItem itemTemplate = _dropdown.GetComponentInChildren<CrossDropdownItem>();

            GameObject content = itemTemplate.RectTransform.parent.gameObject;
            RectTransform contentRectTransform = content.transform as RectTransform;
            Debug.Assert(contentRectTransform);
            itemTemplate.RectTransform.gameObject.SetActive(true);

            // Get the rects of the dropdown and item
            Rect dropdownContentRect = contentRectTransform.rect;
            Rect itemTemplateRect = itemTemplate.RectTransform.rect;

            // Calculate the visual offset between the item's edges and the background's edges
            Vector2 offsetMin = itemTemplateRect.min - dropdownContentRect.min + (Vector2)itemTemplate.RectTransform.localPosition;
            Vector2 offsetMax = itemTemplateRect.max - dropdownContentRect.max + (Vector2)itemTemplate.RectTransform.localPosition;
            Vector2 itemSize = itemTemplateRect.size;

            _items.Clear();

            CrossRevampToggle prev = null;
            for (int i = 0; i < Options.Count; ++i)
            {
                TMP_Dropdown.OptionData data = Options[i];
                CrossDropdownItem item = AddItem(data, Value == i, itemTemplate, _items);
                if (item == null)
                    continue;

                // Automatically set up a toggle state change listener
                item.Toggle.IsOn = Value == i;
                item.Toggle.OnValueChanged.AddListener(x => OnSelectItem(item.Toggle));

                // Select current option
                if (item.Toggle.IsOn)
                    item.Toggle.Select();

                // Automatically set up explicit navigation
                if (prev != null)
                {
                    Navigation prevNav = prev.navigation;
                    Navigation toggleNav = item.Toggle.navigation;
                    prevNav.mode = Navigation.Mode.Explicit;
                    toggleNav.mode = Navigation.Mode.Explicit;

                    prevNav.selectOnDown = item.Toggle;
                    prevNav.selectOnRight = item.Toggle;
                    toggleNav.selectOnLeft = prev;
                    toggleNav.selectOnUp = prev;

                    prev.navigation = prevNav;
                    item.Toggle.navigation = toggleNav;
                }
                prev = item.Toggle;
            }

            // Reposition all items now that all of them have been added
            Vector2 sizeDelta = contentRectTransform.sizeDelta;
            sizeDelta.y = itemSize.y * _items.Count + offsetMin.y - offsetMax.y;
            contentRectTransform.sizeDelta = sizeDelta;

            float extraSpace = dropdownRectTransform.rect.height - contentRectTransform.rect.height;
            if (extraSpace > 0)
                dropdownRectTransform.sizeDelta = new Vector2(dropdownRectTransform.sizeDelta.x, dropdownRectTransform.sizeDelta.y - extraSpace);

            // Invert anchoring and position if dropdown is partially or fully outside of canvas rect.
            // Typically this will have the effect of placing the dropdown above the button instead of below,
            // but it works as inversion regardless of initial setup.
            Vector3[] corners = new Vector3[4];
            dropdownRectTransform.GetWorldCorners(corners);

            RectTransform rootCanvasRectTransform = rootCanvas.transform as RectTransform;
            Debug.Assert(rootCanvasRectTransform);
            Rect rootCanvasRect = rootCanvasRectTransform.rect;
            for (int axis = 0; axis < 2; axis++)
            {
                bool outside = false;
                for (int i = 0; i < 4; i++)
                {
                    Vector3 corner = rootCanvasRectTransform.InverseTransformPoint(corners[i]);
                    if ((corner[axis] < rootCanvasRect.min[axis] && !Mathf.Approximately(corner[axis], rootCanvasRect.min[axis])) ||
                        (corner[axis] > rootCanvasRect.max[axis] && !Mathf.Approximately(corner[axis], rootCanvasRect.max[axis])))
                    {
                        outside = true;
                        break;
                    }
                }
                if (outside)
                    RectTransformUtility.FlipLayoutOnAxis(dropdownRectTransform, axis, false, false);
            }

            for (int i = 0; i < _items.Count; i++)
            {
                RectTransform itemRect = _items[i].RectTransform;
                itemRect.anchorMin = new Vector2(itemRect.anchorMin.x, 0);
                itemRect.anchorMax = new Vector2(itemRect.anchorMax.x, 0);
                itemRect.anchoredPosition = new Vector2(itemRect.anchoredPosition.x, offsetMin.y + itemSize.y * (_items.Count - 1 - i) + itemSize.y * itemRect.pivot.y);
                itemRect.sizeDelta = new Vector2(itemRect.sizeDelta.x, itemSize.y);
            }

            // Fade in the popup
            AlphaFadeList(1f);

            // Make drop-down template and item template inactive
            _template.gameObject.SetActive(false);
            itemTemplate.gameObject.SetActive(false);

            _blocker = CreateBlocker(rootCanvas);

            OnDropdownOpened?.Invoke();
        }
        
        /// <summary>
        /// Create a blocker that blocks clicks to other controls while the dropdown list is open.
        /// </summary>
        /// <remarks>
        /// Override this method to implement a different way to obtain a blocker GameObject.
        /// </remarks>
        /// <param name="rootCanvas">The root canvas the dropdown is under.</param>
        /// <returns>The created blocker object</returns>
        protected virtual GameObject CreateBlocker(Canvas rootCanvas)
        {
            // Create blocker GameObject.
            GameObject blocker = new GameObject("Blocker");

            // Setup blocker RectTransform to cover entire root canvas area.
            RectTransform blockerRect = blocker.AddComponent<RectTransform>();
            blockerRect.SetParent(rootCanvas.transform, false);
            blockerRect.anchorMin = Vector3.zero;
            blockerRect.anchorMax = Vector3.one;
            blockerRect.sizeDelta = Vector2.zero;

            // Make blocker be in separate canvas in same layer as dropdown and in layer just below it.
            Canvas blockerCanvas = blocker.AddComponent<Canvas>();
            blockerCanvas.overrideSorting = true;
            Canvas dropdownCanvas = _dropdown.GetComponent<Canvas>();
            blockerCanvas.sortingLayerID = dropdownCanvas.sortingLayerID;
            blockerCanvas.sortingOrder = dropdownCanvas.sortingOrder - 1;

            // Find the Canvas that this dropdown is a part of
            Canvas parentCanvas = null;
            Transform parentTransform = _template.parent;
            while (parentTransform != null)
            {
                parentCanvas = parentTransform.GetComponent<Canvas>();
                if (parentCanvas != null)
                    break;

                parentTransform = parentTransform.parent;
            }

            // If we have a parent canvas, apply the same raycasters as the parent for consistency.
            if (parentCanvas != null)
            {
                Component[] components = parentCanvas.GetComponents<BaseRaycaster>();
                for (int i = 0; i < components.Length; i++)
                {
                    Type raycasterType = components[i].GetType();
                    if (blocker.GetComponent(raycasterType) == null)
                    {
                        blocker.AddComponent(raycasterType);
                    }
                }
            }
            else
            {
                // Add raycaster since it's needed to block.
                GetOrAddComponent<GraphicRaycaster>(blocker);
            }

            // Add image since it's needed to block, but make it clear.
            Image blockerImage = blocker.AddComponent<Image>();
            blockerImage.color = Color.clear;

            // Add button since it's needed to block, and to close the dropdown when blocking area is clicked.
            Button blockerButton = blocker.AddComponent<Button>();
            blockerButton.onClick.AddListener(Hide);

            return blocker;
        }

        /// <summary>
        /// Convenience method to explicitly destroy the previously generated blocker object
        /// </summary>
        /// <remarks>
        /// Override this method to implement a different way to dispose of a blocker GameObject that blocks clicks to other controls while the dropdown list is open.
        /// </remarks>
        /// <param name="blocker">The blocker object to destroy.</param>
        protected virtual void DestroyBlocker(GameObject blocker)
        {
            Destroy(blocker);
        }

        /// <summary>
        /// Create the dropdown list to be shown when the dropdown is clicked. The dropdown list should correspond to the provided template GameObject, equivalent to instantiating a copy of it.
        /// </summary>
        /// <remarks>
        /// Override this method to implement a different way to obtain a dropdown list GameObject.
        /// </remarks>
        /// <param name="template">The template to create the dropdown list from.</param>
        /// <returns>The created drop down list gameobject.</returns>
        protected virtual GameObject CreateDropdownList(GameObject template)
        {
            return Instantiate(template);
        }

        /// <summary>
        /// Convenience method to explicitly destroy the previously generated dropdown list
        /// </summary>
        /// <remarks>
        /// Override this method to implement a different way to dispose of a dropdown list GameObject.
        /// </remarks>
        /// <param name="dropdownList">The dropdown list GameObject to destroy</param>
        protected virtual void DestroyDropdownList(GameObject dropdownList)
        {
            Destroy(dropdownList);
        }
        
        /// <summary>
        /// Create a dropdown item based upon the item template.
        /// </summary>
        /// <remarks>
        /// Override this method to implement a different way to obtain an option item.
        /// The option item should correspond to the provided template DropdownItem and its GameObject, equivalent to instantiating a copy of it.
        /// </remarks>
        /// <param name="itemTemplate">e template to create the option item from.</param>
        /// <returns>The created dropdown item component</returns>
        protected virtual CrossDropdownItem CreateItem(CrossDropdownItem itemTemplate)
        {
            return Instantiate(itemTemplate);
        }
        
        /// <summary>
        ///  Convenience method to explicitly destroy the previously generated Items.
        /// </summary>
        /// <remarks>
        /// Override this method to implement a different way to dispose of an option item.
        /// Likely no action needed since destroying the dropdown list destroys all contained items as well.
        /// </remarks>
        /// <param name="item">The Item to destroy.</param>
        protected virtual void DestroyItem(CrossDropdownItem item) { }

        // Add a new drop-down list item with the specified values.
        private CrossDropdownItem AddItem(TMP_Dropdown.OptionData data, bool selected, CrossDropdownItem itemTemplate, List<CrossDropdownItem> items)
        {
            // Add a new item to the dropdown.
            CrossDropdownItem item = CreateItem(itemTemplate);
            item.Dropdown = this;
            item.RectTransform.SetParent(itemTemplate.RectTransform.parent, false);

            item.gameObject.SetActive(true);
            item.gameObject.name = "Item " + items.Count + (data.text != null ? ": " + data.text : "");

            if (item.Toggle != null)
            {
                item.Toggle.IsOn = false;
            }

            // Set the item's data
            if (item.Text)
                item.Text.text = data.text;
            if (item.Image)
            {
                item.Image.sprite = data.image;
                item.Image.enabled = (item.Image.sprite != null);
            }

            items.Add(item);
            return item;
        }

        private void AlphaFadeList(float alpha)
        {
            LMotion.Create(_listCanvasGroup.alpha, alpha, _alphaFadeSpeed)
                .BindToAlpha(_listCanvasGroup)
                .AddTo(_listCanvasGroup.gameObject);
        }

        /// <summary>
        /// Hide the dropdown list. I.e. close it.
        /// </summary>
        public void Hide()
        {
            if (_destroyCoroutine != null) return;
            if (_dropdown != null)
            {
                AlphaFadeList(0f);

                // User could have disabled the dropdown during the OnValueChanged call.
                if (IsActive())
                    _destroyCoroutine = StartCoroutine(DelayedDestroyDropdownList(_alphaFadeSpeed));
            }

            if (_blocker != null)
                DestroyBlocker(_blocker);

            _blocker = null;
            Select();
        }

        private IEnumerator DelayedDestroyDropdownList(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            ImmediateDestroyDropdownList();
        }

        private void ImmediateDestroyDropdownList()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] != null)
                    DestroyItem(_items[i]);
            }

            _items.Clear();

            if (_dropdown != null)
                DestroyDropdownList(_dropdown);

            // if (m_AlphaTweenRunner != null)
            //     m_AlphaTweenRunner.StopTween();

            _dropdown = null;
            _destroyCoroutine = null;
        }

        // Change the value and hide the dropdown.
        private void OnSelectItem(CrossRevampToggle crossToggle)
        {
            if (!crossToggle.IsOn)
                crossToggle.IsOn = true;

            int selectedIndex = -1;
            Transform tr = crossToggle.transform;
            Transform parent = tr.parent;
            for (int i = 0; i < parent.childCount; i++)
            {
                if (parent.GetChild(i) == tr)
                {
                    // Subtract one to account for template child.
                    selectedIndex = i - 1;
                    break;
                }
            }

            if (selectedIndex < 0)
                return;

            Value = selectedIndex;
            Hide();
        }
    }
}

#endif // CROSS_REVAMP
