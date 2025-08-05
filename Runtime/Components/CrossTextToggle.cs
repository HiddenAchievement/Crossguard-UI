#if CROSS_REVAMP

using System;
using TMPro;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// A toggle which swaps text, based on on/off state.
    /// </summary>
    [Obsolete("No longer necessary. Effect can be accomplished with the Checked state.", false)]
    public class CrossTextToggle : CrossToggleBase
    {
        [SerializeField]
        private TextMeshProUGUI _label = null;
        public TextMeshProUGUI Label
        {
            get => _label;
            set
            {
                _label = value;
                PlayEffect(true);
            }
        }
        
        [SerializeField]
        private string _onText = null;
        public string OnText
        {
            get => _onText;
            set
            {
                _onText = value;
                PlayEffect(true);
            }
        }
        
        [SerializeField]
        private string _offText = null;
        public string OffText
        {
            get => _offText;
            set
            {
                _offText = value;
                PlayEffect(true);
            }
        }
        
        protected override void PlayEffect(bool instant)
        {
            if (_label == null) return;

            _label.text = IsOn ? _onText : _offText;
        }
    }
}

#endif // CROSS_REVAMP
