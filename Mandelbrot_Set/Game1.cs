
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using FormClosingEventArgs = System.Windows.Forms.FormClosingEventArgs;
using Form = System.Windows.Forms.Form;

namespace Mandelbrot_Set
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private SpriteFont font;
        private int maxiterations = 512;
        private Effect effect;
        private Texture2D tex, crosshair;
        public decimal zoom = 1.0m;
        private decimal xcoo, ycoo;
        public static Random r = new Random();
        private KeyboardState newstate, oldstate;


        public static int Screenwidth = 800;
        public static int Screenheight = 600;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                GraphicsProfile = GraphicsProfile.HiDef,
                PreferredBackBufferWidth = Screenwidth,
                PreferredBackBufferHeight = Screenheight,
                IsFullScreen = false,
                SynchronizeWithVerticalRetrace = true

            };
            IsMouseVisible = true;
            IsFixedTimeStep = false;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Form f = Form.FromHandle(Window.Handle) as Form;
            f.Location = new System.Drawing.Point(0, 0);
            if (f != null)
                f.FormClosing += f_FormClosing;
            base.Initialize();
        }

        private void f_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Exit();
            Thread.Sleep(100);
            base.Exit();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            effect = Content.Load<Effect>("mandelbrot_effect");
            font = Content.Load<SpriteFont>("font");
            crosshair = Content.Load<Texture2D>("crosshair");
            tex = new Texture2D(GraphicsDevice, Screenwidth, Screenheight, false, SurfaceFormat.Color);
            effect.Parameters["aspectratio"].SetValue(tex.Width / (float)tex.Height);
            effect.Parameters["width"].SetValue(tex.Width);
            effect.Parameters["height"].SetValue(tex.Height);
            xcoo = -0.761574m;
            ycoo = -0.0847596m;
            oldstate = Keyboard.GetState();
        }

        protected override void UnloadContent()
        {
        }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            newstate = Keyboard.GetState();
            if (newstate.IsKeyDown(Keys.Add))
            {
                zoom *= 1.05m;
            }

            if (newstate.IsKeyDown(Keys.Subtract))
            {
                zoom /= 1.05m;
            }

            if (newstate.IsKeyDown(Keys.T) && oldstate.IsKeyUp(Keys.T))
            {
                maxiterations *= 2;
            }

            if (newstate.IsKeyDown(Keys.G) && oldstate.IsKeyUp(Keys.G) && maxiterations > 1)
            {
                maxiterations /= 2;
            }

            if (newstate.IsKeyDown(Keys.W))
                ycoo -= 0.02m / (decimal)zoom;
            if (newstate.IsKeyDown(Keys.S))
                ycoo += 0.02m / (decimal)zoom;
            if (newstate.IsKeyDown(Keys.A))
                xcoo -= 0.02m / (decimal)zoom;
            if (newstate.IsKeyDown(Keys.D))
                xcoo += 0.02m / (decimal)zoom;

            decimal shader_xcoo = xcoo, shader_ycoo = ycoo, shader_xcoomult = 1, shader_ycoomult = 1;
            int x, y;
            for (x = 0; ; x++)
            {
                if (Math.Abs(shader_xcoo) < 0.0001m)
                {
                    shader_xcoo *= 10.0m;
                    shader_xcoomult *= 0.1m;
                }
                else
                {
                    break;
                }
            }
            for (y = 0; ; x++)
            {
                if (Math.Abs(shader_ycoo) < 0.0001m)
                {
                    shader_ycoo *= 10.0m;
                    shader_ycoomult *= 0.1m;
                }
                else
                {
                    break;
                }
            }

            effect.Parameters["maxiterations"].SetValue(maxiterations);

            decimal xabs = Math.Abs(xcoo);
            decimal yabs = Math.Abs(ycoo);
            double xx = (double)xcoo;
            long xxInt = BitConverter.DoubleToInt64Bits(xx);
            double yy = (double)ycoo;
            long yyInt = BitConverter.DoubleToInt64Bits(yy);

            effect.Parameters["xcooInt1"].SetValue((int)xxInt);
            effect.Parameters["xcooInt2"].SetValue((int)(xxInt >> 32));
            effect.Parameters["ycooInt1"].SetValue((int)yyInt);
            effect.Parameters["ycooInt2"].SetValue((int)(yyInt >> 32));

            //effect.Parameters["xcoo"].SetValue((float)(((xabs) % 0.0001m) * 10000.0m));
            //effect.Parameters["ycoo"].SetValue((float)(((yabs) % 0.0001m) * 10000.0m));

            //effect.Parameters["xcooint"].SetValue((int)(xcoo * 10000.0m));
            //effect.Parameters["ycooint"].SetValue((int)(ycoo * 10000.0m));
            //if (xcoo < 0)
            //    effect.Parameters["isxneg"].SetValue(true);
            //else
            //    effect.Parameters["isxneg"].SetValue(false);
            //if (ycoo < 0)
            //    effect.Parameters["isyneg"].SetValue(true);
            //else
            //    effect.Parameters["isyneg"].SetValue(false);
            oldstate = newstate;
            base.Update(gameTime);
        }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            effect.Parameters["zoom"].SetValue((float)zoom);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, effect, Matrix.Identity);
            spriteBatch.Draw(tex, Vector2.Zero, Color.White);
            spriteBatch.End();
            spriteBatch.Begin();
            spriteBatch.Draw(crosshair, new Vector2(tex.Width / 2 - 3, tex.Height / 2 - 3), Color.White);
            spriteBatch.DrawString(font, zoom.ToString(), new Vector2(100), Color.Red);
            spriteBatch.DrawString(font, maxiterations.ToString(), new Vector2(100, 130), Color.Red);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}


