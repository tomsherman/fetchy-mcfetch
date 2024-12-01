namespace FetchPoints.Entity.PointsRules {
    public interface IPointsRule {
        public abstract int Apply(ValidatedTransaction transaction);
    }
}