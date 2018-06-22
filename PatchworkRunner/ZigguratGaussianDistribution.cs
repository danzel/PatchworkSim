/* ***************************************************************************
 * This file is part of the Redzen code library.
 * 
 * Copyright 2015-2017 Colin Green (colin.green1@gmail.com)
 *
 * Redzen is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with Redzen; if not, see https://opensource.org/licenses/MIT.
 */

using System;
using System.Diagnostics;

namespace Redzen.Random.Double
{
	// ENHANCEMENT: Further performance improvement can be obtained by using a less precise method
	// whereby we represent the distribution curve as a piecewise linear curve, i.e. approximate
	// the curve using stacked trapezoids instead of stacked rectangles, and skip handling of the
	// corner cases where we would normally perform expensive calculations to obtain precise 
	// results. Such an approach should be suitable for generating mutations for evolutionary
	// computing.
	// For details about this approach see:
	// Hardware-Optimized Ziggurat Algorithm for High-Speed Gaussian Random Number Generators,
	// Hassan M. Edrees, Brian Cheung, McCullen Sandora, David Nummey, Deian Stefan
	// (http://www.ee.cooper.edu/~stefan/pubs/conference/ersa2009.pdf)
	//
	/// <summary>
	/// A fast Gaussian distribution sampler for .Net
	/// Colin Green, 11/09/2011
	///
	/// An implementation of the Ziggurat algorithm for random sampling from a Gaussian 
	/// distribution. See:
	///  - Wikipedia:Ziggurat algorithm (http://en.wikipedia.org/wiki/Ziggurat_algorithm).
	///  - The Ziggurat Method for Generating Random Variables, George Marsaglia and
	///    Wai Wan Tsang (http://www.jstatsoft.org/v05/i08/paper).
	///  - An Improved Ziggurat Method to Generate Normal Random Samples, Jurgen A Doornik 
	///    (http://www.doornik.com/research/ziggurat.pdf)
	///  
	/// 
	/// Ziggurat Algorithm Overview
	/// ============================
	/// 
	/// Consider the right hand side of the Gaussian probability density function (for x >=0) as
	/// described by y = f(x). This half of the distribution is covered by a series of stacked
	/// horizontal rectangles, like so:
	/// 
	///  _____
	/// |     |                    R6  S6
	/// |     |
	/// |_____|_                   
	/// |       |                  R5  S5
	/// |_______|_                 
	/// |         |                R4  S4
	/// |_________|__       
	/// |____________|__           R3  S3
	/// |_______________|________  R2  S2
	/// |________________________| R1  S1
	/// |________________________| R0  S0
	///           (X)
	/// 
	/// 
	/// The Basics
	/// ----------
	/// (1) 
	/// Each rectangle is assigned a number (the R numbers shown above). 
	/// 
	/// (2) 
	/// The right hand edge of each rectangle is placed so that it just covers the distribution,
	/// that is, the bottom right corner is on the curve, and therefore some of the area in the 
	/// top right of the rectangle is outside of the distribution (points with y greater than 
	/// f(x)), except for R0 (see next point). Therefore the rectangles taken together cover an
	/// area slightly larger than the distribution curve.
	/// 
	/// (3) 
	/// R0 is a special case. The tail of the Gaussian effectively projects into x=Infinity
	/// asymptotically approaching zero, thus we do not cover the tail with a rectangle. Instead
	/// we define a cut-off point (x=3.442619855899 in this implementation). R0's right hand
	/// edge is at the cut-off point with its top right corner on the distribution curve. The
	/// tail is then defined as that part of the distribution with x > tailCutOff and is
	/// combined with R0 to form segment S0. Note that the whole of R0 is within the
	/// distribution, unlike the other rectangles.
	/// 
	/// (4)
	/// Segments. Each rectangle is also referred to as a segment with the exception of R0 which
	/// is a special case as explained above. Essentially S[i] == R[i], except for 
	/// S[0] == R[0] + tail.
	/// 
	/// (5)
	/// Each segment has identical area A, this also applies to the special segment S0, thus the
	/// area of R0 is A minus the area represented by the tail. For all other segments the
	/// segment area is the same as the rectangle area.
	/// 
	/// (6)
	/// R[i] has right hand edge x[i]. And from drawing the rectangles over the distribution
	/// curve it is clear that the region of R[i] to the left of x[i+1] is entirely within the
	/// distribution curve, whereas the region greater than x[i+1] is partially above the
	/// distribution curve. 
	/// 
	/// (7)
	/// R[i] has top edge of y[i].
	/// 
	/// 
	/// Operation
	/// ---------
	/// (1)
	/// Randomly select a segment to sample from, call this S[i], this amounts to a low
	/// resolution random y coordinate. Because the segments have equal area we can select from
	/// them with equal probability. (Also see special notes, below).
	/// 
	/// (2)
	/// Segment 0 is a special case, if S0 is selected then generate a random area value w
	/// between 0 and A. If w is less than or equal to the area of R0 then we are sampling a
	/// point from within R0 (step 2A), otherwise we are sampling from the tail (step 2B).
	/// 
	/// (2A)
	/// Sampling from R0. R0 is entirely within the distribution curve and we have already
	/// generated a random area value w. Convert w to an x value that we can return by dividing
	/// w by the height of R0 (y[0]).
	/// 
	/// (2B)
	/// Sampling from the tail. To sample from the tail we fall back to a slow implementation
	/// using logarithms, see: 
	/// Generating a Variable from the Tail of the Normal Distribution, George Marsaglia (1963).
	/// (http://www.tandfonline.com/doi/abs/10.1080/00401706.1964.10490150?journalCode=utch20)
	/// The area represented by the tail is relatively small and therefore this execution
	/// pathway is avoided for a significant proportion of samples generated.
	///
	/// (3)
	/// Sampling from all other rectangles/segments other then R0/S0. 
	/// Randomly select x from within R[i]. If x is less than x[i+1] then x is within the curve,
	/// return x.
	///    
	/// If x is greater than or equal to x[i+1] then generate a random y variable from within
	/// R[i] (this amounts to producing a high resolution y coordinate, a refinement of the low
	/// resolution y coord we effectively produced by selecting a rectangle/segment).
	///    
	/// If y is below f(x) then return x, otherwise we disregard the sample point and return to
	/// step 1. We specifically do *not* re-attempt to sample from R[i] until a valid point is
	/// found (see special notes 1).
	/// 
	/// (4)
	/// Finally, all of the above describes sampling from the positive half of the distribution
	/// (x greater than or equal to zero) hence to obtain a symmetrical distribution we need one
	/// more random bit to decide whether to flip the sign of the returned x.
	/// 
	/// 
	/// Special notes
	/// -------------
	/// (Note 1) 
	/// Segments have equal area and are thus selected with equal probability. However, the area
	/// under the distribution curve covered by each segment/rectangle differs where rectangles
	/// overlap the edge of the distribution curve. Thus it has been suggested that to avoid
	/// sampling bias that the segments should be selected with a probability that reflects the
	/// area of the distribution curve they cover not their total area, this is an incorrect
	/// approach for the algorithm as described above and implemented in this class. To explain
	/// why consider an extreme case. 
	/// 
	/// Say that rectangle R1 covers an area entirely within the distribution curve, now consider
	/// R2 that covers an area only 10% within the curve. Both rectangles are chosen with equal
	/// probability, thus the argument is that R2 will be 10x overrepresented (will generate
	/// sample points as often as R1 despite covering a much smaller proportion of the area under
	/// the distribution curve). In reality sample points within R2 will be rejected 90% of the
	/// time and we disregard the attempt to sample from R2 and go back to step 1 (select a
	/// segment to sample from).
	/// 
	/// If instead we re-attempted sampling from R2 until a valid point was found then R2 would 
	/// indeed become over-represented, hence we do not do this and the algorithm therefore does
	/// not exhibit any such bias.
	/// 
	/// (Note 2)
	/// George Marsaglia's original implementation used a single random number (32bit unsigned
	/// integer) for both selecting the segment and producing the x coordinate with the chosen
	/// segment. The segment index was taken from the least significant bits (so the least
	/// significant 7 bits if using 128 segments). This effectively created a peculiar type of
	/// bias in which all x coords produced within a given segment would have an identical least
	/// significant 7 bits, albeit prior to casting to a floating point value. The bias is perhaps
	/// small especially in comparison to the performance gain (one less call to the RNG). This 
	/// implementation avoids this bias by not re-using random bits in such a way. For more info 
	/// see:
	/// An Improved Ziggurat Method to Generate Normal Random Samples, Jurgen A Doornik 
	/// (http://www.doornik.com/research/ziggurat.pdf)
	/// 
	/// 
	/// Optimizations
	/// -------------
	/// (Optimization 1) 
	/// On selecting a segment/rectangle we generate a random x value within the range of the
	/// rectangle (or the range of the area of S0), this requires multiplying a random number with
	/// range [0,1] to the required x range before performing the first test for x being within the
	/// 'certain' left-hand side of the rectangle. We avoid this multiplication and indeed
	/// conversion of a random integer into a float with range [0,1], thus allowing the first 
	/// comparison to be performed using integer arithmetic.
	/// 
	/// Instead of using the x coord of RN+1 to test whether a randomly generated point within RN
	/// is within the 'certain' left hand side part of the distribution, we precalculate the
	/// probability of a random x coord being within the safe part for each rectangle. Furthermore
	/// we store this probability as a UInt with range [0, 0xffffffff] thus allowing direct
	/// comparison with randomly generated UInts from the RNG, this allows the comparison to be
	/// performed using integer arithmetic. If the test succeeds then we continue to convert the
	/// random value into an appropriate x sample value.
	///  
	/// (Optimization 2)
	/// Simple collapsing of calculations into precomputed values where possible. This affects 
	/// readability, but hopefully the above explanations will help understand the code if necessary.
	/// 
	/// (Optimization 3)
	/// The gaussian probability density function (PDF) contains terms for distribution mean and 
	/// standard deviation. We remove all excess terms and denormalise the function to obtain a 
	/// simpler equation with the same shape. This simplified equation is no longer a PDF as the
	/// total area under the curve is no loner 1.0 (a key property of PDFs), however as it has the
	/// same overall shape it remains suitable for sampling from a Gaussian using rejection methods
	/// such as the Ziggurat algorithm (it's the shape of the curve that matters, not the absolute
	/// area under the curve).
	/// </summary>
	public class ZigguratGaussianDistribution
	{
		#region Static Fields [Defaults]

