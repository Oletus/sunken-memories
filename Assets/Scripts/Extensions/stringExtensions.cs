using System.Text;
using UnityEngine;

namespace LPUnityUtils
{

    public static class stringExtensions {

        // Scramble or un-scramble a string.
        public static string Scramble(this string value, string codeword)
        {
            StringBuilder res = new StringBuilder("", value.Length);
            for ( int i = 0; i < value.Length; i++ )
            {
                int xored = value[i] ^ codeword[i % codeword.Length];
                if ( xored == 0 )
                {
                    // Avoid potential issues with null-terminated strings - don't scramble this character.
                    xored = value[i];
                }
                res.Append((char)(xored));
            }
            return res.ToString();
        }

    }

}
