using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Symphony_Zoo_New.Models
{

    /*
        A Measure knows the ID for its associated midi file stored server-side.
    */
    public class Measure
    {
        private bool inProgress;//This measure was checked out to a client to be composed.
        public bool InProgress
        {
            get { return inProgress; }
            set
            {
                inProgress = value;
                if (inProgress){ guid = Guid.NewGuid(); }
                else { guid = default(Guid); }
            }
        }
        private int fromId;//vertex
        public int FromId { get { return fromId; } set { fromId = value; } }
        private int toid;//vertex
        public int ToId { get { return ToId; } set { ToId = value; } }
        private bool edge;//0 for logical node, 1 for logical edge
        public bool Edge { get { return edge; } set { edge = value; } }
        private Guid guid;
        public Guid Guid { get { return guid; } }
        private byte[] midiData;
        public byte[] MidiData { get { return midiData; } set { midiData = value; } }
        public Measure_DataTransferObject DTO
        {
            get
            {
                return new Measure_DataTransferObject() { Guid = guid, MidiData = MidiData};
            }
        }
    }
}
