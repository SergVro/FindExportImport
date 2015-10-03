using System;

namespace Vro.FindExportImport
{
    public static class Helpers
    {
        public static string GetEntityName(string entityKey)
        {
            if (string.IsNullOrWhiteSpace(entityKey))
            {
                throw new ArgumentException("entityKey");
            }
            var name = "";
            foreach (var c in entityKey)
            {
                if (name.Length == 0)
                {
                    name = c.ToString().ToUpper();
                }
                else if (c.ToString().ToUpper().Equals(c.ToString(), StringComparison.InvariantCulture))
                {
                    name += " " + c;
                }
                else
                {
                    name += c;
                }
            }
            return name.Replace("Entity", "").Trim();
        }
    }
}