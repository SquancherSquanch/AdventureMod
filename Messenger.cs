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
    public class Messenger : AProfession<HumanEntity>
    {
        public static List<string> workPools;
        QuestManager questManager = new QuestManager();
        public Messenger(HumanEntity unit, int exp) : base(unit, exp)
        {
        }

        static Messenger()
        {
            // Note: this type is marked as 'beforefieldinit'.
            List<string> list = new List<string>();
            list.Add("default.faction");
            list.Add("vanilla.build.structure");
            list.Add("vanilla.upgrade.storage");
            list.Add("vanilla.fish");
            Fisherman.workPools = list;
        }

        public override Transform getModel(bool female)
        {
            int i = UnityEngine.Random.Range(0, 13);
            questManager.QuestType = i;
            questManager.LoadQuest();
            switch (i)
            {
                case 0:
                    return (!female) ? AManager<AssetManager>.getInstance().humanArcherModel_M : AManager<AssetManager>.getInstance().humanArcherModel_F;
                case 1:
                    return (!female) ? AManager<AssetManager>.getInstance().humanBlacksmithModel_M : AManager<AssetManager>.getInstance().humanBlacksmithModel_F;
                case 2:
                    return (!female) ? AManager<AssetManager>.getInstance().humanBuilderModel_M : AManager<AssetManager>.getInstance().humanBuilderModel_F;
                case 3:
                    return (!female) ? AManager<AssetManager>.getInstance().humanCarpenterModel_M : AManager<AssetManager>.getInstance().humanCarpenterModel_F;
                case 4:
                    return (!female) ? AManager<AssetManager>.getInstance().humanEngineerModel_M : AManager<AssetManager>.getInstance().humanEngineerModel_F;
                case 5:
                    return (!female) ? AManager<AssetManager>.getInstance().humanFarmerModel_M : AManager<AssetManager>.getInstance().humanFarmerModel_F;
                case 6:
                    return (!female) ? AManager<AssetManager>.getInstance().humanFishermanModel_M : AManager<AssetManager>.getInstance().humanFishermanModel_F;
                case 7:
                    return (!female) ? AManager<AssetManager>.getInstance().humanForagerModel_M : AManager<AssetManager>.getInstance().humanForagerModel_F;
                case 8:
                    return (!female) ? AManager<AssetManager>.getInstance().humanHerderModel_M : AManager<AssetManager>.getInstance().humanHerderModel_F;
                case 9:
                    return (!female) ? AManager<AssetManager>.getInstance().humanInfantryModel_M : AManager<AssetManager>.getInstance().humanInfantryModel_F;
                case 10:
                    return (!female) ? AManager<AssetManager>.getInstance().humanMinerModel_M : AManager<AssetManager>.getInstance().humanMinerModel_F;
                case 11:
                    return (!female) ? AManager<AssetManager>.getInstance().humanStoneMasonModel_M : AManager<AssetManager>.getInstance().humanStoneMasonModel_F;
                case 12:
                    return (!female) ? AManager<AssetManager>.getInstance().humanTailorModel_M : AManager<AssetManager>.getInstance().humanTailorModel_F;
                case 13:
                    return (!female) ? AManager<AssetManager>.getInstance().humanWoodChopperModel_M : AManager<AssetManager>.getInstance().humanWoodChopperModel_F;
            }
            return (!female) ? AManager<AssetManager>.getInstance().humanFishermanModel_M : AManager<AssetManager>.getInstance().humanFishermanModel_F;
        }

        public override Texture2D getAvatar(bool female)
        {
            //Later match switch case to enhance quest GUI.
            return (!female) ? AManager<AssetManager>.getInstance().imageFisherman_M : AManager<AssetManager>.getInstance().imageFisherman_F;
        }

        public override string getProfessionName()
        {
            return "Messenger";
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
                AWorkTask work = this.unit.faction.getWorkPool().getWork(this.unit, Fisherman.workPools.ToArray());
                if (work != null)
                {
                    work.unit = this.unit;
                    return work;
                }
            }
            return null;
        }
    }
}