		/// <summary>
		/// Number of blocks.
		/// </summary>
		const int __blockCount = 128;
		/// <summary>
		/// Right hand x coord of the base rectangle, thus also the left hand x coord of the tail 
		/// (pre-determined/computed for 128 blocks).
		/// </summary>
		const double __R = 3.442619855899;
		/// <summary>
		/// Area of each rectangle (pre-determined/computed for 128 blocks).
		/// </summary>
		const double __A = 9.91256303526217e-3;
		/// <summary>
		/// Scale factor for converting a UInt with range [0,0xffffffff] to a double with range [0,1].
		/// </summary>
		const double __UIntToU = 1.0 / (double)uint.MaxValue;

		#endregion

		#region Instance Fields

		readonly XorShiftRandom _rng;
		readonly double _mean;
		readonly double _stdDev;
		readonly Func<double> _sampleFn;

		// _x[i] and _y[i] describe the top-right position ox rectangle i.
		readonly double[] _x;
		readonly double[] _y;

		// The proportion of each segment that is entirely within the distribution, expressed as uint where 
		// a value of 0 indicates 0% and uint.MaxValue 100%. Expressing this as an integer allows some floating
		// points operations to be replaced with integer ones.
		readonly uint[] _xComp;

		// Useful precomputed values.
		// Area A divided by the height of B0. Note. This is *not* the same as _x[i] because the area 
		// of B0 is __A minus the area of the distribution tail.
		readonly double _A_Div_Y0;

