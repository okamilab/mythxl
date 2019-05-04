using System;

namespace MythXL.Func.Models
{
    public class AnalysesMessage
    {
        public string Address { get; set; }

        public string TxHash { get; set; }

        [Obsolete("Deprecated in message version 1")]
        public string Bytecode { get; set; }

        [Obsolete("Deprecated in message version 1")]
        public string Result { get; set; }

        public string Account { get; set; }

        public int Version { get; set; }
    }
}
