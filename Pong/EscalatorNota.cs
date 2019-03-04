using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Midi;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Runtime.InteropServices;

namespace Pong
{
    
    class EscalatorNota
    {
        public Vector2 Pos;
        public String Nota;
        public Rectangle Rect;
        public bool Remover = false;
        public bool LastStanding = false;

        public EscalatorNota(Vector2 Posicao, string nota)
        {
            Pos.X = Posicao.X;
            Pos.Y = Posicao.Y;
            Nota = nota;
            Rect = new Rectangle((int)Pos.X, (int)Pos.Y, 100, 100);
        }

        public bool ForaDaTela()
        {
            if (Pos.X < 860) return false; 
            else return true;
        }

        public void IncX(float quantidade)
        {
            Pos.X += quantidade;
            Rect = new Rectangle((int)Pos.X, (int)Pos.Y, 100, 100);
        }
    }
}
