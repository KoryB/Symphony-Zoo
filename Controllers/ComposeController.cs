﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Symphony_Zoo_New.Models;
using Symphony_Zoo_New.Utility;

namespace Symphony_Zoo_New.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComposeController : ControllerBase
    {
        //GET api/compose
        [HttpGet]
        public ActionResult<Measure_DataTransferObject[]> Get()
        {
            Measure m;
            Measure needsComposing;
            for (int i = 0; i < 10; i++)
            {
                m = Graph.Instance.GetRandomMeasure();
                if(m.InProgress == false)
                {
                    if (m.Edge)
                    {
                        if (Graph.Instance.GetMeasuresLeavingVertex(m.ToId).ToArray().Length == 0)
                        {
                            //compose a node.
                            needsComposing = new Measure()
                            {
                                Edge = false,
                                FromId = m.ToId,
                                ToId = Graph.Instance.NextAvailableVertexID,
                                InProgress = true
                            };
                            Graph.Instance.AddToGraph(needsComposing);
                            //Possibility #1: In this case, compose a node at the end of an edge.
                            return new Measure_DataTransferObject[] { m.DTO, needsComposing.DTO };
                        }
                    }
                    else
                    {
                        int[] goingTo =
                        (from measure in Graph.Instance.GetMeasuresLeavingVertex(m.ToId)
                         select measure.ToId).ToArray();


                        int[] notGoingTo =
                        (from vertex in Enumerable.Range(0, Graph.Instance.LargestVertexID)
                         where goingTo.Contains(vertex) == false
                         && m.FromId != vertex
                         && m.ToId != vertex
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

                            Graph.Instance.AddToGraph(needsComposing);
                            //Possibility #2: In this case, compose an edge from one node to another node.
                            return new Measure_DataTransferObject[] { m.DTO, needsComposing.DTO, Graph.Instance.GetMeasuresLeavingVertex(GoToVertex).ToArray()[0].DTO };
                        }
                    }
                }
            }
            m = Graph.Instance.GetRandomNodeMeasure();
            //Possibility #3: The catch all - compose an edge from a node to nowhere.
            needsComposing = new Measure()
            {
                Edge = true,
                FromId = m.ToId,
                ToId = Graph.Instance.NextAvailableVertexID,
                InProgress = true
            };

            Graph.Instance.AddToGraph(needsComposing);
            return new Measure_DataTransferObject[] {m.DTO, needsComposing.DTO};
        }

        //POST api/compose
        [HttpPost]
        public void Post([FromBody] Measure_DataTransferObject value)
        {
            Graph.Instance.AddToGraphFromAPI(new Measure { Guid = value.Guid, MidiData = value.MidiData});
        }
    }
}