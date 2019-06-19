using System;
using Nekoyume.Data.Table;
using Nekoyume.Game.Skill;
using UnityEngine;

namespace Nekoyume.Game.Item
{
    [Serializable]
    public class Weapon : Equipment
    {
        private const string SpritePath = "images/equipment/{0}";

        public static Sprite GetSprite(Equipment equipment = null)
        {
            var id = equipment?.Data.resourceId;
            return Resources.Load<Sprite>(string.Format(SpritePath, id));
        }

        public Weapon(Data.Table.Item data, SkillBase skillBase = null)
            : base(data, skillBase)
        {
        }
    }
}
