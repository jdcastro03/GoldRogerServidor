namespace GoldRogerServer.Utils
{
    public class APIResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static APIResponse<T> Fail(string msg)
        {
            //return new APIResponse<T> { Success = false, Message = msg.IndexOf("MSG") == 0 ? msg : "MSG_Ocurrio un error" };
            return new APIResponse<T> { Success = false, Message = msg };
        }
        public static APIResponse<T> Fail(string msg, T data)
        {
            //return new APIResponse<T> { Success = false, Message = msg.IndexOf("MSG") == 0 ? msg : "MSG_Ocurrio un error", Data = data };
            return new APIResponse<T> { Success = false, Message = msg, Data = data };
        }
    }
}
