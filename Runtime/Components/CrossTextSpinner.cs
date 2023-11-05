using TMPro;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// A spinner that contains a list of strings.
    /// </summary>
    public class CrossTextSpinner : CrossSpinnerBase
    {
        [SerializeField]
        private string[] _options = null;
        public string[] Options
        {
            get => _options;
            set
            {
                _options = value;
                UpdateVisuals();
            }
        }

        [SerializeField]
        private TextMeshProUGUI _readout = null;
        public TextMeshProUGUI Readout
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
        public override int Max => _options?.Length - 1 ?? 0;
        protected override void UpdateVisuals()
        {
            if (_options == null) return;
            if (_readout == null) return;
            _readout.text = _options[Value];
        }
    }
}
