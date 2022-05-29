namespace api.Models
{
    public class NullValues
    {
        public NullValues(string column, string option, string value)
        {
            this.column = column;
            this.option = option;
            this.value = value;
        }

        public string column { get; set; }
        public string option { get; set; }
        public string value { get; set; }
    }
}
