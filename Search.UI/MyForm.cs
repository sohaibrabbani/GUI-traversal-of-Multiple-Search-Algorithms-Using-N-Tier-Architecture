using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Glee.Drawing;
using Search.BLL;
using Search.CL;
using Edge = Microsoft.Glee.Drawing.Edge;

namespace Search.UI
{
    public partial class MyForm : Form
    {
        #region Data Types Declarations
        ToolTip toolTip = new ToolTip();
        Graph g = new Graph("graph");
        Manager mgr = new Manager();
        Graph gTemp;
        object selectedObjectAttr;
        object selectedObject;
        bool createGraph= false, startSearch = false;
        bool path = false, steps = false;
        private string start, goal;
        Dictionary<string, decimal> heuristicList = new Dictionary<string, decimal>();
        #endregion

        #region Form Methods
        public MyForm()
        {
            Load += Form1_Load;
            InitializeComponent();
            panelBFS.Visible = false;
            panelIDS.Visible = false;
            panelCSP.Visible = false;
            panelBestFS.Visible = false;
            toolTip.Active = true;
            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 1000;
            toolTip.ReshowDelay = 500;
            // Force the ToolTip text to       toolTip.ShowAlways = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            gViewerIDS.SelectionChanged +=
             gViewer_SelectionChanged;
            gViewerBFS.SelectionChanged +=
                gViewer_SelectionChanged;
            
            Environment.SetEnvironmentVariable("SWI_HOME_DIR", @"C:\Program Files (x86)\swipl");
        }
        #endregion

        #region Graph Viewer Methods

        void gViewer_SelectionChanged(object sender, EventArgs e)
        {

            if (selectedObject != null)
            {
                if (selectedObject is Microsoft.Glee.Drawing.Edge)
                    ((Edge) selectedObject).Attr = selectedObjectAttr as EdgeAttr;
                else if (selectedObject is Node)
                    ((Node) selectedObject).Attr = selectedObjectAttr as NodeAttr;

                selectedObject = null;
            }

            if (gViewerIDS.SelectedObject == null)
            {
                label1.Text = "No object under the mouse";
                gViewerIDS.SetToolTip(toolTip, "");

            }
            else
            {
                selectedObject = gViewerIDS.SelectedObject;

                if (selectedObject is Edge)
                {
                    selectedObjectAttr = ((Edge) gViewerIDS.SelectedObject)?.Attr.Clone();
                    ((Edge) gViewerIDS.SelectedObject).Attr.Color = Color.Yellow;
                    ((Edge) gViewerIDS.SelectedObject).Attr.Fontcolor = Color.Yellow;
                    Edge edge = (Edge) gViewerIDS.SelectedObject;
                    
                    //here you can use e.Attr.Id or e.UserData to get back to you data
                    gViewerIDS.SetToolTip(toolTip, $"edge from {edge.Source} {edge.Target}");

                }
                else if (selectedObject is Node)
                {

                    selectedObjectAttr = ((Node) gViewerIDS.SelectedObject)?.Attr.Clone();
                    (selectedObject as Node).Attr.Color = Color.Yellow;
                    (selectedObject as Node).Attr.Fontcolor = Color.Yellow;
                    //here you can use e.Attr.Id to get back to your data
                    gViewerIDS.SetToolTip(toolTip, $"node {((Node) selectedObject).Attr.Label}");
                }
                label1.Text = selectedObject.ToString();
            }
            gViewerIDS.Invalidate();
        }
        #endregion

        #region Iterative Deepening Search
        
        #region Clear & Set the Graph
        private void CreateNewGraph()
        {
            gTemp = new Graph("graph");
            foreach (var edges in mgr.GetEdgeList())
            {
                gTemp.AddEdge(edges.Source, edges.Target);
            }
            foreach (var node in mgr.GetVertexList())
            {
                Node n = gTemp.AddNode(node.Name);
                n.Attr.Shape = Shape.Circle;
            }
            g = gTemp;

        }
        private void SetStartGoal()
        {

            g.FindNode(start).Attr.Shape = Shape.DoubleCircle;
            g.FindNode(goal).Attr.Shape = Shape.DoubleCircle;
            if (panelBFS.Visible)
            {
                gViewerBFS.Graph = g;
                gViewerBFS.Invalidate();
            }
            else if (panelIDS.Visible)
            {
                gViewerIDS.Graph = g;
                gViewerIDS.Invalidate();
            }
            Thread.Sleep(1000);
            steps = true;
        }
        #endregion

