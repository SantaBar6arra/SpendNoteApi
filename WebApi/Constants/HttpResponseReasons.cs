namespace WebApi.Constants
{
    public static class HttpResponseReasons
    {
        public const string UserNotFound = "no such user found";
        public const string CategoryNotFound = "no such category found";
        public const string SomethingWentWrong = "something went wrong";
        public const string ObjToBeDeletedNotFound = "object that should be deleted not found";
        public const string AccessForbidden = "access forbidden";
    }
}
