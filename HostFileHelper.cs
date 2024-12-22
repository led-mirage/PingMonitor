// © 2024 led-mirage. All rights reserved.

public static class HostFileHelper
{
    public static List<HostGroup> ParseHostFile(string filePath)
    {
        var hostGroups = new List<HostGroup>();
        HostGroup? currentGroup = null;

        try
        {
            foreach (var line in File.ReadLines(filePath))
            {
                var trimmedLine = line.Trim();

                if (string.IsNullOrWhiteSpace(trimmedLine))
                    continue;

                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    var groupName = trimmedLine.Trim('[', ']');
                    currentGroup = new HostGroup(groupName);
                    hostGroups.Add(currentGroup);
                }
                else
                {
                    if (currentGroup == null)
                    {
                        currentGroup = new HostGroup("Default Group");
                        hostGroups.Add(currentGroup);
                    }
                    currentGroup?.AddHost(trimmedLine);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
        }

        return hostGroups;
    }
}