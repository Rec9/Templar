using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace debut_du_jeu
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState oldKeyboard;
        KeyboardState keyboard;
        GameScreen activeScreen;

        menu menu;
        menudujeu menudujeu;
        option option;
        menudepause pause;
        gamemain main;
        GameOverScreen gameover;
        EDM edm;

        bool ecran;
        int timer_musique;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            graphics.IsFullScreen = false;
            
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 800;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }
        protected override void LoadContent()
        {
            ressource.loadcontent(Content);
            spriteBatch = new SpriteBatch(GraphicsDevice);

            gameover = new GameOverScreen(this, spriteBatch, ressource.ecriture, ressource.gameover);
            Components.Add(gameover);
            gameover.hide();

            edm = new EDM(this, spriteBatch);
            Components.Add(edm);
            edm.hide();

            pause = new menudepause(this, spriteBatch, ressource.ecriture, ressource.pixel);
            Components.Add(pause);
            pause.hide();

            menu = new menu(this, spriteBatch, Content.Load<SpriteFont>("SpriteFont"), ressource.templar);
            Components.Add(menu);
            menu.hide();

            menudujeu = new menudujeu(this, spriteBatch, Content.Load<SpriteFont>("spriteFont"), ressource.th);
            Components.Add(menudujeu);
            menudujeu.hide();

            option = new option(this, spriteBatch, Content.Load<SpriteFont>("spriteFont"), ressource.option);
            Components.Add(option);
            option.hide();

            main = new gamemain(this, spriteBatch, activeScreen);
            Components.Add(main);
            main.hide();

            activeScreen = menu;
            activeScreen.Show();

            MediaPlayer.Play(ressource.menu);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0;

            SoundEffect.MasterVolume = 0.05f;

            ecran = false;
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }

        private bool checkKey(Keys theKey)
        {
            return keyboard.IsKeyUp(theKey) &&
                    oldKeyboard.IsKeyDown(theKey);
        }


        protected override void Update(GameTime gameTime)
        {
            
            /* timer_musique++;

            while (MediaPlayer.Volume != 10)
                if (timer_musique == 30)
                {
                    MediaPlayer.Volume++;
                    timer_musique = 0;
                }*/
            
            keyboard = Keyboard.GetState();

            # region gameover
            if (activeScreen == gameover)
            {
                if (checkKey(Keys.Enter))
                {
                    if (gameover.SelectedIndex == 0)
                    {

                    }

                    if (gameover.SelectedIndex == 1)
                    {
                        ressource.selection.Play();
                        activeScreen.hide();
                        activeScreen = menu;
                        activeScreen.Show();
                    }

                    if (gameover.SelectedIndex == 2)
                    {
                        ressource.selection.Play();
                        this.Exit();
                    }


                }

            }
            #endregion

            #region screen_menu_principal

            if (activeScreen == menu)
            {
                if (checkKey(Keys.Enter))
                {
                    if (menu.SelectedIndex == 0)
                    {
                        ressource.selection.Play();
                        activeScreen.hide();
                        activeScreen = menudujeu;
                        activeScreen.Show();
                    }

                    else if (menu.SelectedIndex == 1)
                    {

                    }

                    else if (menu.SelectedIndex == 2)
                    {
                        ressource.selection.Play();
                        activeScreen.hide();
                        activeScreen = edm;
                        activeScreen.Show();
                    }

                    else if (menu.SelectedIndex == 3)
                    {
                        ressource.selection.Play();
                        activeScreen.hide();
                        activeScreen = option;
                        activeScreen.Show();
                    }

                    else if (menu.SelectedIndex == 4)
                    {
                        ressource.selection.Play();
                        this.Exit();
                    }
                }
            }

            #endregion

            #region menu_1_Joueur

            else if (activeScreen == menudujeu)
            {
                if (checkKey(Keys.Enter))
                {

                    if (menudujeu.SelectedIndex == 0)
                    {
                        main = new gamemain(this, spriteBatch, activeScreen);
                        Components.Add(main);
                        main.hide();

                        ressource.selection.Play();
                        activeScreen.hide();
                        activeScreen = main;
                        activeScreen.Show();
                    }

                    else
                        if (menudujeu.SelectedIndex == 1)
                        {

                        }

                        else
                            if (menudujeu.SelectedIndex == 2)
                            {
                                ressource.selection.Play();
                                activeScreen.hide();
                                activeScreen = menu;
                                activeScreen.Show();
                            }
                }
            }

            #endregion

            #region screen_action
            else if (activeScreen == main)
            {
                if (checkKey(Keys.Escape))
                {
                    activeScreen.hide();
                    activeScreen = pause;
                    activeScreen.Show();
                }
            }
            #endregion

            #region screen_pause
            else
                if (activeScreen == pause)
                {
                    if (checkKey(Keys.Enter))
                    {

                        if (pause.SelectedIndex == 0)
                        {

                        }

                        else
                            if (pause.SelectedIndex == 1)
                            {

                            }
                            else
                                if (pause.SelectedIndex == 2)
                                {

                                }
                                else
                                    if (pause.SelectedIndex == 3)
                                    {
                                        ressource.selection.Play();
                                        activeScreen.hide();
                                        activeScreen = menu;
                                        activeScreen.Show();
                                    }
                    }


                    if (checkKey(Keys.Escape))
                    {
                        activeScreen.hide();
                        activeScreen = main;
                        activeScreen.Show();
                    }
                }
            #endregion

            #region screen_option
                else
                    if (activeScreen == option)
                    {
                        if (checkKey(Keys.Enter))
                        {
                            if (option.SelectedIndex == 0)
                            {
                                ressource.selection.Play();

                                if (ecran == false)
                                {
                                    graphics.ToggleFullScreen();
                                    graphics.PreferredBackBufferHeight = 1920;
                                    graphics.PreferredBackBufferWidth = 1080;
                                    
                                    ecran = true;
                                }
                            }

                            if (option.SelectedIndex == 1)
                            {
                                ressource.selection.Play();
                               
                                if (ecran == true)
                                {
                                    graphics.ToggleFullScreen();
                                    graphics.PreferredBackBufferHeight = 1920;
                                    graphics.PreferredBackBufferWidth = 1080;
                                    ecran = false;
                                }
                            }

                            if (option.SelectedIndex == 2)
                            {
                                ressource.selection.Play();
                                MediaPlayer.IsMuted = false;
                            }

                            if (option.SelectedIndex == 3)
                            {
                                ressource.selection.Play();
                                MediaPlayer.IsMuted = true;
                            }

                            if (option.SelectedIndex == 4)
                            {
                                ressource.selection.Play();
                                activeScreen.hide();
                                activeScreen = menu;
                                activeScreen.Show();
                            }
                        }
                    }
                #endregion

            #region screen_EDM

            if (activeScreen == edm)
            {
                if (checkKey(Keys.Escape))
                {
                    activeScreen.hide();
                    activeScreen = pause;
                    activeScreen.Show();
                }
            }
            #endregion

            base.Update(gameTime);

            oldKeyboard = keyboard;

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            base.Draw(gameTime);
            spriteBatch.End();
        }
    }
}
