namespace Search.CL
{
    public class Edge
    {
        public string Source { get; set; }
        public string Target { get; set; }

        public Edge(string from, string to)
        {
            Source = from;
            Target = to;
        }
    }
}