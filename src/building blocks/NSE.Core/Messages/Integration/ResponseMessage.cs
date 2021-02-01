using FluentValidation.Results;

namespace NSE.Core.Messages.Integration
{
    public class ResponseMessage : Message
    {
        // Utilizamos o proprio objeto do FluentValidation pois ja possui um metodo de validação e nossa lista de erros
        public ValidationResult ValidationResult { get; set; }

        public ResponseMessage(ValidationResult validationResult)
        {
            ValidationResult = validationResult;
        }
    }
}
