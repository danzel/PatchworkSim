using System.Collections.Generic;

namespace PatchworkSim.AI.PlacementFinders.PlacementStrategies
{
	static class PlacementHelper
	{
		/// <summary>
		/// Returns a list of holes in the board. The value is the size of each hole
		/// </summary>
		public static void HoleCount(BoardState board, ref List<int> holeSizes)
		{
			for (var x = 0; x < BoardState.Width; x++)
			{
				for (var y = 0; y < BoardState.Height; y++)
				{
					if (!board[x, y])
					{
						holeSizes.Add(FloodFiller.MyFill(ref board, x, y));
					}
				}
			}
		}
	}

	/// <summary>
	/// FloodFill algorithm taken from http://www.adammil.net/blog/v126_A_More_Efficient_Flood_Fill.html
	/// Licence unknown
	/// </summary>
	static class FloodFiller
	{
		/// <summary>
		/// Returns the amount of positions filled
		/// </summary>
		public static int MyFill(ref BoardState array, int x, int y)
		{
			int amountFilled = 0;
			if (!array[y, x])
				_MyFill(ref array, ref amountFilled, x, y);
			return amountFilled;
		}

		static void _MyFill(ref BoardState array, ref int amountFilled, int x, int y)
		{
			// at this point, we know array[y,x] is clear, and we want to move as far as possible to the upper-left. moving
			// up is much more important than moving left, so we could try to make this smarter by sometimes moving to
			// the right if doing so would allow us to move further up, but it doesn't seem worth the complexity
			while (true)
			{
				int ox = x, oy = y;
				while (y != 0 && !array[y - 1, x]) y--;
				while (x != 0 && !array[y, x - 1]) x--;
				if (x == ox && y == oy) break;
			}
			MyFillCore(ref array, ref amountFilled, x, y);
		}

		static void MyFillCore(ref BoardState array, ref int amountFilled, int x, int y)
		{
			// at this point, we know that array[y,x] is clear, and array[y-1,x] and array[y,x-1] are set.
			// we'll begin scanning down and to the right, attempting to fill an entire rectangular block
			int lastRowLength = 0; // the number of cells that were clear in the last row we scanned
			do
			{
				int rowLength = 0, sx = x; // keep track of how long this row is. sx is the starting x for the main scan below
										   // now we want to handle a case like |***|, where we fill 3 cells in the first row and then after we move to
										   // the second row we find the first  | **| cell is filled, ending our rectangular scan. rather than handling
										   // this via the recursion below, we'll increase the starting value of 'x' and reduce the last row length to
										   // match. then we'll continue trying to set the narrower rectangular block
				if (lastRowLength != 0 && array[y, x]) // if this is not the first row and the leftmost cell is filled...
				{
					do
					{
						if (--lastRowLength == 0) return; // shorten the row. if it's full, we're done
					} while (array[y, ++x]); // otherwise, update the starting point of the main scan to match
					sx = x;
				}
				// we also want to handle the opposite case, | **|, where we begin scanning a 2-wide rectangular block and
				// then find on the next row that it has     |***| gotten wider on the left. again, we could handle this
				// with recursion but we'd prefer to adjust x and lastRowLength instead
				else
				{
					for (; x != 0 && !array[y, x - 1]; rowLength++, lastRowLength++)
					{
						amountFilled++;
						array[y, --x] = true; // to avoid scanning the cells twice, we'll fill them and update rowLength here
											  // if there's something above the new starting point, handle that recursively. this deals with cases
											  // like |* **| when we begin filling from (2,0), move down to (2,1), and then move left to (0,1).
											  // the  |****| main scan assumes the portion of the previous row from x to x+lastRowLength has already
											  // been filled. adjusting x and lastRowLength breaks that assumption in this case, so we must fix it
						if (y != 0 && !array[y - 1, x]) _MyFill(ref array, ref amountFilled, x, y - 1); // use _Fill since there may be more up and left
					}
				}

				// now at this point we can begin to scan the current row in the rectangular block. the span of the previous
				// row from x (inclusive) to x+lastRowLength (exclusive) has already been filled, so we don't need to
				// check it. so scan across to the right in the current row
				for (; sx < BoardState.Width && !array[y, sx]; rowLength++, sx++)
				{
					amountFilled++;
					array[y, sx] = true;
				}
				// now we've scanned this row. if the block is rectangular, then the previous row has already been scanned,
				// so we don't need to look upwards and we're going to scan the next row in the next iteration so we don't
				// need to look downwards. however, if the block is not rectangular, we may need to look upwards or rightwards
				// for some portion of the row. if this row was shorter than the last row, we may need to look rightwards near
				// the end, as in the case of |*****|, where the first row is 5 cells long and the second row is 3 cells long.
				// we must look to the right  |*** *| of the single cell at the end of the second row, i.e. at (4,1)
				if (rowLength < lastRowLength)
				{
					for (int end = x + lastRowLength; ++sx < end;) // 'end' is the end of the previous row, so scan the current row to
					{                                          // there. any clear cells would have been connected to the previous
						if (!array[y, sx]) MyFillCore(ref array, ref amountFilled, sx, y); // row. the cells up and left must be set so use FillCore
					}
				}
				// alternately, if this row is longer than the previous row, as in the case |*** *| then we must look above
				// the end of the row, i.e at (4,0)                                         |*****|
				else if (rowLength > lastRowLength && y != 0) // if this row is longer and we're not already at the top...
				{
					for (int ux = x + lastRowLength; ++ux < sx;) // sx is the end of the current row
					{
						if (!array[y - 1, ux]) _MyFill(ref array, ref amountFilled, ux, y - 1); // since there may be clear cells up and left, use _Fill
					}
				}
				lastRowLength = rowLength; // record the new row length
			} while (lastRowLength != 0 && ++y < BoardState.Height); // if we get to a full row or to the bottom, we're done
		}
	}
}
