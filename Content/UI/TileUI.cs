using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using GSMP;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace GSMP.Content.UI
{
    class PracticeButton : UIElement
    {
        Color color = new Color(50, 255, 153);
        Texture2D texture = null;

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, new Vector2(Main.screenWidth + 20, Main.screenHeight - 20) / 2f, color);
        }

    }

    class PracticeBar : UIState
    {
        public PracticeButton PracticeButton;
        private UserInterface _PracticeBar;
        public override void OnInitialize()
        {
            PracticeButton = new PracticeButton();

            Append(PracticeButton);
        }
        public override void Update(GameTime gameTime)
        {
            _PracticeBar?.Update(gameTime);
        }
        //public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        //{
        //    int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        //    if (mouseTextIndex != -1)
        //    {
        //        layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
        //            "YourMod: A Description",
        //            delegate
        //            {
        //                _PracticeBar.Draw(Main.spriteBatch, new GameTime());
        //                return true;
        //            },
        //            InterfaceScaleType.UI)
        //        );
        //    }
        //}
    }

}
