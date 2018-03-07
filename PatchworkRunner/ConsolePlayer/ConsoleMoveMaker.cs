using System;
using System.Text;
using PatchworkSim;
using PatchworkSim.AI;

namespace PatchworkRunner.ConsolePlayer
{
	class ConsoleMoveMaker : IMoveDecisionMaker
	{
		private readonly StringBuilder[] _lines =
		{
			new StringBuilder(),
			new StringBuilder(),
			new StringBuilder(),
			new StringBuilder(),
			new StringBuilder(),
			new StringBuilder(),
			new StringBuilder(),
			new StringBuilder(),
		};

		public string Name => "Console";

		public void MakeMove(SimulationState state)
		{
			//Print out the next X pieces
			foreach (var l in _lines)
				l.Clear();
			for (var i = 0; i < 7; i++)
			{
				var piece = PieceDefinition.AllPieceDefinitions[state.Pieces[(state.NextPieceIndex + i) % state.Pieces.Count]];

				BufferPiece(piece);
			}

			Console.WriteLine("Next Pieces");
			foreach (var l in _lines)
				Console.WriteLine(l.ToString());

			Console.WriteLine("Moves:");
			Console.WriteLine("z: Advance");
			for (var i = 0; i < 3; i++)
			{
				var piece = PieceDefinition.AllPieceDefinitions[state.Pieces[(state.NextPieceIndex + i) % state.Pieces.Count]];
				Console.WriteLine($"{i}: Purchase {piece.Name} [T:{piece.TimeCost} / $:{piece.ButtonsIncome}] for {piece.ButtonCost} Buttons");
			}

			//Print out the players income and button count
			Console.WriteLine($"Position: {state.PlayerPosition[state.ActivePlayer]} / {state.PlayerPosition[state.NonActivePlayer]}");
			Console.WriteLine($"Buttons: {state.PlayerButtonAmount[state.ActivePlayer]} / {state.PlayerButtonAmount[state.NonActivePlayer]}");
			Console.WriteLine($"Income: {state.PlayerButtonIncome[state.ActivePlayer]} / {state.PlayerButtonIncome[state.NonActivePlayer]}");

			//Player enters an action
			while (true)
			{
				var line = Console.ReadLine();

				if (line == "z")
				{
					state.PerformAdvanceMove();
					break;
				}

				if (int.TryParse(line, out var index) && index >= 0 && index < 3)
				{
					var piece = PieceDefinition.AllPieceDefinitions[state.Pieces[(state.NextPieceIndex + index) % state.Pieces.Count]];

					if (Helpers.ActivePlayerCanPurchasePiece(state, piece))
					{
						state.PerformPurchasePiece(state.NextPieceIndex + index);
						break;
					}
				}
			}
		}

		private void BufferPiece(PieceDefinition piece)
		{
			for (var y = 0; y < 5; y++)
			{
				for (var x = 0; x < 5; x++)
				{
					bool here = false;
					if (piece.Bitmap.Width > x && piece.Bitmap.Height > y)
						here = piece.Boolmap[x, y];

					_lines[y].Append(here ? '#' : ' ');
				}

				_lines[y].Append(" | ");
			}

			_lines[5].Append($"$:{piece.ButtonCost.ToString().PadRight(3)} | ");
			_lines[6].Append($"T:{piece.TimeCost.ToString().PadRight(3)} | ");
			_lines[7].Append($"+:{piece.ButtonsIncome.ToString().PadRight(3)} | ");
		}
	}
}