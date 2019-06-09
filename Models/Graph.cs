using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symphony_Zoo_New.Utility;

namespace Symphony_Zoo_New.Models
{
    public class Graph
    {
        private int largestVertexID;
        public int LargestVertexID { get { return largestVertexID; } set { largestVertexID = value; } }
        private List<Measure> edges;

        public int NextAvailableVertexID { get { largestVertexID++; return largestVertexID; } }

        public Graph()
        {
            edges = new List<Measure>();
            Measure firstEverMeasure = new Measure
            {
                InProgress = false,
                FromId = 0,
                ToId = 1,
                Edge = false
            };
            edges.Add(firstEverMeasure);
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
    }
}
