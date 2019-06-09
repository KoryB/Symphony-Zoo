using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Symphony_Zoo_New.Models
{
    public class EdgeDebug_DataTransferObject
    {
        public int FromId;
        public int ToId;
        public byte[] MidiData;
        public bool InProgress;
        public bool Edge;
    }
}
