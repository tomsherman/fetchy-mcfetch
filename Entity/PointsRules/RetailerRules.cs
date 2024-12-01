using System.Text.RegularExpressions;

namespace FetchPoints.Entity.PointsRules {
    public class RetailerRules : IPointsRule
    {
        Regex alphanumericRegex = new Regex("[^a-zA-Z0-9]", RegexOptions.Compiled);

        public int Apply(ValidatedTransaction transaction)
        {
            var scrubbedName = alphanumericRegex.Replace(transaction.Retailer, "");
            return scrubbedName.Length;
        }
    }
}