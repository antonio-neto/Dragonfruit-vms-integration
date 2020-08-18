using System.Collections.Generic;

namespace DragonfruitPlugin.Client.Contracts.Model
{
    public class ChannelGroup
    {
        public ChannelGroup()
        {
            Channels = new List<Channel>();
        }

        public string ChannelGroupName { get; set; }

        public string ObjectID { get; set; }

        public List<Channel> Channels { get; private set; }
    }
}
