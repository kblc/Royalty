using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Services.File
{
    [DataContract(Name = "FileInfoResult")]
    public class FileInfoExecutionResult : Model.BaseExecutionResult<Model.FileInfo>
    {
        public FileInfoExecutionResult() { }
        public FileInfoExecutionResult(Model.FileInfo value ) :base(value) { }
        public FileInfoExecutionResult(Exception ex) : base(ex) { }
    }

    [DataContract(Name = "FileInfoResults")]
    public class FileInfoExecutionResults : Model.BaseExecutionResults<Model.FileInfo>
    {
        public FileInfoExecutionResults() { }
        public FileInfoExecutionResults(Model.FileInfo[] values) : base(values) { }
        public FileInfoExecutionResults(Exception ex) : base(ex) { }
    }
}
