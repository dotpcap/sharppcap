// $Id: AnsiEscapeSequences.cs,v 1.1.1.1 2007-07-03 10:15:18 tamirgal Exp $

/// <summary>************************************************************************
/// Copyright (C) 2001, Patrick Charles and Jonas Lehmann                   *
/// Distributed under the Mozilla Public License                            *
/// http://www.mozilla.org/NPL/MPL-1.1.txt                                *
/// *************************************************************************
/// </summary>
namespace SharpPcap.Packets.Util
{
    
    /// <summary> String constants for color console output.
    /// <p>
    /// This file contains control sequences to print color text on a text 
    /// console capable of interpreting and displaying control sequences.
    /// <p>
    /// A capable console would be 
    /// unix bash, os/2 shell, or command.com w/ ansi.sys loaded
    /// 
    /// </summary>
    /// <author>  Chris Cheetham
    /// </author>
    public struct AnsiEscapeSequences_Fields{
        public readonly static System.String ESCAPE_BEGIN;
        public readonly static System.String ESCAPE_END = "m";
        public readonly static System.String RESET;
        public readonly static System.String BOLD;
        public readonly static System.String UNDERLINE;
        public readonly static System.String INVERSE;
        public readonly static System.String BLACK;
        public readonly static System.String BLUE;
        public readonly static System.String GREEN;
        public readonly static System.String CYAN;
        public readonly static System.String RED;
        public readonly static System.String PURPLE;
        public readonly static System.String BROWN;
        public readonly static System.String LIGHT_GRAY;
        public readonly static System.String DARK_GRAY;
        public readonly static System.String LIGHT_BLUE;
        public readonly static System.String LIGHT_GREEN;
        public readonly static System.String LIGHT_CYAN;
        public readonly static System.String LIGHT_RED;
        public readonly static System.String LIGHT_PURPLE;
        public readonly static System.String YELLOW;
        public readonly static System.String WHITE;
        public readonly static System.String RED_BACKGROUND;
        public readonly static System.String GREEN_BACKGROUND;
        public readonly static System.String YELLOW_BACKGROUND;
        public readonly static System.String BLUE_BACKGROUND;
        public readonly static System.String PURPLE_BACKGROUND;
        public readonly static System.String CYAN_BACKGROUND;
        public readonly static System.String LIGHT_GRAY_BACKGROUND;
        static AnsiEscapeSequences_Fields()
        {
            ESCAPE_BEGIN = "" + (char) 27 + "[";
            RESET = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "0" + AnsiEscapeSequences_Fields.ESCAPE_END;
            BOLD = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "0;1" + AnsiEscapeSequences_Fields.ESCAPE_END;
            UNDERLINE = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "0;4" + AnsiEscapeSequences_Fields.ESCAPE_END;
            INVERSE = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "0;7" + AnsiEscapeSequences_Fields.ESCAPE_END;
            BLACK = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "0;30" + AnsiEscapeSequences_Fields.ESCAPE_END;
            BLUE = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "0;34" + AnsiEscapeSequences_Fields.ESCAPE_END;
            GREEN = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "0;32" + AnsiEscapeSequences_Fields.ESCAPE_END;
            CYAN = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "0;36" + AnsiEscapeSequences_Fields.ESCAPE_END;
            RED = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "0;31" + AnsiEscapeSequences_Fields.ESCAPE_END;
            PURPLE = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "0;35" + AnsiEscapeSequences_Fields.ESCAPE_END;
            BROWN = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "0;33" + AnsiEscapeSequences_Fields.ESCAPE_END;
            LIGHT_GRAY = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "0;37" + AnsiEscapeSequences_Fields.ESCAPE_END;
            DARK_GRAY = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "1;30" + AnsiEscapeSequences_Fields.ESCAPE_END;
            LIGHT_BLUE = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "1;34" + AnsiEscapeSequences_Fields.ESCAPE_END;
            LIGHT_GREEN = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "1;32" + AnsiEscapeSequences_Fields.ESCAPE_END;
            LIGHT_CYAN = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "1;36" + AnsiEscapeSequences_Fields.ESCAPE_END;
            LIGHT_RED = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "1;31" + AnsiEscapeSequences_Fields.ESCAPE_END;
            LIGHT_PURPLE = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "1;35" + AnsiEscapeSequences_Fields.ESCAPE_END;
            YELLOW = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "1;33" + AnsiEscapeSequences_Fields.ESCAPE_END;
            WHITE = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "1;37" + AnsiEscapeSequences_Fields.ESCAPE_END;
            RED_BACKGROUND = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "0;41" + AnsiEscapeSequences_Fields.ESCAPE_END;
            GREEN_BACKGROUND = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "0;42" + AnsiEscapeSequences_Fields.ESCAPE_END;
            YELLOW_BACKGROUND = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "0;43" + AnsiEscapeSequences_Fields.ESCAPE_END;
            BLUE_BACKGROUND = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "0;44" + AnsiEscapeSequences_Fields.ESCAPE_END;
            PURPLE_BACKGROUND = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "0;45" + AnsiEscapeSequences_Fields.ESCAPE_END;
            CYAN_BACKGROUND = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "0;46" + AnsiEscapeSequences_Fields.ESCAPE_END;
            LIGHT_GRAY_BACKGROUND = AnsiEscapeSequences_Fields.ESCAPE_BEGIN + "0;47" + AnsiEscapeSequences_Fields.ESCAPE_END;
        }
    }
    public interface AnsiEscapeSequences
    {
        //UPGRADE_NOTE: Members of interface 'AnsiEscapeSequences' were extracted into structure 'AnsiEscapeSequences_Fields'. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1045'"
        
    }
}