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

// terminar PROGRESSAO DE NIVEL 
// JOGA TRAVA NA AUSENCIA DE MIDI IN EM ALGUNS MODOS, IMPEDIR ISSO
// IMPLEMENTAR MIDI OUT
// recordes (lento e comp problemas)
// To Do: GRAFICOS NO MODO BAIXO CONTINUO
// to do: acesso nao autorizado (som). Menu Minigames 
// To DO: BOOL PRIMEIRA DISTRACAO: AJEITAR SCHEDULING DOS EVENTOS DE AUDIO
// To Do: re-escrever instruções
// PERGUNTAS UNIVERSITÁRIO X BAMBU  
// TO DO: TESTAR NOTA REFERENCIA BOWSER em varias claves
// to do: melhorar verificacao de recordes (só no final)
// to Do: atualizar HOMEM-PALITO com versão silverlight
// atualizar link vs otitie com versão sliverlight
// distracoes: does botoes simultaneos nao funcionam com touchpad, notazero dá problema
// baixar indice de distrações e GBs
// tirar PRIMEIRA DISTRACAO = FALSE dos recordes para só aparecer uma única vez

namespace Pong
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto)]        
        public static extern bool ClipCursor(ref Microsoft.Xna.Framework.Rectangle rect);

        InputDevice MidiIn;

        int a = 0;
            
        int[] PitchTrans = new int[] { 0, -1, 1, -1, 2, 3, -1, 4, -1, 5, -1, 6}; // OctavePosition transposto para CurrentNote
        int[] PitchTrans2 = new int[] { 7, -1, 8, -1, 9, 10, -1, 11, -1, 12, -1, 13 }; // OctavePosition transposto para CurrentNote
        int[] PitchTrans3 = new int[] { 14, -1, 15, -1, 16, 17, -1, 18, -1, 19, -1, 20 }; // OctavePosition transposto para CurrentNote
        int[] PitchTrans4 = new int[] { 21, -1, 22, -1, 23, 24, -1, 25, -1, 26, -1, 27 }; // OctavePosition transposto para CurrentNote        
        int[] ModoJogo = new int[] { 9, 1, 10, 0, 11, 2, 3, 4, 5, 6, 7, 8 };

        int[][] CifraBaixo = // indices para array CifraSprite
        {
            new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 }, // la (==5)
            new int[] { 0, 2, 3, 5, 6, 7, 10, 11, 12, 16}, // b
            new int[] { 0, 17, 2, 3, 5, 7, 10, 11, 18, 15, 16}, // c
            new int[] { 0, 1, 2, 3, 4, 5, 7, 15, 10, 11, 13, 9 },  // d
            new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 13, 14 }, // e
            new int[] { 0, 17, 2, 3, 5, 7, 11, 16}, //f
            new int[] { 0, 17, 2, 3, 5, 7, 10, 11, 15 }, // G
        };

        int NotabaseChegada = 0;
        int NOTABASE = 2;
        float ENERGIA = 0;
        float rot = 0;
        int rotdir = 0;
        int PontosLink = 0;
        int PontosPalavra = 0; 
        int[] Efeito = new int[] { 0, 0, 0, 0, 0, 0, 0 };

        int TextureOffsetX = 0;        
        string[] Palavras = new string[]
        {
            "kodaly",
            "acciaccatura",
            "adagietto",
            "affrettando",
            "cantilena",
            "concertante",
            "fugato",
            "grave",
            "legatissimo",
            "largamente",
            "lontano",
            "marcato",
            "martellato",
            "ossia",
            "ostinato",
            "passepied",
            "pavana",
            "perdendosi",
            "pesante",
            "piacevole",
            "piangevole",
            "pizzicato",
            "precipitato",
            "recitativo",
            "risoluto",
            "ritenueto",
            "smorzando",
            "spicatto",
            "stringendo",
            "tutti",
            "continuo",
            "cantata",
            "canzona",
            "chacona",
            "coral",
            "preludio",
            "codetta",
            "ensemble",
            "diminuto",
            "aumentado",
            "intermezzo",
            "melodrama",
            "musette",
            "motivo",
            "neoclassicismo",
            "classicismo",
            "pathos",
            "jonico",
            "pastoral",
            "partita",
            "ricercare",
            "cantochao",
            "suite",
            "temperamento",
            "tenuto",
            "tremolo",
            "antifonia",
            "bagatela",
            "banjo",
            "batuta",
            "bequadro",
            "cavatina",
            "cembalo",
            "clavecin",
            "clavicembalo",
            "corneto",
            "crescendo",
            "divisi",
            "dinamica",
            "encore",
            "flauta",
            "trompa",
            "fusa",
            "grupetto",
            "hemiola",
            "hexacorde",
            "harpa",
            "hipojonico",
            "arsis",
            "tesis",
            "ictus",
            "isoritmo",
            "kapellmeister",
            "libretto",
            "lira",
            "luthier",
            "marimba",
            "musicologia",
            "nota",
            "ocarina",
            "operabufa",
            "organum",
            "overture",
            "notapedal",
            "polca",
            "polonaise",
            "quarteto",
            "quodlibet",
            "raga",
            "ragtime",
            "ressonancia",
            "rapsodia",
            "salsa",
            "coco",
            "quadrilha",
            "xote",
            "sforzato",
            "reinforzando",
            "sprechgesang",
            "terzetto",
            "toada",
            "cluster",
            "transposicao",
            "virelai",
            "vocalise",
            "xilofone", 
            "polifonia",
            "homofonia",
            "heterofonia",
            "minimalismo",
            "schoenberg",
            "webern",
            "serialismo",
            "triosonata",
            "concertino",
            "stockhausen",
            "pianissimo",
            "kyrie",
            "gloria",
            "agnusdei",
            "sanctus",
            "credo",
            "acorde",
            "gretchen",
            "dominante",
            "beethoven",
            "subdominante",
            "mediante",
            "tricorde",
            "dissonancia",
            "cadencia",
            "compasso",
            "andamento",
            "contratempo",
            "sincope",
            "wagner",
            "mendelssohn",
            "hanonfede",
            "ravel",
            "diatonicismo",
            "pantonal",
            "rachmaninov",
            "schubert",
            "machaut",
            "dufay",
            "johncage",
            "palestrina",
            "dvorak",
            "gershwin",
            "ginastera",
            "janacek",
            "kabalevsky",
            "kodaly",
            "swanwick",
            "suzuki",
            "langlang",
            "medtner",
            "milhaud",
            "liszt", 
            "czerny",
            "blitzkriegbop",
            "urlinie",
            "ursatz",
            "picardia",
            "modalismo",
            "johnadams",
            "philipglass",
            "viola",
            "contrabaixo",
            "trombone",
            "fermata",
            "vibrato",
            "tremolo",
            "andante",
            "allegro",
            "moderato",
            "larghetto",
            "largo",
            "vivace",
            "presto",
            "adagio",
            "prestissimo",
            "allabreve",
            "minueto",
            "sonata",
            "giga",
            "courante",           
            "radiohead",
            "allemande",
            "leggiero",
            "acellerando",
            "ritardando",
            "aria",
            "barroco",
            "fagote",
            "bolero",
            "cantabile",
            "celesta",
            "harpa",
            "rallentando",
            "baiao",
            "tdah",
            "imslp",
            "contrasujeito",
            "cantabile",
            "rubato",
            "enarmonia",
            "bach",
            "tenor",
            "contralto",
            "soprano",
            "habanera",
            "polca",
            "teclado",
            "cdefgabc",
            "escala",
            "bebop",
            "bemol",
            "clarone",
            "stretto",
            "clarinete",
            "contraponto",
            "decrescendo",
            "fuga",
            "piano",
            "glissando",
            "cravo",
            "harmonia",
            "atonal",
            "improviso",
            "clave",
            "lied",
            "leitmotif",
            "madrigal",
            "maestoso",
            "magnificat",
            "mazurka",
            "melodia",
            "moteto",
            "movimento",
            "obbligato",
            "ondasmartenot",
            "opus",
            "orquestra",
            "abertura",
            "passacaglia",
            "pianoforte",
            "ritornello",
            "romantismo",
            "sarabanda",
            "scherzo",
            "sonata",
            "sonatina",
            "espineta",
            "virginal",
            "sinfonia",
            "tarantella",
            "tuba",
            "trompete",
            "arpejo",
            "coda",
            "concerto",
            "guitarra",
            "tocaraul",            
            "mezzoforte",
            "marimba",
            "pentatonica",
            "tercina",
            "valsa",
            "monteverdi",
            "frescobaldi",
            "scarlatti",
            "mozart",
            "frescobaldi",
            "bartok",
            "glinka",
            "mussorgsky",
            "rachmaninov",
            "debussy",
            "haydn",
            "chopin",
            "schumann",
            "scriabin",
            "johncage",            
            "copland",
            "hindemith",
            "messiaen",
            "tandun",
            "guarnieri",
            "verdi",
            "puccini",
            "gounoud",
            "piumosso",
            "anacruse",
            "nepomucemo",
            "ockeghem",
            "pergolesi",
            "prokofiev",
            "poulenc",
            "purcell",
            "rameau",
            "satie",
            "rzewski",
            "jsbach",
            "cpebach",
            "pianola",
            "scarlatti",
            "sibelius",
            "stravinsky",
            "takemitsu",
            "gubaidulina",
        };

        string[] EfeitosColaterais = new string[]
        {   
            "Dores de cabeça",
            "Diarréia",
            "Verborragia",
            "Convulsões",
            "Enurese noturna",
            "Epidermodisplasia verruciforme",
            "Alterações no ciclo menstrual",
            "Perda de senso estético",
            "Hemorróidas",
            "Vazio existencial",
            "Ilusões de grandeza",
            "Depressão",
            "Distonia focal",
            "Alucinações",
            "Paranóia",
            "Egocentrismo",             
            "Gripe H1N1",
            "Mal de Parkinson",
            "Hipertricose auricular",
            "Doenças auto-imunes",
            "Covardia",
            "Pieguice exacerbada",
            "Perda de memoria",
            "Maurício Zamithismos",
            "Disfunção cognitiva",
            "Daltonismo",
            "Alterações hormonais",
            "Aumento de peso",
            "Narcolepsia",
            "Cleptomania",
            "Pianofobia",
            "Melomania",
        };

        Keys ReturnKey(char ch_)
        {
            string ch = ch_.ToString();
            ch.ToLower();

            switch (ch)
            {
                case "a": return Keys.A;
                case "b": return Keys.B;
                case "c": return Keys.C;
                case "d": return Keys.D;
                case "e": return Keys.E;
                case "f": return Keys.F;
                case "g": return Keys.G;
                case "h": return Keys.H;
                case "i": return Keys.I;
                case "j": return Keys.J;
                case "k": return Keys.K;
                case "l": return Keys.L;
                case "m": return Keys.M;
                case "n": return Keys.N;
                case "o": return Keys.O;
                case "p": return Keys.P;
                case "q": return Keys.Q;
                case "r": return Keys.R;
                case "s": return Keys.S;
                case "t": return Keys.T;
                case "u": return Keys.U;
                case "v": return Keys.V;
                case "w": return Keys.W;
                case "y": return Keys.Y;
                case "x": return Keys.X;
                case "z": return Keys.Z;
                case " ": return Keys.Space;
                default: return Keys.None;
            }
            
        }
        string[] TomMaior = new string[]
        {
            "C", "G", "D", "A", "E", "B", "F#", "C#", "F", "Bb", "Eb", "Ab", "Db", "Gb", "B",
            "C", "G", "D", "A", "E", "B", "F#", "C#", "F", "Bb", "Eb", "Ab", "Db", "Gb", "B",
        };

        string[] TomMenor = new string[]
        {
            "a", "e", "b", "f#", "c#", "g#", "d#", "a#", "d", "g", "c", "f", "bb", "eb", "ab",
            "a", "e", "b", "f#", "c#", "g#", "d#", "a#", "d", "g", "c", "f", "bb", "eb", "ab"            
        };


         // tessitura de instrumentos, combinacoes (Formacoes: quinteto de metais, sopros, cordas, etc, familias, organologia)
        string[] PerguntasFacil = new string[]
        {
            "Quais são as notas formadas em cada linha\n(de baixo para cima) em uma pauta com a clave \nde sol na segunda linha?",
            "Uma armadura de clave com 1 bemol geralmente\né um indicativo de qual tonalidade?",
            "As notas Lá, Dó, e Mi constituem uma tríade...\n",
            "Quais são as notas da escala de Ré maior?",

            "Qual a tonalidade relativa menor de Db maior?",
            "Qual destes intervalos não pode ser classificado\ncomo \"justo\"?",
            "Qual destas escalas contém um intervalo de \nsegunda aumentada entre um de seus graus?",
            "As notas Fá#, Do e La soando simultaneamente\nformam um acorde...",

            "Qual das combinações de notas abaixo não é uma\ntríade em primeira inversão?",
            
        }; 

        string[][] AlternativasFacil =
        {
            new string[] { "E, G, B, D, F", "F, A, C, E, G", "G, B, D, F, A", "D, F, A, C, E" },
            new string[] { "Sol Maior", "Ré menor", "Fá menor", "Bb maior" },
            new string[] { "Maior", "Aumentada", "Menor", "Diminuta" },
            new string[] { "D, E, F#, G, A, B, C, D", "D, E, F#, G#, A, B, C#, D", "D, E, F#, G, A, B#, C#, D", "D, E, F#, G, A, B, C#, D" },
            
            new string[] { "Bbm", "Fm", "Bb", "Dbm" },
            new string[] { "Oitava", "Terça", "Quarta", "Quinta", },
            new string[] { "Escala maior", "Escala menor natural", "Escala menor harmônica", "Escala de tons inteiros", },
            new string[] { "Maior", "Aumentado", "Menor", "Diminuto" },

            new string[] { "G - C - E", "G# - B - E", "C - E - A", "D - F - Bb" },

        
        };

        string[] PerguntasMedio = new string[]
        {
            "O termo \"alla breve\" geralmente corresponde a que\nfórmula de compasso?",
            "Quantas notas diferentes existem em uma\nescala de tons inteiros?",
            "Quais são as notas da escala de Db maior?",
            "Qual desses compositores não se enquadra no\nclassicismo musical?",

            "Qual destes compositores não é filho de\nJ.S. Bach?",
            "Qual dos termos abaixo não se relaciona com\na intensidade de um som?", 
            "Qual o nome do 4º grau em uma escala maior?",
            "Qual a inversão do intervalo de quarta aumentada?",

            "Como se chama a tríade formada por dois \nintervalos de terça maior a partir de uma\nnota fundamental?",
            "Qual a constituição intervalar da escala\nmenor natural?",
            "O termo mediante se refere a qual grau de uma\nescala diatônica?",
            "Qual das alternativas abaixo apresenta\numa gradação correta de andamentos (do mais\nlento para o mair rápido)?",

            "Em uma formação típica de quinteto de sopros\nqual destes instrumentos não está presente?",
            "Em uma formação típica de quinteto de sopros\nqual destes instrumentos não está presente?",
            "Em uma formação típica de quinteto de sopros\nqual destes instrumentos não está presente?",


        };

        string[][] AlternativasMedio =
        {
            new string[] { "2/2", "4/4", "4/8", "1/todos e todos/1" },
            new string[] { "5", "6", "7", "8", },
            new string[] { "Db Eb F G Ab Bb C Db",  "Db Eb Fb Gb Ab Bb Cb Db",  "Db Eb F Gb Ab Bb C Db",  "Db Eb F Gb Ab Bb Cb Db",},
            new string[] { "Mozart", "Beethoven", "Haydn", "Rameau"  },

            new string[] { "P.D.Q. Bach", "C.P.E Bach", "W.F. Bach", "J.C. Bach" },
            new string[] { "Dinâmica", "Frequência", "Pressão sonora", "Amplitude" },
            new string[] { "Supertônica", "Submediante", "Subdominante", "Dominante" }, 
            new string[] { "Quinta menor", "Quarta diminuta", "Quarta justa", "Quinta diminuta" }, 

            new string[] { "Aumentada", "Diminuta", "Composta", "Maior" },
            new string[] { "T-T-ST-T-T-T-ST", "T-ST-T-T-ST-T-T", "T-ST-T-T-T-ST-T", "ST-T-T-T-ST-T-T" },
            new string[] { "II", "VII", "III", "VI"  },
            new string[] { "Allegro - Allegretto", "Adagio - Largo ", "Presto - Andante", "Largo - Larghetto"  },

            new string[] { "Saxofone", "Trompa", "Fagote", "Oboé"  },
            new string[] { "Trompa", "Trompete", "Fagote", "Oboé"  },
            new string[] { "Trompa", "Fagote", "Trombone", "Clarinete"  },
        
        };


        string[] PerguntasDificil = new string[]
        {
            "Uma escala maior com o quarto grau aumentado\nresulta em qual modo litúrgico?",
            "Uma escala menor natural com o sexto grau\naumentado resulta em qual modo litúrgico?",            
            "A sarabanda é uma dança de origem ... ",
            "A estrutura seccional ABACA corresponde a \nqual molde formal?",

            "Ordene cronologicamente os compositores:\nI - John Dowland, \nII-Philippe de Vitry, \nIII - Leoninus, \nIV - C.P.E. Bach",
            "Qual é a resolução usual do acorde Eb7?",
            "Quantos tipos de acordes de sétima podem ser\nformados a partir de uma escala maior?",

        };

        string[][] AlternativasDificil =
        {
            new string[] { "Lídio", "Dórico", "Mixolídio", "Hipolídio" },
            new string[] { "Frígio", "Dórico", "Lócrio", "Lídio" },
            new string[] { "Italiana", "Germânica", "Espanhola", "Francesa" }, 
            new string[] { "Forma sonata", "Ternária", "Minueto", "Rondó"  }, 

            new string[] { "III, II, IV, I", "II, III, IV, I", "III, II, I, IV", "I, II, III, IV" },
            new string[] {  "Fm", "Ab", "Bb",  "Bbm" }, 
            new string[] {  "3", "5", "4",  "2" }, 
                        
        };

        string[] PerguntasFoda = new string[]
        {
            "Qual destas escalas não constitui um modo\nde transposição limitada?",
            "Qual destes compositores não compôs música\nespectral (reformular) ?",
            "Qual destes tricordes não pode ser reduzido\na forma prima (0, 1, 6) ? ",
            "Qual intervalo não esta presente no vetor\nintervalar do tricorde (0, 1, 6)?",

            "blah blah blah",

        };

        string[][] AlternativasFoda =
        { 
            new string[] { "Lídia dominante", "Tons inteiros", "Escala octatônica ST/T", "Escala octatônica T/ST" },
            new string[] { "A", "B", "C", "D" },
            new string[] { "A", "B", "C", "D" },
            new string[] { "A", "B", "C", "D" },

            new string[] { "A", "B", "C", "D" },
        };


        int PerguntaTimer = 0;
        int PontosBambu = 0;
        int PontosEscalator = 0;
        int EscalatorRecorde = 0;
        int BambuRecorde = 0;
        int PerguntaAtual = 0;
        TimeSpan TempoSobrando;
        
        
        string[] Notas = new string[] { "C", "C#", "Cb", "Db", "D", "D#", "E", "E#", "Eb", "F", "F#", "Fb", "G", "G#", "Gb", "A", "A#", "Ab", "B", "Bb", "B#" };

        int EscalaTipo = 0;
        int EscalaNota = 0;
        int Rotacao = 0;
        string EscalaAtual = "Nenhuma";
        string[][] EscalasMaiores =
        {
            new string[] { "C",  "D",  "E",  "F",  "G",  "A",  "B"  },
            new string[] { "Db", "Eb", "F",  "Gb", "Ab", "Bb", "C"  },
            new string[] { "D",  "E",  "F#", "G",  "A",  "B",  "C#" },
            new string[] { "Eb", "F",  "G",  "Ab", "Bb", "C",  "D"  },
            new string[] { "E",  "F#", "G#", "A",  "B",  "C#", "D#" },
            new string[] { "F",  "G",  "A",  "Bb", "C",  "D",  "E"  },
            new string[] { "F#", "G#", "A#", "B",  "C#", "D#", "E#" },
            new string[] { "G",  "A",  "B",  "C",  "D",  "E",  "F#" },
            new string[] { "Ab", "Bb", "C",  "Db", "Eb", "F",  "G"  },
            new string[] { "A",  "B",  "C#", "D",  "E",  "F#", "G#" },
            new string[] { "Bb", "C",  "D",  "Eb", "F",  "G",  "A"  },
            new string[] { "B",  "C#", "D#", "E",  "F#", "G#", "A#" },
        };

        string[][] EscalasMenoresNatural = //natural
        {
            new string[] { "C",  "D",  "Eb", "F",  "G",  "Ab", "Bb" },
            new string[] { "C#", "D#", "E",  "F#", "G#", "A",  "B"  },
            new string[] { "D",  "E",  "F",  "G",  "A",  "Bb", "C"  },
            new string[] { "Eb", "F",  "Gb", "Ab", "Bb", "Cb", "Db" },
            new string[] { "E",  "F#", "G",  "A",  "B",  "C",  "D"  },
            new string[] { "F",  "G",  "Ab", "Bb", "C",  "Db", "Eb" },
            new string[] { "F#", "G#", "A",  "B",  "C#", "D",  "E"  },
            new string[] { "G",  "A",  "Bb", "C",  "D",  "Eb", "F"  },
            new string[] { "G#", "A#", "B",  "C#", "D#", "E",  "F#" },
            new string[] { "A",  "B",  "C",  "D",  "E",  "F",  "G"  },
            new string[] { "Bb", "C",  "Db", "Eb", "F",  "Gb", "Ab" },
            new string[] { "B",  "C#", "D",  "E",  "F#", "G",  "A"  },
        };

        string[][] EscalasMenoresHarmonica = 
        {
            new string[] { "C",  "D",  "Eb", "F",  "G",  "Ab", "B" },
            new string[] { "C#", "D#", "E",  "F#", "G#", "A",  "B#" },
            new string[] { "D",  "E",  "F",  "G",  "A",  "Bb", "C#" },
            new string[] { "Eb", "F",  "Gb", "Ab", "Bb", "Cb", "D"  },
            new string[] { "E",  "F#", "G",  "A",  "B",  "C",  "D#" },
            new string[] { "F",  "G",  "Ab", "Bb", "C",  "Db", "E"  },
            new string[] { "F#", "G#", "A",  "B",  "C#", "D",  "E#" },
            new string[] { "G",  "A",  "Bb", "C",  "D",  "Eb", "F#" },
            new string[] { "G#", "A#", "B",  "C#", "D#", "E",  "F##"},
            new string[] { "A",  "B",  "C",  "D",  "E",  "F",  "G#" },
            new string[] { "Bb", "C",  "Db", "Eb", "F",  "Gb", "A"  },
            new string[] { "B",  "C#", "D",  "E",  "F#", "G",  "A#" },
        };

        string[][] EscalasMenoresMelodica = 
        {
            new string[] { "C",  "D",  "Eb", "F",  "G",  "A",   "B"  },
            new string[] { "C#", "D#", "E",  "F#", "G#", "A#",  "B#" },
            new string[] { "D",  "E",  "F",  "G",  "A",  "B",   "C#" },
            new string[] { "Eb", "F",  "Gb", "Ab", "Bb", "C",   "D"  },
            new string[] { "E",  "F#", "G",  "A",  "B",  "C#",  "D#" },
            new string[] { "F",  "G",  "Ab", "Bb", "C",  "D",   "E"  },
            new string[] { "F#", "G#", "A",  "B",  "C#", "D#",  "E#" },
            new string[] { "G",  "A",  "Bb", "C",  "D",  "E",   "F#" },
            new string[] { "G#", "A#", "B",  "C#", "D#", "E#",  "F##"},
            new string[] { "A",  "B",  "C",  "D",  "E",  "F#",  "G#" },
            new string[] { "Bb", "C",  "Db", "Eb", "F",  "G",   "A"  },
            new string[] { "B",  "C#", "D",  "E",  "F#", "G#",  "A#" },
        };


        string[][] Tonalidades =
        {
            new string[] { "C", "Db", "D", "Eb", "E", "F", "F#", "G", "Ab", "A", "Bb", "B" },
            new string[] { "c", "c#", "d", "eb", "e", "f", "f#", "g", "g#", "a", "bb", "b" },
        };

        string[][] AcordeGrausFacil =
        {
            new string[] {"I", "IV", "V", "V7" },
            new string[] {"i", "iv", "V", "V7" },
        };

        string[][] AcordeGrausMedio =
        {
            new string[] {"I", "IV", "V", "V7", "ii", "vi" },
            new string[] {"i", "iv", "V", "V7", "iiº", "VI"  },
        };

        string[][] AcordeGrausDificil =
        {
            new string[] {"I", "IV", "V", "V7", "ii", "vi", "iii", "viiº" },
            new string[] {"i", "iv", "V", "V7", "iiº", "VI", "III" },
        };

        string[][] AcordeGrausPatterns =
        {
            new string[] {"", "", "", "7", "m", "m", "m", "dim" },
            new string[] {"m", "m", "", "7", "dim", "", "" },
        };

        int[] AcordeGrausIndex = new int[] { 0, 3, 4, 4, 1, 5, 2, 6 };


        string[][] AcordeBaixo = 
        {
            // nota = lá (CurrentKey = 5)
            new string[] {
                "Am",   // 0
                "A", 
                "F/A", 
                "Am7", 
                "A7", 
                "F7M/A",  //5
                "F#dim/A", 
                "BØ/A",
                "B7/A",
                "F7/A",
                "Dm/A", // 10
                "Dm7/A",
                "D7/A",
                "Adim",
                "Adim7",   //14
            },            
            new string[]  
            { 
                "Bdim",   //0
                "G/B", // 2
                "BØ", // 3
                "G7/B", // 5
                "G#dim/B", // 6
                "C7M/B", // 7
                "Em/B", // 10
                "Em7/B", // 11
                "E7/B", // 12
                "Bdim7", // 16
            },
            new string[]  
            { 
                "C",   //0
                "Cm", //17
                "Am/C", // 2
                "C7M", // 3
                "Am7/C", //5
                "Dm7/C", //7
                "F/C", // 10
                "F7M/C", //11
                "F#Ø/C", // 18
                "D7/C", // 15
                "C7" // 16
            },
            new string[] 
            { 
                "Dm",   //0
                "D",
                "Bdim/D",
                "Dm7",
                "D7",
                "BØ/D",
                "Em7/D",
                "E7/D",
                "G/D",
                "G7/D",
                "Ddim", // (5b)
                "Ddim7", // 
            },
            new string[] 
            { 
                "Em",   //0
                "E",
                "C/E",
                "Em7",
                "E7",
                "C7M/E",
                "C#dim/E",
                "F7M/E",
                "C7/E",
                "Am/E",
                "Am7/E",                
                "Edim",
                "Edim7",
                "F#Ø/E", // faltou
                "A7/E", // faltou

            },
            new string[]
            { 
                "F",   //0
                "Fm", // 17
                "Dm/F", // 2
                "F7M",  // 3
                "Dm7/F", // 5
                "G7/F", // 7
                "BØ/F", // 11
                "F7", //16
            },
            new string[]  
            { 
                "G",   //0
                "Gm",  //17
                "Em/G", //2
                "G7",   //3
                "Em7/G", // 5
                "Am7/G", //7
                "C/G", // 10
                "C7M/G", // 11
                "A7/G", // 15
            },

        };


        bool PalavraIntroducao = false;
        bool AcabouForca = false;
        int GBTimer = 0;
        int GBTimer2 = 0;        
        int TiroTimer = 0;
        int PizzaTimer = 0;
        bool TrocaNotabase = false;
        int NotabaseTimer = 0;
        bool Distracoes = true;
        bool PrimeiraDistracao = true;
        bool MostrarTeclado = true;
        bool ModoSurpresa = false;
        bool MINIGAME = false;
        bool RespostaErrada = false;
        bool ABSTART = true;
        int DIFICULDADE = 0;
        int CurrentMIDI = 0;
        int Clave = 9;
        int ClaveOriginal = 9;
        int MIDICount = 0;
        int Inversoes = 0;
        int BotaoAtual;
        Sprite[] BG = new Sprite[11];
        ButtonState PrevLeftState;
        ButtonState PrevRightState;
        KeyboardState prevkeyState;
        Texture2D[] NotaZero;
        Texture2D Olivetti;
        Texture2D Selecao;
        Texture2D StatusBar;
        Texture2D TecladoInstrucao;
        Texture2D hand1;
        Texture2D hand2;        
        Texture2D FundoDrMario;
        Texture2D TomQuadrado;
        Texture2D[] HomemPalito;
        Texture2D[] Forca;
        Texture2D[] TonalidadesFig;
        Texture2D TV;
        Texture2D PacMan;
        Texture2D Universitario;
        Texture2D Silvio;
        Texture2D BotaoVazio;
        Texture2D BotaoCheio;
        Texture2D TMonk;
        Texture2D[] Pizza;
        Texture2D Escalator;
        Texture2D Mira;
        Texture2D[] Link;
        Texture2D[] InimigosLink;
        Vector2 MiraPos;
        Texture2D FundoMadeira;
        Texture2D Trofeu;
        int TomStatus = 0;
        bool TomSelecionado = false;
        int TomIndex = 0;
        int TomTipo = 0;
        int TomAtual = 0;
        int TomCount = 0;
        int TomRecorde = 0;
        Texture2D[] facepalm = new Texture2D[2];
        Texture2D BonusMoldura;
        int PizzaStatus = 0;
        int ForcaStatus = 0;
        SoundEffect Mozart;
        SoundEffect Grito;
        SoundEffect DrMario;
        SoundEffect Getheart;
        SoundEffect LinkHit;
        SoundEffect LinkWrong;
        SoundEffect Zelda;
        SoundEffect Beep;
        SoundEffect pacmanintro;
        SoundEffect TicTac;
        SoundEffect PringlesSnd;
        SoundEffect Chomp;
        SoundEffect TromboneFail;
        SoundEffect VirarPagina;
        SoundEffect Burro;
        SoundEffect RespostaCerta;
        SoundEffect Suspense;
        SoundEffect AberturaBambu;
        SoundEffect Tiro;
        SoundEffect DoomMusic;
        Texture2D Faca;
        SoundEffect TetrisLyrics;
        Texture2D Harpsichord;
        Texture2D Cravo;
        Texture2D MarioOver;        
        Texture2D PianoFrame;        
        Texture2D AcordeMoldura;
        Texture2D ClaveFa;
        Texture2D ClaveFaMetade;        
        Texture2D ClaveAcordes;
        Texture2D ClaveSolFa;
        Texture2D ClaveHarmonia;
        Texture2D ClaveInterrogacao;
        Texture2D Shrek;
        Texture2D ClaveSol;
        Texture2D ClaveDo;
        Texture2D ClaveSolMetade;
        Texture2D ClaveDoMetade;               
        Texture2D Clave2P;
        Texture2D Ninja;
        Texture2D Gretchen;
        Texture2D MenuInstrucoes;
        Texture2D MenuInstrucoes2;
        Texture2D TMNT;
        Texture2D Turtle;
        SpriteFont fontSmall;        
        SpriteFont font;
        SpriteFont fontBig;
        SpriteFont Menufont;
        SpriteFont fontEscalator;
        SpriteFont fontEscalatorSmall;
        SpriteFont Engraver;
        SpriteFont EmuFont;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;        
        Texture2D ballSprite;        
        Texture2D piano800;
        Texture2D piano800metade;        
        Texture2D Bowser;
        Texture2D GameBoy;
        Texture2D ClaveBaixoCifrado;
        Texture2D Ps3;
        bool PALITOMORREU = false;
        bool EscalatorMorreu = false;
        bool PALITOSENTADO = false;
        int BowserNotaAtual = 0;
        bool GAMEBOY = false;
        bool PianoShake = false;
        bool PianoShake2 = false;
        int PianoShakeTimer = 0;
        int PianoShakeTimer2 = 0;
        int GBTYPE = 0;
        bool BOTAOA = false;
        bool BOTAOB = false;
        bool BOTAOSTART = false;        
        bool ShowBowser = false;
        float PontosPerdidos = 0;
        bool BowserNotasBool = false;
        bool[] BowserNotasStatus = new bool[7];        
        Vector2 BowserPos = new Vector2(800, 180);
        Vector2 PalitoPos = new Vector2(400, 220);
        Vector2 FacaPos = new Vector2(570, 240);
        int FacaDir = 0;
        int FacaDirX = 0;
        int FacaTimer = 0;
        int FacaCount = 0;
        int FacaRecorde = 0;
        int FacaMovY = 0;
        int LinkRecorde = 0;
        int PalavraRecorde = 0;
        Vector2 PianoOffset = new Vector2(0, 0);
        float BowserRot = 0;
        int pianoDirecao = 0;
        int BowserEnergia = 25;
        int BowserDirecao = 0;
        int Temp = 0;
        int TimerEventos = 0;
        int TimerGameboy = 0;
        int[] BowserNotas = new int[7];
        
        Vector2 ballPosition = new Vector2(150,200);
        Vector2 ballSpeed; // = new Vector2(150, 150);
        Vector2 MMPos = new Vector2(150, 90);
        Vector2 FererPos = new Vector2(0, 490);
        float FererRot = 0;
        int PacSide = 0;
        int FererSide = 0;        
        int LeftButton = 0;        
        int RBStatus = 0;
        Vector2 RBPos = new Vector2(200, 198);
        Vector2 pianoPos = new Vector2(0, 35);
        Texture2D paddleSprite;        
        Texture2D Zombie;
        SoundEffect Scream;
        SoundEffect Burp;
        SoundEffect Morte;
        SoundEffect Coral;
        SoundEffect Conga;
        SoundEffect Mario;
        Vector2 paddlePosition;
        Vector2 paddlePositionRot;
        Vector2 paddlePositionTemp;

        Vector2 paddlePosition2;
        Vector2 paddlePositionRot2;
        Vector2 paddlePositionTemp2;



        Texture2D[] noteSprite;
        Texture2D[] CifraSprite;
        Texture2D[] Menu;
        int MenuKey = -1;        
        Texture2D Ferer;
        Texture2D TDAH;
        SoundEffect[] noteSounds;
        SoundEffect Tetris;
        SoundEffect TetrisOriginal;
        SoundEffect swishSound;
        SoundEffect crashSound;
        SoundEffect Beat;
        SoundEffect OneUp;
        Texture2D RedBull;
        Texture2D Picanha;
        SoundEffectInstance soundEngine;
        SoundEffectInstance soundEngineGrito;
        SoundEffectInstance soundEnginePalavra;        
        SoundEffectInstance soundEngineGretchen;
        SoundEffectInstance soundEnginePac;
        SoundEffectInstance soundEngineScream;
        SoundEffectInstance soundEngineCoral;
        SoundEffectInstance soundEngineMorte;
        SoundEffectInstance soundEngineMario;
        SoundEffectInstance soundEngineSobre;
        Texture2D MM;
        Texture2D Pringles;
        Texture2D DiamanteNegro;
        Texture2D Coca;
        Texture2D m_backGroundColorGreen;
        Texture2D m_backGroundColorWhite;
        Texture2D m_backGroundColorBlack;
        Texture2D m_backGroundColorRed;
        Texture2D m_backGroundColorLightBlue;
        string Ranking = "Ceguinho";
        string[] Recordes = new string[35];
        int CurrentKey = -1;
        int ModoAtual = -1;
        int PrevModoAtual = -1;
        int CurrentKeyMIDI = -1;
        int CurrentNote = 0;
        int Vidas = 3;
        int Vidas2 = 3;
        int Pontos = 0;
        int Pontos2 = 0;
        int alternador = 0;
        int InstrucaoPagina = 0;
        bool ShowMM = false;
        int MMPosInc = 0;
        bool ShowFerer = false;
        bool ShowRB = false;
        int MMType = 0;
        bool Pt250 = false;
        bool[] GB = new bool[7];
        bool Vida1000 = false;
        bool Vida2000 = false;
        bool Vida3000 = false;
        Vector2 MarioOverPos = new Vector2(445, 175);
        int MarioOverDir = 0;
        int OVERTYPE = 0; 
        int SUBMENU = 0;
        int FINAL = 0;
        int halt = 0;
        int MENU = 1;
        int CONTROLE = 0;
        Random Rand = new Random();
        RenderTarget2D renderTarget;
        public Dictionary<Pitch, bool> pitchesPressed = new Dictionary<Pitch, bool>();
        public Dictionary<Pitch, bool> pitchesPressed2 = new Dictionary<Pitch, bool>();
        public Dictionary<Pitch, bool> pitchesPressedController = new Dictionary<Pitch, bool>();
        
        Chord CurrentChord = new Chord("C");
        Stopwatch timer;
        TimeSpan TempoJogo;
        string Clave6Acorde;
        bool Pulo = false;
        bool Queda = false;
        SpriteEffects PalitoState = SpriteEffects.None;
        int VelocidadePulo = 0;
        bool SoltarBotao = false;

        List<EscalatorNota> NotasEscalator = new List<EscalatorNota>();
        List<NotaTonalidade> Tom = new List<NotaTonalidade>();
        List<BonusMsg> Bonus = new List<BonusMsg>();
        List<Link> Links = new List<Link>();
        List<InimigoLink> InimigosLinkLista = new List<InimigoLink>();
        List<NotaLink> NotasLink = new List<NotaLink>();
        List<Palavra> PalavrasLista = new List<Palavra>();
        Camera2D Camera = new Camera2D();


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.Title = "Fong!   (C) 2011  TDAH Games - Diversões frenéticas para multidões entediadas";
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = false;
        }


        int ContaTrofeu()
        {
            int TotalTrofeu = 0;

            for (int i = 0; i < 34; i++)
            {
                if ( TryToParse(Recordes[i]) > 3250) TotalTrofeu++;
            }

            
            return TotalTrofeu;
        }
        int RecordeTotal()
        {
            int Total = 0;for (int i = 0; i < 35; i++)
            {
                Total += TryToParse(Recordes[i]);
            }

            return Total;

        }
        void ResetaGB()
        {
            // GAMEBOY
            GB[0] = false;
            GB[1] = false;
            GB[2] = false;
            GB[3] = false;
            GB[4] = false;
            GB[5] = false;
            GB[6] = false;
        }

        private void PrintStatus()
        {
            Console.Clear();
            Console.WriteLine("Console DEBUG");
            Console.WriteLine();

            // Print the currently pressed notes.
            List<Pitch> pitches = new List<Pitch>(pitchesPressed.Keys);
            pitches.Sort();
            Console.Write("Notas (P1): ");
            for (int i = 0; i < pitches.Count; ++i)
            {
                Pitch pitch = pitches[i];
                if (i > 0)
                {
                    Console.Write(", ");
                }
                Console.Write("{0}", pitch.NotePreferringSharps());
                if (pitch.NotePreferringSharps() != pitch.NotePreferringFlats())
                {
                    Console.Write(" or {0}", pitch.NotePreferringFlats());
                }
            }
            Console.WriteLine();

            // Print the currently pressed notes.
            pitches = new List<Pitch>(pitchesPressed2.Keys);
            pitches.Sort();
            Console.Write("Notas (P2): ");
            for (int i = 0; i < pitches.Count; ++i)
            {
                Pitch pitch = pitches[i];
                if (i > 0)
                {
                    Console.Write(", ");
                }
                Console.Write("{0}", pitch.NotePreferringSharps());
                if (pitch.NotePreferringSharps() != pitch.NotePreferringFlats())
                {
                    Console.Write(" or {0}", pitch.NotePreferringFlats());
                }
            }
            Console.WriteLine();

            // Print the currently held down chord.
            pitches = new List<Pitch>(pitchesPressed.Keys);
            pitches.Sort();
            List<Chord> chords = Chord.FindMatchingChords(pitches);
            Console.Write("Acordes (P1): ");
            for (int i = 0; i < chords.Count; ++i)
            {
                Chord chord = chords[i];
                if (i > 0)
                {
                    Console.Write(", ");
                }
                Console.Write("{0}", chord);
            }
            Console.WriteLine();

            pitches = new List<Pitch>(pitchesPressed2.Keys);
            pitches.Sort();
            chords = Chord.FindMatchingChords(pitches);
            Console.Write("Acordes (P2): ");
            for (int i = 0; i < chords.Count; ++i)
            {
                Chord chord = chords[i];
                if (i > 0)
                {
                    Console.Write(", ");
                }
                Console.Write("{0}", chord);
            }
            Console.WriteLine();

            Process proc = Process.GetCurrentProcess();
            Console.WriteLine("BallSpeed = " + ballSpeed.ToString());
            Console.WriteLine("BallPos = " + ballPosition.ToString());            
            Console.WriteLine("CurrentKeyMidi: " + CurrentKeyMIDI.ToString());
            Console.WriteLine("Clave: " + Clave.ToString());
            Console.WriteLine("CONTROLE: " + CONTROLE.ToString());
            Console.WriteLine("Dificuldade: " + DIFICULDADE.ToString());
            Console.WriteLine("Memória: " +  proc.WorkingSet64.ToString());
            Console.WriteLine("PaddlePos.Y", paddlePosition.Y);
            Console.WriteLine("notabasetimer: " + NotabaseTimer);
            Console.WriteLine("GBTimer: " + GBTimer);
            Console.WriteLine("GBtimer2: " + GBTimer2);
        }

        void DesenhaPerguntas(string[][] alternativas)
        {
            if (Rotacao == 0 | Rotacao == 2)
            {

                spriteBatch.DrawString(font, alternativas[PerguntaAtual][(0 + Rotacao) % 4], new Vector2(145, 355), Color.Black);
                spriteBatch.DrawString(font, alternativas[PerguntaAtual][(1 + Rotacao) % 4], new Vector2(425, 355), Color.Black);
                spriteBatch.DrawString(font, alternativas[PerguntaAtual][(2 + Rotacao) % 4], new Vector2(145, 415), Color.Black);
                spriteBatch.DrawString(font, alternativas[PerguntaAtual][(3 + Rotacao) % 4], new Vector2(425, 415), Color.Black);
            }
            else if (Rotacao == 1 | Rotacao == 3)
            {
                spriteBatch.DrawString(font, alternativas[PerguntaAtual][(2 + Rotacao) % 4], new Vector2(145, 355), Color.Black);
                spriteBatch.DrawString(font, alternativas[PerguntaAtual][(3 + Rotacao) % 4], new Vector2(425, 355), Color.Black);
                spriteBatch.DrawString(font, alternativas[PerguntaAtual][(0 + Rotacao) % 4], new Vector2(145, 415), Color.Black);
                spriteBatch.DrawString(font, alternativas[PerguntaAtual][(1 + Rotacao) % 4], new Vector2(425, 415), Color.Black);
            }

        }


        void TrocarPergunta()
        {
            if (PontosBambu < 100) PerguntaAtual = Rand.Next(PerguntasFacil.Length);
            else if (PontosBambu >= 300) PerguntaAtual = Rand.Next(PerguntasFoda.Length);
            else if (PontosBambu >= 200) PerguntaAtual = Rand.Next(PerguntasDificil.Length);
            else if (PontosBambu >= 100) PerguntaAtual = Rand.Next(PerguntasMedio.Length);            
            
            Rotacao = Rand.Next(4);
        }


        void DesenhaTom(NotaTonalidade Tom)
        {
            Rectangle KeyRectExt = new Rectangle((int)Tom.Pos.X, (int)Tom.Pos.Y, 40, 80);
            Rectangle KeyRectInt = new Rectangle((int)Tom.Pos.X + 3, (int)Tom.Pos.Y + 3, 34, 74);
            if (Tom.PretoBranco == 0) // tecla branca
            {
                spriteBatch.Draw(m_backGroundColorBlack, KeyRectExt, Color.Black); // contorno
                spriteBatch.Draw(m_backGroundColorWhite, KeyRectInt, Color.White); // contorno

                if (Tom.Enarmonia == "") spriteBatch.DrawString(font, Tom.Tom, new Vector2(Tom.Pos.X + 13, Tom.Pos.Y + 25), Color.Black);
                else
                {
                    spriteBatch.DrawString(font, Tom.Tom, new Vector2(Tom.Pos.X + 13, Tom.Pos.Y + 17), Color.Black);
                    spriteBatch.DrawString(font, Tom.Enarmonia, new Vector2(Tom.Pos.X + 8, Tom.Pos.Y + 40), Color.Black);
                }
            }
            else if (Tom.PretoBranco == 1) // tecla preta
            {
                spriteBatch.Draw(m_backGroundColorBlack, KeyRectExt, Color.Black); // contorno
                spriteBatch.DrawString(font, Tom.Tom, new Vector2(Tom.Pos.X + 8, Tom.Pos.Y + 25), Color.White);
            } 
        }

        // escalator
        void DesenhaNota(string nota, float X, float Y, Color cor)
        {            
            string notabemol = nota.Substring(nota.Length - 1, 1);
            string notaLetra = nota.Substring(0, 1);
            if (notabemol == "b")
            {
                string novanota = nota.Substring(0, nota.Length - 1);
                spriteBatch.DrawString(fontEscalator, novanota, new Vector2(X,Y), cor);

                int OffX = 70;
                int OffY = -75;

                if (notaLetra.ToLower() == "c") 
                {
                    OffX -= 17;
                    OffY += 3;
                }
                else if (notaLetra.ToLower() == "d")  
                {
                    OffX -= 7;
                    OffY += 3;
                }
                else if (notaLetra.ToLower() == "e")
                {
                    OffX -= 3;
                    OffY += 5;
                }
                else if (notaLetra.ToLower() == "f")
                {
                    OffX -= 3;
                    OffY += 20;
                    
                }
                else if (notaLetra.ToLower() == "g")
                {
                    OffX -= 10;
                    OffY += 5;
                }
                else if (notaLetra.ToLower() == "a")
                {
                    OffX -= 3;
                    OffY += 6;
                }
                else if (notaLetra.ToLower() == "b")
                {
                    OffX -= 6;
                    OffY += 3;
                }

                spriteBatch.DrawString(Engraver, "b", new Vector2(X + OffX, Y + OffY), cor);
            }
            else spriteBatch.DrawString(fontEscalator, nota, new Vector2(X,Y), cor);   

        }

        void TrocarEscala()
        {
            EscalaTipo = Rand.Next(4);

            EscalaNota = Rand.Next(12);

            if (EscalaNota == 1 || EscalaNota == 6 || EscalaNota == 8) EscalaNota += 1;  // facilitar um pouco

            if (EscalaAtual != "Nenhuma") VirarPagina.Play(); 

            if (EscalaTipo == 0)
            {
                EscalaAtual = EscalasMaiores[EscalaNota][0] + " Maior";
            }
            else if (EscalaTipo == 1)
            {
                EscalaAtual = EscalasMenoresNatural[EscalaNota][0] + " Menor Natural";
            }
            else if (EscalaTipo == 2)
            {
                EscalaAtual = EscalasMenoresHarmonica[EscalaNota][0] + " Menor Harmônica";
            }
            else if (EscalaTipo == 3)
            {
                EscalaAtual = EscalasMenoresMelodica[EscalaNota][0] + " Menor Melódica";
            }
            NotasEscalator.Clear();            
            
        }

        void DesenhaSelecaoMIDI()
        {
            int KeyMIDI = -1;
                List<Pitch> pitches = new List<Pitch>(pitchesPressed.Keys);
                pitches.Sort();

                for (int i = 0; i < pitches.Count; ++i)
                {
                    Pitch pitch = pitches[i];

                    if (Clave == 0)
                    {
                        if (pitch.Octave() == 2) KeyMIDI = PitchTrans[pitch.PositionInOctave()];
                        if (pitch.Octave() == 3) KeyMIDI = PitchTrans2[pitch.PositionInOctave()];
                        else if (pitch == Pitch.C4) KeyMIDI = 14;
                    }
                    else if (Clave == 1 || Clave == 9)
                    {
                        if (pitch.Octave() == 4) KeyMIDI = PitchTrans[pitch.PositionInOctave()];
                        if (pitch.Octave() == 5) KeyMIDI = PitchTrans2[pitch.PositionInOctave()];
                        else if (pitch == Pitch.C6) KeyMIDI = 14;
                    }
                    else if (Clave == 2 || Clave == 11) // CLave de Dó
                    {
                        if (pitch.Octave() == 3) KeyMIDI = PitchTrans[pitch.PositionInOctave()];
                        if (pitch.Octave() == 4) KeyMIDI = PitchTrans2[pitch.PositionInOctave()];
                        else if (pitch == Pitch.C5) KeyMIDI = 14;
                    }
                    else if (Clave == 3)
                    {

                        /*
                        if (pitch.Octave() == 2) KeyMIDI = PitchTrans[pitch.PositionInOctave()];
                        if (pitch.Octave() == 3) KeyMIDI = PitchTrans2[pitch.PositionInOctave()];
                        if (pitch.Octave() == 4) KeyMIDI = PitchTrans3[pitch.PositionInOctave()];
                        if (pitch.Octave() == 5) KeyMIDI = PitchTrans4[pitch.PositionInOctave()];
                        else if (pitch == Pitch.C6) CurrentKeyMIDI = 28;
                   
                         */
                     }
                    else if (Clave == 10) // To DO: Provavelmente corrigir oitava da Clave 10
                    {
                        if (pitch.Octave() == 2) KeyMIDI = PitchTrans[pitch.PositionInOctave()];
                        if (pitch.Octave() == 3) KeyMIDI = PitchTrans2[pitch.PositionInOctave()];
                        else if (pitch == Pitch.C4) KeyMIDI = 14;
                    }
                }
        }

        void DesenhaSelecao() // bola vermelha para indicar a nota selecionada
        {
            if (CurrentKey != -1)
            {

                if (Clave == 9 || Clave == 11)
                {
                    if (CurrentKey <= 7) spriteBatch.Draw(Selecao, (PianoOffset) + new Vector2(215 + (CurrentKey * 53), 135), Color.White);                
                }
                else if (Clave == 10)
                {
                    if (CurrentKey >= 7) spriteBatch.Draw(Selecao, (PianoOffset) + new Vector2(215 + ((CurrentKey-7) * 53), 135), Color.White);
                
                }
                else if (Clave == 0 || Clave == 1 || Clave == 2)
                {
                    spriteBatch.Draw(Selecao, (PianoOffset) + new Vector2(13 + (CurrentKey * 53), 135), Color.White);
                }
            }
        }
        void DesenhaPiano()
        {
            if ( (Clave == 1) && ClaveOriginal == 9 )
            {

                KeyboardState keyState = Keyboard.GetState();


                if (keyState.IsKeyDown(Keys.Down)) halt += 80000;

                if (keyState.IsKeyDown(Keys.Z))
                {
                    spriteBatch.Draw(piano800, pianoPos + PianoOffset, Color.White);
                }
                else
                {
                    spriteBatch.Draw(piano800, new Rectangle((int)pianoPos.X - 372 + a, (int)pianoPos.Y, piano800.Width, piano800.Height), new Rectangle(-372 + a, 0, piano800.Width, piano800.Height), Color.White);
                }

                 a = (int)(Pontos/150 - 400) * 53;
                 if (a > 372) a = 372;
                 else if (a < 0) a = 0;


            }
            else if (Clave != 5 && Clave != 8 && Clave != 9 && Clave != 10 && Clave != 11 && ClaveOriginal != 3 && Clave != 4)
            {
                spriteBatch.Draw(piano800, pianoPos + PianoOffset, Color.White);
            }
            else if (Clave == 5)
            {
                spriteBatch.Draw(PianoFrame, pianoPos, Color.White);
                spriteBatch.Draw(Cravo, new Vector2(150, 30), null, Color.White, 0.09f, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0f);
                //spriteBatch.Draw(ballSprite, new Vector2(600, 25), null, Color.White, 0f, new Vector2(0, 0), 1.5f, SpriteEffects.None, 0f);
                spriteBatch.Draw(ballSprite, new Vector2(600, 40), new Rectangle(0, 10, ballSprite.Width, ballSprite.Height - 20), Color.White, 0f, new Vector2(0, 0), 1.5f, SpriteEffects.None, 0);
                spriteBatch.Draw(CifraSprite[CifraBaixo[CurrentNote - 5][Temp]], new Vector2(600, 25) + new Vector2(65, 85), null, Color.White, 0f, new Vector2(0, 0), 1.5f, SpriteEffects.None, 0f);
            }
            else if (Clave == 4)
            {
                spriteBatch.Draw(PianoFrame, pianoPos, Color.White);

                List<Pitch> pitches = new List<Pitch>(pitchesPressed.Keys);
                pitches.Sort();

                List<Pitch> pitchesFiltradas = new List<Pitch>();
                bool Repetida = false;

                for (int i = 0; i < pitches.Count; ++i)
                {
                    if (i == 0) pitchesFiltradas.Add(pitches[i]);
                    else
                    {
                        for (int k = 0; i - k > 0; ++k)
                        {
                            if (pitches[i].PositionInOctave() == pitches[k].PositionInOctave())
                            {
                                Repetida = true;
                            }
                        }
                        if (Repetida == false) pitchesFiltradas.Add(pitches[i]);
                        Repetida = false;
                    }
                }
                pitchesFiltradas.Sort();

                List<Pitch> pitchesFiltradas2 = new List<Pitch>();
                for (int i = 0; i < pitchesFiltradas.Count; ++i)
                {

                    int p = (int)pitchesFiltradas[i];

                    while (p - (int)pitchesFiltradas[0] > 12)
                    {
                        p -= 12;
                    }

                    pitchesFiltradas2.Add((Midi.Pitch)p);

                }
                pitchesFiltradas2.Sort();


                string Notas = "Notas:  ";
                for (int i = 0; i < pitches.Count; ++i)
                {
                    Pitch pitch = pitches[i];
                    if (i > 0)
                    {
                        Notas += ", ";
                    }

                    Notas += pitch.NotePreferringSharps();
                    Notas += "";
                }

                string Acordes = "Acorde: ";
                List<Chord> chords = Chord.FindMatchingChords(pitchesFiltradas2);
                for (int i = 0; i < chords.Count; ++i)
                {
                    Chord chord = chords[i];
                    if (i > 0)
                    {
                        Acordes += " ou ";
                    }
                    Acordes += chord;
                }

                spriteBatch.DrawString(font, "MIDI IN:  " + MidiIn.Name, new Vector2(50, 60), Color.Red);
                spriteBatch.DrawString(Menufont, Notas, new Vector2(50, 90), Color.Red);
                spriteBatch.DrawString(Menufont, Acordes, new Vector2(50, 130), Color.Red);
                //spriteBatch.DrawString(Menufont, notasdebug, new Vector2(50, 90), Color.Red);
                //spriteBatch.DrawString(Menufont, notasdebug2, new Vector2(50, 130), Color.Red);



            }
            else if (Clave == 9 || Clave == 10 || Clave == 11)
            {
                Vector2 Distancia = new Vector2(200, 0);
                spriteBatch.Draw(piano800metade, pianoPos + PianoOffset + Distancia, Color.White);
            }

        }

        void DesenhaReferencia()
        {
            // desenha nota referencia
            // CORRIGIR GAMEBOY
            if (Clave == 0 || Clave == 1 || Clave == 2)
            {
                if (DIFICULDADE <= 2) spriteBatch.Draw(NotaZero[0 + (Clave * 2)], new Vector2(12, 156) + PianoOffset, null, Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                if (DIFICULDADE < 1 || ModoSurpresa) spriteBatch.Draw(NotaZero[1 + (Clave * 2)], new Vector2(381, 156) + PianoOffset, null, Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
            }
            else if (Clave == 9)
            {
                if (GAMEBOY == true)
                {
                    spriteBatch.Draw(NotaZero[0 + (1 * 2)], new Vector2(12, 156) + PianoOffset, null, Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                }
                else spriteBatch.Draw(NotaZero[2], new Vector2(12 + 200, 156) + PianoOffset, null, Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                

            }
            else if (Clave == 10)
            {
                if (GAMEBOY == true)
                {
                    spriteBatch.Draw(NotaZero[0 + (0 * 2)], new Vector2(12, 156) + PianoOffset, null, Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                }
                else spriteBatch.Draw(NotaZero[1], new Vector2(12 + 200, 156) + PianoOffset, null, Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
            }
            else if (Clave == 11)
            {
                if (GAMEBOY == true)
                {
                    spriteBatch.Draw(NotaZero[0 + (2 * 2)], new Vector2(12, 156) + PianoOffset, null, Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
                }
                else spriteBatch.Draw(NotaZero[4], new Vector2(12 + 200, 156) + PianoOffset, null, Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
            }

            if (GAMEBOY == true && GBTYPE == 1 && (Clave == 8 || Clave == 7 || Clave == 6 || Clave == 5 || Clave == 4 )) // minigame do Bowser
            {
                //talvez
                noteSprite[0] = Content.Load<Texture2D>("C1");
                noteSprite[1] = Content.Load<Texture2D>("D1");
                noteSprite[2] = Content.Load<Texture2D>("E1");
                noteSprite[3] = Content.Load<Texture2D>("F1");
                noteSprite[4] = Content.Load<Texture2D>("G1");
                noteSprite[5] = Content.Load<Texture2D>("A1");
                noteSprite[6] = Content.Load<Texture2D>("B1");
                noteSprite[7] = Content.Load<Texture2D>("C2");
                noteSprite[8] = Content.Load<Texture2D>("D2");
                noteSprite[9] = Content.Load<Texture2D>("E2");
                noteSprite[10] = Content.Load<Texture2D>("F2");
                noteSprite[11] = Content.Load<Texture2D>("G2");
                noteSprite[12] = Content.Load<Texture2D>("A2");
                noteSprite[13] = Content.Load<Texture2D>("B2");
                noteSprite[14] = Content.Load<Texture2D>("C3");
            
                spriteBatch.Draw(NotaZero[0], new Vector2(12 + 200, 156) + PianoOffset, null, Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
            }
        }


        void DrawHUD2P()
        {
            spriteBatch.Draw(StatusBar, new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(fontSmall, "Jogador 1   Vidas: " + Vidas.ToString(), new Vector2(5, 3), Color.White);
            spriteBatch.DrawString(fontSmall, "Pontos: " + Pontos.ToString(), new Vector2(200, 3), Color.White);

            spriteBatch.DrawString(fontSmall, "Jogador 2   Vidas: " + Vidas2.ToString(), new Vector2(450, 3), Color.White);
            spriteBatch.DrawString(fontSmall, "Pontos: " + Pontos2.ToString(), new Vector2(650, 3), Color.White);

            spriteBatch.Draw(m_backGroundColorBlack, new Rectangle(0, 25, 800, 3), Color.White);

        }

        void DrawHUD()
        {
            spriteBatch.Draw(StatusBar, new Vector2(0, 0), Color.White);

            spriteBatch.Draw(m_backGroundColorLightBlue, new Rectangle(260, 4, 300, 20), Color.White);

            if (((-0.3F) + (Pontos * 0.0004F)) < 1)
            {
                spriteBatch.Draw(m_backGroundColorWhite, new Rectangle(263, 6, 294, 16), Color.White);
                spriteBatch.Draw(m_backGroundColorLightBlue, new Rectangle(263, 6, ((int)(Pontos / 50 * 4.5)), 16), Color.White);
            }

            spriteBatch.DrawString(fontSmall, "Vidas: " + Vidas.ToString(), new Vector2(5, 3), Color.White);
            spriteBatch.DrawString(fontSmall, "Pontos: " + Pontos.ToString(), new Vector2(85, 3), Color.White);
            spriteBatch.DrawString(fontSmall, "Status ", new Vector2(200, 3), Color.White);

            string[] Rankings = { 
                    "Cotoco", 
                    "Joseph Klimber",
                    "Maneta",
                    "Mão furada",
                    "Menudo",
                    "Ramone",
                    "Backstreet Boy",
                    "Carteirinha OMB",
                    "Funcionário da OMB",
                    "Presidente da OMB",
                    "Soldado imperial",
                    "Chewbacca",
                    "Guitarrista",
                    "Guitarrista 7 cordas",
                    "Violista cordas soltas",
                    "Violista",
                    "Richard Clayderman",
                    "Maestro",
                    "Violinista mesotônico",
                    "Violinista",
                    "Prova específica",
                    "Vestibular da URGS",
                    "Estudante da URGS",
                    "TP I",
                    "TP II", 
                    "TP III",
                    "TP IV",
                    "Monitor",
                    "Bolsista voluntário",
                    "Bolsista BIC",
                    "Compositor",
                    "Nerd inútil",
                    "Onanista",
                    "Pianista",
                    "Graduado",
                    "Mestrando",
                    "Mestre",
                    "Mestre de cerimônias", // 30
                    "Mestre dos magos",
                    "Cavaleiro Jedi",
                    "Georce Lucas",
                    "Desocupado",
                    "Desempregado",
                    "Acadêmico",
                    "Membro da ANPPOM",
                    "Doutorando",
                    "Doutor",
                    "Dr. McCoy",
                    "Musicólogo",
                    "Susan McClary",
                    "Filho de Bach",
                    "J.S. Bach",
                    "Viciado em Ritalina",
                    "Capitão da Enterprise",
                    "CEO da TDAH Games",
                    "Prof. Substituto",
                    "Prof. Assistente",
                    "Prof. Adjunto",
                    "Pesquisador CNPq",
                };

            if (Pontos / (3250 / Rankings.Length) < Rankings.Length)
            {
                Ranking = Rankings[Pontos / Rankings.Length];
            }
            else Ranking = Rankings[Rankings.Length - 1];

            spriteBatch.DrawString(fontSmall, "Ranking: ", new Vector2(570, 3), Color.White);

            if (Ranking.Length < 17) spriteBatch.DrawString(fontSmall, Ranking, new Vector2(645, 3), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            else spriteBatch.DrawString(fontSmall, Ranking, new Vector2(645, 4), Color.White, 0, new Vector2(0, 0), 0.85f, SpriteEffects.None, 0);

        }

        void Morrer()
        {
            ResetaGB();
            pitchesPressed.Clear();
            pitchesPressed2.Clear();
            Bonus.Clear(); 
            SalvaRecordes();            
            Inversoes = 0;
            Vidas = 3;
            Vidas2 = 3;
            Vida1000 = false;
            Vida2000 = false;
            Vida3000 = false;
            RBPos = new Vector2(200, 198);

            ShowMM = false;
            ShowFerer = false;
            ShowRB = false;
            Pt250 = false;
            MENU = 1;
            SUBMENU = 666;
            MarioOverPos = new Vector2(445, 175);
            MarioOverDir = 0;
            RBStatus = 0;

            if (Pontos < 500) OVERTYPE = 1;
            else OVERTYPE = 0;

            soundEngine.Stop();
            soundEngineGretchen.Stop();
            soundEngineMario.Stop();
            soundEnginePac.Stop();
            ballPosition = new Vector2(150, 200);
            soundEngineMorte.Play();

            if (InputDevice.InstalledDevices.Count != 0)
            {
                MidiIn.StopReceiving();
                MidiIn.Close();
            }
            timer.Stop();
            TempoJogo = timer.Elapsed;
            PizzaStatus = 0;
            Vida1000 = false;
            Vida2000 = false;
            Vida3000 = false;
            if (ModoSurpresa) Clave = 7;

            ModoSurpresa = false;
            MostrarTeclado = true;
            PianoShake = false;
            PianoShake2 = false;

            Clave = ClaveOriginal;

        }
        void Surpresa()
        {
            if (CONTROLE == 0) Clave = Rand.Next(3);
            else if (CONTROLE == 1)
            {
                Clave = Rand.Next(7);
                if (Clave == 3) Clave = 2;
            }
            ConfiguraProximaNota(); // carrega os recursos necessários
        }

        void ConfiguraProximaNota()
        {
            if (Clave == 5)
            {
                if (DIFICULDADE == 2)
                {
                    CifraBaixo = new int[][]// indices para array CifraSprite
                                {
                                    new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 }, // la (==5)
                                    new int[] { 0, 2, 3, 5, 6, 7, 10, 11, 12, 16}, // b
                                    new int[] { 0, 17, 2, 3, 5, 7, 10, 11, 18, 15, 16}, // c
                                    new int[] { 0, 1, 2, 3, 4, 5, 7, 15, 10, 11, 13, 9 },  // d
                                    new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 13, 14 }, // e
                                    new int[] { 0, 17, 2, 3, 5, 7, 11, 16}, //f
                                    new int[] { 0, 17, 2, 3, 5, 7, 10, 11, 15 }, // G
                                };
                }
                if (DIFICULDADE == 1)
                {
                    CifraBaixo = new int[][]// indices para array CifraSprite
                                {
                                    new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, // la (==5)
                                    new int[] { 0, 2, 3, 5, 6, 7}, // b
                                    new int[] { 0, 17, 2, 3, 5, 7}, // c
                                    new int[] { 0, 1, 2, 3, 4, 5, 7},  // d
                                    new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }, // e
                                    new int[] { 0, 17, 2, 3, 5, 7 }, //f
                                    new int[] { 0, 17, 2, 3, 5, 7 }, // G
                                };
                }
                if (DIFICULDADE == 0)
                {
                    CifraBaixo = new int[][]// indices para array CifraSprite
                                {
                                    new int[] { 0, 1, 2, 3, 4, 5 }, // la (==5)
                                    new int[] { 0, 2, 3, 5, 6 }, // b
                                    new int[] { 0, 17, 2, 3, 5,}, // c
                                    new int[] { 0, 1, 2, 3, 4, 5 },  // d
                                    new int[] { 0, 1, 2, 3, 4, 5 }, // e
                                    new int[] { 0, 17, 2, 3, 5 }, //f
                                    new int[] { 0, 17, 2, 3, 5 }, // G
                                };
                }

            }

            if (Clave == 0 || Clave == 5)
            {
                noteSprite[0] = Content.Load<Texture2D>("C1");
                noteSprite[1] = Content.Load<Texture2D>("D1");
                noteSprite[2] = Content.Load<Texture2D>("E1");
                noteSprite[3] = Content.Load<Texture2D>("F1");
                noteSprite[4] = Content.Load<Texture2D>("G1");
                noteSprite[5] = Content.Load<Texture2D>("A1");
                noteSprite[6] = Content.Load<Texture2D>("B1");
                noteSprite[7] = Content.Load<Texture2D>("C2");
                noteSprite[8] = Content.Load<Texture2D>("D2");
                noteSprite[9] = Content.Load<Texture2D>("E2");
                noteSprite[10] = Content.Load<Texture2D>("F2");
                noteSprite[11] = Content.Load<Texture2D>("G2");
                noteSprite[12] = Content.Load<Texture2D>("A2");
                noteSprite[13] = Content.Load<Texture2D>("B2");
                noteSprite[14] = Content.Load<Texture2D>("C3");
            }
            else if (Clave == 1)
            {
                noteSprite[0] = Content.Load<Texture2D>("C1_ClaveSol");
                noteSprite[1] = Content.Load<Texture2D>("D1_ClaveSol");
                noteSprite[2] = Content.Load<Texture2D>("E1_ClaveSol");
                noteSprite[3] = Content.Load<Texture2D>("F1_ClaveSol");
                noteSprite[4] = Content.Load<Texture2D>("G1_ClaveSol");
                noteSprite[5] = Content.Load<Texture2D>("A1_ClaveSol");
                noteSprite[6] = Content.Load<Texture2D>("B1_ClaveSol");
                noteSprite[7] = Content.Load<Texture2D>("C2_ClaveSol");
                noteSprite[8] = Content.Load<Texture2D>("D2_ClaveSol");
                noteSprite[9] = Content.Load<Texture2D>("E2_ClaveSol");
                noteSprite[10] = Content.Load<Texture2D>("F2_ClaveSol");
                noteSprite[11] = Content.Load<Texture2D>("G2_ClaveSol");
                noteSprite[12] = Content.Load<Texture2D>("A2_ClaveSol");
                noteSprite[13] = Content.Load<Texture2D>("B2_ClaveSol");
                noteSprite[14] = Content.Load<Texture2D>("C3_ClaveSol");
            }
            else if (Clave == 2)
            {
                noteSprite[0] = Content.Load<Texture2D>("C1_ClaveDo");
                noteSprite[1] = Content.Load<Texture2D>("D1_ClaveDo");
                noteSprite[2] = Content.Load<Texture2D>("E1_ClaveDo");
                noteSprite[3] = Content.Load<Texture2D>("F1_ClaveDo");
                noteSprite[4] = Content.Load<Texture2D>("G1_ClaveDo");
                noteSprite[5] = Content.Load<Texture2D>("A1_ClaveDo");
                noteSprite[6] = Content.Load<Texture2D>("B1_ClaveDo");
                noteSprite[7] = Content.Load<Texture2D>("C2_ClaveDo");
                noteSprite[8] = Content.Load<Texture2D>("D2_ClaveDo");
                noteSprite[9] = Content.Load<Texture2D>("E2_ClaveDo");
                noteSprite[10] = Content.Load<Texture2D>("F2_ClaveDo");
                noteSprite[11] = Content.Load<Texture2D>("G2_ClaveDo");
                noteSprite[12] = Content.Load<Texture2D>("A2_ClaveDo");
                noteSprite[13] = Content.Load<Texture2D>("B2_ClaveDo");
                noteSprite[14] = Content.Load<Texture2D>("C3_ClaveDo");
            }

            int newNote = Rand.Next(15);
            if (Clave == 3) newNote = Rand.Next(29);
            else if (Clave == 5)
            {
                newNote = 5 + Rand.Next(7);
                Temp = Rand.Next(CifraBaixo[newNote - 5].Length);
                CurrentChord = new Chord(AcordeBaixo[newNote - 5][Temp]);
            }
            else if (Clave == 6)
            {
                int Modo = Rand.Next(2);
                int Tom = Rand.Next(12);
                int Acorde = 0;
                if (DIFICULDADE == 0) Acorde = Rand.Next(AcordeGrausFacil[0].Length - 1);
                else if (DIFICULDADE == 1) Acorde = Rand.Next(AcordeGrausMedio[0].Length - 1);
                else if (DIFICULDADE == 2) Acorde = Rand.Next(AcordeGrausDificil[0].Length - 1);
                Clave6Acorde = Tonalidades[Modo][Tom] + ": " + AcordeGrausDificil[Modo][Acorde];

                string NewChord = "";
                if (Modo == 0) NewChord = EscalasMaiores[Tom][AcordeGrausIndex[Acorde]] + AcordeGrausPatterns[Modo][Acorde];
                else if (Modo == 1) NewChord = EscalasMenoresNatural[Tom][AcordeGrausIndex[Acorde]] + AcordeGrausPatterns[Modo][Acorde];
                CurrentChord = new Chord(NewChord);

            }
            else if (Clave == 4 || Clave == 8)
            {
                string Acorde = "C";
                string[] RootTipo = new string[] { "C", "Db", "C#", "D", "Eb", "E", "F", "F#", "G", "G#", "Ab", "A", "Bb", "B" };

                if (DIFICULDADE == 0 && Pontos <= 1000)
                {
                    string[] PatternTipo = new string[] { "", "m" };
                    Acorde = RootTipo[Rand.Next(14)] + PatternTipo[Rand.Next(2)];
                }
                else if (DIFICULDADE == 0 && Pontos > 1000)
                {
                    string[] PatternTipo = new string[] { "", "m", "7" };
                    Acorde = RootTipo[Rand.Next(14)] + PatternTipo[Rand.Next(3)];
                }
                else if (DIFICULDADE == 1 && Pontos <= 1000)
                {
                    string[] PatternTipo = new string[] { "", "m", "7", "aug", "dim", };
                    Acorde = RootTipo[Rand.Next(14)] + PatternTipo[Rand.Next(5)];
                }
                else if (DIFICULDADE == 1 && Pontos > 1000)
                {
                    string[] PatternTipo = new string[] { "", "m", "7", "aug", "dim", "m7", "7M" };
                    Acorde = RootTipo[Rand.Next(14)] + PatternTipo[Rand.Next(7)];
                }
                else if (DIFICULDADE == 2 && Pontos <= 800)
                {
                    string[] PatternTipo = new string[] { "", "m", "7", "aug", "dim", "m7", "7M", "dim7" };
                    Acorde = RootTipo[Rand.Next(14)] + PatternTipo[Rand.Next(8)];
                }
                else if (DIFICULDADE == 2 && Pontos > 800)
                {
                    string[] PatternTipo = new string[] { "", "m", "7", "aug", "dim", "m7", "dim7", "Ø", "7M", "m7M" };
                    Acorde = RootTipo[Rand.Next(14)] + PatternTipo[Rand.Next(10)];
                }

                CurrentChord = new Chord(Acorde);
            }

            ballSprite = noteSprite[newNote];
            if (Clave == 4 || Clave == 6 || Clave == 8 ) ballSprite = AcordeMoldura;

            CurrentNote = newNote;

            // To DO: Checar se todas as claves foram contemplados (acordes, cifrado, etc)

        }
        void AjustaFacaY()
        {
            if (FacaPos.Y < 210) FacaMovY = 1;
            else if (FacaPos.Y > 240)
            {
                int Chance = Rand.Next(3);
                if (Chance == 2) FacaMovY = 2;
            } 
        }

        void PalitoPular()
        {
            if (Pulo == false)
            {
                Pulo = true;
                VelocidadePulo = -3;
                PalitoPos.Y += VelocidadePulo;
            }
            else
            {
                if (PalitoPos.Y < 150) Queda = true;

                if (!Queda) PalitoPos.Y += VelocidadePulo;
                else PalitoPos.Y -= VelocidadePulo -1;

                if (PalitoPos.Y >= 220)
                {
                    PalitoPos.Y = 220;
                    Pulo = false;
                    Queda = false;
                }
            }
     
        }

        private static int TryToParse(string value)
        {
            int number;
            bool result = Int32.TryParse(value, out number);
            if (result) return number;
            else return 0;
        }

        void VerificaInversoes(Chord acorde, int jogador)
        {
            if (acorde.Inversion != 0)
            {

                Inversoes += 1;

                if (Inversoes == 5)
                {
                    Bonus.Add(new BonusMsg(ballPosition, "+50 pontos\n(5 inversões)", 30, new Vector2(0, -35)));

                    if (jogador == 1) Pontos += 50;
                    else if (jogador == 2) Pontos2 += 50;
                    Inversoes = 0;
                }
                else
                {
                    Bonus.Add(new BonusMsg(ballPosition, "+20 pontos\n (inversão)", 30, new Vector2(0, -35)));
                    if (jogador == 1) Pontos += 20;
                    else if (jogador == 2) Pontos2 += 20;
                } 
                
            }
            else Inversoes = 0;

        }
        void VerificaRecorde()
        {
            int RecordeAtual;

            if (ModoSurpresa)
            {
                try
                {
                    RecordeAtual = Int32.Parse(Recordes[7 + (DIFICULDADE * 11)]);
                }
                catch
                {
                    RecordeAtual = 0;
                }

                if (Pontos > RecordeAtual)
                {
                    Recordes[7 + (DIFICULDADE * 11)] = Pontos.ToString();

                }
            }
            else
            {

                try
                {
                    RecordeAtual = Int32.Parse(Recordes[ClaveOriginal + (DIFICULDADE * 11)]);
                }
                catch
                {
                    RecordeAtual = 0;
                }

                if (Pontos > RecordeAtual)
                {
                    Recordes[ClaveOriginal + (DIFICULDADE * 11)] = Pontos.ToString();

                }
            }

        }
        void SalvaRecordes()
        {
            String RecordesPath = Directory.GetCurrentDirectory() + "\\FDATA";
            StreamWriter sw = new StreamWriter(RecordesPath);

            for (int i = 0; i < 35; i++)
            {
                sw.WriteLine(Recordes[i]);
            }            
            sw.WriteLine(FacaRecorde);
            sw.WriteLine(EscalatorRecorde);
            sw.WriteLine(BambuRecorde);
            sw.WriteLine(TomRecorde);
            sw.WriteLine(LinkRecorde);
            sw.WriteLine(PalavraRecorde);            
            sw.Close();
        }
        void CarregaRecordes()
        {
            String RecordesPath = Directory.GetCurrentDirectory() + "\\FDATA";

            if (File.Exists(RecordesPath))
            {
                StreamReader sr = new StreamReader(RecordesPath);
                for (int i = 0; i < 35; i++)
                {
                    Recordes[i] = sr.ReadLine();
                }
                FacaRecorde = TryToParse(sr.ReadLine());
                EscalatorRecorde = TryToParse(sr.ReadLine());
                BambuRecorde = TryToParse(sr.ReadLine());
                TomRecorde = TryToParse(sr.ReadLine());
                LinkRecorde = TryToParse(sr.ReadLine());
                PalavraRecorde = TryToParse(sr.ReadLine());
                
                sr.Close();

                // TO DO: TIRAR ISSO
                //PrimeiraDistracao = false;
            }
            else
            {
                StreamWriter sw = new StreamWriter(RecordesPath);

                for (int i = 0; i < 35; i++)
                {
                    sw.WriteLine("---");
                }

                for (int i = 0; i < 35; i++ )
                {
                    Recordes[i] = "---";
                }

                sw.WriteLine("0"); // FacaRecorde
                sw.WriteLine("0");  // EscalatorRecorde
                sw.WriteLine("0");  // Bambu Recorde                
                sw.WriteLine("0"); // Tom REcorde
                sw.WriteLine("0"); // Link REcorde
                sw.WriteLine("0"); // Palavra REcorde  
                FacaRecorde = 0;
                EscalatorRecorde = 0;
                BambuRecorde = 0;
                TomRecorde = 0;
                LinkRecorde = 0;
                PalavraRecorde = 0;
                sw.Close();
                PrimeiraDistracao = true;
            }

        }

        protected override void Initialize()
        {      
            base.Initialize();

            ResetaGB();
                        
            BowserPos.X = 800;
            BowserEnergia = 25;
            BowserNotas[0] = Rand.Next(15);
            BowserNotas[1] = Rand.Next(15);
            BowserNotas[2] = Rand.Next(15);
            BowserNotas[3] = Rand.Next(15);
            BowserNotas[4] = Rand.Next(15);
            BowserNotas[5] = Rand.Next(15);
            BowserNotas[6] = Rand.Next(15);            
            CarregaRecordes();

            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            renderTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);

            if (InputDevice.InstalledDevices.Count != 0)
            {
                foreach (InputDevice device in InputDevice.InstalledDevices)
                {
                    MIDICount += 1;
                }
            }
          

            // Set the initial paddle location
            paddlePosition = new Vector2(
                graphics.GraphicsDevice.Viewport.Width / 2 - paddleSprite.Width / 2,
                graphics.GraphicsDevice.Viewport.Height - paddleSprite.Height);

            paddlePosition2 = new Vector2(
                            graphics.GraphicsDevice.Viewport.Width / 2 - paddleSprite.Width / 2,
                            graphics.GraphicsDevice.Viewport.Height - paddleSprite.Height - 472); 

            m_backGroundColorGreen = new Texture2D(GraphicsDevice, 1, 1);
            m_backGroundColorGreen.SetData<Color>(new Color[] { Color.Green });
            m_backGroundColorWhite = new Texture2D(GraphicsDevice, 1, 1);
            m_backGroundColorWhite.SetData<Color>(new Color[] { Color.White });
            m_backGroundColorRed = new Texture2D(GraphicsDevice, 1, 1);
            m_backGroundColorRed.SetData<Color>(new Color[] { Color.Red });
            m_backGroundColorLightBlue = new Texture2D(GraphicsDevice, 1, 1);
            m_backGroundColorLightBlue.SetData<Color>(new Color[] { new Color(135,206,250) });
            m_backGroundColorBlack = new Texture2D(GraphicsDevice, 1, 1);
            m_backGroundColorBlack.SetData<Color>(new Color[] { Color.Black });

            MMPos.X = Rand.Next(600) + 70;
            
        }

        public void NoteOn(NoteOnMessage msg)
        {            
            lock (this)
            {                
                if (Clave != 8)
                {
                    pitchesPressed[msg.Pitch] = true;
                    if (msg.Velocity == 0) pitchesPressed.Remove(msg.Pitch);
                }
                else if (Clave == 8) // 2P
                {                    

                    if (msg.Pitch >= Pitch.C4)
                    {
                        pitchesPressed[msg.Pitch] = true;
                        if (msg.Velocity == 0) pitchesPressed.Remove(msg.Pitch);
                    }
                    else
                    {
                        pitchesPressed2[msg.Pitch] = true;
                        if (msg.Velocity == 0) pitchesPressed2.Remove(msg.Pitch);
                    }

                    // controle
                    if (msg.Pitch == Pitch.DSharp2 || msg.Pitch == Pitch.CSharp2 || msg.Pitch == Pitch.DSharp4 || msg.Pitch == Pitch.CSharp4)
                    {
                        pitchesPressedController[msg.Pitch] = true;
                        pitchesPressed.Remove(msg.Pitch);
                        pitchesPressed2.Remove(msg.Pitch);
                    }

                }
          
            }
        } 
        public void NoteOff(NoteOffMessage msg)
        {
            lock (this)
            {
                if (Clave != 8) pitchesPressed.Remove(msg.Pitch);
                else if (Clave == 8)
                {
                    if (msg.Pitch == Pitch.DSharp2 || msg.Pitch == Pitch.CSharp2 || msg.Pitch == Pitch.DSharp4 || msg.Pitch == Pitch.CSharp4)
                    {
                        pitchesPressedController.Remove(msg.Pitch);
                    }

                    if (msg.Pitch >= Pitch.C4) // verificar: dó central?
                    {
                        pitchesPressed.Remove(msg.Pitch);
                    }                   
                    else
                    {
                        pitchesPressed2.Remove(msg.Pitch);
                    }

                }

            }
        }

        protected override void LoadContent()
        {           
            spriteBatch = new SpriteBatch(GraphicsDevice);
            TMNT = Content.Load<Texture2D>("tmnt");
            FundoMadeira = Content.Load<Texture2D>("FundoMadeira");
            Escalator = Content.Load<Texture2D>("EscalatorPic");
            Engraver = Content.Load<SpriteFont>("SpriteFont5");
            EmuFont = Content.Load<SpriteFont>("FonteEmu");
            TMonk = Content.Load<Texture2D>("Outsiders");
            Mira = Content.Load<Texture2D>("mira");
            Gretchen = Content.Load<Texture2D>("gretchen03");            
            Ninja = Content.Load<Texture2D>("ninja");
            Harpsichord = Content.Load<Texture2D>("harpsichord");
            Cravo = Content.Load<Texture2D>("Cravo");
            MarioOver = Content.Load<Texture2D>("MarioOver");
            facepalm[0] = Content.Load<Texture2D>("facepalm");
            facepalm[1] = Content.Load<Texture2D>("facepalm2");
            TV = Content.Load<Texture2D>("TV");            
            PacMan = Content.Load<Texture2D>("pacman");
            Silvio = Content.Load<Texture2D>("SilvioSantos");
            BotaoVazio = Content.Load<Texture2D>("BotaoVazio");
            BotaoCheio = Content.Load<Texture2D>("BotaoCheio");
            Universitario = Content.Load<Texture2D>("universitario");
            ClaveFa = Content.Load<Texture2D>("ClaveFa");
            ClaveFaMetade = Content.Load<Texture2D>("ClaveFaMetade");
            ClaveSol = Content.Load<Texture2D>("ClaveSol");
            ClaveSolMetade = Content.Load<Texture2D>("ClaveSolMetade");                        
            ClaveSolFa = Content.Load<Texture2D>("ClaveSolFa");
            ClaveDo = Content.Load<Texture2D>("ClaveDo");
            ClaveDoMetade = Content.Load<Texture2D>("ClaveDoMetade");
            Clave2P = Content.Load<Texture2D>("Clave2P");
            ClaveAcordes = Content.Load<Texture2D>("ClaveAcordes");
            ClaveBaixoCifrado = Content.Load<Texture2D>("ClaveBaixoCifrado");
            ClaveHarmonia = Content.Load<Texture2D>("ClaveHarmonia");
            ClaveInterrogacao = Content.Load<Texture2D>("ClaveInterrogacao");
            Turtle = Content.Load<Texture2D>("turtle");
            paddleSprite = Content.Load<Texture2D>("hand");
            GameBoy = Content.Load<Texture2D>("gameboy");
            Ps3 = Content.Load<Texture2D>("Ps3");
            Bowser = Content.Load<Texture2D>("bowser");
            noteSprite = new Texture2D[30];
            CifraSprite = new Texture2D[30];
            noteSounds = new SoundEffect[29];
            Link = new Texture2D[8];
            Link[0] = Content.Load<Texture2D>("linkwalkup-0");
            Link[1] = Content.Load<Texture2D>("linkwalkup-1");
            Link[2] = Content.Load<Texture2D>("linkwalkdown-0");
            Link[3] = Content.Load<Texture2D>("linkwalkdown-1");
            Link[4] = Content.Load<Texture2D>("linkwalkleft-0");
            Link[5] = Content.Load<Texture2D>("linkwalkleft-1");
            Link[6] = Content.Load<Texture2D>("linkwalkright-0");
            Link[7] = Content.Load<Texture2D>("linkwalkright-1");

            InimigosLink = new Texture2D[8];
            InimigosLink[0] = Content.Load<Texture2D>("linkdarknut-0");
            InimigosLink[1] = Content.Load<Texture2D>("linkdarknut-1");
            InimigosLink[2] = Content.Load<Texture2D>("linkocto-0");
            InimigosLink[3] = Content.Load<Texture2D>("linkocto-1");
            InimigosLink[4] = Content.Load<Texture2D>("rupy-0");
            InimigosLink[5] = Content.Load<Texture2D>("rupy-1");
            InimigosLink[6] = Content.Load<Texture2D>("linkstalfos-0");
            InimigosLink[7] = Content.Load<Texture2D>("linkstalfos-1");

            Pizza = new Texture2D[5];
            Forca = new Texture2D[9];
            Forca[0] = Content.Load<Texture2D>("Forca8");
            Forca[1] = Content.Load<Texture2D>("forca7");
            Forca[2] = Content.Load<Texture2D>("Forca6");
            Forca[3] = Content.Load<Texture2D>("Forca5");
            Forca[4] = Content.Load<Texture2D>("Forca4");
            Forca[5] = Content.Load<Texture2D>("Forca3");
            Forca[6] = Content.Load<Texture2D>("Forca2");
            Forca[7] = Content.Load<Texture2D>("forca1");
            Forca[8] = Content.Load<Texture2D>("Forca");
            TomQuadrado = Content.Load<Texture2D>("TomQuadrado");
            FundoDrMario = Content.Load<Texture2D>("FundoDr");
            Olivetti = Content.Load<Texture2D>("olivetti");
            Selecao = Content.Load<Texture2D>("selecao");
            StatusBar = Content.Load<Texture2D>("StatusBar2P");
            TecladoInstrucao = Content.Load<Texture2D>("teclado2_antigo");
            hand1 = Content.Load<Texture2D>("Hand1");
            hand2 = Content.Load<Texture2D>("Hand2");           
            NotaZero = new Texture2D[6];
            NotaZero[0] = Content.Load<Texture2D>("Fa0");
            NotaZero[1] = Content.Load<Texture2D>("Fa1");
            NotaZero[2] = Content.Load<Texture2D>("Sol0");
            NotaZero[3] = Content.Load<Texture2D>("Sol1");
            NotaZero[4] = Content.Load<Texture2D>("Do0");
            NotaZero[5] = Content.Load<Texture2D>("Do1");
            HomemPalito = new Texture2D[3];
            TonalidadesFig = new Texture2D[30];
            TonalidadesFig[0] = Content.Load<Texture2D>("Tonalidade00");
            TonalidadesFig[1] = Content.Load<Texture2D>("Tonalidade01");
            TonalidadesFig[2] = Content.Load<Texture2D>("Tonalidade02");
            TonalidadesFig[3] = Content.Load<Texture2D>("Tonalidade03");
            TonalidadesFig[4] = Content.Load<Texture2D>("Tonalidade04");
            TonalidadesFig[5] = Content.Load<Texture2D>("Tonalidade05");
            TonalidadesFig[6] = Content.Load<Texture2D>("Tonalidade06");
            TonalidadesFig[7] = Content.Load<Texture2D>("Tonalidade07");
            TonalidadesFig[8] = Content.Load<Texture2D>("Tonalidade08");
            TonalidadesFig[9] = Content.Load<Texture2D>("Tonalidade09");
            TonalidadesFig[10] = Content.Load<Texture2D>("Tonalidade10");
            TonalidadesFig[11] = Content.Load<Texture2D>("Tonalidade11");
            TonalidadesFig[12] = Content.Load<Texture2D>("Tonalidade12");
            TonalidadesFig[13] = Content.Load<Texture2D>("Tonalidade13");
            TonalidadesFig[14] = Content.Load<Texture2D>("Tonalidade14");
            TonalidadesFig[15] = Content.Load<Texture2D>("TonalidadeFa0");
            TonalidadesFig[16] = Content.Load<Texture2D>("TonalidadeFa1");
            TonalidadesFig[17] = Content.Load<Texture2D>("TonalidadeFa2");
            TonalidadesFig[18] = Content.Load<Texture2D>("TonalidadeFa3");
            TonalidadesFig[19] = Content.Load<Texture2D>("TonalidadeFa4");
            TonalidadesFig[20] = Content.Load<Texture2D>("TonalidadeFa5");
            TonalidadesFig[21] = Content.Load<Texture2D>("TonalidadeFa6");
            TonalidadesFig[22] = Content.Load<Texture2D>("TonalidadeFa7");
            TonalidadesFig[23] = Content.Load<Texture2D>("TonalidadeFa08");
            TonalidadesFig[24] = Content.Load<Texture2D>("TonalidadeFa09");
            TonalidadesFig[25] = Content.Load<Texture2D>("TonalidadeFa10");
            TonalidadesFig[26] = Content.Load<Texture2D>("TonalidadeFa11");
            TonalidadesFig[27] = Content.Load<Texture2D>("TonalidadeFa12");
            TonalidadesFig[28] = Content.Load<Texture2D>("TonalidadeFa13");
            TonalidadesFig[29] = Content.Load<Texture2D>("TonalidadeFa14");
            Menu = new Texture2D[8];
            TDAH = Content.Load<Texture2D>("tdah");
            MM = Content.Load<Texture2D>("mm-peanut");
            HomemPalito[0] = Content.Load<Texture2D>("palito");
            HomemPalito[1] = Content.Load<Texture2D>("Palito2");
            HomemPalito[2] = Content.Load<Texture2D>("violino");
            DiamanteNegro = Content.Load<Texture2D>("diamantenegro");
            Pringles = Content.Load<Texture2D>("pringles");
            AcordeMoldura = Content.Load<Texture2D>("Acorde");
            Ferer = Content.Load<Texture2D>("ferer");
            RedBull = Content.Load<Texture2D>("RedBull");
            Picanha = Content.Load<Texture2D>("picanha");
            Shrek = Content.Load<Texture2D>("shrek");
            Pizza[0] = Content.Load<Texture2D>("pizza");
            Pizza[1] = Content.Load<Texture2D>("pizza1");
            Pizza[2] = Content.Load<Texture2D>("pizza2");
            Pizza[3] = Content.Load<Texture2D>("pizza3");
            Pizza[4] = Content.Load<Texture2D>("pizza4");            
            Trofeu = Content.Load<Texture2D>("trofeu");
            Faca = Content.Load<Texture2D>("faca");
            Menu[0] = Content.Load<Texture2D>("Menu");
            Menu[1] = Content.Load<Texture2D>("Menu1");
            Menu[2] = Content.Load<Texture2D>("Menu2");
            Menu[3] = Content.Load<Texture2D>("Menu3");
            Menu[4] = Content.Load<Texture2D>("Menu4");
            Menu[5] = Content.Load<Texture2D>("Menu5");
            Menu[6] = Content.Load<Texture2D>("Menu6");
            Menu[7] = Content.Load<Texture2D>("Menu7");
            MenuInstrucoes = Content.Load<Texture2D>("MenuInstrucoes");
            MenuInstrucoes2 = Content.Load<Texture2D>("MenuInstrucoes2");
            Zombie = Content.Load<Texture2D>("Zombie");
            Scream = Content.Load<SoundEffect>("Scream");
            Burro = Content.Load<SoundEffect>("burro");
            Burp = Content.Load<SoundEffect>("BURP");
            Morte = Content.Load<SoundEffect>("morte");
            Mozart = Content.Load<SoundEffect>("mozart");
            Grito = Content.Load<SoundEffect>("aaa");
            Getheart = Content.Load<SoundEffect>("getheart");
            DrMario = Content.Load<SoundEffect>("drmario"); 
            LinkHit = Content.Load<SoundEffect>("Linkgethit");
            LinkWrong = Content.Load<SoundEffect>("LinkWrong");
            Zelda = Content.Load<SoundEffect>("zelda");            
            Beep = Content.Load<SoundEffect>("beep2");
            pacmanintro = Content.Load<SoundEffect>("PACMANintro");
            TicTac = Content.Load<SoundEffect>("tictac");
            PringlesSnd = Content.Load<SoundEffect>("pringlesSnd");
            Chomp = Content.Load<SoundEffect>("chomp");
            VirarPagina = Content.Load<SoundEffect>("trocarEscala");
            AberturaBambu = Content.Load<SoundEffect>("AberturaBambu");
            RespostaCerta = Content.Load<SoundEffect>("RespostaCerta");
            Suspense = Content.Load<SoundEffect>("Suspense");
            TromboneFail = Content.Load<SoundEffect>("TromboneFail");
            Tiro = Content.Load<SoundEffect>("tiro");
            DoomMusic = Content.Load<SoundEffect>("e1m1");
            Coral = Content.Load<SoundEffect>("coral");
            Conga = Content.Load<SoundEffect>("conga");            
            Mario = Content.Load<SoundEffect>("Mario");
            BonusMoldura = Content.Load<Texture2D>("BonusMoldura");
            CifraSprite[0] = Content.Load<Texture2D>("Cifra5");
            CifraSprite[1] = Content.Load<Texture2D>("Cifra#");
            CifraSprite[2] = Content.Load<Texture2D>("Cifra06");
            CifraSprite[3] = Content.Load<Texture2D>("Cifra07");
            CifraSprite[4] = Content.Load<Texture2D>("Cifra07#");
            CifraSprite[5] = Content.Load<Texture2D>("Cifra65");
            CifraSprite[6] = Content.Load<Texture2D>("Cifra6#");
            CifraSprite[7] = Content.Load<Texture2D>("Cifra642");
            CifraSprite[8] = Content.Load<Texture2D>("Cifra6#4#2");
            CifraSprite[9] = Content.Load<Texture2D>("Cifra65b");
            CifraSprite[10] = Content.Load<Texture2D>("Cifra64");
            CifraSprite[11] = Content.Load<Texture2D>("Cifra643");
            CifraSprite[12] = Content.Load<Texture2D>("Cifra6#43");
            CifraSprite[13] = Content.Load<Texture2D>("Cifra5b");
            CifraSprite[14] = Content.Load<Texture2D>("Cifra6#5b");
            CifraSprite[15] = Content.Load<Texture2D>("Cifra64#2");
            CifraSprite[16] = Content.Load<Texture2D>("Cifra07b");
            CifraSprite[17] = Content.Load<Texture2D>("Cifrab");
            CifraSprite[18] = Content.Load<Texture2D>("Cifra64#3");
            noteSounds[0] = Content.Load<SoundEffect>("0_2");
            noteSounds[1] = Content.Load<SoundEffect>("1_2");
            noteSounds[2] = Content.Load<SoundEffect>("2_2");
            noteSounds[3] = Content.Load<SoundEffect>("3_2");
            noteSounds[4] = Content.Load<SoundEffect>("4_2");
            noteSounds[5] = Content.Load<SoundEffect>("5_2");
            noteSounds[6] = Content.Load<SoundEffect>("6_2");
            noteSounds[7] = Content.Load<SoundEffect>("7_2");
            noteSounds[8] = Content.Load<SoundEffect>("8_2");
            noteSounds[9] = Content.Load<SoundEffect>("9_2");
            noteSounds[10] = Content.Load<SoundEffect>("10_2");
            noteSounds[11] = Content.Load<SoundEffect>("11_2");
            noteSounds[12] = Content.Load<SoundEffect>("12_2");
            noteSounds[13] = Content.Load<SoundEffect>("13_2");
            noteSounds[14] = Content.Load<SoundEffect>("14_2");
            noteSounds[15] = Content.Load<SoundEffect>("15");
            noteSounds[16] = Content.Load<SoundEffect>("16");
            noteSounds[17] = Content.Load<SoundEffect>("17");
            noteSounds[18] = Content.Load<SoundEffect>("18");
            noteSounds[19] = Content.Load<SoundEffect>("19");
            noteSounds[20] = Content.Load<SoundEffect>("20");
            noteSounds[21] = Content.Load<SoundEffect>("21");
            noteSounds[22] = Content.Load<SoundEffect>("22");
            noteSounds[23] = Content.Load<SoundEffect>("23");
            noteSounds[24] = Content.Load<SoundEffect>("24");
            noteSounds[25] = Content.Load<SoundEffect>("25");
            noteSounds[26] = Content.Load<SoundEffect>("26");
            noteSounds[27] = Content.Load<SoundEffect>("27");
            noteSounds[28] = Content.Load<SoundEffect>("28");             
            Tetris = Content.Load<SoundEffect>("Tetris");
            TetrisOriginal = Content.Load<SoundEffect>("tetris_original");
            TetrisLyrics = Content.Load<SoundEffect>("MegaMan");
            Beat = Content.Load<SoundEffect>("Hip");
            OneUp = Content.Load<SoundEffect>("1up");
            font = Content.Load<SpriteFont>("SpriteFont1");
            fontEscalator = Content.Load<SpriteFont>("Escalator");
            fontEscalatorSmall = Content.Load<SpriteFont>("SpriteFont6");
            Menufont = Content.Load<SpriteFont>("SpriteFont2");
            fontBig = Content.Load<SpriteFont>("SpriteFont3");
            fontSmall = Content.Load<SpriteFont>("SpriteFont4");
            Coca = Content.Load<Texture2D>("cocacola");

            for (int i = 0; i < 11; i++) BG[i] = new Sprite();
            BG[0].LoadContent(this.Content, "SMB1_1");
            BG[0].Position = new Vector2(0, 25);
            BG[1].LoadContent(this.Content, "SMB1_2");
            BG[1].Position = new Vector2(BG[0].Position.X + BG[0].Size.Width, 25);
            BG[2].LoadContent(this.Content, "SMB1_3");
            BG[2].Position = new Vector2(BG[1].Position.X + BG[1].Size.Width, 25);
            BG[3].LoadContent(this.Content, "SMB1_4");
            BG[3].Position = new Vector2(BG[2].Position.X + BG[2].Size.Width, 25);
            BG[4].LoadContent(this.Content, "SMB1_5");
            BG[4].Position = new Vector2(BG[3].Position.X + BG[3].Size.Width, 25);
            BG[5].LoadContent(this.Content, "SMB1_6");
            BG[5].Position = new Vector2(BG[4].Position.X + BG[4].Size.Width, 25);
            BG[6].LoadContent(this.Content, "SMB1_7");
            BG[6].Position = new Vector2(BG[5].Position.X + BG[5].Size.Width, 25);
            BG[7].LoadContent(this.Content, "SMB1_8");
            BG[7].Position = new Vector2(BG[6].Position.X + BG[6].Size.Width, 25);
            BG[8].LoadContent(this.Content, "SMB1_9");
            BG[8].Position = new Vector2(BG[7].Position.X + BG[7].Size.Width, 25);
            BG[9].LoadContent(this.Content, "SMB1_10");
            BG[9].Position = new Vector2(BG[8].Position.X + BG[8].Size.Width, 25);
            BG[10].LoadContent(this.Content, "SMB1_11");
            BG[10].Position = new Vector2(BG[9].Position.X + BG[9].Size.Width, 25);

            noteSprite[0] = Content.Load<Texture2D>("C1");
            noteSprite[1] = Content.Load<Texture2D>("D1");
            noteSprite[2] = Content.Load<Texture2D>("E1");
            noteSprite[3] = Content.Load<Texture2D>("F1");
            noteSprite[4] = Content.Load<Texture2D>("G1");
            noteSprite[5] = Content.Load<Texture2D>("A1");
            noteSprite[6] = Content.Load<Texture2D>("B1");
            noteSprite[7] = Content.Load<Texture2D>("C2");
            noteSprite[8] = Content.Load<Texture2D>("D2");
            noteSprite[9] = Content.Load<Texture2D>("E2");
            noteSprite[10] = Content.Load<Texture2D>("F2");
            noteSprite[11] = Content.Load<Texture2D>("G2");
            noteSprite[12] = Content.Load<Texture2D>("A2");
            noteSprite[13] = Content.Load<Texture2D>("B2");
            noteSprite[14] = Content.Load<Texture2D>("C3");
            swishSound = Content.Load<SoundEffect>("swish");
            crashSound = Content.Load<SoundEffect>("crash");            
            piano800 = Content.Load<Texture2D>("Piano800");
            piano800metade = Content.Load<Texture2D>("Piano800metade");
            PianoFrame = Content.Load<Texture2D>("PianoFrame");

            soundEngine = Beat.CreateInstance();
            soundEngine.Volume = .5f;
            soundEngine.IsLooped = true;

            soundEngineSobre = TetrisLyrics.CreateInstance();
            soundEngineSobre.Volume = .5f;            

            soundEngineMario = Mozart.CreateInstance();
            soundEngineMario.Volume = .7f;

            soundEngineGretchen = Conga.CreateInstance();
            soundEngineGretchen.Volume = .8f;

            soundEnginePac = pacmanintro.CreateInstance();
            soundEnginePac.Volume = .8f;

            soundEngineScream = Scream.CreateInstance();
            soundEngineScream.Volume = .5f;

            soundEngineGrito = Grito.CreateInstance();
            soundEngineGrito.Volume = .7f;
            soundEngineGrito.IsLooped = false;

            soundEngineCoral= Coral.CreateInstance();
            soundEngineCoral.Volume = 1f;

            soundEngineMorte = Morte.CreateInstance();
            soundEngineMorte.Volume = 1f;

                        
            
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {

            PrintStatus();
            MouseState mouseState3 = Mouse.GetState();
            Window.Title = "mouseState = " + mouseState3.X.ToString() + ", " + mouseState3.Y.ToString();

            Window.Title += " Clave = " + Clave.ToString() + " Clave Original = " + ClaveOriginal.ToString();

            if (FINAL == 1)
            {
                SalvaRecordes();
            }

            KeyboardState keyState = Keyboard.GetState();

            if (MENU == 0 && !GAMEBOY)
            {
                if (keyState.IsKeyDown(Keys.Escape)) Morrer();

            }

            if (GAMEBOY)
            {
                MouseState mouseState = Mouse.GetState();

                if (GBTYPE == 0)
                {
                    this.IsMouseVisible = true;
                    Rectangle MouseRect = new Rectangle(mouseState.X, mouseState.Y, 4, 4);
                    Rectangle BotaoA = new Rectangle(561, 375, 48, 44);
                    Rectangle BotaoB = new Rectangle(511, 400, 48, 44);
                    Rectangle BotaoStart = new Rectangle(454, 476, 45, 30);

                    if (ABSTART)
                    {

                        if (MouseRect.Intersects(BotaoA) && mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released && (!BOTAOA) && (!BOTAOB) && (!BOTAOSTART))
                        {
                            BOTAOA = true;
                        }
                        if (MouseRect.Intersects(BotaoB) && mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released && (BOTAOA) && (!BOTAOB) && (!BOTAOSTART))
                        {
                            BOTAOB = true;
                        }
                        if (MouseRect.Intersects(BotaoStart) && mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released && (BOTAOA) && (BOTAOB))
                        {
                            BOTAOA = false;
                            BOTAOB = false;
                            ABSTART = false;
                            Pontos -= (int)PontosPerdidos;
                            PontosPerdidos = 0;
                            GBTimer = 0;
                            FacaCount = 0;
                            FacaPos.X = 570;
                            FacaPos.Y = 260;

                        }

                        if (Pontos - (int)PontosPerdidos > 0) PontosPerdidos += 0.65f;
                    }
                    else if (!ABSTART)
                    {
                        GBTimer += 1;
                        keyState = Keyboard.GetState();

                        if (GBTimer > 400 && !PALITOMORREU)
                        {
                            FacaTimer += 1 + (GBTimer / 350);

                            if (FacaDir == 0 && FacaTimer > 30)
                            {
                                if (GBTimer < 1400) FacaPos.X -= 3;
                                else if (GBTimer > 3000) FacaPos.X -= 4;
                                else if (GBTimer > 5000) FacaPos.X -= 5;
                                else FacaPos.X -= 3;
                            }
                            else if (FacaDir == 1 && FacaTimer > 30)
                            {
                                if (GBTimer < 1400) FacaPos.X += 3;
                                else if (GBTimer > 3000) FacaPos.X += 4;
                                else if (GBTimer > 5000) FacaPos.X += 5;
                                else FacaPos.X += 3;
                            }
                            else if (FacaDir == 2 && FacaTimer > 30)
                            {
                                if (GBTimer < 1400) FacaPos.Y += 3;
                                else if (GBTimer > 3000) FacaPos.Y += 4;
                                else if (GBTimer > 5000) FacaPos.Y += 5;
                                else FacaPos.Y += 3;

                                if (FacaDirX == 0) FacaPos.X += 1;
                                else if (FacaDirX == 1) FacaPos.X -= 1;
                            }

                            if (FacaMovY == 1) FacaPos.Y += 1;
                            else if (FacaMovY == 2) FacaPos.Y -= 1;


                            int[] FacaY = new int[] { 190, 197, 200, 205, 210, 214, 218, 220, 226, 229, 230, 233, 237, 240, 245, 250, 255, 257, 258, 260 };

                            if (FacaDir == 0 && FacaPos.X < 330)
                            {
                                FacaMovY = 0;
                                FacaDir = Rand.Next(3);
                                FacaCount += 1;

                                if (FacaDir == 0)
                                {
                                    FacaPos.X = 570;
                                    FacaPos.Y = FacaY[Rand.Next(FacaY.Length)];
                                    AjustaFacaY();
                                }
                                else if (FacaDir == 1)
                                {
                                    FacaPos.X = 335;
                                    FacaPos.Y = FacaY[Rand.Next(FacaY.Length)];
                                    AjustaFacaY();
                                }
                                else if (FacaDir == 2)
                                {
                                    FacaPos.Y = 65;
                                    FacaPos.X = Rand.Next(170) + 384;
                                    if (FacaPos.X > 468) FacaDirX = 1;
                                    else FacaDirX = 0;
                                }
                                FacaTimer = 0;

                            }
                            if (FacaDir == 1 && FacaPos.X > 600)
                            {
                                FacaMovY = 0;
                                FacaDir = Rand.Next(3);
                                FacaCount += 1;

                                if (FacaDir == 0)
                                {
                                    FacaPos.X = 570;
                                    FacaPos.Y = FacaY[Rand.Next(FacaY.Length)];
                                    AjustaFacaY();

                                }
                                else if (FacaDir == 1)
                                {
                                    FacaPos.X = 335;
                                    FacaPos.Y = FacaY[Rand.Next(FacaY.Length)];
                                    AjustaFacaY();
                                }
                                else if (FacaDir == 2)
                                {
                                    FacaPos.Y = 60;
                                    FacaPos.X = Rand.Next(170) + 384;
                                    if (FacaPos.X > 468) FacaDirX = 1;
                                    else FacaDirX = 0;

                                }
                                FacaTimer = 0;

                            }
                            else if (FacaDir == 2 && FacaPos.Y > 335)
                            {
                                FacaMovY = 0;
                                FacaDir = Rand.Next(3);
                                FacaCount += 1;

                                if (FacaDir == 0)
                                {
                                    FacaPos.X = 570;
                                    FacaPos.Y = FacaY[Rand.Next(FacaY.Length)];
                                    AjustaFacaY();
                                }
                                else if (FacaDir == 1)
                                {
                                    FacaPos.X = 335;
                                    FacaPos.Y = FacaY[Rand.Next(FacaY.Length)];
                                    AjustaFacaY();
                                }
                                else if (FacaDir == 2)
                                {
                                    FacaPos.Y = 50;
                                    FacaPos.X = Rand.Next(170) + 384;
                                    if (FacaPos.X > 468) FacaDirX = 1;
                                    else FacaDirX = 0;
                                }
                                FacaTimer = 0;

                            }

                            if (keyState.IsKeyDown(Keys.Left) && !(PalitoPos.X < 384))
                            {
                                PalitoPos.X -= 2;
                                PalitoState = SpriteEffects.FlipHorizontally;
                            }
                            if (keyState.IsKeyDown(Keys.Right) && !(PalitoPos.X > 521))
                            {
                                PalitoPos.X += 2;
                                PalitoState = SpriteEffects.None;
                            }
                            if (mouseState.LeftButton == ButtonState.Pressed)
                            {
                                SoltarBotao = false;
                                PalitoPular();
                            }

                            if (Pulo && mouseState.LeftButton == ButtonState.Released)
                            {
                                if (!SoltarBotao)
                                {
                                    SoltarBotao = true;
                                    Queda = true;

                                }
                                PalitoPos.Y -= VelocidadePulo;

                                if (PalitoPos.Y >= 220)
                                {
                                    PalitoPos.Y = 220;
                                    SoltarBotao = false;
                                    Pulo = false;
                                    Queda = false;
                                }
                            }

                            //loop

                        }

                    }


                }
                else if (GBTYPE == 1) // bowser
                {
                    this.IsMouseVisible = true;
                    BowserPos.X -= 3;

                    if (BowserDirecao == 0) BowserRot += 0.03f;
                    else if (BowserDirecao == 1) BowserRot -= 0.03f;

                    if (BowserRot > 0.25f) BowserDirecao = 1;
                    if (BowserRot < -0.25f) BowserDirecao = 0;

                    if (BowserPos.X < -300 && ShowBowser == true)
                    {
                        Vidas -= 1;
                        ShowBowser = false;
                        BowserNotasBool = true;
                        crashSound.Play();
                    }

                    if (BowserEnergia == 0 && ShowBowser == true)
                    {
                        Vidas += 1;
                        ShowBowser = false;
                        OneUp.Play();
                        BowserNotasBool = true;
                    }

                    if (ShowBowser == true)
                    {
                        if (Pontos > 0) Pontos -= 1;
                    }

                    Rectangle MouseRect = new Rectangle(mouseState.X, mouseState.Y, 4, 4);
                    Rectangle BowserRect = new Rectangle((int)(BowserPos.X), (int)(BowserPos.Y),
                    (int)(Bowser.Width * 0.25F), (int)(Bowser.Height * 0.25F) + 20);

                    if (mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released && MouseRect.Intersects(BowserRect) && ShowBowser == true)
                    {
                        BowserEnergia -= 1;
                        Pontos += 15;

                    }

                    if (BowserNotasBool == true)
                    {
                        if (Pontos - (int)PontosPerdidos > 0) PontosPerdidos += 0.20f;
                        if (PontosPerdidos >= 1)
                        {
                            Pontos -= 1;
                            PontosPerdidos = 0;
                        }

                        MouseRect = new Rectangle(mouseState.X, mouseState.Y, 4, 4);
                        Rectangle[] PianoKeys = new Rectangle[15];
                        PianoKeys[0] = new Rectangle(6 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                        PianoKeys[1] = new Rectangle(58 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                        PianoKeys[2] = new Rectangle(110 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                        PianoKeys[3] = new Rectangle(163 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                        PianoKeys[4] = new Rectangle(217 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                        PianoKeys[5] = new Rectangle(269 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                        PianoKeys[6] = new Rectangle(323 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                        PianoKeys[7] = new Rectangle(375 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                        PianoKeys[8] = new Rectangle(428 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                        PianoKeys[9] = new Rectangle(481 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                        PianoKeys[10] = new Rectangle(536 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                        PianoKeys[11] = new Rectangle(588 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                        PianoKeys[12] = new Rectangle(641 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                        PianoKeys[13] = new Rectangle(694 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                        PianoKeys[14] = new Rectangle(746 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);

                        CurrentKey = -1;
                        for (int i = 0; i < 15; i++)
                        {
                            if (MouseRect.Intersects(PianoKeys[i]))
                            {
                                CurrentKey = i;
                                break;
                            }
                        }


                        //check MIDI key
                        CurrentKeyMIDI = -1;
                        List<Pitch> pitches = new List<Pitch>(pitchesPressed.Keys);
                        pitches.Sort();
                        for (int i = 0; i < pitches.Count; ++i)
                        {
                            Pitch pitch = pitches[i];

                            if (Clave == 0)
                            {
                                if (pitch.Octave() == 2) CurrentKeyMIDI = PitchTrans[pitch.PositionInOctave()];
                                if (pitch.Octave() == 3) CurrentKeyMIDI = PitchTrans2[pitch.PositionInOctave()];
                                else if (pitch == Pitch.C4) CurrentKeyMIDI = 14;
                            }
                            else if (Clave == 1 || Clave == 9)
                            {
                                if (pitch.Octave() == 4) CurrentKeyMIDI = PitchTrans[pitch.PositionInOctave()];
                                if (pitch.Octave() == 5) CurrentKeyMIDI = PitchTrans2[pitch.PositionInOctave()];
                                else if (pitch == Pitch.C6) CurrentKeyMIDI = 14;
                            }
                            else if (Clave == 2 || Clave == 11) // CLave de Dó
                            {
                                if (pitch.Octave() == 3) CurrentKeyMIDI = PitchTrans[pitch.PositionInOctave()];
                                if (pitch.Octave() == 4) CurrentKeyMIDI = PitchTrans2[pitch.PositionInOctave()];
                                else if (pitch == Pitch.C5) CurrentKeyMIDI = 14;
                            }
                            else if (Clave == 3 || Clave == 4)
                            {
                                if (pitch.Octave() == 2) CurrentKeyMIDI = PitchTrans[pitch.PositionInOctave()];
                                if (pitch.Octave() == 3) CurrentKeyMIDI = PitchTrans2[pitch.PositionInOctave()];
                                if (pitch.Octave() == 4) CurrentKeyMIDI = PitchTrans3[pitch.PositionInOctave()];
                                if (pitch.Octave() == 5) CurrentKeyMIDI = PitchTrans4[pitch.PositionInOctave()];
                                else if (pitch == Pitch.C6) CurrentKeyMIDI = 28;
                            }
                            else if (Clave == 10) // To DO: Provavelmente corrigir oitava da Clave 10
                            {
                                if (pitch.Octave() == 2) CurrentKeyMIDI = PitchTrans[pitch.PositionInOctave()];
                                if (pitch.Octave() == 3) CurrentKeyMIDI = PitchTrans2[pitch.PositionInOctave()];
                                else if (pitch == Pitch.C4) CurrentKeyMIDI = 14;
                            }

                        }
                        //Window.Title = "CurrentKeyMIDI = " + CurrentKeyMIDI.ToString();

                        if ((mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released && CurrentKey == BowserNotas[BowserNotaAtual])
                            || (CurrentKeyMIDI == BowserNotas[BowserNotaAtual])
                            )
                        {
                            BowserNotasStatus[BowserNotaAtual] = false;
                            BowserNotaAtual += 1;
                        }

                        if (BowserNotaAtual == 7)
                        {
                            GAMEBOY = false;
                            Pontos -= (int)PontosPerdidos;
                            PontosPerdidos = 0;
                            BowserNotasStatus[0] = false;
                            BowserNotasStatus[1] = false;
                            BowserNotasStatus[2] = false;
                            BowserNotasStatus[3] = false;
                            BowserNotasStatus[4] = false;
                            BowserNotasStatus[5] = false;
                            BowserNotasStatus[6] = false;
                            BowserNotaAtual = 0;
                            soundEngineMario.Stop();
                            soundEngine.Play();
                            soundEngine.Volume = 0.5f;
                            if (MINIGAME) soundEngine.Volume = 0.8f;
                            halt = 40;
                        }

                    }

                }
                else if (GBTYPE == 2) // ESCALATOR !!
                {
                    this.IsMouseVisible = false;
                    Rectangle ClipArea = new Rectangle(0, 0, 800 - (int)(Mira.Width * 0.6F), 600 - (int)(Mira.Height * 0.6F));
                    if (graphics.IsFullScreen == true) ClipCursor(ref ClipArea);

                    if (EscalatorMorreu && soundEngineMario.State == SoundState.Stopped)
                    {
                        GAMEBOY = false;
                        soundEngineMario.Stop();
                        soundEngine.Play();
                        soundEngine.Volume = 0.5f;
                        if (MINIGAME) soundEngine.Volume = 0.8f;
                        this.IsMouseVisible = true;
                        ClipArea = new Rectangle(0, 0, 800, 600);
                        if (graphics.IsFullScreen == true) ClipCursor(ref ClipArea);
                        GBTimer = 0;
                        Pontos += PontosEscalator;
                        halt = 60;
                        EscalatorMorreu = false;

                        if (PontosEscalator > EscalatorRecorde)
                        {
                            EscalatorRecorde = PontosEscalator;
                            SalvaRecordes();
                        }

                        PontosEscalator = 0;
                    }

                    Rectangle MiraRect = new Rectangle((int)MiraPos.X + 25, (int)MiraPos.Y + 25, 58, 58);

                    if (!EscalatorMorreu)
                    {
                        MiraPos.X = mouseState.X;
                        MiraPos.Y = mouseState.Y;
                        TiroTimer += 1;
                        GBTimer += 1;

                        if (GBTimer >= 28000)
                        {
                            GBTimer = 1150;
                        }
                        
                        if (GBTimer % (400 - GBTimer / 70) == 0  && GBTimer > 400)
                        {
                            NotasEscalator.Add(new EscalatorNota(new Vector2(-Rand.Next(500), 100), Notas[Rand.Next(Notas.Length)]));
                            NotasEscalator.Add(new EscalatorNota(new Vector2(-Rand.Next(500), 215), Notas[Rand.Next(Notas.Length)]));
                            NotasEscalator.Add(new EscalatorNota(new Vector2(-Rand.Next(500), 330), Notas[Rand.Next(Notas.Length)]));
                            NotasEscalator.Add(new EscalatorNota(new Vector2(-Rand.Next(500), 445), Notas[Rand.Next(Notas.Length)]));
                        }

                        if (GBTimer % 1750 == 0) TrocarEscala();

                        foreach (EscalatorNota nota in NotasEscalator)
                        {
                            nota.IncX((float)Rand.NextDouble() * 4f);

                            // TO DO: tesar isso
                            foreach (EscalatorNota n in NotasEscalator)
                            {
                                if (nota.Rect != n.Rect)
                                {
                                    Rectangle Rect2 = new Rectangle((int)n.Pos.X, (int)n.Pos.Y, 130, 100);
                                    if (Rect2.Intersects(nota.Rect) && n.Pos.X < nota.Pos.X)
                                    {
                                        while (!Rect2.Intersects(nota.Rect))
                                        {
                                            n.IncX(-30);
                                        }
                                    }
                                }
                            }


                            if (nota.ForaDaTela() && !EscalatorMorreu)
                            {
                                if (EscalaTipo == 0)
                                {
                                    foreach (string n in EscalasMaiores[EscalaNota])
                                    {
                                        if (n == nota.Nota) goto sair;
                                    }
                                }
                                else if (EscalaTipo == 1)
                                {
                                    foreach (string n in EscalasMenoresNatural[EscalaNota])
                                    {
                                        if (n == nota.Nota) goto sair;
                                    }
                                }
                                else if (EscalaTipo == 2)
                                {
                                    foreach (string n in EscalasMenoresHarmonica[EscalaNota])
                                    {
                                        if (n == nota.Nota) goto sair;
                                    }
                                }
                                else if (EscalaTipo == 3)
                                {
                                    foreach (string n in EscalasMenoresMelodica[EscalaNota])
                                    {
                                        if (n == nota.Nota) goto sair;
                                    }
                                }

                                soundEngineMario.Stop();
                                soundEngineMario = TromboneFail.CreateInstance();
                                soundEngineMario.Play();
                                EscalatorMorreu = true;
                                nota.LastStanding = true;
                                
                                
                            }
                            sair: ;

                            if ( !EscalatorMorreu && (MiraRect.Intersects(nota.Rect) && mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released && TiroTimer > 30))
                            {
                                nota.Remover = true;

                                if (EscalaTipo == 0)
                                {
                                    foreach (string n in EscalasMaiores[EscalaNota])
                                    {
                                        if (n == nota.Nota) nota.Remover = false;
                                    }
                                }
                                else if (EscalaTipo == 1)
                                {
                                    foreach (string n in EscalasMenoresNatural[EscalaNota])
                                    {
                                        if (n == nota.Nota) nota.Remover = false;
                                    }
                                }
                                else if (EscalaTipo == 2)
                                {
                                    foreach (string n in EscalasMenoresHarmonica[EscalaNota])
                                    {
                                        if (n == nota.Nota) nota.Remover = false;
                                    }
                                }
                                else if (EscalaTipo == 3)
                                {
                                    foreach (string n in EscalasMenoresMelodica[EscalaNota])
                                    {
                                        if (n == nota.Nota) nota.Remover = false;
                                    }
                                }

                                if (nota.Remover == false)
                                {
                                    soundEngineMario.Stop();
                                    soundEngineMario = TromboneFail.CreateInstance();
                                    soundEngineMario.Play();
                                    EscalatorMorreu = true;
                                    nota.LastStanding = true;
                                }
                                else if (nota.Remover == true)
                                {
                                    PontosEscalator += 5;
                                }

                                goto getout;
                            }
                        }

                    getout: ;

                        
                        if (mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released && TiroTimer > 30 && GBTimer > 350)
                        {
                            Tiro.Play();
                            TiroTimer = 0;

                            //if (GBTimer > 40 && GBTimer < 500) GBTimer = 500; // pular intro
                        }

                        // limpar lista
                        EscalatorNota item;
                        for (int index = NotasEscalator.Count - 1; index >= 0; index--)
                        {
                            item = NotasEscalator[index];
                            if (item.Remover == true) NotasEscalator.RemoveAt(index);
                        }

                        if (EscalatorMorreu)
                        {
                            for (int index = NotasEscalator.Count - 1; index >= 0; index--)
                            {
                                item = NotasEscalator[index];
                                if (item.LastStanding == false) NotasEscalator.RemoveAt(index);
                                else {
                                     item .Pos.X = 555;
                                    item.Pos.Y = 160;
                                }
                            }
                        }


                    } //escalator morreu

                }
                else if (GBTYPE == 3)
                {

                    this.IsMouseVisible = true;
                    Rectangle MouseRect = new Rectangle(mouseState.X, mouseState.Y, 4, 4);

                    if (TomStatus == 0)
                    {
                        TomAtual = Rand.Next(TonalidadesFig.Length - 4);

                        if (TomAtual == 8 || TomAtual == 9 || TomAtual == 13 || TomAtual == 14 || TomAtual == 21 || TomAtual == 22) TomAtual += 1 + Rand.Next(3);

                        TomTipo = Rand.Next(2);

                        if (TomTipo == 0)
                        {
                            if (TomAtual > 7 && TomAtual <= 14 || TomAtual >= 23 && TomAtual <= 29)
                            {
                                Tom.Add(new NotaTonalidade(new Vector2(220, 500), "C", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(245, 415), "Db", 1));
                                Tom.Add(new NotaTonalidade(new Vector2(270, 500), "D", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(295, 415), "Eb", 1));
                                Tom.Add(new NotaTonalidade(new Vector2(320, 500), "E", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(370, 500), "F", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(395, 415), "Gb", 1));
                                Tom.Add(new NotaTonalidade(new Vector2(420, 500), "G", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(445, 415), "Ab", 1));
                                Tom.Add(new NotaTonalidade(new Vector2(470, 500), "A", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(495, 415), "Bb", 1));
                                Tom.Add(new NotaTonalidade(new Vector2(520, 500), "B", "Cb", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(570, 500), "C", 0));
                            }
                            else
                            {
                                Tom.Add(new NotaTonalidade(new Vector2(220, 500), "C", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(245, 415), "C#", 1));
                                Tom.Add(new NotaTonalidade(new Vector2(270, 500), "D", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(295, 415), "D#", 1));
                                Tom.Add(new NotaTonalidade(new Vector2(320, 500), "E", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(370, 500), "F", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(395, 415), "F#", 1));
                                Tom.Add(new NotaTonalidade(new Vector2(420, 500), "G", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(445, 415), "G#", 1));
                                Tom.Add(new NotaTonalidade(new Vector2(470, 500), "A", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(495, 415), "A#", 1));
                                Tom.Add(new NotaTonalidade(new Vector2(520, 500), "B", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(570, 500), "C", 0));
                            }
                        }
                        else if (TomTipo == 1)
                        {
                            if (TomAtual > 7 && TomAtual <= 14 || TomAtual >= 23 && TomAtual <= 29)
                            {
                                Tom.Add(new NotaTonalidade(new Vector2(220, 500), "c", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(245, 415), "db", 1));
                                Tom.Add(new NotaTonalidade(new Vector2(270, 500), "d", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(295, 415), "eb", 1));
                                Tom.Add(new NotaTonalidade(new Vector2(320, 500), "e", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(370, 500), "f", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(395, 415), "gb", 1));
                                Tom.Add(new NotaTonalidade(new Vector2(420, 500), "g", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(445, 415), "ab", 1));
                                Tom.Add(new NotaTonalidade(new Vector2(470, 500), "a", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(495, 415), "bb", 1));
                                Tom.Add(new NotaTonalidade(new Vector2(520, 500), "b", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(570, 500), "c", 0));
                            }
                            else
                            {
                                Tom.Add(new NotaTonalidade(new Vector2(220, 500), "c", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(245, 415), "c#", 1));
                                Tom.Add(new NotaTonalidade(new Vector2(270, 500), "d", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(295, 415), "d#", 1));
                                Tom.Add(new NotaTonalidade(new Vector2(320, 500), "e", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(370, 500), "f", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(395, 415), "f#", 1));
                                Tom.Add(new NotaTonalidade(new Vector2(420, 500), "g", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(445, 415), "g#", 1));
                                Tom.Add(new NotaTonalidade(new Vector2(470, 500), "a", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(495, 415), "a#", 1));
                                Tom.Add(new NotaTonalidade(new Vector2(520, 500), "b", 0));
                                Tom.Add(new NotaTonalidade(new Vector2(570, 500), "c", 0));
                            }
                        }

                        TomStatus = 1;
                    }
                    GBTimer += 1;


                    if (mouseState.LeftButton == ButtonState.Released)
                    {
                        TomSelecionado = false;
                    }

                    if (TomSelecionado == false)
                    {
                        TomIndex = 0;

                        foreach (NotaTonalidade tom in Tom)
                        {
                            TomIndex++;
                            Rectangle MouseRect2 = new Rectangle(mouseState.X - 15, mouseState.Y - 10, 36, 25);

                            if (MouseRect2.Intersects(tom.Rect) && mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released)
                            {
                                TomSelecionado = true;
                                goto skipTom;
                            }

                        }
                    }

                skipTom:

                    if (TomSelecionado && !AcabouForca) Tom[TomIndex-1].SetXY(mouseState.X - 15, mouseState.Y - 40);

                    if (GBTimer % 240 == 0)
                    {
                        ForcaStatus += 1;                         
                    }
                    Rectangle ClaveX = new Rectangle(530, 260, 81, 43);
                    foreach (NotaTonalidade tom in Tom)
                    {
                        if (tom.Rect.Intersects(ClaveX))
                        {
                            if (TomTipo == 0)
                            {
                                if (tom.Tom == TomMaior[TomAtual])
                                {
                                    Tom.Clear();
                                    TomStatus = 0;
                                    TomCount += 1;
                                    TomSelecionado = false;
                                    goto leaveMe;
                                }
                            }
                            else if (TomTipo == 1)
                            {
                                if (tom.Tom == TomMenor[TomAtual])
                                {
                                    Tom.Clear();
                                    TomStatus = 0;
                                    TomCount += 1;
                                    TomSelecionado = false;
                                    goto leaveMe;
                                }
                            }

                        }
                        
                    }
                    leaveMe: ;

                    if (ForcaStatus > 8 && !AcabouForca)
                    {
                        soundEngineGrito.Volume = 0.7f;
                        soundEngineGrito.Play();
                        AcabouForca = true;
                        GBTimer = 0;
                        PizzaStatus = 0;
                        halt = 50;
                        soundEngineMario.Volume = 0.7f;
                        soundEngineMario.Stop();

                        if (TomCount > TomRecorde)
                        {
                            TomRecorde = TomCount;
                            SalvaRecordes();
                        }
                        
                    }

                    if (AcabouForca)
                    {

                        if (soundEngineGrito.State == SoundState.Stopped)
                        {
                            GAMEBOY = false;
                            soundEngineGrito.Volume = 0;
                            soundEngineGrito.Stop();
                            Tom.Clear();
                            TomStatus = 0;
                            soundEngine.Play();
                            soundEngine.Volume = 0.5f;
                            if (MINIGAME) soundEngine.Volume = 0.8f;
                            AcabouForca = false;
                            Pontos += (TomCount * 40);
                            TomCount = 0;
                        }

                    }


                }
                else if (GBTYPE == 6)
                {
                    if (!PalavraIntroducao || GBTimer == 1)
                    {
                        GBTimer += 1;
                        GBTimer2 += 1;
                    }
                    if (GBTimer == 1)
                    {
                        PalavraIntroducao = true;
                        PalavrasLista.Clear();

                        string PInicial = Palavras[Rand.Next(Palavras.Length)];
                        while (PInicial.Length != 7) PInicial = Palavras[Rand.Next(Palavras.Length)];
                        PalavrasLista.Add(new Palavra(new Vector2(395, 390), PInicial, false));
                    }

                    if (GBTimer2 % 600 == 0)
                    {
                        PalavrasLista.Add(new Palavra(new Vector2(395, 0), Palavras[Rand.Next(Palavras.Length)], true));                    
                    }

                    if (PalavraIntroducao)
                    {
                        foreach (Palavra p in PalavrasLista)
                        {

                            if (keyState.IsKeyUp(ReturnKey(p.TextoAtual[0])) && prevkeyState.IsKeyDown(ReturnKey(p.TextoAtual[0])))
                            {
                                if (p.CurrentIndex < p.Texto.Length) p.CurrentIndex += 1;
                                p.TextoAtual = p.Texto.Substring(p.CurrentIndex);
                                soundEnginePalavra = noteSounds[p.CurrentIndex + p.Texto.Length].CreateInstance();
                                soundEnginePalavra.Volume = 0.15f;
                                soundEnginePalavra.Play();
                            }

                            if (p.CurrentIndex == p.Texto.Length)
                            {
                                PalavraIntroducao = false;
                                p.TrocaPalavra(Palavras[Rand.Next(Palavras.Length)], new Vector2(Rand.Next(500) + 50, -20));
                            }

                        }
                    }
                    else if (!PalavraIntroducao)
                    {
                        foreach (Palavra p in PalavrasLista)
                        {
                            if (!p.Especial) p.AtualizaPos(0.5f + ((GBTimer / 200) * 0.2f));
                            else if (p.Especial) p.AtualizaPos(3f);

                            if (p.Pos.Y > 570 && !p.Especial)
                            {
                                // todo: menos brusco isso
                                GAMEBOY = false;
                                soundEngineMario.Stop();
                                soundEngine.Play();
                                soundEngine.Volume = 0.5f;
                                if (MINIGAME) soundEngine.Volume = 0.8f;

                                if (PontosPalavra > PalavraRecorde)
                                {
                                    PalavraRecorde = PontosPalavra;
                                    SalvaRecordes();
                                }
                                Pontos += PontosPalavra;
                                halt = 50;
                            }

                            if (p.Pos.Y > 570 && p.Especial)
                            {
                                p.Remover = true;
                            }

                            try
                            {

                                if (keyState.IsKeyUp(ReturnKey(p.TextoAtual[0])) && prevkeyState.IsKeyDown(ReturnKey(p.TextoAtual[0])))
                                {
                                    if (p.CurrentIndex < p.Texto.Length) p.CurrentIndex += 1;
                                    p.TextoAtual = p.Texto.Substring(p.CurrentIndex);

                                    if (!p.Especial)
                                    {
                                        soundEnginePalavra = noteSounds[p.CurrentIndex + p.Texto.Length].CreateInstance();
                                        soundEnginePalavra.Volume = 0.15f;
                                        soundEnginePalavra.Play();
                                    }
                                }
                            }
                            catch
                            {
                            }

                            if (p.CurrentIndex == p.Texto.Length)
                            {

                                if (!p.Especial)
                                {
                                    PontosPalavra += (int)p.Texto.Length; 
                                    p.TrocaPalavra(Palavras[Rand.Next(Palavras.Length)], new Vector2(Rand.Next(500) + 50, -20));
                                }
                                if (p.Especial)
                                {
                                    p.Remover = true;
                                    GBTimer -= 400;
                                }
                           }


                        } // loop foreach

                        Palavra palav;
                        for (int index = PalavrasLista.Count - 1; index >= 0; index--)
                        {
                            palav = PalavrasLista[index];
                            if (palav.Remover == true) PalavrasLista.RemoveAt(index);
                        }                        


                    } // gbt er > 500

                }
                else if (GBTYPE == 5)
                {

                    if (!TrocaNotabase)
                    {

                        GBTimer += 1;
                        foreach (Link l in Links)
                        {
                            l.CheckInput(2);
                        }

                        if (keyState.IsKeyDown(Keys.J))
                        {
                            TrocaNotabase = true;
                            NotabaseTimer = 998;
                        }

                        if (GBTimer % (150 - GBTimer / 70) == 0 && GBTimer > 150)
                        {
                            int tipo = Rand.Next(6);
                            if (tipo == 1) tipo = 0;
                            else if (tipo == 3)
                            {
                                tipo = 6;
                            }
                            else if (tipo == 5) tipo = 4;

                            if (tipo == 4) InimigosLinkLista.Add(new InimigoLink(new Vector2(Rand.Next(300), Rand.Next(300) + 100), tipo, new Vector2(0, 0)));
                            else InimigosLinkLista.Add(new InimigoLink(new Vector2(-40, 90), tipo, new Vector2(2, 2)));
                        }
                    }
                    else if (TrocaNotabase)
                    {
                        if (NotabaseTimer != 875 && NotabaseTimer != 1001) NotabaseTimer -= 1;

                        if (NotabaseTimer == 1001)
                        {
                            if (keyState.IsKeyDown(Keys.K)) NotabaseTimer = 1000;                            
                        }
                        else if (NotabaseTimer == 999)
                        {
                            NOTABASE = Rand.Next(13);

                            NotabaseChegada = Rand.Next(7) + 3;
                            //while (NotabaseChegada < NOTABASE || NOTABASE == NotabaseChegada || NotabaseChegada > (NOTABASE+7) ) NotabaseChegada = Rand.Next(15); 
                            while (NOTABASE == NotabaseChegada) NotabaseChegada = Rand.Next(7) + 3;

                        }
                        else if (NotabaseTimer == 997)
                        {
                            soundEngineMario.Volume = 0;
                            noteSounds[NOTABASE + 14].Play();
                        }
                        else if (NotabaseTimer == 900)
                        {
                            noteSounds[NotabaseChegada + 14].Play();
                        }
                        else if (NotabaseTimer == 875)
                        {
                            if (keyState.IsKeyDown(Keys.J)) NotabaseTimer = 998;
                            if (keyState.IsKeyDown(Keys.K))
                            {
                                TrocaNotabase = false;
                                GBTimer += 1;
                                soundEngineMario.Volume = 0.9f;
                            }

                        }
                    }

                }
                else if (GBTYPE == 10 || GBTYPE == 11 || GBTYPE == 12)  // AVISO DISTRACOES
                {

                    if (mouseState.LeftButton == ButtonState.Pressed && mouseState.RightButton == ButtonState.Pressed || keyState.IsKeyDown(Keys.Z) )
                    {
                        GAMEBOY = false;
                        GBTimer = 0;
                        if (GBTYPE == 10) ShowMM = true;
                        else if (GBTYPE == 11) ShowFerer = true;
                        halt = 80;
                        if (GBTYPE == 12) halt += 80;
                        PrimeiraDistracao = false;
                    }
                }
                else if (GBTYPE == 4) // UNIVERSITARIO
                {
                    GBTimer += 1;
                    BotaoAtual = -1;

                    if (PerguntaTimer > 175 && RespostaErrada)
                    {
                        PerguntaTimer = 0;
                        GAMEBOY = false;
                        RespostaErrada = false;
                        GBTimer = 0;
                        halt = 50;
                        soundEngineMario.Volume = 0.7f;
                        soundEngineMario.Stop();
                        soundEngine.Play();
                        soundEngine.Volume = 0.5f;
                        if (MINIGAME) soundEngine.Volume = 0.8f;
                        PontosBambu = 0;
                    }

                    if (GBTimer >= 650) PerguntaTimer += 1;

                    if (soundEngineMario.State == SoundState.Stopped && GBTimer > 400 && !RespostaErrada)
                    {
                        soundEngineMario = Suspense.CreateInstance();
                        soundEngineMario.Volume = .8f;
                        soundEngineMario.IsLooped = true;
                        soundEngineMario.Play();
                    }

                    MouseState mouseState4 = Mouse.GetState();
                    Window.Title += " mouseState = " + mouseState4.X.ToString() + ", " + mouseState4.Y.ToString();
                    Rectangle MouseRect = new Rectangle(mouseState4.X, mouseState4.Y, 4, 4);
                    Rectangle[] Botao = new Rectangle[4];

                    Botao[0] = new Rectangle(135, 345, 245, 45);
                    Botao[1] = new Rectangle(415, 345, 245, 45);
                    Botao[2] = new Rectangle(135, 404, 245, 45);
                    Botao[3] = new Rectangle(415, 404, 245, 45);

                    for (int i = 0; i < 4; i++)
                    {
                        if (MouseRect.Intersects(Botao[i]))
                        {
                            if (!RespostaErrada) BotaoAtual = i;
                            break;
                        }
                    }

                    if (MouseRect.Intersects(Botao[(PerguntaAtual + Rotacao) % 4]) && mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released && PerguntaTimer > 40 && !RespostaErrada)
                    {
                        TrocarPergunta();
                        PontosBambu += 20;
                        TempoSobrando += new TimeSpan(0, 0, 0, 10);

                        RespostaCerta.Play();
                        PerguntaTimer = 0;
                    }
                    else if (!RespostaErrada && mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released && (MouseRect.Intersects(Botao[0]) || MouseRect.Intersects(Botao[1]) || MouseRect.Intersects(Botao[2]) || MouseRect.Intersects(Botao[3])) && PerguntaTimer > 40 || (TempoSobrando < new TimeSpan(0, 0, 0, 0, 1)))
                    {
                        soundEngineMario.Stop();
                        Burro.Play();
                        RespostaErrada = true;
                        PerguntaTimer = 0;
                        Pontos += PontosBambu;

                        if (PontosBambu > BambuRecorde)
                        {
                            BambuRecorde = PontosBambu;
                            SalvaRecordes();
                        }
                        
                    }

                }

            }

            if (MENU == 1 && !GAMEBOY)
            {
                this.IsMouseVisible = true;

                if (soundEngine.State == SoundState.Stopped && SUBMENU < 666)
                {
                    soundEngine = TetrisOriginal.CreateInstance();
                    soundEngine.Volume = 0.8f;
                    soundEngine.IsLooped = true;
                    soundEngine.Pitch = 0;
                    soundEngine.Play();
                }

                MouseState mouseState = Mouse.GetState();
                Rectangle MouseRect = new Rectangle(mouseState.X, mouseState.Y, 4, 4);
                Rectangle[] MenuKeys = new Rectangle[7];

                MenuKeys[0] = new Rectangle(264, 199, 143, 53);
                MenuKeys[1] = new Rectangle(264, 254, 143, 53);
                MenuKeys[2] = new Rectangle(260, 311, 143, 53);
                MenuKeys[3] = new Rectangle(264, 367, 143, 53);
                MenuKeys[4] = new Rectangle(264, 423, 143, 53);
                MenuKeys[5] = new Rectangle(264, 468, 143, 63);
                MenuKeys[6] = new Rectangle(264, 535, 143, 63);

                //Window.Title = "mouseState = " + mouseState.X.ToString() + ", " + mouseState.Y.ToString();

                MenuKey = -1;
                for (int i = 0; i < 7; i++)
                {
                    if (MouseRect.Intersects(MenuKeys[i]))
                    {
                        MenuKey = i;
                        break;
                    }
                }

                if (SUBMENU == 0) // sai do menu, comeca jogo
                {

                    if (MouseRect.Intersects(MenuKeys[0]) && mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released)
                    {
                        if (Clave == 7) ModoSurpresa = true;
                        else ModoSurpresa = false;

                        ResetaGB();
                        ShowRB = false;
                        MENU = 0;
                        Pontos = 0;
                        Pontos2 = 0;
                        Vidas2 = 3;
                        Inversoes = 0;

                        paddlePosition = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - paddleSprite.Width / 2, graphics.GraphicsDevice.Viewport.Height - paddleSprite.Height);
                        paddlePosition2 = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - paddleSprite.Width / 2, graphics.GraphicsDevice.Viewport.Height - paddleSprite.Height - 472); 

                        RBPos = new Vector2(200, 198);

                        ballSpeed = new Vector2(100, 100); 

                        if (Clave != 4 && Clave != 5 && Clave != 6 && Clave != 8) 
                        {

                            if (DIFICULDADE == 0) ballSpeed = new Vector2(100, 100);
                            else if (DIFICULDADE == 1) ballSpeed = new Vector2(150, 150);
                            else if (DIFICULDADE == 2) ballSpeed = new Vector2(200, 200);

                            if (Clave == 7 && CONTROLE == 1) ballSpeed = new Vector2(100, 100);
                        }
                        

                        if (Clave == 5)
                        {
                            if (DIFICULDADE == 2)
                            {
                                CifraBaixo = new int[][]// indices para array CifraSprite
                                {
                                    new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 }, // la (==5)
                                    new int[] { 0, 2, 3, 5, 6, 7, 10, 11, 12, 16}, // b
                                    new int[] { 0, 17, 2, 3, 5, 7, 10, 11, 18, 15, 16}, // c
                                    new int[] { 0, 1, 2, 3, 4, 5, 7, 15, 10, 11, 13, 9 },  // d
                                    new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 13, 14 }, // e
                                    new int[] { 0, 17, 2, 3, 5, 7, 11, 16}, //f
                                    new int[] { 0, 17, 2, 3, 5, 7, 10, 11, 15 }, // G
                                };
                            }
                            if (DIFICULDADE == 1)
                            {
                                CifraBaixo = new int[][]// indices para array CifraSprite
                                {
                                    new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, // la (==5)
                                    new int[] { 0, 2, 3, 5, 6, 7}, // b
                                    new int[] { 0, 17, 2, 3, 5, 7}, // c
                                    new int[] { 0, 1, 2, 3, 4, 5, 7},  // d
                                    new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }, // e
                                    new int[] { 0, 17, 2, 3, 5, 7 }, //f
                                    new int[] { 0, 17, 2, 3, 5, 7 }, // G
                                };
                            }
                            if (DIFICULDADE == 0)
                            {
                                CifraBaixo = new int[][]// indices para array CifraSprite
                                {
                                    new int[] { 0, 1, 2, 3, 4, 5 }, // la (==5)
                                    new int[] { 0, 2, 3, 5, 6 }, // b
                                    new int[] { 0, 17, 2, 3, 5,}, // c
                                    new int[] { 0, 1, 2, 3, 4, 5 },  // d
                                    new int[] { 0, 1, 2, 3, 4, 5 }, // e
                                    new int[] { 0, 17, 2, 3, 5 }, //f
                                    new int[] { 0, 17, 2, 3, 5 }, // G
                                };
                            }
                        }

                        if (Clave == 0 || Clave == 5 || Clave == 10)
                        {
                            noteSprite[0] = Content.Load<Texture2D>("C1");
                            noteSprite[1] = Content.Load<Texture2D>("D1");
                            noteSprite[2] = Content.Load<Texture2D>("E1");
                            noteSprite[3] = Content.Load<Texture2D>("F1");
                            noteSprite[4] = Content.Load<Texture2D>("G1");
                            noteSprite[5] = Content.Load<Texture2D>("A1");
                            noteSprite[6] = Content.Load<Texture2D>("B1");
                            noteSprite[7] = Content.Load<Texture2D>("C2");
                            noteSprite[8] = Content.Load<Texture2D>("D2");
                            noteSprite[9] = Content.Load<Texture2D>("E2");
                            noteSprite[10] = Content.Load<Texture2D>("F2");
                            noteSprite[11] = Content.Load<Texture2D>("G2");
                            noteSprite[12] = Content.Load<Texture2D>("A2");
                            noteSprite[13] = Content.Load<Texture2D>("B2");
                            noteSprite[14] = Content.Load<Texture2D>("C3");
                        }
                        else if (Clave == 1 || Clave == 9)
                        {
                            noteSprite[0] = Content.Load<Texture2D>("C1_ClaveSol");
                            noteSprite[1] = Content.Load<Texture2D>("D1_ClaveSol");
                            noteSprite[2] = Content.Load<Texture2D>("E1_ClaveSol");
                            noteSprite[3] = Content.Load<Texture2D>("F1_ClaveSol");
                            noteSprite[4] = Content.Load<Texture2D>("G1_ClaveSol");
                            noteSprite[5] = Content.Load<Texture2D>("A1_ClaveSol");
                            noteSprite[6] = Content.Load<Texture2D>("B1_ClaveSol");
                            noteSprite[7] = Content.Load<Texture2D>("C2_ClaveSol");
                            noteSprite[8] = Content.Load<Texture2D>("D2_ClaveSol");
                            noteSprite[9] = Content.Load<Texture2D>("E2_ClaveSol");
                            noteSprite[10] = Content.Load<Texture2D>("F2_ClaveSol");
                            noteSprite[11] = Content.Load<Texture2D>("G2_ClaveSol");
                            noteSprite[12] = Content.Load<Texture2D>("A2_ClaveSol");
                            noteSprite[13] = Content.Load<Texture2D>("B2_ClaveSol");
                            noteSprite[14] = Content.Load<Texture2D>("C3_ClaveSol");
                        }
                        else if (Clave == 2 || Clave == 11)
                        {
                            noteSprite[0] = Content.Load<Texture2D>("C1_ClaveDo");
                            noteSprite[1] = Content.Load<Texture2D>("D1_ClaveDo");
                            noteSprite[2] = Content.Load<Texture2D>("E1_ClaveDo");
                            noteSprite[3] = Content.Load<Texture2D>("F1_ClaveDo");
                            noteSprite[4] = Content.Load<Texture2D>("G1_ClaveDo");
                            noteSprite[5] = Content.Load<Texture2D>("A1_ClaveDo");
                            noteSprite[6] = Content.Load<Texture2D>("B1_ClaveDo");
                            noteSprite[7] = Content.Load<Texture2D>("C2_ClaveDo");
                            noteSprite[8] = Content.Load<Texture2D>("D2_ClaveDo");
                            noteSprite[9] = Content.Load<Texture2D>("E2_ClaveDo");
                            noteSprite[10] = Content.Load<Texture2D>("F2_ClaveDo");
                            noteSprite[11] = Content.Load<Texture2D>("G2_ClaveDo");
                            noteSprite[12] = Content.Load<Texture2D>("A2_ClaveDo");
                            noteSprite[13] = Content.Load<Texture2D>("B2_ClaveDo");
                            noteSprite[14] = Content.Load<Texture2D>("C3_ClaveDo");
                        }
                        else if (Clave == 3)
                        {
                            noteSprite[0] = Content.Load<Texture2D>("C1");
                            noteSprite[1] = Content.Load<Texture2D>("D1");
                            noteSprite[2] = Content.Load<Texture2D>("E1");
                            noteSprite[3] = Content.Load<Texture2D>("F1");
                            noteSprite[4] = Content.Load<Texture2D>("G1");
                            noteSprite[5] = Content.Load<Texture2D>("A1");
                            noteSprite[6] = Content.Load<Texture2D>("B1");
                            noteSprite[7] = Content.Load<Texture2D>("C2");
                            noteSprite[8] = Content.Load<Texture2D>("D2");
                            noteSprite[9] = Content.Load<Texture2D>("E2");
                            noteSprite[10] = Content.Load<Texture2D>("F2");
                            noteSprite[11] = Content.Load<Texture2D>("G2");
                            noteSprite[12] = Content.Load<Texture2D>("A2");
                            noteSprite[13] = Content.Load<Texture2D>("B2");
                            noteSprite[14] = Content.Load<Texture2D>("C1_ClaveSol");
                            noteSprite[15] = Content.Load<Texture2D>("D1_ClaveSol");
                            noteSprite[16] = Content.Load<Texture2D>("E1_ClaveSol");
                            noteSprite[17] = Content.Load<Texture2D>("F1_ClaveSol");
                            noteSprite[18] = Content.Load<Texture2D>("G1_ClaveSol");
                            noteSprite[19] = Content.Load<Texture2D>("A1_ClaveSol");
                            noteSprite[20] = Content.Load<Texture2D>("B1_ClaveSol");
                            noteSprite[21] = Content.Load<Texture2D>("C2_ClaveSol");
                            noteSprite[22] = Content.Load<Texture2D>("D2_ClaveSol");
                            noteSprite[23] = Content.Load<Texture2D>("E2_ClaveSol");
                            noteSprite[24] = Content.Load<Texture2D>("F2_ClaveSol");
                            noteSprite[25] = Content.Load<Texture2D>("G2_ClaveSol");
                            noteSprite[26] = Content.Load<Texture2D>("A2_ClaveSol");
                            noteSprite[27] = Content.Load<Texture2D>("B2_ClaveSol");
                            noteSprite[28] = Content.Load<Texture2D>("C3_ClaveSol");
                        }


                        int newNote = Rand.Next(15);
                        if (Clave == 3) newNote = Rand.Next(29);
                        else if (Clave == 5)
                        {
                            newNote = 5 + Rand.Next(7);
                            Temp = Rand.Next(CifraBaixo[newNote - 5].Length);
                            CurrentChord = new Chord(AcordeBaixo[newNote-5][Temp]);                            
                        }
                        else if (Clave == 6)
                        {
                            int Modo = Rand.Next(2);
                            int Tom = Rand.Next(12);
                            int Acorde = 0;
                            if (DIFICULDADE == 0) Acorde = Rand.Next(AcordeGrausFacil[0].Length-1);
                            else if (DIFICULDADE == 1) Acorde = Rand.Next(AcordeGrausMedio[0].Length - 1);
                            else if (DIFICULDADE == 2) Acorde = Rand.Next(AcordeGrausDificil[0].Length - 1);
                            Clave6Acorde = Tonalidades[Modo][Tom] + ": " + AcordeGrausDificil[Modo][Acorde];

                            string NewChord = "";
                            if (Modo ==0) NewChord = EscalasMaiores[Tom][AcordeGrausIndex[Acorde]] + AcordeGrausPatterns[Modo][Acorde];
                            else if (Modo == 1) NewChord = EscalasMenoresNatural[Tom][AcordeGrausIndex[Acorde]] + AcordeGrausPatterns[Modo][Acorde];
                            CurrentChord = new Chord(NewChord);

                        }
                        else if (Clave == 9 || Clave == 10 || Clave == 11) newNote = Rand.Next(8);

                        if (Clave == 10) newNote += 7;

                        ballSprite = noteSprite[newNote];
                        if (Clave == 4 || Clave == 6 || Clave == 8) ballSprite = AcordeMoldura;

                        CurrentNote = newNote;
                        
                        soundEngine.Stop();
                        soundEngine = Beat.CreateInstance();
                        soundEngine.Volume = 0.5f;

                        if (InputDevice.InstalledDevices.Count != 0)
                        {
                            MidiIn = InputDevice.InstalledDevices[CurrentMIDI];
                            MidiIn.Open();
                            MidiIn.StartReceiving(null);
                            MidiIn.NoteOn += new InputDevice.NoteOnHandler(NoteOn);
                            MidiIn.NoteOff += new InputDevice.NoteOffHandler(NoteOff);
                        }
                        timer = Stopwatch.StartNew();
                        halt = 60;
                        ClaveOriginal = Clave;
                        if (Clave == 7) Surpresa();

                        if (Clave == 8)
                        {
                            MostrarTeclado = true;
                            halt = 400;
                        }
                        

                    }
                    else if (MouseRect.Intersects(MenuKeys[1]) && mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released)
                    {
                        SUBMENU = 2;
                    }
                    else if (MouseRect.Intersects(MenuKeys[2]) && mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released)
                    {
                        SUBMENU = 1;
                    }
                    else if (MouseRect.Intersects(MenuKeys[3]) && mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released)
                    {
                       SUBMENU = 4;
                    }
                    else if (MouseRect.Intersects(MenuKeys[4]) && mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released)
                    {
                        if (RecordeTotal() < 2500) SUBMENU = 6;
                        else SUBMENU = 12;
                    }
                    else if (MouseRect.Intersects(MenuKeys[5]) && mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released)
                    {
                        Efeito[0] = Rand.Next(EfeitosColaterais.Length);
                        while (Efeito[1] == Efeito[0]) Efeito[1] = Rand.Next(EfeitosColaterais.Length);
                        while (Efeito[2] == Efeito[1] || Efeito[2] == Efeito[0]) Efeito[2] = Rand.Next(EfeitosColaterais.Length);
                        while (Efeito[3] == Efeito[2] || Efeito[3] == Efeito[1] || Efeito[3] == Efeito[0]) Efeito[3] = Rand.Next(EfeitosColaterais.Length);
                        while (Efeito[4] == Efeito[3] || Efeito[4] == Efeito[2] || Efeito[4] == Efeito[1] || Efeito[4] == Efeito[0]) Efeito[4] = Rand.Next(EfeitosColaterais.Length);
                        while (Efeito[5] == Efeito[4] || Efeito[5] == Efeito[3] || Efeito[5] == Efeito[2] || Efeito[5] == Efeito[1] || Efeito[5] == Efeito[0]) Efeito[5] = Rand.Next(EfeitosColaterais.Length);
                        while (Efeito[6] == Efeito[5] || Efeito[6] == Efeito[4] || Efeito[6] == Efeito[3] || Efeito[6] == Efeito[2] || Efeito[6] == Efeito[1] || Efeito[6] == Efeito[0]) Efeito[6] = Rand.Next(EfeitosColaterais.Length);
                        SUBMENU = 667;
                        //this.Exit();
                    }
                    else if (MouseRect.Intersects(MenuKeys[6]) && mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released)
                    {
                        SUBMENU = 3;
                        soundEngineSobre.Play();
                        soundEngine.Pause();
                    }

                }
                else if (SUBMENU == 1)
                {
                   keyState = Keyboard.GetState();
                   //if (keyState.IsKeyDown(Keys.Escape)) SUBMENU = 0;

                    if (MouseRect.Intersects(MenuKeys[1]) && mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released)
                    {

                        if (MIDICount > 0)
                        {
                            if (CONTROLE == 0) CONTROLE = 1;
                            else if (CONTROLE == 1) CONTROLE = 0;
                        }
                        else CONTROLE = 0;

                    }
                    else if (MouseRect.Intersects(MenuKeys[2]) && mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released)
                    {
                        CurrentMIDI += 1;
                        if (CurrentMIDI == MIDICount) CurrentMIDI = 0;                        
                        
                    }
                    if (MouseRect.Intersects(MenuKeys[3]) && mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released)
                    {
                        if (DIFICULDADE == 0) DIFICULDADE = 1;
                        else if (DIFICULDADE == 1) DIFICULDADE = 2;
                        else if (DIFICULDADE == 2) DIFICULDADE = 0;
                    }
                    else if (MouseRect.Intersects(MenuKeys[4]) && mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released)
                    {                        
                        SUBMENU = 11;                        
                        
                    }
                    else if (MouseRect.Intersects(MenuKeys[5]) && mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released)
                    {
                        if (Distracoes) Distracoes = false;
                        else if (!Distracoes) Distracoes = true;
                    }
                    else if (MouseRect.Intersects(MenuKeys[6]) && mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released)
                    {
                        SUBMENU = 0;
                    }
                }
                else if (SUBMENU == 2)
                {
                    if (mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released)
                    {
                        InstrucaoPagina += 1;
                        
                    }
                    if (InstrucaoPagina > 2)
                    {
                        InstrucaoPagina = 0;
                        SUBMENU = 0;
                    }
                }
                else if (SUBMENU == 3)
                {
                    if (mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released)
                    {
                        InstrucaoPagina += 1;
                        if (InstrucaoPagina > 1)
                        {
                            InstrucaoPagina = 0;
                            soundEngineSobre.Pause();
                            soundEngine.Resume();
                            SUBMENU = 0;
                        }

                    }
                }
                else if (SUBMENU == 4)
                {
                    if (mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released)
                    {
                        InstrucaoPagina += 1;
                    }
                    if (InstrucaoPagina > 2)
                    {
                        InstrucaoPagina = 0;
                        SUBMENU = 0;
                    }
                        
                }
                else if (SUBMENU == 6)
                {
                    if (mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released)
                    {
                        SUBMENU = 0;
                    }
                }
                else if (SUBMENU == 11) // MODO JOGO
                {
                    ModoAtual = -1;
                    Rectangle[] Modojogo = new Rectangle[12];
                    Modojogo[0] = new Rectangle(280, 280, 110, 90);
                    Modojogo[1] = new Rectangle(390, 280, 110, 90);
                    Modojogo[2] = new Rectangle(510, 280, 110, 90);
                    Modojogo[3] = new Rectangle(620, 280, 110, 90);
                    
                    Modojogo[4] = new Rectangle(280, 380, 110, 90);
                    Modojogo[5] = new Rectangle(390, 380, 110, 90);
                    Modojogo[6] = new Rectangle(510, 380, 110, 90);
                    Modojogo[7] = new Rectangle(620, 380, 110, 90);
                    
                    Modojogo[8] = new Rectangle(280, 480, 110, 90);
                    Modojogo[9] = new Rectangle(390, 480, 110, 90);
                    Modojogo[10] = new Rectangle(510, 480, 110, 90);
                    Modojogo[11] = new Rectangle(620, 480, 110, 90);
                    
                    for (int i = 0; i < 12; i++)
                    {
                        if (MouseRect.Intersects(Modojogo[i]))
                        {
                            ModoAtual = i;
                            Window.Title += " ModoAtual = " + ModoAtual.ToString();
                            break;
                        }
                    }

                    if (ModoAtual != -1 && ModoAtual != PrevModoAtual) Beep.Play();

                    if (ModoAtual != -1) PrevModoAtual = ModoAtual;

                                        
                    if (mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released && ModoAtual != -1)
                    {
                        Clave = ModoJogo[ModoAtual];
                        SUBMENU = 1;
                    }
                }
                else if (SUBMENU == 12) // MODO JOGO
                {
                    ModoAtual = -1;
                    Rectangle[] Modojogo = new Rectangle[12];
                    Modojogo[0] = new Rectangle(280 + 40, 280 + 20, 110, 90);
                    Modojogo[1] = new Rectangle(390 + 40, 280 + 20, 110, 90);
                    Modojogo[2] = new Rectangle(510 + 40, 280 + 20, 110, 90);
                    Modojogo[3] = new Rectangle(0, 0, 0, 0);

                    Modojogo[4] = new Rectangle(280 + 40, 380 + 20, 110, 90);
                    Modojogo[5] = new Rectangle(390 + 40, 380 + 20, 110, 90);
                    Modojogo[6] = new Rectangle(510 + 40, 380 + 20, 110, 90);
                    Modojogo[7] = new Rectangle(0, 0, 0, 0);

                    Modojogo[8] = new Rectangle(0, 0, 0, 0);
                    Modojogo[9] = new Rectangle(0, 0, 0, 0);
                    Modojogo[10] = new Rectangle(0, 0, 0, 0);
                    Modojogo[11] = new Rectangle(0, 0, 0, 0);
                    
                    for (int i = 0; i < 12; i++)
                    {
                        if (MouseRect.Intersects(Modojogo[i]))
                        {
                            ModoAtual = i;
                            Window.Title +=  "ModoAtual = " + ModoAtual.ToString();
                            break;
                        }
                    }

                    if (ModoAtual != -1 && ModoAtual != PrevModoAtual) Beep.Play();

                    if (ModoAtual != -1) PrevModoAtual = ModoAtual;


                    if (mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released && ModoAtual == -1)
                    {                        
                        SUBMENU = 0;
                    }

                    if (mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released && ModoAtual != -1)
                    {
                        if (ModoAtual == 0) //palito
                        {
                            MINIGAME = true;
                            soundEngine.Stop();
                            GAMEBOY = true;
                            GBTYPE = 0;
                            ABSTART = false;
                            soundEngineMario = Mozart.CreateInstance();
                            soundEngineMario.Volume = .7f;
                            soundEngineMario.Play();
                            PalitoPos = new Vector2(400, 220);
                            GBTimer = 140;
                            FacaCount = 0;
                            FacaPos.X = 570;
                            FacaPos.Y = 260;
                            FacaTimer = 0;
                            FacaDir = 0;
                        }
                        else if (ModoAtual == 1) // escalator 
                        {
                            MINIGAME = true;
                            soundEngine.Stop();
                            GAMEBOY = true;
                            GBTYPE = 2;
                            EscalaAtual = "Nenhuma";
                            MiraPos = new Vector2(300, 300);
                            GBTimer = 150;
                            PontosEscalator = 0;
                            NotasEscalator.Clear();
                            soundEngineMario = DoomMusic.CreateInstance();
                            soundEngineMario.Volume = .6f;
                            soundEngineMario.IsLooped = true;
                            soundEngineMario.Play();
                            TrocarEscala();
                        }
                        else if (ModoAtual == 2) // univer
                        {
                            MINIGAME = true;
                            soundEngine.Stop();
                            GAMEBOY = true;
                            RespostaErrada = false;
                            GBTYPE = 4;
                            GBTimer = 250;
                            TempoSobrando = new TimeSpan(0, 0, 0, 31);
                            PontosBambu = 0;
                            PerguntaTimer = 0;
                            TrocarPergunta();
                            soundEngineMario = AberturaBambu.CreateInstance();
                            soundEngineMario.Volume = 1f;
                            soundEngineMario.IsLooped = false;
                            soundEngineMario.Play();
                        }
                        else if (ModoAtual == 4) // link
                        {
                            MINIGAME = true;
                            soundEngine.Stop();
                            GAMEBOY = true;
                            GBTYPE = 5;
                            ENERGIA = 250;
                            PontosLink = 0;
                            Links.Clear();
                            InimigosLinkLista.Clear();
                            GBTimer = 0;
                            TrocaNotabase = false;
                            Links.Add(new Link(new Vector2(200, 200), 0));
                            NotasLink.Add(new NotaLink(new Vector2(135, 540), 3));
                            NotasLink.Add(new NotaLink(new Vector2(275, 540), 4));
                            NotasLink.Add(new NotaLink(new Vector2(410, 540), 5));
                            NotasLink.Add(new NotaLink(new Vector2(550, 540), 6));
                            NotasLink.Add(new NotaLink(new Vector2(710, 100), 7));
                            NotasLink.Add(new NotaLink(new Vector2(710, 270), 8));
                            NotasLink.Add(new NotaLink(new Vector2(710, 420), 9));
                            NotasLink.Add(new NotaLink(new Vector2(710, 540), 10));
                            InimigosLinkLista.Add(new InimigoLink(new Vector2(550, 300), 4, new Vector2(0, 0)));
                            TrocaNotabase = true;
                            NotabaseTimer = 1001;
                            soundEngineMario = Zelda.CreateInstance();
                            soundEngineMario.Volume = 0.9f;
                            soundEngineMario.IsLooped = true;
                            soundEngineMario.Play();
                            noteSprite[0] = Content.Load<Texture2D>("C1_ClaveSol");
                            noteSprite[1] = Content.Load<Texture2D>("D1_ClaveSol");
                            noteSprite[2] = Content.Load<Texture2D>("E1_ClaveSol");
                            noteSprite[3] = Content.Load<Texture2D>("F1_ClaveSol");
                            noteSprite[4] = Content.Load<Texture2D>("G1_ClaveSol");
                            noteSprite[5] = Content.Load<Texture2D>("A1_ClaveSol");
                            noteSprite[6] = Content.Load<Texture2D>("B1_ClaveSol");
                            noteSprite[7] = Content.Load<Texture2D>("C2_ClaveSol");
                            noteSprite[8] = Content.Load<Texture2D>("D2_ClaveSol");
                            noteSprite[9] = Content.Load<Texture2D>("E2_ClaveSol");
                            noteSprite[10] = Content.Load<Texture2D>("F2_ClaveSol");
                            noteSprite[11] = Content.Load<Texture2D>("G2_ClaveSol");
                            noteSprite[12] = Content.Load<Texture2D>("A2_ClaveSol");
                            noteSprite[13] = Content.Load<Texture2D>("B2_ClaveSol");
                            noteSprite[14] = Content.Load<Texture2D>("C3_ClaveSol");
                            GBTimer = 0;
                        }
                        else if (ModoAtual == 5) // ah nao
                        {
                            MINIGAME = true;
                            soundEngine.Stop();
                            GAMEBOY = true;
                            GBTimer = 0;
                            GBTYPE = 6;
                            GBTimer2 = 0;
                            PontosPalavra = 0;
                            PalavrasLista.Clear();
                            PalavrasLista.Add(new Palavra(new Vector2(Rand.Next(400) + 50, -20), Palavras[Rand.Next(Palavras.Length)], false));
                            soundEngineMario = DrMario.CreateInstance();
                            soundEngineMario.Volume = 1.0f;
                            soundEngineMario.IsLooped = true;
                            soundEngineMario.Play();

                        }
                        else if (ModoAtual == 6) // forca
                        {
                            MINIGAME = true;
                            GAMEBOY = true;
                            GBTYPE = 3;
                            soundEngine.Stop(); 
                            soundEngineMario = TicTac.CreateInstance();
                            soundEngineMario.Volume = 1f;
                            soundEngineMario.Play();
                            GBTimer = 0;
                            TomCount = 0;
                            TomStatus = 0;
                            ForcaStatus = 0;
                        }

                    }


                }
                else if (SUBMENU == 5)
                {
                    keyState = Keyboard.GetState();
                    //if (keyState.IsKeyDown(Keys.Escape)) SUBMENU = 0;

                    if (MouseRect.Intersects(MenuKeys[0]) && mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released)
                    {
                        MINIGAME = true;
                        soundEngine.Stop();
                        GAMEBOY = true;
                        GBTYPE = 0;
                        ABSTART = false;
                        soundEngineMario = Mozart.CreateInstance();
                        soundEngineMario.Volume = .7f;
                        soundEngineMario.Play();
                        PalitoPos = new Vector2(400, 220);
                        GBTimer = 140;
                        FacaCount = 0;
                        FacaPos.X = 570;
                        FacaPos.Y = 260;
                        FacaTimer = 0;
                        FacaDir = 0;

                    }
                    else if (MouseRect.Intersects(MenuKeys[2]) && mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released)
                    {
                        MINIGAME = true;
                        soundEngine.Stop();
                        GAMEBOY = true;
                        GBTYPE = 2;
                        EscalaAtual = "Nenhuma";
                        MiraPos = new Vector2(300, 300);
                        GBTimer = 150;
                        PontosEscalator = 0;
                        NotasEscalator.Clear();
                        soundEngineMario = DoomMusic.CreateInstance();
                        soundEngineMario.Volume = .6f;
                        soundEngineMario.IsLooped = true;
                        soundEngineMario.Play();
                        TrocarEscala();
                    }
                    else if (MouseRect.Intersects(MenuKeys[4]) && mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released)
                    {
                        MINIGAME = true; 
                        soundEngine.Stop();                         
                        GAMEBOY = true;
                        RespostaErrada = false;
                        GBTYPE = 4;
                        GBTimer = 250;
                        TempoSobrando = new TimeSpan(0, 0, 0, 31);
                        PontosBambu = 0;
                        PerguntaTimer = 0;
                        TrocarPergunta();
                        soundEngineMario = AberturaBambu.CreateInstance();
                        soundEngineMario.Volume = 1f;
                        soundEngineMario.IsLooped = false;
                        soundEngineMario.Play();                        
                    }
                    else if (MouseRect.Intersects(MenuKeys[6]) && mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released)
                    {
                        SUBMENU = 0;
                    }

                }
                else if (SUBMENU == 667)
                {
                    //MouseState mouseState = Mouse.GetState();
                    soundEngine.Stop();
                    if (GBTimer > 500 || (mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released))
                    {
                        GBTimer = 0;
                        this.Exit();
                    }
                    PrevLeftState = mouseState.LeftButton;
                    PrevRightState = mouseState.RightButton;

                }
                else if (SUBMENU == 666)
                {
                    if ((mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released) || (mouseState.RightButton == ButtonState.Pressed && PrevRightState == ButtonState.Released))
                    {
                        if (soundEngineMorte.State == SoundState.Stopped)
                        {
                            SUBMENU = 0;
                            Pontos = 0;
                            Pontos2 = 0;
                        }

                    }
                }

                PrevLeftState = mouseState.LeftButton;
                PrevRightState = mouseState.RightButton;

            }

            if (FINAL != 2 && MENU != 1 && !GAMEBOY)  // loop principal do jogo
            {

                if (((-0.3F) + (Pontos * 0.0004F)) > 1 && (FINAL == 0))
                {
                    FINAL = 1;
                    soundEngine.Stop();
                    soundEngine.Pitch = 0;
                    soundEngine.Volume = 0.8f;
                    soundEngine = Tetris.CreateInstance();
                    soundEngine.Play();
                    try
                    {
                        soundEngine.IsLooped = true;
                    }
                    catch
                    {
                    }
                    goto Pular;
                }

                if (FINAL == 0) soundEngine.Pitch = (-0.3F) + (Pontos * 0.0004F);
                else soundEngine.Pitch = 0;
            Pular: ;
                  
                TimerEventos += 1;
                if (Pontos > 870) TimerGameboy += 1;


                // PROGRESSAO DE NIVEL
                if ( (Clave == 9 || Clave == 10 || Clave == 11 ) && Pontos > 400 && !GAMEBOY) // To DO: arrumar isso
                {
                    if (Clave == 9) Clave = 1;
                    else if (Clave == 10) Clave = 0;
                    else if (Clave == 11) Clave = 2;                     
                    
                    ConfiguraProximaNota();
                    GAMEBOY = true;
                    GBTYPE = 12;

                    if (ShowMM) ShowMM = false;
                    if (ShowFerer) ShowFerer = false;
                }

                if (Rand.Next(2500) > 2497 && !ShowMM && Pontos > 900 && (ballSpeed.Y < 0) && TimerGameboy > 500 && Clave != 8)
                {
                    // impede que o GBTypes dos minigames repitam
                    GBTYPE = Rand.Next(6);
                    if (GB[GBTYPE] == true)
                    {
                        if (GB[0] == true && GB[1] == true && GB[2] == true && GB[3] == true && GB[4] == true && GB[5] == true && GB[6] == true) goto GameboyFinal;
                        
                        while (GB[GBTYPE] == true) GBTYPE = Rand.Next(7);
                    }
                    GB[GBTYPE] = true;

                    // inicializacao
                    TimerGameboy = 0;
                    soundEngine.Volume = 0;
                    soundEngine.Stop();                    
                    MINIGAME = false;
                    GAMEBOY = true;                    

                    if (GBTYPE == 0)
                    {
                        soundEngine.Pause();
                        soundEngineMario = Mozart.CreateInstance();
                        soundEngineMario.Volume = .7f;
                        ABSTART = true;
                        PalitoPos = new Vector2(400, 220);
                        FacaCount = 0;
                        FacaPos.X = 570;
                        FacaPos.Y = 260;
                        FacaTimer = 0;
                        FacaDir = 0;
                    }
                    if (GBTYPE == 1)
                    {
                        soundEngine.Pause();
                        soundEngineMario = Mario.CreateInstance();
                        soundEngineMario.Volume = .7f;
                        BowserPos.X = 800;
                        BowserEnergia = 25;
                        ShowBowser = true;
                        BowserNotas[0] = Rand.Next(15);
                        BowserNotas[1] = Rand.Next(15);
                        BowserNotas[2] = Rand.Next(15);
                        BowserNotas[3] = Rand.Next(15);
                        BowserNotas[4] = Rand.Next(15);
                        BowserNotas[5] = Rand.Next(15);
                        BowserNotas[6] = Rand.Next(15);
                        BowserNotasStatus[0] = true;
                        BowserNotasStatus[1] = true;
                        BowserNotasStatus[2] = true;
                        BowserNotasStatus[3] = true;
                        BowserNotasStatus[4] = true;
                        BowserNotasStatus[5] = true;
                        BowserNotasStatus[6] = true;
                        BowserNotaAtual = 0;
                        PontosPerdidos = 0;
                        PianoShake = false;
                        PianoShake2 = false;
                        PianoOffset.X = 0;
                        PianoOffset.Y = 0;
                    }
                    if (GBTYPE == 2)
                    {
                        MiraPos = new Vector2(300, 300);
                        soundEngine.Pause();
                        soundEngineMario = DoomMusic.CreateInstance();
                        soundEngineMario.Volume = .6f;
                        soundEngineMario.IsLooped = true;
                        PontosEscalator = 0;
                        NotasEscalator.Clear();
                        EscalaAtual = "Nenhuma";
                        TrocarEscala();
                    }
                    if (GBTYPE == 3)
                    {
                        soundEngineMario = TicTac.CreateInstance();
                        soundEngineMario.Volume = 1f;                        
                        GBTimer = 0;
                        TomCount = 0;
                        TomStatus = 0;
                        ForcaStatus = 0;
                    }
                    if (GBTYPE == 4)
                    {                        
                        RespostaErrada = false;                        
                        GBTimer = 0;
                        TempoSobrando = new TimeSpan(0, 0, 0, 31);
                        PontosBambu = 0;
                        PerguntaTimer = 0;
                        TrocarPergunta();
                        soundEngineMario = AberturaBambu.CreateInstance();
                        soundEngineMario.Volume = 1f;
                        soundEngineMario.IsLooped = false;
                        soundEngine.Volume = 0;
                        soundEngine.Stop();                        

                    }
                    if (GBTYPE == 5) // link
                    {
                        /// Inicialização do Link
                        GAMEBOY = true;
                        GBTYPE = 5;
                        ENERGIA = 250;
                        PontosLink = 0;
                        Links.Clear();
                        InimigosLinkLista.Clear();
                        GBTimer = 0;
                        TrocaNotabase = false;
                        Links.Add(new Link(new Vector2(200, 200), 0));
                        NotasLink.Add(new NotaLink(new Vector2(135, 540), 3));
                        NotasLink.Add(new NotaLink(new Vector2(275, 540), 4));
                        NotasLink.Add(new NotaLink(new Vector2(410, 540), 5));
                        NotasLink.Add(new NotaLink(new Vector2(550, 540), 6));
                        NotasLink.Add(new NotaLink(new Vector2(710, 100), 7));
                        NotasLink.Add(new NotaLink(new Vector2(710, 270), 8));
                        NotasLink.Add(new NotaLink(new Vector2(710, 420), 9));
                        NotasLink.Add(new NotaLink(new Vector2(710, 540), 10));
                        InimigosLinkLista.Add(new InimigoLink(new Vector2(550, 300), 4, new Vector2(0, 0)));
                        TrocaNotabase = true;
                        NotabaseTimer = 1001;
                        soundEngineMario = Zelda.CreateInstance();
                        soundEngineMario.Volume = 0.9f;
                        soundEngineMario.IsLooped = true;
                        soundEngineMario.Play();
                        noteSprite[0] = Content.Load<Texture2D>("C1_ClaveSol");
                        noteSprite[1] = Content.Load<Texture2D>("D1_ClaveSol");
                        noteSprite[2] = Content.Load<Texture2D>("E1_ClaveSol");
                        noteSprite[3] = Content.Load<Texture2D>("F1_ClaveSol");
                        noteSprite[4] = Content.Load<Texture2D>("G1_ClaveSol");
                        noteSprite[5] = Content.Load<Texture2D>("A1_ClaveSol");
                        noteSprite[6] = Content.Load<Texture2D>("B1_ClaveSol");
                        noteSprite[7] = Content.Load<Texture2D>("C2_ClaveSol");
                        noteSprite[8] = Content.Load<Texture2D>("D2_ClaveSol");
                        noteSprite[9] = Content.Load<Texture2D>("E2_ClaveSol");
                        noteSprite[10] = Content.Load<Texture2D>("F2_ClaveSol");
                        noteSprite[11] = Content.Load<Texture2D>("G2_ClaveSol");
                        noteSprite[12] = Content.Load<Texture2D>("A2_ClaveSol");
                        noteSprite[13] = Content.Load<Texture2D>("B2_ClaveSol");
                        noteSprite[14] = Content.Load<Texture2D>("C3_ClaveSol");
                        GBTimer = 0;
                    }
                    else if (GBTYPE == 6)
                    {                        
                        GBTimer = 0;
                        GBTimer2 = 0;
                        GBTYPE = 6;
                        PontosPalavra = 0;
                        PalavrasLista.Clear();
                        PalavrasLista.Add(new Palavra(new Vector2(Rand.Next(400) + 50, -20), Palavras[Rand.Next(Palavras.Length)], false));
                        soundEngineMario = DrMario.CreateInstance();
                        soundEngineMario.Volume = 1.0f;
                        soundEngineMario.IsLooped = true;
                        soundEngineMario.Play();

                    }
                    soundEngineMario.Pitch = 0f;
                    soundEngineMario.Play();
                }
            GameboyFinal: ;

                if (Rand.Next(900) > 898 && !ShowMM && Pontos > 300 && TimerEventos > 50 && Clave != 8)
                {
                    TimerEventos = 0;
                    MMType = Rand.Next(9);

                    if (MMType == 0) MMPos.Y = 90;
                    else if (MMType == 1) MMPos.Y = 60;
                    else if (MMType == 2) MMPos.Y = 110;
                    else if (MMType == 3) MMPos.Y = 70;
                    else if (MMType == 4) MMPos.Y = 65;
                    else if (MMType == 5) MMPos.Y = 80;
                    else if (MMType == 6) MMPos.Y = 65;
                    else if (MMType == 7) MMPos.Y = 90;
                    else if (MMType == 8) MMPos.Y = 105;

                    MMPos.X = Rand.Next(640) + 70;
                    
                    if (MMType == 4)
                    {
                        MMPos.X = Rand.Next(600) + 100;
                        soundEngineScream.Play();
                    }
                    else if (MMType == 6)
                    {
                        soundEngine.Pause();                        
                        soundEngineGretchen.Play();
                    }
                    else if (MMType == 7)
                    {
                        MMPos.X = Rand.Next(500) + 100;
                    }
                    else if (MMType == 8)
                    {
                        PacSide = Rand.Next(2);
                        if (PacSide == 0) MMPos.X = -60;
                        if (PacSide == 1) MMPos.X = 900;
                        soundEngine.Pause();
                        soundEnginePac.Play();
                    }


                    ShowMM = true;
                    if (PrimeiraDistracao)
                    {
                        GAMEBOY = true;
                        GBTYPE = 10;
                        ShowMM = false;
                    }

                }
                if (Rand.Next(1200) > 1198 && !ShowFerer && Pontos > 400 && !(Pontos > 2800) && TimerEventos > 100 && Clave != 8 && Distracoes)
                {
                    TimerEventos = 0;
                    ShowFerer = true;
                    if (PrimeiraDistracao)
                    {
                        GAMEBOY = true;
                        GBTYPE = 11;
                        ShowFerer = false;
                    }

                    FererRot = 0;
                    FererSide = Rand.Next(2);

                    if (FererSide == 0) FererPos.X = 0;
                    else if (FererSide == 1) FererPos.X = 864;

                    int FererPic = Rand.Next(2);

                    if (FererPic == 0) Ferer = Content.Load<Texture2D>("ferer");
                    else if (FererPic == 1)
                    {
                        Ferer = Content.Load<Texture2D>("McClary");
                        soundEngine.Pause();
                        soundEngineCoral.Play();
                    }
                }
                if (Rand.Next(1100) > 1098 && !ShowRB && Pontos > 450 && (RBStatus < 6) && TimerEventos > 100 && Clave != 8)
                {
                    TimerEventos = 0;
                    ShowRB = true; 
                    RBPos.Y = 198;
                    RBPos.X = 5 + Rand.Next(700);
                }
                if (Rand.Next(1500) > 1498 && !PianoShake && Pontos > 500 && TimerEventos > 100)
                {
                    TimerEventos = 0;

                    int Numero = Rand.Next(2);
                    if (Numero == 0)
                    {
                        PianoShake = true;
                        PianoShakeTimer = 0;
                    }
                    else if (Numero == 1)
                    {
                        PianoShake2 = true;
                        PianoShakeTimer2 = 0;
                    }
                }                

                if (PianoShake2)
                {
                    PianoShakeTimer2 += 1;

                    if (PianoShakeTimer2 > 250)
                    {
                        PianoShake2 = false;
                        rot = 0;
                    }
                }

                if (PianoShake)
                {                    
                    if (pianoDirecao == 0)
                    {
                        int t = Rand.Next(2);

                        if (t == 1)
                        {
                            PianoOffset.X += (float)Rand.NextDouble();
                            PianoOffset.Y -= (float)Rand.NextDouble() + 0.2f;
                        }
                        else if (t == 0)
                        {
                            PianoOffset.X += (float)Rand.NextDouble();
                            PianoOffset.Y += (float)Rand.NextDouble();
                        }

                    }
                    else if (pianoDirecao == 1)
                    {
                        int t = Rand.Next(2);

                        if (t == 1)
                        {
                            PianoOffset.X -= (float)Rand.NextDouble();
                            PianoOffset.Y += (float)Rand.NextDouble() + 0.2f;
                        }
                        else if (t == 0)
                        {
                            PianoOffset.X -= (float)Rand.NextDouble();
                            PianoOffset.Y -= (float)Rand.NextDouble();
                        }
                    }

                    if (PianoOffset.X > 20) pianoDirecao = 1;
                    if (PianoOffset.X < -20) pianoDirecao = 0;
                    if (PianoOffset.Y > 20) PianoOffset.Y = 10;
                    if (PianoOffset.Y < -20) PianoOffset.Y = -10;

                    PianoShakeTimer += 1; 
                    
                    if (PianoShakeTimer > 600)
                    {
                        PianoShake = false;
                        PianoShakeTimer = 0;
                        PianoOffset.X = 0;
                        PianoOffset.Y = 0;
                    }
                }


                if (ShowRB)
                {
                    RBPos.Y += 5;
                }

                if (ShowFerer)
                {
                    if (FererSide == 0) FererPos.X += 11; 
                    else if (FererSide == 1) FererPos.X -= 11;
                    FererPos.Y -= Rand.Next(4);
                    FererRot += 0.05F;
                }

                if (FererPos.X > 864 || FererPos.X < 0)
                {
                    ShowFerer = false;
                    FererPos.X = 2000;                    
                    FererPos.Y = Rand.Next(350) + 200;
                    soundEngineCoral.Stop();
                    soundEngine.Resume();
                }

                if (soundEngine.State == SoundState.Stopped && GAMEBOY != true)
                {
                    soundEngine.Volume = 0.5f;
                    if (MINIGAME) soundEngine.Volume = 0.8f;

                    soundEngine.Play();
                    try
                    {
                        soundEngine.IsLooped = true;
                    }
                    catch
                    {
                    }

                }


                if (Pontos > 200 && !Pt250 && Clave != 8)
                {
                    Pt250 = true;
                    RBPos.X = 5 + Rand.Next(700);
                    ShowRB = true;
                    
                }
                if (Pontos > 1000 && !Vida1000 && Clave != 8)
                {
                    Vidas += 1;
                    Vida1000 = true;
                    OneUp.Play();
                }
                if (Pontos > 2000 && !Vida2000 && Clave != 8)
                {
                    Vidas += 1;
                    Vida2000 = true;
                    OneUp.Play();
                }
                if (Pontos > 3000 && !Vida3000 && Clave != 8)
                {
                    Vidas += 1;
                    Vida3000 = true;
                    OneUp.Play();
                }


                // Move the sprite by speed, scaled by elapsed time
                if (halt != 0) halt--;

                if (halt == 0) ballPosition += ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                int maxX = graphics.GraphicsDevice.Viewport.Width - ballSprite.Width;
                int maxY = graphics.GraphicsDevice.Viewport.Height - ballSprite.Height;

                MouseState mouseState = Mouse.GetState();
                //Window.Title = "mouseState = " + mouseState.X.ToString() + ", " + mouseState.Y.ToString();
                

                //if (mouseState.Y < 36 && !(mouseState.X < 0 || mouseState.X > 864)) Mouse.SetPosition(mouseState.X, 36);

                //if (mouseState.X < 1) Mouse.SetPosition(2, mouseState.Y);
                //if (mouseState.X > 853) Mouse.SetPosition(852, mouseState.Y);
                //if (mouseState.Y > 599) Mouse.SetPosition(mouseState.X, 598);

                if (mouseState.LeftButton == ButtonState.Pressed) LeftButton++;
                if (mouseState.LeftButton == ButtonState.Released) LeftButton = 0;

                // Check for bounce
                if ((ballPosition.X > maxX || ballPosition.X < 0) && halt == 0) ballSpeed.X *= -1;                    


                if (Clave != 8 && ClaveOriginal != 3)
                {

                    if (ballPosition.Y < piano800.Height + 10 && halt == 0)
                    {
                        ballSpeed.Y *= -1;
                    }
                    else if (ballPosition.Y > maxY + 25)
                    {
                        // Ball hit the bottom of the screen, so reset ball
                        crashSound.Play();
                        ballPosition.Y = piano800.Height + 15;
                        ballPosition.X = Rand.Next(600) + 20;
                        if (Pontos > 100) Pontos -= 100;

                        Vidas -= 1;
                        ballSpeed.X = ballSpeed.X * 0.88F;
                        ballSpeed.Y = ballSpeed.Y * 0.88F;
                        halt = 40;
                        Inversoes = 0;

                        if (Vidas < 0)
                        {
                            Morrer();
                        }

                    }
                }
                else if (ClaveOriginal == 3)
                {
                    if (ballPosition.Y < 25 && halt == 0)
                    {
                        ballSpeed.Y *= -1;
                    }
                    else if (ballPosition.Y > maxY + 25)
                    {
                        // Ball hit the bottom of the screen, so reset ball
                        crashSound.Play();
                        ballPosition.Y = 60;
                        ballPosition.X = Rand.Next(600) + 20;
                        if (Pontos > 100) Pontos -= 100;

                        Vidas -= 1;
                        ballSpeed.X = ballSpeed.X * 0.88F;
                        ballSpeed.Y = ballSpeed.Y * 0.88F;
                        halt = 40;
                        Inversoes = 0;

                        if (Vidas < 0)
                        {
                            Morrer();
                        }

                    }

                }
                else if (Clave == 8) // 2P
                {
                    if (ballPosition.Y < 8 && halt == 0)
                    {
                        ballSpeed.Y *= -1;
                        Vidas2 -= 1;
                        crashSound.Play();

                        ballSpeed.Y = ballSpeed.Y * 0.93F;
                    }
                    else if (ballPosition.Y > maxY + 22 && halt == 0)
                    {
                        ballSpeed.Y *= -1;
                        Vidas -= 1;
                        crashSound.Play();

                        ballSpeed.Y = ballSpeed.Y * 0.93F;
                    }

                    if (Vidas < 0 || Vidas2 < 0) Morrer();

                }

                Rectangle MouseRect = new Rectangle(mouseState.X, mouseState.Y, 4, 4);
                Rectangle[] PianoKeys = new Rectangle[15];
                PianoKeys[0] = new Rectangle(6 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                PianoKeys[1] = new Rectangle(58 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                PianoKeys[2] = new Rectangle(110 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                PianoKeys[3] = new Rectangle(163 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                PianoKeys[4] = new Rectangle(217 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                PianoKeys[5] = new Rectangle(269 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                PianoKeys[6] = new Rectangle(323 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                PianoKeys[7] = new Rectangle(375 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                PianoKeys[8] = new Rectangle(428 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                PianoKeys[9] = new Rectangle(481 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                PianoKeys[10] = new Rectangle(536 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                PianoKeys[11] = new Rectangle(588 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                PianoKeys[12] = new Rectangle(641 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                PianoKeys[13] = new Rectangle(694 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);
                PianoKeys[14] = new Rectangle(746 + (int)PianoOffset.X, 40 + (int)PianoOffset.Y, 47, 144);

                if (Clave == 9 || Clave == 10 || Clave == 11) // uma oitava
                {
                    PianoKeys[0] = new Rectangle(6 + (int)PianoOffset.X + 200, 40 + (int)PianoOffset.Y, 47, 144);
                    PianoKeys[1] = new Rectangle(58 + (int)PianoOffset.X + 200, 40 + (int)PianoOffset.Y, 47, 144);
                    PianoKeys[2] = new Rectangle(110 + (int)PianoOffset.X + 200, 40 + (int)PianoOffset.Y, 47, 144);
                    PianoKeys[3] = new Rectangle(163 + (int)PianoOffset.X + 200, 40 + (int)PianoOffset.Y, 47, 144);
                    PianoKeys[4] = new Rectangle(217 + (int)PianoOffset.X + 200, 40 + (int)PianoOffset.Y, 47, 144);
                    PianoKeys[5] = new Rectangle(269 + (int)PianoOffset.X + 200, 40 + (int)PianoOffset.Y, 47, 144);
                    PianoKeys[6] = new Rectangle(323 + (int)PianoOffset.X + 200, 40 + (int)PianoOffset.Y, 47, 144);
                    PianoKeys[7] = new Rectangle(375 + (int)PianoOffset.X + 200, 40 + (int)PianoOffset.Y, 47, 144);
                    PianoKeys[8] = new Rectangle(428 + (int)PianoOffset.X + 200, 40 + (int)PianoOffset.Y, 47, 144);
                    PianoKeys[9] = new Rectangle(481 + (int)PianoOffset.X + 200, 40 + (int)PianoOffset.Y, 47, 144);
                    PianoKeys[10] = new Rectangle(536 + (int)PianoOffset.X + 200, 40 + (int)PianoOffset.Y, 47, 144);
                    PianoKeys[11] = new Rectangle(588 + (int)PianoOffset.X + 200, 40 + (int)PianoOffset.Y, 47, 144);
                    PianoKeys[12] = new Rectangle(641 + (int)PianoOffset.X + 200, 40 + (int)PianoOffset.Y, 47, 144);
                    PianoKeys[13] = new Rectangle(694 + (int)PianoOffset.X + 200, 40 + (int)PianoOffset.Y, 47, 144);
                    PianoKeys[14] = new Rectangle(746 + (int)PianoOffset.X + 200, 40 + (int)PianoOffset.Y, 47, 144);

                    if (Clave == 10) // Fa 
                    {
                        PianoKeys[7] = new Rectangle(6 + (int)PianoOffset.X + 200, 40 + (int)PianoOffset.Y, 47, 144);
                        PianoKeys[8] = new Rectangle(58 + (int)PianoOffset.X + 200, 40 + (int)PianoOffset.Y, 47, 144);
                        PianoKeys[9] = new Rectangle(110 + (int)PianoOffset.X + 200, 40 + (int)PianoOffset.Y, 47, 144);
                        PianoKeys[10] = new Rectangle(163 + (int)PianoOffset.X + 200, 40 + (int)PianoOffset.Y, 47, 144);
                        PianoKeys[11] = new Rectangle(217 + (int)PianoOffset.X + 200, 40 + (int)PianoOffset.Y, 47, 144);
                        PianoKeys[12] = new Rectangle(269 + (int)PianoOffset.X + 200, 40 + (int)PianoOffset.Y, 47, 144);
                        PianoKeys[13] = new Rectangle(323 + (int)PianoOffset.X + 200, 40 + (int)PianoOffset.Y, 47, 144);
                        PianoKeys[14] = new Rectangle(375 + (int)PianoOffset.X + 200, 40 + (int)PianoOffset.Y, 47, 144);

                    }

                }
                
                CurrentKey = -1;
                for (int i = 0; i < 15; i++)
                {
                    if (MouseRect.Intersects(PianoKeys[i]))
                    {
                        CurrentKey = i;
                        Window.Title += " Collision = " + CurrentKey.ToString();
                        break;
                    }
                }

                if (Clave == 10)
                {                
                    CurrentKey = -1;                
                    for (int i = 7; i < 15; i++)                
                    {                    
                        if (MouseRect.Intersects(PianoKeys[i]))                    
                        {                        
                            CurrentKey = i;                        
                            Window.Title += " Collision = " + CurrentKey.ToString();                        
                            break;                    
                        }                                
                    }
                }


                Rectangle FererRect =
                new Rectangle((int)FererPos.X - 25, (int)FererPos.Y - 35,
                (int)(Ferer.Width * 0.74F), (int)(Ferer.Height * 0.7F));

                Rectangle[] MMRect = new Rectangle[9];
                MMRect[0] = new Rectangle((int)MMPos.X - 29, (int)MMPos.Y - 27,
                (int)(MM.Width * 0.5F) + 4, (int)(MM.Height * 0.5F) + 8);

                MMRect[1] = new Rectangle((int)(MMPos.X - 18), (int)(MMPos.Y - 6),
                (int)(Pringles.Width * 0.12F) + 15, (int)(Pringles.Height * 0.12F) + 23);

                MMRect[2] = new Rectangle((int)(MMPos.X), (int)(MMPos.Y),
                (int)(DiamanteNegro.Width * 0.5F), (int)(DiamanteNegro.Height * 0.5F));

                MMRect[3] = new Rectangle((int)(MMPos.X - 22), (int)(MMPos.Y + 3),
                (int)(Coca.Width * 0.3F) + 13, (int)(Coca.Height * 0.3F));

                MMRect[4] = new Rectangle((int)(MMPos.X - 10), (int)(MMPos.Y - 20),
                (int)(Zombie.Width * 0.34) + 10, (int)(Zombie.Height * 0.4F));

                MMRect[5] = new Rectangle((int)MMPos.X - 10, (int)MMPos.Y - 20,
                (int)(Shrek.Width * 0.3F), (int)(Shrek.Height * 0.3F));

                MMRect[6] = new Rectangle((int)MMPos.X - 20, (int)MMPos.Y - 24,
                (int)(Gretchen.Width * 0.45F), (int)(Gretchen.Height * 0.45F));

                MMRect[7] = new Rectangle((int)(MMPos.X - 9), (int)(MMPos.Y) - 24,
                (int)(Pizza[0].Width * 0.43F), (int)(Pizza[0].Height * 0.37F));

                MMRect[8] = new Rectangle((int)(MMPos.X - 50), (int)(MMPos.Y) - 35,
                (int)(PacMan.Width * 0.4F) - 35, (int)(PacMan.Height * 0.4F));

                Rectangle RBRect = new Rectangle((int)RBPos.X - 8, (int)RBPos.Y - 10,
                (int)(RedBull.Width * 0.17F), (int)(RedBull.Height * 0.17F));


                // Ball and paddle collide?  Check rectangle intersection between objects
                Rectangle ballRect =
                    new Rectangle((int)ballPosition.X, (int)ballPosition.Y - 25,
                    ballSprite.Width, ballSprite.Height);

                Rectangle handRect =
                    new Rectangle((int)paddlePosition.X, (int)paddlePosition.Y,
                        paddleSprite.Width, paddleSprite.Height);

                Rectangle handRect2 =
                    new Rectangle((int)paddlePosition2.X, (int)paddlePosition2.Y,
                        paddleSprite.Width, paddleSprite.Height);

                if (MMRect[MMType].Intersects(MouseRect) && ((mouseState.RightButton == ButtonState.Pressed && PrevRightState == ButtonState.Released) || (mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released)) && (ShowMM))
                {

                    Bonus.Add(new BonusMsg(new Vector2(MMPos.X, MMPos.Y + 90) , "+40 pontos", 30, new Vector2(0, -35)));
                    Pontos += 40;

                    if (MMType == 6)
                    {
                        soundEngineGretchen.Stop();
                        soundEngine.Resume();
                        ShowMM = false;

                    }
                    else if (MMType == 4)
                    {
                        soundEngineScream.Stop();
                        ShowMM = false;
                    }
                    else if (MMType == 7) // pizza
                    {
                        if (PizzaTimer > 5)
                        {
                            PizzaStatus += 1;
                            PizzaTimer = 0;
                            Chomp.Play();
                        }

                        if (PizzaStatus > 4)
                        {
                            PizzaStatus = 0;
                            ShowMM = false;
                        }
                    }
                    else if (MMType == 3)
                    {
                        Burp.Play();
                        ShowMM = false;
                    }
                    else if (MMType == 1)
                    {
                        PringlesSnd.Play();
                        ShowMM = false;
                    }
                    else if (MMType == 8)
                    {
                        soundEnginePac.Stop();
                        soundEngine.Resume();
                        ShowMM = false;
                    }
                    else ShowMM = false;

                }

                if (RBRect.Intersects(MouseRect) && ((mouseState.RightButton == ButtonState.Pressed && PrevRightState == ButtonState.Released) || (mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released) ) && (ShowRB))
                {
                    RBStatus += 2;
                    ShowRB = false;
                    //MostraBonus = true;
                    //BonusType = 1;
                    //BonusTimer = 30;

                    Bonus.Add(new BonusMsg(RBPos, "Destreza +2", 30, new Vector2(0, -25)));

                    //BonusPosition = RBPos;
                }

                if (FererRect.Intersects(MouseRect) && ((mouseState.RightButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released) || (mouseState.LeftButton == ButtonState.Pressed && PrevLeftState == ButtonState.Released)) && (ShowFerer))
                {
                    Pontos += 120;                    
                    ShowFerer = false;
                    //MostraBonus = true; 
                    //BonusType = 2;
                    //BonusTimer = 30;
                    //BonusPosition = FererPos;

                    Bonus.Add(new BonusMsg(FererPos, "+120 pontos", 25, new Vector2(0, -25)));

                    soundEngineCoral.Stop();
                    soundEngine.Resume();
                    FererPos.Y = Rand.Next(450) + 200;
                }

                if (ShowMM)
                {
                    PontosPerdidos += 0.4f;

                    if (MMType == 8) PontosPerdidos += 0.6f; // pac-man

                    if (PontosPerdidos >= 1)
                    {
                        Pontos -= 1;
                        PontosPerdidos = PontosPerdidos - 1;
                    }

                    if (Pontos < 0) Pontos = 0;

                    if (MMType == 7) PizzaTimer += 1;
                    else if (MMType == 8)
                    {
                        if (PacSide == 0) MMPos.X += 4;
                        else if (PacSide == 1) MMPos.X -= 4;
                    }
                    else if (MMType == 5) // Shrek;
                    {
                        if (MMPosInc == 0) MMPos.X += 10;
                        else if (MMPosInc == 1) MMPos.X -= 10;

                        if (MMPos.X > 830) MMPosInc = 1;
                        if (MMPos.X < -80) MMPosInc = 0;

                        if (BowserDirecao == 0) BowserRot += 0.03f;
                        else if (BowserDirecao == 1) BowserRot -= 0.03f;
                        if (BowserRot > 0.25f) BowserDirecao = 1;
                        if (BowserRot < -0.25f) BowserDirecao = 0;
                    }

                    if (MMType == 8)
                    {

                        if (PacSide == 0 && MMPos.X > 980)
                        {
                            soundEnginePac.Stop();
                            soundEngine.Resume();
                            ShowMM = false;
                        }
                        else if (PacSide == 1 && MMPos.X < -180)
                        {
                            soundEnginePac.Stop();
                            soundEngine.Resume();
                            ShowMM = false;
                        }
                    }
                }

                if (Clave != 4 && Clave != 5 && Clave != 6 && Clave != 8)
                {
                    if (
                        ballRect.Intersects(handRect) && ballSpeed.Y > 0 && (mouseState.LeftButton == ButtonState.Pressed) && (CurrentKey == CurrentNote) && (LeftButton < 25)
                    || (ballRect.Intersects(handRect) && ballSpeed.Y > 0 && (CurrentKeyMIDI == CurrentNote)))
                    {

                        //SoundEffectInstance soundEngineInstance;
                        if (Clave == 1 || Clave == 9) noteSounds[CurrentNote + 14].Play(); // soundEngineInstance = noteSounds[CurrentNote+14].CreateInstance();                        
                        else if (Clave == 2 || Clave == 11) noteSounds[CurrentNote + 7].Play(); // soundEngineInstance = noteSounds[CurrentNote+7].CreateInstance();
                        else noteSounds[CurrentNote].Play(); //soundEngineInstance = noteSounds[CurrentNote].CreateInstance();

                        //soundEngineInstance.Volume = .7f;
                        //soundEngineInstance.Play(); 

                        ballSpeed.Y += 5 + Rand.Next(5);

                        if (ballSpeed.X < 0) ballSpeed.X -= 8;
                        else ballSpeed.X += 8;

                        // Send ball back up the screen
                        ballSpeed.Y *= -1;

                        int newNote = Rand.Next(15);

                        if (Clave == 3) newNote = Rand.Next(29);
                        else if (Clave == 1 && ClaveOriginal == 9)
                        {
                            int Acrescimo = (Pontos / 150) - 400;
                            if (Acrescimo > 15 - 7) Acrescimo = 15 -7;
                            newNote = Rand.Next(8) + Acrescimo-1;
                        }
                        else if (Clave == 9 || Clave == 10 || Clave == 11) newNote = Rand.Next(8);

                        if (Clave == 10) newNote += 7;

                        ballSprite = noteSprite[newNote];
                        CurrentNote = newNote;
                        Pontos += 50;

                        if (DIFICULDADE == 1) Pontos += 10;
                        else if (DIFICULDADE == 2) Pontos += 20;

                        if (ModoSurpresa) Surpresa();

                    }

                }
                else if (Clave == 4 && ballRect.Intersects(handRect) && ballSpeed.Y > 0)
                {
                    List<Pitch> pitches = new List<Pitch>(pitchesPressed.Keys);
                    pitches.Sort();

                    ///
                    List<Pitch> pitchesFiltradas = new List<Pitch>();
                    bool Repetida = false;

                    for (int i = 0; i < pitches.Count; ++i)
                    {
                        if (i == 0) pitchesFiltradas.Add(pitches[i]);
                        else
                        {
                            for (int k = 0; i - k > 0; ++k)
                            {
                                if (pitches[i].PositionInOctave() == pitches[k].PositionInOctave())
                                {
                                    Repetida = true;
                                }
                            }
                            if (Repetida == false) pitchesFiltradas.Add(pitches[i]);
                            Repetida = false;
                        }
                    }
                    pitchesFiltradas.Sort();

                    ///
                    List<Pitch> pitchesFiltradas2 = new List<Pitch>();
                    for (int i = 0; i < pitchesFiltradas.Count; ++i)
                    {

                        int p = (int)pitchesFiltradas[i];

                        while (p - (int)pitchesFiltradas[0] > 12)
                        {
                            p -= 12;
                        }

                        pitchesFiltradas2.Add((Midi.Pitch)p);

                    }
                    pitchesFiltradas2.Sort();

                    List<Chord> chords = Chord.FindMatchingChords(pitchesFiltradas2);
                    for (int i = 0; i < chords.Count; ++i)
                    {
                        Chord chord = chords[i];

                        if (chord.Root == CurrentChord.Root && chord.Pattern == CurrentChord.Pattern && halt == 0)
                        {
                            ballSpeed.Y += 5 + Rand.Next(5);

                            if (ballSpeed.X < 0) ballSpeed.X -= 8;
                            else ballSpeed.X += 8;
                            ballSpeed.Y *= -1;
                            ballSprite = AcordeMoldura;
                            Pontos += 50;

                            VerificaInversoes(chord, 1);                            
                            
                            string Acorde = "C";
                            string[] RootTipo = new string[] { "C", "Db", "C#", "D", "Eb", "E", "F", "F#", "G", "G#", "Ab", "A", "Bb", "B" };

                            if (DIFICULDADE == 0 && Pontos <= 1000)
                            {
                                string[] PatternTipo = new string[] { "", "m" };
                                Acorde = RootTipo[Rand.Next(14)] + PatternTipo[Rand.Next(2)];
                            }
                            else if (DIFICULDADE == 0 && Pontos > 1000)
                            {
                                string[] PatternTipo = new string[] { "", "m", "7" };
                                Acorde = RootTipo[Rand.Next(14)] + PatternTipo[Rand.Next(3)];
                            }
                            else if (DIFICULDADE == 1 && Pontos <= 1000)
                            {
                                string[] PatternTipo = new string[] { "", "m", "7", "aug", "dim", };
                                Acorde = RootTipo[Rand.Next(14)] + PatternTipo[Rand.Next(5)];
                            }
                            else if (DIFICULDADE == 1 && Pontos > 1000)
                            {
                                string[] PatternTipo = new string[] { "", "m", "7", "aug", "dim", "m7", "7M" };
                                Acorde = RootTipo[Rand.Next(14)] + PatternTipo[Rand.Next(7)];
                            }
                            else if (DIFICULDADE == 2 && Pontos <= 800)
                            {
                                string[] PatternTipo = new string[] { "", "m", "7", "aug", "dim", "m7", "7M", "dim7" };
                                Acorde = RootTipo[Rand.Next(14)] + PatternTipo[Rand.Next(8)];
                            }
                            else if (DIFICULDADE == 2 && Pontos > 800)
                            {
                                string[] PatternTipo = new string[] { "", "m", "7", "aug", "dim", "m7", "dim7", "Ø", "7M", "m7M" };
                                Acorde = RootTipo[Rand.Next(14)] + PatternTipo[Rand.Next(10)];
                            }

                            CurrentChord = new Chord(Acorde);

                            if (DIFICULDADE == 1) Pontos += 10;
                            else if (DIFICULDADE == 2) Pontos += 20;
                            if (ModoSurpresa) Surpresa();

                        }

                    }                
                    
                }
                else if (Clave == 5 && ballRect.Intersects(handRect) && ballSpeed.Y > 0)
                {
                    List<Pitch> pitches = new List<Pitch>(pitchesPressed.Keys);
                    pitches.Sort();

                    //
                    List<Pitch> pitchesFiltradas = new List<Pitch>();
                    bool Repetida = false;

                    for (int i = 0; i < pitches.Count; ++i)
                    {
                        if (i == 0) pitchesFiltradas.Add(pitches[i]);
                        else
                        {
                            for (int k = 0; i - k > 0; ++k)
                            {
                                if (pitches[i].PositionInOctave() == pitches[k].PositionInOctave())
                                {
                                    Repetida = true;
                                }
                            }
                            if (Repetida == false) pitchesFiltradas.Add(pitches[i]);
                            Repetida = false;
                        }
                    }
                    pitchesFiltradas.Sort();
                    //

                    List<Pitch> pitchesFiltradas2 = new List<Pitch>();
                    for (int i = 0; i < pitchesFiltradas.Count; ++i)
                    {

                        int p = (int)pitchesFiltradas[i];

                        while (p - (int)pitchesFiltradas[0] > 12)
                        {
                            p -= 12;
                        }

                        pitchesFiltradas2.Add((Midi.Pitch)p);

                    }
                    pitchesFiltradas2.Sort();

                    List<Chord> chords = Chord.FindMatchingChords(pitchesFiltradas2);
                    for (int i = 0; i < chords.Count; ++i)
                    {
                        Chord chord = chords[i];

                        if (chord == CurrentChord && halt == 0)
                        {
                            ballSpeed.Y += 5 + Rand.Next(5);

                            if (ballSpeed.X < 0) ballSpeed.X -= 8;
                            else ballSpeed.X += 8;
                            ballSpeed.Y *= -1;

                            Pontos += 50 + (DIFICULDADE * 10);

                            int newNote = 5 + Rand.Next(7);
                            Temp = Rand.Next(CifraBaixo[newNote - 5].Length);
                            CurrentChord = new Chord(AcordeBaixo[newNote - 5][Temp]);
                            ballSprite = noteSprite[newNote];
                            CurrentNote = newNote;

                            if (DIFICULDADE == 1) Pontos += 10;
                            else if (DIFICULDADE == 2) Pontos += 20;
                            if (ModoSurpresa) Surpresa();
                        }
                 
                    }                

                }
                else if (Clave == 6 && ballRect.Intersects(handRect) && ballSpeed.Y > 0)
                {
                    List<Pitch> pitches = new List<Pitch>(pitchesPressed.Keys);
                    pitches.Sort();

                    //
                    //
                    List<Pitch> pitchesFiltradas = new List<Pitch>();
                    bool Repetida = false;

                    for (int i = 0; i < pitches.Count; ++i)
                    {
                        if (i == 0) pitchesFiltradas.Add(pitches[i]);
                        else
                        {
                            for (int k = 0; i - k > 0; ++k)
                            {
                                if (pitches[i].PositionInOctave() == pitches[k].PositionInOctave())
                                {
                                    Repetida = true;
                                }
                            }
                            if (Repetida == false) pitchesFiltradas.Add(pitches[i]);
                            Repetida = false;
                        }
                    }
                    pitchesFiltradas.Sort();
                    //

                    List<Pitch> pitchesFiltradas2 = new List<Pitch>();
                    for (int i = 0; i < pitchesFiltradas.Count; ++i)
                    {

                        int p = (int)pitchesFiltradas[i];

                        while (p - (int)pitchesFiltradas[0] > 12)
                        {
                            p -= 12;
                        }

                        pitchesFiltradas2.Add((Midi.Pitch)p);

                    }
                    pitchesFiltradas2.Sort();

                    List<Chord> chords = Chord.FindMatchingChords(pitchesFiltradas2);
                    for (int i = 0; i < chords.Count; ++i)
                    {
                        Chord chord = chords[i];

                        if (chord.Root == CurrentChord.Root && chord.Pattern == CurrentChord.Pattern && halt == 0)
                        {
                            ballSpeed.Y += 5; 

                            if (ballSpeed.X < 0) ballSpeed.X -= 7;
                            else ballSpeed.X += 7;
                            ballSpeed.Y *= -1;
                            Pontos += 50;

                            VerificaInversoes(chord, 1);

                            int Modo = Rand.Next(2);
                            int Tom = Rand.Next(12);
                            int Acorde = 0;
                            if (DIFICULDADE == 0) Acorde = Rand.Next(AcordeGrausFacil[0].Length - 1);
                            else if (DIFICULDADE == 1) Acorde = Rand.Next(AcordeGrausMedio[0].Length - 1);
                            else if (DIFICULDADE == 2) Acorde = Rand.Next(AcordeGrausDificil[0].Length - 1);
                            Clave6Acorde = Tonalidades[Modo][Tom] + ": " + AcordeGrausDificil[Modo][Acorde];
                            string NewChord = "";
                            if (Modo == 0) NewChord = EscalasMaiores[Tom][AcordeGrausIndex[Acorde]] + AcordeGrausPatterns[Modo][Acorde];
                            else if (Modo == 1) NewChord = EscalasMenoresNatural[Tom][AcordeGrausIndex[Acorde]] + AcordeGrausPatterns[Modo][Acorde];
                            CurrentChord = new Chord(NewChord);

                            if (DIFICULDADE == 1) Pontos += 10;
                            else if (DIFICULDADE == 2) Pontos += 20;
                            if (ModoSurpresa) Surpresa();

                        }

                    }

                }
                else if (Clave == 8 &&  ( ballRect.Intersects(handRect) || ballRect.Intersects(handRect2) ))
                {

                    bool Intersecao = false;
                    int Jogador = 1;
                    List<Pitch> pitches = new List<Pitch>(pitchesPressed.Keys);

                    if (ballRect.Intersects(handRect) && ballSpeed.Y > 0)
                    {
                        pitches = new List<Pitch>(pitchesPressed.Keys);
                        //Console.WriteLine("HAND_RECT");
                        Intersecao = true;
                        Jogador = 1;
                    }
                    else if (ballRect.Intersects(handRect2) && ballSpeed.Y < 0)
                    {
                        pitches = new List<Pitch>(pitchesPressed2.Keys);
                        //Console.WriteLine("HAND_RECT2");
                        Intersecao = true;
                        Jogador = 2;
                    }

                    if (Intersecao)
                    {
                        pitches.Sort();


                        //
                        List<Pitch> pitchesFiltradas = new List<Pitch>();
                        bool Repetida = false;

                        for (int i = 0; i < pitches.Count; ++i)
                        {
                            if (i == 0) pitchesFiltradas.Add(pitches[i]);
                            else
                            {
                                for (int k = 0; i - k > 0; ++k)
                                {
                                    if (pitches[i].PositionInOctave() == pitches[k].PositionInOctave())
                                    {
                                        Repetida = true;
                                    }
                                }
                                if (Repetida == false) pitchesFiltradas.Add(pitches[i]);
                                Repetida = false;
                            }
                        }
                        pitchesFiltradas.Sort();
                        //

                        List<Pitch> pitchesFiltradas2 = new List<Pitch>();
                        for (int i = 0; i < pitchesFiltradas.Count; ++i)
                        {

                            int p = (int)pitchesFiltradas[i];

                            while (p - (int)pitchesFiltradas[0] > 12)
                            {
                                p -= 12;
                            }

                            pitchesFiltradas2.Add((Midi.Pitch)p);

                        }
                        pitchesFiltradas2.Sort();

                        List<Chord> chords = Chord.FindMatchingChords(pitchesFiltradas2);
                        for (int i = 0; i < chords.Count; ++i)
                        {
                            Chord chord = chords[i];

                            if (chord.Root == CurrentChord.Root && chord.Pattern == CurrentChord.Pattern && halt == 0)
                            {
                                if (ballSpeed.Y > 0) ballSpeed.Y += 3 + Rand.Next(2);
                                else if (ballSpeed.Y < 0) ballSpeed.Y -= 3 + Rand.Next(2);

                                if (ballSpeed.X < 0) ballSpeed.X -= 8;
                                else ballSpeed.X += 8;
                                ballSpeed.Y *= -1;
                                ballSprite = AcordeMoldura;
                                
                                if (Jogador == 1) Pontos += 50;
                                else if (Jogador == 2) Pontos2 += 50;

                                VerificaInversoes(chord, Jogador);

                                string Acorde = "C";
                                string[] RootTipo = new string[] { "C", "Db", "C#", "D", "Eb", "E", "F", "F#", "G", "G#", "Ab", "A", "Bb", "B" };

                                if (DIFICULDADE == 0 && (Pontos + Pontos2) <= 1000)
                                {
                                    string[] PatternTipo = new string[] { "", "m" };
                                    Acorde = RootTipo[Rand.Next(14)] + PatternTipo[Rand.Next(2)];
                                }
                                else if (DIFICULDADE == 0 && (Pontos + Pontos2) > 1000)
                                {
                                    string[] PatternTipo = new string[] { "", "m", "7" };
                                    Acorde = RootTipo[Rand.Next(14)] + PatternTipo[Rand.Next(3)];
                                }
                                else if (DIFICULDADE == 1 && (Pontos + Pontos2) <= 1000)
                                {
                                    string[] PatternTipo = new string[] { "", "m", "7", "aug", "dim", };
                                    Acorde = RootTipo[Rand.Next(14)] + PatternTipo[Rand.Next(5)];
                                }
                                else if (DIFICULDADE == 1 && (Pontos + Pontos2) > 1000)
                                {
                                    string[] PatternTipo = new string[] { "", "m", "7", "aug", "dim", "m7", "7M" };
                                    Acorde = RootTipo[Rand.Next(14)] + PatternTipo[Rand.Next(7)];
                                }
                                else if (DIFICULDADE == 2 && (Pontos + Pontos2) <= 800)
                                {
                                    string[] PatternTipo = new string[] { "", "m", "7", "aug", "dim", "m7", "7M", "dim7" };
                                    Acorde = RootTipo[Rand.Next(14)] + PatternTipo[Rand.Next(8)];
                                }
                                else if (DIFICULDADE == 2 && (Pontos + Pontos2) > 800)
                                {
                                    string[] PatternTipo = new string[] { "", "m", "7", "aug", "dim", "m7", "dim7", "Ø", "7M", "m7M" };
                                    Acorde = RootTipo[Rand.Next(14)] + PatternTipo[Rand.Next(10)];
                                }

                                CurrentChord = new Chord(Acorde);


                            }

                        }

                    }
                }


                int maxPadX = graphics.GraphicsDevice.Viewport.Width - paddleSprite.Width;

                // Update the paddle's position                

                if (CONTROLE == 0 && Clave != 8)
                {
                    if (keyState.IsKeyDown(Keys.Right))
                    {
                        if (paddlePosition.X < maxPadX) paddlePosition.X += (5 + RBStatus);
                    }
                    else if (keyState.IsKeyDown(Keys.Left))
                    {
                        if (paddlePosition.X > 0) paddlePosition.X -= (5 + RBStatus);
                    }


                    /*
                    if (RBStatus == 6) // ASAS
                    {
                        if (keyState.IsKeyDown(Keys.Up))
                        {
                            if (paddlePosition.Y > 185) paddlePosition.Y -= 5;
                        }
                        else if (keyState.IsKeyDown(Keys.Down))
                        {
                            if (paddlePosition.Y <= 491) paddlePosition.Y += 5;
                        }

                        if (paddlePosition.Y > 492) paddlePosition.Y = 492;

                    }
                     */


                }
                else if (CONTROLE == 1 && Clave != 8)
                {                    
                    if (mouseState.X - paddlePosition.X > 5 || mouseState.X - paddlePosition.X < -5  )
                    {

                        if (paddlePosition.X < mouseState.X && paddlePosition.X < maxPadX)
                        {
                            paddlePosition.X += (5 + RBStatus/2);
                        }
                        else if (paddlePosition.X > mouseState.X && paddlePosition.X > 0)
                        {
                            paddlePosition.X -= (5 + RBStatus/2);
                        }
                    }
                }
                else if (Clave == 8 && !MostrarTeclado)
                {

                    List<Pitch> pitches = new List<Pitch>(pitchesPressedController.Keys);
                    pitches.Sort();
                    for (int i = 0; i < pitches.Count; ++i)
                    {
                        Pitch pitch = pitches[i];
                        if (pitch == Pitch.DSharp4)
                        {
                            if (paddlePosition.X < maxPadX) paddlePosition.X += (5);
                        }
                        if (pitch == Pitch.CSharp4)
                        {
                            if (paddlePosition.X > 0) paddlePosition.X -= (5);
                        }

                    }

                    // p2
                    pitches = new List<Pitch>(pitchesPressedController.Keys);
                    pitches.Sort();
                    
                    for (int i = 0; i < pitches.Count; ++i)
                    {
                        Pitch pitch = pitches[i];

                        if (pitch == Pitch.DSharp2)
                        {
                            if (paddlePosition2.X < maxPadX) paddlePosition2.X += (5);
                        }
                        if (pitch == Pitch.CSharp2)
                        {
                            if (paddlePosition2.X > 0) paddlePosition2.X -= (5);
                        }

                    }                                        
                    
                    if (keyState.IsKeyDown(Keys.Right))
                    {
                        if (paddlePosition.X < maxPadX) paddlePosition.X += (8);
                    }
                    else if (keyState.IsKeyDown(Keys.Left))
                    {
                        if (paddlePosition.X > 0) paddlePosition.X -= (8);
                    }
                    
                    if (keyState.IsKeyDown(Keys.C))
                    {
                        if (paddlePosition2.X < maxPadX) paddlePosition2.X += (8);
                    }
                    else if (keyState.IsKeyDown(Keys.Z))
                    {
                        if (paddlePosition2.X > 0) paddlePosition2.X -= (8);
                    }

                }
                VerificaRecorde();

                this.IsMouseVisible = true;
            }
            MouseState mouseStateT = Mouse.GetState();
            PrevLeftState = mouseStateT.LeftButton;
            PrevRightState = mouseStateT.RightButton;
            prevkeyState = Keyboard.GetState();
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            if (MENU == 1)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                if (SUBMENU == 0)
                {
                    switch (MenuKey)
                    {
                        case -1:
                            spriteBatch.Draw(Menu[0], new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);
                            break;

                        case 0:
                            spriteBatch.Draw(Menu[1], new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);
                            break;

                        case 1:
                            spriteBatch.Draw(Menu[2], new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);
                            break;

                        case 2:
                            spriteBatch.Draw(Menu[3], new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);
                            break;

                        case 3:
                            spriteBatch.Draw(Menu[4], new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);
                            break;

                        case 4:
                            spriteBatch.Draw(Menu[5], new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);
                            break;

                        case 5:
                            spriteBatch.Draw(Menu[6], new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);
                            break;

                        case 6:
                            spriteBatch.Draw(Menu[7], new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);
                            break;
                    }
                    
                    spriteBatch.DrawString(Menufont, "Jogar", new Vector2(450, 205), Color.Red);
                    spriteBatch.DrawString(Menufont, "Instruções", new Vector2(450, 260), Color.Red);
                    spriteBatch.DrawString(Menufont, "Configurações", new Vector2(450, 315), Color.Red);
                    spriteBatch.DrawString(Menufont, "Recordes", new Vector2(450, 370), Color.Red);
                    spriteBatch.DrawString(Menufont, "Minigames", new Vector2(450, 425), Color.Red);
                    spriteBatch.DrawString(Menufont, "Sair", new Vector2(450, 480), Color.Red);
                    spriteBatch.DrawString(Menufont, "Sobre", new Vector2(450, 535), Color.Red);
                   
                }
                else if (SUBMENU == 1)
                {
                    switch (MenuKey)
                    {
                        case -1:
                        case 0:
                            spriteBatch.Draw(Menu[0], new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);
                            break;

                        case 1:
                            spriteBatch.Draw(Menu[2], new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);
                            break;

                        case 2:
                            spriteBatch.Draw(Menu[3], new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);
                            break;

                        case 3:
                            spriteBatch.Draw(Menu[4], new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);
                            break;

                        case 4:
                            spriteBatch.Draw(Menu[5], new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);
                            break;

                        case 5:
                            spriteBatch.Draw(Menu[6], new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);
                            break;

                        case 6:
                            spriteBatch.Draw(Menu[7], new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);
                            break;


                    }

                    spriteBatch.DrawString(Menufont, "Modo de controle", new Vector2(450, 260), Color.Red);
                    spriteBatch.DrawString(Menufont, "Mudar entrada MIDI", new Vector2(450, 315), Color.Red);

                    if (InputDevice.InstalledDevices.Count == 0)
                    {
                        if (CONTROLE == 1) spriteBatch.DrawString(font, "MIDI IN: Nenhuma entrada encontrada!", new Vector2(445, 220), Color.Red);
                    }
                    else
                    {
                        MidiIn = InputDevice.InstalledDevices[CurrentMIDI];
                        if (CONTROLE == 1) spriteBatch.DrawString(font, "MIDI IN:  " + MidiIn.Name, new Vector2(445, 220), Color.Red);                        
                    }

                    if (Clave == 3 || Clave == 4 || Clave == 5 || Clave == 6 || Clave == 8)
                    {
                        spriteBatch.Draw(m_backGroundColorRed, new Rectangle(422, 127, 346, 52), Color.White);
                        spriteBatch.Draw(m_backGroundColorWhite, new Rectangle(425, 130, 340, 46), Color.White);
                        spriteBatch.DrawString(font, "Este modo de jogo requer um teclado\nMIDI para funcionar corretamente!", new Vector2(435, 130), Color.Red);
                    }

                    if (CONTROLE == 0)spriteBatch.DrawString(font, "Controle: Teclado + Mouse", new Vector2(445, 190), Color.Red);
                    else if (CONTROLE == 1) spriteBatch.DrawString(font, "Controle: Teclado MIDI + Mouse", new Vector2(445, 190), Color.Red);


                    spriteBatch.DrawString(Menufont, "Dificuldade", new Vector2(450, 370), Color.Red);
                    spriteBatch.DrawString(Menufont, "Modo de jogo", new Vector2(450, 425), Color.Red);
                    if (Distracoes) spriteBatch.DrawString(Menufont, "Distrações", new Vector2(450, 480), Color.Red);
                    else if (!Distracoes) spriteBatch.DrawString(Menufont, "Sem distrações", new Vector2(450, 480), Color.Red);                    
                    spriteBatch.DrawString(Menufont, "Voltar", new Vector2(450, 535), Color.Red);


                    if (DIFICULDADE == 0 )spriteBatch.Draw(Turtle, new Vector2(625, 350), null, Color.White, 0, new Vector2(0, 0), 0.30f, SpriteEffects.None, 0);
                    else if (DIFICULDADE == 1) spriteBatch.Draw(TMNT, new Vector2(625, 350), null, Color.White, 0, new Vector2(0, 0), 0.35f, SpriteEffects.None, 0);
                    else if (DIFICULDADE == 2) spriteBatch.Draw(Ninja, new Vector2(640, 350), null, Color.White, 0, new Vector2(0, 0), 0.80f, SpriteEffects.None, 0);

                    if (Clave == 0) spriteBatch.Draw(ClaveFa, new Vector2(655, 430), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    else if (Clave == 1) spriteBatch.Draw(ClaveSol, new Vector2(655, 430), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    else if (Clave == 2) spriteBatch.Draw(ClaveDo, new Vector2(655, 430), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    else if (Clave == 3) spriteBatch.Draw(ClaveSolFa, new Vector2(655, 430), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    else if (Clave == 4) spriteBatch.Draw(ClaveAcordes, new Vector2(655, 430), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    else if (Clave == 5) spriteBatch.Draw(ClaveBaixoCifrado, new Vector2(655, 430), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    else if (Clave == 6) spriteBatch.Draw(ClaveHarmonia, new Vector2(655, 430), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    else if (Clave == 7) spriteBatch.Draw(ClaveInterrogacao, new Vector2(655, 430), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    else if (Clave == 8) spriteBatch.Draw(Clave2P, new Vector2(655, 430), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    else if (Clave == 9) spriteBatch.Draw(ClaveSolMetade, new Vector2(655, 430), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    else if (Clave == 10) spriteBatch.Draw(ClaveFaMetade, new Vector2(655, 430), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    else if (Clave == 11) spriteBatch.Draw(ClaveDoMetade, new Vector2(655, 430), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                }
                else if (SUBMENU == 2)
                {
                    spriteBatch.Draw(MenuInstrucoes, new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);

                    if (InstrucaoPagina == 0)
                    {
                        spriteBatch.DrawString(Menufont, "Instruções", new Vector2(300, 210), Color.Red);
                        spriteBatch.DrawString(font, "\n                             (modo Teclado + Mouse)", new Vector2(300, 200), Color.Red);
                        spriteBatch.DrawString(font, "Clique na nota correta quando esta estiver ao\nalcance da  mão e  cuide das  distrações que\ncomprometem o rendimento do estudo.", new Vector2(300, 260), Color.Red);
                        
                        spriteBatch.DrawString(Menufont, "Controle", new Vector2(300, 332), Color.Red);
                        spriteBatch.DrawString(font, "\n\n\nSetas direcionais:  movimentar mão.\nBotão esquerdo do mouse:  tocar piano, \natacar répteis e apertar botões.\nBotão direito do mouse: comer chocolates e \nbatatinhas, beber refrigerantes e energéticos,\npegar objetos, dispensar barangas,\nrecolher cabeças voadoras e retirar de\ncirculação discos de qualidade duvidosa.", new Vector2(300, 315), Color.Red);
                    }
                    else if (InstrucaoPagina == 1)
                    {
                        spriteBatch.DrawString(Menufont, "Instruções", new Vector2(300, 210), Color.Red);
                        spriteBatch.DrawString(font, "\n                             (com Teclado MIDI)", new Vector2(300, 200), Color.Red);
                        spriteBatch.DrawString(font, "Execute a nota (ou acorde) no teclado quando \nesta estiver ao alcance  da mão. A posição da \nmão acompanha o ponteiro do mouse. \n\nOs acordes  não devem conter dobramentos \ne  podem  ser  executados  em  qualquer \ninversão.", new Vector2(300, 260), Color.Red);
                        spriteBatch.DrawString(Menufont, "Itens", new Vector2(300, 430), Color.Red);
                        spriteBatch.DrawString(font, "Red Bull:  aumenta a velocidade da mão\nCabeças voadoras:  valem 120 pontos\nDemais objetos:  retiram pontos do jogador", new Vector2(300, 480), Color.Red); 
                    }
                    else if (InstrucaoPagina == 2)
                    {
                        spriteBatch.DrawString(Menufont, "Instruções", new Vector2(300, 210), Color.Red);
                        spriteBatch.DrawString(font, "\n                             (para cantar)", new Vector2(300, 200), Color.Red);
                        spriteBatch.DrawString(font, "Comece por quebrar os espelhos de sua casa, \ndeixe cair os braços, olhe vagamente a parede, \nesqueça. Cante uma nota só, escute por dentro. \nSe  ouvir (mas isto  acontecerá muito  depois) \nalgo como uma  paisagem afundada  no medo, \ncom fogueiras entre as pedras, com  silhuetas \nseminuas  de cócoras, acho que  estará bem \nencaminhado, e do mesmo modo se ouvir um \nrio por onde descem barcos pintados de amarelo \ne preto, se ouvir um gosto de pão, um tato de \ndedos, uma sombra de cavalo. Depois compre \ncadernos de solfejo e uma casaca e por favor \nnão cante pelo nariz e deixe Schumann em paz.", new Vector2(300, 260), Color.Red);
                        spriteBatch.DrawString(font, "(Cortázar)", new Vector2(640, 550), Color.Red);
                    }

                }
                else if (SUBMENU == 3)
                {
                    spriteBatch.Draw(MenuInstrucoes, new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);

                    if (InstrucaoPagina == 0)
                    {
                        spriteBatch.DrawString(Menufont, "Sobre Fong! (v1.0)", new Vector2(315, 195), Color.Red);
                        spriteBatch.DrawString(font, "\nVergonhosamente desenvolvido (sem aporte \nde dinheiro público) pela TDAH GAMES entre \nabril/outubro de 2011. A maioria dos gráficos \ne sons foram roubados da Internet e editados \ncom Paint, GIMP 2 e Audacity.\n\nProgramado por Fernando Rauber em C#\nutilizando o Framework XNA e a biblioteca\nmidi-dot-net. Todos os bugs são culpa\nda Microsoft.\n\nOs direitos de imagem da Gretchen foram \ngraciosamente cedidos pela produtora\nBrasileirinhas.", new Vector2(315, 230), Color.Red);
                    }
                    else if (InstrucaoPagina == 1)
                    {
                        spriteBatch.DrawString(Menufont, "Sobre o autor", new Vector2(315, 210), Color.Red);
                        Rectangle linha = new Rectangle(340, 279, 104, 3);
                        spriteBatch.Draw(m_backGroundColorRed, linha, Color.White);
                        spriteBatch.DrawString(font, "\nO pauloleminski fernando\né um cachorro louco\nque deve ser morto\na pau e pedra\na fogo a pique\nsenão é bem capaz\no filhadaputa\nde fazer chover\nem nosso piquenique\n\n\nContato: bocadito84@hotmail.com", new Vector2(315, 245), Color.Red);
                        

                        
                    }
                    else if (InstrucaoPagina == 2)
                    { 
                        spriteBatch.DrawString(Menufont, "Agradecimentos", new Vector2(315, 210), Color.Red);
                        spriteBatch.DrawString(font, "", new Vector2(315, 245), Color.Red);
                        spriteBatch.DrawString(font, "", new Vector2(315, 245), Color.Red); 

                    }

                }
                else if (SUBMENU == 4)
                {
                    spriteBatch.Draw(MenuInstrucoes2, new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);
                    MouseState mouseState3 = Mouse.GetState();
                    Window.Title = "mouseState = " + mouseState3.X.ToString() + ", " + mouseState3.Y.ToString();

                    if (InstrucaoPagina == 0)
                    {
                        spriteBatch.Draw(ClaveSolMetade, new Vector2(290, 215), null, Color.White, 0, new Vector2(0, 0), 0.9f, SpriteEffects.None, 0);
                        spriteBatch.Draw(ClaveSol, new Vector2(290, 275), null, Color.White, 0, new Vector2(0, 0), 0.9f, SpriteEffects.None, 0);
                        spriteBatch.Draw(ClaveFaMetade, new Vector2(290, 340), null, Color.White, 0, new Vector2(0, 0), 0.9f, SpriteEffects.None, 0);
                        spriteBatch.Draw(ClaveFa, new Vector2(290, 395), null, Color.White, 0, new Vector2(0, 0), 0.9f, SpriteEffects.None, 0);
                        spriteBatch.Draw(ClaveDoMetade, new Vector2(290, 450), null, Color.White, 0, new Vector2(0, 0), 0.9f, SpriteEffects.None, 0);
                        spriteBatch.Draw(ClaveDo, new Vector2(290, 515), null, Color.White, 0, new Vector2(0, 0), 0.9f, SpriteEffects.None, 0);
                        spriteBatch.Draw(Turtle, new Vector2(350, 150), null, Color.White, 0, new Vector2(0, 0), 0.30f, SpriteEffects.None, 0);
                        spriteBatch.Draw(TMNT, new Vector2(490, 142), null, Color.White, 0, new Vector2(0, 0), 0.35f, SpriteEffects.None, 0);
                        spriteBatch.Draw(Ninja, new Vector2(630, 143), null, Color.White, 0, new Vector2(0, 0), 0.80f, SpriteEffects.None, 0);

                        // tartagura
                        spriteBatch.DrawString(font, Recordes[9], new Vector2(395, 235), Color.Red);
                        if (TryToParse(Recordes[9]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(455, 232), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[1], new Vector2(395, 295), Color.Red);
                        if (TryToParse(Recordes[1]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(455, 292), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[10], new Vector2(395, 355), Color.Red);
                        if (TryToParse(Recordes[10]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(455, 352), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[0], new Vector2(395, 415), Color.Red);
                        if (TryToParse(Recordes[0]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(455, 412), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[11], new Vector2(395, 475), Color.Red);
                        if (TryToParse(Recordes[11]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(455, 472), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[2], new Vector2(395, 535), Color.Red);
                        if (TryToParse(Recordes[2]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(455, 532), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);

                        // tartaruga ninja
                        spriteBatch.DrawString(font, Recordes[20], new Vector2(515, 235), Color.Red);
                        if (TryToParse(Recordes[20]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(575, 232), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[12], new Vector2(515, 295), Color.Red);
                        if (TryToParse(Recordes[12]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(575, 292), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[21], new Vector2(515, 355), Color.Red);
                        if (TryToParse(Recordes[21]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(575, 352), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[11], new Vector2(515, 415), Color.Red);
                        if (TryToParse(Recordes[11]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(575, 412), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[22], new Vector2(515, 475), Color.Red);
                        if (TryToParse(Recordes[22]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(575, 472), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[13], new Vector2(515, 535), Color.Red);
                        if (TryToParse(Recordes[13]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(575, 532), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);

                        // ninja
                        spriteBatch.DrawString(font, Recordes[31], new Vector2(635, 235), Color.Red);
                        if (TryToParse(Recordes[31]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(695, 232), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[23], new Vector2(635, 295), Color.Red);
                        if (TryToParse(Recordes[23]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(695, 292), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[32], new Vector2(635, 355), Color.Red);
                        if (TryToParse(Recordes[32]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(695, 352), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[22], new Vector2(635, 415), Color.Red);
                        if (TryToParse(Recordes[22]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(695, 412), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[33], new Vector2(635, 475), Color.Red);
                        if (TryToParse(Recordes[33]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(695, 472), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[24], new Vector2(635, 535), Color.Red);
                        if (TryToParse(Recordes[24]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(695, 532), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                    }
                    else if (InstrucaoPagina == 1)
                    {
                        spriteBatch.Draw(ClaveSolFa, new Vector2(290, 215), null, Color.White, 0, new Vector2(0, 0), 0.9f, SpriteEffects.None, 0);
                        spriteBatch.Draw(ClaveAcordes, new Vector2(290, 275), null, Color.White, 0, new Vector2(0, 0), 0.9f, SpriteEffects.None, 0);
                        spriteBatch.Draw(ClaveBaixoCifrado, new Vector2(290, 335), null, Color.White, 0, new Vector2(0, 0), 0.9f, SpriteEffects.None, 0);
                        spriteBatch.Draw(ClaveHarmonia, new Vector2(290, 395), null, Color.White, 0, new Vector2(0, 0), 0.9f, SpriteEffects.None, 0);
                        spriteBatch.Draw(ClaveInterrogacao, new Vector2(290, 455), null, Color.White, 0, new Vector2(0, 0), 0.9f, SpriteEffects.None, 0);
                        spriteBatch.Draw(Clave2P, new Vector2(290, 515), null, Color.White, 0, new Vector2(0, 0), 0.9f, SpriteEffects.None, 0);

                        spriteBatch.Draw(Turtle, new Vector2(350, 150), null, Color.White, 0, new Vector2(0, 0), 0.30f, SpriteEffects.None, 0);
                        spriteBatch.Draw(TMNT, new Vector2(490, 142), null, Color.White, 0, new Vector2(0, 0), 0.35f, SpriteEffects.None, 0);
                        spriteBatch.Draw(Ninja, new Vector2(630, 143), null, Color.White, 0, new Vector2(0, 0), 0.80f, SpriteEffects.None, 0);

                        // tartagura
                        spriteBatch.DrawString(font, Recordes[3], new Vector2(395, 235), Color.Red);
                        if (TryToParse(Recordes[3]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(455, 232), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[4], new Vector2(395, 295), Color.Red);
                        if (TryToParse(Recordes[4]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(455, 292), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[5], new Vector2(395, 355), Color.Red);
                        if (TryToParse(Recordes[5]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(455, 352), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[6], new Vector2(395, 415), Color.Red);
                        if (TryToParse(Recordes[6]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(455, 412), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[7], new Vector2(395, 475), Color.Red);
                        if (TryToParse(Recordes[7]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(455, 472), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[8], new Vector2(395, 535), Color.Red);
                        if (TryToParse(Recordes[8]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(455, 532), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);

                        // tartaruga ninja
                        spriteBatch.DrawString(font, Recordes[3+11], new Vector2(515, 235), Color.Red);
                        if (TryToParse(Recordes[3+11]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(575, 232), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[4+11], new Vector2(515, 295), Color.Red);
                        if (TryToParse(Recordes[4+11]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(575, 292), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[5+11], new Vector2(515, 355), Color.Red);
                        if (TryToParse(Recordes[5+11]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(575, 352), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[6+11], new Vector2(515, 415), Color.Red);
                        if (TryToParse(Recordes[6+11]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(575, 412), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[7+11], new Vector2(515, 475), Color.Red);
                        if (TryToParse(Recordes[7+11]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(575, 472), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[8+11], new Vector2(515, 535), Color.Red);
                        if (TryToParse(Recordes[8+11]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(575, 532), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);

                        // ninja
                        spriteBatch.DrawString(font, Recordes[3+22], new Vector2(635, 235), Color.Red);
                        if (TryToParse(Recordes[3+22]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(695, 232), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[4+22], new Vector2(635, 295), Color.Red);
                        if (TryToParse(Recordes[4+22]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(695, 292), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[5+22], new Vector2(635, 355), Color.Red);
                        if (TryToParse(Recordes[5+22]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(695, 352), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[6+22], new Vector2(635, 415), Color.Red);
                        if (TryToParse(Recordes[6+22]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(695, 412), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[7+22], new Vector2(635, 475), Color.Red);
                        if (TryToParse(Recordes[7+22]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(695, 472), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, Recordes[8+22], new Vector2(635, 535), Color.Red);
                        if (TryToParse(Recordes[8+22]) > 3250) spriteBatch.Draw(Trofeu, new Vector2(695, 532), null, Color.White, 0, new Vector2(0, 0), 0.08f, SpriteEffects.None, 0);

                        
                    }
                    else if (InstrucaoPagina == 2)
                    {
                        spriteBatch.DrawString(Menufont, "Minigames", new Vector2(300, 200), Color.Red);
                    
                        spriteBatch.DrawString(font, "Homem-palito: " + FacaRecorde + " facadas", new Vector2(300, 240), Color.Red);
                        spriteBatch.DrawString(font, "Escalator: " + EscalatorRecorde + " pontos", new Vector2(300, 280), Color.Red);
                        spriteBatch.DrawString(font, "Tonalidades: " + TomRecorde + " respostas", new Vector2(300, 320), Color.Red);
                        spriteBatch.DrawString(font, "Bambu: " + BambuRecorde + " pontos", new Vector2(300, 360), Color.Red);
                        spriteBatch.DrawString(font, "Link: " + LinkRecorde + " pontos", new Vector2(300, 400), Color.Red);
                        spriteBatch.DrawString(font, "Ah, não! o datilografista virtuoso : " + PalavraRecorde + " pontos", new Vector2(300, 440), Color.Red);
                        spriteBatch.DrawString(Menufont, "Total de Pontos: " + RecordeTotal(), new Vector2(300, 470), Color.Red);
                        spriteBatch.DrawString(Menufont, "Troféus: " + ContaTrofeu(), new Vector2(300, 520), Color.Red);
                    }


                }
                else if (SUBMENU == 5) // Minigames
                {                    
                    switch (MenuKey)
                    {
                        case -1:
                            spriteBatch.Draw(Menu[0], new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);
                            break;

                        case 0:
                            spriteBatch.Draw(Menu[1], new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);
                            break;

                        case 2:
                            spriteBatch.Draw(Menu[3], new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);
                            break;

                        case 4:
                            spriteBatch.Draw(Menu[5], new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);
                            break;

                        case 6:
                            spriteBatch.Draw(Menu[7], new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);
                            break;

                        default:
                            spriteBatch.Draw(Menu[0], new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);
                            break;


                    }

                    spriteBatch.DrawString(Menufont, "Homem-Palito VS.", new Vector2(450, 205), Color.Red);
                    spriteBatch.DrawString(Menufont, "Violinista açougueiro", new Vector2(450, 245), Color.Red);
                    spriteBatch.DrawString(Menufont, "Escalator VS.", new Vector2(450, 315), Color.Red);
                    spriteBatch.DrawString(Menufont, "The Outsiders", new Vector2(450, 355), Color.Red);
                    spriteBatch.DrawString(Menufont, "Universtário VS.", new Vector2(450, 425), Color.Red);
                    spriteBatch.DrawString(Menufont, "Bambu", new Vector2(450, 465), Color.Red);
                    spriteBatch.DrawString(Menufont, "Voltar", new Vector2(450, 535), Color.Red);


                }
                else if (SUBMENU == 6)
                {
                    spriteBatch.Draw(MenuInstrucoes, new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(Menufont, "Acesso não permitido!", new Vector2(315, 300), Color.Red);
                    spriteBatch.DrawString(font, "\nSão necessários no mínimo um total de\n2500 pontos para abrir esta opção. Verifique \nsua pontuação clicando em \"Recordes\"", new Vector2(315, 335), Color.Red);

                }
                else if (SUBMENU == 11)
                {
                    spriteBatch.Draw(MenuInstrucoes, new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);
                    
                    String mensagem = "             Selecione um modo de jogo ";

                    if (ModoAtual == 0) mensagem = "Leitura na clave de sol (uma oitava). A dificuldade\naltera a velocidade inicial.";
                    else if (ModoAtual == 1) mensagem = "Leitura na clave de sol (duas oitavas). A dificuldade\naltera a velocidade inicial.";
                    else if (ModoAtual == 2) mensagem = "Leitura na clave de fá (uma oitava). A dificuldade\naltera a velocidade inicial.";
                    else if (ModoAtual == 3) mensagem = "Leitura na clave de fá (duas oitavas). A dificuldade\naltera a velocidade inicial.";
                    else if (ModoAtual == 4) mensagem = "Leitura na clave de dó (uma oitava). A dificuldade\naltera a velocidade inicial.";
                    else if (ModoAtual == 5) mensagem = "Leitura na clave de dó (duas oitavas). A dificuldade\naltera a velocidade inicial.";
                    else if (ModoAtual == 6) mensagem = "Leitura nas clave de fá e sol (duas oitavas). \nA dificuldade altera a velocidade inicial. Este modo \nrequer um controlador MIDI";
                    else if (ModoAtual == 7) mensagem = "Formação de acordes. A dificuldade altera a \nvariedade e complexidade dos acordes.";
                    else if (ModoAtual == 8) mensagem = "Baixo cifrado. A dificuldade altera a variedade e\ncomplexidade das cifragens.";
                    else if (ModoAtual == 9) mensagem = "Formação de acordes em graus diversos de \nmúltiplas tonalidades. A dificuldade altera a \nvariedade e complexidade dos graus utilizados.";
                    else if (ModoAtual == 10) mensagem = "Modo supresa. Leitura em diferentes claves \n(jogando com o mouse) e também acordes/baixo \ncifrado com teclado MIDI";
                    else if (ModoAtual == 11) mensagem = "Modo dois jogadores. Formação de acordes. \nO jogador 1 forma acordes acima do dó central.";

                    spriteBatch.DrawString(font, mensagem, new Vector2(300, 205), Color.Red);

                    if (ModoAtual != -1)
                    {
                        spriteBatch.Draw(m_backGroundColorRed, new Rectangle(290 + ((ModoAtual%4)*110), 290 +( (ModoAtual/4)*100), 100, 80), Color.White);
                        spriteBatch.Draw(m_backGroundColorWhite, new Rectangle(293 + ((ModoAtual % 4) * 110), 293 + ((ModoAtual / 4) * 100), 94, 74), Color.White);
                    }

                    spriteBatch.Draw(ClaveSolMetade,    new Vector2(300, 300), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    spriteBatch.Draw(ClaveSol,          new Vector2(410, 300), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    spriteBatch.Draw(ClaveFaMetade,     new Vector2(520, 300), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    spriteBatch.Draw(ClaveFa,           new Vector2(630, 300), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);                    
                    spriteBatch.Draw(ClaveDoMetade,     new Vector2(300, 400), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    spriteBatch.Draw(ClaveDo,           new Vector2(410, 400), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    spriteBatch.Draw(ClaveSolFa,        new Vector2(513, 400), null, Color.White, 0, new Vector2(0, 0), 0.97f, SpriteEffects.None, 0);
                    spriteBatch.Draw(ClaveAcordes,      new Vector2(630, 400), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    spriteBatch.Draw(ClaveBaixoCifrado, new Vector2(300, 500), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    spriteBatch.Draw(ClaveHarmonia,     new Vector2(410, 500), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    spriteBatch.Draw(ClaveInterrogacao, new Vector2(520, 500), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                    spriteBatch.Draw(Clave2P,           new Vector2(630, 500), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                }
                else if (SUBMENU == 12) // minigames
                {
                    spriteBatch.Draw(MenuInstrucoes, new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);

                    String mensagem = "                 Selecione um minigame \n        (clique para retornar ao menu inicial) ";
                       
                    if (ModoAtual == 0) mensagem = "Homem-palito VS Violinista Açougueiro";
                    else if (ModoAtual == 1) mensagem = "Escalator VS The Outsiders";
                    else if (ModoAtual == 2) mensagem = "Universitário VS Bambu";
                    else if (ModoAtual == 4) mensagem = "Link VS Otite";
                    else if (ModoAtual == 5) mensagem = "Ah não: o datilografista virtuoso";
                    else if (ModoAtual == 6) mensagem = "Forca de tonalidades";

                    spriteBatch.DrawString(font, mensagem, new Vector2(300, 205), Color.Red); 

                    if (ModoAtual != -1)
                    {
                        spriteBatch.Draw(m_backGroundColorRed, new Rectangle(330 + ((ModoAtual % 4) * 110), 310 + ((ModoAtual / 4) * 100), 100, 80), Color.White);
                        spriteBatch.Draw(m_backGroundColorWhite, new Rectangle(333 + ((ModoAtual % 4) * 110), 313 + ((ModoAtual / 4) * 100), 94, 74), Color.White);
                    }

                    spriteBatch.Draw(HomemPalito[0], new Vector2(325 + 40, 300 + 20), null, Color.White, 0, new Vector2(0, 0), 0.76f, SpriteEffects.None, 0);
                    spriteBatch.Draw(Escalator, new Vector2(410 + 40, 300 + 20), null, Color.White, 0, new Vector2(0, 0), 0.083f, SpriteEffects.None, 0);
                    spriteBatch.Draw(Universitario, new Vector2(532 + 40, 293 + 20), null, Color.White, 0, new Vector2(0, 0), 0.28f, SpriteEffects.None, 0);

                    spriteBatch.Draw(Link[2], new Vector2(310 + 40, 400 + 20), null, Color.White, 0, new Vector2(0, 0), 3.5f, SpriteEffects.None, 0);
                    spriteBatch.Draw(Olivetti, new Vector2(410 + 40, 400 + 20), null, Color.White, 0, new Vector2(0, 0), 0.42f, SpriteEffects.None, 0);
                    spriteBatch.Draw(Forca[7], new Vector2(540 + 40, 395 + 20), null, Color.White, 0, new Vector2(0, 0), 0.26f, SpriteEffects.None, 0);

                }
                else if (SUBMENU == 666)
                {
                    spriteBatch.Draw(MenuInstrucoes, new Vector2(0, -5), null, Color.White, 0, new Vector2(0, 0), 0.98f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(Menufont, "\n\nMórreu! ", new Vector2(305, 210), Color.Red);

                    if (soundEngineMorte.State == SoundState.Stopped)
                    {
                        spriteBatch.DrawString(font, "\n\n\n\n\n\n\n\n\nClique com o mouse para retornar", new Vector2(305, 260), Color.Red);
                    }

                    if (OVERTYPE >= 1) spriteBatch.Draw(facepalm[0], new Vector2(520, 240), null, Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);

                    if (Clave != 8)
                    {
                        if (TempoJogo.Seconds < 10) spriteBatch.DrawString(font, "\n\n\n\nPontos: " + Pontos.ToString() + "\nRecorde: " + Recordes[Clave + (DIFICULDADE * 11)] + "\nRanking: " + Ranking + "\nTempo de jogo: " + TempoJogo.Minutes + ":0" + TempoJogo.Seconds, new Vector2(305, 260), Color.Red);
                        else spriteBatch.DrawString(font, "\n\n\n\nPontos: " + Pontos.ToString() + "\nRecorde: " + Recordes[Clave + (DIFICULDADE * 11)] + "\nRanking: " + Ranking + "\nTempo de jogo: " + TempoJogo.Minutes + ":" + TempoJogo.Seconds, new Vector2(305, 260), Color.Red);
                    }
                    else if (Clave == 8)
                    {
                        if (TempoJogo.Seconds < 10) spriteBatch.DrawString(font, "\n\n\n\nPontos (Jogador 1): " + Pontos.ToString() + "\nPontos (Jogador 2): " + Pontos2.ToString() + "\nTempo de jogo: " + TempoJogo.Minutes + ":0" + TempoJogo.Seconds, new Vector2(305, 260), Color.Red);
                        else spriteBatch.DrawString(font, "\n\n\n\nPontos (Jogador 1): " + Pontos.ToString() + "\nPontos (Jogador 2): " + Pontos2.ToString() + "\nTempo de jogo: " + TempoJogo.Minutes + ":" + TempoJogo.Seconds, new Vector2(305, 260), Color.Red);
                    }

                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    //GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.Point;
                    //GraphicsDevice.SamplerStates[0].MinFilter = TextureFilter.Point;
                    //GraphicsDevice.SamplerStates[0].MipFilter = TextureFilter.Point;


                    if (OVERTYPE == 0)
                    {
                        if (MarioOverPos.Y <= 100) MarioOverDir = 1;

                        if (MarioOverDir == 0) MarioOverPos -= new Vector2(0, 6);
                        else if (MarioOverDir == 1) MarioOverPos += new Vector2(0, 8);

                        spriteBatch.Draw(MarioOver, MarioOverPos, null, Color.White, 0, new Vector2(0, 0), 8f, SpriteEffects.None, 0);
                    }

                }
                else if (SUBMENU == 667) // INTRO
                {
                    GBTimer += 1;
                    GraphicsDevice.Clear(Color.Black);                    
                    spriteBatch.DrawString(fontEscalator, "AVISO", new Vector2(300, 5), Color.White);
                    spriteBatch.DrawString(Menufont, "\nA exposição prolongada a este jogo pode \nprovocar os seguintes efeitos colaterais:", new Vector2(60, 90), Color.White);                    
                    spriteBatch.DrawString(Menufont, "-" + EfeitosColaterais[Efeito[0]], new Vector2(60, 225), Color.White, 0, new Vector2(0, 0), 0.95f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(Menufont, "\n-" + EfeitosColaterais[Efeito[1]], new Vector2(60, 225), Color.White, 0, new Vector2(0, 0), 0.95f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(Menufont, "\n\n-" + EfeitosColaterais[Efeito[2]], new Vector2(60, 225), Color.White, 0, new Vector2(0, 0), 0.95f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(Menufont, "\n\n\n-" + EfeitosColaterais[Efeito[3]], new Vector2(60, 225), Color.White, 0, new Vector2(0, 0), 0.95f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(Menufont, "\n\n\n\n-" + EfeitosColaterais[Efeito[4]], new Vector2(60, 225), Color.White, 0, new Vector2(0, 0), 0.95f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(Menufont, "\n\n\n\n\n-" + EfeitosColaterais[Efeito[5]], new Vector2(60, 225), Color.White, 0, new Vector2(0, 0), 0.95f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(Menufont, "\n\n\n\n\n\n-" + EfeitosColaterais[Efeito[6]], new Vector2(60, 225), Color.White, 0, new Vector2(0, 0), 0.95f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(Menufont, "A persistirem os sintomas, procure atendimento\npsiquiátrico adequado.", new Vector2(60, 500), Color.White);

                }


                spriteBatch.End();
            }

            if (FINAL == 2)
            {
                alternador++;
                if (alternador > 10) alternador = 0;

                GraphicsDevice.Clear(new Color(115, 126, 182));

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                spriteBatch.Draw(Picanha, new Vector2 (340, 130), null, Color.White, 0, new Vector2(0, 0), 0.8f, SpriteEffects.None, 0);

                if (alternador > 5) spriteBatch.DrawString(font, "Fong! (v1.0)  (C) 2011 TDAH Games", new Vector2(250 + Rand.Next(4), 90 + Rand.Next(4)), Color.Red);
                else spriteBatch.DrawString(font, "Fong! (v1.0)  (C) 2011 TDAH Games", new Vector2(250, 90), Color.Red);
                if (alternador > 5) spriteBatch.DrawString(font, "Todos os efeitos sonoros/gráficos foram roubados da Internet (viva o Google!)", new Vector2(80 + Rand.Next(4), 450 + Rand.Next(5)), Color.Red);
                else spriteBatch.DrawString(font, "Todos os efeitos sonoros/gráficos foram roubados da Internet (viva o Google!)", new Vector2(80, 450), Color.Red);
                if (alternador > 5) spriteBatch.DrawString(font, "Tetris Remix por Radioactive Project", new Vector2(240 + Rand.Next(5), 500 + Rand.Next(4)), Color.Red);
                else spriteBatch.DrawString(font, "Tetris Remix por Radioactive Project", new Vector2(240, 500), Color.Red);

                spriteBatch.End();
            }

            if (FINAL != 2 && MENU != 1 && !GAMEBOY) 
            {

                alternador++;
                if (alternador > 10) alternador = 0;

                if (Pontos > 2150 && Pontos < 3300)
                {
                    if (alternador == 5) GraphicsDevice.Clear(Color.Green);
                    else GraphicsDevice.Clear(Color.White);
                }
                else GraphicsDevice.Clear(Color.White);

                //spriteBatch.Begin(SpriteBlendMode.AlphaBlend);


                // TO DO: TESTAR CAMERA

                /*
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend,
                SpriteSortMode.Immediate,
                SaveStateMode.SaveState,
                Camera.get_transformation(GraphicsDevice));
                Camera.Pos = new Vector2(400, 300);
                 */

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                if (PianoShake2) Camera.Rotation = rot * MathHelper.Pi / 180;
                else Camera.Rotation = 0; 

                if (rotdir == 0) rot += 0.27f;
                else rot -= 0.27f;

                if (rot > 10) rotdir = 1;
                if (rot < -10) rotdir = 0;


                    if (BG[0].Position.X < -BG[0].Size.Width) BG[0].Position.X = BG[10].Position.X + BG[10].Size.Width - 1;
                    if (BG[1].Position.X < -BG[1].Size.Width) BG[1].Position.X = BG[0].Position.X + BG[0].Size.Width;
                    if (BG[2].Position.X < -BG[2].Size.Width) BG[2].Position.X = BG[1].Position.X + BG[1].Size.Width;
                    if (BG[3].Position.X < -BG[3].Size.Width) BG[3].Position.X = BG[2].Position.X + BG[2].Size.Width;
                    if (BG[4].Position.X < -BG[4].Size.Width) BG[4].Position.X = BG[3].Position.X + BG[3].Size.Width;
                    if (BG[5].Position.X < -BG[5].Size.Width) BG[5].Position.X = BG[4].Position.X + BG[4].Size.Width;
                    if (BG[6].Position.X < -BG[6].Size.Width) BG[6].Position.X = BG[5].Position.X + BG[5].Size.Width;
                    if (BG[7].Position.X < -BG[6].Size.Width) BG[7].Position.X = BG[6].Position.X + BG[6].Size.Width;
                    if (BG[8].Position.X < -BG[7].Size.Width) BG[8].Position.X = BG[7].Position.X + BG[7].Size.Width;
                    if (BG[9].Position.X < -BG[8].Size.Width) BG[9].Position.X = BG[8].Position.X + BG[8].Size.Width;
                    if (BG[10].Position.X < -BG[10].Size.Width) BG[10].Position.X = BG[9].Position.X + BG[9].Size.Width;

                    Vector2 aDirection = new Vector2(-1, 0);
                    Vector2 aSpeed = new Vector2(160, 0);

                    for (int i = 0; i < 11; i++) BG[i].Position += aDirection * aSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    for (int i = 0; i < 11; i++) BG[i].Draw(this.spriteBatch);

                Rectangle ballRect;
                ballRect = new Rectangle((int)ballPosition.X-4, (int)ballPosition.Y-4, ballSprite.Width+8, ballSprite.Height-4);
                if (Clave == 5) ballRect = new Rectangle((int)ballPosition.X - 4, (int)ballPosition.Y - 4, ballSprite.Width + 8, ballSprite.Height + 8);

                if (Clave != 4 && Clave != 8 && Clave != 7 && Clave != 6) spriteBatch.Draw(m_backGroundColorBlack, ballRect, Color.White);

                Vector2 FixedPosition = new Vector2((int)ballPosition.X, (int)ballPosition.Y);

                if (Clave != 5) spriteBatch.Draw(ballSprite, new Rectangle((int)FixedPosition.X, (int)FixedPosition.Y, ballSprite.Width, ballSprite.Height-12), new Rectangle(0, 0, ballSprite.Width, ballSprite.Height-12), Color.White);
                else if (Clave == 5) spriteBatch.Draw(ballSprite, new Rectangle((int)FixedPosition.X, (int)FixedPosition.Y, ballSprite.Width, ballSprite.Height), new Rectangle(0, 10, ballSprite.Width, ballSprite.Height), Color.White);

                //spriteBatch.Draw(CifraSprite[CifraBaixo[CurrentNote - 5][Temp]], new Vector2(600, 25) + new Vector2(65, 85), null, Color.White, 0f, new Vector2(0, 0), 1.5f, SpriteEffects.None, 0f);

                if (Clave == 5) spriteBatch.Draw(CifraSprite[CifraBaixo[CurrentNote - 5][Temp]], FixedPosition + new Vector2(44, 45), Color.White);

                paddlePositionRot.X = paddlePosition.X - 390;
                paddlePositionRot.Y = paddlePosition.Y;
                paddlePositionTemp.X = paddlePosition.X + 42;
                paddlePositionTemp.Y = paddlePosition.Y + 64;
                if (Clave == 8) spriteBatch.Draw(hand1, paddlePositionTemp, null, Color.White, (paddlePositionRot.X * MathHelper.Pi / 180) / 5, new Vector2(42, 59), 1.0f, SpriteEffects.None, 0f);
                else spriteBatch.Draw(paddleSprite, paddlePositionTemp, null, Color.White, (paddlePositionRot.X * MathHelper.Pi / 180) / 5, new Vector2(42, 59), 1.0f, SpriteEffects.None, 0f);

                 
                if (Clave == 8)
                {
                    paddlePositionRot2.X = paddlePosition2.X - 390;
                    paddlePositionRot2.Y = paddlePosition2.Y;
                    paddlePositionTemp2.X = paddlePosition2.X + 42;
                    paddlePositionTemp2.Y = paddlePosition2.Y + 64;
                    spriteBatch.Draw(hand2, paddlePositionTemp2, null, Color.White, -(paddlePositionRot2.X * MathHelper.Pi / 180) / 5, new Vector2(42, 59), 1.0f, SpriteEffects.FlipVertically, 0f);
                    
                    if (MostrarTeclado) spriteBatch.Draw(TecladoInstrucao, new Vector2(60, 140), Color.White);

                    if (halt == 0) MostrarTeclado = false;
                }
                

                DesenhaPiano();
                DesenhaReferencia();

                foreach (BonusMsg msg in Bonus)
                {
                    spriteBatch.Draw(BonusMoldura, msg.Pos + new Vector2(-25, -50), null, Color.White, 0, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, msg.Msg, msg.Pos + msg.BonusMsgOffset, Color.White);

                    msg.BonusTimer -= 0.5f;
                    if (msg.BonusTimer < 0) msg.Remover = true;
                }

                // limpar lista
                BonusMsg item;
                for (int index = Bonus.Count - 1; index >= 0; index--)
                {
                    item = Bonus[index];
                    if (item.Remover == true) Bonus.RemoveAt(index);
                }


                if (!GAMEBOY) DesenhaSelecao();

                if (ShowRB)
                {
                    Rectangle RBRect = new Rectangle((int)RBPos.X - 8, (int)RBPos.Y - 10,
                (int)(RedBull.Width * 0.17F), (int)(RedBull.Height * 0.17F));

                    spriteBatch.Draw(RedBull, RBPos, null, Color.White, 0, new Vector2(42, 59), 0.17f, SpriteEffects.None, 0);
                }

                if (RBPos.Y > 900) ShowRB = false;


                if (ShowMM)
                {
                    if (MMType == 0) spriteBatch.Draw(MM, MMPos, null, Color.White, (20 * MathHelper.Pi / 180), new Vector2(42, 59), 0.5f, SpriteEffects.None, 0f);
                    else if (MMType == 1) spriteBatch.Draw(Pringles, MMPos, null, Color.White, 0.12F, new Vector2(42, 59), 0.15f, SpriteEffects.None, 0f);
                    else if (MMType == 2) spriteBatch.Draw(DiamanteNegro, MMPos, null, Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0f);
                    else if (MMType == 3) spriteBatch.Draw(Coca, MMPos, null, Color.White, 0.3F, new Vector2(0, 0), 0.3f, SpriteEffects.None, 0f);
                    else if (MMType == 4) spriteBatch.Draw(Zombie, MMPos, null, Color.White, 0, new Vector2(42, 59), 0.4f, SpriteEffects.None, 0f);
                    else if (MMType == 5) spriteBatch.Draw(Shrek, MMPos, null, Color.White, BowserRot, new Vector2(42, 59), 0.3f, SpriteEffects.None, 0f);
                    else if (MMType == 6) spriteBatch.Draw(Gretchen, MMPos, null, Color.White, 0, new Vector2(0, 0), 0.45f, SpriteEffects.None, 0f);
                    else if (MMType == 7) spriteBatch.Draw(Pizza[PizzaStatus], MMPos, null, Color.White, 0, new Vector2(42, 59), 0.3f, SpriteEffects.None, 0f);
                    else if (MMType == 8)
                    {
                        if (PacSide == 0) spriteBatch.Draw(PacMan, MMPos, null, Color.White, (270 * MathHelper.Pi / 180), new Vector2(PacMan.Width / 2, PacMan.Height / 2), 0.4f, SpriteEffects.None, 0f);
                        else if (PacSide == 1) spriteBatch.Draw(PacMan, MMPos, null, Color.White, (270 * MathHelper.Pi / 180), new Vector2(PacMan.Width / 2, PacMan.Height / 2), 0.4f, SpriteEffects.FlipVertically, 0f);
                    }
                }


//                Rectangle ballRect =
                                    //new Rectangle((int)ballPosition.X, (int)ballPosition.Y - 25,
                                    //ballSprite.Width, ballSprite.Height);
                //spriteBatch.Draw(m_backGroundColorGreen, ballRect, Color.White);


                if (Clave == 8) DrawHUD2P();
                else DrawHUD();

                if (ShowFerer)
                {
                    spriteBatch.Draw(Ferer, FererPos, null, Color.White, FererRot, new Vector2(42, 59), 0.6f, SpriteEffects.None, 0);
                }


                CurrentKeyMIDI = -1;
                String Mensagem;

                List<Pitch> pitches = new List<Pitch>(pitchesPressed.Keys);
                pitches.Sort();

                Mensagem = "";
                for (int i = 0; i < pitches.Count; ++i)
                {
                    Pitch pitch = pitches[i];
                    if (i > 0)
                    {
                        Mensagem += ", ";
                    }

                    if (Clave == 0)
                    {
                        if (pitch.Octave() == 2) CurrentKeyMIDI = PitchTrans[pitch.PositionInOctave()];
                        if (pitch.Octave() == 3) CurrentKeyMIDI = PitchTrans2[pitch.PositionInOctave()];
                        else if (pitch == Pitch.C4) CurrentKeyMIDI = 14;
                    }
                    else if (Clave == 1 || Clave == 9)
                    {
                        if (pitch.Octave() == 4) CurrentKeyMIDI = PitchTrans[pitch.PositionInOctave()];
                        if (pitch.Octave() == 5) CurrentKeyMIDI = PitchTrans2[pitch.PositionInOctave()];
                        else if (pitch == Pitch.C6) CurrentKeyMIDI = 14;
                    }
                    else if (Clave == 2 || Clave == 11) // CLave de Dó
                    {
                        if (pitch.Octave() == 3) CurrentKeyMIDI = PitchTrans[pitch.PositionInOctave()];
                        if (pitch.Octave() == 4) CurrentKeyMIDI = PitchTrans2[pitch.PositionInOctave()];
                        else if (pitch == Pitch.C5) CurrentKeyMIDI = 14;
                    }
                    else if (Clave == 3)
                    {
                        if (pitch.Octave() == 2) CurrentKeyMIDI = PitchTrans[pitch.PositionInOctave()];
                        if (pitch.Octave() == 3) CurrentKeyMIDI = PitchTrans2[pitch.PositionInOctave()];
                        if (pitch.Octave() == 4) CurrentKeyMIDI = PitchTrans3[pitch.PositionInOctave()];
                        if (pitch.Octave() == 5) CurrentKeyMIDI = PitchTrans4[pitch.PositionInOctave()];
                        else if (pitch == Pitch.C6) CurrentKeyMIDI = 28;
                    }
                    else if (Clave == 10) // To DO: Provavelmente corrigir oitava da Clave 10
                    {
                        if (pitch.Octave() == 2) CurrentKeyMIDI = PitchTrans[pitch.PositionInOctave()];
                        if (pitch.Octave() == 3) CurrentKeyMIDI = PitchTrans2[pitch.PositionInOctave()];
                        else if (pitch == Pitch.C4) CurrentKeyMIDI = 14;
                    }


                    Mensagem = "Note: " + CurrentNote.ToString() + "MIDI: " + CurrentKeyMIDI.ToString();
                    //Mensagem = pitch.PositionInOctave().ToString();

                    if (pitch.NotePreferringSharps() != pitch.NotePreferringFlats())
                    {
                        //Console.Write(" or {0}", pitch.NotePreferringFlats());
                    
                    }
                }

                List<Chord> chords = Chord.FindMatchingChords(pitches);
                
                Mensagem +=  " Clave: " + Clave.ToString() + "   Acorde: ";
                for (int i = 0; i < chords.Count; ++i)
                {
                    Chord chord = chords[i];

                    if (i > 0)
                    {
                        Mensagem += ", ";
                    }
                    Mensagem += chord.Name.ToString() + " Root: " + chord.Root.ToString() + " Pattern: " + chord.Pattern.ToString();
                    //chord.
                }

                // debug
                //spriteBatch.DrawString(font, Mensagem, new Vector2(0, 300), Color.Red);
                //spriteBatch.DrawString(font, CurrentChord.ToString() + " Root: " + CurrentChord.Root.ToString() + " Inversion" + CurrentChord.Inversion.ToString(), new Vector2(0, 360), Color.Red);

                if (Clave == 4 || Clave == 8)
                {
                    String acorde = CurrentChord.ToString();

                    if (acorde.Length > 3)
                    {
                        string aug = acorde.Substring(acorde.Length - 3, 3);
                        if (aug == "aug") acorde = acorde.Substring(0, acorde.Length - 3) + "aum";
                    }

                    if (acorde.Length == 4) spriteBatch.DrawString(fontBig, acorde, ballPosition + new Vector2(17, 32), Color.Black);
                    else if (acorde.Length == 5) spriteBatch.DrawString(fontBig, acorde, ballPosition + new Vector2(11, 32), Color.Black);
                    else if (acorde.Length == 3) spriteBatch.DrawString(fontBig, acorde, ballPosition + new Vector2(27, 32), Color.Black);
                    else if (acorde.Length == 2) spriteBatch.DrawString(fontBig, acorde, ballPosition + new Vector2(32, 32), Color.Black);
                    else if (acorde.Length == 1) spriteBatch.DrawString(fontBig, acorde, ballPosition + new Vector2(39, 32), Color.Black);
                    else if (acorde.Length > 5) spriteBatch.DrawString(font, acorde, ballPosition + new Vector2(11, 32), Color.Black);

                
                }
                else if (Clave == 6)
                {
                    spriteBatch.DrawString(fontBig, Clave6Acorde, ballPosition + new Vector2(17, 32), Color.Black);
                }

                spriteBatch.End();
            }

            if (GAMEBOY)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                
                if (GBTYPE == 0)
                {
                    MouseState mouseState3 = Mouse.GetState();
                    if (!MINIGAME)
                    {
                        for (int i = 0; i < 11; i++) BG[i].Draw(this.spriteBatch);
                        DesenhaPiano();
                        DesenhaReferencia();

                        paddlePositionRot.X = paddlePosition.X - 390;
                        paddlePositionRot.Y = paddlePosition.Y;
                        paddlePositionTemp.X = paddlePosition.X + 42;
                        paddlePositionTemp.Y = paddlePosition.Y + 64;
                        spriteBatch.Draw(paddleSprite, paddlePositionTemp, null, Color.White, (paddlePositionRot.X * MathHelper.Pi / 180) / 5, new Vector2(42, 59), 1.0f, SpriteEffects.None, 0f);
                        DrawHUD();
                    }

                    spriteBatch.Draw(GameBoy, new Vector2(300, 45), null, Color.White, 0, new Vector2(0, 0), 0.90f, SpriteEffects.None, 0f);    
                    
                    if (ABSTART)
                    {
                        spriteBatch.DrawString(font, "Aperte A, B\n   e Start", new Vector2(410, 145), Color.Black);
                        spriteBatch.DrawString(font, "Pontos:\n" + (Pontos - (int)PontosPerdidos).ToString(), new Vector2(425, 200), Color.Black);
                        Rectangle BotaoA = new Rectangle(561, 375, 48, 44);
                        Rectangle BotaoB = new Rectangle(511, 400, 48, 44);
                        Rectangle BotaoStart = new Rectangle(454, 476, 45, 30);

                        /*
                        spriteBatch.Draw(m_backGroundColorGreen, BotaoA, Color.White);
                        spriteBatch.Draw(m_backGroundColorGreen, BotaoB, Color.White);
                        spriteBatch.Draw(m_backGroundColorGreen, BotaoStart, Color.White); 
                         */

                    }
                    else
                    {

                        if (GBTimer < 200)
                        {                           
                            spriteBatch.DrawString(font, "Homem-Palito\n         VS\n    violinista\n  açougueiro ", new Vector2(410, 150), Color.Black);
                        }
                        else if (GBTimer > 200 && GBTimer <= 400)
                        {
                            spriteBatch.DrawString(fontSmall, "Controle\n\nMouse1: pular\nMouse2: abaixar\nSetas: mover", new Vector2(410, 150), Color.Black);
                        }
                        else if (GBTimer > 400)
                        {
                            MouseState mouseState = Mouse.GetState();
                            KeyboardState keyState = Keyboard.GetState();

                            if (keyState.IsKeyDown(Keys.Down))
                            {
                                PALITOSENTADO = true;
                                goto pulo;
                            }
                            else PALITOSENTADO = false;

                            if (mouseState.RightButton == ButtonState.Pressed) PALITOSENTADO = true;
                            else PALITOSENTADO = false;

                            pulo: ;

                            Rectangle PalitoRect = new Rectangle((int)PalitoPos.X, (int)PalitoPos.Y, (int)(HomemPalito[0].Width * 0.6F) - 6, (int)(HomemPalito[0].Height * 0.6F) - 4);                            
                            
                            if (PALITOSENTADO) PalitoRect = new Rectangle((int)PalitoPos.X, (int)PalitoPos.Y + 18, (int)(HomemPalito[1].Width * 0.6F) - 6, (int)(HomemPalito[1].Height * 0.6F) - 13);

                            Rectangle FacaRect = new Rectangle((int)FacaPos.X + 11, (int)FacaPos.Y, (int)(Faca.Width * 0.1F) -24, (int)(Faca.Height * 0.1F));

                            if (FacaDir == 1) FacaRect = new Rectangle((int)FacaPos.X+12, (int)FacaPos.Y, (int)(Faca.Width * 0.1F) -25, (int)(Faca.Height * 0.1F));
                            else if (FacaDir == 2) FacaRect = new Rectangle((int)FacaPos.X - 5, (int)FacaPos.Y+12, 5, 30);

                            if (FacaRect.Intersects(PalitoRect)) PALITOMORREU = true;
                            
                            //spriteBatch.Draw(m_backGroundColorRed, PalitoRect, Color.White);
                            //spriteBatch.Draw(m_backGroundColorRed, FacaRect, Color.White);


                            if ((PALITOSENTADO) && !PALITOMORREU) spriteBatch.Draw(HomemPalito[1], PalitoPos, null, Color.White, 0, new Vector2(0, 0), 0.60f, PalitoState, 0f);
                            else spriteBatch.Draw(HomemPalito[0], PalitoPos, null, Color.White, 0, new Vector2(0, 0), 0.60f, PalitoState, 0f);
                            
                            if (FacaDir == 0 && FacaTimer > 30) spriteBatch.Draw(Faca, FacaPos, null, Color.White, 0, new Vector2(0, 0), 0.1f, SpriteEffects.None, 0f);
                            else if (FacaDir == 1 && FacaTimer > 30) spriteBatch.Draw(Faca, FacaPos, null, Color.White, 0, new Vector2(0, 0), 0.1f, SpriteEffects.FlipHorizontally, 0f);
                            else if (FacaDir == 2 && FacaTimer > 30) spriteBatch.Draw(Faca, FacaPos, null, Color.White, 90 * (MathHelper.Pi/180), new Vector2(0, 0), 0.1f, SpriteEffects.FlipHorizontally, 0f);

                            spriteBatch.End();

                            // todo: testar isso e mario game over
                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                            
                            /*spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                            GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.Point;
                            GraphicsDevice.SamplerStates[0].MinFilter = TextureFilter.Point;
                            GraphicsDevice.SamplerStates[0].MipFilter = TextureFilter.Point;
                             */

                            if (FacaDir == 0) spriteBatch.Draw(HomemPalito[2], new Vector2(560, FacaPos.Y - 40), null, Color.White, 0, new Vector2(0, 0), 0.70f, SpriteEffects.None, 0f);
                            else if (FacaDir == 1) spriteBatch.Draw(HomemPalito[2], new Vector2(335, FacaPos.Y - 40), null, Color.White, 0, new Vector2(0, 0), 0.70f, SpriteEffects.None, 0f);
                            else if (FacaDir == 2) spriteBatch.Draw(HomemPalito[2], new Vector2(FacaPos.X, 47), null, Color.White, 0, new Vector2(0, 0), 0.70f, SpriteEffects.None, 0f);

                            if (PALITOMORREU)
                            {
                                FacaMovY = 0;
                                spriteBatch.DrawString(font, " Facadas: " + FacaCount.ToString() + "\n +" + (FacaCount * 20).ToString() + " pontos", new Vector2(410, 120), Color.Black);
                            }

                        }
                        //spriteBatch.DrawString(font, GBTimer.ToString() + "  " + FacaPos.Y.ToString(), new Vector2(370, 100), Color.Black);

                        if (PALITOMORREU)
                        {
                            if (soundEngineMario.Pitch >= -0.98F) soundEngineMario.Pitch -= 0.005F;
                        }

                        if (soundEngineMario.Pitch < -0.97F)
                        {
                            GAMEBOY = false;
                            Pulo = false;
                            PALITOMORREU = false;
                            halt = 50;
                            FacaTimer = 0;
                            FacaDir = 0;
                            soundEngineMario.Stop();
                            soundEngine.Play();
                            soundEngine.Volume = 0.6f;
                            if (MINIGAME) soundEngine.Volume = 0.8f;

                            GBTimer = 0;
                            Pontos += (FacaCount * 20);

                            if (FacaCount > FacaRecorde)
                            {
                                FacaRecorde = FacaCount;
                                SalvaRecordes();
                            }
                        }


                    }

                }
                else if (GBTYPE == 1)
                {
                    GraphicsDevice.Clear(Color.White); //new Color(115, 126, 182));
                    if (ShowBowser)
                    {
                        spriteBatch.Draw(m_backGroundColorRed, new Rectangle(280, 454, 300, 20), Color.Red);
                        spriteBatch.Draw(m_backGroundColorWhite, new Rectangle(283, 456, 294, 16), Color.White);
                        spriteBatch.Draw(m_backGroundColorRed, new Rectangle(283, 456, (int)(BowserEnergia * 11.8F), 16), Color.Red);
                        spriteBatch.Draw(Bowser, BowserPos, null, Color.White, BowserRot, new Vector2(0, 0), 0.235f, SpriteEffects.None, 0f);
                        spriteBatch.DrawString(font, "Energia:\n", new Vector2(400, 430), Color.Red);

                    }
                    spriteBatch.Draw(piano800, pianoPos, Color.White);
                    DesenhaReferencia();

                    if (ShowBowser) spriteBatch.DrawString(font, "Clique com o botão esquerdo para atacar\n", new Vector2(250, 0), Color.Red);
                    else spriteBatch.DrawString(font, "Clique nas notas na sequencia abaixo\n", new Vector2(250, 0), Color.Red);
                    
                    if (BowserNotasStatus[0]) spriteBatch.Draw(noteSprite[BowserNotas[0]], new Vector2(75, 500), Color.White);
                    if (BowserNotasStatus[1]) spriteBatch.Draw(noteSprite[BowserNotas[1]], new Vector2(175, 500), Color.White);
                    if (BowserNotasStatus[2]) spriteBatch.Draw(noteSprite[BowserNotas[2]], new Vector2(275, 500), Color.White);
                    if (BowserNotasStatus[3]) spriteBatch.Draw(noteSprite[BowserNotas[3]], new Vector2(375, 500), Color.White);
                    if (BowserNotasStatus[4]) spriteBatch.Draw(noteSprite[BowserNotas[4]], new Vector2(475, 500), Color.White);
                    if (BowserNotasStatus[5]) spriteBatch.Draw(noteSprite[BowserNotas[5]], new Vector2(575, 500), Color.White);
                    if (BowserNotasStatus[6]) spriteBatch.Draw(noteSprite[BowserNotas[6]], new Vector2(675, 500), Color.White);

                    spriteBatch.DrawString(font, "Vidas: " + Vidas.ToString(), new Vector2(5, 0), Color.Red);
                    spriteBatch.DrawString(font, "Pontos: " + Pontos.ToString(), new Vector2(100, 0), Color.Red);
                }
                else if (GBTYPE == 2)
                {

                    if (GBTimer < 150)
                    {
                        spriteBatch.DrawString(Menufont, "   Pausa para jogar videogame ..." , new Vector2(150, 130), Color.Black);
                        spriteBatch.Draw(Ps3, new Vector2(150, 170), null, Color.White, 0, new Vector2(0, 0), 0.8f, SpriteEffects.None, 0f);
                    }
                    else if (GBTimer > 150 && GBTimer < 350)
                    {
                        GraphicsDevice.Clear(Color.White); //new Color(115, 126, 182));
                        spriteBatch.Draw(FundoMadeira, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0f);

                        spriteBatch.Draw(Escalator, new Vector2(140, 230), null, Color.White, 0, new Vector2(0, 0), 0.18f, SpriteEffects.None, 0f);
                        spriteBatch.Draw(TMonk, new Vector2(500, 230), null, Color.White, 0, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0f);
                        spriteBatch.DrawString(fontEscalator, "   ESCALATOR", new Vector2(130, 80), Color.Red);
                        spriteBatch.DrawString(fontEscalator, "         VS", new Vector2(130, 240), Color.Red);
                        spriteBatch.DrawString(fontEscalator, "THE OUTSIDERS", new Vector2(130, 400), Color.Red);
                    }
                    else
                    {
                        GraphicsDevice.Clear(Color.White); //new Color(115, 126, 182));
                        spriteBatch.Draw(FundoMadeira, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0f);

                        if (GBTimer < 1000) spriteBatch.DrawString(Menufont, "Acerte as notas que não fazem parte da escala!", new Vector2(-400 + ((GBTimer - 350) * 2), 550), Color.White);
                        spriteBatch.DrawString(fontEscalatorSmall, "Escala:                              Pontos: ", new Vector2(40, 20), Color.Red);
                        spriteBatch.DrawString(Menufont, EscalaAtual, new Vector2(190, 27), Color.White);
                        spriteBatch.DrawString(Menufont, PontosEscalator.ToString(), new Vector2(680, 25), Color.White);

                        foreach (EscalatorNota nota in NotasEscalator)
                        {
                            DesenhaNota(nota.Nota, nota.Pos.X, nota.Pos.Y, Color.White);
                            //spriteBatch.Draw(m_backGroundColorGreen, nota.Rect, Color.White);
                        }

                        if (EscalatorMorreu) // Exibe notas da escala
                        {
                            int OffsetX = 0;
                            spriteBatch.DrawString(fontEscalator, "NOTA ERRADA: ", new Vector2(40, 160), Color.Red);
                            spriteBatch.DrawString(fontEscalator, "Escala: ", new Vector2(40, 280), Color.Red);

                            if (EscalaTipo == 0)
                            {
                                foreach (string NotaEscala in EscalasMaiores[EscalaNota])
                                {
                                    DesenhaNota(NotaEscala, 40 + OffsetX, 400, Color.White);
                                    string notabemol = NotaEscala.Substring(NotaEscala.Length - 1, 1);
                                    if (notabemol == "b") OffsetX += 100;
                                    else if (notabemol == "#") OffsetX += 110;
                                    else OffsetX += 80;
                                }
                            }
                            else if (EscalaTipo == 1)
                            {
                                foreach (string NotaEscala in EscalasMenoresNatural[EscalaNota])
                                {
                                    DesenhaNota(NotaEscala, 40 + OffsetX, 400, Color.White);
                                    string notabemol = NotaEscala.Substring(NotaEscala.Length - 1, 1);
                                    if (notabemol == "b") OffsetX += 100;
                                    else if (notabemol == "#") OffsetX += 110;
                                    else OffsetX += 80;
                                }
                            }
                            else if (EscalaTipo == 2)
                            {
                                foreach (string NotaEscala in EscalasMenoresHarmonica[EscalaNota])
                                {
                                    DesenhaNota(NotaEscala, 40 + OffsetX, 400, Color.White);
                                    string notabemol = NotaEscala.Substring(NotaEscala.Length - 1, 1);
                                    if (notabemol == "b") OffsetX += 100;
                                    else if (notabemol == "#") OffsetX += 110;
                                    else OffsetX += 80;

                                }
                            }
                            else if (EscalaTipo == 3)
                            {
                                foreach (string NotaEscala in EscalasMenoresMelodica[EscalaNota])
                                {
                                    DesenhaNota(NotaEscala, 40 + OffsetX, 400, Color.White);
                                    string notabemol = NotaEscala.Substring(NotaEscala.Length - 1, 1);

                                    if (notabemol == "b") OffsetX += 100;
                                    else if (notabemol == "#") OffsetX += 110;
                                    else OffsetX += 80;
                                }
                            }


                        }

                        Rectangle MiraRect = new Rectangle((int)MiraPos.X + 25, (int)MiraPos.Y + 25, 58, 58);
                        //spriteBatch.Draw(m_backGroundColorGreen, MiraRect, Color.White);

                        spriteBatch.End();

                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                        /*
                        spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                        GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.Point;
                        GraphicsDevice.SamplerStates[0].MinFilter = TextureFilter.Point;
                        GraphicsDevice.SamplerStates[0].MipFilter = TextureFilter.Point;
                         */

                        spriteBatch.Draw(Mira, MiraPos, null, Color.White, 0, new Vector2(0, 0), 0.6f, SpriteEffects.None, 0f);

                    } 

                }
                else if (GBTYPE == 3) // TRANSPOSIÇÃO
                {
                    GraphicsDevice.Clear(Color.White);
                    if (!MINIGAME) DrawHUD();
                    
                    spriteBatch.Draw(TonalidadesFig[TomAtual], new Vector2(375, 120), null, Color.White, 0, new Vector2(0, 0), 3f, SpriteEffects.None, 0f);
                    if (ForcaStatus <= 8) spriteBatch.Draw(Forca[ForcaStatus], new Vector2(100, 60), Color.White);
                    else if (ForcaStatus > 8) spriteBatch.Draw(Forca[8], new Vector2(100, 60), Color.White);

                    if (TomTipo == 0) spriteBatch.DrawString(font, "Arraste a tonalidade maior correspondente:  " + TomAtual.ToString(), new Vector2(280, 80), Color.Red);
                    else if (TomTipo == 1) spriteBatch.DrawString(font, "Arraste a tonalidade menor correspondente:  " + TomAtual.ToString(), new Vector2(280, 80), Color.Red);

                    //MouseState mouseState = Mouse.GetState();
                    //Rectangle MouseRect2 = new Rectangle(mouseState.X - 15, mouseState.Y -10, 36, 25);
                    //spriteBatch.Draw(m_backGroundColorGreen, MouseRect2, Color.White);
                    
                    spriteBatch.DrawString(font, "Respostas corretas: " + TomCount + "  Pontos: " + TomCount*40, new Vector2(280, 60), Color.Red);

                    foreach (NotaTonalidade tom in Tom)
                    {
                        //spriteBatch.Draw(m_backGroundColorRed, tom.Rect, Color.White);
                        DesenhaTom(tom);
                    }

                    spriteBatch.DrawString(font, "Tom: ", new Vector2(480, 270), Color.Black); 
                    spriteBatch.Draw(TomQuadrado, new Vector2(530, 260), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                    
                    
                }
                else if (GBTYPE == 4) // UNIVERSITÁRIO VS. BAMBU
                {
                    if (!MINIGAME)
                    {
                        //spriteBatch.Draw(piano800, pianoPos, Color.White);
                        for (int i = 0; i < 11; i++) BG[i].Draw(this.spriteBatch);
                        DesenhaPiano();
                        DesenhaReferencia();
                        paddlePositionRot.X = paddlePosition.X - 390;
                        paddlePositionRot.Y = paddlePosition.Y;
                        paddlePositionTemp.X = paddlePosition.X + 42;
                        paddlePositionTemp.Y = paddlePosition.Y + 64;
                        spriteBatch.Draw(paddleSprite, paddlePositionTemp, null, Color.White, (paddlePositionRot.X * MathHelper.Pi / 180) / 5, new Vector2(42, 59), 1.0f, SpriteEffects.None, 0f); 
                        spriteBatch.Draw(paddleSprite, paddlePositionTemp, null, Color.White, (paddlePositionRot.X * MathHelper.Pi / 180) / 5, new Vector2(42, 59), 1.0f, SpriteEffects.None, 0f);
                        DrawHUD();
                    }

                    spriteBatch.Draw(TV, new Vector2(20, 60), null, Color.White, 0, new Vector2(0, 0), 0.4f, SpriteEffects.None, 0f);

                    if (GBTimer < 250)
                    {
                        spriteBatch.DrawString(Menufont, "    Parabéns, você foi sorteado\n para participar de um programa\n               de auditório!", new Vector2(160, 235), Color.White);
                    }
                    else if (GBTimer < 350 && GBTimer >= 250)
                    {
                        spriteBatch.DrawString(Menufont, "UNIVERSITÁRIO\n                            ", new Vector2(275, 240), Color.White);
                        spriteBatch.Draw(Universitario, new Vector2(145, 205), null, Color.White, 0, new Vector2(0, 0), 0.7f, SpriteEffects.None, 0f);
                    }
                    else if (GBTimer < 450 && GBTimer >= 350)
                    {
                        spriteBatch.DrawString(Menufont, "UNIVERSITÁRIO\n           vs\n        ", new Vector2(275, 240), Color.White);
                        spriteBatch.Draw(Universitario, new Vector2(145, 205), null, Color.White, 0, new Vector2(0, 0), 0.7f, SpriteEffects.None, 0f);
                    }
                    else if (GBTimer < 650 && GBTimer >= 450)
                    {
                        spriteBatch.DrawString(Menufont, "UNIVERSITÁRIO\n           vs\n        BAMBU", new Vector2(275, 240), Color.White);
                        spriteBatch.Draw(Silvio, new Vector2(540, 210), null, Color.White, 0, new Vector2(0, 0), 0.25f, SpriteEffects.None, 0f);
                        spriteBatch.Draw(Universitario, new Vector2(145, 205), null, Color.White, 0, new Vector2(0, 0), 0.7f, SpriteEffects.None, 0f);
                    }
                    else if (GBTimer >= 650)
                    {
                        if (PontosBambu < 80) spriteBatch.DrawString(fontBig, PerguntasFacil[PerguntaAtual], new Vector2(150, 160), Color.White);
                        else if (PontosBambu >= 240) spriteBatch.DrawString(fontBig, PerguntasFoda[PerguntaAtual], new Vector2(150, 160), Color.White);
                        else if (PontosBambu >= 160) spriteBatch.DrawString(fontBig, PerguntasDificil[PerguntaAtual], new Vector2(150, 160), Color.White);
                        else if (PontosBambu >= 80) spriteBatch.DrawString(fontBig, PerguntasMedio[PerguntaAtual], new Vector2(150, 160), Color.White);

                        spriteBatch.Draw(BotaoVazio, new Vector2(130, 400), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                        spriteBatch.Draw(BotaoVazio, new Vector2(130, 340), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                        spriteBatch.Draw(BotaoVazio, new Vector2(410, 400), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                        spriteBatch.Draw(BotaoVazio, new Vector2(410, 340), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                        if (BotaoAtual == 0) spriteBatch.Draw(BotaoCheio, new Vector2(130, 340), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                        else if (BotaoAtual == 1) spriteBatch.Draw(BotaoCheio, new Vector2(410, 340), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                        else if (BotaoAtual == 2) spriteBatch.Draw(BotaoCheio, new Vector2(130, 400), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                        else if (BotaoAtual == 3) spriteBatch.Draw(BotaoCheio, new Vector2(410, 400), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);

                        Window.Title = BotaoAtual.ToString();

                        if (PontosBambu < 80) DesenhaPerguntas(AlternativasFacil);
                        else if (PontosBambu >= 240) DesenhaPerguntas(AlternativasFoda);
                        else if (PontosBambu >= 160) DesenhaPerguntas(AlternativasDificil);
                        else if (PontosBambu >= 80) DesenhaPerguntas(AlternativasMedio);

                        if (!RespostaErrada) TempoSobrando -= new TimeSpan(0, 0, 0, 0, gameTime.ElapsedGameTime.Milliseconds);

                        if (TempoSobrando.Seconds < 10) spriteBatch.DrawString(font, "Tempo restante: " + TempoSobrando.Minutes + ":0" + TempoSobrando.Seconds + "   Pontos: " + PontosBambu.ToString(), new Vector2(200, 100), Color.White);
                        else spriteBatch.DrawString(font, "Tempo restante: " + TempoSobrando.Minutes + ":" + TempoSobrando.Seconds + "   Pontos: " + PontosBambu.ToString(), new Vector2(200, 100), Color.White);

                        // debug
                        //spriteBatch.DrawString(font, PerguntaAtual.ToString() + " " + (PerguntaAtual%4).ToString() + " " + Rotacao.ToString() + " " + ((PerguntaAtual+Rotacao)%4).ToString(), new Vector2(120, 100), Color.White);
                    }
                }
                else if (GBTYPE == 6)
                {
                    GraphicsDevice.Clear(Color.Black);
                    
                    spriteBatch.End();

                    // todo: testar isso


                    SamplerState test = new SamplerState 
                    { 
                        AddressU = TextureAddressMode.Wrap,
                        AddressV = TextureAddressMode.Wrap,
                    };

                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, test, DepthStencilState.Default, RasterizerState.CullNone);
                    
                    //spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);

                    //GraphicsDevice.BlendState = BlendState.AlphaBlend;
                    //GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                    //GraphicsDevice.SamplerStates[0] = new SamplerState 
                    //{ 
                        //AddressU = TextureAddressMode.Wrap, 
                        //AddressV = TextureAddressMode.Wrap
                    //};
                    //GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;

                    //GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
                     
                    TextureOffsetX += 1;
                    TextureOffsetX = TextureOffsetX % 64;
                    Rectangle source = new Rectangle(-800, 0, 1600, 600);
                    spriteBatch.Draw(FundoDrMario, new Vector2(TextureOffsetX, 0), source, Color.White, 0, new Vector2(64, 0), 1.0f, SpriteEffects.None, 0.5f);

                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);

                    if (PalavraIntroducao)
                    {
                        spriteBatch.Draw(m_backGroundColorBlack, new Rectangle(128, 128, 544, 352), Color.Black);

                        foreach (Palavra p in PalavrasLista) spriteBatch.DrawString(EmuFont, p.TextoAtual, p.Pos, Color.Red);
                        spriteBatch.DrawString(EmuFont, "Digite \n para iniciar", new Vector2(256, 390), Color.White);
                        spriteBatch.DrawString(Menufont, "                Ah, não!\n     o datilografista virtuoso", new Vector2(200,153), Color.White);
                        spriteBatch.Draw(Olivetti, new Vector2(330, 240), null, Color.White, 0, new Vector2(0, 0), 0.8f, SpriteEffects.None, 0);
                        
                    }
                    else if (!PalavraIntroducao)
                    {

                        foreach (Palavra p in PalavrasLista)
                        {
                            if (!p.Especial) spriteBatch.DrawString(EmuFont, p.TextoAtual, p.Pos, Color.White);
                            else if (p.Especial) spriteBatch.DrawString(EmuFont, p.TextoAtual, p.Pos, Color.Red);
                            
                            //spriteBatch.DrawString(EmuFont, p.TextoAtual[0].ToString(), p.Pos + new Vector2(0,100), Color.Black);                   
                        }
                        spriteBatch.Draw(m_backGroundColorBlack, new Rectangle(0, 544, 800, 100), Color.Black);
                        spriteBatch.DrawString(EmuFont, "Pontos: " + PontosPalavra, new Vector2(300, 560), Color.White);

                    }

                    
                }
                else if (GBTYPE == 5)
                {

                    GraphicsDevice.Clear(new Color(253, 236, 164));
                    spriteBatch.Draw(m_backGroundColorBlack, new Rectangle(0, 0, 800, 100), Color.Black);
                    spriteBatch.DrawString(EmuFont, "                   NOTA Base: ", new Vector2(10, 10), Color.White);
                    if (!TrocaNotabase) spriteBatch.DrawString(fontSmall, "Pressione J para escutar o intervalo novamente ", new Vector2(410, 78), Color.White);
                    spriteBatch.DrawString(EmuFont, "PONTOS: " + PontosLink, new Vector2(10, 10), Color.White);
                                        
                    spriteBatch.DrawString(EmuFont, "ENERGIA:   ", new Vector2(10, 45), Color.White);
                    spriteBatch.Draw(m_backGroundColorWhite, new Rectangle(183, 45, 254, 29), Color.White);
                    spriteBatch.Draw(m_backGroundColorBlack, new Rectangle(185, 47, 250, 25), Color.Black);
                    spriteBatch.Draw(m_backGroundColorRed, new Rectangle(185, 47, (int)ENERGIA, 25), Color.Red);

                    spriteBatch.Draw(noteSprite[NOTABASE], new Rectangle(650, 10, noteSprite[NOTABASE].Width, noteSprite[0].Height - 20), new Rectangle(0, 10, noteSprite[0].Width, noteSprite[0].Height - 20), Color.White);

                    /*
                    if (GBTimer == 130)
                    {
                        TrocaNotabase = true;
                        NotabaseTimer = 1000;
                        GBTimer += 1;
                    }
                     */

                    /*if (GBTimer % 1000 == 0 && !TrocaNotabase) 
                    {
                        TrocaNotabase = true;
                        NotabaseTimer = 1000;                        
                    }
                     */

                    if (!TrocaNotabase)
                    {

                        if (ENERGIA >= 0) ENERGIA -= 0.2f;


                        foreach (NotaLink n in NotasLink)
                        {
                            spriteBatch.Draw(noteSprite[n.Tipo], new Rectangle((int)n.Pos.X, (int)n.Pos.Y, noteSprite[NOTABASE].Width, noteSprite[0].Height - 20), new Rectangle(0, 10, noteSprite[0].Width, noteSprite[0].Height - 20), Color.White);
                            //spriteBatch.Draw(m_backGroundColorGreen, n.Rect, Color.White);                         
                            foreach (Link l in Links)
                            {
                                if (l.Rect.Intersects(n.Rect))
                                {
                                    if (NotabaseChegada == n.Tipo)
                                    {
                                        
                                        ENERGIA += 80;
                                        if (ENERGIA > 250) ENERGIA = 250;
                                        TrocaNotabase = true;
                                        NotabaseTimer = 1000;
                                        PontosLink += 30;

                                        if (n.Pos.X == -5) { n.SetXY(new Vector2(710, n.Pos.Y)); goto t; }
                                        else if (n.Pos.X == 710) { n.SetXY(new Vector2(-5, n.Pos.Y)); goto t; }

                                        if (n.Pos.Y == 540) n.SetXY(new Vector2(n.Pos.X, 100));
                                        else if (n.Pos.Y == 100) n.SetXY(new Vector2(n.Pos.X, 540));
                                    t: ;                                        

                                    }
                                    else if (NotabaseChegada != n.Tipo)
                                    {
                                        if (n.Pos.X == -5) { n.SetXY(new Vector2(710, n.Pos.Y)); goto t2; }
                                        else if (n.Pos.X == 710) { n.SetXY(new Vector2(-5, n.Pos.Y)); goto t2; }

                                        if (n.Pos.Y == 540) n.SetXY(new Vector2(n.Pos.X, 100)); 
                                        else if (n.Pos.Y == 100) n.SetXY(new Vector2(n.Pos.X, 540));
                                        
                                       t2:
                                        ENERGIA -= 40;
                                        LinkWrong.Play();
                                    }
                                }
                            }
                        }


                        foreach (Link l in Links)
                        {
                            if (ENERGIA < 0) l.Dead = true;
                            l.UpdateFrame(10);
                            spriteBatch.Draw(Link[l.Status + l.Frame], l.Pos, null, Color.White, 0, new Vector2(0, 0), 3.0f, SpriteEffects.None, 0);
                            //spriteBatch.Draw(m_backGroundColorGreen, l.Rect, Color.White);
                            if (l.Dead)
                            {

                                // to do: som, animacao, msg
                                GAMEBOY = false;
                                ConfiguraProximaNota();
                                soundEngineMario.Stop();
                                soundEngine.Play();
                                soundEngine.Volume = 0.6f;
                                if (MINIGAME) soundEngine.Volume = 0.8f;

                                if (PontosLink > LinkRecorde)
                                {
                                    LinkRecorde = PontosLink;
                                    SalvaRecordes();
                                }
                                Pontos += PontosLink;
                                halt = 120;
                            }

                        }

                        foreach (InimigoLink i in InimigosLinkLista)
                        {
                            i.UpdateFrame(7);
                            if (i.Tipo != 4 && i.Tipo != 5) spriteBatch.Draw(InimigosLink[i.Tipo + i.Frame], i.Pos, null, Color.White, 0, new Vector2(0, 0), 3.0f, SpriteEffects.None, 0);
                            else spriteBatch.Draw(InimigosLink[i.Tipo + i.Frame], i.Pos, null, Color.White, 0, new Vector2(0, 0), 1.5f, SpriteEffects.None, 0);
                            //spriteBatch.Draw(m_backGroundColorGreen, i.Rect, Color.White); 

                            if (i.Tipo != 4)
                            {
                                if (i.Pos.X > 750) i.Speed.X = -Rand.Next(3);
                                else if (i.Pos.X < 10) i.Speed.X = Rand.Next(3);
                                
                                if (i.Pos.Y < 100) i.Speed.Y = Rand.Next(3);
                                else if (i.Pos.Y > 560) i.Speed.Y = -Rand.Next(3);

                                i.IncXY(i.Speed.X, i.Speed.Y);

                            }

                            foreach (Link l in Links)
                            {
                                if (i.Rect.Intersects(l.Rect))
                                {
                                    if (i.Tipo == 0 || i.Tipo == 2 || i.Tipo == 6)
                                    {
                                        ENERGIA -= 1.7f;
                                        LinkHit.Play();
                                    }
                                    else if (i.Tipo == 4)
                                    {
                                        ENERGIA += 85;
                                        PontosLink += 12;
                                        if (ENERGIA > 250) ENERGIA = 250;
                                        i.Remover = true;
                                        Getheart.Play();
                                    }
                                }
                            }
                        } // foreach inimigo
                    }
                    else if (TrocaNotabase)
                    {
                        foreach (NotaLink n in NotasLink) spriteBatch.Draw(noteSprite[n.Tipo], new Rectangle((int)n.Pos.X, (int)n.Pos.Y, noteSprite[NOTABASE].Width, noteSprite[0].Height - 20), new Rectangle(0, 10, noteSprite[0].Width, noteSprite[0].Height - 20), Color.White);
                        foreach (Link l in Links) spriteBatch.Draw(Link[l.Status + l.Frame], l.Pos, null, Color.White, 0, new Vector2(0, 0), 3.0f, SpriteEffects.None, 0);
                        foreach (InimigoLink i in InimigosLinkLista)
                        {
                           if (i.Tipo != 4 && i.Tipo != 5) spriteBatch.Draw(InimigosLink[i.Tipo + i.Frame], i.Pos, null, Color.White, 0, new Vector2(0, 0), 3.0f, SpriteEffects.None, 0);
                           else spriteBatch.Draw(InimigosLink[i.Tipo + i.Frame], i.Pos, null, Color.White, 0, new Vector2(0, 0), 1.5f, SpriteEffects.None, 0);
                       }


                        if (NotabaseTimer == 1001)
                        {
                            this.IsMouseVisible = true;
                            spriteBatch.Draw(m_backGroundColorBlack, new Rectangle(180, 160, 420, 310), Color.Black);
                            spriteBatch.DrawString(EmuFont, "   LINK x OTITE", new Vector2(200, 180), Color.Aquamarine);
                            spriteBatch.DrawString(EmuFont, "Escute o intervalo\ne  colete  a  nota\ncorreta.Desvie dos\ninimigos  e  pegue\nmoedas para ganhar\nenergia.\n\n  k = Continuar", new Vector2(200, 220), Color.White);
                        } 
                        else
                        {
                            spriteBatch.Draw(m_backGroundColorBlack, new Rectangle(250, 210, 350, 190), Color.Black);
                            spriteBatch.DrawString(EmuFont, "NOTA: ", new Vector2(310, 240), Color.White);
                            spriteBatch.Draw(noteSprite[NOTABASE], new Rectangle(430, 240, noteSprite[NOTABASE].Width, noteSprite[0].Height - 20), new Rectangle(0, 10, noteSprite[0].Width, noteSprite[0].Height - 20), Color.White);
                            if (NotabaseTimer == 875) spriteBatch.DrawString(EmuFont, "J = REPETIR\nK = CONTINUA", new Vector2(305, 325), Color.White);
                        }
                    }

                    InimigoLink item;
                    for (int index = InimigosLinkLista.Count - 1; index >= 0; index--)
                    {
                        item = InimigosLinkLista[index];
                        if (item.Remover == true) InimigosLinkLista.RemoveAt(index);
                    }                        

                }
                else if (GBTYPE == 10 || GBTYPE == 11 || GBTYPE == 12)
                {
                    for (int i = 0; i < 11; i++) BG[i].Draw(this.spriteBatch);
                    DrawHUD();
                    DesenhaPiano();
                    DesenhaReferencia();
                    paddlePositionRot.X = paddlePosition.X - 390;
                    paddlePositionRot.Y = paddlePosition.Y;
                    paddlePositionTemp.X = paddlePosition.X + 42;
                    paddlePositionTemp.Y = paddlePosition.Y + 64;
                    spriteBatch.Draw(paddleSprite, paddlePositionTemp, null, Color.White, (paddlePositionRot.X * MathHelper.Pi / 180) / 5, new Vector2(42, 59), 1.0f, SpriteEffects.None, 0f);
                    if (GBTYPE == 10 || GBTYPE == 11)
                    {
                        spriteBatch.Draw(BonusMoldura, new Vector2(170, 170), null, Color.White, 0, new Vector2(0, 0), 2.2f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, "                                 DISTRAÇÕES\n\n  Clique nas distrações que comprometem o \n  rendimento do estudo para eliminá-las\n  antes que elas acabem com seus pontos.\n  Aperte os dois botões do mouse\n  simultaneamente (OU TECLA Z) para continuar", new Vector2(250, 250), Color.White);
                    }
                    else if (GBTYPE == 12)
                    {
                        spriteBatch.Draw(BonusMoldura, new Vector2(170, 170), null, Color.White, 0, new Vector2(0, 0), 2.2f, SpriteEffects.None, 0);
                        spriteBatch.DrawString(font, "                                 PROGRESSÃO DE NÍVEL\n\n  Parabéns, você passou de nível!\n  Aperte os dois botões do mouse\n  simultaneamente (OU TECLA Z) para continuar\n Próxima nota =", new Vector2(250, 250), Color.White);
                        spriteBatch.Draw(ballSprite, new Rectangle((int)200, (int)400, ballSprite.Width, ballSprite.Height - 12), new Rectangle(0, 0, ballSprite.Width, ballSprite.Height - 12), Color.White);
                    }

                    }


                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}