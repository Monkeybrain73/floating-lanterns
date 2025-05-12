namespace apelanterns.ModConfig
{
    public class ModConfig
    {
        public static ModConfig Loaded { get; set; } = new ModConfig();
        public bool ShowModNameInHud { get; set; } = true;
        public bool ShowModNameInGuis { get; set; } = false;
    }
}
