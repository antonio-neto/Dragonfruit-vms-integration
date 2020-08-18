using System;
using System.Threading.Tasks;

namespace DragonfruitPlugin.Client.Contracts.Abstract
{
    public interface IPluginClientPlatform
    {
        Task GetAuth(string userName, string password, string[] metadata);

        Task GetAuth();

        Task GetChannelGroupAndChannelData(string jsonPath);

        Task GetChannelStream(string channelID, DateTime startTime, DateTime endTime, string path);
    }
}
