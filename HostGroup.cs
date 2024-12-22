// © 2024 led-mirage. All rights reserved.

public class HostGroup
{
    public string GroupName { get; set; }
    public List<string> HostNames { get; set; }

    public HostGroup(string groupName)
    {
        GroupName = groupName;
        HostNames = new List<string>();
    }

    public void AddHost(string hostName)
    {
        HostNames.Add(hostName);
    }
}
