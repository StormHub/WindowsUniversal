using System;

namespace PresentationToolkit.Core.Common
{
    /// <summary>
    /// Extensions for math related functions.
    /// </summary>
    public static class MathExtensions
    {
        private const double DoubleEpsilon = 2.2204460492503131e-016; /* smallest such that 1.0+DoubleEpsilon != 1.0 */
        private const float FloatEpsilon = 1.192092896e-07F;

        /// <summary>
        /// AreClose - Returns whether or not two doubles are "close".  That is, whether or
        /// not they are within epsilon of each other.  Note that this epsilon is proportional
        /// to the numbers themselves to that AreClose survives scalar multiplication.
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this
        /// returns false.  This is important enough to repeat:
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
        /// used for optimizations *only*.
        /// </summary>
        /// <param name="value1">The first double to compare. </param>
        /// <param name="value2">The second double to compare.</param>
        /// <returns>bool - the result of the AreClose comparision.</returns>
        public static bool AreClose(double value1, double value2)
        {
            //in case they are Infinities (then epsilon check does not work)
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (value1 == value2)
            {
                return true;
            }

            // NaN, unknown never equals to unknown
            if (double.IsNaN(value1) 
                || double.IsNaN(value2))
            {
                return false;
            }

            // ReSharper restore CompareOfFloatsByEqualityOperator
            // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DoubleEpsilon 
            double eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * DoubleEpsilon;
            double delta = value1 - value2;

            return (-eps < delta) && (eps > delta);
        }

        /// <summary>
        /// AreClose - Returns whether or not two float are "close".  That is, whether or
        /// not they are within epsilon of each other.
        /// </summary>
        /// <param name="value1">The first double to compare.</param>
        /// <param name="value2">The second double to compare.</param>
        /// <returns>bool - the result of the AreClose comparision.</returns>
        public static bool AreClose(float value1, float value2)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            // Infinity equals infinity
            if (value1 == value2)
            {
                return true;
            }

            // NaN unknown never equals to unknown
            if (float.IsNaN(value1) 
                || float.IsNaN(value2))
            {
                return false;
            }

            // This computes (|a-b| / (|a| + |b| + 10.0f)) < FLT_EPSILON
            float eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0f) * FloatEpsilon;
            float delta = value1 - value2;
            return (-eps < delta) && (eps > delta);
        }
    }
}
