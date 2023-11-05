using TMPro;
using UnityEngine;

namespace HiddenAchievement.CrossguardUi
{
    /// <summary>
    /// 
    /// </summary>
    public class CrossNumberSpinner : CrossSpinnerBase
    {
        [SerializeField]
        private int _step = 1;
        public override int Step => _step;

        [SerializeField]
        private int _min = 0;

        public override int Min
        {
            get => _min;
            set
            {
                _min = value;
                if (Value < _min)
                {
                    Value = _min;
                }
                else
                {
                    _axisControlHelper.UpdateButtons();
                }
            }
        }

        [SerializeField]
        private int _max = 10;
        
        public override int Max
        {
            get => _max;
            set
            {
                _max = value;
                if (Value > _max)
                {
                    Value = _max;
                }
                else
                {
                    _axisControlHelper.UpdateButtons();
                }
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
        
        protected override void UpdateVisuals()
        {
            if (_readout == null) return;
            _readout.text = Value.ToString();
        }
    }
}
