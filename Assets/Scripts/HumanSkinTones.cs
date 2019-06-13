using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    static class HumanSkinTones
    {
        private static Color FromByte(int r, int g, int b)
        {
            return new Color(r / 255.0f, g / 255.0f, b / 255.0f);
        }

        private static Color[] _whiteManInstance;
        private static Color[] _blackManInstance;

        private static Color[] _allTones;


        public static Color[] AllSkinTones
        {
            get
            {
                if (_allTones == null)
                {
                    Color[] white = WhiteSkinTones;
                    Color[] black = BlackSkinTones;
                    _allTones = new Color[white.Length + black.Length];

                    white.CopyTo(_allTones, 0);
                    black.CopyTo(_allTones, white.Length);
                }

                return _allTones;
            }
        }

        public static Color[] BlackSkinTones
        {
            get
            {
                if (_blackManInstance == null)
                {
                    _blackManInstance = new Color[]
                    {
                        FromByte(80, 50, 30),
                        FromByte(123, 0, 0),
                        FromByte(114, 0, 0),
                        FromByte(56, 0, 0),
                        FromByte(148, 10, 0),
                        FromByte(100, 25, 0),
                        FromByte(91, 0, 0),
                        FromByte(113, 2, 0),
                        FromByte(67, 0, 0),
                        FromByte(165, 57, 0),
                        FromByte(134, 4, 0),
                    };
                }

                return _blackManInstance;
            }
        }

        public static Color[] WhiteSkinTones
        {
            get
            {
                if (_whiteManInstance == null)
                {
                    _whiteManInstance = new Color[]
                    {
                        FromByte(225, 173, 164),
                        FromByte(80, 50, 30),
                        FromByte(165, 136, 105),
                        FromByte(223, 185, 151),
                        FromByte(208, 146, 110), // Good one
                        FromByte(189, 151, 120),
                        FromByte(187, 109, 74),
                        FromByte(253, 228, 200),
                        FromByte(239, 214, 189),
                        FromByte(234, 189, 157),
                        FromByte(227, 194, 124),
                        FromByte(224, 177, 132),
                        FromByte(223, 166, 117),
                        FromByte(190, 114, 60),
                        FromByte(255, 224, 196),
                        FromByte(238, 207, 180),
                        FromByte(222, 171, 127),
                        FromByte(217, 145, 100),
                        FromByte(204, 132, 67),
                        FromByte(199, 122, 88),
                        FromByte(255, 220, 177),
                        FromByte(229, 194, 152),
                        FromByte(228, 185, 142),
                        FromByte(226, 185, 143),
                        FromByte(227, 164, 115)
                    };
                }

                return _whiteManInstance;
            }
        }

        public static Color DefaultWhiteManTone => _whiteManInstance[0];
        public static Color BlackManTone => BlackSkinTones[0];
    }
}
