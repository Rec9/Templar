﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Templar
{
    /*ceci est la fusion de la classe actionscreen(useless maintenant) et gamemain */
    class gamemain : GameScreen
    {

        //field ecran 
        Rectangle fenetre;
        switch_map map;

        public Rectangle Fenetre
        {
            get { return fenetre; }
            set { fenetre = value; }
        }

        HUD HUD;

        #region field du jeu
        GamePlayer localPlayer;

        List<wall> Walls;
        List<Personnage> personnage;
        List<NPC> list_zombi;
        List<sort> liste_sort;
        List<potion> liste_objet_map;

        public List<potion> List_Objet_Map
        {
            get { return liste_objet_map; }
            set { liste_objet_map = value; }
        }

        public List<sort> List_Sort
        {
            get { return liste_sort; }
            set { liste_sort = value; }
        }

        public List<wall> List_wall
        {
            get { return Walls; }
            set { Walls = value; }
        }

        public List<NPC> List_Zombie
        {
            get { return list_zombi; }
            set { list_zombi = value; }
        }


        KeyboardState keyboard;
        MouseState mouse;

        Vector2 position_joueur, position_npc;

        Random x;

        bool ClickDown, pressdown;

        int pop_time, score, count_dead_zombi, timer_level_up;
        #endregion

        public gamemain(Game game, SpriteBatch spriteBatch, GameScreen activescreen)
            : base(game, spriteBatch)
        {

            #region init du jeu

            x = new Random();

            liste_sort = new List<sort>();
            list_zombi = new List<NPC>();
            Walls = new List<wall>();
            personnage = new List<Personnage>();
            liste_objet_map = new List<potion>();

            position_joueur = new Vector2(100, 100);

            localPlayer = new GamePlayer(34, 61, 4, 5, 2, 8, position_joueur, 100, ressource.sprite_player, this);
            localPlayer.Niveau = 1;

            pop_time = 0;
            score = 0;
            count_dead_zombi = 0;

            keyboard = new KeyboardState();

            #endregion init du jeu

            map = new switch_map(localPlayer, this);

            fenetre = new Rectangle(0, 0, game.Window.ClientBounds.Width, game.Window.ClientBounds.Height); //taille de la fenetre

            HUD = new HUD(localPlayer, this);

            # region media_player;
            MediaPlayer.Play(ressource.main_theme);
            MediaPlayer.Volume = 0.56f;
            MediaPlayer.IsRepeating = true;
           // SoundEffect.MasterVolume = 0.6f;
            # endregion
        }

        public override void Update(GameTime gameTime)
        {
            map.update();

            int pop_item = x.Next(0, 5);

            #region JEU

            keyboard = Keyboard.GetState();
            mouse = Mouse.GetState();

            #region ZOMBIE

            int a = x.Next(0, 1200);
            int b = x.Next(0, 800);
            position_npc = new Vector2(a, b);
            pop_time++;

            if (pop_time == 120)
            {
                list_zombi.Add(new NPC(24, 32, 4, 2, 1, 15, position_npc, ressource.zombie, localPlayer, this));
                pop_time = 0;
            }

            foreach (NPC zombie in list_zombi)
                zombie.update(mouse, keyboard, Walls, personnage);

            foreach (NPC zombie in list_zombi)
            {
                if (localPlayer.Hitbox_image.Intersects(zombie.Hitbox_image))
                    localPlayer.pv_player--;
            }

            for (int i = 0; i < list_zombi.Count; i++)
            {
                if (list_zombi[i].PV <= 0)
                {
                    if (pop_item == 0)
                    {
                        liste_objet_map.Add(new potion(ressource.potion_vie, localPlayer, this, list_zombi[i], "VIE"));
                    }

                    if (pop_item == 1)
                    {
                        liste_objet_map.Add(new potion(ressource.potion_mana, localPlayer, this, list_zombi[i], "MANA"));
                    }

                    list_zombi.RemoveAt(i);
                    score += 5;

                    localPlayer.XP += 20 / localPlayer.Niveau;
                }
            }


            #endregion ZOMBIE

            #region PLAYER

            localPlayer.update(mouse, keyboard, Walls, personnage); //fait l'update du player

            //cheat code
            if (keyboard.IsKeyDown(Keys.M))
                localPlayer.mana_player = 100;
            if (keyboard.IsKeyDown(Keys.V))
                localPlayer.pv_player = 100;

            //leveling
            if (localPlayer.XP >= 100)
            {
                localPlayer.Niveau++;
                timer_level_up = 0;
                localPlayer.XP -= 100;
                localPlayer.mana_player = localPlayer.pv_player = 100;
            }

            #endregion

            #region WALL
            //fait l'update des murs

            foreach (wall wall in Walls)
                wall.Update(mouse, keyboard);

            //dessine des nouveau mur

            if (mouse.LeftButton == ButtonState.Pressed && !ClickDown)
            {
                Walls.Add(new wall(mouse.X, mouse.Y, ressource.pixel, 32, Color.Black));
                ClickDown = true;
            }
            #endregion WALL

            //evite de trop appuyer
            if (mouse.LeftButton == ButtonState.Released)
                ClickDown = false;
           
            if (keyboard.IsKeyUp(Keys.Space))
                pressdown = false;

            #region ITEM

            if (keyboard.IsKeyDown(Keys.Space) && !pressdown && localPlayer.mana_player > 0)
            {
                if (localPlayer.Active == "feu")
                    ressource.feu.Play();

                liste_sort.Add(localPlayer.Active_Sort);
                pressdown = true;
            }

            foreach (sort sort in liste_sort)
                sort.update();



            for (int i = 0; i < liste_objet_map.Count; i++)
            {
                if (localPlayer.Hitbox_image.Intersects(liste_objet_map[i].Collide))
                {
                    switch (liste_objet_map[i]._Name)
                    {
                        case "VIE":
                            if (localPlayer.pv_player < 100)
                            {
                                localPlayer.pv_player += 25;
                                liste_objet_map.RemoveAt(i);
                            }
                            break;
                        case "MANA":
                            if (localPlayer.mana_player < 100)
                            {
                                localPlayer.mana_player += 25;
                                liste_objet_map.RemoveAt(i);
                            }
                            break;
                    }
                }
            }

            for (int i = 0; i < liste_sort.Count; i++)
            {
                for (int j = 0; j < Walls.Count; j++)
                {
                    if (liste_sort[i].hitbox_object.Intersects(Walls[j].Hitbox))
                    {
                        Walls.RemoveAt(j);
                        liste_sort.RemoveAt(i);
                        break;
                    }
                }
            }

            for (int i = 0; i < liste_sort.Count; i++)
            {
                for (int j = 0; j < list_zombi.Count; j++)
                {
                    if (liste_sort[i].hitbox_object.Intersects(list_zombi[j].Hitbox_image))
                    {
                        list_zombi[j].PV -= 100;
                        liste_sort.RemoveAt(i);
                        break;
                    }
                }
            }

            #endregion SORT

            // collision oO riena  foutre la ce truc xD
            if (position_joueur.X + ressource.sprite_player.Width == game.Window.ClientBounds.Width)
                position_joueur.X = 0;

            // LEVEL UP! 
            if (count_dead_zombi == 5)
            {
                localPlayer.Niveau++;
                count_dead_zombi = 0;
            }

            #endregion update jeu

            HUD.update();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Draw(map.Active_Map, fenetre, Color.White);

            timer_level_up++;

            #region draw du jeu

            foreach (item item in liste_objet_map)
                item.draw(spriteBatch);

            foreach (wall wall in Walls)
                wall.Draw(spriteBatch);

            foreach (NPC zombie in list_zombi)
                zombie.Draw(spriteBatch);

            foreach (sort boule in liste_sort)
                boule.draw(spriteBatch);

            localPlayer.Draw(spriteBatch);

            spriteBatch.DrawString(ressource.ecriture, Convert.ToString(score), new Vector2(500, 0), Color.Yellow);

            if (timer_level_up < 60 && localPlayer.Niveau != 1)
            {
                spriteBatch.DrawString(ressource.ecriture, "LEVEL UP !", new Vector2(localPlayer.position_player.X, localPlayer.position_player.Y - 10), Color.Yellow);

            }

            #endregion draw du jeu

            HUD.draw(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
