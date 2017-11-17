using Apache.Ignite.Core.Cache.Configuration;

namespace IgniteTestCommon
{
    public class Person
    {
        [QuerySqlField(IsIndexed = true)]
        public string Name { get; set; }
        [QuerySqlField(IsIndexed = true)]
        public int SalaryReceived { get; set; }

        public override string ToString()
        {
            return $"Person [Name={Name}, SalaryReceived={SalaryReceived}]";
        }
    }
}
