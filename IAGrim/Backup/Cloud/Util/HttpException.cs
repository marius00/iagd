





namespace IAGrim.Backup.Cloud.Util {
    [Serializable]
    internal class HttpException : Exception {
        private readonly int code;
        public HttpException() {
        }

        public HttpException(int code, string message) : base(message) {
            this.code = code;
        }

        public HttpException(string? message, Exception? innerException) : base(message, innerException) {
        }

        public int Code => code;
    }
}