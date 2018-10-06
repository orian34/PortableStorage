﻿using System.Collections.Generic;
using BaseLibrary.UI;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using PortableStorage.Items.Bags;
using PortableStorage.UI;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using ItemSlot = On.Terraria.UI.ItemSlot;

namespace PortableStorage
{
	public partial class PortableStorage : Mod
	{
		public static PortableStorage Instance;
		public int BagID;

		public GUI<BagUI> BagUI;

		public override void Load()
		{
			Instance = this;

			BagUI = Utility.SetupGUI<BagUI>();
			BagUI.Visible = true;

			On.Terraria.UI.UIElement.GetElementAt += UIElement_GetElementAt;
			ItemSlot.Handle_1 += ItemSlot_Handle;
			ItemSlot.LeftClick_1 += ItemSlot_LeftClick;
		}

		public override void Unload()
		{
			Utility.UnloadNullableTypes();
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int InventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

			if (BagUI != null && InventoryIndex != -1) layers.Insert(InventoryIndex + 1, BagUI.InterfaceLayer);
		}
	}

	public partial class PortableStorage
	{
		private UIElement UIElement_GetElementAt(On.Terraria.UI.UIElement.orig_GetElementAt orig, UIElement self, Vector2 point)
		{
			if (self is BagUI ui)
			{
				UIElement uIElement = null;
				for (int i = ui.Elements.Count - 1; i >= 0; i--)
				{
					if (ui.Elements[i].ContainsPoint(point)) uIElement = ui.Elements[i];
				}

				if (uIElement != null) return uIElement.GetElementAt(point);
				return self.ContainsPoint(point) ? self : null;
			}

			return orig?.Invoke(self, point);
		}

		private void ItemSlot_LeftClick(ItemSlot.orig_LeftClick_1 orig, Item[] inv, int context, int slot)
		{
			if (inv[slot].modItem is BaseBag bag && bag.UI != null) BagUI.UI.HandleBag(bag);

			orig(inv, context, slot);
		}

		private void ItemSlot_Handle(ItemSlot.orig_Handle_1 orig, Item[] inv, int context, int slot)
		{
			Terraria.UI.ItemSlot.OverrideHover(inv, context, slot);
			if (Main.mouseLeftRelease && Main.mouseLeft)
			{
				Terraria.UI.ItemSlot.LeftClick(inv, context, slot);
				Recipe.FindRecipes();
			}
			else Terraria.UI.ItemSlot.RightClick(inv, context, slot);

			Terraria.UI.ItemSlot.MouseHover(inv, context, slot);
		}
	}
}