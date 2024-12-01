using System;

namespace FetchPoints.Entity.PointsRules {
    public class ItemRules : IPointsRule
    {
        public int Apply(ValidatedTransaction transaction)
        {
            var points = 0;

            // 5 points for every two items on the receipt.
            points += (int) Math.Floor(Convert.ToDouble(transaction.Items.Count / 2)) * 5;

            // If the trimmed length of the item description is a multiple of 3, multiply the price by 0.2 and round up to the nearest integer. The result is the number of points earned.
            transaction.Items.ForEach((item) => {
                var desriptionLength = item.ShortDescription.Trim().Length;
                if (desriptionLength % 3 == 0) {
                    points += (int) Math.Ceiling(item.Price * 0.2);
                }
            });

            return points;
        }
    }
}
