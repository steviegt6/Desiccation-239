using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using ReLogic.OS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.Utilities;

namespace Desiccation
{
    public class Desiccation : Mod
    {
        //Mod Loading
        internal static Desiccation Instance;
        public Desiccation() { Instance = this; }

        //Texture Backups Loading
        public Texture2D vanillaLogoDay;
        public Texture2D vanillaLogoNight;
        public Texture2D vanillaSkyBackground;
        public Texture2D vanillaInv;
        public Texture2D vanillaInvHotbar;
        public Texture2D vanillaInvFav;
        public Texture2D vanillaInvNew;
        public Texture2D mmbg1; //Main Menu Front 173
        public Texture2D mmbg2; //Main Menu Middle 172
        public Texture2D mmbg3; //Main Menu Back 171
        public Texture2D[] vanillaCloud = new Texture2D[22];
        public Texture2D vanillaHeart;

        //Miner UI Loading
        internal UserInterface MinerUserInterface;

        //Discord Rich Presence
        public static uint? prevCount;
        public static bool pauseUpdate = false;

        //Main.OnTick Event
        private static bool unloadCalled;

        //Dates
        public static DateTime Date { get; }
        public static bool AustraliaDay { get { DateTime date = Date; int result; if (date.Month == 1) { date = Date; result = ((date.Day == 26) ? 1 : 0); } else { result = 0; } return (byte)result != 0; } }
        public static bool IndependenceDay { get { DateTime date = Date; int result; if (date.Month == 7) { date = Date; result = ((date.Day == 4) ? 1 : 0); } else { result = 0; } return (byte)result != 0; } }

        //Title Messages
        WeightedRandom<string> titleText = new WeightedRandom<string>();
        public static string name;
        public static string title;
        public static bool titleChosen;

        public override void Load()
        {
            //Background Texture Loading 
            mmbg1 = Main.backgroundTexture[173]; //Main Menu Front 173
            mmbg2 = Main.backgroundTexture[172]; //Main Menu Middle 172
            mmbg3 = Main.backgroundTexture[171]; //Main Menu Back 171 
            for (int i = 0; i < vanillaCloud.Length; i++) { vanillaCloud[i] = Main.cloudTexture[i]; }

            //Main.OnTick Event
            Main.OnTick += MainMenuOnTick;
            unloadCalled = false;


            //Texture Loading
            vanillaInv = Main.inventoryBackTexture;
            vanillaInvHotbar = Main.inventoryBack9Texture;
            vanillaInvFav = Main.inventoryBack10Texture;
            vanillaInvNew = Main.inventoryBack15Texture;
            vanillaSkyBackground = Main.backgroundTexture[0];
            vanillaLogoDay = Main.logoTexture;
            vanillaLogoNight = Main.logo2Texture;
            vanillaHeart = Main.heartTexture;

            //Texture Replacig
            Main.inventoryBackTexture = GetTexture("UI/InventoryTabs/Inventory_Back");
            Main.inventoryBack9Texture = GetTexture("UI/InventoryTabs/Inventory_Back9");
            Main.inventoryBack10Texture = GetTexture("UI/InventoryTabs/Inventory_Back10");
            Main.inventoryBack15Texture = GetTexture("UI/InventoryTabs/Inventory_Back15");
            Main.backgroundTexture[0] = GetTexture("UI/Sky");
            Main.logoTexture = Main.logo2Texture = GetTexture("UI/DesiccationLogo"); //Thanks Jaserd for help with the sprite 
            Main.heartTexture = GetTexture("UI/Heart");

            //Miner UI
            if (!Main.dedServ) { MinerUserInterface = new UserInterface(); }

            //Main.OnPostDraw Event
            Main.OnPostDraw += MainMenuDraw;

            //Latest tModLoader Exception
            if (ModLoader.version < new Version(0, 0, 11, 5)) { throw new Exception("This mod only works in the latest tModLoader. Please update tModLoader to v0.11.5"); }
        }

