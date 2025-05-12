
using Vintagestory.API.Common;

[assembly: ModInfo("apelanterns")]

namespace apelanterns.ModSystem
{
    using HarmonyLib;
    using System.Text;
    using Vintagestory.API.Common;
    using Vintagestory.API.Client;
    using apelanterns.ModConfig;
    using Vintagestory.Client.NoObf;


    public class MoreLanternsSystem : ModSystem
    {
        // public IShaderProgram EntityGenericShaderProgram { get; private set; }

        private readonly string thisModID = "apelanterns112";

        // private ICoreServerAPI sapi;
        private ICoreClientAPI capi;


        private readonly Harmony harmony = new Harmony("com.xxxapexxx.apelanterns");

        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);

            if (ModConfig.Loaded.ShowModNameInHud)
            {
                var PSBlockGetPlacedBlockInfoOriginal = typeof(Block).GetMethod(nameof(Block.GetPlacedBlockInfo));
                var PSBlockGetPlacedBlockInfoPostfix = typeof(PS_BlockGetPlacedBlockInfo_Patch).GetMethod(nameof(PS_BlockGetPlacedBlockInfo_Patch.PSBlockGetPlacedBlockInfoPostfix));
                this.harmony.Patch(PSBlockGetPlacedBlockInfoOriginal, postfix: new HarmonyMethod(PSBlockGetPlacedBlockInfoPostfix));
            }

            if (ModConfig.Loaded.ShowModNameInGuis)
            {
                var PSCollectibleGetHeldItemInfoOriginal = typeof(CollectibleObject).GetMethod(nameof(CollectibleObject.GetHeldItemInfo));
                var PSCollectibleGetHeldItemInfoPostfix = typeof(PS_CollectibleGetHeldItemInfo_Patch).GetMethod(nameof(PS_CollectibleGetHeldItemInfo_Patch.PSCollectibleGetHeldItemInfoPostfix));
                this.harmony.Patch(PSCollectibleGetHeldItemInfoOriginal, postfix: new HarmonyMethod(PSCollectibleGetHeldItemInfoPostfix));
            }
            /*
            this.capi = api;
            this.capi.Event.ReloadShader += this.LoadCustomShaders;
            this.LoadCustomShaders();
            */

        }


        public override void StartPre(ICoreAPI api)
        {
            // Load/create common config file in ..\VintageStoryData\ModConfig\thisModID
            var cfgFileName = this.thisModID + ".json";
            try
            {
                ModConfig fromDisk;
                if ((fromDisk = api.LoadModConfig<ModConfig>(cfgFileName)) == null)
                { api.StoreModConfig(ModConfig.Loaded, cfgFileName); }
                else
                { ModConfig.Loaded = fromDisk; }
            }
            catch
            {
                api.StoreModConfig(ModConfig.Loaded, cfgFileName);
            }

            base.StartPre(api);

            // temp stupid bandaid
            // doesn't take effect until after a restart but better than nothing
            // if this doesn't minimize the bug reports then move to harmony patch
            if (api.Side != EnumAppSide.Client)
            {
                var taheight = ClientSettings.Inst.Int["maxTextureAtlasHeight"];
                if (taheight < 4096)
                {
                    try
                    {
                        ClientSettings.Inst.Int["maxTextureAtlasHeight"] = 4096;
                    }
                    catch { }
                }
            }
            // end temp stupid bandaid
        }

        /*
        public bool LoadCustomShaders()
        {

            this.EntityGenericShaderProgram = this.capi.Shader.NewShaderProgram();
            (this.EntityGenericShaderProgram as ShaderProgram).AssetDomain = "game";
            this.capi.Shader.RegisterFileShaderProgram("entityanimated", this.EntityGenericShaderProgram);
            this.EntityGenericShaderProgram.Compile();
            return true;
        }
        */

        public void RegisterClasses(ICoreAPI api)
        {
            // api.RegisterBlockBehaviorClass("RightClickPickupFloatingLantern", typeof(RightClickPickupFloatingLantern));
            api.RegisterBlockBehaviorClass("MoreLanterns.BlockName", typeof(BlockBehaviorName));
            api.RegisterBlockEntityClass("befloatinglantern", typeof(BEFloatingLantern));
            api.RegisterBlockClass("blockfloatinglantern", typeof(BlockFloatingLantern));
        }


        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            api.World.Logger.Event("started 'More Lanterns' mod");
            this.RegisterClasses(api);
        }


        public override void Dispose()
        {

            if (ModConfig.Loaded.ShowModNameInHud)
            {
                var PSBlockGetPlacedBlockInfoOriginal = typeof(Block).GetMethod(nameof(Block.GetPlacedBlockInfo));
                this.harmony.Unpatch(PSBlockGetPlacedBlockInfoOriginal, HarmonyPatchType.Postfix, "*");
            }

            if (ModConfig.Loaded.ShowModNameInGuis)
            {
                var PSCollectibleGetHeldItemInfoOriginal = typeof(CollectibleObject).GetMethod(nameof(CollectibleObject.GetHeldItemInfo));
                this.harmony.Unpatch(PSCollectibleGetHeldItemInfoOriginal, HarmonyPatchType.Postfix, "*");
            }

            base.Dispose();
        }


        // display mod name in the hud for blocks
        public class PS_BlockGetPlacedBlockInfo_Patch
        {
            [HarmonyPostfix]
            public static void PSBlockGetPlacedBlockInfoPostfix(ref string __result, IPlayer forPlayer)
            {
                var domain = forPlayer.Entity?.BlockSelection?.Block?.Code?.Domain;
                if (domain != null)
                {
                    if (domain == "apelanterns")
                    {
                        __result += "\n\n<font color=\"#D8EAA3\"><i>More Lanterns</i></font>\n\n";
                    }
                }
            }
        }


        public class PS_CollectibleGetHeldItemInfo_Patch
        {
            [HarmonyPostfix]
            public static void PSCollectibleGetHeldItemInfoPostfix(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world)
            {
                var domain = inSlot.Itemstack?.Collectible?.Code?.Domain;
                if (domain != null)
                {
                    if (domain == "apelanterns")
                    {
                        dsc.AppendLine("\n<font color=\"#D8EAA3\"><i>\" + Lang.GetMatching(\"game:tabname-apelanterns\") + \"</i></font>");
                    }
                }
            }
        }
    }
}