        #region Get Vertex Info
        private void btnVertexName_Click(object sender, EventArgs e)
        {
            if (!listFromIDS.Items.Contains(txtVertexNameIDS.Text))
            {
                var regex = new Regex("^[a-z]+$");
                if (regex.IsMatch(txtVertexNameIDS.Text))
                {
                    listFromIDS.Items.Add(txtVertexNameIDS.Text);
                    listToIDS.Items.Add(txtVertexNameIDS.Text);
                    listStartIDS.Items.Add(txtVertexNameIDS.Text);
                    listGoalIDS.Items.Add(txtVertexNameIDS.Text);
                    mgr.AddVertex(new Vertex(txtVertexNameIDS.Text));

                }
            }
            else
            {
                MessageBox.Show("Vertex already exists.");
            }
            txtVertexNameIDS.Text = "";
            txtVertexNameIDS.Focus();
        }
        #endregion

        #region Get Edge Info
        private void btnEdge_Click(object sender, EventArgs e)
        {
            bool flag = false;
            if ((listFromIDS.SelectedIndex > -1 && listToIDS.SelectedIndex > -1))
            {
                if (listFromIDS.SelectedItem.ToString() != listToIDS.SelectedItem.ToString())
                {
                    foreach (var edge in mgr.GetEdgeList())
                    {
                        if ((edge.Source == listFromIDS.SelectedItem.ToString()) && (edge.Target == listToIDS.SelectedItem.ToString()))
                        {
                            flag = true;
                            MessageBox.Show("Edge exists already.");
                        }
                    }

                    if (!flag)
                        mgr.AddEdge(new CL.Edge(listFromIDS.SelectedItem.ToString(), listToIDS.SelectedItem.ToString()));
                    listToIDS.Focus();
                }
                else
                {
                    MessageBox.Show("Vertices cannot be same");
                }
            }
            else
            {
                MessageBox.Show("Select both vertices");
            }
        }
        #endregion

        #region Show Steps
        private void btnSteps_Click(object sender, EventArgs e)
        {
            if (createGraph)
            {
                if (startSearch)
                {
                    if (listStartIDS.SelectedIndex > -1 && listGoalIDS.SelectedIndex > -1)
                    {
                        if (steps || path)
                        {
                            CreateNewGraph();
                        }
                        goal = listGoalIDS.SelectedItem.ToString();
                        start = listStartIDS.SelectedItem.ToString();
                        SetStartGoal();
                        if (bGroundWrkStepsIDS.IsBusy != true)
                        {
                            // Start the asynchronous operation.
                            bGroundWrkStepsIDS.RunWorkerAsync();
                        }
                    }
                }
                else
                    MessageBox.Show("Start the search first.");
            }
            else
            {
                MessageBox.Show("Create the graph first.");
            }
        }

