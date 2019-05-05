using System;
using System.Collections.Generic;
using Nekoyume.Game.Controller;
using Nekoyume.UI.Module;
using UniRx;
using UnityEngine.UI;

namespace Nekoyume.UI
{
    public class SelectItemCountPopup : Widget
    {
        private const string CountStringFormat = "총 {0}개";

        public Text titleText;
        public Text countText;
        public Button minusButton;
        public Button plusButton;
        public Button cancelButton;
        public Button okButton;
        public SimpleCountableItemView itemView;
        
        private Model.SelectItemCountPopup _data;
        private readonly List<IDisposable> _disposablesForAwake = new List<IDisposable>();
        private readonly List<IDisposable> _disposablesForSetDate = new List<IDisposable>();

        #region Mono

        protected override void Awake()
        {
            base.Awake();

            this.ComponentFieldsNotNullTest();
            
            minusButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _data.onClickMinus.OnNext(_data);
                    AudioController.PlayClick();
                })
                .AddTo(_disposablesForAwake);

            plusButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _data.onClickPlus.OnNext(_data);
                    AudioController.PlayClick();
                })
                .AddTo(_disposablesForAwake);

            cancelButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _data.onClickClose.OnNext(_data);
                    AudioController.PlayCancel();
                })
                .AddTo(_disposablesForAwake);

            okButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _data.onClickSubmit.OnNext(_data);
                    AudioController.PlayClick();
                })
                .AddTo(_disposablesForAwake);
        }

        private void OnDestroy()
        {
            _disposablesForAwake.DisposeAllAndClear();
            Clear();
        }

        #endregion

        public void Pop(Model.SelectItemCountPopup data)
        {
            if (ReferenceEquals(data, null))
            {
                return;
            }

            AudioController.PlayPopup();
            SetData(data);
            base.Show();
        }

        private void SetData(Model.SelectItemCountPopup data)
        {
            if (ReferenceEquals(data, null))
            {
                Clear();
                return;
            }
            
            _disposablesForSetDate.DisposeAllAndClear();
            _data = data;
            _data.count.Subscribe(SetCount).AddTo(_disposablesForSetDate);
            itemView.SetData(_data.item.Value);
            
            UpdateView();
        }
        
        private void Clear()
        {
            itemView.Clear();
            _data = null;
            _disposablesForSetDate.DisposeAllAndClear();
            
            UpdateView();
        }

        private void UpdateView()
        {
            if (ReferenceEquals(_data, null))
            {
                SetCount(0);
                return;
            }
            
            SetCount(_data.count.Value);
        }
        
        private void SetCount(int count)
        {
            countText.text = string.Format(CountStringFormat, count);
        }
    }
}
