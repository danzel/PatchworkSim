using System;
using System.Text;

namespace PatchworkSim.Loggers
{
    public class ConsoleLogger : ISimulationLogger
    {
	    public bool PrintBoardsAfterPlacement;

		private readonly SimulationState _sim;
	    private bool[][,] _previousBoards = new bool[2][,];

	    public ConsoleLogger(SimulationState sim)
	    {
		    _sim = sim;

		    _previousBoards[0] = (bool[,])sim.PlayerBoardState[0].Clone();
		    _previousBoards[1] = (bool[,])sim.PlayerBoardState[1].Clone();
		}


	    public void PlayerAdvanced(int player)
	    {
		    Console.WriteLine($"Player {player} advanced to {_sim.PlayerPosition[player]}");
	    }

	    public void PlayerPurchasedPiece(int player, PieceDefinition piece)
	    {
		    Console.WriteLine($"Player {player} purchased {piece.Name}");
	    }

	    public void PlayerPlacedPiece(int player, PieceDefinition piece, int x, int y, bool[,] bitmap)
	    {
		    Console.WriteLine($"Player {player} placed {piece.Name} at {x},{y}");

		    if (PrintBoardsAfterPlacement)
			    PrintBoards(true);
	    }

	    public void PrintBoards(bool showDifferenceToLastPrint)
	    {
			for (var y = 0; y < SimulationState.PlayerBoardSize; y++)
			{
				for (var player = 0; player <= 1; player++)
				{
					for (var x = 0; x < SimulationState.PlayerBoardSize; x++)
					{
						if (showDifferenceToLastPrint && _sim.PlayerBoardState[player][x, y] != _previousBoards[player][x, y])
							Console.BackgroundColor = ConsoleColor.DarkGreen;
						Console.Write(_sim.PlayerBoardState[player][x, y] ? '#' : ' ');
						Console.BackgroundColor = ConsoleColor.Black;
					}
					Console.Write('|');
				}
				Console.WriteLine();
			}

			if (showDifferenceToLastPrint)
			{
				_previousBoards[0] = (bool[,])_sim.PlayerBoardState[0].Clone();
				_previousBoards[1] = (bool[,])_sim.PlayerBoardState[1].Clone();
			}

		}
    }
}
