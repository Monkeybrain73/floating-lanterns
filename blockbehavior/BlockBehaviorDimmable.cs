
namespace apelanterns.ModSystem
{
    using System.Text;
    using Vintagestory.API.Client;
    using Vintagestory.API.Common;
    using Vintagestory.API.Config;

    public class BlockBehaviorDimmable: BlockBehavior
    {

        public BlockBehaviorDimmable(Block block) : base(block)
        {
        }

        public override bool OnBlockInteractStart(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling)
        {

            string curType = world.BlockAccessor.GetBlock(blockSel.Position).Variant["rotator"];


            if (byPlayer.WorldData.EntityControls.Sneak)
            {
                if (curType == "l1") curType = "l2";
                else if (curType == "l2") curType = "l3";
                else if (curType == "l3") curType = "l4";
                else if (curType == "l4") curType = "l1";

                StaticUtils.SetBlockState(world, blockSel.Position, "rotator", curType);

                return true;
            }

            return false;
        }


        public override WorldInteraction[] GetPlacedBlockInteractionHelp(IWorldAccessor world, BlockSelection selection, IPlayer forPlayer, ref EnumHandling handled)
        {
            return new WorldInteraction[]
            {
                new WorldInteraction()
                {
                    ActionLangCode = "blockhelp-behavior-rightclickpickup",
                    MouseButton = EnumMouseButton.Right,
                    RequireFreeHand = true
                },

                new WorldInteraction()
                {
                    ActionLangCode = "apelanterns:blockhelp-behavior-dimmable-sneak",
                    MouseButton = EnumMouseButton.Right,
                    HotKeyCode = "ctrl",
                    RequireFreeHand = true
                }

            };

        }

        public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);

            dsc.AppendLine("\n" + Lang.Get("apelanterns:rightclickpickup-help") + "\n" + Lang.Get("apelanterns:dimmable-help"));
        }

    }
}
