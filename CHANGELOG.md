# Changelog

All notable end-user facing changes should be documented in this file.


## [1.2.3] 2025-09-01

### Fixed

- The reset in OnEnable in `OmniTransitioner` is now optional, and defaults to off. You can restore the old behavior by
  turning the "Reset on Enable" flag back on, on the component.

## [1.2.2] 2025-08-05

### Fixed

- Fixed a couple of editor-time warnings where the Pivot and Anchor modules were triggering a
  `RectTransformDimensionsChanged` during Awake.

### Changed

- Created a property to expose (readonly) the transition time for OmniTransitioner.

## [1.2.1] 2025-08-05

### Fixed

- Concurrent transition tweens of the same type should no longer cancel each other.

## [1.2.0] 2025-08-05

### Changed

- Now that they have override equivalents, the non-override UI components have had their names changed to include the
  text `Revamp`, to clarify that these are not overrides.
- All Revamp logic has been moved within a `CROSSGUARD_REVAMP` define guard, so they can be excluded from deployments
  that don't use them.
- The override components have been renamed from `CrossUguiToggle` to `CrossToggle` and from `CrossUguiDropdown` to
  `CrossDropdown`.
- `CrossSlideToggle`, `CrossIconToggle`, and `CrossTextToggle` are now deprecated, since their functionality can be
  achieved, using the `Checked` state with a `ModularTransitione`.
- Documentation has been updated.

## [1.1.11] 2025-08-04

### Fixed

- Fixed an issue where Modular Transitioner didn't initialize into Normal state properly.
- Fixed an issue where `CrossUguiToggle` didn't set the checked state correctly when being reactivated.

## [1.1.10] 2025-08-04

### Fixed

- Fixed some edge conditions associated with enabling/disabling components that are affected by transitioners.
- Fixed some minor construction-time errors.

## [1.1.9] 2025-08-03

### Fixed

- `CrossUguiToggle` should now support the Checked state correctly.

## [1.1.8] 2025-08-03

### Fixed

- Dropdowns should no longer try to do anything in `OnPointerClick` and `OnSubmit` when they're not interactable.
- Editor construction scripts should now use `RectMask2D` instead of an image mask.
- Editor construction scripts have been moved to the Editor folder.
- Fixed a typo in the documentation.

## [1.1.7] 2025-07-11

### Added

- Added `CrossUguiDropdown` which is descended from `TMP_Dropdown`, and uses `CrossUguiToggle` instead of
  `CrossToggle`. If using the Create menu, the setup will fix the selection visibility bug that occurs in stock
  `TMP_Dropdown`.
- Added a `NavSelectedEvent` event to `CrossUguiToggle`. This is for detecting when a toggle is selected by navigation
  events from gamepad or keyboard. (It has nothing to do with whether it has been toggled or not.)

### Fixed

- Added some safety checks to the tweens in `ColorAndScaleTransitioner`, in case the items they're tweening get
  deleted. (TODO: The modules should also have these safeties added.) This should fix some lost reference errors
  that were bubbling out of the tweener.

### Changed

- Modernized some code in `CrossInputField`.

## [1.1.6] 2025-07-10

### Added

- Added `OnDropdownOpened` and `OnDropdownClosed` events to `CrossDropdown`.

### Fixed

- Fixed a `NullReferenceException` in `CrossSlider` that occurred during `OnValidate` in the Editor.
- Corrected the ordering of the subheaders in this changelog.

### Changed

- Cleaned up old code in CrossDropdown.

## [1.1.5] 2025-07-09

### Added

- New `CrossUguiToggle` MonoBehaviour brings Crossguard features to legacy UGUI toggles.

### Changed

- Updated package acquisition guidance in documentation.
- Crossguard Toggle Groups now inreract with ICrossToggle, instead of CrossToggleBase, so that they are compatible with
  CrossUguiToggle.


## [1.1.4] 2025-07-09

### Changed

- Brought documentation up-to-date with current features.

### Removed

- Removed the Animation Transitioner. It wasn't a great sample, we don't use it, and we don't want to support it.


## [1.1.3] 2025-07-08

### Added

- Added two Layout related MonoBehaviours:
  - `MultiAxisContentSizeFitter` - Grows up to a maximum, and then grows on the other axis. Useful for alert boxes.
  - `ShrinkwrapLayoutElement` - A layout container that shrinks or stretches to fit its content. Useful for buttons.


## [1.1.2] 2025-07-07

### Added

- Local Rotation Module
- Image Fill Module
- Pivot Module
- Anchor Min Module
- Anchor Max Module
- Alpha Canvas Group Module

