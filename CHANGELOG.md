# Changelog

All notable end-user facing changes should be documented in this file.


## [1.1.1] 2025-07-07

### Changed

- Pulled modular transition logic into a `ModularTransitionManager` that can be used by more than one thing.

### Added

- Added the `OmniTransitioner` MonoBehaviour, which can be used to transition visual states on a non-Selectable.
- Added `OmniTransitionerState`. You can add as many of these as you want to an `OmniTransitioner` to define the states
  that it will use.


## [1.1.0] 2025-06-24

### Changed

- Replaced the old pools with some new ones.
- Added more comments ton `AbstractTransitioner`.
- Migrated `FindComponent` from `ColorAndScaleTransitioner` to `AbstractTransitioner` so it can be used by other
  Transitioners.
- `AbstractTransitioner.ClearStateFlag` now automatically skips the `Normal` state, so Transitioners don't need to do
  this.

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

## [1.0.8] 2025-06-12

### Changed

- Integrated LitMotion to provide more robust tweening capabilities. This primarily impacted ColorAndScaleTweener, for
  now.

## [1.0.7] 2025-06-07

### Changed

- Updated documentation (and small corrections).

### Added

- Added a `Checked` state that can be used by toggles, if they want more expressive styling, when checked.

### Fixed

- Fixed a bug that caused the `SimpleSpriteSwapTransitioner` to behave badly if a sprite wasn't set on one of the
  states.


## [1.0.6] - 2023-11-06

### Fixed

- `CrossguardInputAdapter` should no longer throw an error about selecting while already selecting
   when selecting an InputField with the mouse after navigating with the keyboard.

## [1.0.5] - 2023-11-04

### Changed

- Updated README to prepare for deployment.

## [1.0.4] - 2023-11-04

### Changed

- Package is now MIT licensed.
- Tweaked documentation formatting.
- Migrated documentation from Strapdown to Markdeep.
- Simplified some redundant code in `ControllerPromptManager.cs`.

### Added

- Added an `IsEditing` flag property to `CrossInputField` to indicate when it's actively editing text.

### Fixed

- Tab/Shift-Tab should no longer skip UI elements after a `CrossInputField`.
- `CrossSpinnerBase` should no longer throw a `NullReferenceException` in the editor.
- Creating an Input Field from the menu should no longer throw a `NullReferenceException`.
- Fixed missing transitions after Awake not called in Play mode in the Editor.
- Fixed asmdef name.
- Fixed internal links in documentation.

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

### Changed

- Improved behavior and fixed some bugs in Toggle Groups.
- ControllerPrompts should now only fire if they're active and enabled.
- AbstractTransitioner now exposes the `AxisNavMode`, in case it is needed
  elsewhere.
- Fixed non-Editor compile error in ColorAndScaleTransitioner.

### Added

- There is now a CrossTextToggle, which switches text when the button is
  toggled.


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