        private void bGroundWrkSteps_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
            Invoke((MethodInvoker)delegate
            {
                g.FindNode(listStartIDS.SelectedItem.ToString()).Attr.Fillcolor = Color.Orange;
                gViewerIDS.Graph = g;
                gViewerIDS.Invalidate();
            });
            foreach (var nodes in mgr.GetSteps())
            {
                int num = 0;
                Thread.Sleep(1000);
                if (!Int32.TryParse(nodes, out num))
                {
                    g.FindNode(nodes).Attr.Fillcolor = Color.Orange;
                    Invoke((MethodInvoker)delegate
                    {
                        gViewerIDS.Graph = g;
                        gViewerIDS.Invalidate();
                    });
                }
                else
                {
                    CreateNewGraph();
                    Invoke((MethodInvoker)delegate
                    {

                        g.FindNode(listGoalIDS.SelectedItem.ToString()).Attr.Shape = Shape.DoubleCircle;
                        g.FindNode(listStartIDS.SelectedItem.ToString()).Attr.Shape = Shape.DoubleCircle;
                        gViewerIDS.Graph = g;
                        gViewerIDS.Invalidate();

                    });
                    Thread.Sleep(1000);
                    Invoke((MethodInvoker)delegate
                    {
                        g.FindNode(listStartIDS.SelectedItem.ToString()).Attr.Fillcolor = Color.Orange;
                        gViewerIDS.Graph = g;
                        gViewerIDS.Invalidate();

                    });
                    steps = true;
                }
            }
        }
        #endregion

