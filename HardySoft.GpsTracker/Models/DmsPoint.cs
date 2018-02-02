namespace HardySoft.GpsTracker.Models
{
    using System;

    /// <summary>
    /// A class to represent a location point in degree/minute/second (DMS) structure.
    /// </summary>
    internal class DmsPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DmsPoint"/> class by taking location decimal value.
        /// </summary>
        /// <param name="locationDecimalValue">The location decimal value to be converted to DMS structure.</param>
        /// <param name="type">A value to indicate if the location point value is a latitude or longitude value.</param>
        public DmsPoint(double locationDecimalValue, LocationPointValueType type)
        {
            if (type == LocationPointValueType.Unknown)
            {
                throw new ArgumentException("Unknown type is not supported.");
            }

            this.Degree = this.ExtractDegrees(locationDecimalValue);
            this.Minute = this.ExtractMinutes(locationDecimalValue);
            this.Second = this.ExtractSeconds(locationDecimalValue);

            if (type == LocationPointValueType.Latitude)
            {
                if (locationDecimalValue < 0)
                {
                    this.Direction = Direction.South;
                }
                else
                {
                    this.Direction = Direction.North;
                }
            }
            else if (type == LocationPointValueType.Longitude)
            {
                if (locationDecimalValue < 0)
                {
                    this.Direction = Direction.West;
                }
                else
                {
                    this.Direction = Direction.East;
                }
            }
        }

        /// <summary>
        /// Gets the degree of DMS structure.
        /// </summary>
        public int Degree { get; private set; }

        /// <summary>
        /// Gets the minute of DMS structure.
        /// </summary>
        public int Minute { get; private set; }

        /// <summary>
        /// Gets the second of DMS structure.
        /// </summary>
        public int Second { get; private set; }

        /// <summary>
        /// Gets the direction of the location.
        /// </summary>
        public Direction Direction { get; private set; }

        /// <summary>
        /// Extracts the degree value from decimal value.
        /// </summary>
        /// <param name="value">The decimal value.</param>
        /// <returns>The degree value.</returns>
        private int ExtractDegrees(double value)
        {
            return (int)value;
        }

        /// <summary>
        /// Extracts the minute value from decimal value.
        /// </summary>
        /// <param name="value">The decimal value.</param>
        /// <returns>The minute value.</returns>
        private int ExtractMinutes(double value)
        {
            value = Math.Abs(value);
            return (int)((value - this.ExtractDegrees(value)) * 60);
        }

        private int ExtractSeconds(double value)
        {
            value = Math.Abs(value);
            double minutes = (value - this.ExtractDegrees(value)) * 60;
            return (int)Math.Round((minutes - this.ExtractMinutes(value)) * 60);
        }
    }
}
