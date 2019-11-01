using Nekoyume.Game.Item;
using UniRx;

namespace Nekoyume.UI.Model
{
    public class CombinationMaterial : CountEditableItem
    {
        public readonly ReactiveProperty<bool> IsRequireNCG = new ReactiveProperty<bool>(false);
        
        public CombinationMaterial(ItemBase item, int count, int minCount, int maxCount) : base(item, count, minCount, maxCount)
        {
        }

        public override void Dispose()
        {
            IsRequireNCG.Dispose();
            base.Dispose();
        }
    }
}
