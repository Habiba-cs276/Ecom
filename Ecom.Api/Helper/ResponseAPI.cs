namespace Ecom.Api.Helper
{
    public class ResponseAPI<TData>
    {
        public ResponseAPI(int statusCode, TData? data = default , string? message=null)
        {
            StatusCode = statusCode;
            Message = message?? GetMessageFromStatusCode(statusCode);
            Model = data;   
        }
        private string? GetMessageFromStatusCode(int statusCode)
        {
            return statusCode switch
            {
                200 => "Done",
                400=>"Bad Request",
                401=>"Un Authorived",
                500=>"Server Error",
                _=>null,
            };

        }
        public int StatusCode { get; set; } 
        public string? Message { get; set; }    
        public TData? Model { get; set; }  

    }
}
