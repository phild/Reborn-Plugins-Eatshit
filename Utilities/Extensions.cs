using System.Collections.Generic;
using System.Linq;
using ff14bot.Enums;
using ff14bot.Managers;

namespace EatShit.Utilities
{
    public static class Extensions
    {
        /// <summary>
        ///     Filter bagslots to only those that can be used to give the Well Fed Buff
        /// </summary>
        public static IEnumerable<BagSlot> FoodItems(this IEnumerable<BagSlot> inventory)
        {
            return InventoryManager.FilledSlots.Where(x => !x.IsCollectable && (int)x.Item.ItemRole == 5);
        }

		private static bool IsFoodItem(this BagSlot slot)
		{
			return (slot.Item.EquipmentCatagory == ItemUiCategory.Meal || slot.Item.EquipmentCatagory == ItemUiCategory.Ingredient);
		}

		public static IEnumerable<BagSlot> GetFoodItems(this IEnumerable<BagSlot> bags)
		{
			return bags
				.Where(s => s.IsFoodItem());
		}

		public static bool ContainsFooditem(this IEnumerable<BagSlot> bags, string foodName)
		{
			return bags
				.Select(s => s.EnglishName)
				.Contains(foodName);
			
		}

		public static BagSlot GetFoodItem(this IEnumerable<BagSlot> bags, string foodName)
		{
			return bags.First(s => s.EnglishName == foodName);
		}
	}
}