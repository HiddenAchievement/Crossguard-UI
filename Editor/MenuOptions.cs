using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// Provides Menu Options for creating CrossGuard UI elements.
    /// </summary>
    internal static class MenuOptions
    {
        
        [MenuItem("GameObject/UI/Crossguard UI/Button - Text", false, 1000)]
        public static void AddTextButton(MenuCommand menuCommand)
        {
            GameObject go = CrossDefaultControls.CreateTextButton(GetStandardResources());

            // Override font size
            // TMP_Text textComponent = go.GetComponentInChildren<TMP_Text>();
            // textComponent.fontSize = 24;

            PlaceUIElementRoot(go, menuCommand);
        }
        
        [MenuItem("GameObject/UI/Crossguard UI/Button - Icon", false, 1001)]
        public static void AddIconButton(MenuCommand menuCommand)
        {
            GameObject go = CrossDefaultControls.CreateIconButton(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

#if CROSS_REVAMP
        [MenuItem("GameObject/UI/Crossguard UI/Toggle (Revamp)", false, 1100)]
        public static void AddToggle(MenuCommand menuCommand)
        {
            GameObject go = CrossDefaultControls.CreateRevampToggle(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);            
        }
#endif // CROSS_REVAMP

        [MenuItem("GameObject/UI/Crossguard UI/Toggle", false, 1104)]
        public static void AddUguiToggle(MenuCommand menuCommand)
        {
            GameObject go = CrossDefaultControls.CreateUguiToggle(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem("GameObject/UI/Crossguard UI/Spinner - Text", false, 1200)]
        public static void AddTextSpinner(MenuCommand menuCommand)
        {
            GameObject go = CrossDefaultControls.CreateTextSpinner(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }
        
        [MenuItem("GameObject/UI/Crossguard UI/Spinner - Icon", false, 1201)]
        public static void AddIconSpinner(MenuCommand menuCommand)
        {
            GameObject go = CrossDefaultControls.CreateIconSpinner(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);           
        }
        
        [MenuItem("GameObject/UI/Crossguard UI/Spinner - Number", false, 1202)]
        public static void AddNumberSpinner(MenuCommand menuCommand)
        {
            GameObject go = CrossDefaultControls.CreateNumberSpinner(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);           
        }
        
        [MenuItem("GameObject/UI/Crossguard UI/Slider", false, 1300)]
        public static void AddSlider(MenuCommand menuCommand)
        {
            GameObject go = CrossDefaultControls.CreateSlider(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);  
        }
        
        [MenuItem("GameObject/UI/Crossguard UI/Input Field", false, 1302)]
        public static void AddInputField(MenuCommand menuCommand)
        {
            GameObject go = CrossDefaultControls.CreateInputField(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);     
        }
#if CROSS_REVAMP
        [MenuItem("GameObject/UI/Crossguard UI/Dropdown (Revamp)", false, 1301)]
        public static void AddDropdown(MenuCommand menuCommand)
        {
            GameObject go = CrossDefaultControls.CreateRevampDropdown(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);              
        }
#endif // CROSS_REVAMP

        [MenuItem("GameObject/UI/Crossguard UI/Dropdown", false, 1301)]
        public static void AddUguiDropdown(MenuCommand menuCommand)
        {
            GameObject go = CrossDefaultControls.CreateUguiDropdown(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);              
        }
        
        [MenuItem("GameObject/UI/Crossguard UI/Scroll View", false, 1303)]
        public static void AddScrollView(MenuCommand menuCommand)
        {
            GameObject go = CrossDefaultControls.CreateScrollView(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);  
        }

        [MenuItem("GameObject/UI/Crossguard UI/Scrollbar", false, 1304)]
        public static void AddScrollbar(MenuCommand menuCommand)
        {
            GameObject go = CrossDefaultControls.CreateScrollbar(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);       
        }
        
        private const string kUILayerName = "UI";
        
        private const string kStandardSpritePath = "UI/Skin/UISprite.psd";
        private const string kBackgroundSpritePath = "UI/Skin/Background.psd";
        private const string kInputFieldBackgroundPath = "UI/Skin/InputFieldBackground.psd";
        private const string kKnobPath = "UI/Skin/Knob.psd";
        private const string kCheckmarkPath = "UI/Skin/Checkmark.psd";
        private const string kDropdownArrowPath = "UI/Skin/DropdownArrow.psd";
        private const string kMaskPath = "UI/Skin/UIMask.psd";

        private const string kStylePath = "Packages/com.hiddenachievement.crossguardui/Content/Default Styles/";

        private const string kButtonStylePath = kStylePath + "Button Style.asset";
        private const string kDropdownStylePath = kStylePath + "Dropdown Style.asset";
        private const string kDropdownItemStylePath = kStylePath + "Dropdown Item Style.asset";
        private const string kInputFieldStylePath = kStylePath + "Input Field Style.asset";
        private const string kScrollbarStylePath = kStylePath + "Scrollbar Style.asset";
        private const string kSliderStylePath = kStylePath + "Slider Style.asset";
        private const string kSpinnerStylePath = kStylePath + "Spinner Style.asset";
        private const string kSpinnerButtonStylePath = kStylePath + "Spinner Button Style.asset";
        private const string kToggleStylePath = kStylePath + "Toggle Style.asset";
        private const string kIconToggleStylePath = kStylePath + "Icon Toggle Style.asset";
        private const string kSlideToggleStylePath = kStylePath + "Slide Toggle Style.asset";


        private static CrossDefaultControls.Resources s_StandardResources;

        private static CrossDefaultControls.Resources GetStandardResources()
        {
            if (s_StandardResources.Standard == null)
            {
                s_StandardResources.Standard = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
                s_StandardResources.Background = AssetDatabase.GetBuiltinExtraResource<Sprite>(kBackgroundSpritePath);
                s_StandardResources.InputField =
                    AssetDatabase.GetBuiltinExtraResource<Sprite>(kInputFieldBackgroundPath);
                s_StandardResources.Knob = AssetDatabase.GetBuiltinExtraResource<Sprite>(kKnobPath);
                s_StandardResources.Checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>(kCheckmarkPath);
                s_StandardResources.Dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>(kDropdownArrowPath);
                s_StandardResources.Mask = AssetDatabase.GetBuiltinExtraResource<Sprite>(kMaskPath);

                s_StandardResources.ButtonStyle = AssetDatabase.LoadAssetAtPath<ColorAndScaleStyle>(kButtonStylePath);
                s_StandardResources.DropdownStyle =
                    AssetDatabase.LoadAssetAtPath<ColorAndScaleStyle>(kDropdownStylePath);
                s_StandardResources.DropdownItemStyle =
                    AssetDatabase.LoadAssetAtPath<ColorAndScaleStyle>(kDropdownItemStylePath);
                s_StandardResources.InputFieldStyle =
                    AssetDatabase.LoadAssetAtPath<ColorAndScaleStyle>(kInputFieldStylePath);
                s_StandardResources.ScrollbarStyle =
                    AssetDatabase.LoadAssetAtPath<ColorAndScaleStyle>(kScrollbarStylePath);
                s_StandardResources.SliderStyle = AssetDatabase.LoadAssetAtPath<ColorAndScaleStyle>(kSliderStylePath);
                s_StandardResources.SpinnerStyle = AssetDatabase.LoadAssetAtPath<ColorAndScaleStyle>(kSpinnerStylePath);
                s_StandardResources.SpinnerButtonStyle =
                    AssetDatabase.LoadAssetAtPath<ColorAndScaleStyle>(kSpinnerButtonStylePath);
                s_StandardResources.ToggleStyle = AssetDatabase.LoadAssetAtPath<ColorAndScaleStyle>(kToggleStylePath);
                s_StandardResources.IconToggleStyle =
                    AssetDatabase.LoadAssetAtPath<ColorAndScaleStyle>(kIconToggleStylePath);
                s_StandardResources.SlideToggleStyle =
                    AssetDatabase.LoadAssetAtPath<ColorAndScaleStyle>(kSlideToggleStylePath);
            }

            return s_StandardResources;
        }

        private static void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
        {
            SceneView sceneView = SceneView.lastActiveSceneView;

            // Couldn't find a SceneView. Don't set position.
            if (sceneView == null || sceneView.camera == null)
                return;

            // Create world space Plane from canvas position.
            Vector2 localPlanePosition;
            Camera camera = sceneView.camera;
            Vector3 position = Vector3.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform,
                new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2), camera, out localPlanePosition))
            {
                // Adjust for canvas pivot
                localPlanePosition.x = localPlanePosition.x + canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
                localPlanePosition.y = localPlanePosition.y + canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;

                localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRTransform.sizeDelta.x);
                localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRTransform.sizeDelta.y);

                // Adjust for anchoring
                position.x = localPlanePosition.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
                position.y = localPlanePosition.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;

                Vector3 minLocalPosition;
                minLocalPosition.x = canvasRTransform.sizeDelta.x * (0 - canvasRTransform.pivot.x) +
                                     itemTransform.sizeDelta.x * itemTransform.pivot.x;
                minLocalPosition.y = canvasRTransform.sizeDelta.y * (0 - canvasRTransform.pivot.y) +
                                     itemTransform.sizeDelta.y * itemTransform.pivot.y;

                Vector3 maxLocalPosition;
                maxLocalPosition.x = canvasRTransform.sizeDelta.x * (1 - canvasRTransform.pivot.x) -
                                     itemTransform.sizeDelta.x * itemTransform.pivot.x;
                maxLocalPosition.y = canvasRTransform.sizeDelta.y * (1 - canvasRTransform.pivot.y) -
                                     itemTransform.sizeDelta.y * itemTransform.pivot.y;

                position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
                position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
            }

            itemTransform.anchoredPosition = position;
            itemTransform.localRotation = Quaternion.identity;
            itemTransform.localScale = Vector3.one;
        }
        
        private static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
        {
            GameObject parent = menuCommand.context as GameObject;
            bool explicitParentChoice = true;
            if (parent == null)
            {
                parent = GetOrCreateCanvasGameObject();
                explicitParentChoice = false;

                // If in Prefab Mode, Canvas has to be part of Prefab contents,
                // otherwise use Prefab root instead.
                PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage != null && !prefabStage.IsPartOfPrefabContents(parent))
                    parent = prefabStage.prefabContentsRoot;
            }
            if (parent.GetComponentsInParent<Canvas>(true).Length == 0)
            {
                // Create canvas under context GameObject,
                // and make that be the parent which UI element is added under.
                GameObject canvas = MenuOptions.CreateNewUI();
                Undo.SetTransformParent(canvas.transform, parent.transform, "");
                parent = canvas;
            }

            GameObjectUtility.EnsureUniqueNameForSibling(element);

            SetParentAndAlign(element, parent);
            if (!explicitParentChoice) // not a context click, so center in sceneview
                SetPositionVisibleinSceneView(parent.GetComponent<RectTransform>(), element.GetComponent<RectTransform>());

            // This call ensure any change made to created Objects after they where registered will be part of the Undo.
            Undo.RegisterFullObjectHierarchyUndo(parent == null ? element : parent, "");

            // We have to fix up the undo name since the name of the object was only known after reparenting it.
            Undo.SetCurrentGroupName("Create " + element.name);

            Selection.activeGameObject = element;
        }

        private static void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (parent == null)
                return;

            Undo.SetTransformParent(child.transform, parent.transform, "");

            RectTransform rectTransform = child.transform as RectTransform;
            if (rectTransform)
            {
                rectTransform.anchoredPosition = Vector2.zero;
                Vector3 localPosition = rectTransform.localPosition;
                localPosition.z = 0;
                rectTransform.localPosition = localPosition;
            }
            else
            {
                child.transform.localPosition = Vector3.zero;
            }
            child.transform.localRotation = Quaternion.identity;
            child.transform.localScale = Vector3.one;

            SetLayerRecursively(child, parent.layer);
        }

        private static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform t = go.transform;
            for (int i = 0; i < t.childCount; i++)
                SetLayerRecursively(t.GetChild(i).gameObject, layer);
        }
        
        static public GameObject GetOrCreateCanvasGameObject()
        {
            GameObject selectedGo = Selection.activeGameObject;

            // Try to find a gameobject that is the selected GO or one if its parents.
            Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
            if (IsValidCanvas(canvas))
                return canvas.gameObject;

            // No canvas in selection or its parents? Then use any valid canvas.
            // We have to find all loaded Canvases, not just the ones in main scenes.
            Canvas[] canvasArray = StageUtility.GetCurrentStageHandle().FindComponentsOfType<Canvas>();
            for (int i = 0; i < canvasArray.Length; i++)
                if (IsValidCanvas(canvasArray[i]))
                    return canvasArray[i].gameObject;

            // No canvas in the scene at all? Then create a new one.
            return CreateNewUI();
        }

        static bool IsValidCanvas(Canvas canvas)
        {
            if (canvas == null || !canvas.gameObject.activeInHierarchy)
                return false;

            // It's important that the non-editable canvas from a prefab scene won't be rejected,
            // but canvases not visible in the Hierarchy at all do. Don't check for HideAndDontSave.
            if (EditorUtility.IsPersistent(canvas) || (canvas.hideFlags & HideFlags.HideInHierarchy) != 0)
                return false;

            if (StageUtility.GetStageHandle(canvas.gameObject) != StageUtility.GetCurrentStageHandle())
                return false;

            return true;
        }
        
        static public GameObject CreateNewUI()
        {
            // Root for the UI
            var root = ObjectFactory.CreateGameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            root.layer = LayerMask.NameToLayer(kUILayerName);
            Canvas canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            // Works for all stages.
            StageUtility.PlaceGameObjectInCurrentStage(root);
            bool customScene = false;
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                Undo.SetTransformParent(root.transform, prefabStage.prefabContentsRoot.transform, "");
                customScene = true;
            }

            Undo.SetCurrentGroupName("Create " + root.name);

            // If there is no event system add one...
            // No need to place event system in custom scene as these are temporary anyway.
            // It can be argued for or against placing it in the user scenes,
            // but let's not modify scene user is not currently looking at.
            if (!customScene)
                CreateEventSystem(false);
            return root;
        }
        
        private static void CreateEventSystem(bool select)
        {
            CreateEventSystem(select, null);
        }


        private static void CreateEventSystem(bool select, GameObject parent)
        {
            var esys = UnityEngine.Object.FindObjectOfType<EventSystem>();
            if (esys == null)
            {
                var eventSystem = new GameObject("EventSystem");
                GameObjectUtility.SetParentAndAlign(eventSystem, parent);
                esys = eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();

                Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
            }

            if (select && esys != null)
            {
                Selection.activeGameObject = esys.gameObject;
            }
        }
        
        
        
    }
}