        #region Show Path
        private void btnPath_Click(object sender, EventArgs e)
        {
            if (createGraph)
            {
                if (startSearch)
                {
                    if (listStartIDS.SelectedIndex > -1 && listGoalIDS.SelectedIndex > -1)
                    {
                        if (steps || path)
                        {
                            CreateNewGraph();
                        }
                        goal = listGoalIDS.SelectedItem.ToString();
                        start = listStartIDS.SelectedItem.ToString();
                        SetStartGoal();
                        if (bGroundWrkPathIDS.IsBusy != true)
                        {
                            // Start the asynchronous operation.
                            bGroundWrkPathIDS.RunWorkerAsync();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Start the search first.");
                }
            }
            else
            {
                MessageBox.Show("Create the graph first.");
            }
        }
        private void bGroundWrkPath_DoWork(object sender, DoWorkEventArgs e)
        {
            var pathList = mgr.GetPath();
            foreach (string vertex in pathList)
            {
                Thread.Sleep(1000);
                g.FindNode(vertex).Attr.Fillcolor = Color.Red;
                Invoke((MethodInvoker)delegate
                {
                    gViewerIDS.Graph = g;
                    gViewerIDS.Invalidate();
                });
            }
            path = true;
        }
        #endregion

        #region Create the Graph
        private void btnCreateGraph_Click(object sender, EventArgs e)
        {
            g.GraphAttr.NodeAttr.Padding = 3;

            Node n;
            foreach (var edges in mgr.GetEdgeList())
            {
                g.AddEdge(edges.Source, edges.Target);
            }
            var vList = mgr.GetVertexList();
            foreach (var node in vList)
            {
                n = g.AddNode(node.Name);
                n.Attr.Shape = Shape.Circle;
            }
            if (vList.Count > 0)
                createGraph = true;
            //layout the graph and draw it
            gViewerIDS.Graph = g;
        }
        #endregion

        #region Start the Search
        private void btnStartSearch_Click(object sender, EventArgs e)
        {
            if (createGraph)
            {
                if (listStartIDS.SelectedIndex > -1 && listGoalIDS.SelectedIndex > -1)
                    if (listStartIDS.SelectedItem.ToString() != listGoalIDS.SelectedItem.ToString())
                    {

                        mgr.AddEdgesToDbIDS();
                        mgr.AddGoalToDbIDS(listGoalIDS.SelectedItem.ToString());
                        if (mgr.GetPathAndStepsIDS(listStartIDS.SelectedItem.ToString()))
                        {
                            startSearch = true;
                        }

                    }
                    else
                    {
                        MessageBox.Show("Start & Goal state cannot be same");
                    }
            }
            else
            {
                MessageBox.Show("Create the graph first.");
            }
        }
        #endregion

        #region Reset the Form
        private void btnReset_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
        #endregion

        #region OnClick of IDS Button
        private void btnIDS_Click(object sender, EventArgs e)
        {
            this.Text = "Iterative Deepening Search";
            panelButtons.Visible = false;
            panelIDS.Visible = true;
        }


        #endregion
        #endregion

        #region Breadth First Search

        #region Reset the Form
        private void btnResetBFS_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
        #endregion

        #region Get Vertex Info
        private void btnVertexBFS_Click(object sender, EventArgs e)
        {
            if (!listFromBFS.Items.Contains(txtVertexNameBFS.Text))
            {
                var regex = new Regex("^[a-z]+$");
                if (regex.IsMatch(txtVertexNameBFS.Text))
                {
                    listFromBFS.Items.Add(txtVertexNameBFS.Text);
                    listToBFS.Items.Add(txtVertexNameBFS.Text);
                    listStartBFS.Items.Add(txtVertexNameBFS.Text);
                    listGoalBFS.Items.Add(txtVertexNameBFS.Text);
                    mgr.AddVertex(new Vertex(txtVertexNameBFS.Text));
                }
            }
            else
            {
                MessageBox.Show("Vertex already exists.");
            }
            txtVertexNameBFS.Text = "";
            txtVertexNameBFS.Focus();
        }
        #endregion

        #region Get Edge Info
        private void btnEdgeBFS_Click(object sender, EventArgs e)
        {
            bool flag = false;
            if ((listFromBFS.SelectedIndex > -1 && listToBFS.SelectedIndex > -1))
            {
                if (listFromBFS.SelectedItem.ToString() != listToBFS.SelectedItem.ToString())
                {
                    foreach (var edge in mgr.GetEdgeList())
                    {
                        if ((edge.Source == listFromBFS.SelectedItem.ToString()) && (edge.Target == listToBFS.SelectedItem.ToString()))
                        {
                            flag = true;
                            MessageBox.Show("Edge exists already.");
                        }
                    }
                    if (!flag)
                    {
                        mgr.AddEdge(new CL.Edge(listFromBFS.SelectedItem.ToString(), listToBFS.SelectedItem.ToString()));
                    }
                    listToBFS.Focus();
                }
                else
                {
                    MessageBox.Show("Vertices cannot be same");
                }
            }
            else
            {
                MessageBox.Show("Select both vertices");
            }
        }

        #endregion

        #region Start the Search
        private void btnStartSearchBFS_Click(object sender, EventArgs e)
        {
            if (createGraph)
            {
                if (listStartBFS.SelectedIndex > -1 && listGoalBFS.SelectedIndex > -1)
                    if (listStartBFS.SelectedItem.ToString() != listGoalBFS.SelectedItem.ToString())
                    {
                        mgr.AddEdgesToDbBFS();
                        mgr.AddGoalToDbBFS(listGoalBFS.SelectedItem.ToString());
                        if (mgr.GetPathAndStepsBFS(listStartBFS.SelectedItem.ToString()))
                        {
                            startSearch = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Start & Goal state cannot be same");
                    }
            }
            else
            {
                MessageBox.Show("Create the graph first.");
            }
        }
        #endregion

        #region Show Path
        private void btnPathBFS_Click(object sender, EventArgs e)
        {
            if (createGraph)
            {
                if (startSearch)
                {
                    if (listStartBFS.SelectedIndex > -1 && listGoalBFS.SelectedIndex > -1)
                    {
                        if (steps || path)
                        {
                            CreateNewGraph();
                        }
                        goal = listGoalBFS.SelectedItem.ToString();
                        start = listStartBFS.SelectedItem.ToString();
                        SetStartGoal();
                        if (bGroundWrkPathBFS.IsBusy != true)
                        {
                            // Start the asynchronous operation.
                            bGroundWrkPathBFS.RunWorkerAsync();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Start the search first.");
                }
            }
            else
            {
                MessageBox.Show("Create the graph first.");
            }
        }
        private void bGroundWrkPathBFS_DoWork(object sender, DoWorkEventArgs e)
        {
            var pathList = mgr.GetPath();
            for (var i = pathList.Length - 1; i >= 0; i--)
            {
                Thread.Sleep(1000);
                (g.FindNode(pathList[i]) as Node).Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Red;
                this.Invoke((MethodInvoker)delegate
                {
                    gViewerBFS.Graph = g;
                    gViewerBFS.Invalidate();
                });
            }
            path = true;
        }
        #endregion

        #region Show Steps
        private void btnStepsBFS_Click(object sender, EventArgs e)
        {
            if (createGraph)
            {
                if (startSearch)
                {
                    if (listStartBFS.SelectedIndex > -1 && listGoalBFS.SelectedIndex > -1)
                    {
                        if (steps || path)
                        {
                            CreateNewGraph();
                        }
                        goal = listGoalBFS.SelectedItem.ToString();
                        start = listStartBFS.SelectedItem.ToString();
                        SetStartGoal();
                        if (bGroundWrkStepsBFS.IsBusy != true)
                        {
                            // Start the asynchronous operation.
                            bGroundWrkStepsBFS.RunWorkerAsync();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Start the search first.");
                }
            }
            else
            {
                MessageBox.Show("Create the graph first.");
            }

        }

        private void bGroundWrkStepsBFS_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
            this.Invoke((MethodInvoker)delegate
            {
                g.FindNode(listStartBFS.SelectedItem.ToString()).Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Orange;
                gViewerBFS.Graph = g;
                gViewerBFS.Invalidate();
            });
            var traversalList = mgr.GetSteps();
            for (var nodes = traversalList.Length - 1; nodes >= 0; nodes--)
            {
                Thread.Sleep(1000);
                (g.FindNode(traversalList[nodes]) as Node).Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Orange;
                this.Invoke((MethodInvoker)delegate
                {
                    gViewerBFS.Graph = g;
                    gViewerBFS.Invalidate();
                });
            }
            steps = true;
        }

        #endregion

        #region Create the Graph
        private void btnCreateGraphBFS_Click(object sender, EventArgs e)
        {
            g.GraphAttr.NodeAttr.Padding = 3;

            Node n;
            foreach (var edges in mgr.GetEdgeList())
            {
                g.AddEdge(edges.Source, edges.Target);
            }
            var nodes = mgr.GetVertexList();
            foreach (var node in nodes)
            {
                n = g.AddNode(node.Name);
                n.Attr.Shape = Microsoft.Glee.Drawing.Shape.Circle;
            }

            //layout the graph and draw it
            gViewerBFS.Graph = g;
            if (nodes.Count > 0)
                createGraph = true;
        }

        #endregion

        #region OnClick of BFS Button
        private void btnBFS_Click(object sender, EventArgs e)
        {
            Text = "Breadth First Search";
            panelButtons.Visible = false;
            panelBFS.Visible = true;
        }

        #endregion

        #endregion

        #region Constraint Satisfaction Problem

        #region OnClick of CSP Button
        private void btnCSP_Click(object sender, EventArgs e)
        {
            Text = "Map Colouring CSP";
            panelButtons.Visible = false;
            panelCSP.Visible = true;
        }
        #endregion

        #region Get Vertex Info
        private void btnVertexCSP_Click(object sender, EventArgs e)
        {
            if (!listFromCSP.Items.Contains(txtVertexNameCSP.Text))
            {
                var regex = new Regex("^[a-z]+$");
                if (regex.IsMatch(txtVertexNameCSP.Text))
                {
                    listFromCSP.Items.Add(txtVertexNameCSP.Text);
                    listToCSP.Items.Add(txtVertexNameCSP.Text);
                    mgr.AddVertexCSP(new Vertex(txtVertexNameCSP.Text));
                }
            }
            else
            {
                MessageBox.Show("Vertex already exists.");
            }
            txtVertexNameCSP.Text = "";
            txtVertexNameCSP.Focus();
        }
        #endregion

        #region Get Edge Info
        private void btnEdgeCSP_Click(object sender, EventArgs e)
        {
            bool flag = false;
            if ((listFromCSP.SelectedIndex > -1 && listToCSP.SelectedIndex > -1))
            {
                if (listFromCSP.SelectedItem.ToString() != listToCSP.SelectedItem.ToString())
                {
                    foreach (var i in mgr.GetEdgeList())
                    {
                        if (((i.Source == listFromCSP.SelectedItem.ToString()) && (i.Target == listToCSP.SelectedItem.ToString())) || ((i.Target == listFromCSP.SelectedItem.ToString()) && (i.Source == listToCSP.SelectedItem.ToString())))
                        {
                            flag = true;
                            MessageBox.Show("Edge exists already.");
                        }
                    }
                    if (!flag)
                        mgr.AddEdgeCSP(new CL.Edge(listFromCSP.SelectedItem.ToString(), listToCSP.SelectedItem.ToString()));
                    listToCSP.Focus();

                }
                else
                {
                    MessageBox.Show("Vertices cannot be same");
                }
            }
            else
            {
                MessageBox.Show("Select both vertices");
            }
        }
        #endregion

        #region Show Path
        private void btnPathCSP_Click(object sender, EventArgs e)
        {
            if (createGraph)
            {
                if (startSearch)
                {
                    if (steps || path)
                    {
                        CreateNewGraph();
                    }
                    steps = true;
                    if (bGroundWrkPathCSP.IsBusy != true)
                    {
                        // Start the asynchronous operation.
                        bGroundWrkPathCSP.RunWorkerAsync();
                    }
                }
                else
                {
                    MessageBox.Show("Start the search first.");
                }
            }
            else
            {
                MessageBox.Show("Create the graph first.");
            }
        }
        private void bGroundWrkPathCSP_DoWork(object sender, DoWorkEventArgs e)
        {
            gViewerCSP.Graph = g;
            gViewerCSP.Invalidate();
            Thread.Sleep(1000);
            var pathList = mgr.GetPath();
            for (var i = pathList.Length - 1; i >= 0; i--)
            {
                if (pathList[i].Split('/')[1] == "red")
                {
                    (g.FindNode(pathList[i].Split('/')[0]) as Node).Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Red;
                }
                else if (pathList[i].Split('/')[1] == "blue")
                {
                    (g.FindNode(pathList[i].Split('/')[0]) as Node).Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Blue;
                }
                else if (pathList[i].Split('/')[1] == "green")
                {
                    (g.FindNode(pathList[i].Split('/')[0]) as Node).Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Green;
                }
                this.Invoke((MethodInvoker)delegate
                {
                    gViewerCSP.Graph = g;
                    gViewerCSP.Invalidate();
                });
            }
        }
        #endregion

        #region Show Steps
        private void btnStepsCSP_Click(object sender, EventArgs e)
        {
            if (createGraph)
            {
                if (startSearch)
                {
                    if (steps || path)
                    {
                        CreateNewGraph();
                    }
                    steps = true;
                    if (bGroundWrkStepsCSP.IsBusy != true)
                    {
                        // Start the asynchronous operation.
                        bGroundWrkStepsCSP.RunWorkerAsync();
                    }
                }
                else
                {
                    MessageBox.Show("Start the search first.");
                }
            }
            else
            {
                MessageBox.Show("Create the graph first.");
            }
        }

        private void bGroundWrkStepsCSP_DoWork(object sender, DoWorkEventArgs e)
        {
            gViewerCSP.Graph = g;
            gViewerCSP.Invalidate();
            Thread.Sleep(1000);
            foreach (var nodes in mgr.GetSteps())
            {
                if (nodes.Split('/')[1] == "red")
                {
                    (g.FindNode(nodes.Split('/')[0]) as Node).Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Red;
                }
                else if (nodes.Split('/')[1] == "blue")
                {
                    (g.FindNode(nodes.Split('/')[0]) as Node).Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Blue;
                }
                else if (nodes.Split('/')[1] == "green")
                {
                    (g.FindNode(nodes.Split('/')[0]) as Node).Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Green;
                }
                //(g.FindNode(nodes) as Node).Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Orange;
                this.Invoke((MethodInvoker)delegate
                {
                    gViewerCSP.Graph = g;
                    gViewerCSP.Invalidate();
                });
                Thread.Sleep(1000);
            }
        }

        #endregion

        #region Create the Graph
        private void btnCreateGraphCSP_Click(object sender, EventArgs e)
        {
            g.GraphAttr.NodeAttr.Padding = 3;
            Node n;
            foreach (var edges in mgr.GetEdgeList())
            {
                g.AddEdge(edges.Source, edges.Target);
            }
            var nodes = mgr.GetVertexList();
            foreach (var node in nodes)
            {
                n = g.AddNode(node.Name);
                n.Attr.Shape = Microsoft.Glee.Drawing.Shape.Circle;
            }
            //layout the graph and draw it
            gViewerCSP.Graph = g;
            if (nodes.Count > 0)
                createGraph = true;

        }
        #endregion

        #region Start the Search
        private void btnStartSearchCSP_Click(object sender, EventArgs e)
        {
            if (createGraph)
            {
                mgr.AddNeighboursToDbBFS();
                if (mgr.GetPathAndStepsCSP())
                {
                    startSearch = true;
                }
            }
            else
                MessageBox.Show("Create a graph first.");
        }
        #endregion

        #region Reset the Form
        private void btnResetCSP_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
        #endregion

        #endregion

        #region Best First Search

        #region Reset the Form
        private void btnResetBestFS_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
        #endregion

        #region Get Vertex Info

        private void btnVertexBestFS_Click(object sender, EventArgs e)
        {
            if (!listFromBestFS.Items.Contains(txtVertexBestFS.Text))
            {
                var regex = new Regex("^[a-z]+$");
                if (regex.IsMatch(txtVertexBestFS.Text))
                {
                    listFromBestFS.Items.Add(txtVertexBestFS.Text + "=" + txtHeuristicBestFS.Value);
                    listToBestFS.Items.Add(txtVertexBestFS.Text + "=" + txtHeuristicBestFS.Value);
                    listStartBestFS.Items.Add(txtVertexBestFS.Text + "=" + txtHeuristicBestFS.Value);
                    listGoalBestFS.Items.Add(txtVertexBestFS.Text + "=" + txtHeuristicBestFS.Value);
                    heuristicList.Add(txtVertexBestFS.Text, txtHeuristicBestFS.Value);
                    mgr.AddVertex(new Vertex(txtVertexBestFS.Text + "=" + txtHeuristicBestFS.Value));

                }
            }
            else
            {
                MessageBox.Show("Vertex already exists.");
            }
            txtVertexBestFS.Text = "";
            txtVertexBestFS.Focus();
        }
        #endregion

        #region Get Edge Info
        private void btnEdgeBestFS_Click(object sender, EventArgs e)
        {
            bool flag = false;
            if ((listFromBestFS.SelectedIndex > -1 && listToBestFS.SelectedIndex > -1))
            {
                if (listFromBestFS.SelectedItem.ToString() != listToBestFS.SelectedItem.ToString())
                {
                    foreach (var edge in mgr.GetEdgeList())
                    {
                        if ((edge.Source == listFromBestFS.SelectedItem.ToString()) && (edge.Target == listToBestFS.SelectedItem.ToString()))
                        {
                            flag = true;
                            MessageBox.Show("Edge exists already.");
                        }
                    }
                    if (!flag)
                        mgr.AddEdge(new CL.Edge(listFromBestFS.SelectedItem.ToString(), listToBestFS.SelectedItem.ToString()));
                    listToBestFS.Focus();
                }
                else
                {
                    MessageBox.Show("Vertices cannot be same");
                }
            }
            else
            {
                MessageBox.Show("Select both vertices");
            }
        }
        #endregion

        #region Start the Search
        private void btnStartSearchBestFS_Click(object sender, EventArgs e)
        {
            if (createGraph)
            {
                if (listStartBestFS.SelectedIndex > -1 && listGoalBestFS.SelectedIndex > -1)
                    if (listStartBestFS.SelectedItem.ToString() != listGoalBestFS.SelectedItem.ToString())
                    {
                        mgr.AddEdgesToDbBestFS();
                        mgr.AddHeuristicToDb();
                        mgr.AddStartToDbBestFS(listStartBestFS.SelectedItem.ToString());
                        mgr.AddGoalToDbBestFS(listGoalBestFS.SelectedItem.ToString());
                        if (mgr.GetPathAndStepsBestFS())
                        {
                            startSearch = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Start & Goal state cannot be same");
                    }
            }
            else
                MessageBox.Show("Create the graph first");
        }
        #endregion

        #region Show Path
        private void btnPathBestFS_Click(object sender, EventArgs e)
        {
            if (createGraph)
            {
                if (startSearch)
                {
                    if (listStartBestFS.SelectedIndex > -1 && listGoalBestFS.SelectedIndex > -1)
                    {
                        if (steps || path)
                        {
                            CreateNewGraph();
                        }
                        goal = listGoalBestFS.SelectedItem.ToString();
                        start = listStartBestFS.SelectedItem.ToString();
                        SetStartGoal();
                        if (bGroundWrkPathBestFS.IsBusy != true)
                        {
                            // Start the asynchronous operation.
                            bGroundWrkPathBestFS.RunWorkerAsync();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Start the search first.");
                }
            }
            else
            {
                MessageBox.Show("Create the graph first.");
            }
        }

        private void bGroundWrkPathBestFS_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (var nodes in mgr.GetPath())
            {

                Thread.Sleep(1000);
                string s = nodes + "=" + heuristicList[nodes];
                g.FindNode(nodes + "=" + heuristicList[nodes]).Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Red;
                this.Invoke((MethodInvoker)delegate
                {
                    gViewerBestFS.Graph = g;
                    gViewerBestFS.Invalidate();
                });
            }
            path = true;
        }

        #endregion

        #region Show Steps
        private void btnStepsBestFS_Click(object sender, EventArgs e)
        {
            if (createGraph)
            {
                if (startSearch)
                {
                    if (listStartBestFS.SelectedIndex > -1 && listGoalBestFS.SelectedIndex > -1)
                    {
                        if (steps || path)
                        {
                            CreateNewGraph();
                        }
                        goal = listGoalBestFS.SelectedItem.ToString();
                        start = listStartBestFS.SelectedItem.ToString();
                        SetStartGoal();
                        if (bGroundWrkStepsBestFS.IsBusy != true)
                        {
                            // Start the asynchronous operation.
                            bGroundWrkStepsBestFS.RunWorkerAsync();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Start the search first.");
                }
            }
            else
            {
                MessageBox.Show("Create the graph first.");
            }
        }

        private void bGroundWrkStepsBestFS_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
            this.Invoke((MethodInvoker)delegate
            {
                g.FindNode(listStartBestFS.SelectedItem.ToString()).Attr.Fillcolor =
                    Microsoft.Glee.Drawing.Color.Orange;
                gViewerBestFS.Graph = g;
                gViewerBestFS.Invalidate();
            });
            foreach (var nodes in mgr.GetStepsBestFS())
            {
                Thread.Sleep(1000);
                (g.FindNode(nodes) as Node).Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Orange;
                this.Invoke((MethodInvoker)delegate
                {
                    gViewerBestFS.Graph = g;
                    gViewerBestFS.Invalidate();
                });
            }
            steps = true;
        }

        #endregion

        #region Create the Graph
        private void btnCreatGraphBestFS_Click(object sender, EventArgs e)
        {
            //this is abstract.dot of GraphViz
            g.GraphAttr.NodeAttr.Padding = 3;
            Node n;
            foreach (var edges in mgr.GetEdgeList())
            {
                g.AddEdge(edges.Source, edges.Target);
            }
            var nodes = mgr.GetVertexList();
            foreach (var node in nodes)
            {
                n = g.AddNode(node.Name);
                n.Attr.Shape = Microsoft.Glee.Drawing.Shape.Circle;
            }

            //layout the graph and draw it
            gViewerBestFS.Graph = g;
            if (nodes.Count > 0)
                createGraph = true;
        }
        #endregion

        #region OnClick of BestFS Button
        private void btnBestFS_Click(object sender, EventArgs e)
        {

            Text = "Best First Search";
            panelButtons.Visible = false;
            panelBestFS.Visible = true;
        }
        #endregion

        #endregion


    }
}
