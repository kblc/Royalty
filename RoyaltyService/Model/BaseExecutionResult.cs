using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Helpers;

namespace RoyaltyService.Model
{
    [DataContract(Name = "Result")]
    public class BaseExecutionResult
    {
        public BaseExecutionResult() { }

        public BaseExecutionResult(Exception ex) { Exception = ex; }

        [DataMember(IsRequired = false)]
        public string Error { get; set; }

        [IgnoreDataMember]
        private Exception exception = null;
        [IgnoreDataMember]
        public Exception Exception { get { return exception; } set { exception = value; Error = value?.GetExceptionText(includeStackTrace: false, clearText: true); } }
    }

    [DataContract(Name = "ResultWithValue")]
    public abstract class BaseExecutionResult<T> : BaseExecutionResult
    {
        public BaseExecutionResult() { }
        public BaseExecutionResult(T value) { Value = value; }
        public BaseExecutionResult(Exception ex) : base(ex) { }

        [DataMember(IsRequired = false)]
        public T Value { get; set; }
    }

    [DataContract(Name = "ResultWithValues")]
    public abstract class BaseExecutionResults<T> : BaseExecutionResult
    {
        public BaseExecutionResults() { }
        public BaseExecutionResults(T[] values) { Values = values; }
        public BaseExecutionResults(Exception ex) : base(ex) { }

        [DataMember(IsRequired = false)]
        public T[] Values { get; set; }
    }
}
