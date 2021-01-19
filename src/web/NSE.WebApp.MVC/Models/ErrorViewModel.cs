using System.Collections.Generic;

namespace NSE.WebApp.MVC.Models
{
    public class ErrorViewModel
    {
        public int ErroCode { get; set; }
        public string Titulo { get; set; }
        public string Mensagem { get; set; }

        // TODO : Remover
        public bool ShowRequestId { get; set; }
        public string RequestId { get; set; }
    }

    public class ResponseResult
    {
        public string Title { get; set; }
        public int Status { get; set; }
        public ResponseErrorMessages Errors { get; set; }
    }

    public class ResponseErrorMessages
    {
        public List<string> Mensagens { get; set; }
    }
}
