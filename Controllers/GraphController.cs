using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
//using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Symphony_Zoo_New.Models;
using Symphony_Zoo_New.Utility;

namespace Symphony_Zoo_New.Controllers
{


    //[Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class GraphController : ControllerBase
    {
        private string graphFile;
        private Graph graph;
        

        public GraphController()
        {
            graphFile = "graph.json";
            graph = new Graph();
        }
        
        //GET api/graph
        [HttpGet]
        public ActionResult<Measure_DataTransferObject[]> Get()
        {
            Measure m;
            Measure needsComposing;
            for (int i = 0; i < 10; i++)
            {
                m = graph.GetRandomMeasure();
                if(m.InProgress == false)
                {
                    if (m.Edge)
                    {
                        if (graph.GetMeasuresLeavingVertex(m.ToId).ToArray().Length == 0)
                        {
                            //compose a node.
                            needsComposing = new Measure()
                            {
                                Edge = false,
                                FromId = m.ToId,
                                ToId = graph.NextAvailableVertexID,
                                InProgress = true
                            };
                            graph.AddToGraph(needsComposing);
                            //Possibility #1: In this case, compose a node at the end of an edge.
                            return new Measure_DataTransferObject[] { m.DTO, needsComposing.DTO };
                        }
                    }
                    else
                    {
                        int[] goingTo =
                        (from measure in graph.GetMeasuresLeavingVertex(m.ToId)
                         select measure.ToId).ToArray();


                        int[] notGoingTo =
                        (from vertex in Enumerable.Range(0, graph.LargestVertexID)
                         where goingTo.Contains(vertex) == false
                         select vertex).ToArray();

                        if (notGoingTo.Length > 0)
                        {
                            int GoToVertex = notGoingTo[RandomProvider.Next(notGoingTo.Length)];

                            needsComposing = new Measure()
                            {
                                Edge = true,
                                FromId = m.ToId,
                                ToId = GoToVertex,
                                InProgress = true
                            };

                            graph.AddToGraph(needsComposing);
                            //Possibility #2: In this case, compose an edge from one node to another node.
                            return new Measure_DataTransferObject[] { m.DTO, needsComposing.DTO, graph.GetMeasuresLeavingVertex(GoToVertex).ToArray()[0].DTO };
                        }
                    }
                }
            }
            m = graph.GetRandomNodeMeasure();
            //Possibility #3: The catch all - compose an edge from a node to nowhere.
            needsComposing = new Measure()
            {
                Edge = true,
                FromId = m.ToId,
                ToId = graph.NextAvailableVertexID,
                InProgress = true
            };

            graph.AddToGraph(needsComposing);
            return new Measure_DataTransferObject[] {m.DTO, needsComposing.DTO};
        }

        //POST api/graph
        [HttpPost]
        public void Post([FromBody] Measure_DataTransferObject value)
        {
            graph.AddToGraphFromAPI(new Measure { Guid = value.Guid, MidiData = value.MidiData});
        }
    }
}