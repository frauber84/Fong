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
    class Palavra
    {
        public Vector2 Pos;
        public string Texto;
        public string TextoAtual;
        public bool remover = false;
        public int CurrentIndex = 0;
        public bool Especial;
        public bool Remover = false;

        public Palavra(Vector2 pos, String text, bool especial)
        {
            Texto = text;
            TextoAtual = text;
            Pos = pos;
            CurrentIndex = 0;
            Especial = especial;
            Remover = false;
        }

        public void TrocaPalavra(string palavra, Vector2 newpos)
        {
            CurrentIndex = 0;
            Texto = palavra;
            TextoAtual = palavra;
            Pos = newpos;
            
        }

        public void AtualizaPos(float speed)
        {
            Pos.Y += speed;
        }

    }
}
