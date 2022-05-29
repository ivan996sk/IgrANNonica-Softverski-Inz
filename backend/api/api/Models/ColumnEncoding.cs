namespace api.Models
{
    public class ColumnEncoding
    {
        public ColumnEncoding(string columnName, string encoding)
        {
            this.columnName = columnName;
            this.encoding = encoding;
        }

        public string columnName { get; set; }
        public string encoding { get; set; }
    }
}
