using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Symphony_Zoo_New.Utility;
using Newtonsoft.Json;


namespace Symphony_Zoo_New.Models
{
    public sealed class Graph
    {
        private static Graph instance = null;//singleton code
        private static readonly object padlock = new object();//singleton code

        private int largestVertexID;
        private string savedFileLocation = "saved/";
        public int LargestVertexID { get { return largestVertexID; } set { largestVertexID = value; } }
        private List<Measure> edges;

        public int NextAvailableVertexID { get { largestVertexID++; return largestVertexID; } }

        Graph()
        {
            // DON: If you find from/to that is larger than largestVertexID, reset this to that!
            largestVertexID = 0;
            edges = new List<Measure>();

            // DON: Attempt to load file from [wherever], and deserialize it into the "edges" variable.
            // DON: If that fails, create the firstEverMeasure object and have edges start with just that.
            foreach(string file in Directory.EnumerateFiles(savedFileLocation)) 
            {
                // Deserialize the file to look at the measure that it describes, could use error checking.
                Measure curMeasure = JsonConvert.DeserializeObject<Measure>(file);

                // Check to see if a vertex ID exists that is larger than largestVertexID.
                if(curMeasure.ToId > largestVertexID) largestVertexID = curMeasure.ToId;
                if(curMeasure.FromId > largestVertexID) largestVertexID = curMeasure.FromId;

                // Push this measure into the edges List! If this loop never ran, make firstEverMeasure!
                AddToGraph(curMeasure);
            }
            
            // If edges is empty, I'm taking that to mean that the loop found nothing in savedFileLocation.
            if(edges.Count == 0) 
            {
                Measure firstEverMeasure = new Measure
                {
                    InProgress = false,
                    FromId = 0,
                    ToId = NextAvailableVertexID,
                    Edge = false
                };

                AddToGraph(firstEverMeasure); // DON: Use this, not .Add()
            }
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

                    // DON: Serialize and write out a file, filename is going to be checkedOutMeasure[idx]
                    // Opting to use the index into the edges list as a best practice, rather than measure.
                    string fname = "measure_" + checkedOutMeasure[0];
                    string fdata = JsonConvert.SerializeObject(edges[checkedOutMeasure[0]]);

                    // DON: Just overwrite whatever is already there with that name.
                    File.WriteAllText(savedFileLocation + fname, fdata);
                }
            }
        }

        public void AddToGraph(Measure measure)
        {
            // DON: Save measure to a file name, that is the file name of the index where it's going into edges.
            string fname = "measure_" + edges.Count.ToString();
            string fdata = JsonConvert.SerializeObject(measure);

            // WriteAllText can potentially be slow with large files, StreamWriter can be an alternative if needed.
            File.WriteAllText(savedFileLocation + fname, fdata);

            // Add the measure to edges.
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
