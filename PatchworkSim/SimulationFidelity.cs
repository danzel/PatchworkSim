namespace PatchworkSim
{
	public enum SimulationFidelity
	{
		/// <summary>
		/// Full Quality. Players must place all pieces they purchase
		/// </summary>
		FullSimulation,

		/// <summary>
		/// Low Quality. No piece placing is required, we don't know if the player could have placed a given piece.
		/// Much faster to simulate as deciding where a piece goes is hard
		/// </summary>
		NoPiecePlacing
	}
}
