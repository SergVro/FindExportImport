using System;

namespace Vro.FindExportImport
{
    public static class Helpers
    {
        public static string GetEntityName(string entityKey)
        {
            if (String.IsNullOrWhiteSpace(entityKey))
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

        public const string AllSitesId = "84bfaf5c52a349a0bc61a9ffb6983a66";
        public const string AllLanguages = "7d2da0a9fc754533b091fa6886a51c0d";
        public const string PageBestBetSelector = "PageBestBetSelector";
        public const string CommerceBestBetSelector = "CommerceBestBetSelector";
    }
}