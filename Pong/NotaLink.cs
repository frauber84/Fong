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
    class NotaLink
    {
        public Vector2 Pos;
        public int Tipo;
        public Rectangle Rect;
        public bool TrocarPosicao = false;

        public NotaLink(Vector2 position, int tipo)
        {
            Pos = position;
            Tipo = tipo;
            Rect = new Rectangle((int)Pos.X+20, (int)Pos.Y + 7, 55, 50);
            TrocarPosicao = false;
        }

        public void SetXY(Vector2 pos)
        {
            Pos = pos;
            Rect = new Rectangle((int)Pos.X + 20, (int)Pos.Y + 7, 55, 50);
        }



    }
}
