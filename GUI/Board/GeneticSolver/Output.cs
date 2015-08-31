using Newtonsoft.Json;

namespace GeneticSolver
{
	internal sealed class Output
	{
		[JsonProperty(PropertyName = "problemId")]
		public int ProblemId { get; set; }

		[JsonProperty(PropertyName = "seed")]
		public uint Seed { get; set; }

		[JsonProperty(PropertyName = "solution")]
		public string Commands { get; set; }
	}
}