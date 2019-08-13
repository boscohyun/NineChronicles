﻿using Nekoyume.BlockChain;
using Nekoyume.Data;
using Nekoyume.Game.Item;
using System.Linq;
using UnityEngine;

namespace Nekoyume.UI.Model
{
    public class RecipeInfo
    {
        public class MaterialInfo
        {
            public int id;
            public int amount = 1;
            public Sprite sprite;
            public bool isEnough;
            public bool isObtained;

            public MaterialInfo(int id, Sprite sprite, int count = 1)
            {
                this.id = id;
                this.sprite = sprite;
                var inventory = States.Instance.currentAvatarState.Value.inventory;
                isEnough = inventory.HasItem(id, count);
                isObtained = true;
            }
        }

        public int recipeId;
        public int resultId;
        public int resultAmount = 1;
        public string resultName;
        public Sprite resultSprite;
        public MaterialInfo[] materialInfos = new MaterialInfo[5];

        public RecipeInfo(int id, int resultId, params int[] materialIds)
        {
            recipeId = id;
            this.resultId = resultId;
            resultName = GetEquipmentName(resultId);
            resultSprite = ItemBase.GetSprite(resultId);

            for (int i = 0; i < materialInfos.Length; ++i)
            {
                var sprite = ItemBase.GetSprite(materialIds[i]);
                int count = materialInfos.Count(item => item != null && item.id == materialIds[i]) + 1;
                materialInfos[i] = new MaterialInfo(materialIds[i], sprite, count);
            }
        }

        private string GetEquipmentName(int id)
        {
            if (id == 0) return string.Empty;
            var equips = Tables.instance.ItemEquipment;
            if (equips.ContainsKey(id))
            {
                return equips[id].name;
            }
            else
            {
                Debug.LogError("Item not found!");
                return string.Empty;
            }
        }
    }
}
