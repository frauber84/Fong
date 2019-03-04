using System;
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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Pong
{
    class Link    
    {
        public Vector2 Pos;
        public int Status;
        public bool Dead;
        public Rectangle Rect;
        public int Frame ;
        public int fCounter;
        public bool Walking;

        public Link(Vector2 position, int Status)
        {
            Pos = position;
            Dead = false;
            Status = 0;
            Rect = new Rectangle((int)Pos.X, (int)Pos.Y+4, 40, 36);
            Frame = 0;
            fCounter = 0;
            Walking = false;
        }

        public void SetXY(float X, float Y)
        {
            Pos += new Vector2(X, Y);
            Rect = new Rectangle((int)Pos.X, (int)Pos.Y+4, 40, 36);
        }

        public void UpdateFrame(int fSpeed)
        {
            if (Walking)
            {
                fCounter++;
                if (fCounter == fSpeed)
                {
                    if (Frame == 0) Frame = 1;
                    else if (Frame == 1) Frame = 0;
                    fCounter = 0;
                }
            }
            else
            {
                Frame = 0;
                fCounter = 0;
            }

        }

        public void CheckInput(int speed)
        {
            KeyboardState keyState = Keyboard.GetState();
                            
            if (keyState.IsKeyDown(Keys.Up))
            {
                Status = 0;
                SetXY(0, -speed);
                Walking = true;
            }
            else if (keyState.IsKeyDown(Keys.Down))
            {
                Status = 2;
                SetXY(0, speed);
                Walking = true;
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                Status = 4;
                SetXY(speed, 0);
                Walking = true;
            }
            else if (keyState.IsKeyDown(Keys.Left))
            {
                Status = 6;
                SetXY(-speed, 0);                
                Walking = true;
            }

            if (Pos.X < 0) Pos.X = 0;
            if (Pos.X > 754) Pos.X = 754;
            if (Pos.Y < 100) Pos.Y = 100;
            if (Pos.Y > 550) Pos.Y = 550;

            if (  ! (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.Down))) Walking = false;

        }

    }
}
