namespace FetchPoints.Entity.PointsRules {
    internal interface IPointsRule {
        internal abstract int Apply(ValidatedTransaction transaction);
    }
}