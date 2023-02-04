using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.UI;


namespace Slipstream.Utils
{
    //Credit to Bubbet for most of the code here
    public static class ExtraHealthbarSegment
    {
        private static List<Type> barDataTypes = new List<Type>();

		public static Color defaultBaseColor = new Color(1f, 1f, 1f, 1f);
		public static void AddType<T>() where T : BarData, new()
        {
            barDataTypes.Add(typeof(T));
        }

		[SystemInitializer]
		private static void Init()
        {
            On.RoR2.UI.HealthBar.Awake += HealthBar_Awake;
            On.RoR2.UI.HealthBar.CheckInventory += HealthBar_CheckInventory;
            On.RoR2.UI.HealthBar.UpdateBarInfos += HealthBar_UpdateBarInfos;

            IL.RoR2.UI.HealthBar.ApplyBars += HealthBar_ApplyBars;
        }

        private static void HealthBar_UpdateBarInfos(On.RoR2.UI.HealthBar.orig_UpdateBarInfos orig, HealthBar self)
        {
			//Calls the tracker Update info every time healthbars are updated.

			orig(self);
			var tracker = self.GetComponent<SlipHealthbarInfoTracker>();
			tracker.UpdateInfo();
		}

        private static void HealthBar_CheckInventory(On.RoR2.UI.HealthBar.orig_CheckInventory orig, HealthBar self)
        {
			//Checks if the tracker is attatched to a body and has an inventory.

			orig(self);
			var tracker = self.GetComponent<SlipHealthbarInfoTracker>();
			if (!tracker) return;
			var source = self.source;
			if (!source) return;
			var body = source.body;
			if (!body) return;
			var inv = body.inventory;
			if (!inv) return;
			tracker.CheckInventory(body);
		}

        private static void HealthBar_Awake(On.RoR2.UI.HealthBar.orig_Awake orig, HealthBar self)
		{
			orig(self);
			self.gameObject.AddComponent<SlipHealthbarInfoTracker>().Init(self);
		}

		private static void HealthBar_ApplyBars(ILContext il)
        {
			var c = new ILCursor(il);

			var cls = -1;
			FieldReference fld = null;

			//Moves IL cursor right after instruction 10, AllocateElements call
			c.GotoNext(
				x => x.MatchLdloca(out cls),
				x => x.MatchLdcI4(0),
				x => x.MatchStfld(out fld)
			);

			c.GotoNext(MoveType.After,
				x => x.MatchCallOrCallvirt<HealthBar.BarInfoCollection>(nameof(HealthBar.BarInfoCollection.GetActiveCount))
			);

			//Injects IL to update the amount of healthbars enabled
			c.Emit(OpCodes.Ldarg_0);
			c.EmitDelegate<Func<int, HealthBar, int>>((i, bar) =>
			{
				var tracker = bar.GetComponent<SlipHealthbarInfoTracker>();
				i += tracker.barInfos.Count(x => x.info.enabled);
				return i;
			});

			//Moves IL cursor to instruction 83 call
			c.Index = il.Instrs.Count - 2;

			//Injects coded to apply the healthbar
			c.Emit(OpCodes.Ldloca, cls);
			c.Emit(OpCodes.Ldarg_0);
			c.Emit(OpCodes.Ldloca, cls);
			c.Emit(OpCodes.Ldfld, fld);
			c.EmitDelegate<Func<HealthBar, int, int>>((bar, i) =>
			{
				var tracker = bar.GetComponent<SlipHealthbarInfoTracker>();
				tracker.ApplyBar(ref i);
				return i;
				//return tracker.ApplyBar();
			});
			c.Emit(OpCodes.Stfld, fld); ;
		}

        public abstract class BarData
		{
			//Holds bar data such as visuals, the tracker it corresponds to, and the actual bar

			public SlipHealthbarInfoTracker tracker;
			public HealthBar bar;
			public HealthBar.BarInfo info;
			public HealthBarStyle.BarStyle? cachedStyle;
			private Image _imageReference;
			public virtual Image ImageReference
			{
				get => _imageReference;
				set
				{
					if (_imageReference && _imageReference != value)
					{
						_imageReference.material = bar.barAllocator.elementPrefab.GetComponent<Image>().material;
					}
					_imageReference = value;
				}
			}

			public abstract HealthBarStyle.BarStyle GetStyle();

			public virtual void UpdateInfo(ref HealthBar.BarInfo info, HealthComponent healthSource)
			{
				if (cachedStyle == null) cachedStyle = GetStyle();
				var style = cachedStyle.Value;

				info.enabled &= style.enabled;
				info.color = style.baseColor;
				info.imageType = style.imageType;
				info.sprite = style.sprite;
				info.sizeDelta = style.sizeDelta;
			}

			public virtual void CheckInventory(ref HealthBar.BarInfo info, CharacterBody body) { }

			//Sets bar information such as the beginning and end of the bar and the sprite/material
			public virtual void ApplyBar(ref HealthBar.BarInfo info, Image image, HealthComponent source, ref int i)
			{
				image.type = info.imageType;
				image.sprite = info.sprite;
				image.color = info.color;


				var rectTransform = (RectTransform)image.transform;
				rectTransform.anchorMin = new Vector2(info.normalizedXMin, 0f);
				rectTransform.anchorMax = new Vector2(info.normalizedXMax, 1f);
				rectTransform.anchoredPosition = Vector2.zero;
				rectTransform.sizeDelta = new Vector2(info.sizeDelta * 0.5f + 1f, info.sizeDelta + 1f);

				i++;
			}

		}

		public class SlipHealthbarInfoTracker : MonoBehaviour
		{
			public List<BarData> barInfos;
			public HealthBar healthBar;

			public void CheckInventory(CharacterBody body)
			{
				foreach (var barInfo in barInfos)
				{
					barInfo.CheckInventory(ref barInfo.info, body);
				}
			}
			public void UpdateInfo()
			{
				if (!healthBar || !healthBar.source) return;
				var healthBarValues = healthBar.source.GetHealthBarValues();
				foreach (var barInfo in barInfos)
				{
					if (barInfo.tracker == null)
						barInfo.tracker = this;
					if (barInfo.bar == null) // I cant do this in the init because it loses its reference somehow -Bubbet
						barInfo.bar = healthBar;
					barInfo.UpdateInfo(ref barInfo.info, healthBar.source);
				}
			}
			public void ApplyBar(ref int i)
			{
				foreach (var barInfo in barInfos)
				{
					ref var info = ref barInfo.info;
					if (!info.enabled)
					{
						barInfo.ImageReference = null; // Release the reference.
						continue;
					}

					Image image = healthBar.barAllocator.elements[i];
					barInfo.ImageReference = image;
					barInfo.ApplyBar(ref barInfo.info, image, healthBar.source, ref i);
				}
			}

			//barInfo.ApplyBar(ref barInfo.info, image, healthBar.source, ref i);

			public void Init(HealthBar healthBar)
			{
				this.healthBar = healthBar;
				barInfos = barDataTypes.Select(dataType => (BarData)Activator.CreateInstance(dataType)).ToList();
			}
		}
	}
}
