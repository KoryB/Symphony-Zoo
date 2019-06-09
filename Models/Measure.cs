﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Symphony_Zoo_New.Models
{
    public class Measure
    {
        private bool inProgress;//This measure was checked out to a client to be composed.
        public bool InProgress
        {
            get { return inProgress; }
            set
            {
                if (value == true){ guid = Guid.NewGuid(); }
                else { guid = default(Guid); }
                inProgress = value;
            }
        }
        private int fromId;//vertex
        public int FromId { get { return fromId; } set { fromId = value; } }
        private int toId;//vertex
        public int ToId { get { return toId; } set { toId = value; } }
        private bool edge;//0 for logical node, 1 for logical edge
        public bool Edge { get { return edge; } set { edge = value; } }
        private Guid guid;
        public Guid Guid { get { return guid; } set { guid = value; } }
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
