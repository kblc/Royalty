using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Helpers;

namespace RoyaltyService.Model
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MapperInitializeAttribute : Attribute { }

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

    [DataContract]
    public abstract class BaseExecutionResult<T> : BaseExecutionResult
    {
        public BaseExecutionResult() { }
        public BaseExecutionResult(T value) { Value = value; }
        public BaseExecutionResult(Exception ex) : base(ex) { }

        [DataMember(IsRequired = false)]
        public T Value { get; set; }
    }

    [DataContract]
    public abstract class BaseExecutionResults<T> : BaseExecutionResult
    {
        public BaseExecutionResults() { }
        public BaseExecutionResults(T[] values) { Values = values; }
        public BaseExecutionResults(Exception ex) : base(ex) { }

        [DataMember(IsRequired = false)]
        public T[] Values { get; set; }
    }

    [DataContract]
    public class GuidExecutionResult : BaseExecutionResult<Guid>
    {
        public GuidExecutionResult() { }
        public GuidExecutionResult(Guid value) { Value = value; }
        public GuidExecutionResult(Exception ex) : base(ex) { }
    }

    [DataContract]
    public class GuidExecutionResults : BaseExecutionResults<Guid>
    {
        public GuidExecutionResults() { }
        public GuidExecutionResults(Guid[] values) { Values = values; }
        public GuidExecutionResults(Exception ex) : base(ex) { }
    }

    [DataContract]
    public class LongExecutionResult : BaseExecutionResult<long>
    {
        public LongExecutionResult() { }
        public LongExecutionResult(long value) { Value = value; }
        public LongExecutionResult(Exception ex) : base(ex) { }
    }

    [DataContract]
    public class LongExecutionResults : BaseExecutionResults<long>
    {
        public LongExecutionResults() { }
        public LongExecutionResults(long[] values) { Values = values; }
        public LongExecutionResults(Exception ex) : base(ex) { }
    }
}
