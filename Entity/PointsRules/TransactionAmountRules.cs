namespace FetchPoints.Entity.PointsRules {
    internal class TransactionAmountRules : IPointsRule
    {
        public int Apply(ValidatedTransaction transaction)
        {
            var points = 0;

            // 50 points if the total is a round dollar amount with no cents.
            if (transaction.Total.ToString("F2").EndsWith(".00")) points += 50;

            // 25 points if the total is a multiple of 0.25.
            if (transaction.Total / 0.25 % 1 == 0) points += 25;
            return points;
        }
    }
}
