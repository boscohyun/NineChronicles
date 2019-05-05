﻿using System;
using EnhancedUI.EnhancedScroller;
using Nekoyume.UI.Module;
using UniRx;
using UnityEngine;

namespace Nekoyume.UI.Scroller
{
    [RequireComponent(typeof(RectTransform))]
    public class InventoryCellView : EnhancedScrollerCellView
    {
        public InventoryItemView[] items;

        #region Mono

        private void Awake()
        {
            this.ComponentFieldsNotNullTest();
        }

        private void OnDisable()
        {
            Clear();
        }

        #endregion
        
        public void SetData(ReactiveCollection<Model.InventoryItem> dataList, int firstIndex)
        {
            if (ReferenceEquals(dataList, null))
            {
                Clear();
                return;
            }
            
            var dataCount = dataList.Count;
            for (int i = 0; i < items.Length; i++)
            {
                var index = firstIndex + i;
                var item = items[i];
                if (index < dataCount)
                {
                    item.SetData(dataList[index]);
                    item.gameObject.SetActive(true);
                }
                else
                {
                    item.gameObject.SetActive(false);
                }
            }
        }

        public void Clear()
        {
            foreach (var item in items)
            {
                item.Clear();
            }
        }
    }
}
