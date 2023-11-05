using UnityEngine;
using UnityEngine.UI;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// 
    /// </summary>
    public class CrossIconSpinner : CrossSpinnerBase
    {
        [SerializeField]
        private Sprite[] _icons = null;

        public Sprite[] Icons
        {
            get => _icons;
            set
            {
                _icons = value;
                UpdateVisuals();
            }
        }
        
        
        [SerializeField]
        private Image _readout = null;
        public Image Readout
        {
            get => _readout;
            set
            {
                _readout = value;
                UpdateVisuals();
            }
        }

        public override int Step => 1;
        public override int Min => 0;
        public override int Max => _icons?.Length - 1 ?? 0;
        
        protected override void UpdateVisuals()
        {
            if (_readout == null) return;
            if (_icons == null) return;
            _readout.overrideSprite = _icons[Value];
        }
    }
}
