namespace DragonfruitPlugin.Runner
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var plugin = new OcularisDragonfruitPlugin.Domain.OcularisPluginClientPlatform();
            await plugin.GetAuth(args[0], args[1], new string[] {
                    args[2]
                });

            await plugin.GetChannelGroupAndChannelData(args[3]);
        }
    }
}
