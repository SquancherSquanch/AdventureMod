using System;
using System.Collections.Generic;
using Timber_and_Stone.Tasks;
using Timber_and_Stone.Utility;
using UnityEngine;
using Timber_and_Stone;
using Timber_and_Stone.API;
using Timber_and_Stone.API.Event;
using Timber_and_Stone.Event;
using Timber_and_Stone.API.Event.Task;
using Timber_and_Stone.Profession.Human;
using EventHandler = Timber_and_Stone.API.Event.EventHandler;
using System.Linq;

namespace Plugin.Squancher.AdventureMod
{
    public class Bandit : AProfession<HumanEntity>
    {
        public static List<string> workPools;
        QuestManager questManager = new QuestManager();
        public static int image;

        public Bandit(HumanEntity unit, int exp) : base(unit, exp)
        {
        }

        static Bandit()
        {
            // Note: this type is marked as 'beforefieldinit'.
            List<string> list = new List<string>();
            list.Add("default.faction");
            Fisherman.workPools = list;
        }

        public override Transform getModel(bool female)
        {
            int i = UnityEngine.Random.Range(0, 100);
            image = i < 50 ? 0: 1;
           if (image == 0)
                return (!female) ? AManager<AssetManager>.getInstance().humanArcherModel_M : AManager<AssetManager>.getInstance().humanArcherModel_F;
           else
                return (!female) ? AManager<AssetManager>.getInstance().humanInfantryModel_M : AManager<AssetManager>.getInstance().humanInfantryModel_F;
        }

        public override Texture2D getAvatar(bool female)
        {
            //Later match switch case to enhance quest GUI.
            if (image == 0)
            {
                return (!female) ? AManager<AssetManager>.getInstance().imageArcher_M : AManager<AssetManager>.getInstance().imageArcher_F;
            }
            if (image == 1)
            {
                return (!female) ? AManager<AssetManager>.getInstance().imageInfantry_M : AManager<AssetManager>.getInstance().imageInfantry_F;
            }
            return (!female) ? AManager<AssetManager>.getInstance().imageInfantry_M : AManager<AssetManager>.getInstance().imageInfantry_F;
        }

        public override string getProfessionName()
        {
            return "Bandit";
        }

        public override ItemList GetDefaultItems()
        {
            return null;
        }

        public override List<string> getWorkPools()
        {
            return new List<string>();
        }

        public override ATask getProfessionTask()
        {
            if (this.unit.faction != null)
			{
				List<string> list = new List<string>(Fisherman.workPools);
				if (this.unit.preferences["preference.patrol"])
				{
					list.Add("vanilla.military.patrol");
				}
				if (this.unit.preferences["preference.guard"])
				{
					list.Add("vanilla.military.guard");
				}
				if (this.unit.preferences["preference.operatesiege"])
				{
					list.Add("vanilla.military.ballista");
				}
				AWorkTask work = this.unit.faction.getWorkPool().getWork(this.unit, list.ToArray());
				if (work != null)
				{
					work.unit = this.unit;
					return work;
				}
			}
			return new TaskInfantry(this.unit);
        }
    }
}