		#endregion

		#region Constructors

		/// <summary>
		/// Construct with the specified RNG seed..
		/// </summary>
		public ZigguratGaussianDistribution(int seed)
			: this(new XorShiftRandom(seed), 0.0, 1.0)
		{ }

		/// <summary>
		/// Construct with the specified RNG seed..
		/// </summary>
		public ZigguratGaussianDistribution(int seed, double mean, double stdDev)
			: this(new XorShiftRandom(seed), mean, stdDev)
		{
		}

		/// <summary>
		/// Construct with the provided RNG source.
		/// </summary>
		/// <param name="rng">Random source.</param>
		/// <param name="mean">Distribution mean.</param>
		/// <param name="stdDev">Distribution standard deviation.</param>
		public ZigguratGaussianDistribution(XorShiftRandom rng, double mean, double stdDev)
		{
			_rng = rng;
			_mean = mean;
			_stdDev = stdDev;

			// Note. We predetermine which of these four function variants to use at construction time,
			// thus avoiding the two condition tests on each invocation of Sample(). 
			// I.e. this is a micro-optimization.
			if (0.0 == mean)
			{
				if (1.0 == stdDev)
				{
					_sampleFn = () => { return SampleStandard(); };
				}
				else
				{
					_sampleFn = () => { return SampleStandard() * stdDev; };
				}
			}
			else
			{
				if (1.0 == stdDev)
				{
					_sampleFn = () => { return _mean + SampleStandard(); };
				}
				else
				{
					_sampleFn = () => { return _mean + (SampleStandard() * stdDev); };
				}
			}


			// Initialise rectangle position data. 
			// _x[i] and _y[i] describe the top-right position ox Box i.

			// Allocate storage. We add one to the length of _x so that we have an entry at _x[_blockCount], this avoids having 
			// to do a special case test when sampling from the top box.
			_x = new double[__blockCount + 1];
			_y = new double[__blockCount];

			// Determine top right position of the base rectangle/box (the rectangle with the Gaussian tale attached). 
			// We call this Box 0 or B0 for short.
			// Note. x[0] also describes the right-hand edge of B1. (See diagram).
			_x[0] = __R;
			_y[0] = GaussianPdfDenorm(__R);

			// The next box (B1) has a right hand X edge the same as B0. 
			// Note. B1's height is the box area divided by its width, hence B1 has a smaller height than B0 because
			// B0's total area includes the attached distribution tail.
			_x[1] = __R;
			_y[1] = _y[0] + (__A / _x[1]);

			// Calc positions of all remaining rectangles.
			for (int i = 2; i < __blockCount; i++)
			{
				_x[i] = GaussianPdfDenormInv(_y[i - 1]);
				_y[i] = _y[i - 1] + (__A / _x[i]);
			}

			// For completeness we define the right-hand edge of a notional box 6 as being zero (a box with no area).
			_x[__blockCount] = 0.0;

			// Useful precomputed values.
			_A_Div_Y0 = __A / _y[0];
			_xComp = new uint[__blockCount];

			// Special case for base box. _xComp[0] stores the area of B0 as a proportion of __R 
			// (recalling that all segments have area __A, but that the base segment is the combination of B0 and the distribution tail).
			// Thus -xComp[0[ is the probability that a sample point is within the box part of the segment.
			_xComp[0] = (uint)(((__R * _y[0]) / __A) * (double)uint.MaxValue);

			for (int i = 1; i < __blockCount - 1; i++)
			{
				_xComp[i] = (uint)((_x[i + 1] / _x[i]) * (double)uint.MaxValue);
			}
			_xComp[__blockCount - 1] = 0;  // Shown for completeness.

			// Sanity check. Test that the top edge of the topmost rectangle is at y=1.0.
			// Note. We expect there to be a tiny drift away from 1.0 due to the inexactness of floating
			// point arithmetic.
			Debug.Assert(Math.Abs(1.0 - _y[__blockCount - 1]) < 1e-10);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Take a sample from the distribution.
		/// </summary>
		public double Sample()
		{
			return _sampleFn();
		}

		/// <summary>
		/// Take a sample from the distribution.
		/// </summary>
		/// <param name="mean">Distribution mean.</param>
		/// <param name="stdDev">Distribution standard deviation.</param>
		/// <returns>A new random sample.</returns>
		public double Sample(double mean, double stdDev)
		{
			return mean + (SampleStandard() * stdDev);
		}

		/// <summary>
		/// Take a sample from the standard gaussian distribution, i.e. with mean of 0 and standard deviation of 1.
		/// </summary>
		public double SampleStandard()
		{
			for (; ; )
			{
				// Select box at random.
				byte u = _rng.NextByte();
				int i = (int)(u & 0x7F);
				double sign = ((u & 0x80) == 0) ? -1.0 : 1.0;

				// Generate uniform random value with range [0,0xffffffff].
				uint u2 = _rng.NextUInt();

				// Special case for the base segment.
				if (0 == i)
				{
					if (u2 < _xComp[0])
					{   // Generated x is within R0.
						return u2 * __UIntToU * _A_Div_Y0 * sign;
					}
					// Generated x is in the tail of the distribution.
					return SampleTail() * sign;
				}

				// All other segments.
				if (u2 < _xComp[i])
				{   // Generated x is within the rectangle.
					return u2 * __UIntToU * _x[i] * sign;
				}

				// Generated x is outside of the rectangle.
				// Generate a random y coordinate and test if our (x,y) is within the distribution curve.
				// This execution path is relatively slow/expensive (makes a call to Math.Exp()) but relatively rarely executed,
				// although more often than the 'tail' path (above).
				double x = u2 * __UIntToU * _x[i];
				if (_y[i - 1] + ((_y[i] - _y[i - 1]) * _rng.NextDouble()) < GaussianPdfDenorm(x))
				{
					return x * sign;
				}
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Sample from the distribution tail (defined as having x >= __R).
		/// </summary>
		/// <returns></returns>
		private double SampleTail()
		{
			double x, y;
			do
			{
				// Note. we use NextDoubleNonZero() because Log(0) returns NaN and will also tend to be a very slow execution path (when it occurs, which is rarely).
				x = -Math.Log(_rng.NextDoubleNonZero()) / __R;
				y = -Math.Log(_rng.NextDoubleNonZero());
			}
			while (y + y < x * x);
			return __R + x;
		}

		/// <summary>
		/// Gaussian probability density function, denormalised, that is, y = e^-(x^2/2).
		/// </summary>
		private double GaussianPdfDenorm(double x)
		{
			return Math.Exp(-(x * x / 2.0));
		}

		/// <summary>
		/// Inverse function of GaussianPdfDenorm(x)
		/// </summary>
		private double GaussianPdfDenormInv(double y)
		{
			// Operates over the y range (0,1], which happens to be the y range of the pdf, 
			// with the exception that it does not include y=0, but we would never call with 
			// y=0 so it doesn't matter. Remember that a Gaussian effectively has a tail going
			// off into x == infinity, hence asking what is x when y=0 is an invalid question
			// in the context of this class.
			return Math.Sqrt(-2.0 * Math.Log(y));
		}

		#endregion
	}
}
/* ***************************************************************************
 * This file is part of the Redzen code library.
 * 
 * Copyright 2015-2017 Colin Green (colin.green1@gmail.com)
 *
 * Redzen is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with Redzen; if not, see https://opensource.org/licenses/MIT.
 */


namespace Redzen.Random
{
	/// <summary>
	/// A fast random number generator for .NET
	/// Colin Green, January 2005
	/// 
	/// Note. A forked version of this class exists in Math.Net at time of writing (XorShift class).
	/// 
	/// Key points:
	///  1) Based on a simple and fast xor-shift pseudo random number generator (RNG) specified in: 
	///  Marsaglia, George. (2003). Xorshift RNGs.
	///  http://www.jstatsoft.org/v08/i14/paper
	///  
	///  This particular implementation of xorshift has a period of 2^128-1. See the above paper to see
	///  how this can be easily extended if you need a longer period. At the time of writing I could find no 
	///  information on the period of System.Random for comparison.
	/// 
	///  2) Faster than System.Random. Up to 8x faster, depending on which methods are called.
	/// 
	///  3) Direct replacement for System.Random. This class implements all of the methods that System.Random 
	///  does plus some additional methods. The like named methods are functionally equivalent.
	///  
	///  4) Allows fast re-initialisation with a seed, unlike System.Random which accepts a seed at construction
	///  time which then executes a relatively expensive initialisation routine. This provides a vast speed improvement
	///  if you need to reset the pseudo-random number sequence many times, e.g. if you want to re-generate the same
	///  sequence of random numbers many times. An alternative might be to cache random numbers in an array, but that 
	///  approach is limited by memory capacity and the fact that you may also want a large number of different sequences 
	///  cached. Each sequence can be represented by a single seed value (int) when using FastRandom.
	/// </summary>
	public sealed class XorShiftRandom
	{
		#region Instance Fields

		// The +1 ensures NextDouble doesn't generate 1.0. +129 (0x81) is the equivalent value for NextFloat.
		const double REAL_UNIT_INT = 1.0 / (int.MaxValue + 1.0);
		const double REAL_UNIT_UINT = 1.0 / (uint.MaxValue + 1.0);
		const float REAL_UNIT_UINT_F = 1f / (uint.MaxValue + 129f);
		const uint Y = 842502087;
		const uint Z = 3579807591;
		const uint W = 273326509;

		uint _x, _y, _z, _w;

		#endregion

		#region Constructors

		/// <summary>
		/// Initialises a new instance using an int value as seed.
		/// This constructor signature is provided to maintain compatibility with
		/// System.Random
		/// </summary>
		public XorShiftRandom(int seed)
		{
			Reinitialise(seed);
		}

		#endregion

		#region Public Methods [Re-initialisation]

		/// <summary>
		/// Re-initialises using an int value as a seed.
		/// </summary>
		public void Reinitialise(int seed)
		{
			// The only stipulation stated for the xorshift RNG is that at least one of
			// the seeds x,y,z,w is non-zero. We fulfil that requirement by only allowing
			// resetting of the x seed.

			// The first random sample will be very closely related to the value of _x we set here. 
			// Thus setting _x = seed will result in a close correlation between the bit patterns of the seed and
			// the first random sample, therefore if the seed has a pattern (e.g. 1,2,3) then there will also be 
			// a recognisable pattern across the first random samples.
			//
			// Such a strong correlation between the seed and the first random sample is an undesirable
			// characteristic of a RNG, therefore we significantly weaken any correlation by hashing the seed's bits. 
			// This is achieved by multiplying the seed with four large primes each with bits distributed over the
			// full length of a 32bit value, finally adding the results to give _x.
			_x = (uint)(seed * 3575866506U);

			_y = Y;
			_z = Z;
			_w = W;

			_bitBuffer = 0;
			_bitMask = 1;
		}

		#endregion

		#region Public Methods [System.Random functionally equivalent methods]

		/// <summary>
		/// Generates a random int over the range 0 to int.MaxValue-1.
		/// MaxValue is not generated in order to remain functionally equivalent to System.Random.Next().
		/// This does slightly eat into some of the performance gain over System.Random, but not much.
		/// For better performance see:
		/// 
		/// Call NextInt() for an int over the range 0 to int.MaxValue.
		/// 
		/// Call NextUInt() and cast the result to an int to generate an int over the full Int32 value range
		/// including negative values. 
		/// </summary>
		public int Next()
		{
			uint t = _x ^ (_x << 11);
			_x = _y; _y = _z; _z = _w;
			_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8));

			// Handle the special case where the value int.MaxValue is generated. This is outside of 
			// the range of permitted values, so we therefore call Next() to try again.
			uint rtn = _w & 0x7FFFFFFF;
			if (rtn == 0x7FFFFFFF)
			{
				return Next();
			}
			return (int)rtn;
		}

		/// <summary>
		/// Generates a random int over the range 0 to upperBound-1, and not including upperBound.
		/// </summary>
		public int Next(int upperBound)
		{
			if (upperBound < 0)
			{
				throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >=0");
			}

			uint t = _x ^ (_x << 11);
			_x = _y; _y = _z; _z = _w;

			// ENHANCEMENT: Can we do this without converting to a double and back again?
			// The explicit int cast before the first multiplication gives better performance.
			// See comments in NextDouble.
			return (int)((REAL_UNIT_UINT * (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8)))) * upperBound);
		}

