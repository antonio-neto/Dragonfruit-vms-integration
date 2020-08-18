using System.Collections.Generic;

namespace DragonfruitPlugin.Client.Contracts.Model
{
    public class ChannelHierarchy
    {
        public ChannelHierarchy()
        {
            ChannelGroups = new List<ChannelGroup>();
        }

        public List<ChannelGroup> ChannelGroups { get; private set; }
    }
}
