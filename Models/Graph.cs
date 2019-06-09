using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symphony_Zoo_New.Utility;

namespace Symphony_Zoo_New.Models
{
    public sealed class Graph
    {
        private static Graph instance = null;//singleton code
        private static readonly object padlock = new object();//singleton code

        private int largestVertexID;
        public int LargestVertexID { get { return largestVertexID; } set { largestVertexID = value; } }
        private List<Measure> edges;

        public int NextAvailableVertexID { get { largestVertexID++; return largestVertexID; } }

        Graph()
        {
            largestVertexID = 0;
            edges = new List<Measure>();
            Measure firstEverMeasure = new Measure
            {
                InProgress = false,
                FromId = 0,
                ToId = NextAvailableVertexID,
                Edge = false
            };
            edges.Add(firstEverMeasure);
        }

        public static Graph Instance //singleton code
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Graph();
                    }
                    return instance;
                }
            }
        }

        public void AddToGraphFromAPI(Measure measure)
        {
            if(measure != null)
            {
                int[] checkedOutMeasure =
                (from edge in edges
                 where edge.Guid == measure.Guid
                 select edges.IndexOf(edge)).ToArray();
                if (checkedOutMeasure.Length > 0)
                {
                    //save the midi file
                    edges[checkedOutMeasure[0]].Edge = !GetMeasuresEnteringVertex(edges[checkedOutMeasure[0]].FromId).First().Edge;
                    edges[checkedOutMeasure[0]].InProgress = false;
                    edges[checkedOutMeasure[0]].MidiData = measure.MidiData;
                }
            }
        }

        public void AddToGraph(Measure measure)
        {
            edges.Add(measure);
        }

        public Measure GetRandomMeasure()
        {
            return edges[RandomProvider.Next(edges.Count)];
        }

        public Measure GetRandomEdgeMeasure()
        {
            Measure[] edgeMeasures =
            (from edge in edges
             where edge.Edge == true
             select edge).ToArray();

            return edgeMeasures[RandomProvider.Next(edgeMeasures.Length)];
        }
        
        public Measure GetRandomNodeMeasure()
        {
            Measure[] edgeMeasures =
            (from edge in edges
             where edge.Edge == false
             select edge).ToArray();

            return edgeMeasures[RandomProvider.Next(edgeMeasures.Length)];
        }


        public IEnumerable<Measure> GetMeasuresLeavingVertex(int vertexID)
        {

            return
                from edge in edges
                where edge.FromId == vertexID
                select edge;
        }

        public IEnumerable<Measure> GetMeasuresEnteringVertex(int vertexID)
        {
            return
                from edge in edges
                where edge.ToId == vertexID
                select edge;
        }

        public IEnumerable<Edge_DataTransferObject> GetCompletedMeasures()
        {
            return
                from edge in edges
                where edge.InProgress == false
                select edge.Edge_DTO;
        }

        public IEnumerable<EdgeDebug_DataTransferObject> GetAllMeasures()//for debug use only.
        {
            return 
                from edge in edges
                select edge.EdgeDebug_DTO;
        }
    }
}
