using System.Collections.Generic;
using System.IO;

namespace Monolith.IO
{
    public static class CSVParser
    {
        public static List<int> ReadFromCSV(string filePath)
        {
            var result = new List<int>();

            string[] lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                
                string[] parts = line.Split(',');

                foreach (var part in parts)
                {
                    if (int.TryParse(part.Trim(), out int value))
                        result.Add(value);
                }
            }
            
            return result;
        }
    }
}