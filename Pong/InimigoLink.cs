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
    class InimigoLink
    {
        public Vector2 Pos;
        public int Tipo;
        public Rectangle Rect;
        public int Frame;
        public int fCounter;
        public bool Remover = false;
        public Vector2 Speed;

        public InimigoLink(Vector2 position, int tipo, Vector2 speed)
        {
            Pos = position;
            Tipo = tipo;
            if (Tipo == 4) Rect = new Rectangle((int)Pos.X, (int)Pos.Y + 4, 22, 42);
            else Rect = new Rectangle((int)Pos.X, (int)Pos.Y + 4, 40, 36);
            Frame = 0;
            fCounter = 0;
            Remover = false;
            Speed = speed;
        }

        public void IncXY(float X, float Y)
        {
            Pos += new Vector2(X, Y);
            if (Tipo == 4) Rect = new Rectangle((int)Pos.X, (int)Pos.Y + 4, 22, 42);
            else Rect = new Rectangle((int)Pos.X, (int)Pos.Y + 4, 40, 36);
        }

        public void UpdateFrame(int fSpeed)
        {
                fCounter++;
                if (fCounter == fSpeed)
                {
                    if (Frame == 0) Frame = 1;
                    else if (Frame == 1) Frame = 0;
                    fCounter = 0;
                }

        }
    }
}
