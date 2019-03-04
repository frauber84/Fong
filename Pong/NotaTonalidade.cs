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
    class NotaTonalidade
    {
        public Vector2 Pos;
        public String Tom;
        public String Enarmonia;
        public Rectangle Rect;
        public bool Remover = false;
        public bool LastStanding = false;
        public int PretoBranco;
        public bool Reordenar = false;

        public NotaTonalidade(Vector2 Posicao, string tonalidade, int pretobranco)
        {
            Pos.X = Posicao.X;
            Pos.Y = Posicao.Y;
            Tom = tonalidade;
            Enarmonia = "";
            Rect = new Rectangle((int)Pos.X, (int)Pos.Y, 40, 80);
            PretoBranco = pretobranco; // 0 == branco, 1 == preto
        }

        public NotaTonalidade(Vector2 Posicao, string tonalidade, string enarmonia, int pretobranco)
        {
            Pos.X = Posicao.X;
            Pos.Y = Posicao.Y;
            Tom = tonalidade;
            Enarmonia = enarmonia;
            Rect = new Rectangle((int)Pos.X, (int)Pos.Y, 40, 80);
            PretoBranco = pretobranco; // 0 == branco, 1 == preto
        }

        public void SetXY(float X, float Y)
        {
            Pos = new Vector2(X,Y);
            Rect = new Rectangle((int)Pos.X, (int)Pos.Y, 40, 80);
        }
    }    
}
