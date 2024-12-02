namespace FetchPoints.Entity.PointsRules {
    internal class TransactionTimestampRules : IPointsRule
    {
        public int Apply(ValidatedTransaction transaction)
        {
            var points = 0;

            // 6 points if the day in the purchase date is odd.
            if (transaction.Timestamp.Day % 2 == 1) points += 6;

            // 10 points if the time of purchase is after 2:00pm and before 4:00pm.
            if (14 <= transaction.Timestamp.Hour && transaction.Timestamp.Hour <= 16) points += 10;

            return points;
        }
    }
}
