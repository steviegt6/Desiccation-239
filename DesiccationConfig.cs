using System.IO;
using Terraria;

internal static class DesiccationConfig
{
    private static bool loaded = false;
    public static byte team = 0;
    private static readonly string configfolder = Path.Combine(Main.SavePath, "Mod Configs");
    private static readonly string configpath = Path.Combine(configfolder, "Desiccation");

    public static void SaveTeam(byte team)
    {
        if (!Directory.Exists(configfolder))
        {
            Directory.CreateDirectory(configfolder);
        }
        using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(configpath, FileMode.OpenOrCreate)))
        {
            binaryWriter.Write(team);
        }
    }

    public static byte LoadTeam()
    {
        loaded = true;
        if (!File.Exists(configpath))
        {
            SaveTeam(0);
            return team;
        }
        using (BinaryReader binaryReader = new BinaryReader(File.Open(configpath, FileMode.Open)))
        {
            team = binaryReader.ReadByte();
        }
        return team;
    }

    public static byte GetTeam()
    {
        if (loaded)
        {
            return team;
        }
        return LoadTeam();
    }
}