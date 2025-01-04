using System;
using System.Text;

namespace PatchworkSim.AI.MoveMakers;

/// <summary>
/// Delegates move making to child move makers based on our position on the board
/// </summary>
public class RangeSplitByBoardPositionDelegationMoveMaker : IMoveDecisionMaker
{
	public string Name {
		get
		{
			var b = new StringBuilder();
			for (var i = 0; i < _children.Length; i++)
			{
				b.Append(_children[i].Name);
				b.Append(";");
			}

			return b.ToString();
		}

	}

	private readonly IMoveDecisionMaker[] _children;

	public RangeSplitByBoardPositionDelegationMoveMaker(IMoveDecisionMaker[] children)
	{
		if (children.Length != SimulationState.EndLocation)
			throw new Exception($"Expected {SimulationState.EndLocation} children, but was given {children.Length}");

		_children = children;
	}

	public void MakeMove(SimulationState state)
	{
		_children[state.PlayerPosition[state.ActivePlayer]].MakeMove(state);
	}

}
