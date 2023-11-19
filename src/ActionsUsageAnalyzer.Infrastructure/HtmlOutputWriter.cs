using System.IO;
using ActionsUsageAnalyser.Domain;

namespace ActionsUsageAnalyzer.Infrastructure
{
    public class HtmlOutputWriter : IOutputWriter
    {
        private readonly TextWriter writer;

        public HtmlOutputWriter(string filePath)
        {
            writer = File.CreateText(filePath);
            writer.WriteLine("<html>");
            writer.WriteLine("<head>");
            writer.WriteLine("<style>");
            writer.WriteLine("body { background-color: red; }");
            writer.WriteLine("h1, h2, h3, h4, h5, h6 { color: green; }");
            writer.WriteLine("</style>");
            writer.WriteLine("</head>");
            writer.WriteLine("<body>");
        }

        public void WriteLine(string line)
        {
            writer.WriteLine(line);
        }

        public void WriteTitle(int level, string line)
        {
            writer.WriteLine($"<h{level}>{line}</h{level}>");
        }

        public void BeginTable(params string[] headers)
        {
            writer.WriteLine("<table>");
            writer.WriteLine("<tr>");
            foreach (var header in headers)
            {
                writer.WriteLine($"<th>{header}</th>");
            }
            writer.WriteLine("</tr>");
        }

        public void WriteTableRow(params string[] columns)
        {
            writer.WriteLine("<tr>");
            foreach (var column in columns)
            {
                writer.WriteLine($"<td>{column}</td>");
            }
            writer.WriteLine("</tr>");
        }

        public void EndTable()
        {
            writer.WriteLine("</table>");
        }

        public void Dispose()
        {
            writer.WriteLine("</body>");
            writer.WriteLine("</html>");
            writer.Close();
            writer.Dispose();
        }
    }
}