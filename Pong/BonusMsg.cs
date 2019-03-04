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
    class BonusMsg
    {
        public Vector2 Pos;
        public Vector2 BonusMsgOffset;
        public float BonusTimer;
        public string Msg;
        public bool Remover;

        public BonusMsg(Vector2 pos, string msg, float timer, Vector2 offset)
        {
            Pos = pos;
            Msg = msg;
            BonusTimer = timer;
            BonusMsgOffset = offset;
            Remover = false;
        }
    }
}
