using System.Collections.Generic;
using Search.CL;
using Search.DAL;

namespace Search.BLL
{
    public class Manager
    {
        #region Declarations

        List<Vertex> nodeList = new List<Vertex>();
        //List<VertexBestFS> nodeListBestFS = new List<VertexBestFS>();
        List<Edge> edgeList = new List<Edge>();
        List<string> queryList = new List<string>();
        Dictionary<string, string> neighbours = new Dictionary<string, string>();
        Repository rep = new Repository();
        #endregion

        #region Common Shared Methods

        public void AddEdge(Edge edge)
        {
            edgeList.Add(edge);
        }
        public void AddVertex(Vertex vertex)
        {
            nodeList.Add(vertex);
        }
        public List<Edge> GetEdgeList()
        {
            return edgeList;
        }
        public List<Vertex> GetVertexList()
        {
            return nodeList;
        }
        public string[] GetPath()
        {
            if (queryList.Count > 0)
            {
                #region getPath

                string pathString = queryList[0];
                pathString = pathString.Replace("[", "");
                pathString = pathString.Replace("]", "");
                return pathString.Split(',');

                #endregion
            }
            return null;
        }
        public string[] GetSteps()
        {
            if (queryList.Count > 0)
            {
                #region getSteps

                string stepsString = queryList[1];
                stepsString = stepsString.Replace("[", "");
                stepsString = stepsString.Replace("]", "");
                return stepsString.Split(',');

                #endregion
            }
            return null;
        }

        #endregion

        #region Constarint Satisfaction Problem

        public void AddVertexCSP(Vertex vertex)
        {
            nodeList.Add(vertex);
            neighbours.Add(vertex.Name, "");
        }
        public void AddEdgeCSP(Edge edge)
        {
            edgeList.Add(edge);
            if (neighbours[edge.Source] == "")
            {
                neighbours[edge.Source] = edge.Target;
            }
            else
                neighbours[edge.Source] = neighbours[edge.Source] + "," + edge.Target;
            if (neighbours[edge.Target] == "")
            {
                neighbours[edge.Target] = edge.Source;
            }
            else
                neighbours[edge.Target] = neighbours[edge.Target] + "," + edge.Source;
        }
        public Dictionary<string, string> GetNeighbourListCSP()
        {
            return neighbours;
        }
        public bool GetPathAndStepsCSP()
        {
            queryList = rep.GetPathAndStepsCSP();
            if (queryList.Count > 0)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region Beadth First Search
        public void AddEdgesToDbBFS()
        {
            rep.AddEdgesBFS(edgeList);
        }
        public void AddNeighboursToDbBFS()
        {
            rep.AddNeighboursCSP(neighbours);
        }
        public void AddGoalToDbBFS(string goal)
        {
            rep.AddGoalBFS(goal);
        }
        public bool GetPathAndStepsBFS(string start)
        {
            queryList = rep.GetPathAndStepsBFS(start);
            if (queryList.Count > 0)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region Iterative Deepening Search

        public void AddEdgesToDbIDS()
        {
            rep.AddEdgesIDS(edgeList);
        }
        public void AddGoalToDbIDS(string goal)
        {
            rep.AddGoalIDS(goal);
        }
        public bool GetPathAndStepsIDS(string start)
        {
            queryList = rep.GetPathAndStepsIDS(start);
            if (queryList.Count > 0)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region Best First Search

        public void AddEdgesToDbBestFS()
        {
            rep.AddEdgesBestFS(edgeList);
        }
        public void AddHeuristicToDb()
        {
            rep.AddHeuristicBestFS(nodeList);
        }
        public void AddGoalToDbBestFS(string goal)
        {
            rep.AddGoalBestFS(goal);
        }
        public void AddStartToDbBestFS(string goal)
        {
            rep.AddStartBestFS(goal);
        }
        public bool GetPathAndStepsBestFS()
        {
            queryList = rep.GetPathAndStepsBestFS();
            if (queryList.Count > 0)
            {
                return true;
            }
            return false;
        }
        public string[] GetStepsBestFS()
        {
            if (queryList.Count > 0)
            {
                #region getSteps

                string stepsString = queryList[1];
                stepsString = stepsString.Replace("[", "");
                stepsString = stepsString.Replace("]", "");
                var traversalList = stepsString.Split(',');
                for (var i = 0; i < traversalList.Length; i++)
                {
                    var nodeFormat = traversalList[i].Split('-');
                    traversalList[i] = nodeFormat[1] + "=" + nodeFormat[0];
                }
                #endregion

                return traversalList;
            }
            return null;
        }
        
        #endregion
        
    }
}
