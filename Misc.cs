using System.Collections.Generic;
using System.Speech.Synthesis;
using System.Data.SQLite;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace GenXdev.Helpers
{
    public static class Misc
    {
        public static SQLiteConnection SQLiteConnection = new SQLiteConnection();
        public static SqlConnection SqlConnection = new SqlConnection();
        public static SpeechSynthesizer SpeechCustomized = new SpeechSynthesizer();
        public static SpeechSynthesizer Speech = new SpeechSynthesizer();
    }
}