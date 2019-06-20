﻿using System;
using System.Linq;
using ContainerLibrary;
using PortableStorage.Global;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PortableStorage.Items.Special
{
	public class NinjaArsenalBelt : BaseBag
	{
		// note: if I'm not lazy implement ammo-like system where Ninja's Arsenal Belt doesn't replenish throwable items instead when player uses it it checks whether any bag has it and use it from that, also draw the amount in the bottom-right corner of the slot

		public override string Texture => "PortableStorage/Textures/Items/NinjaArsenalBelt";
			
		public NinjaArsenalBelt()
		{
			Handler = new ItemHandler(9);
			Handler.OnContentsChanged += slot => item.SyncBag();
			Handler.IsItemValid += (slot, item) => item.thrown;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ninja's Arsenal Belt");
			Tooltip.SetDefault($"Stores {Handler.Slots} stacks of throwing supplies");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

			item.width = 26;
			item.height = 30;
		}

		public override void UpdateInventory(Player player)
		{
			if (UI != null) return;

			for (int i = 0; i < player.inventory.Length; i++)
			{
				Item item = player.inventory[i];
				if (item == null || item.IsAir || !item.thrown || item.stack == item.maxStack) continue;

				Item itemBag = Handler.Items.FirstOrDefault(x => x.type == item.type && x.stack > 1);
				if (itemBag != null)
				{
					int count = Math.Min(item.maxStack - item.stack, itemBag.stack - 1);
					item.stack += count;
					itemBag.stack -= count;
					if (itemBag.stack <= 0) itemBag.TurnToAir();
				}
			}
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.NinjaShirt);
			recipe.AddIngredient(ItemID.IronChainmail, 3);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}