### Changed

- Module components are now tracked by Transform, rather than RectTransform. This allows us to potentially use the
  OmniTransitioner for things that are not UI elements.
- Renamed some modules to better fit their specific role.
- Reorganized some modules.

## [1.1.1] 2025-07-07

### Added

- Added the `OmniTransitioner` MonoBehaviour, which can be used to transition visual states on a non-Selectable.
- Added `OmniTransitionerState`. You can add as many of these as you want to an `OmniTransitioner` to define the states
  that it will use.

### Changed

- Pulled modular transition logic into a `ModularTransitionManager` that can be used by more than one thing.

## [1.1.0] 2025-06-24

### Added

- Added the `ModularTransitioner`, which is a new transitioner that allows you to specify component appearance with
  modular parts.
- Added a starting set of modules:
  - `Alpha (Renderer)` - Tweens the Alpha of the Canvas Renderer.
  - `Color (RGB)` - Tweens the Color (but not Alpha) of the Canvas Renderer.
  - `Color (RGBA)` - Tweens the Color and Alpha of the Canvas Renderer.
  - `Position` - Tweens the anchoredPosition of the RectTransform.
  - `Scale` - Tweens the Scale of the RectTransform.
  - `Sprite` - Swaps the Sprite of an Image.

### Fixed

- `ColorAndScaleTransitioner` now sets up the Checked style correctly.

### Changed

- Replaced the old pools with some new ones.
- Added more comments ton `AbstractTransitioner`.
- Migrated `FindComponent` from `ColorAndScaleTransitioner` to `AbstractTransitioner` so it can be used by other
  Transitioners.
- `AbstractTransitioner.ClearStateFlag` now automatically skips the `Normal` state, so Transitioners don't need to do
  this.

## [1.0.8] 2025-06-12

### Changed

- Integrated LitMotion to provide more robust tweening capabilities. This primarily impacted ColorAndScaleTweener, for
  now.

## [1.0.7] 2025-06-07

### Added

- Added a `Checked` state that can be used by toggles, if they want more expressive styling, when checked.

### Fixed

- Fixed a bug that caused the `SimpleSpriteSwapTransitioner` to behave badly if a sprite wasn't set on one of the
  states.

### Changed

- Updated documentation (and small corrections).

## [1.0.6] - 2023-11-06

### Fixed

- `CrossguardInputAdapter` should no longer throw an error about selecting while already selecting
   when selecting an InputField with the mouse after navigating with the keyboard.

## [1.0.5] - 2023-11-04

### Changed

- Updated README to prepare for deployment.

## [1.0.4] - 2023-11-04

### Added

- Added an `IsEditing` flag property to `CrossInputField` to indicate when it's actively editing text.

### Fixed

- Tab/Shift-Tab should no longer skip UI elements after a `CrossInputField`.
- `CrossSpinnerBase` should no longer throw a `NullReferenceException` in the editor.
- Creating an Input Field from the menu should no longer throw a `NullReferenceException`.
- Fixed missing transitions after Awake not called in Play mode in the Editor.
- Fixed asmdef name.
- Fixed internal links in documentation.

### Changed

- Package is now MIT licensed.
- Tweaked documentation formatting.
- Migrated documentation from Strapdown to Markdeep.
- Simplified some redundant code in `ControllerPromptManager.cs`.

## [1.0.3] - 2021-03-12

### Changed

- Form navigation with Tab and Shift-Tab should now skip disabled and
  non-interactable elements.
- Exposed a `Selectable` property for `CommandPrompt`, so that it can be set at
  runtime.
- Exposed `Min` and `Max` setters for `CrossNumberSpinner`, so that they can
  be set at runtime.

### Removed

- Removed some unwanted debug messages from CrossRadioGroup.

## [1.0.2] - 2021-02-26

### Added

- There is now a CrossTextToggle, which switches text when the button is
  toggled.

### Changed

- Improved behavior and fixed some bugs in Toggle Groups.
- ControllerPrompts should now only fire if they're active and enabled.
- AbstractTransitioner now exposes the `AxisNavMode`, in case it is needed
  elsewhere.
- Fixed non-Editor compile error in ColorAndScaleTransitioner.

## [1.0.1] - 2021-02-22

### Changed

- Corrected a bad link in the documentation.
- Adjusted repeat defaults in AxisControlHelper.
- Fixed a bug where Transitioners with Select on Enable on would sometimes
  select before there was an EventSystem available.

## [1.0.0] - 2021-02-16

### Changed

- Broke the Package out of the Test project, and moved it to its own repository.
- Added installation instructions to README.
- Began CHANGELOG.
