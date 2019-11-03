using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace Desiccation
{
    public class DesiccationPlayer : ModPlayer
    {
        //Electric Class Loading
        public float electricDamage = 0f;
        private Dictionary<int, int> itemToMusicReference;

        //Team Auto Join
        private bool teamLoaded;
        private int team;

        public override void ResetEffects()
        {
            //Electric Class
            electricDamage = 0f;
        }

        public override void Initialize()
        {
            //Vanity Music Reflection
            FieldInfo itemToMusicField = typeof(SoundLoader).GetField("itemToMusic", BindingFlags.Static | BindingFlags.NonPublic);
            itemToMusicReference = (Dictionary<int, int>)itemToMusicField.GetValue(null);
        }

        public override void UpdateVanityAccessories()
        {
            //Vanity Music
            for (int n = 13; n < 18 + player.extraAccessorySlots; n++)
            {
                Item item = player.armor[n];
                //Vanlla Music Boxes
                if (item.type >= 562 && item.type <= 574)
                {
                    Main.musicBox2 = item.type - 562;
                }
                if (item.type >= 1596 && item.type <= 1609)
                {
                    Main.musicBox2 = item.type - 1596 + 13;
                }
                if (item.type == 1610)
                {
                    Main.musicBox2 = 27;
                }
                if (item.type == 1963)
                {
                    Main.musicBox2 = 28;
                }
                if (item.type == 1964)
                {
                    Main.musicBox2 = 29;
                }
                if (item.type == 1965)
                {
                    Main.musicBox2 = 30;
                }
                if (item.type == 2742)
                {
                    Main.musicBox2 = 31;
                }
                if (item.type == 3044)
                {
                    Main.musicBox2 = 32;
                }
                if (item.type == 3235)
                {
                    Main.musicBox2 = 33;
                }
                if (item.type == 3236)
                {
                    Main.musicBox2 = 34;
                }
                if (item.type == 3237)
                {
                    Main.musicBox2 = 35;
                }
                if (item.type == 3370)
                {
                    Main.musicBox2 = 36;
                }
                if (item.type == 3371)
                {
                    Main.musicBox2 = 37;
                }
                if (item.type == 3796)
                {
                    Main.musicBox2 = 38;
                }
                if (item.type == 3869)
                {
                    Main.musicBox2 = 39;
                }
                //Modded Music Boxes
                if (itemToMusicReference.ContainsKey(item.type))
                {
                    Main.musicBox2 = itemToMusicReference[item.type];
                }
            }
        }

        public override void OnEnterWorld(Player player)
        {
            //Auto Team
            if (Main.LocalPlayer == player)
            {
                team = DesiccationConfig.GetTeam();
                Main.LocalPlayer.team = team;
            }
            //Enter Text
            Main.NewText("Thanks for playing with Desiccation", 145, 220, 196);

            //Background Swap Back
            BackgroundReReplacing();
        }

        public void BackgroundReReplacing()
        {
            for (int i = 0; i < Desiccation.Instance.vanillaCloud.Length; i++) { Main.cloudTexture[i] = Desiccation.Instance.vanillaCloud[i]; }
            Main.backgroundTexture[173] = Desiccation.Instance.mmbg1; //Main Menu Front 173 = Main Menu Front 173 Backup
            Main.backgroundTexture[172] = Desiccation.Instance.mmbg2; //Main Menu Middle 172 = Main Menu Middle 172 Backup
            Main.backgroundTexture[171] = Desiccation.Instance.mmbg3; //Main Menu Back 171 = Main Menu Back 171 Backup
        }

        public override void SendClientChanges(ModPlayer localPlayer)
        {
            //Auto Team
            if (team != Main.LocalPlayer.team)
            {
                team = Main.LocalPlayer.team;
                DesiccationConfig.SaveTeam((byte)team);
            }
            if (!teamLoaded)
            {
                if (Main.LocalPlayer.team > 0)
                {
                    NetMessage.SendData(45, -1, -1, null, Main.myPlayer, 0f, 0f, 0f, 0, 0, 0);
                }
                teamLoaded = true;
            }
        }
    }

}