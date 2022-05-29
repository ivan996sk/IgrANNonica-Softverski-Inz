namespace api.Models
{
    public class ColumnInfo
    {
        public ColumnInfo() { }

        public ColumnInfo(string columnName, bool isNumber, int numNulls, float mean, float min, float max, float median, string[] uniqueValues, int[]uniqueValuesCount, float[] uniqueValuesPercent, float q1, float q3)
        {
            this.columnName = columnName;
            this.isNumber = isNumber;
            this.numNulls = numNulls;
            this.mean = mean;
            this.min = min;
            this.max = max;
            this.median = median;
            this.uniqueValues = uniqueValues;
            this.uniqueValuesPercent = uniqueValuesPercent;
            this.uniqueValuesCount = uniqueValuesCount;
            this.q1 = q1;
            this.q3 = q3;
        }

        public string columnName { get; set; }
        public bool isNumber { get; set; }
        public int numNulls { get; set; }
        public float mean { get; set; }
        public float min { get; set; }
        public float max { get; set; }
        public float median { get; set; }
        public string[] uniqueValues { get; set; }
        public int[] uniqueValuesCount { get; set; }
        public float[] uniqueValuesPercent { get; set; }


        public float q1 { get; set; }
        public float q3 { get; set; }

    }
}
