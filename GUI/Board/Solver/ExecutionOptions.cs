namespace Solver
{
	public class ExecutionOptions
	{
		public double MinEstimation { get; set; }

		public double MaxHeight { get; set; }

		public double MaxWidth { get; set; }

		// Variables

		// 10?
		public double AttractorRatio { get; set; }

		// 3 ?
		public double EdgeRatio { get; set; }

		// 20?
		public double DepthPenaltyRatio { get; set; }

		// 10 ?
		public double AdjacencyDownRatio { get; set; }

		// 1 ?
		public double AdjacencySideRatio { get; set; }

		// 10?
		public double CornerCellsBonus { get; set; }

		// 50 ?
		public double HiddenHolesPenalty { get; set; }
	}
}