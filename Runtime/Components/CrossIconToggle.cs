#if CROSS_REVAMP

using System;
using UnityEngine;
using UnityEngine.UI;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// A Toggle that swaps icons when you click on it.
    /// </summary>
    [Obsolete("No longer necessary. Effect can be accomplished with the Checked state.", false)]
    public class CrossIconToggle : CrossToggleBase
    {
        [SerializeField]
        private Image _icon = null;
        public Image Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                PlayEffect(true);
            }
        }
        
        [SerializeField]
        private Sprite _onIcon = null;
        public Sprite OnIcon
        {
            get => _onIcon;
            set
            {
                _onIcon = value;
                PlayEffect(true);
            }
        }

        [SerializeField]
        private Sprite _offIcon = null;
        public Sprite OffIcon
        {
            get => _offIcon;
            set
            {
                _offIcon = value;
                PlayEffect(true);
            }
        }

        protected override void PlayEffect(bool instant)
        {
            if (_icon == null) return;

            _icon.overrideSprite = IsOn ? _onIcon : _offIcon;
        }
    }
}

#endif // CROSS_REVAMP
