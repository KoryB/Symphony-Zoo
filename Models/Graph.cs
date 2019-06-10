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

    /* This directed graph contains a list of Measures.
     * A Measure can be thought of as an edge.
     * A Measure keeps track of which vertex # it comes from
     * and which vertex # it goes to.
     * Vertices are not represented anywhere else in the program.
     */
    public sealed class Graph
    {
        //The actual content of the graph
        private List<Measure> edges;

        //singleton code
        private static Graph instance = null;

        //for thread safety of singleton
        private static readonly object padlock = new object();

        //the largest known vertex in the graph
        private int largestVertexID;
        public int LargestVertexID { get { return largestVertexID; } set { largestVertexID = value; } }

        //Use this getter when creating a new vertex connecting a new measure.
        public int NextAvailableVertexID { get { largestVertexID++; return largestVertexID; } }

        //folder containing "measure_#" files for graph persistence
        private string savedFileLocation = "saved/";


        //constructor
        Graph()
        {
            largestVertexID = 0;
            edges = new List<Measure>();

            //Attempt to load file from savedFileLocation, and deserialize it into the "edges" variable.
            foreach(string file in Directory.EnumerateFiles(savedFileLocation)) 
            {
                //just in case there were other random files in the folder we don't care about
                string prefix = "saved/measure_";
                if (file.StartsWith(prefix) && int.TryParse(file.Substring(prefix.Length, file.Length - prefix.Length), out int throwAway))
                {
                    try
                    {
                        // Deserialize the file to look at the measure that it describes, could use error checking.
                        Measure curMeasure = JsonConvert.DeserializeObject<Measure>(File.ReadAllText(file));

                        // Check to see if a vertex ID exists that is larger than largestVertexID.
                        largestVertexID = Math.Max(largestVertexID, Math.Max(curMeasure.FromId, curMeasure.ToId));

                        // Push this measure into the edges List!
                        AddToGraph(curMeasure);
                    }
                    catch(Exception e)
                    {
                        Console.Write("\n ERROR lOADING MEASURE FILE:\n");
                        Console.WriteLine(e.Message);
                    }
                    
                }
            }

            //If no files are found in savedFileLocation,
            //create the firstEverMeasure object and have edges start with just that.
            if (edges.Count == 0) 
            {
                Measure firstEverMeasure = new Measure
                {
                    InProgress = false,
                    FromId = 0,
                    ToId = NextAvailableVertexID,
                    Edge = false
                };

                //Use AddToGraph, not edges.Add()
                AddToGraph(firstEverMeasure);
            }
        }

        /* singleton code
         * Everywhere else in the app should go through
         * Graph.Instance to use the graph class.
         */
        public static Graph Instance
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

        //Use this to add a completed measure that arrives at POST api/compose.
        public void AddToGraphFromAPI(Measure measure)
        {
            if(measure != null)
            {
                //Find the corresponding measure idx in the graph that they checked out.
                int[] checkedOutMeasure =
                (from edge in edges
                 where edge.Guid == measure.Guid
                 select edges.IndexOf(edge)).ToArray();

                //CheckedOutMeasure will be size zero if their guid is incorrect.
                //Proceed if checkedOutMeasure contains a match
                if (checkedOutMeasure.Length > 0)
                {
                    //No more than one correctly matched guid should be possible.
                    if (checkedOutMeasure.Length > 1)
                    {
                        Console.WriteLine("That is very strange. More than one measure in the graph had guid " + measure.Guid.ToString());
                    }
                    else
                    {
                        //Save the completed midi file into the graph.
                        edges[checkedOutMeasure[0]].Edge = !GetMeasuresEnteringVertex(edges[checkedOutMeasure[0]].FromId).First().Edge;
                        edges[checkedOutMeasure[0]].InProgress = false;
                        edges[checkedOutMeasure[0]].MidiData = measure.MidiData;

                        // Serialize and write out the Measure.
                        // Opting to use the index into the edges list as a best practice, rather than measure.
                        string fname = "measure_" + checkedOutMeasure[0];
                        string fdata = JsonConvert.SerializeObject(edges[checkedOutMeasure[0]]);

                        // Just overwrite whatever is already there with that name.
                        File.WriteAllText(savedFileLocation + fname, fdata);
                    }
                }
            }
        }


        //Use this to add a measure to the graph and immediately check it out to the user via GET /api/compose
        public void AddToGraph(Measure measure)
        {
            // Add the measure to edges.
            edges.Add(measure);

            // Save measure to a file name, that is the file name of the index where it's going into edges.
            string fname = "measure_" + (edges.Count - 1).ToString();
            string fdata = JsonConvert.SerializeObject(measure);

            // WriteAllText can potentially be slow with large files, StreamWriter can be an alternative if needed.
            File.WriteAllText(savedFileLocation + fname, fdata);
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

        //For debug use only. Used through GET api/graphdebug
        //EdgeDebug_DataTransferObject contains way more metadata than
        //Edge_DataTransferObject
        public IEnumerable<EdgeDebug_DataTransferObject> GetAllMeasures()
        {
            return 
                from edge in edges
                select edge.EdgeDebug_DTO;
        }
    }
}
