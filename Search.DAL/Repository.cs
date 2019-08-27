using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Search.CL;
using System.IO;
using SbsSW.SwiPlCs;

namespace Search.DAL
{
    public class Repository
    {
        #region Iterative Deepening Search

        public void AddEdgesIDS(List<Edge> edgeList)
        {
            string file = File.ReadAllText("IDS.pl");
            File.Delete("IDS(Temp).pl");
            using (StreamWriter write = new StreamWriter(@"IDS(Temp).pl"))
            {
                write.WriteLine(file);
                foreach (var edges in edgeList)
                {
                    write.WriteLine("arc(" + edges.Source + "," + edges.Target + ").");
                }
            }
        }

        public void AddGoalIDS(string goal)
        {
            using (StreamWriter write = new StreamWriter(@"IDS(Temp).pl", true))
            {
                write.WriteLine("goal(" + goal + ").");
            }
        }

        public List<string> GetPathAndStepsIDS(string start)
        {
            List<String> listBox = new List<string>();
            string[] p = {"-q", "-f", @"IDS(Temp).pl"};
            if (!PlEngine.IsInitialized)
                PlEngine.Initialize(p);
            if (PlEngine.IsInitialized)
            {
                using (PlQuery consult = new PlQuery("iterativeDeepening(" + start + ",Path,Steps)"))
                {
                    foreach (var v in consult.SolutionVariables)
                    {
                        listBox.Add(v["Path"].ToString());
                        listBox.Add(v["Steps"].ToString());
                    }
                }
                PlEngine.PlCleanup();
            }
            return listBox;
        }

        #endregion

        #region Breadth First Search

        public void AddEdgesBFS(List<Edge> edgeList)
        {
            string file = File.ReadAllText("BFS_1.pl");
            File.Delete("BFS.pl");
            using (StreamWriter write = new StreamWriter(@"BFS.pl"))
            {
                write.WriteLine(file);
                foreach (var edges in edgeList)
                {
                    write.WriteLine("arc(" + edges.Source + "," + edges.Target + ").");
                }
            }
        }

        public void AddGoalBFS(string goal)
        {
            using (StreamWriter write = new StreamWriter(@"BFS.pl", true))
            {
                write.WriteLine("goal(" + goal + ").");
            }
        }

        public List<string> GetPathAndStepsBFS(string start)
        {
            List<String> listBox = new List<string>();
            string[] p = {"-q", "-f", @"BFS.pl"};
            if (!PlEngine.IsInitialized)
                PlEngine.Initialize(p);
            if (PlEngine.IsInitialized)
            {
                using (PlQuery consult = new PlQuery("bfs(" + start + ",Path,Steps)"))
                {
                    foreach (var v in consult.SolutionVariables)
                    {
                        listBox.Add(v["Path"].ToString());
                        listBox.Add(v["Steps"].ToString());
                    }
                }
                PlEngine.PlCleanup();
            }
            return listBox;
        }

        #endregion

        #region Constraint Satisfaction Problem

        public void AddNeighboursCSP(Dictionary<string, string> neighbours)
        {
            string file = File.ReadAllText("CSP.pl");
            File.Delete("CSP(Temp).pl");
            using (StreamWriter write = new StreamWriter(@"CSP(Temp).pl"))
            {
                write.WriteLine(file);
                foreach (var neighbour in neighbours)
                {
                    write.WriteLine("neighbour(" + neighbour.Key + ",[" + neighbour.Value + "]).");
                }
            }
        }

        public List<string> GetPathAndStepsCSP()
        {
            List<String> listBox = new List<string>();
            string[] p = {"-q", "-f", @"CSP(Temp).pl"};
            if (!PlEngine.IsInitialized)
                PlEngine.Initialize(p);
            if (PlEngine.IsInitialized)
            {
                PlQuery consult = new PlQuery("colour_countries(X,List)");

                foreach (var v in consult.SolutionVariables)
                {
                    listBox.Add(v["X"].ToString());
                    listBox.Add(v["List"].ToString());
                }
            }
            return listBox;
        }
        #endregion

        #region Best First Search

        public void AddEdgesBestFS(List<Edge> edgeList)
        {
            string file = File.ReadAllText("bestFirstSearch.pl");
            File.Delete("BFS.pl");
            using (StreamWriter write = new StreamWriter(@"BFS.pl"))
            {
                write.WriteLine(file);
                foreach (var edges in edgeList)
                {
                    write.WriteLine("arc(" + edges.Source.Split('=')[0] + "," + edges.Target.Split('=')[0] + ").");
                }
            }
        }
        public void AddHeuristicBestFS(List<Vertex> nodeList)
        {
            using (StreamWriter write = new StreamWriter(@"BFS.pl", true))
            {
                foreach (var nodes in nodeList)
                {
                    write.WriteLine("h(" + nodes.Name.Split('=')[0] + "," + nodes.Name.Split('=')[1] + ").");
                }
            }
        }

        public void AddStartBestFS(string start)
        {
            using (StreamWriter write = new StreamWriter(@"BFS.pl", true))
            {
                write.WriteLine("start(" + start.Split('=')[0] + ").");
            }
        }

        public void AddGoalBestFS(string goal)
        {
            using (StreamWriter write = new StreamWriter(@"BFS.pl", true))
            {
                write.WriteLine("goal(" + goal.Split('=')[0] + ").");
            }
        }

        public List<string> GetPathAndStepsBestFS()
        {
            List<string> listBox = new List<string>();
            string[] p = {"-q", "-f", @"BFS.pl"};
            if (!PlEngine.IsInitialized)
                PlEngine.Initialize(p);
            if (PlEngine.IsInitialized)
            {
                PlQuery path = new PlQuery("bestfs(Path,Steps)");
                foreach (var v in path.SolutionVariables)
                {
                    listBox.Add(v["Path"].ToString());
                    listBox.Add(v["Steps"].ToString());
                }

            }
            return listBox;
        }

        #endregion
    }
}