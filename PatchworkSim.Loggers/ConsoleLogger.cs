using System;

namespace PatchworkSim.Loggers
{
    public class ConsoleLogger : ISimulationLogger
    {
	    public bool PrintBoardsAfterPlacement;

		private readonly SimulationState _sim;
	    private BoardState[] _previousBoards = new BoardState[2];

	    public ConsoleLogger(SimulationState sim)
	    {
		    _sim = sim;

		    _previousBoards[0] = sim.PlayerBoardState[0];
		    _previousBoards[1] = sim.PlayerBoardState[1];
		}


	    public void PlayerAdvanced(int player)
	    {
		    Console.WriteLine($"Player {player} advanced to {_sim.PlayerPosition[player]}");
	    }

	    public void PlayerPurchasedPiece(int player, PieceDefinition piece)
	    {
		    Console.WriteLine($"Player {player} purchased {piece.Name}");
	    }

	    public void PlayerPlacedPiece(int player, PieceDefinition piece, int x, int y, PieceBitmap bitmap)
	    {
		    Console.WriteLine($"Player {player} placed {piece.Name} at {x},{y}");

		    if (PrintBoardsAfterPlacement)
			    PrintBoards(true);
	    }

	    public void PrintBoards(bool showDifferenceToLastPrint)
	    {
			for (var y = 0; y < BoardState.Height; y++)
			{
				for (var player = 0; player <= 1; player++)
				{
					for (var x = 0; x < BoardState.Width; x++)
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
				_previousBoards[0] = _sim.PlayerBoardState[0];
				_previousBoards[1] = _sim.PlayerBoardState[1];
			}
		}

		public static void PrintBoard(BoardState board)
		{
			for (var y = 0; y < BoardState.Height; y++)
			{
				for (var x = 0; x < BoardState.Width; x++)
				{
					Console.Write(board[x, y] ? '#' : ' ');
				}
				Console.WriteLine("|");
			}
			Console.WriteLine("---------+");
		}

	    public static void PrintBoardsDiff(BoardState[] oldBoards, BoardState[] boards)
	    {
		    for (var y = 0; y < BoardState.Height; y++)
		    {
			    for (var player = 0; player < boards.Length; player++)
			    {
				    for (var x = 0; x < BoardState.Width; x++)
				    {
					    if (oldBoards[player][x, y] != boards[player][x, y])
						    Console.BackgroundColor = ConsoleColor.DarkGreen;
					    Console.Write(boards[player][x, y] ? '#' : ' ');
					    Console.BackgroundColor = ConsoleColor.Black;
				    }
				    Console.Write('|');
			    }
			    Console.WriteLine();
		    }
	    }
	}
}
