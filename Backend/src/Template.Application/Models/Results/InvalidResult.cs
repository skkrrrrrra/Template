using Domain.Enums;

namespace Template.Application.Models.Results
{
    public class InvalidResult<T> : Result<T>
    {
        private List<string> _errors = new List<string>();

        public InvalidResult()
        {
        }

        public InvalidResult(string error)
        {
            _errors.Add(error + ";");
        }

        public InvalidResult(List<string> errors)
        {
            _errors.AddRange(errors.Select(error => error + ";").ToList());
        }

        public override ResultType ResultType => ResultType.Invalid;

        public override List<string> Errors =>  _errors;

        public override T Data => default;
    }
}