        public override void Unload()
        {
            //Texture Backup Replacing
            Main.logoTexture = vanillaLogoDay;
            Main.logo2Texture = vanillaLogoNight;
            Main.backgroundTexture[0] = vanillaSkyBackground;
            Main.inventoryBackTexture = vanillaInv;
            Main.inventoryBack9Texture = vanillaInvHotbar;
            Main.inventoryBack10Texture = vanillaInvFav;
            Main.inventoryBack15Texture = vanillaInvNew;
            for (int i = 0; i < vanillaCloud.Length; i++) { Main.cloudTexture[i] = vanillaCloud[i]; }
            Main.backgroundTexture[173] = mmbg1; //Main Menu Front 173 = Main Menu Front 173 Backup
            Main.backgroundTexture[172] = mmbg2; //Main Menu Middle 172 = Main Menu Middle 172 Backup
            Main.backgroundTexture[171] = mmbg3; //Main Menu Back 171 = Main Menu Back 171 Backup
            Main.heartTexture = vanillaHeart;

            //Main.OnTick Event
            Main.OnTick -= MainMenuOnTick;
            unloadCalled = true;

            //Main.OnPostDraw Event
            Main.OnPostDraw -= MainMenuDraw;
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            //Miner UI
            int inventory = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (inventory != -1)
            {
                layers.Insert(inventory, new LegacyGameInterfaceLayer(
                    "Miner: Miner Light UI",
                    delegate
                    {
                        // If the current UIState of the UserInterface is null, nothing will draw. We don't need to track a separate .visible value.
                        MinerUserInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }

            //Revive Countdown
            int deathText = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Death Text"));
            if (deathText != -1)
            {
                layers.Insert(deathText, new LegacyGameInterfaceLayer("Desiccation: Respawn Timer",
                    delegate
                    {
                        if (Main.player[Main.myPlayer].dead)
                        {
                            Player player = Main.player[Main.myPlayer];
                            float num = player.respawnTimer / 60f;
                            string value = string.Format("{0:f1}", num);
                            DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, Main.fontDeathText, value, new Vector2((Main.screenWidth / 2) - Main.fontDeathText.MeasureString(value).X / 2f, (Main.screenHeight / 2 - 70)), Main.player[Main.myPlayer].GetDeathAlpha(Color.Transparent), 0f, default, 1f, SpriteEffects.None, 0f);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            //Miner UI
            MinerUserInterface?.Update(gameTime);
        }

        public override void PostSetupContent()
        {
            //Census Compatibility
            Mod censusMod = ModLoader.GetMod("Census");
            if (censusMod != null)
            {
                censusMod.Call("TownNPCCondition", NPCType("Miner"), "Have either a silver or tungsten pickaxe in your inventory, 1 gold in your inventory and for the merchant to be housed");
            }
        }

        public void MainMenuOnTick()
        {
            //Menu Time
            Mod Overhaul = ModLoader.GetMod("OverhaulMod");
            if (Overhaul == null && Main.gameMenu)
            {
                Main.time = 40000;
                Main.dayTime = true;
                Main.sunModY = 5;
                BackgroundReplacing();
                return;
            }

            //Logo Replacing Unload
            if (Main.menuMode == 10006 && !unloadCalled)
            {
                Unload();
                return;
            }
        }

        public void BackgroundReplacing()
        {
            //Background Texture Loading
            List<int> loadbgNumbers = new List<int>() { 173, 171, 172, 20, 21, 22 };
            foreach (int loadbgNumber in loadbgNumbers) { Main.instance.LoadBackground(loadbgNumber); }



            //Texture Replacing
            for (int i = 0; i < vanillaCloud.Length; i++) { Main.cloudTexture[i] = GetTexture("UI/Blank"); }
            Main.backgroundTexture[173] = Main.backgroundTexture[20]; //Main Menu Front 173 = Desert Front 20
            Main.backgroundTexture[172] = Main.backgroundTexture[21]; //Main Menu Middle 172 = Desert Middle 21
            Main.backgroundTexture[171] = Main.backgroundTexture[22]; //Main Menu Back 171 = Desert Back 22 
        }

        public void MainMenuDraw(GameTime gameTime)
        {
            //Main Menu Text
            if (Main.gameMenu)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
                byte b3 = (byte)((255 + Main.tileColor.R * 2) / 3);
                Color color = new Color(b3, b3, b3, 255);
                for (int num121 = 0; num121 < 5; num121++)
                {
                    Color colorBlack = Color.Black;
                    if (num121 == 4)
                    {
                        colorBlack = color;
                        colorBlack.R = (byte)((255 + colorBlack.R) / 2);
                        colorBlack.G = (byte)((255 + colorBlack.R) / 2);
                        colorBlack.B = (byte)((255 + colorBlack.R) / 2);
                    }
                    colorBlack.A = (byte)(colorBlack.A * 0.3f);
                    int num120 = 0;
                    int num119 = 0;
                    if (num121 == 0)
                    {
                        num120 = -2;
                    }
                    if (num121 == 1)
                    {
                        num120 = 2;
                    }
                    if (num121 == 2)
                    {
                        num119 = -2;
                    }
                    if (num121 == 3)
                    {
                        num119 = 2;
                    }
                    string supportMessage = "To Support Development: ";
                    string patreonShortURL = "\npatreon.com/Desiccation";
                    string drawVersion = "Desiccation v" + Version + Environment.NewLine + "Thanks for using Desiccation!" + Environment.NewLine + supportMessage;
                    Vector2 origin3 = Main.fontMouseText.MeasureString(drawVersion);
                    origin3.X *= 0.5f;
                    origin3.Y *= 0.5f;
                    Vector2 origin4 = Main.fontMouseText.MeasureString(drawVersion);
                    origin4.X *= 0.5f;
                    origin4.Y *= 0.5f;

                    //Version & Thank draw
                    DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, Main.fontMouseText, drawVersion, new Vector2(origin3.X + num120 + 10f, Main.screenHeight - origin3.Y + num119 - 612f), colorBlack, 0f, origin3, 1f, SpriteEffects.None, 0f);
                    if (num121 == 4)
                    {
                        colorBlack = Main.DiscoColor;
                    }
                    origin3 = Main.fontMouseText.MeasureString(supportMessage);
                    Vector2 urlSize = Main.fontMouseText.MeasureString(patreonShortURL);

                    //Support & Patreon draw
                    DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, Main.fontMouseText, patreonShortURL, new Vector2(origin4.X + num120 - 107f, Main.screenHeight - origin3.Y + num119 - 612f), colorBlack, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    if (num121 == 4 && Main.mouseLeft && new Rectangle((int)origin4.X + num120 - 107, Main.screenHeight - (int)origin3.Y + num119 - 612, (int)urlSize.X, (int)urlSize.Y).Contains(new Point(Main.mouseX, Main.mouseY)))
                    {
                        Main.PlaySound(10, -1, -1, 1, 1f, 0f);
                        Process.Start("https://www.patreon.com/Desiccation");
                        Main.mouseLeft = false;
                    }
                }
                //Layer FixS
                Main.DrawCursor(Main.DrawThickCursor());
                Main.spriteBatch.End();
                return;
            }
        }

        public override void UpdateMusic(ref int music, ref MusicPriority priority)
        {
            //Loading Title Messages
            if (titleChosen == false)
            {
                if (AustraliaDay) { name = "Australia Day!: "; }
                if (IndependenceDay) { name = "Independence Day!: "; }
                else { name = "Terraria with Desiccation: "; }
                titleText.Add("Using Discord");
                titleText.Add("Wow Jopojelly, Very Smart");
                titleText.Add("Zap!");
                titleText.Add("Milo Milk");
                titleText.Add("Upside Down");
                titleText.Add("Minecraft");
                titleText.Add("All Dried Up");
                titleText.Add("Eerie Messages from 2001");
                titleText.Add("Driest Mod!");
                titleText.Add("Moon SLime");
                titleText.Add("Kindness");
                titleText.Add("Party Like its 1999");
                titleText.Add("You Asked For This");
                titleText.Add("Comb your hair");
                titleText.Add("Jump");
                titleText.Add("It's a Bug!");
                titleText.Add("Very Old");
                titleText.Add("Sand in a Sandgun");
                titleText.Add("Im Big, im Bad");
                titleText.Add("Really Really Bad");
                titleText.Add("Americano");
                titleText.Add("Human");
                titleText.Add("I Remeber You");
                titleText.Add("Pairs Great With Recipe Browser");
                titleText.Add("Terraria 9");
                titleText.Add("100 Mil");
                title = name + titleText;
                Platform.Current.SetWindowUnicodeTitle(Main.instance.Window, title);
                titleChosen = true;
            }
        }

        public override void PreSaveAndQuit()
        {
            WorldGen.setBG(0, 6);
        }

        //KEEP THE FOLLOWING COMMENTED OUT CODE

        /* MM Music IL
         //Il Editing
        IL.Terraria.Main.UpdateAudio += HookUpdateAudio;
        void HookUpdateAudio(ILContext il)
        {
            var c = new ILCursor(il).Goto(0);
            if (c.TryGotoNext(i =>
             i.MatchLdarg(out int empty),
             i => i.MatchLdfld(out FieldReference reference),
             i => i.MatchStsfld(out FieldReference reference2),
             i => i.MatchLdcR4(out float anotherUseless),
             i => i.MatchStloc(out int empty2)))
            {
                c.Index += 5;
                c.EmitDelegate<Action>(() =>
                {
                    if (Main.gameMenu)
                    {
                        if (Main.netMode != 2)
                        {
                            Main.curMusic = GetSoundSlot(SoundType.Music, "Sounds/Music/Megalovania");
                        }
                        else
                        {
                            Main.curMusic = 0;
                        }
                    }
                    TaskCompletionSource<bool> musicFaded = new TaskCompletionSource<bool>();
                    Task<bool> t1 = musicFaded.Task;
                    Task.Factory.StartNew(() =>
                    {
                        if (unloadCalled)
                        {
                            Main.musicFade[Main.curMusic] = 0f;
                            musicFaded.SetResult(true);
                        }
                    });
                });
            }
        }
        */

        /*Discord RP
          Mod.Unload
         DiscordRP.RPControl.Disable();

          Mod.Load
        DiscordRP.RPControl.Enable();
        DiscordRP.RPControl.presence.details = string.Format("Main Menu");
        DiscordRP.RPControl.presence.largeImageKey = string.Format("icon");
        DiscordRP.RPControl.presence.largeImageText = string.Format("Early Alpha");
        DateTime date = DateTime.Now;
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        long timenow = Convert.ToInt64((date.ToUniversalTime() - epoch).TotalSeconds);
        DiscordRP.RPControl.presence.startTimestamp = timenow;
        DiscordRP.RPControl.Update();

        public static void UpdaterLoad()
        {
            //Rich Presence
            Main.OnTick += RPOnTick;
        }

        public static void UpdaterUnload()
        {
            //Rich Presence
            Main.OnTick -= RPOnTick;
        }

        public override void PreSaveAndQuit()
        {
            //Rich Presence
            DiscordRP.RPControl.presence.details = string.Format("Main Menu");
            DiscordRP.RPControl.presence.state = null;
            DiscordRP.RPControl.presence.largeImageKey = string.Format("icon");
            DiscordRP.RPControl.presence.largeImageText = string.Format("Early Alpha"); //TODO: version
            DiscordRP.RPControl.presence.smallImageKey = null;
            DiscordRP.RPControl.presence.smallImageText = null;
            DiscordRP.RPControl.Update();
            UpdaterUnload();
        }

        public static void RPOnTick()
        {
            //Rich Presence
            if (!Main.dedServ && !Main.gameMenu)
            {
                Player RPlayer = Main.player[Main.myPlayer];
                if ((prevCount == null || prevCount + 180 <= Main.GameUpdateCount) || (Main.gamePaused && !pauseUpdate))
                {
                    if (Main.gamePaused)
                    {
                        pauseUpdate = true;
                    }
                    prevCount = Main.GameUpdateCount;
                    DiscordRP.RPUtility.player = RPlayer;
                    DiscordRP.RPUtility.Update();
                }
                else if (!Main.gamePaused)
                {
                    pauseUpdate = false;
                }
                else return;
            }
            else return;
        }*/
    }
}