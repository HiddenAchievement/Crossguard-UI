using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Events;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// Utility class for creating default implementations of Crossguard UI controls.
    /// </summary>
    public static class CrossDefaultControls
    {
        public struct Resources
        {
            public Sprite Standard;
            public Sprite Background;
            public Sprite InputField;
            public Sprite Knob;
            public Sprite Checkmark;
            public Sprite Dropdown;
            public Sprite Mask;

            public ColorAndScaleStyle ButtonStyle;
            public ColorAndScaleStyle DropdownStyle;
            public ColorAndScaleStyle DropdownItemStyle;
            public ColorAndScaleStyle InputFieldStyle;
            public ColorAndScaleStyle ScrollbarStyle;
            public ColorAndScaleStyle SliderStyle;
            public ColorAndScaleStyle SpinnerStyle;
            public ColorAndScaleStyle SpinnerButtonStyle;
            public ColorAndScaleStyle ToggleStyle;
            public ColorAndScaleStyle IconToggleStyle;
            public ColorAndScaleStyle SlideToggleStyle;
        }
        
        private const float  kWidth       = 160f;
        private const float  kThickHeight = 30f;
        private const float  kThinHeight  = 20f;
        private static Vector2 s_ThickElementSize       = new Vector2(kWidth, kThickHeight);
        private static Vector2 s_ThinElementSize        = new Vector2(kWidth, kThinHeight);
        private static Vector2 s_ImageElementSize       = new Vector2(100f, 100f);
        private static Vector2 s_ThickSquareElementSize = new Vector2(kThickHeight, kThickHeight);
        private static Vector2 s_ThinSquareElementSize  = new Vector2(kThinHeight, kThinHeight);
        private static Color s_DefaultSelectableColor   = new Color(1, 1, 1, 1);
        private static Color   s_DefaultSelectorColor   = new Color(1f, 0.6f, 0.1f, 1f);
        private static Color   s_TextColor              = new Color(50f / 255f, 50f / 255f, 50f / 255f, 1f);
        private static Color   s_FillColor              = new Color(0.80f, 0.14f, 0.47f);
        
        
        private static GameObject CreateUIElementRoot(string name, Vector2 size)
        {
            GameObject child = new GameObject(name);
            RectTransform rectTransform = child.AddComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            return child;
        }
        private static GameObject CreateUIObject(string name, GameObject parent)
        {
            GameObject go = new GameObject(name);
            go.AddComponent<RectTransform>();
            SetParentAndAlign(go, parent);
            return go;
        }
        
        private static void SetDefaultTextValues(TMP_Text lbl)
        {
            // Set text values we want across UI elements in default controls.
            // Don't set values which are the same as the default values for the Text component,
            // since there's no point in that, and it's good to keep them as consistent as possible.
            lbl.color = s_TextColor;
            lbl.fontSize = 14;
        }

        private static void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (parent == null)
                return;

#if UNITY_EDITOR
            Undo.SetTransformParent(child.transform, parent.transform, "");
#else
            child.transform.SetParent(parent.transform, false);
#endif
            SetLayerRecursively(child, parent.layer);
        }

        private static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform t = go.transform;
            for (int i = 0; i < t.childCount; i++)
                SetLayerRecursively(t.GetChild(i).gameObject, layer);
        }

        private static void ConfigureSelector(Resources resources, GameObject selector)
        {
            if (selector == null) return;
            
            Image selectorImg = selector.AddComponent<Image>();
            selectorImg.sprite = resources.Standard;
            selectorImg.type = Image.Type.Sliced;
            selectorImg.color = s_DefaultSelectorColor;
            selectorImg.fillCenter = false;
            
            RectTransform selectorRT = selector.GetComponent<RectTransform>();
            selectorRT.sizeDelta = new Vector2(12, 12);
            selectorRT.anchorMin = Vector2.zero;
            selectorRT.anchorMax = Vector2.one;
        }
        
        
        private static GameObject CreateButton(Resources resources, Vector2 size)
        {
            GameObject buttonRoot = CreateUIElementRoot("Button", size);
            GameObject selector = CreateUIObject("Selector", buttonRoot);

            Image image = buttonRoot.AddComponent<Image>();
            image.sprite = resources.Standard;
            image.type = Image.Type.Sliced;
            image.color = s_DefaultSelectableColor;

            CrossButton bt = buttonRoot.AddComponent<CrossButton>();
            bt.transition = Selectable.Transition.None;

            ConfigureSelector(resources, selector);

            ColorAndScaleTransitioner transitioner = buttonRoot.AddComponent<ColorAndScaleTransitioner>();
            transitioner.Style = resources.ButtonStyle;

            return buttonRoot;
        }

        public static (GameObject, GameObject) CreateSpinner(Resources resources, Vector2 size)
        {
            GameObject spinnerRoot = CreateUIElementRoot("Spinner", size + new Vector2(38, 0));
            GameObject selector   = CreateUIObject("Selector", spinnerRoot);
            GameObject background = CreateUIObject("Background", spinnerRoot);

            ConfigureSelector(resources, selector);

            Image bgImage = background.AddComponent<Image>();
            bgImage.sprite = resources.Standard;
            bgImage.type = Image.Type.Sliced;
            bgImage.color = s_DefaultSelectableColor;

            RectTransform backgroundRT = background.GetComponent<RectTransform>();
            backgroundRT.sizeDelta = size;

            Vector2 spinnerButtonSize = s_ThickSquareElementSize - new Vector2(4, 4);

            CrossButton decButton = CreateDecrementButton(resources, spinnerRoot, spinnerButtonSize)
                .GetComponent<CrossButton>();
            CrossButton incButton = CreateIncrementButton(resources, spinnerRoot, spinnerButtonSize)
                .GetComponent<CrossButton>();

            AxisControlHelper axisControl = spinnerRoot.AddComponent<AxisControlHelper>();
            axisControl.DecrementButton = decButton;
            axisControl.IncrementButton = incButton;
            
            return (spinnerRoot, background);
        }

        private static GameObject CreateSpinnerButton(string name, Resources resources, GameObject parent, Vector2 size)
        {
            GameObject go = CreateUIObject(name, parent);
            
            Image image = go.AddComponent<Image>();
            image.sprite = resources.Knob;
            image.color = s_DefaultSelectableColor;
            
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = size;

            CrossButton button = go.AddComponent<CrossButton>();
            button.transition = Selectable.Transition.None;
            Navigation nav = button.navigation;
            nav.mode = Navigation.Mode.None;
            button.navigation = nav;

            ColorAndScaleTransitioner transitioner = go.AddComponent<ColorAndScaleTransitioner>();
            transitioner.Style = resources.SpinnerButtonStyle;

            return go;
        }
        
        private static GameObject CreateDecrementButton(Resources resources, GameObject parent, Vector2 size)
        {
            GameObject go = CreateSpinnerButton("Decrement Button", resources, parent, size);

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0.5f);
            rt.anchorMax = new Vector2(0, 0.5f);
            rt.pivot = new Vector2(1, 0.5f);
            rt.anchoredPosition = new Vector2(kThinHeight, 0);

            return go;
        }
        
        private static GameObject CreateIncrementButton(Resources resources, GameObject parent, Vector2 size)
        {
            GameObject go = CreateSpinnerButton("Increment Button", resources, parent, size);

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(1, 0.5f);
            rt.anchorMax = new Vector2(1, 0.5f);
            rt.pivot = new Vector2(0, 0.5f);
            rt.anchoredPosition = new Vector2(-kThinHeight, 0);

            return go;
        }

        /// <summary>
        /// Create the basic UI Text button.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Button
        ///         -Selector
        ///         -Icon
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateTextButton(Resources resources)
        {
            GameObject button = CreateButton(resources, s_ThickElementSize);

            GameObject label = new GameObject("Label");
            RectTransform textRectTransform = label.AddComponent<RectTransform>();
            SetParentAndAlign(label, button);
            
            TextMeshProUGUI text = label.AddComponent<TextMeshProUGUI>();
            text.text = "Button";
            text.alignment = TextAlignmentOptions.Center;
            text.raycastTarget = false;
            SetDefaultTextValues(text);
        
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.sizeDelta = Vector2.zero;
            
            return button;
        }
        
        /// <summary>
        /// Create the basic UI Icon button.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Button
        ///         -Selector
        ///         -Text
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateIconButton(Resources resources)
        {
            GameObject button = CreateButton(resources, s_ThickSquareElementSize);

            GameObject icon = new GameObject("Icon");
            RectTransform iconRT = icon.AddComponent<RectTransform>();
            SetParentAndAlign(icon, button);
            
            Image iconImage = icon.AddComponent<Image>();
            iconImage.sprite = resources.Checkmark;
            iconImage.raycastTarget = false;
            iconRT.sizeDelta = s_ThickSquareElementSize - new Vector2(4, 4);

            return button;
        }

        /// <summary>
        /// Create the basic UI Toggle.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Toggle
        ///         - Selector
        ///         - Background
        ///             - Checkmark
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateToggle(Resources resources)
        {
            GameObject toggleRoot = CreateUIElementRoot("Toggle", s_ThinSquareElementSize);
            GameObject selector = CreateUIObject("Selector", toggleRoot);
            GameObject background = CreateUIObject("Background", toggleRoot);
            GameObject checkmark = CreateUIObject("Checkmark", background);

            CrossToggle crossToggle = toggleRoot.AddComponent<CrossToggle>();
            crossToggle.transition = Selectable.Transition.None;
            crossToggle.IsOn = true;

            ConfigureSelector(resources, selector);
            
            Image bgImage = background.AddComponent<Image>();
            bgImage.sprite = resources.Standard;
            bgImage.type = Image.Type.Sliced;
            bgImage.color = s_DefaultSelectableColor;

            Image checkmarkImage = checkmark.AddComponent<Image>();
            checkmarkImage.sprite = resources.Checkmark;
            crossToggle.CheckGraphic = checkmarkImage;
            crossToggle.CheckTransition = CrossToggle.ToggleTransition.None;

            RectTransform bgRect = background.GetComponent<RectTransform>();
            bgRect.sizeDelta        = s_ThinSquareElementSize;

            RectTransform checkmarkRect = checkmark.GetComponent<RectTransform>();
            checkmarkRect.anchorMin = new Vector2(0.5f, 0.5f);
            checkmarkRect.anchorMax = new Vector2(0.5f, 0.5f);
            checkmarkRect.anchoredPosition = Vector2.zero;
            checkmarkRect.sizeDelta = new Vector2(20f, 20f);

            ColorAndScaleTransitioner transitioner = toggleRoot.AddComponent<ColorAndScaleTransitioner>();
            transitioner.Style = resources.ToggleStyle;

            return toggleRoot;
        }

        /// <summary>
        /// Create the UI Slide Toggle.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Toggle
        ///         - Selector
        ///         - Background
        ///         - Sliding Area
        ///             - Fill
        ///                 - Handle
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateSlideToggle(Resources resources)
        {
            Vector2 size = new Vector2(kThickHeight * 1.5f, kThickHeight);
            GameObject toggleRoot = CreateUIElementRoot("Toggle", size);
            GameObject selector    = CreateUIObject("Selector",     toggleRoot);
            GameObject background  = CreateUIObject("Background",   toggleRoot);
            GameObject slidingArea = CreateUIObject("Sliding Area", toggleRoot);
            GameObject fill        = CreateUIObject("Fill",         slidingArea);
            GameObject handle      = CreateUIObject("Handle",       fill);

            CrossSlideToggle slideToggle = toggleRoot.AddComponent<CrossSlideToggle>();
            slideToggle.transition = Selectable.Transition.None;
            slideToggle.IsOn = true;

            ConfigureSelector(resources, selector);

            Image bgImage = background.AddComponent<Image>();
            bgImage.sprite = resources.Standard;
            bgImage.type = Image.Type.Sliced;
            bgImage.color = s_DefaultSelectableColor;
            
            RectTransform backgroundRT = background.GetComponent<RectTransform>();
            backgroundRT.anchorMin = new Vector2(0, 0);
            backgroundRT.anchorMax = new Vector2(1, 1);
            backgroundRT.sizeDelta = new Vector2(0, -8);
            
            RectTransform slidingAreaRT = slidingArea.GetComponent<RectTransform>();
            slidingAreaRT.anchorMin = new Vector2(0, 0);
            slidingAreaRT.anchorMax = new Vector2(1, 1);
            slidingAreaRT.sizeDelta = new Vector2(-18, -12);

            Image fillImage = fill.AddComponent<Image>();
            fillImage.sprite = resources.Standard;
            fillImage.type = Image.Type.Sliced;
            fillImage.raycastTarget = false;
            fillImage.color = s_FillColor;

            RectTransform fillRT = fill.GetComponent<RectTransform>();
            fillRT.anchorMin = new Vector2(0, 0);
            fillRT.anchorMax = new Vector2(1, 1);
            fillRT.anchoredPosition = new Vector2(-2, 0);
            fillRT.sizeDelta = new Vector2(6, -4);

            slideToggle.Fill = fillRT;

            Image handleImage = handle.AddComponent<Image>();
            handleImage.sprite = resources.Knob;
            handleImage.color = s_DefaultSelectableColor;

            RectTransform handleRT = handle.GetComponent<RectTransform>();
            handleRT.sizeDelta = new Vector2(kThickHeight, kThickHeight);
            handleRT.anchorMin = new Vector2(1, 0.5f);
            handleRT.anchorMax = new Vector2(1, 0.5f);

            ColorAndScaleTransitioner transitioner = toggleRoot.AddComponent<ColorAndScaleTransitioner>();
            transitioner.Style = resources.SlideToggleStyle;
            
            return toggleRoot;
        }

        /// <summary>
        /// Create the UI Icon Toggle.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Toggle
        ///         - Selector
        ///         - Background
        ///         - Icon
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateIconToggle(Resources resources)
        {
            GameObject toggleRoot = CreateUIElementRoot("Toggle", s_ThickSquareElementSize);
            GameObject selector    = CreateUIObject("Selector",   toggleRoot);
            GameObject background  = CreateUIObject("Background", toggleRoot);
            GameObject icon        = CreateUIObject("Icon",       toggleRoot);

            CrossIconToggle iconToggle = toggleRoot.AddComponent<CrossIconToggle>();
            iconToggle.transition = Selectable.Transition.None;
            iconToggle.IsOn = true;

            ConfigureSelector(resources, selector);
            
            Image bgImage = background.AddComponent<Image>();
            bgImage.sprite = resources.Standard;
            bgImage.type = Image.Type.Sliced;
            bgImage.color = s_DefaultSelectableColor;
            
            RectTransform backgroundRT = background.GetComponent<RectTransform>();
            backgroundRT.anchorMin = new Vector2(0, 0);
            backgroundRT.anchorMax = new Vector2(1, 1);
            backgroundRT.sizeDelta = new Vector2(0, 0);

            Image iconImage = icon.AddComponent<Image>();
            iconImage.sprite = resources.Dropdown;
            iconImage.color = s_DefaultSelectableColor;
            iconImage.raycastTarget = false;
            
            RectTransform iconRT = icon.GetComponent<RectTransform>();
            iconRT.sizeDelta = s_ThickSquareElementSize - new Vector2(2, 2);

            iconToggle.Icon = iconImage;
            iconToggle.OffIcon = resources.Checkmark;
            iconToggle.OnIcon = resources.Dropdown;

            ColorAndScaleTransitioner transitioner = toggleRoot.AddComponent<ColorAndScaleTransitioner>();
            transitioner.Style = resources.IconToggleStyle;
            
            return toggleRoot;
        }
        
        /// <summary>
        /// Create the UI Text Toggle.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Toggle
        ///         - Selector
        ///         - Background
        ///         - Label
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateTextToggle(Resources resources)
        {
            GameObject toggleRoot = CreateUIElementRoot("Toggle", s_ThickSquareElementSize);
            GameObject selector   = CreateUIObject("Selector",   toggleRoot);
            GameObject background = CreateUIObject("Background", toggleRoot);
            GameObject label      = CreateUIObject("Label",      toggleRoot);

            CrossTextToggle textToggle = toggleRoot.AddComponent<CrossTextToggle>();
            textToggle.transition = Selectable.Transition.None;
            textToggle.IsOn = true;

            ConfigureSelector(resources, selector);
            
            Image bgImage = background.AddComponent<Image>();
            bgImage.sprite = resources.Standard;
            bgImage.type = Image.Type.Sliced;
            bgImage.color = s_DefaultSelectableColor;
            
            RectTransform backgroundRT = background.GetComponent<RectTransform>();
            backgroundRT.anchorMin = new Vector2(0, 0);
            backgroundRT.anchorMax = new Vector2(1, 1);
            backgroundRT.sizeDelta = new Vector2(0, 0);

            TextMeshProUGUI labelText = label.AddComponent<TextMeshProUGUI>();
            labelText.text = "A";
            labelText.alignment = TextAlignmentOptions.Center;
            labelText.raycastTarget = false;
            SetDefaultTextValues(labelText);
            textToggle.Label = labelText;

            RectTransform labelRT = label.GetComponent<RectTransform>();
            labelRT.anchorMin = Vector2.zero;
            labelRT.anchorMax = Vector2.one;
            labelRT.sizeDelta = Vector2.zero;

            ColorAndScaleTransitioner transitioner = toggleRoot.AddComponent<ColorAndScaleTransitioner>();
            transitioner.Style = resources.ButtonStyle;
            
            return toggleRoot;
        }
        
        /// <summary>
        /// Create the Legacy UGUI Toggle.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Toggle
        ///         - Selector
        ///         - Background
        ///             - Checkmark
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateUguiToggle(Resources resources)
        {
            GameObject toggleRoot = CreateUIElementRoot("Toggle", s_ThinSquareElementSize);
            GameObject selector   = CreateUIObject("Selector", toggleRoot);
            GameObject background = CreateUIObject("Background", toggleRoot);
            GameObject checkmark  = CreateUIObject("Checkmark", background);

            CrossUguiToggle crossToggle = toggleRoot.AddComponent<CrossUguiToggle>();
            crossToggle.transition = Selectable.Transition.None;
            crossToggle.IsOn = true;

            ConfigureSelector(resources, selector);
            
            Image bgImage  = background.AddComponent<Image>();
            bgImage.sprite = resources.Standard;
            bgImage.type   = Image.Type.Sliced;
            bgImage.color  = s_DefaultSelectableColor;

            Image checkmarkImage  = checkmark.AddComponent<Image>();
            checkmarkImage.sprite = resources.Checkmark;
            crossToggle.graphic   = checkmarkImage;
            crossToggle.toggleTransition = Toggle.ToggleTransition.None;

            RectTransform bgRect = background.GetComponent<RectTransform>();
            bgRect.sizeDelta     = s_ThinSquareElementSize;

            RectTransform checkmarkRect = checkmark.GetComponent<RectTransform>();
            checkmarkRect.anchorMin = new Vector2(0.5f, 0.5f);
            checkmarkRect.anchorMax = new Vector2(0.5f, 0.5f);
            checkmarkRect.anchoredPosition = Vector2.zero;
            checkmarkRect.sizeDelta = new Vector2(20f, 20f);

            ColorAndScaleTransitioner transitioner = toggleRoot.AddComponent<ColorAndScaleTransitioner>();
            transitioner.Style = resources.ToggleStyle;

            return toggleRoot;
        }

        /// <summary>
        /// Create the UI Icon Spinner.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Toggle
        ///         - Selector
        ///         - Background
        ///             - Readout
        ///         - Decrement Button
        ///         - Increment Button
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateIconSpinner(Resources resources)
        {
            GameObject spinnerRoot, background;
            (spinnerRoot, background) = CreateSpinner(resources, s_ThickSquareElementSize);
            GameObject readout = CreateUIObject("Readout", background);
            
            Image readoutImage = readout.AddComponent<Image>();
            readoutImage.sprite = resources.Checkmark;
            readoutImage.raycastTarget = false;
            readoutImage.color = s_DefaultSelectableColor;
            
            RectTransform readoutRT = readoutImage.GetComponent<RectTransform>();
            readoutRT.sizeDelta = s_ThickSquareElementSize - new Vector2(4, 4);
            
            CrossIconSpinner spinner = spinnerRoot.AddComponent<CrossIconSpinner>();
            spinner.Readout = readoutImage;
            spinner.Icons = new[]
            {
                resources.Checkmark,
                resources.Dropdown,
                resources.Knob
            };
            
            ColorAndScaleTransitioner transitioner = spinnerRoot.AddComponent<ColorAndScaleTransitioner>();
            transitioner.Style = resources.SpinnerStyle;

            return spinnerRoot;
        }
        
        /// <summary>
        /// Create the UI Number Spinner.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Toggle
        ///         - Selector
        ///         - Background
        ///             - Readout
        ///         - Decrement Button
        ///         - Increment Button
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateNumberSpinner(Resources resources)
        {
            GameObject spinnerRoot, background;
            (spinnerRoot, background) = CreateSpinner(resources, s_ThickSquareElementSize);
            GameObject readout = CreateUIObject("Readout", background);
            
            TextMeshProUGUI readoutText = readout.AddComponent<TextMeshProUGUI>();
            readoutText.text = "8";
            readoutText.alignment = TextAlignmentOptions.Center;
            readoutText.raycastTarget = false;
            SetDefaultTextValues(readoutText);

            RectTransform readoutRT = readout.GetComponent<RectTransform>();
            readoutRT.anchorMin = Vector2.zero;
            readoutRT.anchorMax = Vector2.one;
            readoutRT.sizeDelta = Vector2.zero;
            
            CrossNumberSpinner spinner = spinnerRoot.AddComponent<CrossNumberSpinner>();
            spinner.Readout = readoutText;

            ColorAndScaleTransitioner transitioner = spinnerRoot.AddComponent<ColorAndScaleTransitioner>();
            transitioner.Style = resources.SpinnerStyle;

            return spinnerRoot;
        }
        
        /// <summary>
        /// Create the UI Number Spinner.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Toggle
        ///         - Selector
        ///         - Background
        ///             - Readout
        ///         - Decrement Button
        ///         - Increment Button
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateTextSpinner(Resources resources)
        {
            GameObject spinnerRoot, background;
            (spinnerRoot, background) = CreateSpinner(resources, s_ThickElementSize);
            GameObject readout = CreateUIObject("Readout", background);
            
            TextMeshProUGUI readoutText = readout.AddComponent<TextMeshProUGUI>();
            readoutText.text = "A";
            readoutText.alignment = TextAlignmentOptions.Center;
            readoutText.raycastTarget = false;
            SetDefaultTextValues(readoutText);

            RectTransform readoutRT = readout.GetComponent<RectTransform>();
            readoutRT.anchorMin = Vector2.zero;
            readoutRT.anchorMax = Vector2.one;
            readoutRT.sizeDelta = Vector2.zero;
            
            CrossTextSpinner spinner = spinnerRoot.AddComponent<CrossTextSpinner>();
            spinner.Readout = readoutText;
            spinner.Options = new[]
            {
                "Apples", "Oranges", "Pears"
            };

            ColorAndScaleTransitioner transitioner = spinnerRoot.AddComponent<ColorAndScaleTransitioner>();
            transitioner.Style = resources.SpinnerStyle;

            return spinnerRoot;
        }
        
        
        /// <summary>
        /// Create the basic UI Slider.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Slider
        ///         - Selector
        ///         - Background
        ///         - Fill Area
        ///             - Fill
        ///         - Handle Slide Area
        ///             - Handle
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateSlider(Resources resources)
        {
            // Create GOs Hierarchy
            GameObject root = CreateUIElementRoot("Slider", s_ThinElementSize);
            GameObject selector    = CreateUIObject("Selector", root);
            GameObject background = CreateUIObject("Background", root);
            GameObject fillArea = CreateUIObject("Fill Area", root);
            GameObject fill = CreateUIObject("Fill", fillArea);
            GameObject handleArea = CreateUIObject("Handle Slide Area", root);
            GameObject handle = CreateUIObject("Handle", handleArea);

            ConfigureSelector(resources, selector);
            
            // Background
            Image backgroundImage = background.AddComponent<Image>();
            backgroundImage.sprite = resources.Background;
            backgroundImage.type = Image.Type.Sliced;
            backgroundImage.color = s_DefaultSelectableColor;
            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0, 0.25f);
            backgroundRect.anchorMax = new Vector2(1, 0.75f);
            backgroundRect.sizeDelta = new Vector2(0, 0);

            // Fill Area
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0.25f);
            fillAreaRect.anchorMax = new Vector2(1, 0.75f);
            fillAreaRect.anchoredPosition = new Vector2(-5, 0);
            fillAreaRect.sizeDelta = new Vector2(-20, 0);

            // Fill
            Image fillImage = fill.AddComponent<Image>();
            fillImage.sprite = resources.Standard;
            fillImage.type = Image.Type.Sliced;
            fillImage.color = s_FillColor;

            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.sizeDelta = new Vector2(10, 0);

            // Handle Area
            RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
            handleAreaRect.sizeDelta = new Vector2(-20, 0);
            handleAreaRect.anchorMin = new Vector2(0, 0);
            handleAreaRect.anchorMax = new Vector2(1, 1);

            // Handle
            Image handleImage = handle.AddComponent<Image>();
            handleImage.sprite = resources.Knob;
            handleImage.color = s_DefaultSelectableColor;

            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 0);

            // Setup slider component
            CrossSlider slider = root.AddComponent<CrossSlider>();
            slider.fillRect = fill.GetComponent<RectTransform>();
            slider.handleRect = handle.GetComponent<RectTransform>();
            slider.direction = Slider.Direction.LeftToRight;

            ColorAndScaleTransitioner transitioner = root.AddComponent<ColorAndScaleTransitioner>();
            transitioner.Style = resources.SliderStyle;
            
            return root;
        }



        public static GameObject CreateInputField(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Input Field", s_ThickElementSize);
            GameObject selector = CreateUIObject("Selector", root);
            GameObject textArea = CreateUIObject("Text Area", root);
            GameObject childPlaceholder = CreateUIObject("Placeholder", textArea);
            GameObject childText = CreateUIObject("Text", textArea);

            ConfigureSelector(resources, selector);

            Image image = root.AddComponent<Image>();
            image.sprite = resources.InputField;
            image.type = Image.Type.Sliced;
            image.color = s_DefaultSelectableColor;

            CrossInputField inputField = root.AddComponent<CrossInputField>();
            inputField.selectionColor = new Color(0.797f, 0.638f, 1f);

            // Use UI.Mask for Unity 5.0 - 5.1 and 2D RectMask for Unity 5.2 and up
            textArea.AddComponent<RectMask2D>();

            RectTransform textAreaRectTransform = textArea.GetComponent<RectTransform>();
            textAreaRectTransform.anchorMin = Vector2.zero;
            textAreaRectTransform.anchorMax = Vector2.one;
            textAreaRectTransform.sizeDelta = Vector2.zero;
            textAreaRectTransform.offsetMin = new Vector2(8, 6);
            textAreaRectTransform.offsetMax = new Vector2(-8, -7);

            TextMeshProUGUI text = childText.AddComponent<TextMeshProUGUI>();
            text.text = "";
            text.enableWordWrapping = false;
            text.extraPadding = true;
            text.richText = true;
            SetDefaultTextValues(text);
            text.margin = new Vector4(2, 0, 0, 2); // To keep italics within the mask.

            TextMeshProUGUI placeholder = childPlaceholder.AddComponent<TextMeshProUGUI>();
            placeholder.text = "Enter text...";
            placeholder.fontSize = 14;
            placeholder.fontStyle = FontStyles.Italic;
            placeholder.enableWordWrapping = false;
            placeholder.extraPadding = true;
            placeholder.margin = new Vector4(2, 0, 0, 2); // To keep italics within the mask.

            // Make placeholder color half as opaque as normal text color.
            Color placeholderColor = text.color;
            placeholderColor.a *= 0.5f;
            placeholder.color = placeholderColor;

            // Add Layout component to placeholder.
            placeholder.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;

            RectTransform textRectTransform = childText.GetComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.sizeDelta = Vector2.zero;
            textRectTransform.offsetMin = new Vector2(0, 0);
            textRectTransform.offsetMax = new Vector2(0, 0);

            RectTransform placeholderRectTransform = childPlaceholder.GetComponent<RectTransform>();
            placeholderRectTransform.anchorMin = Vector2.zero;
            placeholderRectTransform.anchorMax = Vector2.one;
            placeholderRectTransform.sizeDelta = Vector2.zero;
            placeholderRectTransform.offsetMin = new Vector2(0, 0);
            placeholderRectTransform.offsetMax = new Vector2(0, 0);

            inputField.textViewport = textAreaRectTransform;
            inputField.textComponent = text;
            inputField.placeholder = placeholder;
            inputField.fontAsset = text.font;

            ColorAndScaleTransitioner transitioner = root.AddComponent<ColorAndScaleTransitioner>();
            transitioner.Style = resources.InputFieldStyle;

            return root;
        }


        /// <summary>
        /// Create the basic UI dropdown.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Dropdown
        ///         - Button
        ///             - Selector
        ///             - Label
        ///             - Arrow
        ///         - Template
        ///             - Viewport
        ///                 - Content
        ///                     - Item
        ///                         - Item Background
        ///                         - Item Checkmark
        ///                         - Item Label
        ///             - Scrollbar
        ///                 - Sliding Area
        ///                     - Handle
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateDropdown(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Dropdown", s_ThickElementSize);
            GameObject button         = CreateUIObject("Button",          root);
            GameObject selector       = CreateUIObject("Selector",        button);
            GameObject label          = CreateUIObject("Label",           button);
            GameObject arrow          = CreateUIObject("Arrow",           button);
            GameObject template       = CreateUIObject("Template",        root);
            GameObject viewport       = CreateUIObject("Viewport",        template);
            GameObject content        = CreateUIObject("Content",         viewport);
            GameObject item           = CreateUIObject("Item",            content);
            GameObject itemBackground = CreateUIObject("Item Background", item);
            GameObject itemCheckmark  = CreateUIObject("Item Checkmark",  item);
            GameObject itemLabel      = CreateUIObject("Item Label",      item);

            ConfigureSelector(resources, selector);
            
            // Sub controls.

            GameObject scrollbar = CreateScrollbar(resources, true);
            scrollbar.name = "Scrollbar";
            SetParentAndAlign(scrollbar, template);

            CrossScrollbar scrollbarScrollbar = scrollbar.GetComponent<CrossScrollbar>();
            scrollbarScrollbar.SetDirection(Scrollbar.Direction.BottomToTop, true);
            Navigation nav = scrollbarScrollbar.navigation;
            nav.mode = Navigation.Mode.None;
            scrollbarScrollbar.navigation = nav;

            RectTransform vScrollbarRT = scrollbar.GetComponent<RectTransform>();
            vScrollbarRT.anchorMin = Vector2.right;
            vScrollbarRT.anchorMax = Vector2.one;
            vScrollbarRT.pivot = Vector2.one;
            vScrollbarRT.sizeDelta = new Vector2(vScrollbarRT.sizeDelta.x, 0);

            // Setup item UI components.

            TextMeshProUGUI itemLabelText = itemLabel.AddComponent<TextMeshProUGUI>();
            SetDefaultTextValues(itemLabelText);
            itemLabelText.alignment = TextAlignmentOptions.Left;

            Image itemBackgroundImage = itemBackground.AddComponent<Image>();
            itemBackgroundImage.color = new Color32(245, 245, 245, 255);

            Image itemCheckmarkImage = itemCheckmark.AddComponent<Image>();
            itemCheckmarkImage.sprite = resources.Checkmark;

            CrossToggle itemCrossToggle = item.AddComponent<CrossToggle>();
            itemCrossToggle.CheckGraphic = itemCheckmarkImage;
            itemCrossToggle.IsOn = true;
            
            ColorAndScaleTransitioner itemTransitioner = item.AddComponent<ColorAndScaleTransitioner>();
            itemTransitioner.Style = resources.DropdownItemStyle;

            // Setup template UI components.

            Image templateImage = template.AddComponent<Image>();
            templateImage.sprite = resources.Standard;
            templateImage.type = Image.Type.Sliced;

            ScrollRect templateScrollRect = template.AddComponent<ScrollRect>();
            templateScrollRect.content = (RectTransform)content.transform;
            templateScrollRect.viewport = (RectTransform)viewport.transform;
            templateScrollRect.horizontal = false;
            templateScrollRect.movementType = ScrollRect.MovementType.Clamped;
            templateScrollRect.verticalScrollbar = scrollbarScrollbar;
            templateScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            templateScrollRect.verticalScrollbarSpacing = -3;
            templateScrollRect.scrollSensitivity = 20;

            Mask scrollRectMask = viewport.AddComponent<Mask>();
            scrollRectMask.showMaskGraphic = false;

            Image viewportImage = viewport.AddComponent<Image>();
            viewportImage.sprite = resources.Mask;
            viewportImage.type = Image.Type.Sliced;

            // Setup dropdown UI components.

            TextMeshProUGUI labelText = label.AddComponent<TextMeshProUGUI>();
            SetDefaultTextValues(labelText);
            labelText.alignment = TextAlignmentOptions.Left;

            Image arrowImage = arrow.AddComponent<Image>();
            arrowImage.sprite = resources.Dropdown;

            Image backgroundImage = button.AddComponent<Image>();
            backgroundImage.sprite = resources.Standard;
            backgroundImage.color = s_DefaultSelectableColor;
            backgroundImage.type = Image.Type.Sliced;

            RectTransform buttonRT = button.GetComponent<RectTransform>();
            buttonRT.anchorMin = new Vector2(0, 0);
            buttonRT.anchorMax = new Vector2(1, 1);
            buttonRT.sizeDelta = new Vector2(0, 0);

            CrossDropdown dropdown = root.AddComponent<CrossDropdown>();
            dropdown.targetGraphic = backgroundImage;
            dropdown.Template = template.GetComponent<RectTransform>();
            dropdown.CaptionText = labelText;
            dropdown.ItemText = itemLabelText;

            // Setting default Item list.
            itemLabelText.text = "Option A";
            dropdown.Options.Add(new TMP_Dropdown.OptionData {text = "Option A" });
            dropdown.Options.Add(new TMP_Dropdown.OptionData {text = "Option B" });
            dropdown.Options.Add(new TMP_Dropdown.OptionData {text = "Option C" });
            dropdown.RefreshShownValue();

            // Set up RectTransforms.

            RectTransform labelRT = label.GetComponent<RectTransform>();
            labelRT.anchorMin = Vector2.zero;
            labelRT.anchorMax = Vector2.one;
            labelRT.offsetMin = new Vector2(10, 6);
            labelRT.offsetMax = new Vector2(-25, -7);

            RectTransform arrowRT = arrow.GetComponent<RectTransform>();
            arrowRT.anchorMin = new Vector2(1, 0.5f);
            arrowRT.anchorMax = new Vector2(1, 0.5f);
            arrowRT.sizeDelta = new Vector2(20, 20);
            arrowRT.anchoredPosition = new Vector2(-15, 0);

            RectTransform templateRT = template.GetComponent<RectTransform>();
            templateRT.anchorMin = new Vector2(0, 0);
            templateRT.anchorMax = new Vector2(1, 0);
            templateRT.pivot = new Vector2(0.5f, 1);
            templateRT.anchoredPosition = new Vector2(0, 2);
            templateRT.sizeDelta = new Vector2(0, 150);

            RectTransform viewportRT = viewport.GetComponent<RectTransform>();
            viewportRT.anchorMin = new Vector2(0, 0);
            viewportRT.anchorMax = new Vector2(1, 1);
            viewportRT.sizeDelta = new Vector2(-18, 0);
            viewportRT.pivot = new Vector2(0, 1);

            RectTransform contentRT = content.GetComponent<RectTransform>();
            contentRT.anchorMin = new Vector2(0f, 1);
            contentRT.anchorMax = new Vector2(1f, 1);
            contentRT.pivot = new Vector2(0.5f, 1);
            contentRT.anchoredPosition = new Vector2(0, 0);
            contentRT.sizeDelta = new Vector2(0, 28);

            RectTransform itemRT = item.GetComponent<RectTransform>();
            itemRT.anchorMin = new Vector2(0, 0.5f);
            itemRT.anchorMax = new Vector2(1, 0.5f);
            itemRT.sizeDelta = new Vector2(0, 20);

            RectTransform itemBackgroundRT = itemBackground.GetComponent<RectTransform>();
            itemBackgroundRT.anchorMin = Vector2.zero;
            itemBackgroundRT.anchorMax = Vector2.one;
            itemBackgroundRT.sizeDelta = Vector2.zero;

            RectTransform itemCheckmarkRT = itemCheckmark.GetComponent<RectTransform>();
            itemCheckmarkRT.anchorMin = new Vector2(0, 0.5f);
            itemCheckmarkRT.anchorMax = new Vector2(0, 0.5f);
            itemCheckmarkRT.sizeDelta = new Vector2(20, 20);
            itemCheckmarkRT.anchoredPosition = new Vector2(10, 0);

            RectTransform itemLabelRT = itemLabel.GetComponent<RectTransform>();
            itemLabelRT.anchorMin = Vector2.zero;
            itemLabelRT.anchorMax = Vector2.one;
            itemLabelRT.offsetMin = new Vector2(20, 1);
            itemLabelRT.offsetMax = new Vector2(-10, -2);

            template.SetActive(false);

            ColorAndScaleTransitioner transitioner = root.AddComponent<ColorAndScaleTransitioner>();
            transitioner.Style = resources.DropdownStyle;
            
            return root;
        }
        
        /// <summary>
        /// Create the basic UGUI dropdown.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Dropdown
        ///         - Button
        ///             - Selector
        ///             - Label
        ///             - Arrow
        ///         - Template
        ///             - Viewport
        ///                 - Content
        ///                     - Item
        ///                         - Item Background
        ///                         - Item Checkmark
        ///                         - Item Label
        ///             - Scrollbar
        ///                 - Sliding Area
        ///                     - Handle
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateUguiDropdown(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Dropdown", s_ThickElementSize);
            GameObject button         = CreateUIObject("Button",          root);
            GameObject selector       = CreateUIObject("Selector",        button);
            GameObject label          = CreateUIObject("Label",           button);
            GameObject arrow          = CreateUIObject("Arrow",           button);
            GameObject template       = CreateUIObject("Template",        root);
            GameObject viewport       = CreateUIObject("Viewport",        template);
            GameObject content        = CreateUIObject("Content",         viewport);
            GameObject item           = CreateUIObject("Item",            content);
            GameObject itemBackground = CreateUIObject("Item Background", item);
            GameObject itemCheckmark  = CreateUIObject("Item Checkmark",  item);
            GameObject itemLabel      = CreateUIObject("Item Label",      item);

            ConfigureSelector(resources, selector);
            
            // Sub controls.

            GameObject scrollbar = CreateScrollbar(resources, true);
            scrollbar.name = "Scrollbar";
            SetParentAndAlign(scrollbar, template);

            CrossScrollbar scrollbarScrollbar = scrollbar.GetComponent<CrossScrollbar>();
            scrollbarScrollbar.SetDirection(Scrollbar.Direction.BottomToTop, true);
            Navigation nav = scrollbarScrollbar.navigation;
            nav.mode = Navigation.Mode.None;
            scrollbarScrollbar.navigation = nav;

            RectTransform vScrollbarRT = scrollbar.GetComponent<RectTransform>();
            vScrollbarRT.anchorMin = Vector2.right;
            vScrollbarRT.anchorMax = Vector2.one;
            vScrollbarRT.pivot = Vector2.one;
            vScrollbarRT.sizeDelta = new Vector2(vScrollbarRT.sizeDelta.x, 0);

            // Setup item UI components.

            TextMeshProUGUI itemLabelText = itemLabel.AddComponent<TextMeshProUGUI>();
            SetDefaultTextValues(itemLabelText);
            itemLabelText.alignment = TextAlignmentOptions.Left;

            Image itemBackgroundImage = itemBackground.AddComponent<Image>();
            itemBackgroundImage.color = new Color32(245, 245, 245, 255);

            Image itemCheckmarkImage = itemCheckmark.AddComponent<Image>();
            itemCheckmarkImage.sprite = resources.Checkmark;

            CrossUguiToggle itemCrossToggle = item.AddComponent<CrossUguiToggle>();
            itemCrossToggle.graphic = itemCheckmarkImage;
            itemCrossToggle.IsOn = true;
            
            ColorAndScaleTransitioner itemTransitioner = item.AddComponent<ColorAndScaleTransitioner>();
            itemTransitioner.Style = resources.DropdownItemStyle;

            // Setup template UI components.

            Image templateImage = template.AddComponent<Image>();
            templateImage.sprite = resources.Standard;
            templateImage.type = Image.Type.Sliced;

            ScrollRect templateScrollRect = template.AddComponent<ScrollRect>();
            templateScrollRect.content = (RectTransform)content.transform;
            templateScrollRect.viewport = (RectTransform)viewport.transform;
            templateScrollRect.horizontal = false;
            templateScrollRect.movementType = ScrollRect.MovementType.Clamped;
            templateScrollRect.verticalScrollbar = scrollbarScrollbar;
            templateScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            templateScrollRect.verticalScrollbarSpacing = -3;
            templateScrollRect.scrollSensitivity = 20;

            Mask scrollRectMask = viewport.AddComponent<Mask>();
            scrollRectMask.showMaskGraphic = false;

            Image viewportImage = viewport.AddComponent<Image>();
            viewportImage.sprite = resources.Mask;
            viewportImage.type = Image.Type.Sliced;

            // Setup dropdown UI components.

            TextMeshProUGUI labelText = label.AddComponent<TextMeshProUGUI>();
            SetDefaultTextValues(labelText);
            labelText.alignment = TextAlignmentOptions.Left;

            Image arrowImage = arrow.AddComponent<Image>();
            arrowImage.sprite = resources.Dropdown;

            Image backgroundImage = button.AddComponent<Image>();
            backgroundImage.sprite = resources.Standard;
            backgroundImage.color = s_DefaultSelectableColor;
            backgroundImage.type = Image.Type.Sliced;

            RectTransform buttonRT = button.GetComponent<RectTransform>();
            buttonRT.anchorMin = new Vector2(0, 0);
            buttonRT.anchorMax = new Vector2(1, 1);
            buttonRT.sizeDelta = new Vector2(0, 0);

            CrossUguiDropdown dropdown = root.AddComponent<CrossUguiDropdown>();
            dropdown.targetGraphic = backgroundImage;
            dropdown.template = template.GetComponent<RectTransform>();
            dropdown.captionText = labelText;
            dropdown.itemText = itemLabelText;
            UnityEventTools.AddVoidPersistentListener(itemCrossToggle.OnNavSelected, dropdown.OnToggleSelected);

            // Setting default Item list.
            itemLabelText.text = "Option A";
            dropdown.options.Add(new TMP_Dropdown.OptionData {text = "Option A" });
            dropdown.options.Add(new TMP_Dropdown.OptionData {text = "Option B" });
            dropdown.options.Add(new TMP_Dropdown.OptionData {text = "Option C" });
            dropdown.RefreshShownValue();

            // Set up RectTransforms.
            RectTransform labelRT = label.GetComponent<RectTransform>();
            labelRT.anchorMin = Vector2.zero;
            labelRT.anchorMax = Vector2.one;
            labelRT.offsetMin = new Vector2(10, 6);
            labelRT.offsetMax = new Vector2(-25, -7);

            RectTransform arrowRT = arrow.GetComponent<RectTransform>();
            arrowRT.anchorMin = new Vector2(1, 0.5f);
            arrowRT.anchorMax = new Vector2(1, 0.5f);
            arrowRT.sizeDelta = new Vector2(20, 20);
            arrowRT.anchoredPosition = new Vector2(-15, 0);

            RectTransform templateRT = template.GetComponent<RectTransform>();
            templateRT.anchorMin = new Vector2(0, 0);
            templateRT.anchorMax = new Vector2(1, 0);
            templateRT.pivot = new Vector2(0.5f, 1);
            templateRT.anchoredPosition = new Vector2(0, 2);
            templateRT.sizeDelta = new Vector2(0, 150);

            RectTransform viewportRT = viewport.GetComponent<RectTransform>();
            viewportRT.anchorMin = new Vector2(0, 0);
            viewportRT.anchorMax = new Vector2(1, 1);
            viewportRT.sizeDelta = new Vector2(-18, 0);
            viewportRT.pivot = new Vector2(0, 1);

            RectTransform contentRT = content.GetComponent<RectTransform>();
            contentRT.anchorMin = new Vector2(0f, 1);
            contentRT.anchorMax = new Vector2(1f, 1);
            contentRT.pivot = new Vector2(0.5f, 1);
            contentRT.anchoredPosition = new Vector2(0, 0);
            contentRT.sizeDelta = new Vector2(0, 28);

            RectTransform itemRT = item.GetComponent<RectTransform>();
            itemRT.anchorMin = new Vector2(0, 0.5f);
            itemRT.anchorMax = new Vector2(1, 0.5f);
            itemRT.sizeDelta = new Vector2(0, 20);

            RectTransform itemBackgroundRT = itemBackground.GetComponent<RectTransform>();
            itemBackgroundRT.anchorMin = Vector2.zero;
            itemBackgroundRT.anchorMax = Vector2.one;
            itemBackgroundRT.sizeDelta = Vector2.zero;

            RectTransform itemCheckmarkRT = itemCheckmark.GetComponent<RectTransform>();
            itemCheckmarkRT.anchorMin = new Vector2(0, 0.5f);
            itemCheckmarkRT.anchorMax = new Vector2(0, 0.5f);
            itemCheckmarkRT.sizeDelta = new Vector2(20, 20);
            itemCheckmarkRT.anchoredPosition = new Vector2(10, 0);

            RectTransform itemLabelRT = itemLabel.GetComponent<RectTransform>();
            itemLabelRT.anchorMin = Vector2.zero;
            itemLabelRT.anchorMax = Vector2.one;
            itemLabelRT.offsetMin = new Vector2(20, 1);
            itemLabelRT.offsetMax = new Vector2(-10, -2);

            template.SetActive(false);

            ColorAndScaleTransitioner transitioner = root.AddComponent<ColorAndScaleTransitioner>();
            transitioner.Style = resources.DropdownStyle;
            
            return root;
        }
        
        /// <summary>
        /// Create the basic UI Scrollview.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Scrollview
        ///         - Viewport
        ///             - Content
        ///         - Scrollbar Horizontal
        ///             - Sliding Area
        ///                 - Handle
        ///         - Scrollbar Vertical
        ///             - Sliding Area
        ///                 - Handle
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateScrollView(Resources resources)
        {
            GameObject root = CreateUIElementRoot("Scroll View", new Vector2(200, 200));
            GameObject viewport = CreateUIObject("Viewport", root);
            GameObject content = CreateUIObject("Content", viewport);

            // Sub controls.
            GameObject hScrollbar = CreateScrollbar(resources, true);
            hScrollbar.name = "Scrollbar Horizontal";
            SetParentAndAlign(hScrollbar, root);
            RectTransform hScrollbarRT = hScrollbar.GetComponent<RectTransform>();
            hScrollbarRT.anchorMin = Vector2.zero;
            hScrollbarRT.anchorMax = Vector2.right;
            hScrollbarRT.pivot = Vector2.zero;
            hScrollbarRT.sizeDelta = new Vector2(0, hScrollbarRT.sizeDelta.y);

            GameObject vScrollbar = CreateScrollbar(resources);
            vScrollbar.name = "Scrollbar Vertical";
            SetParentAndAlign(vScrollbar, root);
            vScrollbar.GetComponent<Scrollbar>().SetDirection(Scrollbar.Direction.BottomToTop, true);
            RectTransform vScrollbarRT = vScrollbar.GetComponent<RectTransform>();
            vScrollbarRT.anchorMin = Vector2.right;
            vScrollbarRT.anchorMax = Vector2.one;
            vScrollbarRT.pivot = Vector2.one;
            vScrollbarRT.sizeDelta = new Vector2(vScrollbarRT.sizeDelta.x, 0);

            // Setup RectTransforms.

            // Make viewport fill entire scroll view.
            RectTransform viewportRT = viewport.GetComponent<RectTransform>();
            viewportRT.anchorMin = Vector2.zero;
            viewportRT.anchorMax = Vector2.one;
            viewportRT.sizeDelta = Vector2.zero;
            viewportRT.pivot = Vector2.up;

            // Make context match viewpoprt width and be somewhat taller.
            // This will show the vertical scrollbar and not the horizontal one.
            RectTransform contentRT = content.GetComponent<RectTransform>();
            contentRT.anchorMin = Vector2.up;
            contentRT.anchorMax = Vector2.one;
            contentRT.sizeDelta = new Vector2(0, 300);
            contentRT.pivot = Vector2.up;

            // Setup UI components.
            ScrollRect scrollRect = root.AddComponent<ScrollRect>();
            scrollRect.content = contentRT;
            scrollRect.viewport = viewportRT;
            scrollRect.horizontalScrollbar = hScrollbar.GetComponent<Scrollbar>();
            scrollRect.verticalScrollbar = vScrollbar.GetComponent<Scrollbar>();
            scrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
            scrollRect.horizontalScrollbarSpacing = -3;
            scrollRect.verticalScrollbarSpacing = -3;
            scrollRect.scrollSensitivity = 20;

            Image rootImage = root.AddComponent<Image>();
            rootImage.sprite = resources.Background;
            rootImage.type = Image.Type.Sliced;
            rootImage.color = Color.white;

            Mask viewportMask = viewport.AddComponent<Mask>();
            viewportMask.showMaskGraphic = false;

            Image viewportImage = viewport.AddComponent<Image>();
            viewportImage.sprite = resources.Mask;
            viewportImage.type = Image.Type.Sliced;

            return root;
        }

        
        
        
        

        /// <summary>
        /// Create the basic UI Scrollbar.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Scrollbar
        ///         - Sliding Area
        ///             - Handle
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateScrollbar(Resources resources, bool skipSelector = false)
        {
            // Create GOs Hierarchy
            GameObject scrollbarRoot = CreateUIElementRoot("Scrollbar", s_ThinElementSize);
            GameObject selector = skipSelector ? null : CreateUIObject("Selector", scrollbarRoot);
            GameObject sliderArea = CreateUIObject("Sliding Area", scrollbarRoot);
            GameObject handle = CreateUIObject("Handle", sliderArea);
            
            ConfigureSelector(resources, selector);

            Image bgImage = scrollbarRoot.AddComponent<Image>();
            bgImage.sprite = resources.Background;
            bgImage.type = Image.Type.Sliced;
            bgImage.color = s_DefaultSelectableColor;

            Image handleImage = handle.AddComponent<Image>();
            handleImage.sprite = resources.Standard;
            handleImage.type = Image.Type.Sliced;
            handleImage.color = s_DefaultSelectableColor;

            RectTransform sliderAreaRect = sliderArea.GetComponent<RectTransform>();
            sliderAreaRect.sizeDelta = new Vector2(-20, -20);
            sliderAreaRect.anchorMin = Vector2.zero;
            sliderAreaRect.anchorMax = Vector2.one;

            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 20);

            CrossScrollbar scrollbar = scrollbarRoot.AddComponent<CrossScrollbar>();
            scrollbar.handleRect = handleRect;

            ColorAndScaleTransitioner transitioner = scrollbarRoot.AddComponent<ColorAndScaleTransitioner>();
            transitioner.Style = resources.ScrollbarStyle;
            
            return scrollbarRoot;
        }


    }
}