		/// <summary>
		/// Generates a random int over the range lowerBound to upperBound-1, and not including upperBound.
		/// upperBound must be >= lowerBound. lowerBound may be negative.
		/// </summary>
		public int Next(int lowerBound, int upperBound)
		{
			if (lowerBound > upperBound)
			{
				throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >=lowerBound");
			}

			uint t = _x ^ (_x << 11);
			_x = _y; _y = _z; _z = _w;

			// Test if range will fit into an Int32.
			int range = upperBound - lowerBound;
			if (range >= 0)
			{
				return lowerBound + (int)((REAL_UNIT_UINT * (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8)))) * range);
			}

			// When range is less than 0 then an overflow has occurred and therefore we must resort to using long integer arithmetic (which is slower).
			return lowerBound + (int)((REAL_UNIT_UINT * (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8)))) * ((long)upperBound - (long)lowerBound));
		}

		/// <summary>
		/// Generates a random double. Values returned are over the range [0, 1). That is, inclusive of 0.0 and exclusive of 1.0.
		/// </summary>
		public double NextDouble()
		{
			uint t = _x ^ (_x << 11);
			_x = _y; _y = _z; _z = _w;

			// N.B. Here we're using the full 32 bits of randomness, whereas System.Random uses 31 bits.
			return REAL_UNIT_UINT * (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8)));
		}

		/// <summary>
		/// Fills the provided byte array with random bytes.
		/// </summary>
		/// <param name="buffer"></param>
		public unsafe void NextBytes(byte[] buffer)
		{
			// For improved performance the below loop operates on these stack allocated copies of the heap variables.
			// Notes. doing this means that these heavily used variables are located near to other local/stack variables,
			// thus they will very likely be cached in the same CPU cache line.
			uint x = _x, y = _y, z = _z, w = _w;

			uint t;
			int i = 0;

			// Get a pointer to the start of [buffer]; to do this we must pin [buffer] because it is allocated
			// on the heap and therefore could be moved by the GC at any time (if we didn't pin it).
			fixed (byte* pBuffer = buffer)
			{
				// A pointer to 32 bit size segments of [buffer].
				uint* pUInt = (uint*)pBuffer;

				// Create and store new random bytes in groups of four.
				for (int bound = buffer.Length / 4; i < bound; i++)
				{
					// Generate 32 random bits and assign to the segment that pUInt is currently pointing to.
					t = (x ^ (x << 11));
					x = y; y = z; z = w;
					pUInt[i] = w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));
				}
			}

			// Fill any trailing entries in [buffer] that occur when the its length is not a multiple of four.
			// Note. We do this using safe C# therefore can unpin [buffer]; i.e. its preferable to hold pins for the 
			// shortest duration possible because they have an impact on the effectiveness of the garbage collector.

			// Convert back to one based indexing instead of groups of four bytes.
			i = i * 4;

			// Fill up any remaining bytes in the buffer.
			if (i < buffer.Length)
			{
				// Generate a further 32 random bits.
				t = (x ^ (x << 11));
				x = y; y = z; z = w;
				w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));

				// Allocate one byte at a time until we reach the end of the buffer.
				while (i < buffer.Length)
				{
					// Allocate byte.
					buffer[i++] = (byte)w;

					// Shift right 8 bits.
					w >>= 8;
				}
			}

			// Update the state variables on the heap.
			_x = x; _y = y; _z = z; _w = w;
		}

		#endregion

		#region Public Methods [Methods not present on System.Random]

		/// <summary>
		/// Generates a random float. Values returned are over the range [0, 1). That is, inclusive of 0.0 and exclusive of 1.0.
		/// </summary>
		public float NextFloat()
		{
			uint t = _x ^ (_x << 11);
			_x = _y; _y = _z; _z = _w;

			// N.B. Here we're using the full 32 bits of randomness, whereas System.Random uses 31 bits.
			return REAL_UNIT_UINT_F * (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8)));
		}

		/// <summary>
		/// Generates a uint. Values returned are over the full range of a uint, 
		/// uint.MinValue to uint.MaxValue, inclusive.
		/// 
		/// This is the fastest method for generating a single random number because the underlying
		/// random number generator algorithm generates 32 random bits that can be cast directly to 
		/// a uint.
		/// </summary>
		public uint NextUInt()
		{
			uint t = _x ^ (_x << 11);
			_x = _y; _y = _z; _z = _w;
			return _w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8));
		}

		/// <summary>
		/// Generates a random int over the range 0 to int.MaxValue, inclusive. 
		/// This method differs from Next() only in that the range is 0 to int.MaxValue
		/// and not 0 to int.MaxValue-1.
		/// 
		/// The slight difference in range means this method is slightly faster than Next()
		/// but is not functionally equivalent to System.Random.Next().
		/// </summary>
		public int NextInt()
		{
			uint t = _x ^ (_x << 11);
			_x = _y; _y = _z; _z = _w;
			return (int)(0x7FFFFFFF & (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8))));
		}

		/// <summary>
		/// Generates a random double. Values returned are over the range (0, 1). That is, exclusive of both 0.0 and 1.0.
		/// </summary>
		public double NextDoubleNonZero()
		{
			uint t = _x ^ (_x << 11);
			_x = _y; _y = _z; _z = _w;

			// Here we generate a random value from 0 to 0xff ff ff fe, and add one
			// to generate a random value from 1 to 0xff ff ff ff.
			return REAL_UNIT_UINT * ((0xFFFFFFFE & (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8)))) + 1U);
		}

		// Buffer 32 bits in bitBuffer, return 1 at a time, keep track of how many have been returned
		// with bitMask.
		uint _bitBuffer;
		uint _bitMask;

		/// <summary>
		/// Generates a single random bit.
		/// This method's performance is improved by generating 32 bits in one operation and storing them
		/// ready for future calls.
		/// </summary>
		public bool NextBool()
		{
			if (0 == _bitMask)
			{
				// Generate 32 more bits.
				uint t = _x ^ (_x << 11);
				_x = _y; _y = _z; _z = _w;
				_bitBuffer = _w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8));

				// Reset the bitMask that tells us which bit to read next.
				_bitMask = 0x80000000;
				return (_bitBuffer & _bitMask) == 0;
			}

			return (_bitBuffer & (_bitMask >>= 1)) == 0;
		}

		// Buffer of random bytes. A single UInt32 is used to buffer 4 bytes.
		// _byteBufferState tracks how bytes remain in the buffer, a value of 
		// zero  indicates that the buffer is empty.
		uint _byteBuffer;
		byte _byteBufferState;

		/// <summary>
		/// Generates a single random byte with range [0,255].
		/// This method's performance is improved by generating 4 bytes in one operation and storing them
		/// ready for future calls.
		/// </summary>
		public byte NextByte()
		{
			if (0 == _byteBufferState)
			{
				// Generate 4 more bytes.
				uint t = _x ^ (_x << 11);
				_x = _y; _y = _z; _z = _w;
				_byteBuffer = _w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8));
				_byteBufferState = 0x4;
				return (byte)_byteBuffer;  // Note. Masking with 0xFF is unnecessary.
			}
			_byteBufferState >>= 1;
			return (byte)(_byteBuffer >>= 8);
		}

		#endregion
	}
}